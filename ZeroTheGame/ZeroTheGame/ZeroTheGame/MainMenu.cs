using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ZeroTheGame
{
    class MainMenu
    {
        #region fields

        // constants for the drawing fields
        const int BACKGROUND_OFSET_FROM_WINDOW = 20;
        const float BUTTONS_WIDTH_OFSET = 0.5f;
        const float BUTTONS_HEIGHT_OFSET = 0.12f;
        const float BUTTONS_X_OFSET = 0.25f;
        const float BUTTON_RESUME_GAME_Y_OFSET = -0.35f;
        const float BUTTON_SAVE_GAME_Y_OFSET = -0.20f;
        const float BUTTON_LOAD_GAME_Y_OFSET = -0.05f;
        const float BUTTON_OPTIONS_Y_OFSET = 0.10f;
        const float BUTTON_EXIT_GAME_Y_OFSET = 0.25f;
        const float POINTER_ZERO_X_OFSET = 0.5f;
        const float POINTER_ZERO_Y_OFSET = 0.8f;

        // states of the menu
        enum MenuState
        {
            ResumeGame,
            SaveGame,
            LoadGame,
            Options,
            ExitGame
        }
        MenuState menuState;

        // drawing fields
        Rectangle backgroundDrawRectangle;
        Texture2D backgroundTexture;

        Rectangle buttonResumeGameRectangle;
        Texture2D buttonResumeGameTexture;

        Rectangle buttonLoadGameRectangle;
        Texture2D buttonLoadGameTexture;

        Rectangle buttonSaveGameRectangle;
        Texture2D buttonSaveGameTexture;

        Rectangle buttonOptionsRectangle;
        Texture2D buttonOptionsTexture;

        Rectangle buttonExitGameRectangle;
        Texture2D buttonExitGameTexture;

        Rectangle pointerZeroRectangle;
        Texture2D pointerZeroTexture;
        int pointerZeroTextureWidth;
        int pointerZeroTextureHeight;

        // keyboard management
        KeyboardState previousKeys;

        #endregion fields

        #region constructor

        /// <summary>
        /// create the main menu
        /// </summary>
        /// <param name="content">the content manager</param>
        /// <param name="windowWidth">the width of the game window</param>
        /// <param name="windowHeight">the height of the game window</param>
        public MainMenu(ContentManager content, int windowWidth, int windowHeight)
        {
            menuState = MenuState.ResumeGame;

            ////sets the textures for the menu and their initial place on the screen ////
            // load the background texture and set the rectangle according the game window size
            backgroundTexture = content.Load<Texture2D>("images//MenuBackgroundRectangle");
            backgroundDrawRectangle = new Rectangle
                (windowWidth / BACKGROUND_OFSET_FROM_WINDOW,
                windowHeight / BACKGROUND_OFSET_FROM_WINDOW,
                windowWidth - 2 * windowWidth / BACKGROUND_OFSET_FROM_WINDOW,
                windowHeight - 2 * windowHeight / BACKGROUND_OFSET_FROM_WINDOW);

            // load the resume game button texture and set the rectangle according the background
            buttonResumeGameTexture = content.Load<Texture2D>("images//MenuButtonResumeGame");
            buttonResumeGameRectangle = new Rectangle
                (backgroundDrawRectangle.Center.X - (int)(backgroundDrawRectangle.Width * BUTTONS_X_OFSET),
                backgroundDrawRectangle.Center.Y + (int)(backgroundDrawRectangle.Height * BUTTON_RESUME_GAME_Y_OFSET),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_OFSET),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_OFSET));

            // load the save game button texture and set the rectangle according the background
            buttonSaveGameTexture = content.Load<Texture2D>("images//MenuButtonSaveGame");
            buttonSaveGameRectangle = new Rectangle
                (backgroundDrawRectangle.Center.X - (int)(backgroundDrawRectangle.Width * BUTTONS_X_OFSET),
                backgroundDrawRectangle.Center.Y + (int)(backgroundDrawRectangle.Height * BUTTON_SAVE_GAME_Y_OFSET),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_OFSET),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_OFSET));

            // load the load game button texture and set the rectangle according the background
            buttonLoadGameTexture = content.Load<Texture2D>("images//MenuButtonLoadGame");
            buttonLoadGameRectangle = new Rectangle
                (backgroundDrawRectangle.Center.X - (int)(backgroundDrawRectangle.Width * BUTTONS_X_OFSET),
                backgroundDrawRectangle.Center.Y + (int)(backgroundDrawRectangle.Height * BUTTON_LOAD_GAME_Y_OFSET),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_OFSET),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_OFSET));

            // load the options button texture and set the rectangle according the background
            buttonOptionsTexture = content.Load<Texture2D>("images//MenuButtonOptions");
            buttonOptionsRectangle = new Rectangle
                (backgroundDrawRectangle.Center.X - (int)(backgroundDrawRectangle.Width * BUTTONS_X_OFSET),
                backgroundDrawRectangle.Center.Y + (int)(backgroundDrawRectangle.Height * BUTTON_OPTIONS_Y_OFSET),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_OFSET),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_OFSET));

            // load the exit game button texture and set the rectangle according the background
            buttonExitGameTexture = content.Load<Texture2D>("images//MenuButtonExitGame");
            buttonExitGameRectangle = new Rectangle
                (backgroundDrawRectangle.Center.X - (int)(backgroundDrawRectangle.Width * BUTTONS_X_OFSET),
                backgroundDrawRectangle.Center.Y + (int)(backgroundDrawRectangle.Height * BUTTON_EXIT_GAME_Y_OFSET),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_OFSET),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_OFSET));

            // load the pointer texture and set the rectangle according the resume game button position
            pointerZeroTexture = content.Load<Texture2D>("images//ZeroSingleImage");
            pointerZeroTextureWidth = backgroundDrawRectangle.Width / 10;
            pointerZeroTextureHeight = backgroundDrawRectangle.Height / 10;
            pointerZeroRectangle = new Rectangle
                (buttonResumeGameRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                buttonResumeGameRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                pointerZeroTextureWidth,
                pointerZeroTextureHeight);
        }

        #endregion constructor

        #region public methods

        /// <summary>
        /// update the main menu
        /// </summary>
        /// <param name="keys">the keyboard pressed keys</param>
        /// <param name="runningGameState">pass a reference of the RunningGameState variable</param>
        /// <param name="mode">pass a reference of the mode variable</param>
        /// <param name="RunningMenuStates">pass a reference of the rummingMenuState</param>
        public void Update(
            KeyboardState keys, 
            ref RunningGameStates runningGameState, 
            ref Mode mode, 
            ref RunningMenuStates runningMenuState,
            SaveGameMenu saveGameMenu,
            LoadGameMenu loadGameMenu)
        {
            // check what is the current state of the menu
            switch (menuState)
            {
                case MenuState.ResumeGame:
                    // set the pointer of Zero on the left side of the button for start game
                    pointerZeroRectangle = new Rectangle
                        (buttonResumeGameRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                        buttonResumeGameRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if enter key previously was pressed and now is released
                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        mode = Mode.GameMode;
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuState.SaveGame;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuState.ExitGame;
                    }
                    
                    break;

                case MenuState.SaveGame:

                    pointerZeroRectangle = new Rectangle
                        (buttonSaveGameRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                        buttonSaveGameRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        saveGameMenu.ReadSlots();
                        saveGameMenu.MenuState = SaveGameMenu.MenuStates.SelectSlot;
                        saveGameMenu.SlotPosition = 0;
                        runningMenuState = RunningMenuStates.SaveGameMenu;
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuState.LoadGame;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuState.ResumeGame;
                    }

                    break;

                case MenuState.LoadGame:

                    pointerZeroRectangle = new Rectangle
                        (buttonLoadGameRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                        buttonLoadGameRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        loadGameMenu.ReadSlots();
                        loadGameMenu.MenuState = LoadGameMenu.MenuStates.SelectSlot;
                        loadGameMenu.SlotPosition = 0;
                        runningMenuState = RunningMenuStates.LoadGameMenu;
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuState.Options;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuState.SaveGame;
                    }

                    break;

                case MenuState.Options:

                    pointerZeroRectangle = new Rectangle
                        (buttonOptionsRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                        buttonOptionsRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        runningMenuState = RunningMenuStates.OptionsMenu;
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuState.ExitGame;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuState.LoadGame;
                    }

                    break;

                case MenuState.ExitGame:

                    pointerZeroRectangle = new Rectangle
                        (buttonExitGameRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                        buttonExitGameRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        Game1.self.Exit();
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuState.ResumeGame;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuState.Options;
                    }

                    break;
            }

            if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
            {
                mode = Mode.GameMode;
            }
            previousKeys = keys;
        }

        /// <summary>
        /// draw the menu
        /// </summary>
        /// <param name="spriteBatch">the sprite batch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundTexture, backgroundDrawRectangle, Color.White);
            spriteBatch.Draw(buttonResumeGameTexture, buttonResumeGameRectangle, Color.White);
            spriteBatch.Draw(buttonSaveGameTexture, buttonSaveGameRectangle, Color.White);
            spriteBatch.Draw(buttonLoadGameTexture, buttonLoadGameRectangle, Color.White);
            spriteBatch.Draw(buttonOptionsTexture, buttonOptionsRectangle, Color.White);
            spriteBatch.Draw(buttonExitGameTexture, buttonExitGameRectangle, Color.White);
            spriteBatch.Draw(pointerZeroTexture, pointerZeroRectangle, Color.White);
        }

        #endregion public methods
    }
}
