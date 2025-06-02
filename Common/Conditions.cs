using GvMod.Common.GlobalNPCs;
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
            return "When killing a resurrected enemy.";
        }
    }
}
