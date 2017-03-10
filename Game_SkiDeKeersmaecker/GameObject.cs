using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_SkiDeKeersmaecker
{
    public abstract class GameObject
    {
        public bool _Positive { get; set; } //true = Yin, false = Yang
        protected Surface _imageActive;
        protected Point _position; public Point _Position {
            get { return _position; }
            set {
                if (value.X < 0) value.X = 0;
                else if (value.X + _widthCollisionRect >= 1024) value.X = 102 - _widthCollisionRect;
                else if (value.Y < 0) value.Y = 0;
                else if (value.Y + _heightCollisionRect >= 576 - 24) value.Y = 576 - 24 - _heightCollisionRect;
                _position = value;
            }
        }
        public Rectangle _collisionRect;
        protected int _heightCollisionRect; public int __HeightCollisionRect { get { return _heightCollisionRect; } }
        protected int _widthCollisionRect; public int _WidthCollisionRect { get { return _widthCollisionRect; } }

    }
}
