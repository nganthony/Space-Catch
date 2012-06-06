using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace SortingGame
{
    public class Item: DrawableGameComponent
    {
        #region States

        //Various states for an item
        public enum ItemState
        {
            GoDown = 1,
            GoLeftDown,
            GoRightDown,
            GoUp,
            GoLeftUp,
            GoRightUp,
            InCage
        }

        //Location state of an item
        public enum LocationState
        {
            TopLeftCorner,
            TopRightCorner,
            BottomLeftCorner,
            BottomRightCorner
        }

        //Specifying which item has been spawned
        public enum ItemNumber
        {
            ItemOne,
            ItemTwo,
            ItemThree,
            ItemFour
        }

        #endregion

        #region Fields

        Game curGame;

        //Current state of an item
        ItemState gameItemState;

        //Current location state
        public LocationState gameLocationState;

        //Position of the item
        public Vector2 Position;

        //Texture of the item
        Texture2D itemTexture;

        //Animation to be played when item is initialized
        Animation itemAnimation;

        //Rectangle bounding the item (used for collision detection)
        Rectangle itemRectangle;

        //The amount of time since the item has been initialized
        int burnTime = 0;

        //The amount of time since the last flash (when the item is about to explode)
        int signalTime = 0;

        //The amount of time until the item will explode
        const int gameOverBurnTime = 10000;

        #endregion

        #region Properties

        public ItemState GameItemState
        {
            get { return gameItemState; }
            set { gameItemState = value; }
        }

        public LocationState GameLocationState
        {
            get { return gameLocationState; }
            set { gameLocationState = value; }
        }

        public ItemNumber GameItemNumber
        {
            get;
            set;
        }

        //Texture height of the item
        public int Height
        {
            get { return itemAnimation.FrameHeight; }
        }

        //Texture width of the item
        public int Width
        {
            get { return itemAnimation.FrameWidth; }
        }

        //Rectangle bounding the item (for collision and touch detection)
        public Rectangle ItemRectangle
        {
            get
            {
                //Since the item is drawn at the origin, alter the rectangle so that it starts
                //from the top corner of the texture
                itemRectangle = new Rectangle((int)(itemAnimation.spritePosition.X - itemAnimation.FrameWidth / 2 * itemAnimation.scale),
                    (int)(itemAnimation.spritePosition.Y - itemAnimation.FrameHeight / 2 * itemAnimation.scale),
                    (int)(itemAnimation.FrameWidth * itemAnimation.scale), (int)(itemAnimation.FrameHeight * itemAnimation.scale));

                //Gives some padding to the rectangle
                itemRectangle.Inflate(15, 15);

                return itemRectangle;
            }
        }

        //Has the item been trapped in the four corners?
        public bool TrappedInCorner
        {
            get;
            set;
        }

        //Is the item being dragged by a gesture?
        public bool IsDragged
        {
            get;
            set;
        }

        #endregion

        #region Intialize

        public Item(Game game): base(game)
        {
            curGame = (SortingGame)game;
        }

        public void Initialize(string textureString, Vector2 itemPosition)
        {
            //Initialize starting position
            Position = itemPosition;

            //Initialize item's texture
            itemTexture = Game.Content.Load<Texture2D>(textureString);

            //Animation to be played for texture. This is a 1 frame "animation". Just in case we want an animation in the future.
            itemAnimation = new Animation();
            itemAnimation.Initialize(itemTexture, Position, 1, true, 200, 270, 160, 0.50f);

            //Initialize all position state flags to be false
            TrappedInCorner = false;
            IsDragged = false;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            //Update item's animation
            itemAnimation.spritePosition = Position;
            itemAnimation.Update(gameTime);

            //Indicate the flash time interval and when flash of item should begin
            FlashItem(gameTime, 60, 6000);
        }

        #endregion

        #region Other Methods

        //Blinks the item to let the player know that the item is about to explode
        public void FlashItem(GameTime gameTime, int interval, int timeToBlink)
        {
            //Start to count the signal time when the amount of time that the 
            //item has been initialized reaches the time to blink the item
            if (burnTime >= timeToBlink)
            {
                signalTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            //Reset the signal time after it has reached interval time
            //for "blinking" effect
            if (signalTime >= interval*2)
            {
                signalTime = 0;
            }

            //Flash the item white and red
            itemAnimation.Color = signalTime <= interval/2 ? Color.White : Color.Tomato;   
        }

        public bool IsItemExpired(GameTime gameTime)
        {
            //Calculate how much seconds has elapsed since the item was spawned
            burnTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            //Item has passed the expire time
            if (burnTime >= gameOverBurnTime)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the item animation
            itemAnimation.Draw(spriteBatch);
        }

        #endregion
    }
}
