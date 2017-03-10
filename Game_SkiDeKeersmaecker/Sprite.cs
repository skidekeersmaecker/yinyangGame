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
    public abstract class Sprite: GameObject
    {
        protected Rectangle _visibleRect;
        protected Surface _imageRight;
        protected Surface _imageLeft;

        private bool endLoop;
        protected bool _isStandingLeft; public bool _IsStandingLeft { get { return _isStandingLeft; } }
        protected int _stepSizeAnimation;
        protected int _moveLeftStartAnimation;
        protected int _moveRightStartAnimation;

        protected List<Rectangle> _moveNumber;

        protected bool _left; public bool _Left { get { return _left; } }
        protected bool _right; public bool _Right { get { return _right; } }
        protected bool _up; public bool _Up { get { return _up; } }
        protected bool _down; public bool _Down { get { return _down; } }

        protected Point _spawnPoint; public Point _SpawnPoint
        {
            get { return _spawnPoint; }
            set
            {
                if (value.X < 0) value.X = 0;
                else if (value.X + _widthCollisionRect >= 1024) value.X = 102 - _widthCollisionRect;
                else if (value.Y < 0) value.Y = 0;
                else if (value.Y + _heightCollisionRect >= 576 - 24) value.Y = 576 - 24 - _heightCollisionRect;
                _spawnPoint = value;
            }
        }

        protected bool _isFast, _isOnPlatform;
        protected byte _animationCounter;
        protected byte _standLeft, _standRight, _walk, _run, _jump;
        protected byte _speedSlow, _speedFast;
        protected int _dy, _gravity;
        protected bool _upLeft, _upRight, _upFast;

        protected GameObject[,] _collisionObjects;
        protected List<Sprite> _collisionSprites;
        
        public Sprite _SpriteToInteract;

        protected bool _fire; public bool _Fire { get { return _fire; } }
        protected bool _fireUp; public bool _FireUp { get { return _fireUp; } }
        protected bool _fireDown; public bool _FireDown { get { return _fireDown; } }
        public List<Projectile> _projectiles;
        protected bool _isDead;
        public bool _scannedOverDeath;
        public bool _canShoot;

        public bool _IsDead
        {
            get { return _isDead; }
            set { _isDead = value; }
        }

        protected int _death;

        public Sprite()
        {
            _Positive = true;
            _imageActive = new Surface("TestSprite.png");
            endLoop = false;

            _spawnPoint = new Point(100, 100);
            _position = _spawnPoint;
            _heightCollisionRect = 64;
            _widthCollisionRect = 32;
            _collisionRect = new Rectangle(_position.X + 16, _position.Y, _widthCollisionRect, _heightCollisionRect);
            _stepSizeAnimation = 64;
            _moveLeftStartAnimation = 7 * 64;
            _moveRightStartAnimation = 0;
            _left = _right = _up = _down = _isStandingLeft = _isOnPlatform = _upLeft = _upRight = _upFast = false;

            _moveNumber = new List<Rectangle>();
            _moveNumber.Add(new Rectangle(_moveRightStartAnimation, _stepSizeAnimation * 8, _stepSizeAnimation, _stepSizeAnimation));   // 0 staanRechts
            _moveNumber.Add(new Rectangle(_moveLeftStartAnimation, _stepSizeAnimation * 8, _stepSizeAnimation, _stepSizeAnimation));    // 1 staanLinks
            _moveNumber.Add(new Rectangle(_moveRightStartAnimation, _stepSizeAnimation * 4, _stepSizeAnimation, _stepSizeAnimation));   // 2 wandelen
            _moveNumber.Add(new Rectangle(_moveRightStartAnimation, _stepSizeAnimation * 0, _stepSizeAnimation, _stepSizeAnimation));   // 3 rennen
            _moveNumber.Add(new Rectangle(_moveRightStartAnimation, _stepSizeAnimation * 5, _stepSizeAnimation, _stepSizeAnimation));   // 4 springen
            _moveNumber.Add(new Rectangle(_moveRightStartAnimation, _stepSizeAnimation * 2, _stepSizeAnimation, _stepSizeAnimation));   // 5 sterven

            _standRight = 0; _standLeft = 1; _walk = 2; _run = 3; _jump = 4; _death = 5; 
            _speedSlow = 5; _speedFast = 8;
            _dy = 0; _gravity = 18; //18: HIGH JUMP, 9: LOW JUMP

            _fire = _fireUp = _fireDown = false;
            _projectiles = new List<Projectile>();

            _isDead = _scannedOverDeath = false;
        }

        public abstract void Update();
        public abstract void Draw(Surface video);
        protected void AnimationLoop()
        {
            if (!_isStandingLeft)
            {
                _visibleRect.X += _stepSizeAnimation;
                if (_visibleRect.X >= _stepSizeAnimation * 8) //zijkant rechts (8 = aantal steps)
                {
                    _visibleRect.X = _moveRightStartAnimation;
                }
            }
            if (_isStandingLeft)
            {
                _visibleRect.X -= _stepSizeAnimation;
                if (_visibleRect.X <= 0) //zijkant links (begin, want ik loop achterstevoren)
                {
                    _visibleRect.X = _moveLeftStartAnimation;
                }
            }
        } 
        protected void SetAnimations()
        {
            _animationCounter++;
            if (_animationCounter > 3)
            {
                _animationCounter = 0;
                if (!_isDead)
                {
                    //stand
                    if (!_right && !_left)
                    {
                        if (!_isStandingLeft) _visibleRect = _moveNumber[_standRight];
                        else if (_isStandingLeft) _visibleRect = _moveNumber[_standLeft];
                    }

                    //walk key down
                    if (_right) SetRight();
                    if (_left) SetLeft();

                    //walk key up
                    if (_right && _isStandingLeft) _isStandingLeft = false;
                    if (_left && !_isStandingLeft) _isStandingLeft = true;

                    //run
                    if (_isFast)
                    {
                        if (_right || _left)
                            _visibleRect = _moveNumber[_run];
                        if (!_left && !_right)
                        {
                            if (!_isStandingLeft) _visibleRect = _moveNumber[_standRight];
                            else if (_isStandingLeft) _visibleRect = _moveNumber[_standLeft];
                        }
                    }
                    else if (!_isDead) //release run
                    {
                        _visibleRect = _moveNumber[_walk];
                        if (!_right && !_left)
                        {
                            if (!_isStandingLeft) _visibleRect = _moveNumber[_standRight];
                            else if (_isStandingLeft) _visibleRect = _moveNumber[_standLeft];
                        }
                    }
                }
                else if(_isDead)
                {
                    if (_isStandingLeft)
                    {
                        _imageActive = _imageLeft; 
                        _visibleRect = _moveNumber[_death];
                    }
                    else if (!_isStandingLeft)
                    {
                        _imageActive = _imageLeft; //normaal _imageRight maar zit bug in animatie waardoor fout loopt
                        _visibleRect = _moveNumber[_death];
                    }
                }
            }
        }
        protected void CheckCollisionSurrounding(GameObject[,] collisionObjects)
        {
            _isOnPlatform = false;
            for (int x = 0; x < collisionObjects.GetLength(0); x++)
            {
                for (int y = 0; y < collisionObjects.GetLength(1); y++)
                {
                    if (collisionObjects[x, y] != null)
                    {
                        if (_collisionRect.X + _widthCollisionRect >= collisionObjects[x, y]._collisionRect.X - 6 * (2) && _collisionRect.X - 16 <= collisionObjects[x, y]._collisionRect.X - 8 * (2) && _collisionRect.Y + _heightCollisionRect + 16 >= collisionObjects[x, y]._collisionRect.Y && _collisionRect.Y - 16 * (2) <= collisionObjects[x, y]._collisionRect.Y)
                        {
                            //Console.WriteLine("Collision is true");
                            if (_collisionRect.X + _widthCollisionRect - 6 * (2) < collisionObjects[x, y]._collisionRect.X && _position.Y + _heightCollisionRect - 10 * (2) > collisionObjects[x, y]._collisionRect.Y && _position.Y < collisionObjects[x, y]._collisionRect.Y)
                            {
                                //left
                                _position.X = collisionObjects[x, y]._collisionRect.X - _widthCollisionRect - 8 * (2);
                            }
                            else if (_collisionRect.X > collisionObjects[x, y]._collisionRect.X && _position.Y + _heightCollisionRect - 10 * (2) > collisionObjects[x, y]._collisionRect.Y && _position.Y < collisionObjects[x, y]._collisionRect.Y)
                            {
                                //right
                                _position.X = collisionObjects[x, y]._collisionRect.X - 6 * (2);
                            }
                            else if (_collisionRect.Y <= collisionObjects[x, y]._collisionRect.Y)
                            {
                                //top
                                _position.Y = collisionObjects[x, y]._collisionRect.Y - _heightCollisionRect - 2 * (2);
                                _dy = 19;
                                _isOnPlatform = true;
                            }
                            else if (_collisionRect.Y >= collisionObjects[x, y]._collisionRect.Y)
                            {
                                //bottom
                                _position.Y = collisionObjects[x, y]._collisionRect.Y + 14 * (2) + 1 * (2);
                                _dy = 19; //als tegen onder blokje springt dan zal hero meteen terug naar beneden vallen
                            }
                        }
                    }
                }
            }
        }
        protected void CheckCollisionSprites(List<Sprite> spriteArrayToCollisionWith)
        {
            foreach (var sprite in spriteArrayToCollisionWith)
            {
                if (!sprite._isDead && _collisionRect.X + _widthCollisionRect >= sprite._collisionRect.X + 10 && _collisionRect.X - 24 <= sprite._collisionRect.X && _collisionRect.Y + _heightCollisionRect + 20 >= sprite._collisionRect.Y && _collisionRect.Y - _widthCollisionRect <= sprite._collisionRect.Y + 12)
                {
                    _isDead = true;
                }
            }
        }
        protected void SetLeft()
        {
            _imageActive = _imageLeft;
            if (!_isFast)
                _visibleRect = _moveNumber[_walk];
            if (_isFast)
                _visibleRect = _moveNumber[_run];
        }
        protected void SetRight()
        {
            _imageActive = _imageRight;
            if (!_isFast)
                _visibleRect = _moveNumber[_walk];
            if (_isFast)
                _visibleRect = _moveNumber[_run];
        }
        protected void SetMovement()
        {
            if (_right && !_left && _isOnPlatform && !_isDead)
            {
                //move
                _position.X += _speedSlow;
                if (_isFast)
                    _position.X += _speedFast;

                _isStandingLeft = false;

                //visible animation
                AnimationLoop();
            }
            if (_left && !_right && _isOnPlatform && !_isDead)
            {
                //move
                _position.X -= _speedSlow;
                if (_isFast)
                    _position.X -= _speedFast;

                _isStandingLeft = true;

                //visible animation
                AnimationLoop();
            }
            if (_up && !_isDead)
            {
                //move
                _position.Y -= _gravity;// 18;
            }

            //collisionRectangle position
            _collisionRect.X = _position.X;
            _collisionRect.Y = _position.Y;

            //dead
            if (_IsDead)
            {
                _left = _right = _up = _down = _fire = _fireDown = _fireUp = false;

                //visible animation LOOP ONCE
                if(!endLoop)
                {
                    endLoop = true;
                    AnimationLoop(); 
                }
            }
        }
        protected void SetPhysics()
        {
            //gravity
            _dy += 2;
            _position.Y += _dy;

            //falling/jump voortgezette beweging
            if (_upRight && !_upLeft && !_isOnPlatform)
            {

                _position.X += _speedSlow;
                if (_upFast)
                    _position.X += _speedFast;
            }
            if (_upLeft && !_upRight && !_isOnPlatform)
            {
                _position.X -= _speedSlow;
                if (_upFast)
                    _position.X -= _speedFast;
            }
            if (_isOnPlatform)
            {
                if (!_left) _upLeft = false; else _upLeft = true;
                if (!_right) _upRight = false; else _upRight = true;
                if (!_isFast) _upFast = false; else _upFast = true;
            }
        }
        protected void SetProjectiles()
        {
            foreach (var projectile in _projectiles.ToList())
            {
                projectile.Update();
            }
        }
        public void RemoveFromList(List<Sprite> array)
        {
            if (array == null)
                return;
            array.Remove(this);
        }

    }
}
