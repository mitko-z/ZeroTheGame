using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ZeroTheGame
{
    /// <summary>
    /// A class for a projectile
    /// </summary>
    public class Projectile
    {
        #region Fields

        bool active;
        ProjectileType type;

        // drawing fields
        Texture2D texture;
        Rectangle drawRectangle;
        // field for flipping horizontally the character when moving left and right
        SpriteEffects flipTheSprite;
        // if rotation is needed
        float rotation;

        // velocity calculation fields
        Vector2 position;
        Vector2 velocity;

        // attack fields
        int damage;

        // random number generator
        Random randomNumberGenerator = new Random();

        #endregion

        #region Constructors

        private Projectile() { }

        /// <summary>
        ///  Constructs a projectile with the given y velocity
        /// </summary>
        /// <param name="type">the projectile type</param>
        /// <param name="texture">the texture for the projectile</param>
        /// <param name="x">the x location of the center of the projectile</param>
        /// <param name="y">the y location of the center of the projectile</param>
        /// <param name="yVelocity">the y velocity for the projectile</param>
        public Projectile(ContentManager content, ProjectileType type, int x, int y, float size, Vector2 velocity)
        {
            this.type = type;
            this.texture = content.Load<Texture2D>("images//"+type.NameOfTexture);
            this.velocity = velocity;
            // set the draw rectangle
            int widthHeightRatio = this.texture.Width / this.texture.Height;
            int drawRectangleWidth = (int)size / 2;
            int drawRectangleHeight = (int)(size / widthHeightRatio) / 2;
            position = new Vector2(x, y);
            drawRectangle = new Rectangle(
                (int)position.X - drawRectangleWidth / 2, 
                (int)position.Y - drawRectangleHeight / 2, 
                drawRectangleWidth, 
                drawRectangleHeight);
            active = true;

            // set the damage
            damage = randomNumberGenerator.Next(type.MinDamage, type.MaxDamage);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets whether or not the projectile is active
        /// </summary>
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// Gets the projectile type
        /// </summary>
        public ProjectileType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the collision rectangle for the projectile
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
        }

        /// <summary>
        /// get and set the velocity of the projectile
        /// </summary>
        public Vector2 Velocity
        {
            set { velocity = value; }
            get { return velocity; }
        }

        public int Damage
        {
            set { damage = value; }
            get { return damage; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the projectile's location and makes inactive when it
        /// leaves the game window
        /// </summary>
        public void Update(GameTime gameTime, int windowWidth, int windowHeight)
        {
            // move the projectile
            position.X += velocity.X * gameTime.ElapsedGameTime.Milliseconds;
            position.Y += velocity.Y * gameTime.ElapsedGameTime.Milliseconds;
            drawRectangle.X = (int)position.X;
            drawRectangle.Y = (int)position.Y;

            // set the projectile inactive if reached out of the game window
            if (
                drawRectangle.Bottom < 0 ||
                drawRectangle.Top > windowHeight || 
                drawRectangle.Right < 0 || 
                drawRectangle.Left > windowWidth)
            {
                active = false;
            }
        }

        /// <summary>
        /// Draws the projectile
        /// </summary>
        /// <param name="spriteBatch">the texture batch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                //spriteBatch.Draw(texture, drawRectangle, Color.White);
                // flip the image if necessary
                if (velocity.X > 0)
                {
                    flipTheSprite = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    flipTheSprite = SpriteEffects.None;
                }
                // calculation of the rotation of the sprite
                if (velocity.X == 0)
                {
                    velocity.X += 0.00000000001f; // in order to avoid division of 0
                    
                }
                if (velocity.Y == 0)
                {
                    velocity.Y += 0.00000000001f;
                }
                rotation = (float)Math.Atan(velocity.Y / velocity.X);
                spriteBatch.Draw(texture, drawRectangle, null, Color.White, rotation, new Vector2(), flipTheSprite, 0);
            }
        }

        #endregion
    }
}
