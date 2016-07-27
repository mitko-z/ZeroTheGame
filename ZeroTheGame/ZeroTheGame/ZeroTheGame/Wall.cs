using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZeroTheGame
{
    public class Wall
    {
        #region fields

        // drawing variables
        Texture2D texture;
        Rectangle drawRectangle;

        #endregion fields

        #region constructor

        /// <summary>
        /// Constructor - create a wall
        /// </summary>
        /// <param name="texture">the texture with which the wall should be drawn</param>
        /// <param name="x">the x coordinate of the wall</param>
        /// <param name="y">the y coordinate of the wall</param>
        /// <param name="width">the width of the wall</param>
        /// <param name="height">the height of the wall</param>
        public Wall(Texture2D texture, int x, int y, int width, int height)
        {
            this.texture = texture;

            // set where to place the wall
            drawRectangle = new Rectangle(x, y, width, height);

        }

        #endregion constructor

        #region properties

        /// <summary>
        /// get and set the rectangle needed for collisions issues
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
            set { drawRectangle = value; }
        }

        #endregion properties

        #region public methods

        /// <summary>
        /// update the wall
        /// </summary>
        public void Update()
        {
            // the wall for now will be static so no update needed
        }

        /// <summary>
        /// draw the wall
        /// </summary>
        /// <param name="spritebatch">the texture batch</param>
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, drawRectangle, Color.White);
        }

        #endregion public methods

    }
}
