using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace ZeroTheGame
{
    /// <summary>
    /// This is the main type for the game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region fields

        public static Game1 self;

        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // states
        Mode mode;
        RunningGameStates runningGameState;
        RunningMenuStates runningMenuState;
        RunningMenuStates previousRunningMenuState;

        // display text on screen variables
        SpriteFont font;
        string[] finishedLevelStrings = { "" };

        //// Zero's definitions ////
        // the Zero object
        ZeroCharacter zeroCharacter;
        // list with pressedKeysFromKeyboard which control Zero's actions
        List<Keys> controllingKeysForZero = new List<Keys>();
        // rectangle which holds the position of Zero from the previous momennt
        Rectangle previousZeroSRectangle;
        // the direction of Zero
        Vector2 zeroDirection;
        // Zero's width and height for particular level
        public float sizeOfCharacter;

        //// Creatures definitions ////
        // list of creatures
        List<Creature> creatures = new List<Creature>();
        // the previous position of the creatures
        Rectangle[] previousCreaturesRectangles = new Rectangle[1000];
        // List of creatures types
        List<CreatureType> creatureTypes = new List<CreatureType>();


        // List of walls
        List<Wall> walls = new List<Wall>();

        //// weapons definitions ////
        // list of weapons
        List<Weapon> weapons = new List<Weapon>();
        // list of weapons types
        List<WeaponType> weaponsTypes = new List<WeaponType>();

        //// projectiles definitions ////
        // list of projectiles
        List<Projectile> projectiles = new List<Projectile>();
        // list of projectile definitions
        List<ProjectileType> projectilesTypes = new List<ProjectileType>();

        // End of the level object
        EndOfLevel endOfLevel;

        // game window variables
        int windowWidth;
        int windowHeight;

        //// time management ////
        // difne how many miliseconds to show the screen between the levels before to advance to the next level
        // the pause is necessary to avoid any keys pressed in the previous level
        const int timePauseBetweenLevels = 1000;
        // how miliseconds left before checking whether any key is pressed to advance to the next level
        int timeElapsedBetweenLevels = 0;

        //// levels info ////
        int currentLevel = 1;
        Texture2D currectLevelBackgroundTexture;
        // total number of levels in the game
        int numberOfLevels;

        //// menus definitions
        // StartMenu startMenu;		// <-- ToDo
        MainMenu mainMenu;
        SaveGameMenu saveGameMenu;
        LoadGameMenu loadGameMenu;
        OptionsMenu optionsMenu;

        // keyboard info management
        KeyboardState pressedKeysFromKeyboard;
        KeyboardState previousPressedKeysFromKeyboard;

        #endregion fields

        #region constructor

        public Game1()
        {
            self = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Components.Add(new GamerServicesComponent(this));
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
        }

        #endregion constructor

        #region Initialize method
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

        }
        #endregion Initialize method

        #region LoadContent method
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of the content.
        /// </summary>
        protected override void LoadContent()
        {
            // defining the game to begin with 
            mode = Mode.GameMode;
            runningMenuState = RunningMenuStates.MainMenu;
            //previousRunningMenuState = runningMenuState;

            // load texture font
            font = Content.Load<SpriteFont>("Fonts//SpriteFont1");

            // loads in variables the width and height of the game window
            windowWidth = graphics.PreferredBackBufferWidth;
            windowHeight = graphics.PreferredBackBufferHeight;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load data about the first level
            LoadCurrentLevel();

            // define the pressedKeysFromKeyboard which control Zero.
            // they must be defined in pairs in strict order; one pair controls one action:
            // 0 and 1 - move up
            // 2, 3 - move down
            // 4, 5 - left
            // 6, 7 - right
            // 8, 9 - fire
            // 10, 11 - take item from the ground (which is not owned by anyone)
            // 12, 13 - leaves the item
            controllingKeysForZero.Add(Keys.W);             // move up
            controllingKeysForZero.Add(Keys.Up);            // move up
            controllingKeysForZero.Add(Keys.S);             // move down
            controllingKeysForZero.Add(Keys.Down);          // move down
            controllingKeysForZero.Add(Keys.A);             // move left
            controllingKeysForZero.Add(Keys.Left);          // move left
            controllingKeysForZero.Add(Keys.D);             // move right
            controllingKeysForZero.Add(Keys.Right);         // move right
            controllingKeysForZero.Add(Keys.LeftControl);   // fire
            controllingKeysForZero.Add(Keys.Space);         // fire
            controllingKeysForZero.Add(Keys.F);             // take weapon
            controllingKeysForZero.Add(Keys.Enter);         // take weapon
            controllingKeysForZero.Add(Keys.R);             // drop weapon
            controllingKeysForZero.Add(Keys.Back);          // drop weapon

			// initialization of the Zero character
            zeroCharacter = new ZeroCharacter(Content.Load<Texture2D>("images//SmileyWalkTighter"), 10, 10, 4, 4, (int)sizeOfCharacter, null);

            //// initial set of the weapons and projectile types characterisitcs
            // make new projectile type
            ProjectileType projectileType = new ProjectileType("arrow", 0.1f, 10, 20);
            // add the projectile type created above to the projectile types list
            projectilesTypes.Add(projectileType);
            // add a new weapon type to the list of weapon types
            weaponsTypes.Add(new WeaponType("bow-and-arrow", 1000, (int)sizeOfCharacter * 3, projectileType));
            // make another projectile add it to the projectiles list and add also new weapon with that projectile
            projectileType = new ProjectileType("Fireball", 0.2f, 10, 40);
            projectilesTypes.Add(projectileType);
            weaponsTypes.Add(new WeaponType("Fireball", 1500, (int)sizeOfCharacter * 5, projectileType));
            // make another projectile add it to the projectiles list and add also new weapon with that projectile
            projectileType = new ProjectileType("StarProjectile", 0.4f, 40, 100);
            projectilesTypes.Add(projectileType);
            weaponsTypes.Add(new WeaponType("StarsWeapon", 700, (int)sizeOfCharacter * 10, projectileType));

            // initialization of the creature types
            creatureTypes.Add(new CreatureType("monsterSpriteSheet", 1, 7, 3, 2000, 10, 50, 1, 100, 1, null));
            creatureTypes.Add(new CreatureType("Jelly", 2, 2, 1, 1000, 20, 70, 1, 100, 1.5f, null));
            // make new list of projectile types to which the new creature type will be resistent to
            List<ProjectileType> creatureResistentToProjectiles = new List<ProjectileType>();
            creatureResistentToProjectiles.Add(projectilesTypes[0]); // resistent to arrows
            creatureResistentToProjectiles.Add(projectilesTypes[1]); // resistent to fire
            creatureTypes.Add(new CreatureType("WalkingSquare", 1, 5, 2, 1000, 100, 150, 1, 300, 1.5f, creatureResistentToProjectiles));

            // menus initializations
            // startMenu = new StartMenu(Content, windowWidth, windowHeight);
            mainMenu = new MainMenu(Content, windowWidth, windowHeight);
            saveGameMenu = new SaveGameMenu(Content, windowWidth, windowHeight);
            loadGameMenu = new LoadGameMenu(Content, windowWidth, windowHeight, self);
            optionsMenu = new OptionsMenu(Content, windowWidth, windowHeight);

        }
        #endregion LoadContent method

        #region UnloadContent method
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion UnloadContent method

        #region Update method
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            pressedKeysFromKeyboard = Keyboard.GetState();

			// if Esc key was pressed and we now playing
            if (
                previousPressedKeysFromKeyboard.IsKeyDown(Keys.Escape) && 
                pressedKeysFromKeyboard.IsKeyUp(Keys.Escape) && 
                runningGameState == RunningGameStates.Playing
               )
            //this.Exit();
            {
				// go to the menu
                mode = Mode.MenuMode;
            }

            if (mode == Mode.GameMode)
            {
                switch (runningGameState)
                {
                    case RunningGameStates.StartScreen:
                        // load data about level 1
                        LoadCurrentLevel();
                        // wait some time in order to avoid key pressing from before starting the game or from other game state
                        timeElapsedBetweenLevels += gameTime.ElapsedGameTime.Milliseconds;
                        // if the time amount of timePauseBetweenLevels was reached and then any key was pressed
                        if (timeElapsedBetweenLevels > timePauseBetweenLevels)
                        {
                            // if any key is pressed
                            if (Keyboard.GetState().GetPressedKeys().Length > 0)
                            {
                                runningGameState = RunningGameStates.Playing;
                            }
                            timeElapsedBetweenLevels = timePauseBetweenLevels + 1;
                        }
                        break;

					// the playing state
                    case RunningGameStates.Playing: 
                        // save the previous place of game characters connected with collision issues
                        previousZeroSRectangle = zeroCharacter.CollisionRectangle;
                        for (int i = 0; i < creatures.Count; i++)
                        {
                            previousCreaturesRectangles[i] = creatures[i].CollisionRectangle;
                        }

                        //// update the Zero's state ////                    
                        zeroCharacter.Update(
                            Content,
                            gameTime,
                            pressedKeysFromKeyboard,
                            controllingKeysForZero,
                            windowWidth,
                            windowHeight,
                            weapons,
                            zeroDirection);
                        // check if Zero is dead
                        if (zeroCharacter.Life <= 0)
                        {
                            runningGameState = RunningGameStates.GameOver;
                        }
                        //update the directionOfProjectile of Zero in order to pass it to Zero's update method, so the projectiles fired in the last 
                        // directionOfProjectile of where Zero moved
                        if (previousZeroSRectangle != zeroCharacter.CollisionRectangle)
                        {
                            if (previousZeroSRectangle.X != zeroCharacter.CollisionRectangle.X)
                            {
                                zeroDirection.X =
                                    -Math.Abs(previousZeroSRectangle.X - zeroCharacter.CollisionRectangle.X) /
                                    (previousZeroSRectangle.X - zeroCharacter.CollisionRectangle.X);
                            }
                            else
                            {
                                zeroDirection.X = 0;
                            }
                            if (previousZeroSRectangle.Y != zeroCharacter.CollisionRectangle.Y)
                            {
                                zeroDirection.Y =
                                    -Math.Abs(previousZeroSRectangle.Y - zeroCharacter.CollisionRectangle.Y) /
                                    (previousZeroSRectangle.Y - zeroCharacter.CollisionRectangle.Y);
                            }
                            else
                            {
                                zeroDirection.Y = 0;
                            }
                        }

                        // update the creatures
                        foreach (Creature creature in creatures)
                        {
                            // update creature
                            creature.Update(windowWidth, windowHeight, walls, zeroCharacter, gameTime);

                            //// check if any projectile fired from Zero hits any creature ///
                            // check if Zero has a weapon to fire with
                            if (zeroCharacter.ActiveWeapon != null)
                            {
                                // goes through the projectiles fired in the moment
                                for (int i = 0; i < zeroCharacter.ActiveWeapon.Projectiles.Count; i++)
                                {
                                    // if the current projectile hits the creature
                                    if (zeroCharacter.ActiveWeapon.Projectiles[i].CollisionRectangle.Intersects(creature.CollisionRectangle))
                                    {
                                        bool foundResistentToProjectile = false;
                                        // if the creature is resistent to any projectiles
                                        if (creature.Type.ResistentToProjectiles != null)
                                        {
                                            // goes through which projectiles the creature is resistent
                                            foreach (ProjectileType projectileType in creature.Type.ResistentToProjectiles)
                                            {
                                                // if the projectile which hit the creature is resitstent 
                                                if (zeroCharacter.ActiveWeapon.Projectiles[i].Type == projectileType)
                                                {
                                                    foundResistentToProjectile = true;
                                                }
                                            }
                                        }
                                        if (!foundResistentToProjectile)
                                        {
                                            // lower the creature's maxHealth
                                            creature.Health -= zeroCharacter.ActiveWeapon.Projectiles[i].Damage;
                                            // make inactive the projectile which hit the creature
                                            zeroCharacter.ActiveWeapon.Projectiles[i].Active = false;
                                        }
                                    }
                                }
                            }
                        }

                        // removes the dead creatures
                        for (int i = creatures.Count - 1; i >= 0; i--)
                        {
                            if (creatures[i].Health <= 0)
                            {
                                creatures.RemoveAt(i);
                            }
                        }

                        // Make so that the characters can't walk through walls
                        foreach (Wall wall in walls)
                        {
                            //// check for collisions on the upper side ///
                            // creates partial rectangle only for the upper side of the wall
                            Rectangle wallSPartialRectangle = new Rectangle(
                                wall.CollisionRectangle.X,
                                wall.CollisionRectangle.Y,
                                wall.CollisionRectangle.Width,
                                wall.CollisionRectangle.Height / 2);
                            // check for Zero
                            Rectangle zeroSPartialRectangle = new Rectangle(
                                zeroCharacter.CollisionRectangle.Center.X,
                                zeroCharacter.CollisionRectangle.Y,
                                1,
                                zeroCharacter.CollisionRectangle.Height);
                            if (zeroSPartialRectangle.Intersects(wallSPartialRectangle))
                            {
                                zeroCharacter.CollisionRectangle = new Rectangle(
                                    zeroCharacter.CollisionRectangle.X,
                                    previousZeroSRectangle.Y - 1,
                                    zeroCharacter.CollisionRectangle.Width,
                                    zeroCharacter.CollisionRectangle.Height);
                            }
                            // check for the creatures
                            for (int i = 0; i < creatures.Count; i++)
                            {
                                Rectangle creautreSPartialRectangle = new Rectangle(
                                creatures[i].CollisionRectangle.Center.X,
                                creatures[i].CollisionRectangle.Y,
                                1,
                                creatures[i].CollisionRectangle.Height);
                                if (creautreSPartialRectangle.Intersects(wallSPartialRectangle))
                                {
                                    creatures[i].CollisionRectangle = new Rectangle(
                                        creatures[i].CollisionRectangle.X,
                                        previousCreaturesRectangles[i].Y - 1,
                                        creatures[i].CollisionRectangle.Width,
                                        creatures[i].CollisionRectangle.Height);
                                }
                            }

                            //// check for collisions on the bottom side ///
                            // make partial rectangle only for the bottom side of the wall
                            wallSPartialRectangle = new Rectangle(
                                wall.CollisionRectangle.X,
                                wall.CollisionRectangle.Center.Y,
                                wall.CollisionRectangle.Width,
                                wall.CollisionRectangle.Height / 2);

                            // check for Zero
                            zeroSPartialRectangle = new Rectangle(
                                zeroCharacter.CollisionRectangle.Center.X,
                                zeroCharacter.CollisionRectangle.Y,
                                1,
                                zeroCharacter.CollisionRectangle.Height);
                            if (zeroSPartialRectangle.Intersects(wallSPartialRectangle))
                            {
                                zeroCharacter.CollisionRectangle = new Rectangle(
                                    zeroCharacter.CollisionRectangle.X,
                                    previousZeroSRectangle.Y + 1,
                                    zeroCharacter.CollisionRectangle.Width,
                                    zeroCharacter.CollisionRectangle.Height);
                            }
                            // check for the creatures
                            for (int i = 0; i < creatures.Count; i++)
                            {
                                Rectangle creautreSPartialRectangle = new Rectangle(
                                    creatures[i].CollisionRectangle.Center.X,
                                    creatures[i].CollisionRectangle.Y,
                                    1,
                                    creatures[i].CollisionRectangle.Height);
                                if (creautreSPartialRectangle.Intersects(wallSPartialRectangle))
                                {
                                    creatures[i].CollisionRectangle = new Rectangle(
                                        creatures[i].CollisionRectangle.X,
                                        previousCreaturesRectangles[i].Y + 1,
                                        creatures[i].CollisionRectangle.Width,
                                        creatures[i].CollisionRectangle.Height);
                                }
                            }

                            //// check for collisions on the left side ///
                            // make partial rectangle only for the left side of the wall
                            wallSPartialRectangle = new Rectangle(
                                wall.CollisionRectangle.X,
                                wall.CollisionRectangle.Y,
                                wall.CollisionRectangle.Width / 2,
                                wall.CollisionRectangle.Height);

                            // check for Zero
                            zeroSPartialRectangle = new Rectangle(
                                zeroCharacter.CollisionRectangle.X,
                                zeroCharacter.CollisionRectangle.Center.Y,
                                zeroCharacter.CollisionRectangle.Width,
                                1);
                            if (zeroSPartialRectangle.Intersects(wallSPartialRectangle))
                            {
                                zeroCharacter.CollisionRectangle = new Rectangle(
                                    previousZeroSRectangle.X - 1,
                                    zeroCharacter.CollisionRectangle.Y,
                                    zeroCharacter.CollisionRectangle.Width,
                                    zeroCharacter.CollisionRectangle.Height);
                            }
                            // check for the creatures
                            for (int i = 0; i < creatures.Count; i++)
                            {
                                Rectangle creautreSPartialRectangle = new Rectangle(
                                    creatures[i].CollisionRectangle.X,
                                    creatures[i].CollisionRectangle.Center.Y,
                                    creatures[i].CollisionRectangle.Width,
                                    1);
                                if (creautreSPartialRectangle.Intersects(wallSPartialRectangle))
                                {
                                    creatures[i].CollisionRectangle = new Rectangle(
                                        previousCreaturesRectangles[i].X - 1,
                                        creatures[i].CollisionRectangle.Y,
                                        creatures[i].CollisionRectangle.Width,
                                        creatures[i].CollisionRectangle.Height);
                                }
                            }

                            //// check for collisions on the right side ///
                            // make partial rectangle only for the right side of the wall
                            wallSPartialRectangle = new Rectangle(
                                wall.CollisionRectangle.Center.X,
                                wall.CollisionRectangle.Y,
                                wall.CollisionRectangle.Width / 2,
                                wall.CollisionRectangle.Height);

                            // check for Zero
                            zeroSPartialRectangle = new Rectangle(
                                zeroCharacter.CollisionRectangle.X,
                                zeroCharacter.CollisionRectangle.Center.Y,
                                zeroCharacter.CollisionRectangle.Width,
                                1);
                            if (zeroSPartialRectangle.Intersects(wallSPartialRectangle))
                            {
                                zeroCharacter.CollisionRectangle = new Rectangle(
                                    previousZeroSRectangle.X + 1,
                                    zeroCharacter.CollisionRectangle.Y,
                                    zeroCharacter.CollisionRectangle.Width,
                                    zeroCharacter.CollisionRectangle.Height);
                            }
                            // check for the creatures
                            for (int i = 0; i < creatures.Count; i++)
                            {
                                Rectangle creautreSPartialRectangle = new Rectangle(
                                    creatures[i].CollisionRectangle.X,
                                    creatures[i].CollisionRectangle.Center.Y,
                                    creatures[i].CollisionRectangle.Width,
                                    1);
                                if (creautreSPartialRectangle.Intersects(wallSPartialRectangle))
                                {
                                    creatures[i].CollisionRectangle = new Rectangle(
                                        previousCreaturesRectangles[i].X + 1,
                                        creatures[i].CollisionRectangle.Y,
                                        creatures[i].CollisionRectangle.Width,
                                        creatures[i].CollisionRectangle.Height);
                                }
                            }

                            // check for each projectiles fired from Zero and remove it if it hit the wall
                            if (zeroCharacter.ActiveWeapon != null)
                            {
                                for (int i = 0; i < zeroCharacter.ActiveWeapon.Projectiles.Count; i++)
                                {
                                    if (zeroCharacter.ActiveWeapon.Projectiles[i].CollisionRectangle.Intersects(new Rectangle(
                                        wall.CollisionRectangle.Center.X - wall.CollisionRectangle.Width / 4,
                                        wall.CollisionRectangle.Center.Y - wall.CollisionRectangle.Height / 4,
                                        wall.CollisionRectangle.Width / 2,
                                        wall.CollisionRectangle.Height / 2)))
                                    {
                                        zeroCharacter.ActiveWeapon.Projectiles[i].Active = false;
                                    }
                                }
                            }
                        }

                        // removes the inactive projectiles
                        if (zeroCharacter.ActiveWeapon != null)
                        {
                            for (int i = zeroCharacter.ActiveWeapon.Projectiles.Count - 1; i >= 0; i--)
                            {
                                if (!zeroCharacter.ActiveWeapon.Projectiles[i].Active)
                                {
                                    zeroCharacter.ActiveWeapon.Projectiles.RemoveAt(i);
                                }
                            }
                        }
                        // check if zero reached the end of the level
                        if (zeroCharacter.CollisionRectangle.Intersects(endOfLevel.CollisionRectangle))
                        {
                            if (currentLevel >= numberOfLevels)
                            {
                                runningGameState = RunningGameStates.GameFinished;
                            }
                            else
                            {
                                runningGameState = RunningGameStates.FinishedLevelScreen;
                            }
                        }
                        break;

                    case RunningGameStates.FinishedLevelScreen:
                        // wait some time in order to avoid key pressing from the playing state
                        timeElapsedBetweenLevels += gameTime.ElapsedGameTime.Milliseconds;
                        // if the time amount of timePauseBetweenLevels was reached and then any key was pressed
                        if (timeElapsedBetweenLevels > timePauseBetweenLevels)
                        {
                            finishedLevelStrings[0] = "< PRESS A KEY WHEN READY ! >";
                            if (Keyboard.GetState().GetPressedKeys().Length > 0)
                            {
                                // load the next level
                                currentLevel++;
                                LoadCurrentLevel();
                                // set initial position for Zero
                                zeroCharacter.SetInitialLevelPosition(10, 10, (int)sizeOfCharacter);
                                // start the level
                                runningGameState = RunningGameStates.Playing;
                                finishedLevelStrings[0] = "";
                                timeElapsedBetweenLevels = 0;
                            }
                        }
                        break;

                    case RunningGameStates.GameOver:
                        // wait some time in order to avoid key pressing from the playing state
                        timeElapsedBetweenLevels += gameTime.ElapsedGameTime.Milliseconds;
                        // if the time amount of timePauseBetweenLevels was reached and then any key was pressed
                        if (timeElapsedBetweenLevels > timePauseBetweenLevels)
                        {
                            finishedLevelStrings[0] = "< PRESS A KEY TO START OVER ! >";
                            if (Keyboard.GetState().GetPressedKeys().Length > 0)
                            {
                                // load the first level
                                currentLevel = 1;
                                LoadCurrentLevel();
                                // set initial position for Zero
                                zeroCharacter.SetInitialLevelPosition(10, 10, (int)sizeOfCharacter);
                                // removes Zero's weapon
                                zeroCharacter.ActiveWeapon = null;

                                finishedLevelStrings[0] = "";
                                timeElapsedBetweenLevels = 0;
                                // start the game
                                runningGameState = RunningGameStates.StartScreen;
                            }
                        }
                        break;

                    case RunningGameStates.GameFinished:
                        // wait some time in order to avoid key pressing from the playing state
                        timeElapsedBetweenLevels += gameTime.ElapsedGameTime.Milliseconds;
                        // if the time amount of timePauseBetweenLevels was reached and then any key was pressed
                        if (timeElapsedBetweenLevels > timePauseBetweenLevels*5)
                        {
                            // finishedLevelStrings[0] = "< PRESS A KEY TO START OVER ! >";
                            if (Keyboard.GetState().GetPressedKeys().Length > 0)
                            {
                                // load the first level
                                currentLevel = 1;
                                LoadCurrentLevel();
                                // set initial position for Zero
                                zeroCharacter.SetInitialLevelPosition(10, 10, (int)sizeOfCharacter);
                                // removes Zero's weapon
                                zeroCharacter.ActiveWeapon = null;

                                finishedLevelStrings[0] = "";
                                timeElapsedBetweenLevels = 0;
                                // start the game
                                runningGameState = RunningGameStates.StartScreen;
                            }
                        }
                        break;
                } // end of switch (runningGameState)
            } // end of if(mode)
            else // mode is Mode.MenuMode; ToDo: change this line in more states of Mode available
            {
                // set menus active only in playing state of the game
                if (runningGameState == RunningGameStates.Playing)
                {
                    previousRunningMenuState = runningMenuState;
                    switch(runningMenuState)
                    {
                        case RunningMenuStates.MainMenu:
                            mainMenu.Update(
                                pressedKeysFromKeyboard, 
                                ref runningGameState, 
                                ref mode, 
                                ref runningMenuState, 
                                saveGameMenu,
                                loadGameMenu);
                            break;
                        case RunningMenuStates.SaveGameMenu:
                            if (true)  // the program alays goes in this block - the definitions below only needed for this case
                                       // in order to pass them to the save game menu method
                            {
                                if (previousRunningMenuState == RunningMenuStates.MainMenu)
                                {
                                    saveGameMenu = new SaveGameMenu(Content, windowWidth, windowHeight);
                                }

                                List<Vector2> creaturesPosition = new List<Vector2>();
                                List<CreatureType> creaturesType = new List<CreatureType>();
                                List<float> creaturesLives = new List<float>();
                                List<int> creaturesDamages = new List<int>();
                                for (int i = 0; i < creatures.Count; i++)
                                {
                                    creaturesPosition.Add(new Vector2(creatures[i].CollisionRectangle.X, creatures[i].CollisionRectangle.Y));
                                    creaturesType.Add(creatures[i].Type);
                                    creaturesLives.Add(creatures[i].Health);
                                    creaturesDamages.Add(creatures[i].Damage);
                                }
                                List<WeaponType> weaponsTypes = new List<WeaponType>();
                                List<Vector2> weaponsPositions = new List<Vector2>();
                                List<bool> weaponsOwned = new List<bool>();
                                for (int i = 0; i < weapons.Count; i++)
                                {
                                    weaponsTypes.Add(weapons[i].Type);
                                    weaponsPositions.Add(new Vector2(weapons[i].CollisionRectangle.Center.X, weapons[i].CollisionRectangle.Center.Y));
                                    weaponsOwned.Add(weapons[i].IsOwned);
                                }
                                saveGameMenu.Update
                                    (
                                     gameTime,
                                     pressedKeysFromKeyboard,
                                     ref mode,
                                     ref runningMenuState,
                                     currentLevel,
                                     new Vector2(zeroCharacter.CollisionRectangle.X, zeroCharacter.CollisionRectangle.Y),
                                     zeroCharacter.Life,
                                     zeroCharacter.MaximumLife,
                                     zeroCharacter.SkillToFireWithWeapon,
                                     zeroCharacter.ActiveWeapon,
                                     controllingKeysForZero,
                                     creaturesPosition,
                                     creaturesType,
                                     creaturesLives,
                                     creaturesDamages,
                                     weaponsTypes,
                                     weaponsPositions,
                                     weaponsOwned,
                                     windowWidth,
                                     windowHeight
                                    );
                            }
                            break;

                        case RunningMenuStates.LoadGameMenu:
                            if (true)  // we goes for sure in this block, the idea is that we need the definitions only for this case
                            {
                                if (previousRunningMenuState == RunningMenuStates.MainMenu)
                                {
                                    loadGameMenu = new LoadGameMenu(Content, windowWidth, windowHeight, self);
                                }
                                
                                loadGameMenu.Update
                                    (
                                     pressedKeysFromKeyboard,
                                     ref mode,
                                     ref runningMenuState,
                                     ref currentLevel,
                                     ref zeroCharacter,
                                     ref controllingKeysForZero,
                                     ref creatures,
                                     ref weapons,
                                     ref windowWidth,
                                     ref windowHeight,
                                     self,
                                     Content
                                    );

                            }
                            break;

                        case RunningMenuStates.OptionsMenu:
                            optionsMenu.Update
                                (
                                 ref graphics, 
                                 pressedKeysFromKeyboard, 
                                 ref windowWidth, 
                                 ref windowHeight, 
                                 controllingKeysForZero, 
                                 ref mode, 
                                 ref runningMenuState,
                                 zeroCharacter,
                                 walls,
                                 creatures,
                                 weapons,
                                 endOfLevel
                                );
                            break;
                    }
                }
            }
            previousPressedKeysFromKeyboard = pressedKeysFromKeyboard;
            base.Update(gameTime);
        }
        #endregion Update method

        #region Draw method
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            // draws the game objects
            spriteBatch.Begin();

            switch (runningGameState)
            {
                case RunningGameStates.StartScreen:

                    // draw the start screen of the game
                    spriteBatch.Draw
                        (
                         Content.Load<Texture2D>("images//startscreenimage"), 
                         new Rectangle(0, 0, windowWidth, windowHeight), 
                         Color.White
                        );
                    //if (mode == Mode.MenuMode)
                    //{
                    //    startMenu.Draw(spriteBatch);
                    //}
                    break;

                case RunningGameStates.Playing:

                    // draw the background
                    spriteBatch.Draw(currectLevelBackgroundTexture, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);

                    // Zero draw himself
                    zeroCharacter.Draw(spriteBatch);
                    // draw Zero's life if there are any creatures alive
                    if (creatures.Count > 0)
                    {
                        spriteBatch.Draw(
                            Content.Load<Texture2D>("images//lifeBar"),
                            new Rectangle(
                                zeroCharacter.CollisionRectangle.X,
                                zeroCharacter.CollisionRectangle.Y,
                                zeroCharacter.CollisionRectangle.Width * zeroCharacter.Life / zeroCharacter.MaximumLife,
                                zeroCharacter.CollisionRectangle.Height / 10
                                ),
                            Color.White);
                    }
                    // the walls draw themselves
                    foreach (Wall wall in walls)
                    {
                        wall.Draw(spriteBatch);
                    }
                    // the weapons draw themselves
                    foreach (Weapon weapon in weapons)
                    {
                        weapon.Draw(spriteBatch);
                    }

                    // the end of level object draw itself
                    endOfLevel.Draw(spriteBatch);

                    // creatures draw themselves
                    foreach (Creature creature in creatures)
                    {
                        creature.Draw(spriteBatch);
                    }

                    //// Draw any necesary info on screen ////
                    // draw which level is
                    // spriteBatch.DrawString(font, "Labyrinth# " + currentLevel, new Vector2(10, 10), Color.Black);
                    // draw which weapon Zero currently have
                    if (zeroCharacter.ActiveWeapon != null)
                    {
                        spriteBatch.Draw(
                            zeroCharacter.ActiveWeapon.Texture, 
                            new Rectangle(
                                200,
                                5,
                                zeroCharacter.ActiveWeapon.CollisionRectangle.Width/2,
                                zeroCharacter.ActiveWeapon.CollisionRectangle.Height/2), 
                            Color.White);
                    }

                    // draw menus if Mode.MenuMode is active
                    if (mode == Mode.MenuMode)
                    {
                        switch (runningMenuState)
                        {
                            case RunningMenuStates.MainMenu:
                                mainMenu.Draw(spriteBatch);
                                break;
                            case RunningMenuStates.SaveGameMenu:
                                saveGameMenu.Draw(spriteBatch);
                                break;
                            case RunningMenuStates.LoadGameMenu:
                                loadGameMenu.Draw(spriteBatch);
                                break;
                            case RunningMenuStates.OptionsMenu:
                                optionsMenu.Draw(spriteBatch);
                                break;
                        }
                    }

                    break;

                case RunningGameStates.FinishedLevelScreen:

                    // draw the end of the level screen
                    spriteBatch.Draw
                        (
                         Content.Load<Texture2D>("images//finishedlevelscreenimage"),
                         new Rectangle(0, 0, windowWidth, windowHeight),
                         Color.White
                        );
                    DrawCenteredTextsOnScreen(font, finishedLevelStrings);

                    break;

                case RunningGameStates.GameOver:

                    // draw the end of the level screen
                    spriteBatch.Draw
                        (
                         Content.Load<Texture2D>("images//gameoverscreenimage"),
                         new Rectangle(0, 0, windowWidth, windowHeight),
                         Color.White
                        );
                    DrawCenteredTextsOnScreen(font, finishedLevelStrings);

                    break;

                case RunningGameStates.GameFinished:

                    // draw the End of the game screen
                    spriteBatch.Draw
                        (
                         Content.Load<Texture2D>("images//finalscreenimage"),
                         new Rectangle(0, 0, windowWidth, windowHeight),
                         Color.White
                        );

                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion Draw method

        #region LoadCurrentLevel method
        /// <summary>
        /// Load data from the LevelsInfo.dat file about the current level and set the variables. 
        /// The method uses the global for Game1 class variable currentLevel to determine which level's data to load.
        /// The LevelsInfo.dat file must contains data in specific order in order data to be loaded properly by the method.
        /// </summary>
        public void LoadCurrentLevel()
        {
            // levels specific variables
            const int MAX_NUMBERS_OF_COLUMNS_IN_LEVEL = 100;
            const int MAX_NUMBERS_OF_ROWS_IN_LEVEL = 100;
            int[,] stateOfLevel = new int[MAX_NUMBERS_OF_COLUMNS_IN_LEVEL, MAX_NUMBERS_OF_ROWS_IN_LEVEL];
            char[] buffer = new char[10];
            string whatWasReadFromFile;

            // open the file with info for the levels
            using (var stream = TitleContainer.OpenStream("LevelsInfo.dat"))
            {
                using (var textReader = new StreamReader(stream))
                {
                    //// read the numbers of levels ////
                    //read the line with comment
                    textReader.ReadLine();
                    // read the numbers of levels
                    textReader.Read(buffer, 0, 2);
                    whatWasReadFromFile = new string(buffer);
                    numberOfLevels = Int32.Parse(whatWasReadFromFile);
                    // read the rest of the line
                    textReader.ReadLine();

                    // enter in a loop where overwrite the data for each level so remain only the level equal to currentLevel
                    for (int i = 1; i <= currentLevel; i++)
                    {
                        // clear the walls
                        walls.Clear();
                        // clear the creatures
                        creatures.Clear();
                        // remove all the weapons from the list of weapons which are not currently owned by someone
                        for (int j = weapons.Count - 1; j >= 0; j--)
                        {
                            if (!weapons[j].IsOwned)
                            {
                                weapons.RemoveAt(j);
                            }
                        }
                        // clear all of the projectiles fired from Zero
                        for (int j = weapons.Count - 1; j >= 0; j--)
                        {
                            if (weapons[j].IsOwned)
                            {
                                weapons[j].Projectiles.Clear();
                            }
                        }

                        //// read the level No. ////
                        //read the comment line
                        textReader.ReadLine();
                        // read the levels No.
                        textReader.Read(buffer, 0, 2);
                        whatWasReadFromFile = new string(buffer);
                        int levelNo = Int32.Parse(whatWasReadFromFile);
                        // read the rest of the line
                        textReader.ReadLine();

                        //// read name for the texture for the background ////
                        //read the comment line
                        textReader.ReadLine();
                        // read the name and load it to the global for Game1 texture for the background level
                        string currentLevelBackgroundName = textReader.ReadLine();
                        currectLevelBackgroundTexture = Content.Load<Texture2D>("images//" + currentLevelBackgroundName);

                        //// read name for the texture of walls ////
                        //read the comment line
                        textReader.ReadLine();
                        // read the name
                        string textureOfWall = textReader.ReadLine();

                        //// read the index for the type of weapon ////
                        //read the comment line
                        textReader.ReadLine();
                        // read the index for the type of weapon 
                        textReader.Read(buffer, 0, 2);
                        whatWasReadFromFile = new string(buffer);
                        int indexForCreaturesType = Int32.Parse(whatWasReadFromFile);
                        // read the rest of the line
                        textReader.ReadLine();

                        //// read the index for the type of weapon ////
                        //read the comment line
                        textReader.ReadLine();
                        // read the index for the type of weapon 
                        textReader.Read(buffer, 0, 2);
                        whatWasReadFromFile = new string(buffer);
                        int indexForWeaponType = Int32.Parse(whatWasReadFromFile);
                        // read the rest of the line
                        textReader.ReadLine();

                        //// read the numbers of columnsForAnimation for the level table////
                        //read the comment line
                        textReader.ReadLine();
                        // read the numbers of columnsForAnimation
                        textReader.Read(buffer, 0, 2);
                        whatWasReadFromFile = new string(buffer);
                        int numberOfColumnsForLevel = Int32.Parse(whatWasReadFromFile);
                        // read the rest of the line
                        textReader.ReadLine();

                        //// read the numbers of rowsForAnimation for the level table ////
                        //read the comment line
                        textReader.ReadLine();
                        // read the numbers of rowsForAnimation
                        textReader.Read(buffer, 0, 2);
                        whatWasReadFromFile = new string(buffer);
                        int numberOfRowsForLevel = Int32.Parse(whatWasReadFromFile);
                        // read the rest of the line
                        textReader.ReadLine();

                        //// set the size of Zero and the other characters for this level ////
                        float width = 0.5f * windowWidth / numberOfColumnsForLevel;
                        float height = 0.5f * windowHeight / numberOfRowsForLevel;
                        // takes the bigger than the two options
                        if (width > height)
                            sizeOfCharacter = width;
                        else
                            sizeOfCharacter = height;

                        //// read the table with info about the current level ////
                        //read the comment line
                        textReader.ReadLine();
                        for (int j = 0; j < numberOfRowsForLevel; j++)
                        {
                            for (int k = 0; k < numberOfColumnsForLevel; k++)
                            {
                                textReader.Read(buffer, 0, 2);
                                whatWasReadFromFile = new string(buffer);
                                stateOfLevel[j, k] = Int32.Parse(whatWasReadFromFile);
                            }
                            // read the rest of the line
                            textReader.ReadLine();
                        }

                        // makes new objects for the level with the read from the file data 
                        // and add them to apropriate lists of objects
                        for (int j = 0; j < numberOfRowsForLevel; j++)
                        {
                            for (int k = 0; k < numberOfColumnsForLevel; k++)
                            {
                                int widthOfLevelObject = windowWidth / numberOfColumnsForLevel;
                                int heightOfLevelObject = windowHeight / numberOfRowsForLevel;
                                switch (stateOfLevel[j, k])
                                {
                                    case 1:
                                        // set a wall on [j-th,k-th] place on this level
                                        Wall wall = new Wall(Content.Load<Texture2D>("images//" + textureOfWall),
                                            k * widthOfLevelObject,
                                            j * heightOfLevelObject,
                                            widthOfLevelObject,
                                            heightOfLevelObject);
                                        walls.Add(wall);
                                        break;
                                    case 2:
                                        // set a creature on [j-th,k-th] place on this level
                                        Creature creature = new Creature(
                                            creatureTypes[indexForCreaturesType], 
                                            Content, 
                                            k * widthOfLevelObject, 
                                            j * heightOfLevelObject, 
                                            (int)sizeOfCharacter);
                                        creatures.Add(creature);
                                        break;
                                    case 3:
                                        // set a weapon on [j-th,k-th] place on this level
                                        Weapon weapon = new Weapon(
                                            Content,
                                            weaponsTypes[indexForWeaponType],
                                            k * widthOfLevelObject,
                                            j * heightOfLevelObject,
                                            (int)sizeOfCharacter,
                                            false);
                                        weapons.Add(weapon);
                                        break;
                                    case 9:
                                        //set the end of the level on [j-th,k-th] place on this level
                                        endOfLevel = new EndOfLevel
                                            (
                                             Content.Load<Texture2D>("images//mushroomHouse"),
                                             k * widthOfLevelObject,
                                             j * heightOfLevelObject,
                                             widthOfLevelObject,
                                             heightOfLevelObject
                                            );
                                        break;
                                }
                            }
                        }
                    }
                    textReader.Close();
                }
            }
        }

        #endregion LoadCurrentLevel method

        #region private methods

        /// <summary>
        /// Draws strings centered on screen
        /// </summary>
        /// <param name="font">the font used</param>
        /// <param name="strings">the array of strings</param>
        private void DrawCenteredTextsOnScreen(SpriteFont font, string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                Vector2 ceneteringStringVector = font.MeasureString(strings[i]);
                spriteBatch.DrawString(
                    font,
                    strings[i],
                    new Vector2((windowWidth / 2) + 2, (windowHeight / 2) + 2 + i * 30),
                    Color.Black,
                    0.0f,
                    new Vector2(ceneteringStringVector.X / 2, ceneteringStringVector.Y / 2),
                    1.0f,
                    SpriteEffects.None,
                    0.0f);
            }
        }

        

        #endregion private methods
    }
       
}
