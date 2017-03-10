using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_SkiDeKeersmaecker
{
    class LevelYang : Level, IVisible
    {
        public LevelYang(int levelIn)
        {
            _level = levelIn;
            _numberOfEnemies = 19 * _level; //20 enemies to kill * level
            _numberOfAllies = 3; //3 lives
            _positive = false;
            CreateTileArray();
            _blokTileArray = new BlockStandard[_byteTileArray.GetLength(0), _byteTileArray.GetLength(1)];
            CreateWorld();
            _CollisionObjects = _blokTileArray;

            //Sprites in level:
            //SPAWNPOINTS    => first points are for enemies (first enemy = first spawnpoint, second =  second, ...) then the allies in same sequence with player as FIRST ONE
            _spawnPoints.Add(new Point(80, 140)); //enemies
            _spawnPoints.Add(new Point(492, 140));
            _spawnPoints.Add(new Point(880, 140));
            _spawnPoints.Add(new Point(170, 364));
            _spawnPoints.Add(new Point(800, 364));
            _spawnPoints.Add(new Point(20, 492));
            _spawnPoints.Add(new Point(940, 492));
            _spawnPoints.Add(new Point(492, 252)); //allies => PLAYER (first in allies if there are allies)
            //further points to add for not-beginning spawnpoints but as possible points for enemies to spawn (eliminates safe spaces for player) adding points with same values increases the chance to spawn on that certain place
            _spawnPoints.Add(new Point(80, 140));
            _spawnPoints.Add(new Point(880, 140));
            _spawnPoints.Add(new Point(20, 492));
            _spawnPoints.Add(new Point(940, 492));

            //ENEMIES
            for (int i = 0; i < 7; i++) //for enemies who CAN'T shoot (other for-loop to add enemies who can shoot with 'true')
            {
                _Enemies.Add(new NPC(_positive, _CollisionObjects, _Allies, _yPlatforms, _xPoints, _spawnPoints[i], true, false));
            }

            //ALLIES
            _Allies.Add(new Player(_positive, _CollisionObjects, _Enemies, _spawnPoints[7], false));

            foreach (var enemy in _Enemies) //enemies get to know their information so they can interact with them
            {
                enemy._SpriteToInteract = _Allies[0]; //HEEFT DUS NOG GEEN LIST OM MEE TE INTERACTEREN
            }


            //Points for NPC to interact with:
            _xPoints[0].X = 50;                      //point 00
            _xPoints[0].JumpUpAt = true;

            _xPoints[1].X = 402;                     //point 01
            _xPoints[1].JumpUpAt = true;

            _xPoints[2].X = 563;                     //point 02
            _xPoints[2].JumpUpAt = true;

            _xPoints[3].X = 913;                     //point 03
            _xPoints[3].JumpUpAt = true;

            _xPoints[4].X = 230;                     //point 04
            _xPoints[4].TurnAt = true;

            _xPoints[5].X = 492;                     //point 05
            _xPoints[5].TurnAt = true;
            _xPoints[5].ShootAt = true;

            _xPoints[6].X = 750;                     //point 06
            _xPoints[6].TurnAt = true;

            _xPoints[7].X = 40;                      //point 07
            _xPoints[7].ShootAt = true;

            _xPoints[8].X = 940;                     //point 08
            _xPoints[8].ShootAt = true;

            _xPoints[10].X = 224;                    //point 10
            _xPoints[10].JumpUpAt = true;

            _xPoints[11].X = 737;                    //point 11
            _xPoints[11].JumpUpAt = true;

            _xPoints[12].X = 160;                    //point 12
            _xPoints[12].TurnAt = true;
            _xPoints[12].ShootAt = true;

            _xPoints[13].X = 320;                    //point 13
            _xPoints[13].TurnAt = true;
            _xPoints[13].ShootAt = true;

            _xPoints[14].X = 660;                    //point 14
            _xPoints[14].TurnAt = true;
            _xPoints[14].ShootAt = true;

            _xPoints[15].X = 820;                    //point 15
            _xPoints[15].TurnAt = true;
            _xPoints[15].ShootAt = true;

            _xPoints[20].X = 340;                    //point 20
            _xPoints[20].JumpUpAt = true;

            _xPoints[21].X = 320;                    //point 21
            _xPoints[21].IsFast = true;
            _xPoints[21].JumpUpAt = true;

            _xPoints[22].X = 658;                    //point 22
            _xPoints[22].IsFast = true;
            _xPoints[22].JumpUpAt = true;

            _xPoints[23].X = 625;                    //point 23
            _xPoints[23].JumpUpAt = true;

            _xPoints[24].X = 320;                    //point 24
            _xPoints[24].TurnAt = true;
            _xPoints[24].ShootAt = true;

            _xPoints[25].X = 492;                    //point 25
            _xPoints[25].TurnAt = true;

            _xPoints[26].X = 660;                    //point 26
            _xPoints[26].TurnAt = true;
            _xPoints[26].ShootAt = true;

            _xPoints[30].X = 120;                    //point 30
            _xPoints[30].IsFast = true;
            _xPoints[30].JumpSameLevelAt = true;

            _xPoints[31].X = 412;                    //point 31
            _xPoints[31].IsFast = true;
            _xPoints[31].JumpSameLevelAt = true;

            _xPoints[32].X = 568;                    //point 32
            _xPoints[32].IsFast = true;
            _xPoints[32].JumpSameLevelAt = true;

            _xPoints[33].X = 842;                    //point 33
            _xPoints[33].IsFast = true;
            _xPoints[33].JumpSameLevelAt = true;

            _xPoints[34].X = 80;                     //point 34
            _xPoints[34].TurnAt = true;

            _xPoints[35].X = 492;                    //point 35
            _xPoints[35].TurnAt = true;
            _xPoints[35].ShootAt = true;

            _xPoints[36].X = 880;                    //point 36
            _xPoints[36].TurnAt = true;

            _xPoints[37].X = 100;                    //point 36
            _xPoints[37].ShootAt = true;

            _xPoints[38].X = 110;                    //point 36
            _xPoints[38].ShootAt = true;
        }

        public override void Update()
        {
            base.Update();
            ScanDeadAndSpawnNew(_Enemies, true);
            ScanDeadAndSpawnNew(_Allies, false);
        }

        protected override void CreateTileArray()
        {
            _byteTileArray = new byte[,] //RECOMMENDED TO USE PLATFORM AT THE SAME ROWS (OTHERWISE CHANGE Y-VALUE OF PLATFORMS ACCORDINGLY)
             {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0},  //PLATFORM 3
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},  //PLATFORM 2
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0},  //PLATFORM 1
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}   //PLATFORM -
            };
        }
    }
}
