using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ZeroTheGame
{
    public class Weapon
    {
        #region fields

        // type of the weapon
        WeaponType type;

        // drawing variables
        Texture2D texture;
        Rectangle drawRectangle;

        // firing fields
        ProjectileType projectileType;
        int attackRate;
        List<Projectile> projectiles = new List<Projectile>();
        int elapsedTimeFromAnAttack;

        // if it is owend by someone
        bool isOwned;

        // random number generator
        Random randomNumberGenerator = new Random();

        #endregion fields

        #region constructors

        public Weapon() { }

        /// <summary>
        /// Constructor - create a weapon placed somewhere in the level
        /// </summary>
        /// <param name="texture">the texture with which the weapon should be drawn</param>
        /// <param name="x">the x coordinate of the left side of the weapon</param>
        /// <param name="y">the y coordinate of the top side of the weapon</param>
        /// <param name="width">the width of the weapon</param>
        /// <param name="height">the height of the weapon</param>
        /// <param name="ProjectileType">which type of projectiles the weapon can fire</param>
        /// <param name="projectileSpeed">what is projectileSpeed of the projectiles</param>>
        /// <param name="attackRate">what is the time between two projectile fired, in miliseconds</param>>
        /// <param name="isOwned">specify if the weapon initially is set on the map (false) or set directly on a character (true)</param>>
        public Weapon(
            ContentManager content,
            WeaponType weaponType, 
            int x, 
            int y, 
            float size,
            bool isOwned)
        {
            // set the type of the weapon
            type = weaponType;

            // set the drawing of the weapon
            int width = (int)size;
            int height = (int)(2 * size / 3);
            texture = content.Load<Texture2D>("images//" + weaponType.NameOfTexture);
            drawRectangle = new Rectangle(x + width / 2, y + height / 2, width, height);

            // set the firing characteristics
            this.projectileType = weaponType.ProjectileType;
            attackRate = weaponType.AttackRate;
            
            // if the weapon is owned
            this.isOwned = isOwned;

            // set initially the time fo attack
            elapsedTimeFromAnAttack = attackRate;
        }

        #endregion constructors

        #region properies

        public WeaponType Type
        {
            set { type = value; }
            get { return type; }
        }

        /// <summary>
        /// get and set the rectangle needed for collisions issues
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
            set { drawRectangle = value; }
        }

        /// <summary>
        /// what is what is the time between two projectile fired, in miliseconds
        /// </summary>
        public int AttackRate
        {
            get { return attackRate; }
        }

        /// <summary>
        /// set and get if the weapon is set on the map (false) or set directly on a character (true)
        /// </summary>
        public bool IsOwned
        {
            set { isOwned = value; }
            get { return isOwned; }
        }

        /// <summary>
        /// gets the list of projectiles fired by the weapon
        /// </summary>
        public List<Projectile> Projectiles
        {
            get { return projectiles; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        #endregion properies

        #region public methods

        public void Update
            (
             ContentManager content, 
             GameTime gameTime, 
             Vector2 weaponCoordinates, 
             bool setToStartFire, 
             Vector2 firingDirection, 
             float errorInFiring,
             int windowWidth,
             int windowHeight
            )
        {
            drawRectangle.X = (int)weaponCoordinates.X;
            drawRectangle.Y = (int)weaponCoordinates.Y;

            // update the time for attack
            elapsedTimeFromAnAttack += gameTime.ElapsedGameTime.Milliseconds;

            // if the weapon is owned by someone (that someone wear it and is able to fire with it) 
            // and command for fire is set and the time between two fires expired 
            if (isOwned && setToStartFire && (elapsedTimeFromAnAttack > attackRate))
            {
                Vector2 directionOfProjectile; // = new Vector2
                            //( // the directionOfProjectile is not completely accurate with the idea the character to improve his firing skill with the time
                            //firingDirection.X * projectileType.Speed + randomNumberGenerator.Next(100) * errorInFiring / 100,
                            //firingDirection.Y * projectileType.Speed + randomNumberGenerator.Next(100) * errorInFiring / 100
                            //);
                if (firingDirection.X == 0)
                {
                    directionOfProjectile.X =
                        firingDirection.X * projectileType.Speed +
                        randomNumberGenerator.Next(-50, 50) * errorInFiring / 100;
                    directionOfProjectile.Y = firingDirection.Y * projectileType.Speed;
                }
                else
                {
                    directionOfProjectile.X = firingDirection.X * projectileType.Speed;
                    directionOfProjectile.Y = 
                        firingDirection.Y * projectileType.Speed + 
                        randomNumberGenerator.Next(-50, 50) * errorInFiring / 100;
                }
                // add new projectile
                projectiles.Add
                    (
                     new Projectile
                        (
                        content,
                        projectileType,
                        drawRectangle.Center.X,
                        drawRectangle.Center.Y,
                        drawRectangle.Width,
                        directionOfProjectile
                        )
                    );
                elapsedTimeFromAnAttack = 0;
            }
            foreach (Projectile projectile in projectiles)
            {
                projectile.Update(gameTime, windowWidth, windowHeight);
            }
            // removes any inactive projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (!projectiles[i].Active)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// draw the weapon if necessary and any projectiles fired from this weapon
        /// </summary>
        /// <param name="spritebatch">the texture batch</param>
        public void Draw(SpriteBatch spritebatch)
        {
            // draw it if the weapon it is not owend by anyone, i.e. it is placed somewhere on the map
            if (!isOwned)
            {
                spritebatch.Draw(texture, drawRectangle, Color.White);
            }

            // draw the projectiles which the weapon fired
            foreach (Projectile projectile in projectiles)
            {
                if (projectile.Active)
                {
                    projectile.Draw(spritebatch);
                }
            }
        }
        #endregion
    }
}
