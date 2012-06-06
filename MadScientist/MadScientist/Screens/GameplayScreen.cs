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
    class GameplayScreen : PlayScreen
    {
        #region Fields

        //The state holding the number of items to spawn at once
        public enum SpawnState
        {
            SpawnOne,
            SpawnTwo,
            SpawnThree
        }

        //The current spawn state
        SpawnState currentSpawnState;

        //The item that is selected by touch gestures
        Item selectedItem;

        //The previous item that is selected by touch gestures
        Item prevSelectedItem;

        //Cages to put the items in
        Cage cage1;
        Cage cage2;

        //Bottom screen location
        Vector2 bottomScreen;

        //Top screen location
        Vector2 topScreen;

        //Velocity of an item
        const int positionX = 3;
        const int positionY = 4;

        //Used to alternate spawn locations
        bool spawningBottom = true;

        #endregion

        #region Initialize

        public override void LoadContent()
        {
            base.LoadContent();
            LoadAssets();
        }

        public void LoadAssets()
        {
            cage1 = new Cage(ScreenManager.Game, "Textures/planetcage1");
            cage2 = new Cage(ScreenManager.Game, "Textures/planetcage2");

            Vector2 cage1Position = new Vector2(0, Viewport.Height / 2 - cage1.Texture.Height / 2);
            Vector2 cage2Position = new Vector2(Viewport.Width - cage2.Texture.Width,
                Viewport.Height / 2 - cage1.Texture.Height / 2);

            bottomScreen = new Vector2(Viewport.Width / 2, Viewport.Height);
            topScreen = new Vector2(Viewport.Width / 2, 0);

            cage1.Initialize(cage1Position);
            cage2.Initialize(cage2Position);

            CageBatch.Add(cage1);
            CageBatch.Add(cage2);
        }

        #endregion

        #region Update Method

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //Only update if the game hasn't ended
            if (!GameOver)
            {
                if (!IsPaused)
                {
                    //Update helper methods
                    UpdateSpawnState();
                    UpdateSpawn(gameTime);
                    UpdateCollision();
                    UpdateItemPosition();
                    UpdateItemLocation();
                }
            }

            else
            {
                //The game has ended, exit game play screen
                ExitPlayScreen(gameTime);
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        #endregion

        #region Update Helper Methods

        //Chooses a random item, initializes it, and adds it to the item list
        public void AddRandomItem(Item.ItemState addItemState, Vector2 addItemLocation)
        {
            //Create new item and an item animation
            Item gameItem = new Item(ScreenManager.Game);
            Animation itemAnimation = new Animation();

            //Folder where textures are located in the project
            string textureFolder = "Textures/";

            //Name of the texture image
            string addItemString = "";

            int randChoice = Random.Next(1, 10);

            //Logic to choose a random item to spawn
            if (randChoice <= 5)
            {
                addItemString = textureFolder + "rocketship";
                gameItem.GameItemNumber = Item.ItemNumber.ItemOne;
            }

            else
            {
                addItemString = textureFolder + "spaceship";
                gameItem.GameItemNumber = Item.ItemNumber.ItemTwo;
            }

            gameItem.GameItemState = addItemState;

            ItemBatch.Add(gameItem);

            //Initialize the game item
            gameItem.Initialize(addItemString, addItemLocation);
        }

        //Updates the spawn according to which spawn state the game is currently in
        public void UpdateSpawn(GameTime gameTime)
        {
            ElapsedSpawnTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            //Spawn the items at a specific time interval
            if (ElapsedSpawnTime > IntervalSpawnTime)
            {
                ElapsedSpawnTime = 0;

                switch (currentSpawnState)
                {
                    //Spawns a random item one at a time
                    case SpawnState.SpawnOne:

                        //Chooses a random direction to move the item in when it is first initialized
                        int state1Choice = Random.Next(1, 4);
                        int state2Choice = Random.Next(4, 7);

                        //Alternating sequence of spawn location
                        if (!spawningBottom)
                        {
                            AddRandomItem((Item.ItemState)state1Choice, topScreen);
                            spawningBottom = true;
                        }
                        else
                        {         
                            AddRandomItem((Item.ItemState)state2Choice, bottomScreen);
                            spawningBottom = false;
                        }

                        break;

                    //Spawns two random items at once
                    case SpawnState.SpawnTwo:
                        if (spawningBottom)
                        {
                            AddRandomItem(Item.ItemState.GoRightUp, bottomScreen);
                            AddRandomItem(Item.ItemState.GoLeftUp, bottomScreen);
                            spawningBottom = false;
                        }
                        else if (!spawningBottom)
                        {
                            AddRandomItem(Item.ItemState.GoRightDown, topScreen);
                            AddRandomItem(Item.ItemState.GoLeftDown, topScreen);
                            spawningBottom = true;
                        }
                        break;

                    //Spawns three random items at once
                    case SpawnState.SpawnThree:
                        if (spawningBottom)
                        {
                            AddRandomItem(Item.ItemState.GoUp, bottomScreen);
                            AddRandomItem(Item.ItemState.GoRightUp, bottomScreen);
                            AddRandomItem(Item.ItemState.GoLeftUp, bottomScreen);
                            spawningBottom = false;
                        }
                        else if (!spawningBottom)
                        {
                            AddRandomItem(Item.ItemState.GoDown, topScreen);
                            AddRandomItem(Item.ItemState.GoRightDown, topScreen);
                            AddRandomItem(Item.ItemState.GoLeftDown, topScreen);
                            spawningBottom = true;
                        }
                        break;    
                }
            }
        }

        //Update spawn state as time progresses to make the game more difficult
        public void UpdateSpawnState()
        {
            if (Score <= 10)
                currentSpawnState = SpawnState.SpawnOne;

            else if (Score > 10 && Score <= 40)
            {
                IntervalSpawnTime = 500;
            }

            else if (Score > 40 && Score <= 60)
            {
                IntervalSpawnTime = 400;
            }

            else if (Score > 60 && Score <= 80)
            {
                currentSpawnState = SpawnState.SpawnTwo;
                IntervalSpawnTime = 1500;
            }

            else if (Score > 80  && Score <= 100)
            {
                IntervalSpawnTime = 1000;
            }

            else if (Score > 100 && Score <= 140)
            {
                currentSpawnState = SpawnState.SpawnThree;
                IntervalSpawnTime = 1800;
            }

            else if (Score > 140 && Score <= 180)
            {
                IntervalSpawnTime = 1500;
            }

            else if (Score > 180 && Score <= 230)
            {
                IntervalSpawnTime = 1200;
            }

            else if(Score > 230 && Score <= 280)
            {
                IntervalSpawnTime = 1000;
            }

            else if (Score > 280 && Score <= 320)
            {
                currentSpawnState = SpawnState.SpawnOne;
                IntervalSpawnTime = 390;
                }

            else if (Score > 320)
            {
                currentSpawnState = SpawnState.SpawnThree;
                IntervalSpawnTime = 1000;
            }

            else if (Score > 500)
            {
                IntervalSpawnTime = 900;
            }
            
        }

        //Updates state of items with collisions
        public void UpdateItemPosition()
        {
            foreach (Item item in ItemBatch)
            {
                //Only want to update the position of an item when an item is not touched by a gesture
                if (!item.IsDragged)
                {
                    switch (item.GameItemState)
                    {
                        case Item.ItemState.GoDown:
                            item.Position.Y += positionY;
                            break;

                        case Item.ItemState.GoLeftDown:
                            item.Position.Y += positionY;
                            item.Position.X -= positionX;
                            break;

                        case Item.ItemState.GoRightDown:
                            item.Position.Y += positionY;
                            item.Position.X += positionX;
                            break;

                        case Item.ItemState.GoUp:
                            item.Position.Y -= positionY;
                            break;

                        case Item.ItemState.GoRightUp:
                            item.Position.Y -= positionY;
                            item.Position.X += positionX;
                            break;

                        case Item.ItemState.GoLeftUp:
                            item.Position.Y -= positionY;
                            item.Position.X -= positionX;
                            break;
                    }
                }

                //The position of the item can not go beyond the width of the screen
                item.Position.X = MathHelper.Clamp(item.Position.X, item.Width / 2,
                    Viewport.Width - item.Width / 2);
            }
        }
        
        //Input the collision event and update the item accordingly
        public void UpdateCollision()
        {
            //Rectangle representing cage collision box
            Rectangle cage1Rect = cage1.CageRectangle;
            Rectangle cage2Rect = cage2.CageRectangle;

            foreach (Item item in ItemBatch)
            {
                Rectangle itemRect = item.ItemRectangle;

                //Collides with cage 1
                if (itemRect.Intersects(cage1Rect))
                {
                    //These update sequences are only used when the item is not trapped
                    //in any of the corners
                    if (!item.TrappedInCorner)
                    {
                        if (item.GameItemState == Item.ItemState.GoLeftUp)
                        {
                            item.GameItemState = Item.ItemState.GoRightUp;
                        }

                        else if (item.GameItemState == Item.ItemState.GoLeftDown)
                        {
                            item.GameItemState = Item.ItemState.GoRightDown;
                        }
                    }
                    
                    //If the item is trapped in a corner, move the item up and down
                    else
                    {
                        if (item.GameLocationState == Item.LocationState.TopLeftCorner)
                        {
                            item.GameItemState = Item.ItemState.GoUp;
                        }
                        else if (item.GameLocationState == Item.LocationState.BottomLeftCorner)
                        {
                            item.GameItemState = Item.ItemState.GoDown;
                        }
                    }
                }
                
                //Collides with cage 2
                else if (itemRect.Intersects(cage2Rect))
                {
                    if (!item.TrappedInCorner)
                    {
                        if (item.GameItemState == Item.ItemState.GoRightDown)
                        {
                            item.GameItemState = Item.ItemState.GoLeftDown;
                        }
                        else if (item.GameItemState == Item.ItemState.GoRightUp)
                        {
                            item.GameItemState = Item.ItemState.GoLeftUp;
                        }
                    }

                    else
                    {
                        if (item.GameLocationState == Item.LocationState.TopRightCorner)
                        {
                            item.GameItemState = Item.ItemState.GoUp;
                        }
                        else if (item.GameLocationState == Item.LocationState.BottomRightCorner)
                        {
                            item.GameItemState = Item.ItemState.GoDown;
                        }
                    }
                }
                
                //Collides with top screen
                else if(item.Position.Y < item.Height / 2)
                {
                    if (!item.TrappedInCorner)
                    {
                        if (item.GameItemState == Item.ItemState.GoRightUp)
                        {
                            item.GameItemState = Item.ItemState.GoRightDown;
                        }
                        else if (item.GameItemState == Item.ItemState.GoLeftUp ||
                            item.GameItemState == Item.ItemState.GoUp)
                        {
                            item.GameItemState = Item.ItemState.GoLeftDown;
                        }
                    }

                    else
                    {
                        if (item.GameLocationState == Item.LocationState.TopLeftCorner ||
                            item.GameLocationState == Item.LocationState.TopRightCorner)
                        {
                            item.GameItemState = Item.ItemState.GoDown;
                        }
                    }
                }

                //Collides with bottom screen
                else if (item.Position.Y > Viewport.Height - item.Height / 2)
                {
                    if (!item.TrappedInCorner)
                    {
                        if (item.GameItemState == Item.ItemState.GoRightDown ||
                            item.GameItemState == Item.ItemState.GoDown)
                        {
                            item.GameItemState = Item.ItemState.GoRightUp;
                        }
                        else if (item.GameItemState == Item.ItemState.GoLeftDown)
                        {
                            item.GameItemState = Item.ItemState.GoLeftUp;
                        }
                    }

                    else
                    {
                        if (item.GameLocationState == Item.LocationState.BottomLeftCorner ||
                                item.GameLocationState == Item.LocationState.BottomRightCorner)
                        {
                            item.GameItemState = Item.ItemState.GoUp;
                        }
                    }
                }
            }
        }

        //Updates the location of the item
        public void UpdateItemLocation()
        {
            //The rectangle above cage 1
            Rectangle cage1Top = new Rectangle(0, 0, cage1.Width, (Viewport.Height - cage1.Height) / 2);

            //The rectangle below cage 1
            Rectangle cage1Bottom = new Rectangle(0, (int)cage1.Position.Y + cage1.Height,
                cage1.Width, (Viewport.Height - cage1.Height) / 2);

            //The rectangle above cage 2
            Rectangle cage2Top = new Rectangle((int)cage2.Position.X, 0, cage2.Width, (Viewport.Height - cage2.Height) / 2);

            //The rectangle below cage 2
            Rectangle cage2Bottom = new Rectangle((int)cage2.Position.X, (int)cage2.Position.Y + cage2.Height,
                cage2.Width, (Viewport.Height - cage2.Height) / 2);

            foreach (Item item in ItemBatch)
            {
                //Point where the item is located
                Point itemPoint = new Point((int)item.Position.X, (int)item.Position.Y);

                //The item is in the top cage1 location
                if (cage1Top.Contains(itemPoint))
                {
                    item.TrappedInCorner = true;
                    item.GameLocationState = Item.LocationState.TopLeftCorner;
                }
                
                //The item is in the top cage2 location
                else if (cage2Top.Contains(itemPoint))
                {
                    item.TrappedInCorner = true;
                    item.GameLocationState = Item.LocationState.TopRightCorner;
                }
                
                //The item is in the bottom cage1 location
                else if (cage1Bottom.Contains(itemPoint))
                {
                    item.TrappedInCorner = true;
                    item.GameLocationState = Item.LocationState.BottomLeftCorner;
                }

                //The item is in the bottom cage2 location
                else if (cage2Bottom.Contains(itemPoint))
                {
                    item.TrappedInCorner = true;
                    item.GameLocationState = Item.LocationState.BottomRightCorner;
                }

                //The item is in mid aisle location
                else
                {
                    item.TrappedInCorner = false;
                }
            }
        }

        //Ends the game and adds explosion when player has put item in wrong cage
        public void ItemExplode(Item explodeItem)
        {
            AddExplosion(explodeItem.Position);
            ItemBatch.Remove(explodeItem);
            GameOver = true;
        }

        //Adds point to player score and adds implosion animation
        public void ItemImplode(Item implodeItem)
        {
            AddImplosion(implodeItem.Position);
            ItemBatch.Remove(implodeItem);
            Score++;
        }

        #endregion

        #region Handle Input

        //Handles touch input on the game play screen
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.IsPauseGame(null))
            {
                ScreenManager.AddMenuScreen(this);
            }

            //Only handle input when the game is not over
            if (!GameOver)
            {
                // see if we have a new primary point down. when the first touch
                // goes down, we do hit detection to try and select one of our sprites.
                if (input.TouchState.Count > 0 && input.TouchState[0].State == TouchLocationState.Pressed)
                {
                    // convert the touch position into a Point for hit testing
                    Point touchPoint = new Point((int)input.TouchState[0].Position.X, (int)input.TouchState[0].Position.Y);

                    //If the pause button has been pressed, invoke the pause button event handler
                    if (PauseButton.ButtonRect.Contains(touchPoint))
                    {
                        PauseButton.OnSelectEntry();
                    }

                    // iterate our sprites to find which sprite is being touched. we iterate backwards
                    // since that will cause sprites that are drawn on top to be selected before
                    // sprites drawn on the bottom.
                    selectedItem = null;

                    for (int i = ItemBatch.Count - 1; i >= 0; i--)
                    {
                        Item currentItem = ItemBatch[i];
                        if (currentItem.ItemRectangle.Contains(touchPoint))
                        {
                            selectedItem = currentItem;
                            break;
                        }
                    }

                    if (selectedItem != null)
                    {
                        selectedItem.IsDragged = true;

                        // we also move the sprite to the end of the list so it
                        // draws on top of the other sprites
                        ItemBatch.Remove(selectedItem);
                        ItemBatch.Add(selectedItem);
                    }
                }

                foreach (GestureSample gestureSample in input.Gestures)
                {
                    switch (gestureSample.GestureType)
                    {
                        case GestureType.FreeDrag:
                            if (selectedItem != null)
                            {
                                if (!IsPaused)
                                {
                                    //Move the position of the item along with the gesture
                                    selectedItem.Position += gestureSample.Delta;
                                }
                            }
                            break;

                        case GestureType.DragComplete:
                            if (prevSelectedItem != null)
                            {
                                Point selectedItemPoint = new Point((int)selectedItem.Position.X, (int)selectedItem.Position.Y);
                                prevSelectedItem.IsDragged = false;

                                //The item lands in cage1
                                if(cage1.CageRectangle.Contains(selectedItemPoint))
                                {

                                    //The item was placed in the correct cage
                                    if (prevSelectedItem.GameItemNumber == Item.ItemNumber.ItemOne)
                                    {
                                        ItemImplode(prevSelectedItem);
                                    }

                                    //The item was not placed in the correct cage
                                    else
                                    {
                                        ItemExplode(prevSelectedItem);
                                    }
                                }

                                //The item lands in cage2
                                else if(cage2.CageRectangle.Contains(selectedItemPoint))
                                {
                                    //The item was placed in the correct cage
                                    if (prevSelectedItem.GameItemNumber == Item.ItemNumber.ItemTwo)
                                    {
                                        ItemImplode(prevSelectedItem); 
                                    }

                                    //The item was not placed in the correct cage
                                    else
                                    {
                                        ItemExplode(prevSelectedItem);
                                    }
                                }
                            }

                            break;

                        case GestureType.Tap:
                            //Even though GestureType.Tap is not a gesture used in this game,
                            //for some reason if it is not used, the selected items will lose its state
                            //and become frozen. Will fix this in future.
                            if (selectedItem != null)
                            {
                                selectedItem.IsDragged = false;
                            }

                            break;
                    }

                    //Keep a copy of the previous item to be used for
                    //acting on the drag complete state
                    prevSelectedItem = selectedItem;
                }
            }
            
        }

        #endregion 
    }
}
