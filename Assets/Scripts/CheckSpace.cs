using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class CheckSpace : MonoBehaviour
    {
        bool isOccupied = false;
        Point position = new Point(-1,-1);

        public bool IsOccupied { get { return this.isOccupied; } }
        public Point Position { get { return this.position; } }

        //TODO: decide whether or not to store a pawn object for example.

        /// <summary>
        /// Position of checkspace.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void initiatePoint(int x, int y)
        {
            this.position = new Point(x, y);
        }

        /// <summary>
        /// Change the occupation of a checkspace.
        /// </summary>
        /// <param name="newOccupationStatus">'True' = Occupied, 'False' = Vacant</param>
        public void changeOccupation(bool newOccupationStatus)
        {
            this.isOccupied = newOccupationStatus;
        }
    }
}
