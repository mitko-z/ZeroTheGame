using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroTheGame
{
    public class CreatureType
    {
        #region fields
        string nameOfTexture;
        int rowsForAnimation;
        int columnsForAnimation;
        float speed;
        float attackRate;
        float minDamage;
        float maxDamage;
        float attackRange;
        float maxHealth;
        float size;
        List<ProjectileType> resistentToProjectiles;
        #endregion fields
        
        #region constructors

        private CreatureType() { }

        /// <summary>
        /// create a creature type
        /// </summary>
        /// <param name="nameOfTexture">the name of the texture (or texture atlas) used for displaying the creature</param>
        /// <param name="rowsForAnimation">how many rows there are on the texture atlas (1 if this is not animation)</param>
        /// <param name="columnsForAnimation">how many columns there are on the texture atlas (1 if this is not animation)</param>
        /// <param name="speed">what is the speed of the creature</param>
        /// <param name="attackRate">how many miliseconds between two attacks of the creature</param>
        /// <param name="minDamage">what is the minimum damage the creature can make when attack</param>
        /// <param name="maxDamage">what is the maximum damage the creature can make when attack</param></param>
        /// <param name="attackRange">how far it can attack (for now it is inactive)</param></param>
        /// <param name="health">with how many health the creature is created</param>
        /// <param name="size">what is the size according the standard size of an object in this game</param>
        public CreatureType(
            string nameOfTexture, 
            int rowsForAnimation, 
            int columnsForAnimation, 
            float speed, 
            float attackRate, 
            float minDamage, 
            float maxDamage, 
            float attackRange,
            float health,
            float size,
            List<ProjectileType> resistentToProjectiles)
        {
            this.nameOfTexture=nameOfTexture;
            this.rowsForAnimation = rowsForAnimation;
            this.columnsForAnimation = columnsForAnimation;
            this.speed = speed;
            this.attackRate = attackRate;
            this.minDamage = minDamage;
            this.maxDamage = maxDamage;
            this.attackRange = attackRange;
            this.maxHealth = health;
            this.size = size;
            this.resistentToProjectiles = resistentToProjectiles;
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
        public int RowsForAnimation
        {
            set { rowsForAnimation = value; }
            get { return rowsForAnimation; }
        }

        public int ColumnsForAnimation
        {
            set { columnsForAnimation = value; }
            get { return columnsForAnimation; }
        }

        public float Speed
        {
            set { speed = value; }
            get { return speed; }
        }

        public float Size
        {
            set { size = value; }
            get { return size; }
        }

        public List<ProjectileType> ResistentToProjectiles
        {
            set { resistentToProjectiles = value; }
            get { return resistentToProjectiles; }
        }

        /// <summary>
        /// how many miliseconds between two attacks
        /// </summary>
        public float AttackRate
        {
            set { attackRate = value; }
            get { return attackRate; }
        }

        public float MinDamage
        {
            set { minDamage = value; }
            get { return minDamage; }
        }

        public float MaxDamage
        {
            set { maxDamage = value; }
            get { return maxDamage; }
        }

        /// <summary>
        /// how far it fires
        /// </summary>
        public float AttackRange
        {
            set { attackRange = value; }
            get { return attackRange; }
        }

        public float MaxHealth
        {
            set { maxHealth = value; }
            get { return maxHealth; }
        }
        #endregion properties

    }
}
