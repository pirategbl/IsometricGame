using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace IsometricGame.Source.ACharacter
{
    class Character
    {
        private Texture2D texture;
        private Vector2 matrixPosition;
        private Vector2 absolutePosition;
        public const int speed = 1;
        private Camera2D _camera;

        public Character(ContentManager aContent, BoxingViewportAdapter va)
        {
            this.texture = aContent.Load<Texture2D>("character/character");
            this.matrixPosition = new Vector2(3, 9);
            _camera = new Camera2D(va);
        }

        public Texture2D getTexture()
        {
            return this.texture;
        }

        public Vector2 getMatrixPosition()
        {
            return this.matrixPosition;
        }

        public void setMatrixPosition(Vector2 aPos)
        {
            this.matrixPosition = aPos;
        }

        public Vector2 getPosition()
        {
            return this.absolutePosition;
        }

        public void setPosition(Vector2 aPos)
        {
            this.absolutePosition = aPos;
        }

        public Camera2D getCamera()
        {
            return this._camera;
        }

        public void Update(KeyboardState ks, Vector2 mapSize)
        {
            this.matrixPosition = AIsometricCoord.IsometricCoord.IsoToMap(this.absolutePosition);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(this.texture, new Vector2(this.absolutePosition.X, this.absolutePosition.Y - (texture.Height / 1.2f)), 
                null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
        
    }
}
