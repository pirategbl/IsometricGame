using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IsometricGame.Source.AStage;
using IsometricGame.Source.ACharacter;
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
        
        private Vector2 _worldPosition;
        SpriteFont fonte;

        private bool __debugger = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1280, 720);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fonte = Content.Load<SpriteFont>("font");
            stage1 = new Stage(Content, viewportAdapter);
        }
        
        protected override void UnloadContent()
        {

        }
        
        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            const float movementSpeed = 3;

            stage1.mainCharacter.Update(Keyboard.GetState(), stage1.getMapSize());
            stage1.Update(Keyboard.GetState(), Mouse.GetState());
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGreen);
            var transformMatrix = stage1.mainCharacter.getCamera().GetViewMatrix(Vector2.Zero);
            
            stage1.Draw(spriteBatch, GraphicsDevice);
            spriteBatch.End();
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"Arrows: Move Character");
                stringBuilder.AppendLine($"WASD: Move Camera");
                stringBuilder.AppendLine($"Camera Position: [{stage1.getCamera().Pos.X:0}, {stage1.getCamera().Pos.Y:0}]");
                stringBuilder.AppendLine($"Character Position: Move [{stage1.mainCharacter.getPosition().X:0}, {stage1.mainCharacter.getPosition().Y:0}]");
                stringBuilder.AppendLine($"Character Matrix Position: Move [{(int)stage1.mainCharacter.getMatrixPosition().Y}, {(int)stage1.mainCharacter.getMatrixPosition().X}]");

                spriteBatch.Begin(blendState: BlendState.AlphaBlend);
                spriteBatch.DrawString(fonte, stringBuilder.ToString(), new Vector2(475, 5), Color.DarkBlue);
                spriteBatch.End();

            base.Draw(gameTime);
        }
        
    }
    
}
