using GvMod.Common.GlobalNPCs;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace GvMod.Common
{
    public class MirrorShardDropCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            return info.npc.GetGlobalNPC<BossResurrection>().resurrected;
        }

        public bool CanShowItemDropInUI()
        {
            return false;
        }

        public string GetConditionDescription()
        {
            // TODO: Get the translations for this string
            return "From a resurrected enemy.";
        }
    }

    public class RarityDropCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        int minRarity = 4;

        public RarityDropCondition(int minRarity)
        {
            this.minRarity = minRarity;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return info.npc.rarity >= minRarity || (NPC.downedMoonlord && info.npc.lifeMax > 10);
        }

        public bool CanShowItemDropInUI()
        {
            return NPC.downedMoonlord;
        }

        public string GetConditionDescription()
        {
            // TODO: Get the translations for this string
            return "Rarely from anyone.";
        }
    }
}
