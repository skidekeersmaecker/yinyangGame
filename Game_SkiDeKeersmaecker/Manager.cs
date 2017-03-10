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
    public class Manager
    {
        private Surface video;
        private int videoTile = 64;
        private int videoHeight = 9; //voor goede resolutieverhoudingen te setten
        private int videoWidth = 16; // ^
        private Surface startScreen;
        private Surface endScreen;
        private Surface[] endScreens;
        private Level activeLevel;
        private int level;
        private List<Level> levels;
        private bool start;
        private bool end;
        
        public Manager()
        {
            //INIT VIDEO
            videoHeight *= videoTile; //nu 576
            videoWidth *= videoTile; //nu 1024
            video = Video.SetVideoMode(videoWidth, videoHeight);
            startScreen = new Surface("StartScreen.png");
            endScreen = new Surface("EndScreenGameOverDefault.png");
            endScreens = new Surface[10];
            for (int i = 0; i < endScreens.GetLength(0); i++)
            {
                endScreens[i] = new Surface(i + 1 + ".png");
            }
            level = 1;
            levels = new List<Level>();
            levels.Add(new LevelYin(level));
            levels.Add(new LevelYang(level));
            activeLevel = levels[0];
            start = end = false;

            Events.KeyboardDown += Events_KeyboardDownManager;

            Events.Tick += Events_Tick;
            Events.Fps = 20;
            Events.Run();
        }

        private void Events_KeyboardDownManager(object sender, SdlDotNet.Input.KeyboardEventArgs e)
        {
            start = true;
            if (end) { end = start = false; ResetGame(); }
        }

        private void Events_Tick(object sender, TickEventArgs e)
        {
            if (activeLevel._Positive) video.Fill(Color.Black); else video.Fill(Color.White);
            if (start && !end) activeLevel.Update();
            activeLevel.Draw(video);
            if (!start && !end) video.Blit(startScreen);
            else if (start && end) video.Blit(endScreen);
            SetLevels();
            video.Update();
            Console.WriteLine("level" + level + " number to kill: " + activeLevel._NumberOfEnemies + " lives left: " + activeLevel._NumberOfAllies);
        }

        private void SetLevels()
        {
            if (levels[0]._GameWon) activeLevel = levels[1];
            if (levels[1]._GameWon)
            {
                level++;
                levels.Add(new LevelYin(level));
                levels.Add(new LevelYang(level));
                levels[0].RemoveFromList(levels);
                levels[0].RemoveFromList(levels); // 1 wordt 0 na dat de vorige verwijderd is
                activeLevel = levels[0];
            }
            if (levels[0]._GameOver || levels[1]._GameOver) end = true;

            //setBackground:
            switch (level)
            {
                case 1:
                    endScreen = endScreens[0];
                    break;
                case 2:
                    endScreen = endScreens[1];
                    break;
                case 3:
                    endScreen = endScreens[2];
                    break;
                case 4:
                    endScreen = endScreens[3];
                    break;
                case 5:
                    endScreen = endScreens[4];
                    break;
                case 6:
                    endScreen = endScreens[5];
                    break;
                case 7:
                    endScreen = endScreens[6];
                    break;
                case 8:
                    endScreen = endScreens[7];
                    break;
                case 9:
                    endScreen = endScreens[8];
                    break;
                case 10:
                    endScreen = endScreens[9];
                    break;
            }
        }
        private void ResetGame()
        {
            level = 1;
            levels.Add(new LevelYin(level));
            levels.Add(new LevelYang(level));
            levels[0].RemoveFromList(levels);
            levels[0].RemoveFromList(levels);
            activeLevel = levels[0];
        }
    }
}
