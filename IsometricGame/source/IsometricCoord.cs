using Microsoft.Xna.Framework;

namespace IsometricGame.Source.AIsometricCoord
{
    static class IsometricCoord
    {
        // These constants represent the size of each tile. Every tile must have these dimension.
        public const int TileWidth = 64;
        public const int TileHeight = 32;
        //

        public static Vector2 MapToIso(Vector2 aPosition)
        {
            float x = (aPosition.X - aPosition.Y) * (TileWidth / 2);
            float y = (aPosition.X + aPosition.Y) * (TileHeight / 2);
            return new Vector2(x, y);
        }
        public static Vector2 IsoToMap(Vector2 aPosition)
        {
            float x = ((aPosition.X / (TileWidth / 2)) + (aPosition.Y / (TileHeight / 2))) / 2; //Valores provisorios para testes
            float y = ((aPosition.Y / (TileHeight / 2)) - (aPosition.X / (TileWidth / 2))) / 2;
            return new Vector2(x, y);
        }
    }
}