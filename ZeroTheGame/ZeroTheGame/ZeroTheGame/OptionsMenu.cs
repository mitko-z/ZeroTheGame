using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.GamerServices;

namespace ZeroTheGame
{
    public class OptionsMenu
    {
        #region fields

        // constants for the drawing fields
        const int BACKGROUND_OFSET_FROM_WINDOW = 20;
        const float GRAPHICS_X_OFSETT = 10;
        const float WINDOWED_640X480_Y_OFSETT = 26.4f;
        const float WINDOWED_800X600_Y_OFSETT = 32.5f;
        const float WINDOWED_1024X768_Y_OFSETT = 38.6f;
        const float WINDOWED_1280X800_Y_OFSETT = 44.7f;
        const float FULSCREEN_Y_OFSETT = 50.8f;
        const float KEYS_X_OFSETT = 54;
        const float MOVE_UP_Y_OFSETT = 26.4f;
        const float MOVE_DOWN_Y_OFSETT = 32.5f;
        const float MOVE_LEFT_Y_OFSETT = 38.6f;
        const float MOVE_RIGHT_Y_OFSETT = 44.7f;
        const float TAKE_WEAPON_Y_OFSETT = 50.8f;
        const float LEAVE_WEAPON_Y_OFSETT = 56.9f;
        const float FIRE_Y_OFSETT = 62.7f;
        const float BUTTON_OK_X_OFSET = 75;
        const float BUTTON_OK_Y_OFSET = 85;
        const float BUTTON_OK_WIDTH_PERCENTAGE = 20;
        const float BUTTON_OK_HEIGHT_PERCENTAGE = 10;
        const float POINTER_ZERO_X_OFSET = 0.5f;
        const float POINTER_ZERO_Y_OFSET = 0.8f;
        const string PRESS_A_KEY_STRING = "< press a key >";
        const float KEYS_STRING_X_OFSETT = 80;

        // states of the menu
        public enum MenuStates
        {
            Windowed640x480Setting,
            Windowed800x600Setting,
            Windowed1024x768Setting,
            Windowed1280x800Setting,
            FullScreenSetting,
            MoveUpKeySetting,
            MoveDownKeySetting,
            MoveLeftKeySetting,
            MoveRightKeySetting,
            TakeWeaponKeySetting,
            LeaveWeaponKeySetting,
            FireKeySetting,
            OkButton
        }
        MenuStates menuState;
        MenuStates waitForKeySetting;

        // drawing fields
        Rectangle backgroundDrawRectangle;
        Texture2D backgroundTexture;

        Rectangle buttonOKRectangle;
        Texture2D buttonOKTexture;

        Rectangle pointerZeroRectangle;
        Texture2D pointerZeroTexture;
        int pointerZeroTextureWidth;
        int pointerZeroTextureHeight;

        // text drawing on screen
        SpriteFont font;
        string
            moveUpKeyString,
            moveDownKeyString,
            moveLeftKeyString,
            moveRightKeyString,
            takeWeaponKeyString,
            leaveWeaponKeyString,
            fireKeyString;
        Vector2
            moveUpKeyStringVector,
            moveDownKeyStringVector,
            moveLeftKeyStringVector,
            moveRightKeyStringVector,
            takeWeaponKeyStringVector,
            leaveWeaponKeyStringVector,
            fireKeyStringVector;

        // keyboard management
        KeyboardState previousKeys;

        // new window size management
        int newWindowWidth;
        int newWindowHeight;

        #endregion fields

        #region constructor

        /// <summary>
        /// create the save game menu
        /// </summary>
        /// <param name="content">the content manager</param>
        /// <param name="windowWidth">the width of the game window</param>
        /// <param name="windowHeight">the height of the game window</param>
        public OptionsMenu(ContentManager content, int windowWidth, int windowHeight)
        {
            // load texture font
            font = content.Load<SpriteFont>("Fonts//SpriteFont2");

            menuState = MenuStates.Windowed640x480Setting;
            waitForKeySetting = MenuStates.OkButton;

            //sets the textures for the menu and their initial place on the screen
            backgroundTexture = content.Load<Texture2D>("images//MenuOptionsCompleteView");
            backgroundDrawRectangle = new Rectangle
                (windowWidth / BACKGROUND_OFSET_FROM_WINDOW,
                windowHeight / BACKGROUND_OFSET_FROM_WINDOW,
                windowWidth - 2 * windowWidth / BACKGROUND_OFSET_FROM_WINDOW,
                windowHeight - 2 * windowHeight / BACKGROUND_OFSET_FROM_WINDOW);

            buttonOKTexture = content.Load<Texture2D>("images//MenuButtonOK");
            buttonOKRectangle = new Rectangle
                (backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * BUTTON_OK_X_OFSET / 100),
                backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * BUTTON_OK_Y_OFSET / 100),
                (int)(backgroundDrawRectangle.Width * BUTTON_OK_WIDTH_PERCENTAGE / 100),
                (int)(backgroundDrawRectangle.Height * BUTTON_OK_HEIGHT_PERCENTAGE / 100));

            pointerZeroTexture = content.Load<Texture2D>("images//ZeroSingleImage");
            pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
            pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
            // backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * SLOTS_X_OFSET / 100)
            // 
            pointerZeroRectangle = new Rectangle(
                backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * GRAPHICS_X_OFSETT / 100),
                backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * WINDOWED_640X480_Y_OFSETT / 100),
                pointerZeroTextureWidth,
                pointerZeroTextureHeight);

            // define displaying of keys controlling keys text on screen
            moveUpKeyStringVector = new Vector2
                (backgroundDrawRectangle.X + backgroundDrawRectangle.Width * KEYS_STRING_X_OFSETT / 100, 
                backgroundDrawRectangle.Y + backgroundDrawRectangle.Height * (MOVE_UP_Y_OFSETT + 1) / 100);
            moveDownKeyStringVector = new Vector2(backgroundDrawRectangle.X + backgroundDrawRectangle.Width * KEYS_STRING_X_OFSETT / 100,
                backgroundDrawRectangle.Y + backgroundDrawRectangle.Height * (MOVE_DOWN_Y_OFSETT + 1) / 100); ;
            moveLeftKeyStringVector = new Vector2(backgroundDrawRectangle.X + backgroundDrawRectangle.Width * KEYS_STRING_X_OFSETT / 100,
                backgroundDrawRectangle.Y + backgroundDrawRectangle.Height * (MOVE_LEFT_Y_OFSETT + 1) / 100);
            moveRightKeyStringVector = new Vector2(backgroundDrawRectangle.X + backgroundDrawRectangle.Width * KEYS_STRING_X_OFSETT / 100,
                backgroundDrawRectangle.Y + backgroundDrawRectangle.Height * (MOVE_RIGHT_Y_OFSETT + 1) / 100);
            takeWeaponKeyStringVector = new Vector2(backgroundDrawRectangle.X + backgroundDrawRectangle.Width * KEYS_STRING_X_OFSETT / 100,
                backgroundDrawRectangle.Y + backgroundDrawRectangle.Height * (TAKE_WEAPON_Y_OFSETT + 1) / 100);
            leaveWeaponKeyStringVector = new Vector2(backgroundDrawRectangle.X + backgroundDrawRectangle.Width * KEYS_STRING_X_OFSETT / 100,
                backgroundDrawRectangle.Y + backgroundDrawRectangle.Height * (LEAVE_WEAPON_Y_OFSETT + 1) / 100);
            fireKeyStringVector = new Vector2(backgroundDrawRectangle.X + backgroundDrawRectangle.Width * (KEYS_STRING_X_OFSETT - 10)/ 100,
                backgroundDrawRectangle.Y + backgroundDrawRectangle.Height * (FIRE_Y_OFSETT + 1) / 100);

            // define empty strings for displaying controlling keys on screen
            moveUpKeyString = "";
            moveDownKeyString = "";
            moveLeftKeyString = "";
            moveRightKeyString = "";
            takeWeaponKeyString = "";
            leaveWeaponKeyString = "";
            fireKeyString = "";
        }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Updates the options menu
        /// </summary>
        /// <param name="graphics">the Graphics manager</param>
        /// <param name="keys">the keyboard pressed keys</param>
        /// <param name="windowWidth">pass a reference to the game window width controlling variable</param>
        /// <param name="windowHeight">pass a reference to the game window height controlling variable</param>
        /// <param name="controllingKeysForZero">pass a reference to the list of controlling keys for Zero</param>
        public void Update(
            ref GraphicsDeviceManager graphics, 
            KeyboardState keys, 
            ref int windowWidth, 
            ref int windowHeight, 
            List<Keys> controllingKeysForZero,
            ref Mode mode, 
            ref RunningMenuStates runningMenuState,
            ZeroCharacter zeroCharacter,
            List<Wall> walls,
            List<Creature> creatures,
            List<Weapon> weapons,
            EndOfLevel endOfLevel)
        {
            // updates the text for displayung of the controlling keys
            if(moveUpKeyString != PRESS_A_KEY_STRING)
                moveUpKeyString = controllingKeysForZero[0] + " OR " + controllingKeysForZero[1];
            if (moveDownKeyString != PRESS_A_KEY_STRING)
                moveDownKeyString = controllingKeysForZero[2] + " OR " + controllingKeysForZero[3];
            if (moveLeftKeyString != PRESS_A_KEY_STRING)
                moveLeftKeyString = controllingKeysForZero[4] + " OR " + controllingKeysForZero[5];
            if (moveRightKeyString != PRESS_A_KEY_STRING)
                moveRightKeyString = controllingKeysForZero[6] + " OR " + controllingKeysForZero[7];
            if (fireKeyString != PRESS_A_KEY_STRING)
                fireKeyString = controllingKeysForZero[8] + " OR " + controllingKeysForZero[9];
            if (takeWeaponKeyString != PRESS_A_KEY_STRING)
                takeWeaponKeyString = controllingKeysForZero[10] + " OR " + controllingKeysForZero[11];
            if (leaveWeaponKeyString != PRESS_A_KEY_STRING)
                leaveWeaponKeyString = controllingKeysForZero[12] + " OR " + controllingKeysForZero[13];

            // check what is the current state of the menu
            switch (menuState)
            {
                case MenuStates.Windowed640x480Setting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * GRAPHICS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * WINDOWED_640X480_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if enter key previously was pressed and now is released
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        newWindowWidth = 640;
                        newWindowHeight = 480;
                        SetNewWindowSize(graphics, ref windowWidth, ref windowHeight, zeroCharacter, walls, creatures, weapons, endOfLevel);
                        menuState = MenuStates.OkButton; // goes to the OK Button
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuStates.Windowed800x600Setting;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuStates.FireKeySetting;
                    }

                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.Windowed800x600Setting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * GRAPHICS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * WINDOWED_800X600_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if enter key previously was pressed and now is released
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        newWindowWidth = 800;
                        newWindowHeight = 600;
                        SetNewWindowSize(graphics, ref windowWidth, ref windowHeight, zeroCharacter, walls, creatures, weapons, endOfLevel);
                        menuState = MenuStates.OkButton; // goes to the OK Button
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuStates.Windowed1024x768Setting;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuStates.Windowed640x480Setting;
                    }

                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.Windowed1024x768Setting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * GRAPHICS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * WINDOWED_1024X768_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        newWindowWidth = 1024;
                        newWindowHeight = 768;
                        SetNewWindowSize(graphics, ref windowWidth, ref windowHeight, zeroCharacter, walls, creatures, weapons, endOfLevel);
                        menuState = MenuStates.OkButton; // goes to the OK Button
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuStates.Windowed1280x800Setting;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuStates.Windowed800x600Setting;
                    }

                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.Windowed1280x800Setting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * GRAPHICS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * WINDOWED_1280X800_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if enter key previously was pressed and now is released
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        newWindowWidth = 1280;
                        newWindowHeight = 800;
                        SetNewWindowSize(graphics, ref windowWidth, ref windowHeight, zeroCharacter, walls, creatures, weapons, endOfLevel);
                        menuState = MenuStates.OkButton; // goes to the OK Button
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuStates.FullScreenSetting;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuStates.Windowed1024x768Setting;
                    }

                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.FullScreenSetting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * GRAPHICS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * FULSCREEN_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        if (!graphics.IsFullScreen)
                        {
                            newWindowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                            newWindowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                            // set new coordinates and size of the game objects
                            float aspectRatioWidth = (float)newWindowWidth / (float)windowWidth;
                            float aspectRatioHeight = (float)newWindowHeight / (float)windowHeight;
                            zeroCharacter.CollisionRectangle = new Rectangle(
                                (int)(zeroCharacter.CollisionRectangle.X * aspectRatioWidth),
                                (int)(zeroCharacter.CollisionRectangle.Y * aspectRatioHeight),
                                (int)(zeroCharacter.CollisionRectangle.Width * aspectRatioWidth),
                                (int)(zeroCharacter.CollisionRectangle.Height * aspectRatioHeight));
                            foreach (Wall wall in walls)
                            {
                                wall.CollisionRectangle = new Rectangle(
                                    (int)(wall.CollisionRectangle.X * aspectRatioWidth),
                                    (int)(wall.CollisionRectangle.Y * aspectRatioHeight),
                                    (int)(wall.CollisionRectangle.Width * aspectRatioWidth),
                                    (int)(wall.CollisionRectangle.Height * aspectRatioHeight));
                            }
                            endOfLevel.CollisionRectangle = new Rectangle(
                                (int)(endOfLevel.CollisionRectangle.X * aspectRatioWidth),
                                (int)(endOfLevel.CollisionRectangle.Y * aspectRatioHeight),
                                (int)(endOfLevel.CollisionRectangle.Width * aspectRatioWidth),
                                (int)(endOfLevel.CollisionRectangle.Height * aspectRatioHeight));
                            foreach (Creature creature in creatures)
                            {
                                creature.CollisionRectangle = new Rectangle(
                                    (int)(creature.CollisionRectangle.X * aspectRatioWidth),
                                    (int)(creature.CollisionRectangle.Y * aspectRatioHeight),
                                    (int)(creature.CollisionRectangle.Width * aspectRatioWidth),
                                    (int)(creature.CollisionRectangle.Height * aspectRatioHeight));
                            }
                            foreach (Weapon weapon in weapons)
                            {
                                weapon.CollisionRectangle = new Rectangle(
                                    (int)(weapon.CollisionRectangle.X * aspectRatioWidth),
                                    (int)(weapon.CollisionRectangle.Y * aspectRatioHeight),
                                    (int)(weapon.CollisionRectangle.Width * aspectRatioWidth),
                                    (int)(weapon.CollisionRectangle.Height * aspectRatioHeight));
                            }
                            graphics.IsFullScreen = true;
                            graphics.PreferredBackBufferWidth = windowWidth = newWindowWidth;
                            graphics.PreferredBackBufferHeight = windowHeight = newWindowHeight;
                            graphics.ToggleFullScreen();
                            graphics.ApplyChanges();
                        }
                        menuState = MenuStates.OkButton; // goes to the OK Button
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        menuState = MenuStates.MoveUpKeySetting;
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        menuState = MenuStates.Windowed1280x800Setting;
                    }

                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.MoveUpKeySetting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * KEYS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * MOVE_UP_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if key setting mode is valid
                    if (waitForKeySetting == menuState)
                    {
                        // display on the screen PRESS_A_KEY_STRING text instead which are the controlling keys
                        moveUpKeyString = PRESS_A_KEY_STRING;
                        if (keys.GetPressedKeys().Length != 0)
                        {
                            // check if the pressed key already is present in other of the controllling keys
                            Keys pressedKey = keys.GetPressedKeys()[0];
                            bool isNewPressedKey = true;
                            foreach (Keys controllingKey in controllingKeysForZero)
                            {
                                if (pressedKey == controllingKey)
                                {
                                    isNewPressedKey = false;
                                }
                            }
                            // if it is new key - not present in the other keys
                            if (isNewPressedKey)
                            {
                                // take the first key pressed (if it was a combination)
                                controllingKeysForZero[0] = keys.GetPressedKeys()[0];
                                // key setting mode is not already valid
                                waitForKeySetting = MenuStates.OkButton;
                                moveUpKeyString = "";
                            }
                        }
                    }
                    else
                    {
                        // if key setting mode is not valid
                        if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                        {
                            menuState = MenuStates.MoveDownKeySetting;
                        }
                        if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                        {
                            menuState = MenuStates.FullScreenSetting;
                        }
                    }
                    // if space key is pressed goes in key setting mode
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        waitForKeySetting = menuState;
                    }
                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.MoveDownKeySetting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * KEYS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * MOVE_DOWN_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if key setting mode is valid
                    if (waitForKeySetting == menuState)
                    {
                        // display on the screen PRESS_A_KEY_STRING text instead which are the controlling keys
                        moveDownKeyString = PRESS_A_KEY_STRING;
                        if (keys.GetPressedKeys().Length != 0)
                        {
                            // check if the pressed key already is present in other of the controllling keys
                            Keys pressedKey = keys.GetPressedKeys()[0];
                            bool isNewPressedKey = true;
                            foreach (Keys controllingKey in controllingKeysForZero)
                            {
                                if (pressedKey == controllingKey)
                                {
                                    isNewPressedKey = false;
                                }
                            }
                            // if it is new key - not present in the other keys
                            if (isNewPressedKey)
                            {
                                // take the first key pressed (if it was a combination)
                                controllingKeysForZero[2] = keys.GetPressedKeys()[0];
                                // key setting mode is not already valid
                                waitForKeySetting = MenuStates.OkButton;
                                moveDownKeyString = "";
                            }
                        }
                    }
                    else
                    {
                        // if key setting mode is not valid
                        if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                        {
                            menuState = MenuStates.MoveLeftKeySetting;
                        }
                        if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                        {
                            menuState = MenuStates.MoveUpKeySetting;
                        }
                    }

                    // if space key is pressed goes in key setting mode
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        waitForKeySetting = menuState;
                    }

                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.MoveLeftKeySetting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * KEYS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * MOVE_LEFT_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if key setting mode is valid
                    if (waitForKeySetting == menuState)
                    {
                        // display on the screen PRESS_A_KEY_STRING text instead which are the controlling keys
                        moveLeftKeyString = PRESS_A_KEY_STRING;
                        if (keys.GetPressedKeys().Length != 0)
                        {
                            // check if the pressed key already is present in other of the controllling keys
                            Keys pressedKey = keys.GetPressedKeys()[0];
                            bool isNewPressedKey = true;
                            foreach (Keys controllingKey in controllingKeysForZero)
                            {
                                if (pressedKey == controllingKey)
                                {
                                    isNewPressedKey = false;
                                }
                            }
                            // if it is new key - not present in the other keys
                            if (isNewPressedKey)
                            {
                                // take the first key pressed (if it was a combination)
                                controllingKeysForZero[4] = keys.GetPressedKeys()[0];
                                // key setting mode is not already valid
                                waitForKeySetting = MenuStates.OkButton;
                                moveLeftKeyString = "";
                            }
                        }
                    }
                    else
                    {
                        // if key setting mode is not valid
                        if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                        {
                            menuState = MenuStates.MoveRightKeySetting;
                        }
                        if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                        {
                            menuState = MenuStates.MoveDownKeySetting;
                        }
                    }

                    // if space key is pressed goes in key setting mode
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        waitForKeySetting = menuState;
                    }
                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.MoveRightKeySetting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * KEYS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * MOVE_RIGHT_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if key setting mode is valid
                    if (waitForKeySetting == menuState)
                    {
                        // display on the screen PRESS_A_KEY_STRING text instead which are the controlling keys
                        moveRightKeyString = PRESS_A_KEY_STRING;
                        if (keys.GetPressedKeys().Length != 0)
                        {
                            // check if the pressed key already is present in other of the controllling keys
                            Keys pressedKey = keys.GetPressedKeys()[0];
                            bool isNewPressedKey = true;
                            foreach (Keys controllingKey in controllingKeysForZero)
                            {
                                if (pressedKey == controllingKey)
                                {
                                    isNewPressedKey = false;
                                }
                            }
                            // if it is new key - not present in the other keys
                            if (isNewPressedKey)
                            {
                                // take the first key pressed (if it was a combination)
                                controllingKeysForZero[6] = keys.GetPressedKeys()[0];
                                // key setting mode is not already valid
                                waitForKeySetting = MenuStates.OkButton;
                                moveUpKeyString = "";
                            }
                        }
                    }
                    else
                    {
                        // if key setting mode is not valid
                        if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                        {
                            menuState = MenuStates.TakeWeaponKeySetting;
                        }
                        if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                        {
                            menuState = MenuStates.MoveLeftKeySetting;
                        }
                    }
                    // if space key is pressed goes in key setting mode
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        waitForKeySetting = menuState;
                    }
                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.TakeWeaponKeySetting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * KEYS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * TAKE_WEAPON_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if key setting mode is valid
                    if (waitForKeySetting == menuState)
                    {
                        // display on the screen PRESS_A_KEY_STRING text instead which are the controlling keys
                        takeWeaponKeyString = PRESS_A_KEY_STRING;
                        if (keys.GetPressedKeys().Length != 0)
                        {
                            // check if the pressed key already is present in other of the controllling keys
                            Keys pressedKey = keys.GetPressedKeys()[0];
                            bool isNewPressedKey = true;
                            foreach (Keys controllingKey in controllingKeysForZero)
                            {
                                if (pressedKey == controllingKey)
                                {
                                    isNewPressedKey = false;
                                }
                            }
                            // if it is new key - not present in the other keys
                            if (isNewPressedKey)
                            {
                                // take the first key pressed (if it was a combination)
                                controllingKeysForZero[8] = keys.GetPressedKeys()[0];
                                // key setting mode is not already valid
                                waitForKeySetting = MenuStates.OkButton;
                                takeWeaponKeyString = "";
                            }
                        }
                    }
                    else
                    {
                        // if key setting mode is not valid
                        if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                        {
                            menuState = MenuStates.LeaveWeaponKeySetting;
                        }
                        if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                        {
                            menuState = MenuStates.MoveRightKeySetting;
                        }
                    }
                    // if space key is pressed goes in key setting mode
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        waitForKeySetting = menuState;
                    }
                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.LeaveWeaponKeySetting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * KEYS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * LEAVE_WEAPON_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if key setting mode is valid
                    if (waitForKeySetting == menuState)
                    {
                        // display on the screen PRESS_A_KEY_STRING text instead which are the controlling keys
                        leaveWeaponKeyString = PRESS_A_KEY_STRING;
                        if (keys.GetPressedKeys().Length != 0)
                        {
                            // check if the pressed key already is present in other of the controllling keys
                            Keys pressedKey = keys.GetPressedKeys()[0];
                            bool isNewPressedKey = true;
                            foreach (Keys controllingKey in controllingKeysForZero)
                            {
                                if (pressedKey == controllingKey)
                                {
                                    isNewPressedKey = false;
                                }
                            }
                            // if it is new key - not present in the other keys
                            if (isNewPressedKey)
                            {
                                // take the first key pressed (if it was a combination)
                                controllingKeysForZero[10] = keys.GetPressedKeys()[0];
                                // key setting mode is not already valid
                                waitForKeySetting = MenuStates.OkButton;
                                leaveWeaponKeyString = "";
                            }
                        }
                    }
                    else
                    {
                        // if key setting mode is not valid
                        if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                        {
                            menuState = MenuStates.FireKeySetting;
                        }
                        if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                        {
                            menuState = MenuStates.TakeWeaponKeySetting;
                        }
                    }
                    // if space key is pressed goes in key setting mode
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        waitForKeySetting = menuState;
                    }
                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.FireKeySetting:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle(
                        backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * KEYS_X_OFSETT / 100),
                        backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * FIRE_Y_OFSETT / 100),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if key setting mode is valid
                    if (waitForKeySetting == menuState)
                    {
                        // display on the screen PRESS_A_KEY_STRING text instead which are the controlling keys
                        fireKeyString = PRESS_A_KEY_STRING;
                        if (keys.GetPressedKeys().Length != 0)
                        {
                            // check if the pressed key already is present in other of the controllling keys
                            Keys pressedKey = keys.GetPressedKeys()[0];
                            bool isNewPressedKey = true;
                            foreach (Keys controllingKey in controllingKeysForZero)
                            {
                                if (pressedKey == controllingKey)
                                {
                                    isNewPressedKey = false;
                                }
                            }
                            // if it is new key - not present in the other keys
                            if (isNewPressedKey)
                            {
                                // take the first key pressed (if it was a combination)
                                controllingKeysForZero[12] = keys.GetPressedKeys()[0];
                                // key setting mode is not already valid
                                waitForKeySetting = MenuStates.OkButton;
                                fireKeyString = "";
                            }
                        }
                    }
                    else
                    {
                        // if key setting mode is not valid
                        if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                        {
                            menuState = MenuStates.Windowed640x480Setting;
                        }
                        if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                        {
                            menuState = MenuStates.LeaveWeaponKeySetting;
                        }

                        if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                        {
                            // set initial parameters for this menu
                            menuState = MenuStates.Windowed640x480Setting;
                            waitForKeySetting = MenuStates.OkButton;
                            // goes back to the MainMenu
                            runningMenuState = RunningMenuStates.MainMenu;
                        }
                    }
                    // if space key is pressed goes in key setting mode
                    if (previousKeys.IsKeyDown(Keys.Space) && keys.IsKeyUp(Keys.Space))
                    {
                        waitForKeySetting = menuState;
                    }
                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.OkButton;
                    }

                    break;

                case MenuStates.OkButton:

                    // set the size of the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 10;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 10;
                    // set the pointer of Zero on the left side of the current slot
                    pointerZeroRectangle = new Rectangle
                        (buttonOKRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                        buttonOKRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        // set initial parameters for this menu
                        menuState = MenuStates.Windowed640x480Setting;
                        waitForKeySetting = MenuStates.OkButton;
                        // set the next pressing of Esc to display MainMenu but not OptionsMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                        // set the mode for playing
                        mode = Mode.GameMode;
                    }
                    // if Esc was pressed
                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // goes back in the selections of options
                        menuState = MenuStates.Windowed640x480Setting;
                    }

                    break;

                
            }
            previousKeys = keys;
        }

        private void SetNewWindowSize(GraphicsDeviceManager graphics, ref int windowWidth, ref int windowHeight, ZeroCharacter zeroCharacter, List<Wall> walls, List<Creature> creatures, List<Weapon> weapons, EndOfLevel endOfLevel)
        {
            if (graphics.IsFullScreen)
            {
                graphics.IsFullScreen = false;
                graphics.ToggleFullScreen();
            }
            if (windowWidth != newWindowWidth && windowHeight != newWindowHeight)
            {
                // set new coordinates and size of the game objects
                float aspectRatioWidth = (float)newWindowWidth / (float)windowWidth;
                float aspectRatioHeight = (float)newWindowHeight / (float)windowHeight;
                zeroCharacter.CollisionRectangle = new Rectangle(
                    (int)(zeroCharacter.CollisionRectangle.X * aspectRatioWidth),
                    (int)(zeroCharacter.CollisionRectangle.Y * aspectRatioHeight),
                    (int)(zeroCharacter.CollisionRectangle.Width * aspectRatioWidth),
                    (int)(zeroCharacter.CollisionRectangle.Height * aspectRatioHeight));
                foreach (Wall wall in walls)
                {
                    wall.CollisionRectangle = new Rectangle(
                        (int)(wall.CollisionRectangle.X * aspectRatioWidth),
                        (int)(wall.CollisionRectangle.Y * aspectRatioHeight),
                        (int)(wall.CollisionRectangle.Width * aspectRatioWidth),
                        (int)(wall.CollisionRectangle.Height * aspectRatioHeight));
                }
                endOfLevel.CollisionRectangle = new Rectangle(
                    (int)(endOfLevel.CollisionRectangle.X * aspectRatioWidth),
                    (int)(endOfLevel.CollisionRectangle.Y * aspectRatioHeight),
                    (int)(endOfLevel.CollisionRectangle.Width * aspectRatioWidth),
                    (int)(endOfLevel.CollisionRectangle.Height * aspectRatioHeight));
                foreach (Creature creature in creatures)
                {
                    creature.CollisionRectangle = new Rectangle(
                        (int)(creature.CollisionRectangle.X * aspectRatioWidth),
                        (int)(creature.CollisionRectangle.Y * aspectRatioHeight),
                        (int)(creature.CollisionRectangle.Width * aspectRatioWidth),
                        (int)(creature.CollisionRectangle.Height * aspectRatioHeight));
                }
                foreach (Weapon weapon in weapons)
                {
                    weapon.CollisionRectangle = new Rectangle(
                        (int)(weapon.CollisionRectangle.X * aspectRatioWidth),
                        (int)(weapon.CollisionRectangle.Y * aspectRatioHeight),
                        (int)(weapon.CollisionRectangle.Width * aspectRatioWidth),
                        (int)(weapon.CollisionRectangle.Height * aspectRatioHeight));
                }
                graphics.PreferredBackBufferWidth = windowWidth = newWindowWidth;
                graphics.PreferredBackBufferHeight = windowHeight = newWindowHeight;
                graphics.ApplyChanges();
            }
        }

        /// <summary>
        /// draw the menu
        /// </summary>
        /// <param name="spriteBatch">the sprite batch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundTexture, backgroundDrawRectangle, Color.White);
            spriteBatch.Draw(buttonOKTexture, buttonOKRectangle, Color.White);
            spriteBatch.Draw(pointerZeroTexture, pointerZeroRectangle, Color.White);
            spriteBatch.DrawString(font, moveUpKeyString, moveUpKeyStringVector, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, moveDownKeyString, moveDownKeyStringVector, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, moveLeftKeyString, moveLeftKeyStringVector, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, moveRightKeyString, moveRightKeyStringVector, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, takeWeaponKeyString, takeWeaponKeyStringVector, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, leaveWeaponKeyString, leaveWeaponKeyStringVector, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, fireKeyString, fireKeyStringVector, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
        }

        #endregion public methods
    }
}
