using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Physics;

namespace GvMod.Common.Players
{
    public class PlayerBuffs : ModPlayer
    {
        public bool AlchemicalField { get; set; }
        public bool InfiniteSurge { get; set; }

        public override void ResetEffects()
        {
            AlchemicalField = false;
            InfiniteSurge = false;
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            if (AlchemicalField)
            {
                healValue *= 2;
            }
        }

        public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
        {
            if (AlchemicalField)
            {
                healValue *= 2;
            }
        }

        public override bool OnPickup(Item item)
        {
            return base.OnPickup(item);
        }
    }
}
