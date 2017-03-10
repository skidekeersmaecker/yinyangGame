using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdlDotNet.Graphics;
using SdlDotNet.Core;
using System.Threading;

namespace Game_SkiDeKeersmaecker
{
    public class Player : Sprite, IVisible
    {
        //RRR: code voor smooth gameplay Hero. Vervangt SetAnimations die alle non-event animations regelt. Dus indien wisselen => alle code met een 'RRR' in commentaar zetten en in update() SetAnimations(); plaatsen

        private bool setLeft, setRight;

        public Player(bool positiveIn)
        {
            _animationCounter = 1;

            setLeft = setRight = false;

            _Positive = positiveIn;
            if (_Positive)
            {
                _imageRight = new Surface("SpriteYinRight.png");
                _imageLeft = new Surface("SpriteYinLeft.png");
            }
            else
            {
                _imageRight = new Surface("SpriteYangRight.png");
                _imageLeft = new Surface("SpriteYangLeft.png");
            }

            _imageActive = _imageRight;
            _SpawnPoint = new Point(100, 100);
            _position = _SpawnPoint;
            _visibleRect = _moveNumber[0];

            Events.KeyboardDown += Events_KeyboardDown;
            Events.KeyboardUp += Events_KeyboardUp;
        }

        public Player(bool positiveIn, GameObject[,] collisionObjectsIn, List<Sprite> enemiesIn, Point spawnPointIn, bool canShootIn)
        {
            _collisionObjects = collisionObjectsIn;
            _collisionSprites = enemiesIn;
            _canShoot = canShootIn;
            _animationCounter = 1;

            _up = true;
            setLeft = setRight = false;

            _Positive = positiveIn;
            if (_Positive)
            {
                _imageRight = new Surface("SpriteYinRight.png");
                _imageLeft = new Surface("SpriteYinLeft.png");
            }
            else
            {
                _imageRight = new Surface("SpriteYangRight.png");
                _imageLeft = new Surface("SpriteYangLeft.png");
            }

            _imageActive = _imageRight;
            _spawnPoint = spawnPointIn;
            _position = _spawnPoint;
            _visibleRect = _moveNumber[0];

            Events.KeyboardDown += Events_KeyboardDown;
            Events.KeyboardUp += Events_KeyboardUp;
        }

        private void Events_KeyboardDown(object sender, SdlDotNet.Input.KeyboardEventArgs e)
        {
            if (e.Key == SdlDotNet.Input.Key.D)
            {
                if (!_isDead)
                {
                    setRight = _right = true;
                    _isStandingLeft = _left = false;
                    SetRight(); //RRR
                    if (_isOnPlatform)
                    {
                        _upRight = true;
                    }
                }
            }
            if (e.Key == SdlDotNet.Input.Key.A)
            {
                if (!_isDead)
                {
                    setLeft = _isStandingLeft = _left = true;
                    _right = false;
                    SetLeft(); //RRR
                    if (_isOnPlatform)
                    {
                        _upLeft = true;
                    }
                }
            }
            if (e.Key == SdlDotNet.Input.Key.W)
            {
                if (!_isDead)
                {
                    _up = true;
                    if (_isOnPlatform) _dy = -6;
                }
            }
            if (e.Key == SdlDotNet.Input.Key.Space)
            {
                if(_canShoot && !_isDead) _projectiles.Add(new Projectile(_Positive, _collisionObjects, _collisionSprites, this));
                _fire = true;
            }
            if (e.Key == SdlDotNet.Input.Key.LeftShift)
            {
                if (!_isDead)
                {
                    _isFast = true;
                    if (_right || _left)
                        _visibleRect = _moveNumber[_run];
                    if (!_left && !_right) //RRR
                    {
                        if (!_isStandingLeft) _visibleRect = _moveNumber[_standRight];
                        else if (_isStandingLeft) _visibleRect = _moveNumber[_standLeft];
                    }
                    if (_isOnPlatform)
                    {
                        _upFast = true;
                    }
                }
            }
            if (e.Key == SdlDotNet.Input.Key.RightArrow)
            {
                if (!_right && !_left && !_isDead)
                {
                    _isStandingLeft = false;
                    _visibleRect = _moveNumber[_standRight]; //RRR
                }
            }
            if (e.Key == SdlDotNet.Input.Key.LeftArrow)
            {
                if (!_right && !_left && !_isDead)
                {
                    _isStandingLeft = true;
                    _visibleRect = _moveNumber[_standLeft]; //RRR
                }
            }
            if (e.Key == SdlDotNet.Input.Key.UpArrow)
            {
                _fireDown = false;
                _fireUp = true;
            }
            if (e.Key == SdlDotNet.Input.Key.DownArrow)
            {
                _fireUp = false;
                _fireDown = true;
            }
        }

        private void Events_KeyboardUp(object sender, SdlDotNet.Input.KeyboardEventArgs e)
        {
            if (e.Key == SdlDotNet.Input.Key.D)
            {                
                if (!_isDead)
                {
                    setRight = _right = false;
                    if (!_left) //RRR
                        _visibleRect = _moveNumber[_standRight];
                    if (setLeft)
                    {
                        SetLeft(); //RRR
                        _left = _isStandingLeft = true;
                    }
                }
            }
            if (e.Key == SdlDotNet.Input.Key.A)
            {
                if (!_isDead)
                {
                    setLeft = _left = false;
                    if (!_right) //RRR
                        _visibleRect = _moveNumber[_standLeft];
                    if (setRight)
                    {
                        SetRight(); //RRR
                        _right = true;
                        _isStandingLeft = false;
                    }
                }
            }
            if (e.Key == SdlDotNet.Input.Key.W)
            {
                if (!_isDead)
                {
                    _isOnPlatform = false;
                }
            }
            if (e.Key == SdlDotNet.Input.Key.LeftShift)
            {
                if (!_isDead)
                {
                    _isFast = false;
                    _visibleRect = _moveNumber[_walk]; //RRR
                    if (!_left && !_right) //RRR
                    {
                        if (!_isStandingLeft) _visibleRect = _moveNumber[_standRight];
                        else if (_isStandingLeft) _visibleRect = _moveNumber[_standLeft];
                    }
                }
            }
            
            if (e.Key == SdlDotNet.Input.Key.Space)
            {
                _fire = false;
            }
            if (e.Key == SdlDotNet.Input.Key.UpArrow)
            {
                _fireUp = false;
            }
            if (e.Key == SdlDotNet.Input.Key.DownArrow)
            {
                _fireDown = false;
            }
        }

        public override void Update()
        {
            SetHeroNonEventAnimations(); //RRR
            //SetAnimations(); //Uncomment if RRR
            SetPhysics();
            SetMovement();
            SetCollisionBorder();
            CheckCollisionSurrounding(_collisionObjects);
            if(_Positive) CheckCollisionSprites(_collisionSprites);
            if (_canShoot) SetProjectiles();

            //Console.WriteLine("up: " + _up);
            //Console.WriteLine("x: " + _position.X + " y: " + _position.Y);
            //Console.WriteLine("y: " + _position.Y + " dy: " + _dy);
            //Console.WriteLine("Left: " + _left + " upLeft: " + _upLeft + " Right: " + _right + " upRight: " + _upRight);
            //Console.WriteLine("is on platform: " + _isOnPlatform);
            //Console.WriteLine("isStandingLeft: " + _isStandingLeft);
            //Console.WriteLine("heroX: " + _collisionRect.X + " heroY: " + _collisionRect.Y);
        }

        public override void Draw(Surface video)
        {
            video.Blit(_imageActive, _position, _visibleRect);
            if (_canShoot)
            {
                foreach (var projectile in _projectiles)
                {
                    projectile.Draw(video);
                }
            }
        }

        private void SetHeroNonEventAnimations() //RRR
        {
            _animationCounter++;
            if (_animationCounter > 3)
            {
                _animationCounter = 0;
                if (_isDead)
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
        private void SetCollisionBorder()
        {
            if (_collisionRect.X < -16) _position.X = -16;
            if (_collisionRect.X >= 1024 - 48) _position.X = 1024 - 48;
            if (_collisionRect.Y <= 0) _position.Y = 0;
            if (_collisionRect.Y >= 576 - _heightCollisionRect) _position.Y = 576;
        }

    }
}
