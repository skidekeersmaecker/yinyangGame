using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdlDotNet.Graphics;
using System.Drawing;

namespace Game_SkiDeKeersmaecker
{
    public class NPC: Sprite, IVisible
    {
        //in points: places goes as next; 00 is point 0 on platform 0 => 11 is point 1 from platform 1 (first = 0)
        //in points: even places (0,2,4,...,38) are for going right, all uneven places (1,3,5,...,39) are for going left
        private int[] yPlatforms;
        private PointLevel[] xPoints;
        private bool[] platform, point;

        public NPC(bool positiveIn, GameObject[,] collisionObjectsIn, List<Sprite> enemiesIn, int[] yPlatformsIn,  PointLevel[] xPointsIn, Point spawnPointIn, bool startLeft, bool canShootIn)
        {
            _collisionObjects = collisionObjectsIn;
            _collisionSprites = enemiesIn;
            _canShoot = canShootIn;
            _up = true;
            _Positive = positiveIn;
            if (_Positive)
            {
                _imageRight = new Surface("SpriteYangRight.png");
                _imageLeft = new Surface("SpriteYangLeft.png");
            }
            else
            {
                _imageRight = new Surface("SpriteYinRight.png");
                _imageLeft = new Surface("SpriteYinLeft.png");
            }
            _imageActive = _imageRight;
            _spawnPoint = spawnPointIn;
            _position = _spawnPoint;
            _visibleRect = _moveNumber[0];

            _left = startLeft;
            _right = !_left;

            yPlatforms = new int[5];
            for (int i = 0; i < yPlatforms.GetLength(0); i++)
            {
                yPlatforms[i] = yPlatformsIn[i];
            }
            platform = new bool[5];
            for (int i = 0; i < platform.GetLength(0); i++)
            {
                platform[i] = false;
            }
            xPoints = new PointLevel[40];
            for (int i = 0; i < xPoints.GetLength(0); i++)
            {
                xPoints[i] = xPointsIn[i];
            }
            point = new bool[40];
            for (int i = 0; i < point.GetLength(0); i++)
            {
                point[i] = false;
            }
        }

        public override void Update()
        {
            SetAnimations();
            SetPhysics();
            SetMovement();
            CheckCollisionSurrounding(_collisionObjects);
            SetAIParameters();
            SetAI();
            if(!_Positive) CheckCollisionSprites(_collisionSprites);
            if (_canShoot) SetProjectiles();

            //Console.WriteLine("spawnX: " + _spawnPoint.X + " posX: " + _position.X + " spawnY: " + _spawnPoint.Y + " posY: " + _position.Y);
            //Console.WriteLine("isStandingLeft: " + _isStandingLeft);
            //Console.WriteLine("left: " + _left + " right: " + _right + " up: " + _up);
            //Console.WriteLine("heroX: " + Hero._collisionRect.X + " heroY: " + Hero._collisionRect.Y);
            //Console.WriteLine("enemy Y: " + _collisionRect.Y + " hero Y: " + Hero._collisionRect.Y);
            //Console.WriteLine("plat0: " + platform0 + " plat1: " + platform2 + " plat3: " + platform2 + " plat3: " + platform3);
            //Console.WriteLine("plat3: " + platform3 + " point4: " + point4 + " right: " + _right);
            //Console.WriteLine(platform3 + "  " + point6 + "  " + _right + "  " +  _isOnPlatform);
            //Console.WriteLine("point6: " + point6 + " point7: " + point7);
            //Console.WriteLine("heroX: " + Hero._collisionRect.X + " enemyX: " + _collisionRect.X);
            /*int i = 0;
            foreach (var item in xPoints)
            {
                Console.WriteLine("point" + i + "   " + item.ToString());
                i++;
            }*/
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

        private void SetAI()
        {
            _isFast = false;
            //Platform 0
            if (platform[0]) //per platform: reageren op enkel de punten van het platform waar hij zich bevindt
            {
                ChooseActions(0, xPoints);
            }
            //Platform 1
            if (platform[1])
            {
                ChooseActions(1, xPoints);
            }
            //Platform 2
            if (platform[2])
            {
                ChooseActions(2, xPoints);
            }
            //Platform 3
           if (platform[3]) 
            {
                ChooseActions(3, xPoints);
            }
        }
        private void SetAIParameters()
        {
            //BORDER
            if (_collisionRect.X < -16) { _position.X = -16; _left = false; _right = true; }
            if (_collisionRect.X >= 1024 - 48) { _position.X = 1024 - 58; _right = false; _left = true; }

            //SET PLATFORMS
            for (int i = 0; i < platform.GetLength(0) - 1; i++)
            {
                if (_collisionRect.Y < yPlatforms[i] && _collisionRect.Y >= yPlatforms[i+1]) platform[i] = true; else platform[i] = false;
            }

            //SET POINTS
            for (int i = 0; i < point.GetLength(0); i++)
            {
                if (_collisionRect.X >= xPoints[i].X && _collisionRect.X <= xPoints[i].X + 8) point[i] = true; else point[i] = false;
            }
        }
        protected void ChooseActions(int platform, PointLevel[] pointArray)
        {
            int platformBegin = 0;
            int platformEnd = 0;

            if (_right) //only if going RIGHT on this point I can jump on upper platform (if going LEFT I'd jump up to nothing)
            {
                if (platform == 0) { platformBegin = 0; platformEnd = 10; }
                if (platform == 1) { platformBegin = 10; platformEnd = 20; }
                if (platform == 2) { platformBegin = 20; platformEnd = 30; }
                if (platform == 3) { platformBegin = 30; platformEnd = 40; }

                for (int i = platformBegin; i < platformEnd; i += 2) //all even points on platform 0 (going right) => only points on its own platform count
                {
                    if (point[i])
                    {
                        //ACTIONS GIVEN BY LEVEL THROUGH POINTLEVEL OBJECT
                        if (pointArray[i].JumpUpAt) JumpToUpperPlatform(pointArray[i].IsFast);
                        if (pointArray[i].JumpSameLevelAt) JumpOnSamePlatform(pointArray[i].IsFast);
                        if (pointArray[i].RunAt) RunAtPoint();
                        if (pointArray[i].WalkAt) WalkAtPoint();
                    }
                }
            }

            if (_left) //only if going LEFT on this point I can jump on upper platform (if going RIGHT I'd jump up to nothing)
            {
                if (platform == 0) { platformBegin = 1; platformEnd = 11; }
                if (platform == 1) { platformBegin = 11; platformEnd = 21; }
                if (platform == 2) { platformBegin = 21; platformEnd = 31; }
                if (platform == 3) { platformBegin = 31; platformEnd = 41; }

                for (int i = platformBegin; i < platformEnd; i += 2) //all uneven points on platform 0 (going left) => only points on its own platform count
                {
                    if (point[i])
                    {
                        if (pointArray[i].JumpUpAt) JumpToUpperPlatform(pointArray[i].IsFast);
                        if (pointArray[i].JumpSameLevelAt) JumpOnSamePlatform(pointArray[i].IsFast);
                        if (pointArray[i].RunAt) RunAtPoint();
                        if (pointArray[i].WalkAt) WalkAtPoint();
                    }
                }
            }

            //going either way
            if (platform == 0) { platformBegin = 0; platformEnd = 10; }
            if (platform == 1) { platformBegin = 10; platformEnd = 20; }
            if (platform == 2) { platformBegin = 20; platformEnd = 30; }
            if (platform == 3) { platformBegin = 30; platformEnd = 40; }

            for (int i = platformBegin; i < platformEnd; i++)
            {                
                if (point[i])
                {
                    if (pointArray[i].StopAt) StopAtPoint();
                    if (pointArray[i].IgnoreThis) IgnoreAtPoint();
                    if (pointArray[i].HoldAt) HoldAtPoint();
                    if (pointArray[i].TurnAt) TurnAtPoint();
                    if (pointArray[i].ShootAt) ShootAtPoint();
                }
            }
        }

        private void StopAtPoint()
        {

        }
        private void IgnoreAtPoint()
        {

        }
        private void HoldAtPoint()
        {

        }
        private void TurnAtPoint()
        {
            if (_collisionRect.X < _SpriteToInteract._collisionRect.X) { _left = false; _right = true; }
            else { _right = false; _left = true; }
        }
        private void JumpToUpperPlatform(bool isFastIn)
        {
            if (_SpriteToInteract._collisionRect.Y < _collisionRect.Y && !_isDead)
            {
                _isFast = isFastIn;
                _up = true;
                if (_isOnPlatform) _dy = -6;
            }
        }
        private void JumpOnSamePlatform(bool isFastIn)
        {
            if (_SpriteToInteract._collisionRect.Y <= _collisionRect.Y && !_isDead)
            {
                _isFast = isFastIn;
                _up = true;
                if (_isOnPlatform) _dy = -6;
            }
        }
        private void RunAtPoint()
        {

        }
        private void WalkAtPoint()
        {

        }
        private void ShootAtPoint()
        {
            if (_collisionRect.X < _SpriteToInteract._collisionRect.X) { _left = false; _right = true; }
            if (_canShoot && _isOnPlatform && _SpriteToInteract._collisionRect.Y - 10 <= _collisionRect.Y)
            {
                if ((_collisionRect.X < _SpriteToInteract._collisionRect.X && _right) || (_collisionRect.X > _SpriteToInteract._collisionRect.X && _left))
                {
                    _projectiles.Add(new Projectile(_Positive, _collisionObjects, _collisionSprites, this) { Speed = 10 });
                }
            }
        }

        private bool IsEven(bool[] array, int platformIn)
        {
            int platformBegin = 0;
            int platformEnd = 0;

            if (platformIn == 0) { platformBegin = 0; platformEnd = 10; }
            if (platformIn == 1) { platformBegin = 10; platformEnd = 20; }
            if (platformIn == 2) { platformBegin = 20; platformEnd = 30; }
            if (platformIn == 3) { platformBegin = 30; platformEnd = 40; }

            bool[] arrayEven = new bool[40];
            for (int i = 0; i < arrayEven.GetLength(0); i++)
            {
                arrayEven[i] = false;
            }
            for (int i = platformBegin; i < platformEnd; i += 2)
            {
                arrayEven[i] = array[i];
            }
            if (arrayEven.Contains(true)) return true;
            else return false;
        } //NOT USED
        private bool IsUneven(bool[] array, int platformIn)
        {
            int platformBegin = 0;
            int platformEnd = 0;

            if (platformIn == 0) { platformBegin = 1; platformEnd = 11; }
            if (platformIn == 1) { platformBegin = 11; platformEnd = 21; }
            if (platformIn == 2) { platformBegin = 21; platformEnd = 31; }
            if (platformIn == 3) { platformBegin = 31; platformEnd = 41; }

            bool[] arrayEven = new bool[40];
            for (int i = 0; i < arrayEven.GetLength(0); i++)
            {
                arrayEven[i] = false;
            }
            for (int i = platformBegin; i < platformEnd; i += 2)
            {
                arrayEven[i] = array[i];
            }
            if (arrayEven.Contains(true)) return true;
            else return false;
        } //NOT USED
    }
}