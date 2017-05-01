using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class Point
    {
        public float X { get { return this.x; } }
        public float Y { get { return this.y; } }

        private float x;
        private float y;

        public Point(int x, int y)
        {
            this.x = (float)x;
            this.y = (float)y;
        }

        public Point(double x, double y)
        {
            this.x = (float)x;
            this.y = (float)y;
        }

        public Point(long x, long y)
        {
            this.x = (float)x;
            this.y = (float)y;
        }


    }
}
