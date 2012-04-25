using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;

namespace SortingGame
{
    class GameoverScreen: MenuScreen
    {

        #region Fields

        //The final score that the player has received for a game
        public int FinalScore
        {
            get;
            set;
        }

        #endregion

        #region Initialize

        public GameoverScreen()
            : base(String.Empty)
        {
            IsPopup = true;

            //A menu entry to switch back to gameplay screen
            MenuEntry playAgainMenuEntry = new MenuEntry("REPLAY");

            //A menu entry to swtich back to main menu
            MenuEntry backToMainMenu = new MenuEntry("MAIN MENU");

            playAgainMenuEntry.Selected += PlayAgainSelected;
            backToMainMenu.Selected += BackToMainMenuSelected;

            MenuEntries.Add(playAgainMenuEntry);
            MenuEntries.Add(backToMainMenu);
        }

        #endregion

        #region Other Methods

        //Method that is added to the replay event handler
        public void PlayAgainSelected(object sender, EventArgs e)
        {
            //Switch to game play screen and removes this screen
            ScreenManager.AddScreen(new BackgroundScreen("gameplaybackground"), null);
            ScreenManager.AddScreen(new GameplayScreen(), null);
            ScreenManager.RemoveScreen(this);

        }

        //Method that is added to the main menu event handler
        public void BackToMainMenuSelected(object sender, EventArgs e)
        {
            //Add the main menu screen
            ScreenManager.AddMenuScreen(this);
        }

        //Overrides the default menu entry locations
        protected override void UpdateMenuEntryLocations()
        {
            base.UpdateMenuEntryLocations();

            foreach (MenuEntry menuentry in MenuEntries)
            {
                menuentry.Position = new Vector2(menuentry.Position.X, menuentry.Position.Y + 120.0f);
            }
        }

        //Overrides the back button functionality on the phone
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            //Return to main menu screen
            ScreenManager.AddMenuScreen(this);
        }

        #endregion

        #region Draw

        //Renders the game's score and the all time high score
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            spriteBatch.Begin();
            
            string scoreText = "SCORE: " + FinalScore;

            string hiscoreText = "HIGH SCORE: " + HighScoreScreen.highScore[0].Value;

            float textWidth = ScreenManager.Font.MeasureString(scoreText).X;

            float hiscoreTextWidth = ScreenManager.Font.MeasureString(hiscoreText).X;
            
            Vector2 textPosition = new Vector2(viewport.Width / 2 - textWidth / 2, viewport.Height / 2 - 70);

            Vector2 hiscoreTextPosition = new Vector2(viewport.Width / 2 - hiscoreTextWidth / 2, viewport.Height / 2);

            spriteBatch.DrawString(ScreenManager.Font, scoreText, textPosition + new Vector2(4, 4), Color.Red);

            spriteBatch.DrawString(ScreenManager.Font, scoreText, textPosition, Color.White);

            spriteBatch.DrawString(ScreenManager.Font, hiscoreText, hiscoreTextPosition + new Vector2(4, 4), Color.Red);

            spriteBatch.DrawString(ScreenManager.Font, hiscoreText, hiscoreTextPosition, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion
    }
}
