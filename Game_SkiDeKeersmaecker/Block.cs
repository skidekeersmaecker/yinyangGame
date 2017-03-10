using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_SkiDeKeersmaecker
{
    public abstract class Block: GameObject
    {
        protected Surface _imageYin;
        protected Surface _imageYang;
        public Block(bool positiveIn, Point positionIn)
        {
            _Positive = positiveIn;
            if (_Positive) _imageActive = new Surface("WhiteBlock32.png"); else _imageActive = new Surface("BlackBlock32.png");
            _position = positionIn;
            _widthCollisionRect = 32;
            _heightCollisionRect = 32;
            _collisionRect = new Rectangle(_position.X, _position.Y, _widthCollisionRect, _heightCollisionRect);
        }

        public virtual void Draw(Surface video)
        {
            video.Blit(video);
        }

        public virtual void Update()
        {
            
        }
    }
}
