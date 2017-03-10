using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_SkiDeKeersmaecker
{
    public class PointLevel
    {
        private int x;
        public int X
        {
            get { return x; }
            set
            {
                x = value;
                if (value < 0) value = 0;
                else if (value > 1024) value = 1024;
            }
        }

        private bool stopAt;                //dominates all; if true: the rest false => character does absolutely nothing (trapped)
        private bool ignoreThis;            //dominates everything but stopAt (point 'doesn't' exist)
        private bool holdAt;                //dominates all movement (turnAt & jumpAt) 
        private bool turnAt;                //dominates nothing
        private bool jumpUpAt;              //dominates jumpSamePlatformAt
        private bool jumpSamePlatformAt;    //dominates nothing
        private bool runAt;                 //dominates walk only
        private bool walkAt;                //dominates nothing
        private bool shootAt;               //dominates nothing
        private bool isFast;                //dominates nothing

        public bool IsFast
        {
            get { return isFast; }
            set
            {
                if (stopAt || ignoreThis || holdAt) value = false;
                isFast = value;
            }
        }
        public bool ShootAt
        {
            get { return shootAt; }
            set
            {
                if (stopAt || ignoreThis || holdAt) value = false;
                shootAt = value;
            }
        }
        public bool WalkAt
        {
            get { return walkAt; }
            set
            {
                if (stopAt || ignoreThis || holdAt || runAt) value = false;
                walkAt = value;
            }
        }
        public bool RunAt
        {
            get { return runAt; }
            set
            {
                if (stopAt || ignoreThis || holdAt) value = false;
                runAt = value;
            }
        }
        public bool JumpUpAt
        {
            get { return jumpUpAt; }
            set
            {
                if (stopAt || ignoreThis || holdAt) value = false;
                jumpUpAt = value;
            }
        }
        public bool JumpSameLevelAt
        {
            get { return jumpSamePlatformAt; }
            set
            {
                if (stopAt || ignoreThis || holdAt || jumpUpAt) value = false;
                jumpSamePlatformAt = value;
            }
        }
        public bool TurnAt
        {
            get { return turnAt; }
            set
            {
                if (stopAt || ignoreThis || holdAt) value = false;
                turnAt = value;
            }
        }
        public bool HoldAt
        {
            get { return holdAt; }
            set
            {
                if (stopAt || ignoreThis) value = false;
                holdAt = value;
            }
        }
        public bool IgnoreThis
        {
            get { return ignoreThis; }
            set
            {
                if (StopAt) value = false;
                ignoreThis = value;
            }
        }
        public bool StopAt
        {
            get { return stopAt; }
            set { stopAt = value; }
        }

        public PointLevel()
        {
            x = 0;
            ignoreThis = turnAt = jumpUpAt = stopAt = shootAt = false;
        }

    }
}
