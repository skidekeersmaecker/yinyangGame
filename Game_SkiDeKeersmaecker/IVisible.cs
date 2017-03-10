using SdlDotNet.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_SkiDeKeersmaecker
{
    interface IVisible
    {
        void Update();
        void Draw(Surface video);
    }
}
