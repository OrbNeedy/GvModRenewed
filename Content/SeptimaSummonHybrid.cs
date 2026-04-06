using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content
{
    public class SeptimaSummonHybrid : DamageClass
    {
        public override void SetDefaultStats(Player player)
        {
            player.GetCritChance<SeptimaSummonHybrid>() += 0.05f;
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic || damageClass == ModContent.GetInstance<SeptimaDamage>() || 
                damageClass == ModContent.GetInstance<SecondaryAttackDamage>())
                return StatInheritanceData.Full;
            if (damageClass == Summon)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic || damageClass == ModContent.GetInstance<SeptimaDamage>() || 
                damageClass == ModContent.GetInstance<SecondaryAttackDamage>())
                return true;
            if (damageClass == Summon)
                return true;

            return base.GetEffectInheritance(damageClass);
        }

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic || damageClass == ModContent.GetInstance<SeptimaDamage>() || 
                damageClass == ModContent.GetInstance<SecondaryAttackDamage>())
                return true;
            if (damageClass == Summon)
                return true;

            return base.GetsPrefixesFor(damageClass);
        }
    }
}
