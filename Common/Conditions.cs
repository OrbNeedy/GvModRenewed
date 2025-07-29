using GvMod.Common.GlobalNPCs;
using GvMod.Common.Players;
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
            return info.npc.rarity >= minRarity;
        }

        public bool CanShowItemDropInUI()
        {
            return false;
        }

        public string GetConditionDescription()
        {
            // TODO: Get the translations for this string
            return "From very rare enemies.";
        }
    }

    public class MinLifeDropCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        int minLife = 10;

        public MinLifeDropCondition(int minLife)
        {
            this.minLife = minLife;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return info.npc.lifeMax >= minLife && NPC.downedMoonlord;
        }

        public bool CanShowItemDropInUI()
        {
            return false;
        }

        public string GetConditionDescription()
        {
            // TODO: Get the translations for this string
            return "Rarely from anyone after moonlord is defeated.";
        }
    }

    public class AfterPlanteraDropCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            return NPC.downedPlantBoss;
        }

        public bool CanShowItemDropInUI()
        {
            return false;
        }

        public string GetConditionDescription()
        {
            // TODO: Get the translations for this string
            return "After Plantera is defeated.";
        }
    }

    public class SeptimaLumpCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            SeptimaPlayer adept = info.player.GetModPlayer<SeptimaPlayer>();
            return adept.CurrentEP < adept.GetTotalMaxEP();
        }

        public bool CanShowItemDropInUI()
        {
            return false;
        }

        public string GetConditionDescription()
        {
            // TODO: Get the translations for this string
            return "Anyone if the player lacks EP.";
        }
    }
}
