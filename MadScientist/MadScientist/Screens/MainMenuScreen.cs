using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;

namespace SortingGame
{
    class MainMenuScreen : MenuScreen
    {

        #region Fields

        //Menu entry to play the game
        MenuEntry playMenuEntry = new MenuEntry("PLAY");

        //MenuEntry to view highscores
        MenuEntry highscoresMenuEntry = new MenuEntry("SCORES");

        //Menu entry to view tutorial screen
        MenuEntry tutorialMenuEntry = new MenuEntry("TUTORIAL");

        //Menu entry to view options screen
        MenuEntry optionsMenuEntry = new MenuEntry("OPTIONS");

        #endregion

        #region Initialize

        public MainMenuScreen()
            : base(String.Empty)
        {
            IsPopup = true;

            //Add methods to menu entry event handler
            playMenuEntry.Selected += PlayMenuEntrySelected;
            highscoresMenuEntry.Selected += HighscoresMenuEntrySelected;
            tutorialMenuEntry.Selected += TutorialMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;

            //Add the menu entries to menu entries list
            MenuEntries.Add(playMenuEntry);
            MenuEntries.Add(tutorialMenuEntry);
            MenuEntries.Add(highscoresMenuEntry);         
            MenuEntries.Add(optionsMenuEntry);

        }

        #endregion

        #region Other Methods

        //Method that is added to play menu entry event handler
        public void PlayMenuEntrySelected(object sender, EventArgs e)
        {
            //Switch to game play screen
            ScreenManager.AddScreen(new BackgroundScreen("gameplaybackground"), null);
            ScreenManager.AddScreen(new GameplayScreen(), null);
        }

        //Method that is added to highscores menu entry event handler
        public void HighscoresMenuEntrySelected(object sender, EventArgs e)
        {
            //Switch to highscore screen
            ScreenManager.AddScreen(new BackgroundScreen("scorescreen"), null);
            ScreenManager.AddScreen(new HighScoreScreen(), null);
        }

        //Method that is added to tutorial menu entry event handler
        public void TutorialMenuEntrySelected(object sender, EventArgs e)
        {
            //Switch to tutorial screen
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new TutorialScreen(), null);
        }

        //Method that is added to options menu entry event handler
        public void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            //Switch to options screen
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new OptionsScreen(), null);
        }

        //Override the back button functionality on the phone
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            //Save high scores
            HighScoreScreen.SaveHighscore();

            //Exit game
            ScreenManager.Game.Exit();
        }

        #endregion
    }
}
