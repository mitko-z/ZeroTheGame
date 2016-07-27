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
    class StartMenu
    {
        #region fields

        // constants for the drawing fields
        const int BACKGROUND_OFSET_FROM_WINDOW = 20;
        const float BUTTONS_WIDTH_OFSET = 0.5f;
        const float BUTTONS_HEIGHT_OFSET = 0.12f;
        const float BUTTONS_X_OFSET = 0.25f;
        const float BUTTON_START_GAME_Y_OFSET = -0.35f;
        const float BUTTON_LOAD_GAME_Y_OFSET = -0.15f;
        const float BUTTON_OPTIONS_Y_OFSET = 0.05f;
        const float BUTTON_EXIT_GAME_Y_OFSET = 0.25f;
        const float POINTER_ZERO_X_OFSET = 0.5f;
        const float POINTER_ZERO_Y_OFSET = 0.8f;

        // states of the menu
        enum MenuState
        {
            StartGame,
            LoadGame,
            Options,
            ExitGame
        }
        MenuState menuState;

        // drawing fields
        Rectangle backgroundDrawRectangle;
        Texture2D backgroundTexture;

        Rectangle buttonStartGameRectangle;
        Texture2D buttonStartGameTexture;

        Rectangle buttonLoadGameRectangle;
        Texture2D buttonLoadGameTexture;

        Rectangle buttonOptionsRectangle;
        Texture2D buttonOptionsTexture;

        Rectangle buttonExitGameRectangle;
        Texture2D buttonExitGameTexture;

        Rectangle pointerZeroRectangle;
        Texture2D pointerZeroTexture;
        int pointerZeroTextureWidth;
        int pointerZeroTextureHeight;

        // time management
        int elapsedTimeInMiliseconds;

        // keyboard management
        KeyboardState previousKeys;

        #endregion fields

        #region constructor

        /// <summary>
        /// create the start menu
        /// </summary>
        /// <param name="content">the content manager</param>
        /// <param name="windowWidth">the width of the game window</param>
        /// <param name="windowHeight">the height of the game window</param>
        public StartMenu(ContentManager content, int windowWidth, int windowHeight)
        {
            menuState = MenuState.StartGame;

            elapsedTimeInMiliseconds = 0;

            previousKeys = Keyboard.GetState();

            //sets the textures for the menu and their initial place on the screen
            backgroundTexture = content.Load<Texture2D>("images//MenuBackgroundRectangle");
            backgroundDrawRectangle = new Rectangle
                (windowWidth / BACKGROUND_OFSET_FROM_WINDOW,
                windowHeight / BACKGROUND_OFSET_FROM_WINDOW,
                windowWidth - 2 * windowWidth / BACKGROUND_OFSET_FROM_WINDOW,
                windowHeight - 2 * windowHeight / BACKGROUND_OFSET_FROM_WINDOW);

            buttonStartGameTexture = content.Load<Texture2D>("images//MenuButtonStartGame");
            buttonStartGameRectangle = new Rectangle
                (backgroundDrawRectangle.Center.X - (int)(backgroundDrawRectangle.Width * BUTTONS_X_OFSET),
                backgroundDrawRectangle.Center.Y + (int)(backgroundDrawRectangle.Height * BUTTON_START_GAME_Y_OFSET),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_OFSET),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_OFSET));

            buttonLoadGameTexture = content.Load<Texture2D>("images//MenuButtonLoadGame");
            buttonLoadGameRectangle = new Rectangle
                (backgroundDrawRectangle.Center.X - (int)(backgroundDrawRectangle.Width * BUTTONS_X_OFSET),
                backgroundDrawRectangle.Center.Y + (int)(backgroundDrawRectangle.Height * BUTTON_LOAD_GAME_Y_OFSET),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_OFSET),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_OFSET));

            buttonOptionsTexture = content.Load<Texture2D>("images//MenuButtonOptions");
            buttonOptionsRectangle = new Rectangle
                (backgroundDrawRectangle.Center.X - (int)(backgroundDrawRectangle.Width * BUTTONS_X_OFSET),
                backgroundDrawRectangle.Center.Y + (int)(backgroundDrawRectangle.Height * BUTTON_OPTIONS_Y_OFSET),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_OFSET),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_OFSET));

            buttonExitGameTexture = content.Load<Texture2D>("images//MenuButtonExitGame");
            buttonExitGameRectangle = new Rectangle
                (backgroundDrawRectangle.Center.X - (int)(backgroundDrawRectangle.Width * BUTTONS_X_OFSET),
                backgroundDrawRectangle.Center.Y + (int)(backgroundDrawRectangle.Height * BUTTON_EXIT_GAME_Y_OFSET),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_OFSET),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_OFSET));

            pointerZeroTexture = content.Load<Texture2D>("images//ZeroSingleImage");
            pointerZeroTextureWidth = backgroundDrawRectangle.Width / 10;
            pointerZeroTextureHeight = backgroundDrawRectangle.Height / 10;
            pointerZeroRectangle = new Rectangle
                (buttonStartGameRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                buttonStartGameRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                pointerZeroTextureWidth,
                pointerZeroTextureHeight);
        }

        #endregion constructor

        #region public methods

        /// <summary>
        /// update the start menu
        /// </summary>
        /// <param name="gameTime">the game time</param>
        /// <param name="keys">the keyboard pressed keys</param>
        /// <param name="runningGameState">pass a reference of the RunningGameState variable</param>
        /// <param name="mode">pass a reference of the mode variable</param>
        public void Update(GameTime gameTime, KeyboardState keys, ref RunningGameStates runningGameState, ref Mode mode)
        {
            elapsedTimeInMiliseconds += gameTime.ElapsedGameTime.Milliseconds;

            // check what is the current state of the menu
            switch (menuState)
            {
                case MenuState.StartGame:
                    // set the pointer of Zero on the left side of the button for start game
                    pointerZeroRectangle = new Rectangle
                        (buttonStartGameRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                        buttonStartGameRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if the time of 1 sec has passed from the first time the update method was called
                    // this is set in order to avoiding of previous pressing of keys
                    if (elapsedTimeInMiliseconds > 1000)
                    {
                        elapsedTimeInMiliseconds = 1001; // in order the constant addition of miliseconds to not overload the variable
                        // if enter key previously was pressed and now is released
                        if (previousKeys.IsKeyDown(Keys.Enter)&& keys.IsKeyUp(Keys.Enter))
                        {
                            mode = Mode.GameMode;
                            runningGameState = RunningGameStates.Playing;
                        }
                    }
                    
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuState.LoadGame;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuState.ExitGame;
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
                        // TODO: call the LoadGameMenu object when is ready
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuState.Options;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuState.StartGame;
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
                        // TODO: call the OptionsMenu object when is ready
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
                        menuState = MenuState.StartGame;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuState.Options;
                    }

                    break;
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
            spriteBatch.Draw(buttonStartGameTexture, buttonStartGameRectangle, Color.White);
            spriteBatch.Draw(buttonLoadGameTexture, buttonLoadGameRectangle, Color.White);
            spriteBatch.Draw(buttonOptionsTexture, buttonOptionsRectangle, Color.White);
            spriteBatch.Draw(buttonExitGameTexture, buttonExitGameRectangle, Color.White);
            spriteBatch.Draw(pointerZeroTexture, pointerZeroRectangle, Color.White);
        }

        #endregion public methods
    }
}
