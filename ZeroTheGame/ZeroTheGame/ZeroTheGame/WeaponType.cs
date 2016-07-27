using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroTheGame
{
    public class WeaponType
    {
        #region fields
        string nameOfTexture;
        int attackRate;
        int range;
        ProjectileType projectileType;
        #endregion fields

        #region constructors

        private WeaponType() { }

        /// <summary>
        /// create a weapon type
        /// </summary>
        /// <param name="nameOfTexture">the name of the texture used for displaying the weapon</param>
        /// <param name="attackRate">how many miliseconds between two fires</param>
        /// <param name="range">how far it fires</param>
        /// <param name="projectileType">with what kind of projectiles fires the weapon</param>
        public WeaponType(string nameOfTexture, int attackRate, int range, ProjectileType projectileType)
        {
            this.nameOfTexture = nameOfTexture;
            this.attackRate = attackRate;
            this.range = range;
            this.projectileType = projectileType;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// the name of the texture of the weapon
        /// </summary>
        public string NameOfTexture
        {
            set { nameOfTexture = value; }
            get { return nameOfTexture; }
        }
        /// <summary>
        /// how many miliseconds between two fires
        /// </summary>
        public int AttackRate
        {
            set { attackRate = value; }
            get { return attackRate; }
        }
        /// <summary>
        /// how far it fires
        /// </summary>
        public int Range
        {
            set { range = value; }
            get { return range; }
        }

        /// <summary>
        /// with what kind of projectiles fires the weapon
        /// </summary>
        public ProjectileType ProjectileType
        {
            set { projectileType = value; }
            get { return projectileType; }
        }
        #endregion properties
    }
}
