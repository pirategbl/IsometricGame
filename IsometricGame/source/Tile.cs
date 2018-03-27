using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace IsometricGame.Source.AStage
{
    class Tile
    {
        public Vector2 Position { get; set; }
        private int texture; // Being an integer reduces the memory size; the textures will be located at the Stage class
                            // in the switch-case "statement" in the case with the same number.


        public Tile(int whichTile)
        {
            this.texture = whichTile;
        }

        public void setTexture(int whichTile)
        {
            this.texture = whichTile;
        }

        public int getTexture()
        {
            return this.texture;
        }

    }
}
