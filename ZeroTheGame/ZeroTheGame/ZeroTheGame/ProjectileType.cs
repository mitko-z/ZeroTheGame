using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroTheGame
{
    public class ProjectileType
    {
        #region fields
        string nameOfTexture;
        float speed;
        int minDamage;
        int maxDamage;
        #endregion fields

        #region constructors

        private ProjectileType() { }

        /// <summary>
        /// create a projectile tyoe
        /// </summary>
        /// <param name="nameOfTexture">the name of the texture used for displaying the projectile</param>
        /// <param name="speed">how fast the projectile moves</param>
        /// <param name="minDamage">the minimal damage the projectile takes</param>
        /// <param name="maxDamage">the maximal damage the projectile takes</param>
        public ProjectileType(string nameOfTexture, float speed, int minDamage, int maxDamage)
        {
            this.nameOfTexture = nameOfTexture;
            this.speed = speed;
            this.minDamage = minDamage;
            this.maxDamage = maxDamage;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// the name of the texture used for displaying the projectile
        /// </summary>
        public string NameOfTexture
        {
            set { nameOfTexture = value; }
            get { return nameOfTexture; }
        }
        /// <summary>
        /// how fast the projectile moves
        /// </summary>
        public float Speed
        {
            set { speed = value; }
            get { return speed; }
        }
        /// <summary>
        /// the minimal damage the projectile takes
        /// </summary>
        public int MinDamage
        {
            set { minDamage = value; }
            get { return minDamage; }
        }
        /// <summary>
        /// the maximal damage the projectile takes
        /// </summary>
        public int MaxDamage
        {
            set { maxDamage = value; }
            get { return maxDamage; }
        }
        #endregion properties
    }
}
