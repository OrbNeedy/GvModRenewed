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
            player.GetArmorPenetration<SpecialAttackDamage>() += 100;
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == ModContent.GetInstance<SeptimaDamage>() || damageClass == DamageClass.Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == ModContent.GetInstance<SeptimaDamage>() || damageClass == DamageClass.Generic)
                return true;

            return base.GetEffectInheritance(damageClass);
        }

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            if (damageClass == ModContent.GetInstance<SeptimaDamage>() || damageClass == DamageClass.Generic)
                return true;

            return base.GetsPrefixesFor(damageClass);
        }
    }
}
