using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;


namespace SortingGame
{
    class CreditsScreen: MenuScreen
    {
        #region Fields

        //Menu entry to go back to the last screen shown
        MenuEntry backMenuEntry;

        //Font used to write credits on to screen
        SpriteFont creditFont;

        #endregion

        #region Initialize

        public CreditsScreen(): base(String.Empty)
        {
            IsPopup = true;

            backMenuEntry = new MenuEntry("BACK");

            //Add the method defined below to the event handler
            backMenuEntry.Selected += BackMenuEntrySelected;

            //Add to list of menu entries
            MenuEntries.Add(backMenuEntry);
        }

        public override void LoadContent()
        {
            creditFont = Load<SpriteFont>("CreditFont");
        }

        #endregion

        #region Other Methods

        //Method that is invoked as the event handler for the back menu entry
        public void BackMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new OptionsScreen(), null);
        }

        //Override menu entry locations
        protected override void UpdateMenuEntryLocations()
        {
            base.UpdateMenuEntryLocations();

            foreach (MenuEntry menuentry in MenuEntries)
            {
                menuentry.Position = new Vector2(menuentry.Position.X + 10, menuentry.Position.Y + 230.0f);
            }
        }

        //Overrides the back button functionality on the phone
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            //Remove this screen
            ScreenManager.RemoveScreen(this);

            //Return to options screen
            ScreenManager.AddScreen(new OptionsScreen(), null);
        }

        #endregion

        #region Draw

        //Renders the credit text on the screen
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            string creditString = "  DESIGN, CODE, ART: Anthony Ng";

            string musicCreditString = "          MUSIC: MusicByPedro \n" + "   youtube.com/user/MusicByPedro \n" +
                "            Song: Funky Game";

            int creditStringWidth = (int)creditFont.MeasureString(creditString).X / 2;

            int musicCreditStringWidth = (int)creditFont.MeasureString(musicCreditString).X / 2;

            Vector2 creditPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - creditStringWidth,
                ScreenManager.GraphicsDevice.Viewport.Height / 2 - 80);

            Vector2 musicCreditPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - musicCreditStringWidth,
                ScreenManager.GraphicsDevice.Viewport.Height / 2);      

            ScreenManager.SpriteBatch.DrawString(creditFont, creditString, creditPosition + new Vector2(4, 4), Color.Black);
            ScreenManager.SpriteBatch.DrawString(creditFont, creditString, creditPosition, Color.White);

            ScreenManager.SpriteBatch.DrawString(creditFont, musicCreditString, musicCreditPosition + new Vector2(4, 4), Color.Black);
            ScreenManager.SpriteBatch.DrawString(creditFont, musicCreditString, musicCreditPosition, Color.White);

            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
