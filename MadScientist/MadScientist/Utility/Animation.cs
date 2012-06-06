using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace SortingGame
{
    public class Animation
    {
        #region Fields

        //Sprite sheet for animation
        Texture2D spriteSheet;

        //Location of sprite
        public Vector2 spritePosition;

        //Source rectangle to display the frame
        Rectangle frameRectangle;

        //Number of frames the animation has
        int numberOfFrames;

        //The frame that the animation is currently on
        int frameIndex;

        //The width of the frame
        public int FrameWidth;

        //The height of the frame
        public int FrameHeight;

        //Should we keep on looping the animation?
        bool IsRepeat;

        //Is the animation currently playing?
        bool Active;

        //The amount of time to switch to the next frame
        int frameTime;

        //The elapsed time since the last frame was displayed
        int elapsedTime;

        //Used to adjust the size of the sprite
        public float scale;

        Color color = Color.White;

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        #endregion

        #region Intialize

        public void Initialize(Texture2D spriteSheet, Vector2 spritePosition, int numberOfFrames, bool IsRepeat, 
            int frameTime, int frameWidth, int frameHeight, float scale)
        {
            this.spriteSheet = spriteSheet;
            this.spritePosition = spritePosition;
            this.numberOfFrames = numberOfFrames;
            this.IsRepeat = IsRepeat;
            this.frameTime = frameTime;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.scale = scale;

            Active = true;
            elapsedTime = 0;
            frameIndex = 0;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            //If the animation is not active, do not update
            if (Active == false)
                return;

            //The amount of time since the last frame was displayed
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime >= frameTime)
            {
                //Switch frames if the time since last frame is more than frame time
                frameIndex++;

                //Reset the time since last frame to zero since we just switched frames
                elapsedTime = 0;

                //If the current frame is equal to the total number of frames
                //decide whether we should stop the animation or continue again
                //from the first frame
                if (frameIndex == numberOfFrames)
                {
                    if (IsRepeat)
                    {
                        frameIndex = 0;
                    }

                    else
                    {
                        Active = false;
                    }
                }
            }

            //Change the frame by multiplying the frame width/ height by a factor (frame index)
            frameRectangle = new Rectangle(FrameWidth * frameIndex, 0, FrameWidth, FrameHeight);
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw each frame only if the animation is in active state
            if (Active)
            {
                spriteBatch.Draw(spriteSheet, spritePosition, frameRectangle, color, 0.0f,
                    new Vector2(FrameWidth / 2, FrameHeight / 2), scale, SpriteEffects.None, 0.0f);
            }
        }

        #endregion
    }
}
