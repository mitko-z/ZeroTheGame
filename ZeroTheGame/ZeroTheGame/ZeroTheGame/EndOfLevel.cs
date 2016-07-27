using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ZeroTheGame
{
    public class EndOfLevel
    {
        #region fields

        // drawing variables
        Texture2D texture;
        Rectangle drawRectangle;

        #endregion fields

        #region constructor

        /// <summary>
        /// Constructor - create the end of level
        /// </summary>
        /// <param name="texture">the texture with which the it should be drawn</param>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <param name="width">the width</param>
        /// <param name="height">the height </param>
        public EndOfLevel (Texture2D texture, int x, int y, int width, int height)
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
        /// update the end of the level
        /// </summary>
        public void Update()
        {
            // the end of the level for now will be static so no update needed
        }

        /// <summary>
        /// draw the end of the level object
        /// </summary>
        /// <param name="spritebatch"> the texture batch</param>
        public void Draw(SpriteBatch spritebatch)
        {
                spritebatch.Draw(texture, drawRectangle, Color.White);
        }

        #endregion public methods
    }
}
