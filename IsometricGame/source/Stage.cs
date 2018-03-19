using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IsometricGame.Source.ACharacter;
using System.Xml;
using IsometricGame.Source.AIsometricCoord;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.Xna.Framework.Input;

namespace IsometricGame.Source.AStage
{
    class Stage
    {
        // Stores the Width of the map
        private int mapWidth;

        // Stores the Height of the map
        private int mapHeight;

        // This XML is the stage itself, with all informations: layers, size, etc
        XmlDocument xmlMap = new XmlDocument();

        // This list contains all extracted layers from the XML
        XmlNodeList layers;

        // This list has all textures used by the stage (can be an Array, needs to be tested)
        private List<Texture2D> textureList;

        // This list contains all layers as a integer matrix
        private List<int[,]> allLayers;

        public Character mainCharacter;

        public Stage(ContentManager content, BoxingViewportAdapter view)
        {
            
            xmlMap.Load("Content/maps/teste.tmx");

            /* Makes a list of every layer in the given stage */
            layers = xmlMap.GetElementsByTagName("layer");

            /* These instructions will determine the size of the stage matrix */
            mapWidth = int.Parse(xmlMap.DocumentElement.Attributes.GetNamedItem("width").Value);
            mapHeight = int.Parse(xmlMap.DocumentElement.Attributes.GetNamedItem("height").Value);
            /*        
             *        */
            allLayers = new List<int[,]>();
            /* Makes an instance of the Texture List */
            textureList = new List<Texture2D>();
            /* Sometimes the thread doesn't load everything, since it's all happening so fast */
            //Thread tFillStageMatrix = new Thread(ConvertToInt);
            //Thread tLoadTextures = new Thread(() => LoadTextures(content)); //Lambda Expression
            //tLoadTextures.Start();
            //tFillStageMatrix.Start();
            LoadTextures(content);
            ChangeStage(1);
            mainCharacter = new Character(content, view);

            mainCharacter.setPosition(IsometricCoord.MapToIso(new Vector2(14, 2))); // X = J Y = I
            // The number inside this instruction tell
            // the position inside the matrix

            /*System.Console.WriteLine(mainCharacter.getMatrixPosition());
            System.Console.WriteLine(mainCharacter.getPosition());
            System.Console.WriteLine(AIsometricCoord.IsometricCoord.CoordToMatrix(mainCharacter.getPosition(), mapWidth, mapHeight));*/
            System.Console.WriteLine();
        }

        public void ChangeStage(int aWhichStage)
        {
            /* Loads the XML Stage */
            switch (aWhichStage)
            {
                case 1:
                    xmlMap.Load("Content/maps/testing.tmx");
                    break;
            }

            /* Makes a list of every layer in the given stage */
            layers = xmlMap.GetElementsByTagName("layer");

            /* These instructions will determine the size of the stage matrix */
            mapWidth = int.Parse(xmlMap.DocumentElement.Attributes.GetNamedItem("width").Value);
            mapHeight = int.Parse(xmlMap.DocumentElement.Attributes.GetNamedItem("height").Value);
            /*        
             *        */
            allLayers = new List<int[,]>();

            for (int i = 0; i < layers.Count; i++)
            {
                allLayers.Add(new int[mapWidth, mapHeight]);
            }

            ConvertToInt();
        }

        /* This method converts the stage matrix inside the layers List to a integer matrix, since it's stored as a string */
        private void ConvertToInt()
        {
            /* Converts the string into the integer matrix proper */

            for(int l = 0; l < allLayers.Count; l++)
            {
                /* Gets the string which is like " 1,1,1,1,1,..." */
                string FirstLayerArrayWithCommas = layers[l].InnerText;

                /* Removes the commas so the string becomes only numbers and "\n" */
                string[] FirstLayerSplittedArray = FirstLayerArrayWithCommas.Split(',');

                for (int i = 0; i < mapHeight; i++)
                {
                    for (int j = 0; j < mapWidth; j++)
                    {
                        /* Since "SplittedArray" is a one dimensional array, the tiles are organized like this:
                            Row-Column-Column-Column(...)-Row(...)
                            So to fill the stage matrix correctly, I'm using the math below:
                            The first column is filled ENTIRELY (all of the first column's rows), then the second, then the third, and so on...
                            It's very important to note, however, that in order for this algorithm to work, the map must have the exact same
                            size of Height and Width */
                        allLayers[l][j,i] = int.Parse(FirstLayerSplittedArray[i + (j * mapHeight)]);
                    }
                }
            }
        }

        /* Load the Textures */
        private void LoadTextures(ContentManager ct)
        {
            textureList.Add(ct.Load<Texture2D>("tiles/paperRedTile"));
            textureList.Add(ct.Load<Texture2D>("tiles/brick"));
        }
               
        /* It doesn't actually Draw the stage, since PlaceTile is the method that really does it, however it prepares the positioning of each tile */
        public void Draw(SpriteBatch sb)
        {            
            if(allLayers.Count > 1)
            {
                Vector2 position;
                int tile;
                // Floor Layer
                for (int i = 0; i < mapHeight; i++)
                {
                    for (int j = 0; j < mapWidth; j++)
                    {
                        

                        /* Gets the tile based on the */
                        tile = allLayers[0][i, j];
                        position.X = j;
                        position.Y = i;
                        position = IsometricCoord.MapToIso(position);
                        PlaceTile(tile, position, sb);
                        
                    }
                }

                // Draws the tiles that should be "behind" the character
                tile = allLayers[1][(int)mainCharacter.getMatrixPosition().Y - 1, (int)mainCharacter.getMatrixPosition().X - 1];
                position.X = (int)mainCharacter.getMatrixPosition().X - 1 - 1;
                position.Y = (int)mainCharacter.getMatrixPosition().Y - 1 - 1; // -1 From the Layer offset, + 1 from the character offset
                position = IsometricCoord.MapToIso(position);
                PlaceTile(tile, position, sb);

                tile = allLayers[1][(int)mainCharacter.getMatrixPosition().Y - 1, (int)mainCharacter.getMatrixPosition().X];
                position.X = (int)mainCharacter.getMatrixPosition().X - 1;
                position.Y = (int)mainCharacter.getMatrixPosition().Y - 1 - 1;
                position = IsometricCoord.MapToIso(position);
                PlaceTile(tile, position, sb);

                tile = allLayers[1][(int)mainCharacter.getMatrixPosition().Y, (int)mainCharacter.getMatrixPosition().X - 1];
                position.X = (int)mainCharacter.getMatrixPosition().X - 1 - 1;
                position.Y = (int)mainCharacter.getMatrixPosition().Y - 1;
                position = IsometricCoord.MapToIso(position);
                PlaceTile(tile, position, sb);

                /*tile = allLayers[1][(int)mainCharacter.getMatrixPosition().Y - 1, (int)mainCharacter.getMatrixPosition().X + 1];
                position.X = (int)mainCharacter.getMatrixPosition().X + 1 - 1;
                position.Y = (int)mainCharacter.getMatrixPosition().Y - 1 - 1;
                position = IsometricCoord.MapToIso(position);
                PlaceTile(tile, position, sb);*/

                tile = allLayers[1][(int)mainCharacter.getMatrixPosition().Y + 1, (int)mainCharacter.getMatrixPosition().X - 1];
                position.X = (int)mainCharacter.getMatrixPosition().X - 1 - 1;
                position.Y = (int)mainCharacter.getMatrixPosition().Y + 1 - 1;
                position = IsometricCoord.MapToIso(position);
                PlaceTile(tile, position, sb);
                //

                mainCharacter.Draw(sb);

                // Draws the tiles that "hide" the character
                
                tile = allLayers[1][(int)mainCharacter.getMatrixPosition().Y - 1, (int)mainCharacter.getMatrixPosition().X + 1];
                position.X = (int)mainCharacter.getMatrixPosition().X + 1 - 1;
                position.Y = (int)mainCharacter.getMatrixPosition().Y - 1 - 1;
                position = IsometricCoord.MapToIso(position);
                PlaceTile(tile, position, sb);
                
                tile = allLayers[1][(int)mainCharacter.getMatrixPosition().Y + 1, (int)mainCharacter.getMatrixPosition().X];
                position.X = (int)mainCharacter.getMatrixPosition().X - 1;
                position.Y = (int)mainCharacter.getMatrixPosition().Y - 1 + 1; // -1 From the Layer offset, + 1 from the character offset
                position = IsometricCoord.MapToIso(position);
                PlaceTile(tile, position, sb);

                tile = allLayers[1][(int)mainCharacter.getMatrixPosition().Y, (int)mainCharacter.getMatrixPosition().X + 1];
                position.X = (int)mainCharacter.getMatrixPosition().X + 1 - 1;
                position.Y = (int)mainCharacter.getMatrixPosition().Y - 1;
                position = IsometricCoord.MapToIso(position);
                PlaceTile(tile, position, sb);
                
                tile = allLayers[1][(int)mainCharacter.getMatrixPosition().Y + 1, (int)mainCharacter.getMatrixPosition().X + 1];
                position.X = (int)mainCharacter.getMatrixPosition().X + 1 - 1;
                position.Y = (int)mainCharacter.getMatrixPosition().Y + 1 - 1;
                position = IsometricCoord.MapToIso(position);
                PlaceTile(tile, position, sb);
                //

                if (Keyboard.GetState().IsKeyDown(Keys.F))
                {
                    for (int l = 1; l < allLayers.Count; l++)
                    {
                        for (int i = 0; i < mapHeight; i++)
                        {
                            for (int j = 0; j < mapWidth; j++)
                            {
                                tile = allLayers[l][i, j];
                                position.X = j - 1;
                                position.Y = i - 1;
                                position = IsometricCoord.MapToIso(position);
                                PlaceTile(tile, position, sb);

                            }
                        }
                    }
                }
            }
        }

        public void PlaceTile(int aTileType, Vector2 aPos, SpriteBatch aSb)
        {
            switch (aTileType)
            {
                case 0:
                    break;
                case 1:
                    aSb.Draw(textureList[0], aPos, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                    break;
                case 2:
                    aSb.Draw(textureList[1], aPos, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                    break;
            }
        }

        public Vector2 getMapSize()
        {
            return new Vector2(mapWidth, mapHeight);
        }

        public void Update(KeyboardState ks)
        {
            if (ks.IsKeyDown(Keys.W))
            {
                Vector2 positionAux;
                Vector2 matrixAux;
                positionAux = mainCharacter.getPosition() + new Vector2(0, -Character.speed);
                matrixAux = IsometricCoord.IsoToMap(positionAux);
                if (matrixAux.X >= 0 && matrixAux.X < mapWidth && matrixAux.Y >= 0 && matrixAux.Y < mapHeight && !doesItCollide(matrixAux))
                    mainCharacter.setPosition(new Vector2(mainCharacter.getPosition().X + 0, mainCharacter.getPosition().Y - Character.speed));
            }
            if (ks.IsKeyDown(Keys.S))
            {
                Vector2 positionAux;
                Vector2 matrixAux;
                positionAux = mainCharacter.getPosition() + new Vector2(0, Character.speed);
                matrixAux = IsometricCoord.IsoToMap(positionAux);
                if (matrixAux.X >= 0 && matrixAux.X < mapWidth && matrixAux.Y >= 0 && matrixAux.Y < mapHeight && !doesItCollide(matrixAux))
                    mainCharacter.setPosition(new Vector2(mainCharacter.getPosition().X + 0, mainCharacter.getPosition().Y + Character.speed));
            }
            if (ks.IsKeyDown(Keys.A))
            {
                Vector2 positionAux;
                Vector2 matrixAux;
                positionAux = mainCharacter.getPosition() + new Vector2(-Character.speed, 0);
                matrixAux = IsometricCoord.IsoToMap(positionAux);
                if (matrixAux.X >= 0 && matrixAux.X < mapWidth && matrixAux.Y >= 0 && matrixAux.Y < mapHeight && !doesItCollide(matrixAux))
                    mainCharacter.setPosition(new Vector2(mainCharacter.getPosition().X + -Character.speed, mainCharacter.getPosition().Y + 0));
            }
            if (ks.IsKeyDown(Keys.D))
            {
                Vector2 positionAux;
                Vector2 matrixAux;
                positionAux = mainCharacter.getPosition() + new Vector2(Character.speed, 0);
                matrixAux = IsometricCoord.IsoToMap(positionAux);
                if (matrixAux.X >= 0 && matrixAux.X < mapWidth && matrixAux.Y >= 0 && matrixAux.Y < mapHeight && !doesItCollide(matrixAux))
                    mainCharacter.setPosition(new Vector2(mainCharacter.getPosition().X + Character.speed, mainCharacter.getPosition().Y + 0));
            }
        }

        public bool doesItCollide(Vector2 aux)
        {
            if (allLayers[1][(int)aux.Y, (int)aux.X] != 0)
                return true;
            return false;
        }
    }
}
