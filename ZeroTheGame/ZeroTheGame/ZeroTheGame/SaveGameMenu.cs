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
    public class SaveGameMenu
    {
        #region fields

        // constants for the drawing fields
        const int BACKGROUND_OFSET_FROM_WINDOW = 20;
        const int SLOTS = 10;
        const float SLOTS_X_OFSET = 5;
        const float SLOT_TOP_OFSET = 14;
        const float SLOTS_OFSET_BETWEEN_EACHOTHER = 10;
        const float SLOT_WIDTH_PERCENTAGE = 90;
        const float SLOT_HEIGHT_PERCENTAGE = 5;
        const float BUTTON_SAVE_X_OFSET = 50;
        const float BUTTONS_CANCEL_X_OFSET = 75;
        const float BUTTONS_Y_OFSET = 85;
        const float BUTTONS_WIDTH_PERCENTAGE = 20;
        const float BUTTONS_HEIGHT_PERCENTAGE = 10;
        const float POINTER_ZERO_X_OFSET = 0.5f;
        const float POINTER_ZERO_Y_OFSET = 0.8f;
        const float TEXT_OFSET_IN_SLOTS = 0.05f;

        // states of the menu
        public enum MenuStates
        {
            SelectSlot,
            SaveButton,
            CancelBotton
        }
        MenuStates menuState;

        // drawing fields
        Rectangle backgroundDrawRectangle;
        Texture2D backgroundTexture;

        Rectangle[] slotDrawRectangle = new Rectangle[SLOTS];
        Texture2D[] slotTexture = new Texture2D[SLOTS];

        Rectangle buttonSaveRectangle;
        Texture2D buttonSaveTexture;

        Rectangle buttonCancelRectangle;
        Texture2D buttonCancelTexture;

        Rectangle pointerZeroRectangle;
        Texture2D pointerZeroTexture;
        int pointerZeroTextureWidth;
        int pointerZeroTextureHeight;

        // text drawing on screen
        SpriteFont font;
        string[] slotsNamesStrings = new string[SLOTS];

        // which is the current slot
        int slotPossition;
        // which is the current slot for loading
        int slotPositionForLoading;

        // keyboard management
        KeyboardState previousKeys;

        // saving and reading files management
        StorageDevice device;
        string containerName = "MyGamesStorage";
        string filenamePart1 = "SaveFile"; // part 2 is the slotPosition number
        string filenameExtension = ".dat";
        [Serializable]
        public struct SaveGameData
        {
            public DateTime currentTime;
            public int currentLevel;
            public Vector2 zeroPosition;
            public int zeroHealth;
            public int zeroMaxHealth;
            public float zeroSkillToFireWithWeapon;
            public Weapon zeroActiveWeapon;
            public List<Keys> controllingKeysForZero;
            public List<Vector2> creaturesPosition;
            public List<CreatureType> creaturesType;
            public List<float> creaturesLives;
            public List<int> creaturesDamages;
            public List<WeaponType> weaponsTypes;
            public List<Vector2> weaponsPositions;
            public List<bool> weaponsOwned;
            public int windowWidth;
            public int windowHeight;
        }

        SaveGameData readDataFromFile = new SaveGameData(); // what must be saved
        SaveGameData saveDataToFile = new SaveGameData(); // initial reading of the save files

        #endregion fields

        #region constructor

        /// <summary>
        /// create the save game menu
        /// </summary>
        /// <param name="content">the content manager</param>
        /// <param name="windowWidth">the width of the game window</param>
        /// <param name="windowHeight">the height of the game window</param>
        public SaveGameMenu(ContentManager content, int windowWidth, int windowHeight)
        {
            // load texture font
            font = content.Load<SpriteFont>("Fonts//SpriteFont2");

            menuState = MenuStates.SelectSlot;

            //sets the textures for the menu and their initial place on the screen
            backgroundTexture = content.Load<Texture2D>("images//MenuSaveGameBackgroundRectangle");
            backgroundDrawRectangle = new Rectangle
                (windowWidth / BACKGROUND_OFSET_FROM_WINDOW,
                windowHeight / BACKGROUND_OFSET_FROM_WINDOW,
                windowWidth - 2 * windowWidth / BACKGROUND_OFSET_FROM_WINDOW,
                windowHeight - 2 * windowHeight / BACKGROUND_OFSET_FROM_WINDOW);

            for (int i = 0; i < SLOTS; i++)
            {
                slotTexture[i] = content.Load<Texture2D>("images//MenuSlot");
                int slot_width = (int)(backgroundDrawRectangle.Width * SLOT_WIDTH_PERCENTAGE / 100);
                int slot_height = (int)(backgroundDrawRectangle.Height * SLOT_HEIGHT_PERCENTAGE / 100);
                slotDrawRectangle[i] = new Rectangle(
                    backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * SLOTS_X_OFSET / 100),
                    (int)(backgroundDrawRectangle.Y + (SLOT_TOP_OFSET * backgroundDrawRectangle.Height / 100) + i * (slot_height + SLOTS_OFSET_BETWEEN_EACHOTHER)),
                    slot_width,
                    slot_height);
            }

            buttonSaveTexture = content.Load<Texture2D>("images//MenuButtonSave");
            buttonSaveRectangle = new Rectangle
                (backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * BUTTON_SAVE_X_OFSET / 100),
                backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * BUTTONS_Y_OFSET / 100),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_PERCENTAGE / 100),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_PERCENTAGE / 100));

            buttonCancelTexture = content.Load<Texture2D>("images//MenuButtonCancel");
            buttonCancelRectangle = new Rectangle
                (backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * BUTTONS_CANCEL_X_OFSET / 100),
                backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * BUTTONS_Y_OFSET / 100),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_PERCENTAGE / 100),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_PERCENTAGE / 100));

            pointerZeroTexture = content.Load<Texture2D>("images//ZeroSingleImage");
            pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
            pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
            pointerZeroRectangle = new Rectangle
                (slotDrawRectangle[0].X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                slotDrawRectangle[0].Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                pointerZeroTextureWidth,
                pointerZeroTextureHeight);

            ReadSlots();

            slotPossition = 0;
        }

        #endregion constructor

        #region properties

        public MenuStates MenuState
        {
            get { return menuState; }
            set { menuState = value; }
        }

        public int SlotPosition
        {
            get { return slotPossition; }
            set { slotPossition = value; }
        }

        #endregion properties

        #region public methods

        /// <summary>
        /// update the save game menu
        /// </summary>
        /// <param name="gameTime">the game time</param>
        /// <param name="keys">the keyboard pressed keys</param>
        /// <param name="runningGameState">pass a reference of the RunningGameState variable</param>
        /// <param name="mode">pass a reference of the mode in game</param>
        /// <param name="runningMenuState">pass a reference of the state of the menus</param>
        /// <param name="currentLevel">the current level</param>
        /// <param name="zeroPosition">where currently Zero is</param>
        /// <param name="zeroHealth">How much healt Zero has</param>
        /// <param name="zeroMaxHealth">What is the max health od Zero</param>
        /// <param name="zeroSkillToFireWithWeapon">How skillful with weapons Zero is</param>
        /// <param name="zeroActiveWeapon">what is the current weapon Zero has</param>
        /// <param name="controllingKeysForZero">the keyboard keys which controlls Zero</param>
        /// <param name="creaturesPosition">all positions of the creatures</param>
        /// <param name="creaturesType">the types of the creatures in this level</param>
        /// <param name="creaturesLives">the current health points the creatures has</param>
        /// <param name="creaturesDamages">the current damage points the creatures has</param>
        /// <param name="weaponsTypes">the weapon types in this level</param>
        /// <param name="weaponsPositions">the positions of the weapons</param>
        /// <param name="windowWidth">the game window widht</param>
        /// <param name="windowHeight">the game window height</param>
        public void Update
            (
            GameTime gameTime, 
            KeyboardState keys, 
            ref Mode mode, 
            ref RunningMenuStates runningMenuState, 
            int currentLevel, 
            Vector2 zeroPosition,
            int zeroHealth,
            int zeroMaxHealth,
            float zeroSkillToFireWithWeapon,
            Weapon zeroActiveWeapon,
            List<Keys> controllingKeysForZero,
            List<Vector2> creaturesPosition,
            List<CreatureType> creaturesType,
            List<float> creaturesLives,
            List<int> creaturesDamages,
            List<WeaponType> weaponsTypes,
            List<Vector2> weaponsPositions,
            List<bool> weaponsOwned,
            int windowWidth,
            int windowHeight
            )
        {
            
            // check what is the current state of the menu
            switch (menuState)
            {
                    // if it is in position for slot selection
                case MenuStates.SelectSlot:
                    // set the size for the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                    // set the pointer on the left side of the current slot
                    pointerZeroRectangle = new Rectangle
                        (slotDrawRectangle[slotPossition].X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                        slotDrawRectangle[slotPossition].Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if enter key previously was pressed and now is released
                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        menuState = MenuStates.SaveButton; // goes to the Save Button
                    }
                    if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                    {
                        // next slot
                        slotPossition++;
                        if (slotPossition == SLOTS)
                        {
                            slotPossition = 0;
                        }
                    }
                    if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                    {
                        //previous slot
                        slotPossition--;
                        if (slotPossition < 0)
                        {
                            slotPossition = SLOTS - 1;
                        }
                    }

                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // goes back to the MainMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                    }

                    break;
                case MenuStates.SaveButton:

                    // set the size of the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 10;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 10;
                    // set the pointer of Zero on the left side of the current slot
                    pointerZeroRectangle = new Rectangle
                        (buttonSaveRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                        buttonSaveRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if enter key previously was pressed and now is released
                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        //// preparation for saving data to file ////
                        saveDataToFile.controllingKeysForZero = controllingKeysForZero;
                        saveDataToFile.creaturesDamages = creaturesDamages;
                        saveDataToFile.creaturesLives = creaturesLives;
                        saveDataToFile.creaturesPosition = creaturesPosition;
                        saveDataToFile.creaturesType = creaturesType;
                        saveDataToFile.currentLevel = currentLevel;
                        saveDataToFile.currentTime = DateTime.Now;
                        saveDataToFile.weaponsPositions = weaponsPositions;
                        saveDataToFile.weaponsTypes = weaponsTypes;
                        saveDataToFile.weaponsOwned = weaponsOwned;
                        saveDataToFile.windowHeight = windowHeight;
                        saveDataToFile.windowWidth = windowWidth;
                        saveDataToFile.zeroActiveWeapon = zeroActiveWeapon;
                        saveDataToFile.zeroHealth = zeroHealth;
                        saveDataToFile.zeroMaxHealth = zeroMaxHealth;
                        saveDataToFile.zeroPosition = zeroPosition;
                        saveDataToFile.zeroSkillToFireWithWeapon = zeroSkillToFireWithWeapon;
                        // start saving
                        InitiateSave();
                        // set the next pressing of Esc to display MainMenu but not SaveGameMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                        // set the mode for playing
                        mode = Mode.GameMode;
                    }
                    // if Left or Right key was pressed
                    if (
                        (previousKeys.IsKeyDown(Keys.Left) && keys.IsKeyUp(Keys.Left)) || 
                        (previousKeys.IsKeyDown(Keys.Right) && keys.IsKeyUp(Keys.Right))
                       )
                    {
                        // goes to the Cancel button
                        menuState = MenuStates.CancelBotton;
                    }
                    // if Esc was pressed
                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // goes in the Slot Selection mode
                        menuState = MenuStates.SelectSlot;
                    }

                    break;

                case MenuStates.CancelBotton:
                    // set the size of the pointer
                    pointerZeroTextureWidth = backgroundDrawRectangle.Width / 10;
                    pointerZeroTextureHeight = backgroundDrawRectangle.Height / 10;
                    // set the pointer of Zero on the left side of the current slot
                    pointerZeroRectangle = new Rectangle
                        (buttonCancelRectangle.X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                        buttonCancelRectangle.Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                        pointerZeroTextureWidth,
                        pointerZeroTextureHeight);
                    // if enter key previously was pressed and now is released
                    if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                    {
                        // set the next pressing of Esc to display MainMenu but not SaveGameMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                        // set the mode for playing
                        mode = Mode.GameMode;
                    }
                    // if Left or Right key was pressed
                    if (
                        (previousKeys.IsKeyDown(Keys.Left) && keys.IsKeyUp(Keys.Left)) ||
                        (previousKeys.IsKeyDown(Keys.Right) && keys.IsKeyUp(Keys.Right))
                       )
                    {
                        // goes to the Save Button
                        menuState = MenuStates.SaveButton;
                    }
                    // if Esc was pressed
                    if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                    {
                        // goes in the Slot Selection mode
                        menuState = MenuStates.SelectSlot;
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
            for (int i = 0; i < SLOTS; i++)
            {
                spriteBatch.Draw(slotTexture[i], slotDrawRectangle[i], Color.White);
            }
            spriteBatch.Draw(buttonSaveTexture, buttonSaveRectangle, Color.White);
            spriteBatch.Draw(buttonCancelTexture, buttonCancelRectangle, Color.White);
            spriteBatch.Draw(pointerZeroTexture, pointerZeroRectangle, Color.White);
            // spriteBatch.DrawString(font, slotsNamesStrings[0], new Vector2(10, 110), Color.Black);
            for (slotPositionForLoading = 0; slotPositionForLoading < SLOTS; slotPositionForLoading++)
            {
                spriteBatch.DrawString
                    (
                     font, 
                     slotsNamesStrings[slotPositionForLoading], 
                     new Vector2
                         (
                          slotDrawRectangle[slotPositionForLoading].X + TEXT_OFSET_IN_SLOTS*slotDrawRectangle[slotPositionForLoading].Width, 
                          slotDrawRectangle[slotPositionForLoading].Y
                         ), 
                     Color.Black, 
                     0, 
                     new Vector2(0, 0), 
                     0.9f, 
                     SpriteEffects.None, 
                     0
                    );
            }
        }

        /// <summary>
        /// read initial data for displaying in which slots game data was saved
        /// </summary>
        public void ReadSlots()
        {
            for (slotPositionForLoading = 0; slotPositionForLoading < SLOTS; slotPositionForLoading++)
            {
                InitiateRead();
            }
        }

        #endregion public methods

        #region private methods

        private void InitiateSave()
        {
            if (!Guide.IsVisible)
            {
                device = null;
                StorageDevice.BeginShowSelector(PlayerIndex.One, this.SaveToDevice, null);
            }
        }

        void SaveToDevice(IAsyncResult result)
        {
            string filename = filenamePart1 + slotPossition + filenameExtension;
            device = StorageDevice.EndShowSelector(result);
            if (device != null && device.IsConnected)
            {
                IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = device.EndOpenContainer(r);
                if (container.FileExists(filename))
                    container.DeleteFile(filename);
                Stream stream = container.CreateFile(filename);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                serializer.Serialize(stream, saveDataToFile);
                stream.Close();
                container.Dispose();
                result.AsyncWaitHandle.Close();
            }
        }

        private void InitiateRead()
        {
            if (!Guide.IsVisible)
            {
                device = null;
                StorageDevice.BeginShowSelector(PlayerIndex.One, this.ReadFromDevice, null);
            }
        }

        private void ReadFromDevice(IAsyncResult result)
        {
            string filename = filenamePart1 + slotPositionForLoading + filenameExtension;
            device = StorageDevice.EndShowSelector(result);
            IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(r);
            result.AsyncWaitHandle.Close();
            if (container.FileExists(filename))
            {
                Stream stream = container.OpenFile(filename, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                SaveGameData SaveData = (SaveGameData)serializer.Deserialize(stream);
                stream.Close();
                container.Dispose();
                //Update the game based on the save game file
                readDataFromFile = SaveData;
                slotsNamesStrings[slotPositionForLoading] = readDataFromFile.currentTime.ToString() + " - Labyrinth " + readDataFromFile.currentLevel.ToString();
            }
            else
            {
                slotsNamesStrings[slotPositionForLoading] = "< EMPTY >";
            }
        }

        #endregion private methods
    }
}
