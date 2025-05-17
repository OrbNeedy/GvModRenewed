using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content
{
    public class SeptimaDamage : DamageClass
    {
        public override void SetDefaultStats(Player player)
        {
            player.GetCritChance<SeptimaDamage>() += 0.05f;
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }
    }
}
