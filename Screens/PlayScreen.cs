using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Threading;
using Microsoft.Xna.Framework.GamerServices;

namespace SortingGame
{
    public abstract class PlayScreen: GameScreen
    {
        #region Fields

        //Screen dimensions
        public Viewport Viewport
        {
            get { return viewport; }
        }

        Viewport viewport;

        //Is the game over?
        public bool GameOver
        {
            get { return gameOver; }
            set { gameOver = value; }
        }

        bool gameOver = false;

        //List to store the items
        public List<Item> ItemBatch
        {
            get { return itemBatch; }
            set { ItemBatch = value; }
        }

        List<Item> itemBatch = new List<Item>();

        //List to store the cages
        public List<Cage> CageBatch
        {
            get { return cageBatch; }
            set { cageBatch = value; }
        }

        List<Cage> cageBatch = new List<Cage>();

        //List to store the animations to be played
        public List<Animation> EffectList
        {
            get { return effectList; }
            set { effectList = value; }
        }

        List<Animation> effectList = new List<Animation>();

        //Score the player has obtained
        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        int score = 0;

        //The time between each spawn
        public int IntervalSpawnTime
        {
            get { return intervalSpawnTime; }
            set { intervalSpawnTime = value; }
        }

        int intervalSpawnTime = 600;

        //The amount of time that has elapsed since the last spawn
        public int ElapsedSpawnTime
        {
            get { return elapsedSpawnTime; }
            set { elapsedSpawnTime = value; }
        }

        int elapsedSpawnTime = 0;

        //Random number generator
        public Random Random
        {
            get { return random; }
        }

        Random random = new Random();

        //Has the game been paused?
        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }

        bool isPaused;

        string gameOverString = "GAME OVER";

        Texture2D explosionTexture;
        Texture2D implosionTexture;
        Texture2D pauseTexture;

        //Button to pause the game
        public Button PauseButton;

        //Method that is added to event handler to pause button event handler
        public void PauseButtonSelected(object sender, EventArgs e)
        {
            //If the game is paused, unpause game
            if (IsPaused)
                IsPaused = false;
            //If the game is not paused, pause the game
            else
                IsPaused = true;
        }

        //The amount of time that has elapsed since the game has ended.
        //This is to indicate when we should transition off the gameplay screen
        int elapsedGameOverTime;

        //This is used to blink the pause button when it has been pressed
        int elapsedPauseTime = 0;

        //The amount of time that has passed so that the screen can be removed
        const int timeToRemoveScreen = 3000;

        //Sound effect for dropping items in cage
        SoundEffect itemdrop;

        #endregion

        #region Initialize

        public PlayScreen()
        {
            //Define the gestures to be used in this screen
            EnabledGestures = GestureType.FreeDrag | GestureType.DragComplete | GestureType.Tap;

            //The game has just started so the game is not over
            gameOver = false;

            IsPopup = true;

            PauseButton = new Button();
            PauseButton.Selected += PauseButtonSelected;

            //The game has just started so do not pause the game
            isPaused = false;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            //Explosion/Implostion Animations
            explosionTexture = Load<Texture2D>("Textures/explosion");
            implosionTexture = Load<Texture2D>("Textures/implosion");

            //Pause button texture
            pauseTexture = Load<Texture2D>("Textures/pause");
            //Pause button position
            Vector2 pausePosition = new Vector2(25, 25);

            //Intialize button with its texture and position
            PauseButton.Initialize(pauseTexture, pausePosition);

            itemdrop = Load<SoundEffect>("Sounds/itemdrop");
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //Update the items when the game has not been paused
            if(!IsPaused)
            {
                foreach (Item item in itemBatch)
                {
                    //Update each item
                    item.Update(gameTime);

                    //Check if the item has expired. If it has end the game.
                    if (item.IsItemExpired(gameTime))
                    {
                        AddExplosion(item.Position);
                        gameOver = true;
                    }
                }

                //Update animations
                foreach (Animation effect in effectList)
                {
                    effect.Update(gameTime);
                }
            }
            
        }

        #endregion

        #region Methods

        //Exits the play screen and adds highscore screen when the score is in high scores
        protected void ExitPlayScreen(GameTime gameTime)
        {
            itemBatch.Clear();

            //When the game has ended, start the timer. This allows for the bomb animation to occur.
            elapsedGameOverTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedGameOverTime >= timeToRemoveScreen)
            {
                // If is in high score, gets the player's name
                if (HighScoreScreen.IsInHighscores(Score))
                {
                    if (!Guide.IsVisible)
                    {
                        Guide.BeginShowKeyboardInput(PlayerIndex.One,
                            "YOU GOT A HIGH SCORE!", "What is your name (max 8 characters)?", "Player",
                                AfterPlayerEnterName, null);
                    }
                }

                if (elapsedGameOverTime >= timeToRemoveScreen + 200)
                {
                    GameoverScreen gameOverScreen = new GameoverScreen();

                    //Keep track of the final score
                    gameOverScreen.FinalScore = score;

                    //Remove the gameplay screen and add the background and gameover screen

                    ScreenManager.AddScreen(new BackgroundScreen("menuscreen"), null);
                    ScreenManager.AddScreen(gameOverScreen, null);
                    ScreenManager.RemoveScreen(this);
                }
            }
        }

        private void AfterPlayerEnterName(IAsyncResult result)
        {
            // Get the name entered by the user
            string playerName = Guide.EndShowKeyboardInput(result);

            if (!string.IsNullOrEmpty(playerName))
            {
                // Ensure that it is valid
                if (playerName != null && playerName.Length > 8)
                {
                    playerName = playerName.Substring(0, 8);
                }

                // Puts it in high score
                HighScoreScreen.PutHighScore(playerName, Score);
            }
        }

        //Intializes the explosion animation at the specified position
        public void AddExplosion(Vector2 explosionPosition)
        {
            Animation effectAnimation = new Animation();
            effectAnimation.Initialize(explosionTexture, explosionPosition, 12, false, 45, 134, 134, 12.0f);
            effectList.Add(effectAnimation);

            //If the sound option is on, play the explosion sound
            if (OptionsScreen.IsSoundOn())
            {
                AudioManager.PlaySound("Explosion");
            }
        }

        //Initializes the implosion animation at the specified position
        public void AddImplosion(Vector2 implosionPosition)
        {
            Animation effectAnimation = new Animation();
            effectAnimation.Initialize(implosionTexture, implosionPosition, 12, false, 45, 134, 134, 1.0f);
            effectList.Add(effectAnimation);

            if (OptionsScreen.IsSoundOn())
                itemdrop.Play();
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            foreach (Cage cage in cageBatch)
            {
                cage.Draw(spriteBatch);
            }

            foreach (Item item in itemBatch)
            {
                item.Draw(spriteBatch);
            }

            foreach (Animation effect in effectList)
            {
                effect.Draw(spriteBatch);
            }

            DrawPlayerScore();

            DrawGameOverString();

            DrawButtons(gameTime);

            spriteBatch.End();
        }

        #endregion

        #region Draw Helper Methods

        //Draws the current score on the screen
        public void DrawPlayerScore()
        {
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, score.ToString(),
                new Vector2((float)viewport.Width - 100, 10), Color.White);
        }

        //Draws the game over string
        public void DrawGameOverString()
        {
            if (GameOver)
            {
                Vector2 stringDimensions = ScreenManager.Font.MeasureString(gameOverString);

                Vector2 gameOverPosition = new Vector2(400 - stringDimensions.X / 2, 240 - stringDimensions.Y / 2);

                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, gameOverString,
                    gameOverPosition + new Vector2(4, 4), Color.Black);

                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, gameOverString,
                    gameOverPosition, Color.White);
            }
        }

        //Draws all buttons for the screen
        public void DrawButtons(GameTime gameTime)
        {
            PauseButton.Draw(ScreenManager.SpriteBatch);
            BlinkPauseButton(gameTime, 100);
        }

        //Blinks the pause button when the pause button has been pressed
        public void BlinkPauseButton(GameTime gameTime, int interval)
        {
            if (IsPaused)
            {
                elapsedPauseTime += (int)gameTime.ElapsedGameTime.Milliseconds;

                if (elapsedPauseTime > interval * 2)
                {
                    elapsedPauseTime = 0;
                }

                //Alternate between colors when the elapsed time has reached the interval time
                if (elapsedPauseTime < interval)
                {
                    PauseButton.Color = Color.White;
                }
                else if (elapsedPauseTime > interval)
                {
                    PauseButton.Color = Color.Black;
                }
            }
            else
                PauseButton.Color = Color.White;
        }

        #endregion
    }
}
