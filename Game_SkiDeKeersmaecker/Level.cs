using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_SkiDeKeersmaecker
{
    public abstract class Level
    {
        protected bool _positive; public bool _Positive { get { return _positive; } }
        protected int _tileHeight;
        protected int _tileWidth;
        protected byte[,] _byteTileArray;
        protected Block[,] _blokTileArray;
        public GameObject[,] _CollisionObjects;
        public List<Sprite> _Enemies;
        public Sprite _Enemy;
        public List<Sprite> _Allies;
        public Sprite _Ally;
        protected int[] _yPlatforms;
        protected PointLevel[] _xPoints;
        protected int _numberOfAllies; public int _NumberOfAllies { get { return _numberOfAllies; } }
        protected int _numberOfEnemies; public int _NumberOfEnemies { get { return _numberOfEnemies; } }
        protected bool _gameOver; public bool _GameOver{ get { return _gameOver; } }
        protected bool _gameWon; public bool _GameWon { get { return _gameWon; } }
        protected List<Point> _spawnPoints;
        private Random random;
        protected int _level;
        public Level()
        {
            _positive = true;
            CreateTileArray();
            _tileHeight = _tileWidth = 16;
            _blokTileArray = new BlockStandard[_byteTileArray.GetLength(0), _byteTileArray.GetLength(1)];
            CreateWorld();
            _CollisionObjects = _blokTileArray;

            _yPlatforms = new int[5]; //RECOMMENDED LEVELS (OTHERWISE CHANGE LEVELS OF _byteTileArray ACCORDINGLY => in inherited classes that are playable levels)
            _yPlatforms[0] = 576; //bottom of platform 1
            _yPlatforms[1] = 444; //between platform 1 and 2
            _yPlatforms[2] = 334; //between platform 2 and 3
            _yPlatforms[3] = 220; //between platform 3 and 4
            _yPlatforms[4] = 0;   //top of platform 4
            _xPoints = new PointLevel[40]; //points from bottom platform to upper starting with point 0; jump right = even, jump left = uneven
            for (int i = 0; i < _xPoints.GetLength(0); i++)
            {
                _xPoints[i] = new PointLevel();
            }
            _Enemies = new List<Sprite>();                                      
            _Allies = new List<Sprite>();
            _spawnPoints = new List<Point>();
            random = new Random();

            _level = 1;
            _numberOfAllies = 3;
            _numberOfEnemies = 9;
            _gameOver = _gameOver = false;
        }

        protected abstract void CreateTileArray();
        protected void CreateWorld()
        {
            for (int x = 0; x < _byteTileArray.GetLength(0); x++)
            {
                for (int y = 0; y < _byteTileArray.GetLength(1); y++)
                {
                    if (_byteTileArray[x, y] == 1)
                    {
                        _blokTileArray[x, y] = new BlockStandard(_positive, new Point(y * _tileWidth, x * _tileHeight));
                    }
                }
            }
        }

        public virtual void Draw(Surface video)
        {
            for (int x = 0; x < _byteTileArray.GetLength(0); x++)
            {
                for (int y = 0; y < _byteTileArray.GetLength(1); y++)
                {
                    if (_blokTileArray[x, y] != null)
                    {
                        _blokTileArray[x, y].Draw(video);
                    }
                }
            }

            foreach (var enemy in _Enemies)
            {
                enemy.Draw(video);
            }
            foreach (var ally in _Allies)
            {
                ally.Draw(video);
            }
        }

        public virtual void Update()
        {
            ProjectilesUpdate(_Allies);
            ProjectilesUpdate(_Enemies);
            foreach (var enemy in _Enemies)
            {
                enemy.Update();
            }
            foreach (var ally in _Allies)
            {
                ally.Update();
            }
        }

        protected void ScanDeadAndSpawnNew(List<Sprite> spriteArrayToScan, bool canShoot)
        {
            if (spriteArrayToScan != null)
            {
                int r = random.Next(_spawnPoints.Count);
                int index = spriteArrayToScan.FindIndex(sprite => sprite._IsDead && !sprite._scannedOverDeath);
                if (index >= 0)
                {
                    spriteArrayToScan[index]._scannedOverDeath = true;
                    if (spriteArrayToScan[0].GetType() == typeof(NPC) && _numberOfEnemies > 0)
                    {                                                                                               //SPAWN ON RANDOM PLACES
                        spriteArrayToScan.Add(new NPC(_positive, _CollisionObjects, _Allies, _yPlatforms, _xPoints, _spawnPoints[r], DoStartLeft(_spawnPoints[r]), canShoot) { _SpriteToInteract = _Allies[0] }); //_SpriteToInteract needs to get the player assigned to get his information
                        if (spriteArrayToScan.Count > 50) spriteArrayToScan[0].RemoveFromList(spriteArrayToScan); //PREVENTS MEMORY REACHING MAX
                        _numberOfEnemies--;
                        if (_numberOfEnemies <= 0)
                            _gameWon = true;
                    }
                    else if (spriteArrayToScan[0].GetType() == typeof(Player) && _numberOfAllies > 0) //spawns new life for PLAYER as long there are enough lives in _numberOfAllies left
                    {
                        //spriteArrayToScan[index].RemoveFromList(spriteArrayToScan);
                        spriteArrayToScan[0] = new Player(_positive, _CollisionObjects, _Enemies, _spawnPoints[r], canShoot); //spawn random as well
                        _numberOfAllies--;
                        if (_numberOfAllies <= 0) _gameOver = true;
                    }
                }
            }
        }
        protected void ProjectilesUpdate(List<Sprite> spritesArray)
        {
            foreach (var sprite in spritesArray)
            {
                for (int i = sprite._projectiles.Count - 1; i >= 0; --i) //reverse loop for deleting elements in list
                {
                    if (sprite._projectiles[i].Hit) sprite._projectiles[i].RemoveFromList(sprite._projectiles);
                }
            }
        }
        protected bool DoStartLeft(Point pointToSpawn)
        {
            if (_Allies[0]._collisionRect.X < pointToSpawn.X) return true;
            else return false;
        }
        public void RemoveFromList(List<Level> array)
        {
            if (array == null)
                return;
            array.Remove(this);
        }
    }
}
