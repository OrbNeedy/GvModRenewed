using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content
{
    public class SpecialAttackDamage : DamageClass
    {
        public override void SetDefaultStats(Player player)
        {
            player.GetCritChance<SpecialAttackDamage>() += 0.05f;
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == ModContent.GetInstance<SeptimaDamage>())
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }
    }
}
