using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using System.Threading;
using System.Xml;

namespace IsometricGameStage
{
    class Stage
    {
        // Stores the Width of the map
        private int mapWidth;

        // Stores the Height of the map
        private int mapHeight;

        // This is the matrix for the first layer of the stage
        private int[,] stageFirstLayer;

        // This is the matrix for the second layer of the stage (layers go bottom-up)
        private int[,] stageSecondLayer;

        // This XML is the stage itself, with all informations: layers, size, etc
        XmlDocument xmlMap = new XmlDocument();

        // This list contains all extracted layers from the XML
        XmlNodeList layers;

        // This list has all textures used by the stage (can be an Array, needs to be tested)
        private List<Texture2D> textureList;

        // These constants represent the size of each tile. Every tile must have these dimension.
        private const int TileWidth = 64;
        private const int TileHeight = 32;
        //

        public Stage(ContentManager content)
        {
            /* Loads the XML Stage */
            xmlMap.Load("Content/maps/sketch.tmx");

            /* Makes a list of every layer in the given stage */
            layers = xmlMap.GetElementsByTagName("layer");

            /* These instructions will determine the size of the stage matrix */
            mapWidth = int.Parse(xmlMap.DocumentElement.Attributes.GetNamedItem("width").Value);
            mapHeight = int.Parse(xmlMap.DocumentElement.Attributes.GetNamedItem("height").Value);
            /*                                                                */

            /* These two instructions make the layers matrices */
            stageFirstLayer = new int[mapHeight, mapWidth];
            stageSecondLayer = new int[mapHeight, mapWidth];
            /*                                                  */

            /* Makes an instance of the Texture List */
            textureList = new List<Texture2D>();

            /* Sometimes the thread doesn't load everything, since it's all happening so fast */
            //Thread tFillStageMatrix = new Thread(ConvertToInt);
            //Thread tLoadTextures = new Thread(() => LoadTextures(content)); //Lambda Expression
            //tLoadTextures.Start();
            //tFillStageMatrix.Start();
            ConvertToInt();
            LoadTextures(content);
        }

        /* This method converts the stage matrix inside the layers List to a integer matrix, since it's stored as a string */
        private void ConvertToInt()
        {
            /* Gets the string which is like " 1,1,1,1,1,..." */
            string FirstLayerArrayWithCommas = layers[0].InnerText;

            /* Removes the commas so the string becomes only numbers and "\n" */
            string[] FirstLayerSplittedArray = FirstLayerArrayWithCommas.Split(',');

            /* Same as the above */
            string SecondLayerArrayWithCommas = layers[1].InnerText;
            string[] SecondLayerSplittedArray = SecondLayerArrayWithCommas.Split(',');
            /*                   */

            /* Converts the string into the integer matrix proper */
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
                    stageFirstLayer[j, i] = int.Parse(FirstLayerSplittedArray[i + (j * mapHeight)]);
                    stageSecondLayer[j, i] = int.Parse(SecondLayerSplittedArray[i + (j * mapHeight)]);
                }
            }
        }

        /* Load the Textures */
        private void LoadTextures(ContentManager ct)
        {
            textureList.Add(ct.Load<Texture2D>("tiles/brick"));
            textureList.Add(ct.Load<Texture2D>("tiles/pencil"));
            textureList.Add(ct.Load<Texture2D>("tiles/brick"));
            textureList.Add(ct.Load<Texture2D>("tiles/brick2"));
        }

        /* It doesn't actually Draw the stage, since PlaceTile is the method that really does it, however it prepares the positioning of each tile */
        public void Draw(SpriteBatch sb, Vector2 aCameraPosition)
        {
            /* First Layer */
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    Vector2 position;
                    int tile;

                    /* Gets the tile based on the */
                    tile = stageFirstLayer[i, j];
                    position.X = j + aCameraPosition.X;
                    position.Y = i + aCameraPosition.Y;
                    position = twoDToIso(position);
                    PlaceTile(tile, position, sb);
                }
            }

            /* Second Layer */
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    Vector2 position;
                    int tile;
                    tile = stageSecondLayer[i, j];
                    position.X = j + aCameraPosition.X - 1;
                    position.Y = i + aCameraPosition.Y - 1;
                    position = twoDToIso(position);
                    PlaceTile(tile, position, sb);
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
                    aSb.Draw(textureList[1], aPos, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                    break;
                case 2:
                    aSb.Draw(textureList[2], aPos, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                    break;
                case 3:
                    aSb.Draw(textureList[3], aPos, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                    break;
            }
        }

        public Vector2 twoDToIso(Vector2 aPosition)
        {
            float x = (aPosition.X - aPosition.Y) * (TileWidth / 2);
            float y = (aPosition.X + aPosition.Y) * (TileHeight / 2);
            return new Vector2(x, y);
        }

        public Vector2 IsoTo2D(Vector2 aPosition)
        {
            float x = ((aPosition.X / (TileWidth / 2)) + (aPosition.Y / (TileHeight / 2))) / 2;
            float y = ((aPosition.Y / (TileHeight / 2)) - (aPosition.X / (TileWidth / 2))) / 2;
            return new Vector2(x, y);
        }
    }
}
