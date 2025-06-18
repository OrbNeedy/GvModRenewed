using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace GvMod.Common.Systems
{
    public class TimeModificationSystem : ModSystem
    {
        public bool stoppingTime = false;

        public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
        {
            if (stoppingTime)
            {
                timeRate = 0;
                eventUpdateRate = 0;
                stoppingTime = false;
            }
            base.ModifyTimeRate(ref timeRate, ref tileUpdateRate, ref eventUpdateRate);
        }
    }
}
