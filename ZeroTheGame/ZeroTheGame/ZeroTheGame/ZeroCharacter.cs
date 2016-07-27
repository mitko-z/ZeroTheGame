using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ZeroTheGame
{
    public class ZeroCharacter
    {
        #region fields

        //// drawing and animation fields ////
        // the image atlas
        Texture2D texture;
        // where on the game window to put the character
        Rectangle destinationRectangle;
        // which frame from the texture atlas to show
        Rectangle sourceRectangle;
        // field for flipping horizontally the character when moving left and right
        SpriteEffects flipTheSprite;
        // how many rows has the texture atlas
        int rowsForAnimation;
        // how many columns has the texture atlas
        int columnsForAnimation;
        // which is the current image from the texture atlas
        int currentFrame;
        // how many images there are on the atlas
        int totalFrames;
        // with what speed the character move
        int speed;
        // if the character must move
        bool isActionNeeded = false;

        //// Health fields ///
        int maximumLife = 100; 
        int life;

        //// fighting fields ////
        // what is the weapon Zero curently wear and fire with
        Weapon activeWeapon;
        // how skilled he is in firing
        float skillToFireWithWeapon = 0.96f; // max 1
        // when command to firing is set
        bool startToFire;

        Random randomNumberGenerator = new Random();
        #endregion fields

        #region constructors

        private ZeroCharacter() { }

        /// <summary>
        /// Create the Zero character
        /// </summary>
        /// <param name="texture">the texture atlas - an image with all frames needed for animating Zero</param>
        /// <param name="x">the x coordinate of the center of the animation</param>
        /// <param name="y">the y coordinate of the center of the animation</param>
        /// <param name="rowsForAnimation">how many rowsForAnimation has the texture atlas</param>
        /// <param name="columnsForAnimation">how many columnsForAnimation has the texture atlas</param>
        /// <param name="size">the size of Zero for the current level</param>
        /// <param name="weapon">point to the weapon which Zero wear, null if none</param>
        public ZeroCharacter(Texture2D texture, int x, int y, int rowsForAnimation, int columnsForAnimation, float size, Weapon weapon)
        {
            this.texture = texture;

            // set where initially the character should be
            destinationRectangle = new Rectangle(x, y, (int)size, (int)(size / 2));

            // set the drawing and animation characteristics
            currentFrame = 0;
            totalFrames = rowsForAnimation * columnsForAnimation;
            this.rowsForAnimation = rowsForAnimation;
            this.columnsForAnimation = columnsForAnimation;
            this.speed = 5;
            flipTheSprite = SpriteEffects.None;

            // set the maxHealth characteristics
            life = maximumLife;

            // set the fighting characteristics
            activeWeapon = weapon;
            startToFire = false;
        }

        #endregion constructor

        #region properties

        /// <summary>
        /// get and set the rectangle needed for collisions issues
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return destinationRectangle; }
            set { destinationRectangle = value; }
        }
        /// <summary>
        /// set and get whether the character to be animated
        /// </summary>
        public bool IsActionNeeded
        {
            set { isActionNeeded = value; }
            get { return isActionNeeded; }
        }

        /// <summary>
        /// get how skilled the character is
        /// </summary>
        public float SkillToFireWithWeapon
        {
            get { return skillToFireWithWeapon; }
            set { skillToFireWithWeapon = value; }
        }

        /// <summary>
        /// Set and get Zero's life. It cannot be set more than Zero.MaximumLife or less than 0
        /// </summary>
        public int Life
        {
            set
            {
                if (value > maximumLife)
                {
                    life = maximumLife;
                }

                if (value < 0)
                {
                    life = 0;
                }
                else
                {
                    life = value;
                }
            }
            get { return life; }
        }

        /// <summary>
        /// Set and get Zero's Maximum life points
        /// </summary>
        public int MaximumLife
        {
            set { maximumLife = value; }
            get { return maximumLife; }
        }

        /// <summary>
        /// point to the weapon which Zero wear, null if none
        /// </summary>
        public Weapon ActiveWeapon
        {
            set { activeWeapon = value; }
            get { return activeWeapon; }
        }

        public bool StartToFire
        {
            get { return startToFire; }
        }

        #endregion properties

        #region public methods

        public void SetInitialLevelPosition(int x, int y, float size)
        {
            // set where initially the character should be
            destinationRectangle = new Rectangle(x, y, (int)size, (int)(size / 2));

            // set the maxHealth characteristics
            life = maximumLife;

            // set the fighting characteristics
            startToFire = false;

        }

        /// <summary>
        /// updates the state of Zero
        /// </summary>
        /// <param name="pressedKeysFromKeyboard">the keyboard state</param>
        /// <param name="controllingKeys">list of pressedKeysFromKeyboard which controls the actions of Zero.
        /// they must be defined in pairs in this strict order:
        /// 0 and 1 - move up
        /// 2, 3 - move down
        /// 4, 5 - left
        /// 6, 7 - right
        /// 8, 9 - fire (inactive fire for now; just movement)
        /// 10, 11 - take a weapon from the ground</param>
        /// <param name="windowWidth">how big is the game window's width</param>
        /// <param name="windowHeight">how big is the game window's height</param>
        public void Update
            (ContentManager content, 
            GameTime gametime, 
            KeyboardState keys, 
            List<Keys> controllingKeys, 
            int windowWidth, 
            int windowHeight, 
            List<Weapon> weapons, 
            Vector2 firingDirection)
        {
            keys = Keyboard.GetState();
            isActionNeeded = false;
            foreach (Keys key in controllingKeys)
            {
                if (keys.IsKeyDown(key))
                {
                    isActionNeeded = true;
                }
            }
            if (isActionNeeded)
            {
                currentFrame++;
                if (currentFrame > totalFrames)
                {
                    currentFrame = 0;
                }
                // check if pressedKeysFromKeyboard for moving up are pressed
                if (keys.IsKeyDown(controllingKeys[0]) || keys.IsKeyDown(controllingKeys[1]))
                {
                    destinationRectangle.Y -= speed;
                }
                // check if pressedKeysFromKeyboard for moving down are pressed
                if (keys.IsKeyDown(controllingKeys[2]) || keys.IsKeyDown(controllingKeys[3]))
                {
                    destinationRectangle.Y += speed;
                }
                // check if pressedKeysFromKeyboard for moving left are pressed
                if (keys.IsKeyDown(controllingKeys[4]) || keys.IsKeyDown(controllingKeys[5]))
                {
                    destinationRectangle.X -= speed;
                    // setting flip the texture so the Zero's image to look to left
                    flipTheSprite = SpriteEffects.FlipHorizontally;
                }
                // check if pressedKeysFromKeyboard for moving right are pressed
                if (keys.IsKeyDown(controllingKeys[6]) || keys.IsKeyDown(controllingKeys[7]))
                {
                    destinationRectangle.X += speed;
                    // set to none flipping (if any previous has been made)
                    flipTheSprite = SpriteEffects.None;
                }
                // check if pressedKeysFromKeyboard for firing with weapon are pressed
                if (keys.IsKeyDown(controllingKeys[8]) || keys.IsKeyDown(controllingKeys[9]))
                {
                    // if zero dont have a weapon set
                    if (activeWeapon != null)
                    {
                        startToFire = true;
                        skillToFireWithWeapon += randomNumberGenerator.Next(1) / 100;
                        if (skillToFireWithWeapon > 1)
                        {
                            skillToFireWithWeapon = 1;
                        }
                    }
                }
                // check if pressedKeysFromKeyboard for taking an item are pressed
                if (keys.IsKeyDown(controllingKeys[10]) || keys.IsKeyDown(controllingKeys[11]))
                {
                    foreach (Weapon weapon in weapons)
                    //for (int i = 0; i < weapons.Count; i++)
                    {
                        if (weapon.CollisionRectangle.Intersects(destinationRectangle) && activeWeapon == null && !weapon.IsOwned)
                        {
                            activeWeapon = weapon;
                            activeWeapon.IsOwned = true;
                            //weapons.RemoveAt(i);
                        }
                    }
                }
                if (keys.IsKeyDown(controllingKeys[12]) || keys.IsKeyDown(controllingKeys[13]))
                {
                    if (activeWeapon != null)
                    {
                        activeWeapon.IsOwned = false;
                        activeWeapon = null;
                    }
                }
            }
            //reset animation if no action needed
            else
            {
                currentFrame = 0;
            }
            SetSourceRectangle();

            // update weapon's state
            if (activeWeapon != null)
            {
                activeWeapon.Update
                    (content,
                    gametime,
                    new Vector2(destinationRectangle.X, destinationRectangle.Y),
                    startToFire,
                    firingDirection,
                    (1 - skillToFireWithWeapon),
                    windowWidth,
                    windowHeight
                    );
                startToFire = false;
            }
            // keeps the character in the game windows
            if (destinationRectangle.X < 0)
                destinationRectangle.X = 0;
            if (destinationRectangle.X + destinationRectangle.Width > windowWidth)
                destinationRectangle.X = windowWidth - destinationRectangle.Width;
            if (destinationRectangle.Y < 0)
                destinationRectangle.Y = 0;
            if (destinationRectangle.Y + destinationRectangle.Height > windowHeight)
                destinationRectangle.Y = windowHeight - destinationRectangle.Height;
        }

        /// <summary>
        /// Draw Zero
        /// </summary>
        /// <param name="spritebatch">the texture batch</param>
        public void Draw(SpriteBatch spritebatch)
        {
            // draw Zero
            spritebatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White, 0, new Vector2(0,0), flipTheSprite, 0);
        }

        #endregion public methods

        #region private methods

        private void SetSourceRectangle()
        {
            int width = texture.Width / columnsForAnimation;
            int height = texture.Height / rowsForAnimation;
            int row = (int)((float)currentFrame / (float)columnsForAnimation);
            int column = currentFrame % columnsForAnimation;

            sourceRectangle = new Rectangle(width * column, height * row, width, height);
        }

        #endregion private methods

    }
}
