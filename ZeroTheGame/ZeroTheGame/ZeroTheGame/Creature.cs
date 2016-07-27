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
    public class Creature
    {
        #region fields

        CreatureType type;

        // drawing variables
        Texture2D texture;
        Rectangle destinationRectangle;
        Rectangle sourceRectangle;
        SpriteEffects flipTheSprite;

        // if the creature must move
        bool isMovementAnimationNeeded;
        Vector2 moveToPoint;

        // variables for the animation
        int currentFrame;
        int totalFrames;

        // attack fields
        int damage;
        int elapsedTimeFromAnAttack;

        // maxHealth fields
        float health;

        // random number generator
        Random randomNumberGenerator = new Random();

        #endregion fields

        #region constructors

        private Creature() { }

        /// <summary>
        /// create a creature
        /// </summary>
        /// <param name="type">the type of the creature</param>
        /// <param name="content">the content manager</param>
        /// <param name="x">the x coordinate of the center of the creature</param>
        /// <param name="y">the y coordinate of the center of the creature</param>
        /// <param name="size">the size of the creature for the current level</param>
        public Creature(CreatureType type, ContentManager content, int x, int y, float size)
        {
            // set the type of the creature
            this.type = type;

            // set the texture (or texture atlas)
            this.texture = content.Load<Texture2D>("images//" + type.NameOfTexture);

            // set where initially the character should be
            destinationRectangle = new Rectangle(x, y, (int)(size*type.Size), (int)(2*size*type.Size/3));

            currentFrame = 0;
            totalFrames = type.RowsForAnimation * type.ColumnsForAnimation;

            flipTheSprite = SpriteEffects.None;

            // set the damage
            damage = randomNumberGenerator.Next((int)type.MinDamage, (int)type.MinDamage);
            // set the attack rate
            elapsedTimeFromAnAttack = 0;

            // set the health
            health = type.MaxHealth;
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
            set { isMovementAnimationNeeded = value; }
            get { return isMovementAnimationNeeded; }
        }

        /// <summary>
        /// how much damage make on each attack
        /// </summary>
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public float Health
        {
            set { health = value; }
            get { return health; }
        }

        public CreatureType Type
        {
            set { type = value; }
            get { return type; }
        }

        #endregion properties

        #region public methods

        /// <summary>
        /// Update the state of the creature
        /// </summary>
        /// <param name="windowWidth">how big is the game window's width</param>
        /// <param name="windowHeight">how big is the game window's height</param>
        /// <param name="walls">the walls defined in Game1.cs</param>
        /// <param name="zeroCharacter">The Zero's object</param>
        public void Update(int windowWidth, int windowHeight, List<Wall> walls, ZeroCharacter zeroCharacter, GameTime gameTime)
        {
            //// check if Zero can be seen around
            // define and places where the creature to look to
            List<Rectangle> placesToLookTo = new List<Rectangle>();
            // set the places where to look to - it makes two "circles" around the creature; the second is twice bigger
            // than the first one (thanks to the multiplier).
            // it is not clear circles but just it is on its right (when angle is 0 then cos(angle) is 1 and sin(angle) 
            // is 0, so x goes on right, y is the same), on its bottom (angle = PI/2, cos = 0, sin = 1), on its left and top
            for (int multiplier = 1; multiplier < 3; multiplier++)
            {
                for (double angle = 0; angle < 2 * Math.PI; angle += 0.1)
                {
                    placesToLookTo.Add(new Rectangle(
                        (int)(destinationRectangle.Center.X + 1.5 * destinationRectangle.Width * Math.Cos(angle) * multiplier),
                        (int)(destinationRectangle.Center.Y + 1.5 * destinationRectangle.Height * Math.Sin(angle) * multiplier),
                        destinationRectangle.Width,
                        destinationRectangle.Height));
                }
            }

            //int additionalPlacesToLookTo = 4; // additional variable used in helping if there is a wall nearby
            bool foundZero = false;

            for (int i = 0; i < placesToLookTo.Count; i++)
            {
                // check every wall in this level
                foreach (Wall wall in walls)
                {
                    // if on this place there is no wall
                    if (!placesToLookTo[i].Intersects(wall.CollisionRectangle))
                    {
                        // look if Zero is here
                        if (placesToLookTo[i].Intersects(zeroCharacter.CollisionRectangle))
                        {
                            foundZero = true;
                        }
                    }
                    //else
                    //{
                    //    // but if there is a wall on this place, then the creature cannot see 
                    //    // through a wall - a place with index +4 must be removed
                    //    if (i < 4)
                    //    {
                    //        placesToLookTo.RemoveAt(i + additionalPlacesToLookTo);
                    //        additionalPlacesToLookTo--; // on the previous line a place was removed from the list with places
                    //                                    // so additionalPlacesToLookTo must be substracted
                    //    }
                    //}
                    
                }
                
            }
            if (foundZero)
            {
                SetMovementToTarget(new Vector2(zeroCharacter.CollisionRectangle.Center.X, zeroCharacter.CollisionRectangle.Center.Y));
            }
            else
            {
                isMovementAnimationNeeded = false;
            }
            // update the time for attack
            elapsedTimeFromAnAttack += gameTime.ElapsedGameTime.Milliseconds;

            // check if the creature has reached to its target
            if (destinationRectangle.Intersects(new Rectangle((int)moveToPoint.X, (int)moveToPoint.Y, 1,1)))
            {
                isMovementAnimationNeeded = false;
                if (elapsedTimeFromAnAttack >= type.AttackRate)
                {
                    zeroCharacter.Life = zeroCharacter.Life - damage;
                    elapsedTimeFromAnAttack = 0;
                }
            }

            // sets which part of the creature's movement animation to show
            SetSourceRectangle();

            // if it need to move
            if (isMovementAnimationNeeded)
            {
                // if it still not reached the target
                if (!destinationRectangle.Intersects(new Rectangle((int)moveToPoint.X, (int)moveToPoint.Y, 1, 1)))
                {
                    // set to which X and Y directionOfProjectile to move
                    Vector2 direction;
                    if (destinationRectangle.X - moveToPoint.X < 0)
                    {
                        direction.X = 1;
                        flipTheSprite = SpriteEffects.None;
                    }
                    else
                    {
                        direction.X = -1;
                        flipTheSprite = SpriteEffects.FlipHorizontally;
                    }

                    if (destinationRectangle.Y - moveToPoint.Y < 0)
                    {
                        direction.Y = 1;
                    }
                    else
                    {
                        direction.Y = -1;
                    }

                    destinationRectangle.X = destinationRectangle.X + (int)(direction.X * type.Speed);
                    destinationRectangle.Y = destinationRectangle.Y + (int)(direction.Y * type.Speed);

                    currentFrame++;
                    if (currentFrame > totalFrames)
                    {
                        currentFrame = 0;
                    }
                }
                else
                {
                    isMovementAnimationNeeded = false;
                }

            }
            else
            {
                currentFrame = 0;
            }

            // keeps the creature in the game windows
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
        /// Draw the creature
        /// </summary>
        /// <param name="spritebatch">the texture batch</param>
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White, 0, new Vector2(0, 0), flipTheSprite, 0);
        }

        #endregion public methods

        #region private methods

        private void SetSourceRectangle()
        {
            int width = texture.Width / type.ColumnsForAnimation;
            int height = texture.Height / type.RowsForAnimation;
            int row = (int)((float)currentFrame / (float)type.ColumnsForAnimation);
            int column = currentFrame % type.ColumnsForAnimation;

            sourceRectangle = new Rectangle(width * column, height * row, width, height);
        }

        /// <summary>
        /// Makes the creature to move to a given target. The methods works only with the update method.
        /// </summary>
        /// <param name="target">x and y coordinates on the game screen</param>
        private void SetMovementToTarget(Vector2 target)
        {
            moveToPoint = target;
            isMovementAnimationNeeded = true;
        }

        #endregion private methods

    }
}
