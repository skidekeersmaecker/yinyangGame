using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_SkiDeKeersmaecker
{
    public class BlockStandard : Block, IVisible
    {
        public BlockStandard(bool positiveIn, Point positionIn) : base(positiveIn, positionIn)
        {
            _Positive = positiveIn;
            _position = positionIn;
        }

        public override void Draw(Surface video)
        {
            base.Update();
            video.Blit(_imageActive, _position);
        }
    }
}
