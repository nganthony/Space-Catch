using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;

namespace SortingGame
{
    class BackgroundScreen : GameScreen
    {
        #region Fields

        //Texture for background
        Texture2D backgroundTexture;

        //Name of the background image
        string textureString;

        #endregion

        #region Initialize

        public BackgroundScreen(string textureString)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.textureString = textureString;
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            //It is assumed that all textures will be located in the Texture folder
            string textureFolder = "Textures/";

            backgroundTexture = content.Load<Texture2D>(textureFolder + textureString);

            base.LoadContent();
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, Vector2.Zero, new Color(255, 255, 255, TransitionAlpha));

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
