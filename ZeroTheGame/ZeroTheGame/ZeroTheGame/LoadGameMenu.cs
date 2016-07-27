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
    public class LoadGameMenu
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
        const string TEXT_FOR_EMPTY_SLOT = "< EMPTY >";

        // states of the menu
        public enum MenuStates
        {
            SelectSlot,
            LoadButton,
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
        int slotPosition;
        // which is the current slot for loading
        int slotPositionForLoading;
        // if there are any saves available
        bool thereAreSaves = false;

        // keyboard management
        KeyboardState previousKeys;

        // loading and reading files management
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

        SaveGameData readDataFromFile = new SaveGameData(); // initial (like a demo) reading data from the slots
        SaveGameData loadDataFromFile = new SaveGameData(); // the real loading the data from a file

        #endregion fields

        #region constructor

        /// <summary>
        /// create the save game menu
        /// </summary>
        /// <param name="content">the content manager</param>
        /// <param name="windowWidth">the width of the game window</param>
        /// <param name="windowHeight">the height of the game window</param>
        public LoadGameMenu(ContentManager content, int windowWidth, int windowHeight, Game1 game1)
        {
            // load texture font
            font = content.Load<SpriteFont>("Fonts//SpriteFont2");

            // set the initial state of this menu to selection of slot to load
            menuState = MenuStates.SelectSlot;

            ////set the textures for the menu and their initial place on the screen ////
            // load the background texture and set the rectangle according the game window size
            backgroundTexture = content.Load<Texture2D>("images//MenuLoadGameBackgroundRectangle");
            backgroundDrawRectangle = new Rectangle
                (windowWidth / BACKGROUND_OFSET_FROM_WINDOW,
                windowHeight / BACKGROUND_OFSET_FROM_WINDOW,
                windowWidth - 2 * windowWidth / BACKGROUND_OFSET_FROM_WINDOW,
                windowHeight - 2 * windowHeight / BACKGROUND_OFSET_FROM_WINDOW);

            // load the slots texture and set the rectangle according the backgorund
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

            // load the load button texture and set the rectangle according the background
            buttonSaveTexture = content.Load<Texture2D>("images//MenuButtonLoad");
            buttonSaveRectangle = new Rectangle
                (backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * BUTTON_SAVE_X_OFSET / 100),
                backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * BUTTONS_Y_OFSET / 100),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_PERCENTAGE / 100),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_PERCENTAGE / 100));

            // load the cancel button texture and set the rectangle according the background
            buttonCancelTexture = content.Load<Texture2D>("images//MenuButtonCancel");
            buttonCancelRectangle = new Rectangle
                (backgroundDrawRectangle.X + (int)(backgroundDrawRectangle.Width * BUTTONS_CANCEL_X_OFSET / 100),
                backgroundDrawRectangle.Y + (int)(backgroundDrawRectangle.Height * BUTTONS_Y_OFSET / 100),
                (int)(backgroundDrawRectangle.Width * BUTTONS_WIDTH_PERCENTAGE / 100),
                (int)(backgroundDrawRectangle.Height * BUTTONS_HEIGHT_PERCENTAGE / 100));

            // load the pointer texture and set the rectangle according the first slot position
            pointerZeroTexture = content.Load<Texture2D>("images//ZeroSingleImage");
            pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
            pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
            pointerZeroRectangle = new Rectangle
                (slotDrawRectangle[0].X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                slotDrawRectangle[0].Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                pointerZeroTextureWidth,
                pointerZeroTextureHeight);

            ReadSlots();

            // set the initial position to the first slot
            slotPosition = 0;

            // initiate loadDataFromFile objects
            loadDataFromFile.zeroPosition = new Vector2();
            loadDataFromFile.zeroActiveWeapon = null;
            loadDataFromFile.creaturesPosition = new List<Vector2>();
            loadDataFromFile.creaturesType = new List<CreatureType>();
            loadDataFromFile.creaturesLives = new List<float>();
            loadDataFromFile.creaturesDamages = new List<int>();
            loadDataFromFile.weaponsTypes = new List<WeaponType>();
            loadDataFromFile.weaponsPositions = new List<Vector2>();
            loadDataFromFile.weaponsOwned = new List<bool>();
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
            get { return slotPosition; }
            set { slotPosition = value; }
        }

        #endregion properties

        #region public methods

        /// <summary>
        /// update the load game menu
        /// </summary>
        /// <param name="keys">the keyboard pressed keys</param>
        /// <param name="runningGameState">pass a reference of the RunningGameState variable</param>
        /// <param name="mode">pass a reference of the mode in game</param>
        /// <param name="runningMenuState">pass a reference of the state of the menus</param>
        /// <param name="currentLevel">pass a reference to the current level field</param>
        /// <param name="zeroCharacter">pass a reference to the ZeroCharacter object</param>
        /// <param name="controllingKeysForZero">pass a reference to the list of keys which control Zero</param>
        /// <param name="creatures">pass a reference to the list of creatures</param>
        /// <param name="weapons">pass a reference to the list of weapons</param>
        /// <param name="windowWidth">pass a reference to the game window width field</param>
        /// <param name="windowHeight">pass a reference to the game window height field</param>
        /// <param name="game1">the game1 object</param>
        public void Update
            (
            KeyboardState keys,
            ref Mode mode,
            ref RunningMenuStates runningMenuState,
            ref int currentLevel,
            ref ZeroCharacter zeroCharacter,
            ref List<Keys> controllingKeysForZero,
            ref List<Creature> creatures,
            ref List<Weapon> weapons,
            ref int windowWidth,
            ref int windowHeight,
            Game1 game1,
            ContentManager content
            )
        {

            // check what is the current state of the menu
            switch (menuState)
            {
                // if it is in position for slot selection
                case MenuStates.SelectSlot:
                    // check if there are any saves
                    for (int i = 0; i < slotsNamesStrings.Count(); i++)
                    {
                        if (slotsNamesStrings[i] != TEXT_FOR_EMPTY_SLOT)
                        {
                            thereAreSaves = true;
                            break;
                        }
                    }
                    if (thereAreSaves)
                    {
                        // set the size for the pointer
                        pointerZeroTextureWidth = backgroundDrawRectangle.Width / 20;
                        pointerZeroTextureHeight = backgroundDrawRectangle.Height / 20;
                        // set the pointer on the left side of the current slot
                        pointerZeroRectangle = new Rectangle
                            (slotDrawRectangle[slotPosition].X - (int)(pointerZeroTextureWidth * POINTER_ZERO_X_OFSET),
                            slotDrawRectangle[slotPosition].Center.Y - (int)(pointerZeroTextureHeight * POINTER_ZERO_Y_OFSET),
                            pointerZeroTextureWidth,
                            pointerZeroTextureHeight);
                        // if enter key previously was pressed and now is released
                        if (previousKeys.IsKeyDown(Keys.Enter) && keys.IsKeyUp(Keys.Enter))
                        {
                            menuState = MenuStates.LoadButton; // goes to the load button
                        }
                        if (previousKeys.IsKeyDown(Keys.Down) && keys.IsKeyUp(Keys.Down))
                        {
                            // next slot
                            slotPosition++;
                            if (slotPosition == SLOTS)
                            {
                                slotPosition = 0;
                            }
                        }
                        if (previousKeys.IsKeyDown(Keys.Up) && keys.IsKeyUp(Keys.Up))
                        {
                            //previous slot
                            slotPosition--;
                            if (slotPosition < 0)
                            {
                                slotPosition = SLOTS - 1;
                            }
                        }

                        if (previousKeys.IsKeyDown(Keys.Escape) && keys.IsKeyUp(Keys.Escape))
                        {
                            // goes back to the MainMenu
                            runningMenuState = RunningMenuStates.MainMenu;
                        }
                    }
                    else
                    {
                        menuState = MenuStates.CancelBotton;
                    }

                    break;
                case MenuStates.LoadButton:
                    // check if there are any saves
                    for (int i = 0; i < slotsNamesStrings.Count(); i++)
                    {
                        if (slotsNamesStrings[i] != TEXT_FOR_EMPTY_SLOT)
                        {
                            thereAreSaves = true;
                            break;
                        }
                    }
                    if (thereAreSaves)
                    {
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
                            // load the data from file
                            InitiateLoad();
                            //// load data to the game objects ////
                            currentLevel = loadDataFromFile.currentLevel;
                            // load initial data for the current level
                            game1.LoadCurrentLevel();
                            // clear the initial loaded weapons for the level
                            weapons.Clear();
                            // add weapons in the weapons list from the loaded data
                            for (int i = 0; i < loadDataFromFile.weaponsTypes.Count; i++)
                            {
                                weapons.Add(new Weapon(
                                    content,
                                    loadDataFromFile.weaponsTypes[i],
                                    (int)loadDataFromFile.weaponsPositions[i].X,
                                    (int)loadDataFromFile.weaponsPositions[i].Y,
                                    game1.sizeOfCharacter,
                                    loadDataFromFile.weaponsOwned[i]));
                                    //false));
                            }

                            // make "new" Zero character
                            zeroCharacter = new ZeroCharacter(
                                content.Load<Texture2D>("images//SmileyWalkTighter"),
                                (int)loadDataFromFile.zeroPosition.X,
                                (int)loadDataFromFile.zeroPosition.Y,
                                4,
                                4,
                                game1.sizeOfCharacter,
                                null);
                            
                            // load the keys which control Zero
                            controllingKeysForZero = loadDataFromFile.controllingKeysForZero;
                            
                            // check if any weapon in the list of weapons is owned and if yes then set it for Zero's weapon
                            foreach (Weapon weapon in weapons)
                            {
                                if (weapon.IsOwned)
                                {
                                    zeroCharacter.ActiveWeapon = weapon;
                                }
                            }

                            // check if there are any info about creatures loaded
                            if (loadDataFromFile.creaturesDamages.Count == 0)
                            {
                                creatures.Clear();
                            }
                            else
                            {
                                for (int i = 0; i < creatures.Count; i++)
                                {
                                    if (i < loadDataFromFile.creaturesDamages.Count)
                                    {
                                        creatures[i].Damage = loadDataFromFile.creaturesDamages[i];
                                        creatures[i].Health = loadDataFromFile.creaturesLives[i];
                                        creatures[i].CollisionRectangle = new Rectangle(
                                            (int)loadDataFromFile.creaturesPosition[i].X,
                                            (int)loadDataFromFile.creaturesPosition[i].Y,
                                            creatures[i].CollisionRectangle.Width,
                                            creatures[i].CollisionRectangle.Height);
                                        creatures[i].Type = loadDataFromFile.creaturesType[i];
                                    }
                                    else // if the creatures initially loaded for the curent level are more than 
                                    // the loaded data then some of them are killed in the time of saving the game
                                    {
                                        creatures.RemoveAt(i);
                                        i--;
                                    }
                                }
                            }

                            
                            
                            // check if there is change in game window resolution and only then set the new one
                            if (windowHeight != loadDataFromFile.windowHeight || windowWidth != loadDataFromFile.windowWidth)
                            {
                                windowHeight = loadDataFromFile.windowHeight;
                                windowWidth = loadDataFromFile.windowWidth;
                                game1.graphics.PreferredBackBufferHeight = windowHeight;
                                game1.graphics.PreferredBackBufferWidth = windowWidth;
                            }

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
                    }
                    else
                    {
                        menuState = MenuStates.CancelBotton;
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
                        // set the next pressing of Esc to display MainMenu but not LoadeGameMenu
                        runningMenuState = RunningMenuStates.MainMenu;
                        // set the mode to display the main menu
                        mode = Mode.MenuMode;
                    }
                    // if Left or Right key was pressed
                    if (
                        (previousKeys.IsKeyDown(Keys.Left) && keys.IsKeyUp(Keys.Left)) ||
                        (previousKeys.IsKeyDown(Keys.Right) && keys.IsKeyUp(Keys.Right))
                       )
                    {
                        // goes to the load button
                        menuState = MenuStates.LoadButton;
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
            for (slotPositionForLoading = 0; slotPositionForLoading < SLOTS; slotPositionForLoading++)
            {
                spriteBatch.DrawString
                    (
                     font,
                     slotsNamesStrings[slotPositionForLoading],
                     new Vector2
                         (
                          slotDrawRectangle[slotPositionForLoading].X + TEXT_OFSET_IN_SLOTS * slotDrawRectangle[slotPositionForLoading].Width,
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
            // read which slots there are saves to
            for (slotPositionForLoading = 0; slotPositionForLoading < SLOTS; slotPositionForLoading++)
            {
                InitiateRead();
            }
        }

        #endregion public methods

        #region private methods

        private void InitiateRead()
        {
            if (!Guide.IsVisible)
            {
                device = null;
                StorageDevice.BeginShowSelector(PlayerIndex.One, this.ReadFromDevice, null);
            }
        }

        void ReadFromDevice(IAsyncResult result)
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
                slotsNamesStrings[slotPositionForLoading] = 
                    readDataFromFile.currentTime.ToString() + 
                    " - Labyrinth " + 
                    readDataFromFile.currentLevel.ToString();
            }
            else
            {
                slotsNamesStrings[slotPositionForLoading] = TEXT_FOR_EMPTY_SLOT;
            }
        }

        private void InitiateLoad()
        {
            if (!Guide.IsVisible)
            {
                device = null;
                StorageDevice.BeginShowSelector(PlayerIndex.One, this.LoadFromDevice, null);
            }
        }

        void LoadFromDevice(IAsyncResult result)
        {
            string filename = filenamePart1 + slotPosition + filenameExtension;
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
                loadDataFromFile = SaveData;
                
            }
        }

        #endregion private methods
    }
}
