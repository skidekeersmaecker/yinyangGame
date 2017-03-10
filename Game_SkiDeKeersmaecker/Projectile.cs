using SdlDotNet.Core;
using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_SkiDeKeersmaecker
{
    public class Projectile: GameObject, IVisible
    {
        private GameObject[,] collisionObjects;
        public List<Sprite> enemyList;
        public Sprite enemy;
        private Sprite attacker;

        private Surface imageYinWhite;
        private Surface imageYangWhite;
        private Surface imageYinBlack;
        private Surface imageYangBlack;

        private bool hit, isTurnedLeft, isTurnedUp, isTurnedDown;
        private int speed; public int Speed
        {
            get { return speed; }
            set
            {
                if (value < 0) value = 0;
                else if (value > 19) value = 19;
                speed = value;
            }
        }

        public bool Hit { get { return hit; } }

        public Projectile(bool positiveIn, GameObject[,] collisionObjectsIn, List<Sprite> enemyListIn, Sprite attackerIn) //SPR-NPC / SPR-PLAYER
        {
            _Positive = positiveIn;
            collisionObjects = collisionObjectsIn;
            enemyList = enemyListIn;
            attacker = attackerIn;

            isTurnedLeft = attacker._IsStandingLeft;
            isTurnedUp = attacker._FireUp;
            isTurnedDown = attacker._FireDown;

            imageYinWhite = new Surface("yinWhite.png");
            imageYangWhite = new Surface("yangWhite.png");
            imageYinBlack = new Surface("yinBlack.png");
            imageYangBlack = new Surface("yangBLack.png");

            if (_Positive)
            {
                if (attacker.GetType() == typeof(Player)) _imageActive = imageYinWhite;
                else _imageActive = imageYangWhite;
            }
            else
            {
                if (attacker.GetType() == typeof(Player)) _imageActive = imageYinBlack;
                else _imageActive = imageYangBlack;
            }
            
            hit = false;
            speed = 19; //MAX SPEED

            _heightCollisionRect = 18;
            _widthCollisionRect = 15;
            _collisionRect = new Rectangle(_position.X, _position.Y, _widthCollisionRect, _heightCollisionRect);

            if (attacker._IsStandingLeft) _position = new Point(attacker._collisionRect.X, attacker._collisionRect.Y + attacker.__HeightCollisionRect / 4);
            else _position = new Point(attacker._collisionRect.X + attacker._WidthCollisionRect, attacker._collisionRect.Y + attacker.__HeightCollisionRect / 4);
        }

        public void Update()
        {
            SetMovement();
            CheckCollisionSurrounding(collisionObjects);
            CheckCollisionSprites(enemyList);
        }

        public void Draw(Surface video)
        {           
            video.Blit(_imageActive, _position);
        }

        private void CheckCollisionSurrounding(GameObject[,] collisionObjects)
        {
            for (int x = 0; x < collisionObjects.GetLength(0); x++)
            {
                for (int y = 0; y < collisionObjects.GetLength(1); y++)
                {
                    if (collisionObjects[x, y] != null && (_collisionRect.X + _widthCollisionRect >= collisionObjects[x, y]._collisionRect.X + 3 && _collisionRect.X - 24 <= collisionObjects[x, y]._collisionRect.X && _collisionRect.Y + _heightCollisionRect - 16 >= collisionObjects[x, y]._collisionRect.Y && _collisionRect.Y - 16 <= collisionObjects[x, y]._collisionRect.Y))
                    {
                            hit = true;
                    }
                }
            }
            if (_collisionRect.X <= 0 || _collisionRect.X >= 1024 - _widthCollisionRect || _collisionRect.Y <= 0 || _collisionRect.Y >= 576) hit = true;

        }
        public void CheckCollisionSprites(List<Sprite> enemyList)
        {
            foreach (var enemy in enemyList)
            {
                if (!enemy._IsDead && _collisionRect.X + _widthCollisionRect >= enemy._collisionRect.X + 24 && _collisionRect.X - 32 <= enemy._collisionRect.X && _collisionRect.Y + _heightCollisionRect + 10 >= enemy._collisionRect.Y && _collisionRect.Y - _widthCollisionRect <= enemy._collisionRect.Y + 20)
                {
                    enemy._IsDead = true;
                    hit = true;
                }
            }
        }

        private void SetMovement()
        {
            //move horizontally
            if (isTurnedLeft) _position.X -= speed; //19 (max speed)
            else _position.X += speed; //19 (max speed)

            //move vertically
            if (isTurnedUp) _position.Y -= speed / 2;
            if (isTurnedDown) _position.Y += speed / 2;

            //collisionRectangle position
            _collisionRect.X = _position.X;
            _collisionRect.Y = _position.Y;
        }

        public void RemoveFromList(List<Projectile> array)
        {
            if (array == null)
                return;
            array.Remove(this);
        }
    }
}
