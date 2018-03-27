using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IsometricGame.Source.ACharacter;
using System.Xml;
using IsometricGame.Source.AIsometricCoord;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.Xna.Framework.Input;
using System.Threading;

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
        XmlNodeList xmlLayers;

        // This list has all textures used by the stage (can be an Array, needs to be tested)
        private Texture2D[] textureList;

        // This list contains all layers as a integer matrix
        private List<Tile[,]> layers;

        public Character mainCharacter;

        private Vector2[,] depthSorting;

        private Camera camera = new Camera();

        private float previousMouseScrollWheelValue;
        private float currentMouseScrollWheelValue;

        public Stage(ContentManager content, BoxingViewportAdapter view)
        {
            camera.Pos = new Vector2(0,0);
            xmlMap.Load("Content/maps/teste.tmx");

            /* Makes a list of every layer in the given stage */
            xmlLayers = xmlMap.GetElementsByTagName("layer");

            /* These instructions will determine the size of the stage matrix */
            mapWidth = int.Parse(xmlMap.DocumentElement.Attributes.GetNamedItem("width").Value);
            mapHeight = int.Parse(xmlMap.DocumentElement.Attributes.GetNamedItem("height").Value);
            /*       */
            layers = new List<Tile[,]>();

            depthSorting = new Vector2[3,3];
            /* Makes an instance of the Texture List */
            textureList = new Texture2D[2];
            /* Sometimes the thread doesn't load everything, since it's all happening so fast */
            //Thread tFillStageMatrix = new Thread(ConvertToInt);
            //tFillStageMatrix.Start();
            LoadTextures(content);
            ChangeStage(1);
            mainCharacter = new Character(content, view);

            mainCharacter.setPosition(IsometricCoord.MapToIso(new Vector2(1, 1))); // X = J Y = I
            
            // The number inside this instruction tell
            // the position inside the matrix
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
            xmlLayers = xmlMap.GetElementsByTagName("layer");

            /* These instructions will determine the size of the stage matrix */
            mapWidth = int.Parse(xmlMap.DocumentElement.Attributes.GetNamedItem("width").Value);
            mapHeight = int.Parse(xmlMap.DocumentElement.Attributes.GetNamedItem("height").Value);
            /*       */
            layers = new List<Tile[,]>();

            for (int i = 0; i < xmlLayers.Count; i++)
            {
                layers.Add(new Tile[mapWidth, mapHeight]);
            }

            SetUpTextures();
            SetUpPosition();
        }

        private void LoadTextures(ContentManager ct)
        {
            textureList[0] = (ct.Load<Texture2D>("tiles/paperRedTile"));
            textureList[1] = (ct.Load<Texture2D>("tiles/brick"));
        }

        /* This method converts the stage matrix inside the layers List to a integer matrix, since it's stored as a string */
        private void SetUpTextures()
        {
            /* Converts the string into the integer matrix proper */

            for(int l = 0; l < layers.Count; l++)
            {
                /* Gets the string which is like " 1,1,1,1,1,..." */
                string FirstLayerArrayWithCommas = xmlLayers[l].InnerText;

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
                        layers[l][j, i] = new Tile(int.Parse(FirstLayerSplittedArray[i + (j * mapHeight)]));
                    }
                }
            }
        }

        public void SetUpPosition()
        {
            Vector2 pos;

            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    pos.X = j;
                    pos.Y = i;
                    pos = IsometricCoord.MapToIso(pos);
                    layers[0][i, j].Position = pos;
                }
            }
                for (int i = 0; i < mapHeight; i++)
                {
                    for (int j = 0; j < mapWidth; j++)
                    {
                        pos.X = j - 1;
                        pos.Y = i - 1;
                        pos = IsometricCoord.MapToIso(pos);
                        layers[1][i, j].Position = pos;
                    }
                }
        }
        /* Load the Textures */
        

        /* It doesn't actually Draw the stage, since PlaceTile is the method that really does it, however it prepares the positioning of each tile */
        public void Draw(SpriteBatch sb, GraphicsDevice device)
        {
            sb.Begin(SpriteSortMode.Immediate,
                        null,
                        null,
                        null,
                        null,
                        null,
                        camera.get_transformation(device /*Send the variable that has your graphic device here*/));

            if (layers.Count > 1)
            {
                Vector2 position;
                int tile;
                // Floor Layer
                for (int i = 0; i < mapHeight; i++)
                {
                    for (int j = 0; j < mapWidth; j++)
                    {
                        PlaceTile(layers[0][i, j].getTexture(), layers[0][i, j].Position, sb);
                    }
                }
            }

            

            PlaceTile(layers[1][(int)depthSorting[0, 0].Y, (int)depthSorting[0, 0].X].getTexture(),
                    layers[1][(int)depthSorting[0, 0].Y, (int)depthSorting[0, 0].X].Position, sb);
            PlaceTile(layers[1][(int)depthSorting[0, 1].Y, (int)depthSorting[0, 1].X].getTexture(),
                    layers[1][(int)depthSorting[0, 1].Y, (int)depthSorting[0, 1].X].Position, sb);           

<<<<<<< HEAD
            PlaceTile(layers[1][(int)depthSorting[0, 2].Y, (int)depthSorting[0, 2].X].getTexture(),
                layers[1][(int)depthSorting[0, 2].Y, (int)depthSorting[0, 2].X].Position, sb);

         
            for (int i = 0; i <= depthSorting[0, 0].Y; i++)
            {
                for (int j = 0; j < mapWidth; j++)
=======
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
>>>>>>> a320c73ca020864e45d7b4643f4c1466c55148a8
                {
                    PlaceTile(layers[1][i, j].getTexture(), layers[1][i, j].Position, sb);
                }
            }

            /*PlaceTile(layers[1][(int)depthSorting[1, 0].Y, (int)depthSorting[1, 0].X].getTexture(),
                   layers[1][(int)depthSorting[1, 0].Y, (int)depthSorting[1, 0].X].Position, sb);*/
            for(int i = 0; i <= depthSorting[1,0].X; i++)
            {
                PlaceTile(layers[1][(int)depthSorting[1,0].Y, i].getTexture(), layers[1][(int)depthSorting[1, 0].Y, i].Position, sb);
            }

            mainCharacter.Draw(sb); //depthSorting CANNOT be based on character positioning

            for (int i = (int)depthSorting[1, 2].X; i < mapWidth; i++)
            {
                PlaceTile(layers[1][(int)depthSorting[1, 0].Y, i].getTexture(), layers[1][(int)depthSorting[1, 0].Y, i].Position, sb);
            }

            PlaceTile(layers[1][(int)depthSorting[2, 0].Y, (int)depthSorting[2, 0].X].getTexture(),
                    layers[1][(int)depthSorting[2, 0].Y, (int)depthSorting[2, 0].X].Position, sb);

            PlaceTile(layers[1][(int)depthSorting[2, 1].Y, (int)depthSorting[2, 1].X].getTexture(),
                    layers[1][(int)depthSorting[2, 1].Y, (int)depthSorting[2, 1].X].Position, sb);

            for (int i = (int)depthSorting[1, 2].X; i < mapWidth; i++)
            {
                PlaceTile(layers[1][(int)depthSorting[1, 2].Y, i].getTexture(), layers[1][(int)depthSorting[1, 2].Y, i].Position, sb);
            }

            PlaceTile(layers[1][(int)depthSorting[2, 2].Y, (int)depthSorting[2, 2].X].getTexture(),
                    layers[1][(int)depthSorting[2, 2].Y, (int)depthSorting[2, 2].X].Position, sb);

            for (int i = (int)depthSorting[2,2].Y; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    PlaceTile(layers[1][i, j].getTexture(), layers[1][i, j].Position, sb);
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

        public Camera getCamera()
        {
            return this.camera;
        }

        public void Update(KeyboardState ks, MouseState ms)
        {
            previousMouseScrollWheelValue = currentMouseScrollWheelValue;
            currentMouseScrollWheelValue = ms.ScrollWheelValue;

            if (ks.IsKeyDown(Keys.D1))
                mainCharacter.setPosition(IsometricCoord.MapToIso(new Vector2(1,1)));
            if (ks.IsKeyDown(Keys.D2))
                mainCharacter.setPosition(IsometricCoord.MapToIso(new Vector2(18, 1)));
            if (ks.IsKeyDown(Keys.D3))
                mainCharacter.setPosition(IsometricCoord.MapToIso(new Vector2(15, 5)));

            if (ks.IsKeyDown(Keys.W))
                camera.Move(new Vector2(0, -2));
            if (ks.IsKeyDown(Keys.S))
                camera.Move(new Vector2(0, 2));
            if (ks.IsKeyDown(Keys.A))
                camera.Move(new Vector2(-2, 0));
            if (ks.IsKeyDown(Keys.D))
                camera.Move(new Vector2(2, 0));
            if (currentMouseScrollWheelValue > previousMouseScrollWheelValue)
                camera.Zoom = camera.Zoom + 0.05f;
            if (currentMouseScrollWheelValue < previousMouseScrollWheelValue)
                camera.Zoom = camera.Zoom - 0.05f;

            if (ks.IsKeyDown(Keys.Up))
            {
                Vector2 positionAux;
                Vector2 matrixAux;
                positionAux = mainCharacter.getPosition() - new Vector2(0, 1);
                matrixAux = IsometricCoord.IsoToMap(positionAux);
                if (matrixAux.Y > 0 && matrixAux.X > 0 && !doesItCollide(matrixAux))
                    mainCharacter.setPosition(new Vector2(mainCharacter.getPosition().X, mainCharacter.getPosition().Y - Character.speed));
            }

            if (ks.IsKeyDown(Keys.Right))
            {
                Vector2 positionAux;
                Vector2 matrixAux;
                positionAux = mainCharacter.getPosition() + new Vector2(1, 0);
                matrixAux = IsometricCoord.IsoToMap(positionAux);
                if (matrixAux.Y > 0 && matrixAux.X < mapWidth - 1 && !doesItCollide(matrixAux))
                    mainCharacter.setPosition(new Vector2(mainCharacter.getPosition().X + Character.speed, mainCharacter.getPosition().Y));
            }

            if (ks.IsKeyDown(Keys.Down))
            {
                Vector2 positionAux;
                Vector2 matrixAux;
                positionAux = mainCharacter.getPosition() + new Vector2(0, 1);
                matrixAux = IsometricCoord.IsoToMap(positionAux);
                if (matrixAux.Y < mapHeight - 1 && matrixAux.X < mapWidth - 1 && !doesItCollide(matrixAux))
                    mainCharacter.setPosition(new Vector2(mainCharacter.getPosition().X, mainCharacter.getPosition().Y + Character.speed));
            }

            if (ks.IsKeyDown(Keys.Left))
            {
                Vector2 positionAux;
                Vector2 matrixAux;
                positionAux = mainCharacter.getPosition() - new Vector2(1, 0);
                matrixAux = IsometricCoord.IsoToMap(positionAux);
                if (matrixAux.X > 0 && matrixAux.Y < mapHeight - 1 && !doesItCollide(matrixAux))
                    mainCharacter.setPosition(new Vector2(mainCharacter.getPosition().X - Character.speed, mainCharacter.getPosition().Y));
            }

            depthSorting[0,0] = new Vector2(mainCharacter.getMatrixPosition().X - 1, mainCharacter.getMatrixPosition().Y - 1);
            depthSorting[0,1] = new Vector2(mainCharacter.getMatrixPosition().X, mainCharacter.getMatrixPosition().Y - 1);
            depthSorting[0,2] = new Vector2(mainCharacter.getMatrixPosition().X + 1, mainCharacter.getMatrixPosition().Y - 1);
            depthSorting[1,0] = new Vector2(mainCharacter.getMatrixPosition().X - 1, mainCharacter.getMatrixPosition().Y);
            depthSorting[1,1] = new Vector2(mainCharacter.getMatrixPosition().X, mainCharacter.getMatrixPosition().Y);
            depthSorting[1,2] = new Vector2(mainCharacter.getMatrixPosition().X + 1, mainCharacter.getMatrixPosition().Y);
            depthSorting[2,0] = new Vector2(mainCharacter.getMatrixPosition().X - 1, mainCharacter.getMatrixPosition().Y + 1);
            depthSorting[2,1] = new Vector2(mainCharacter.getMatrixPosition().X, mainCharacter.getMatrixPosition().Y + 1);
            depthSorting[2,2] = new Vector2(mainCharacter.getMatrixPosition().X + 1, mainCharacter.getMatrixPosition().Y + 1);
        }

        public bool doesItCollide(Vector2 aux)
        {
            if (layers[1][(int)aux.Y, (int)aux.X].getTexture() != 0)
                return true;
            return false;
        }
        

    }
}
