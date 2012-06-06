using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace SortingGame
{
    public class Cage: DrawableGameComponent
    {
        #region Fields

        //The current game that the cage is initialized with
        Game curGame;

        //Texture of cage
        Texture2D cageTexture;

        //Position of cage
        Vector2 cagePosition;

        //Rectangle bound the cage (for collision detection)
        Rectangle cageRectangle;

        #endregion

        #region Properties

        public Texture2D Texture
        {
            get { return cageTexture; }
            set { cageTexture = value; }
        }

        //Width of the cage
        public int Width
        {
            get { return cageTexture.Width; }
        }

        //Height of the cage
        public int Height
        {
            get { return cageTexture.Height; }
        }

        public Vector2 Position
        {
            get { return cagePosition; }
            set { cagePosition = value; }
        }

        public Rectangle CageRectangle
        {
            get
            {
                cageRectangle = new Rectangle((int)cagePosition.X, 
                    (int)cagePosition.Y,
                    cageTexture.Width, 
                    cageTexture.Height); 

                return cageRectangle;
            }
        }

        #endregion

        #region Initialize

        public Cage(Game game, string textureString): base(game)
        {
            curGame = (SortingGame)game;

            //Initialize the cage texture
            cageTexture = Game.Content.Load<Texture2D>(textureString);
        }

        public void Initialize(Vector2 gameCagePosition)
        {
            //Initialize the position of the cage
            cagePosition = gameCagePosition;
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            //Render the cage onto the screen
            spriteBatch.Draw(cageTexture, cagePosition, null, Color.White, 0f,
               Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        #endregion
    }
}
