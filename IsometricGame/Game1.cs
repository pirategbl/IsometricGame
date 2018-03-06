using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IsometricGameStage;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Text;

namespace IsometricGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        // Variable used for a given stage
        private Stage stage1;
        //Camera for testing
        private Camera2D _camera;
        private Vector2 _worldPosition;
        SpriteFont fonte;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            stage1 = new Stage(Content);
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fonte = Content.Load<SpriteFont>("font");
        }
        
        protected override void UnloadContent()
        {

        }
        
        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            const float movementSpeed = 5;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                _camera.Move(new Vector2(movementSpeed, movementSpeed) * deltaTime);
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                _camera.Move(new Vector2(movementSpeed, -movementSpeed) * deltaTime);
            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                _camera.Move(new Vector2(-movementSpeed, -movementSpeed) * deltaTime);
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                _camera.Move(new Vector2(-movementSpeed, movementSpeed) * deltaTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _worldPosition = _camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y));
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            var transformMatrix = _camera.GetViewMatrix(Vector2.Zero);

            spriteBatch.Begin(transformMatrix: transformMatrix);
            stage1.Draw(spriteBatch, _camera.Position);
            spriteBatch.End();



            var rectangle = _camera.BoundingRectangle;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"WASD: Move [{_camera.Position.X:0}, {_camera.Position.Y:0}]");
            stringBuilder.AppendLine($"Bounds: [{rectangle.X:0}, {rectangle.Y:0}, {rectangle.Width:0}, {rectangle.Height:0}]");

            spriteBatch.Begin(blendState: BlendState.AlphaBlend);
            spriteBatch.DrawString(fonte, stringBuilder.ToString(), new Vector2(5, 5), Color.DarkBlue);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
