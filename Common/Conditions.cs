using GvMod.Common.GlobalNPCs;
using GvMod.Common.Players;
using GvMod.Common.Systems;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace GvMod.Common
{
    public static class CustomConditions
    {
        public const int MinDragonVeinDistance = 64;

        public static Condition NearDragonVein = new Condition("Mods.GvMod.Conditions.MinimalDistance", 
            () => ModContent.GetInstance<DragonVeinsSystem>().CheckPlayerDistance(Main.LocalPlayer, MinDragonVeinDistance));

        public static Condition AnyDragonVein = new Condition("Mods.GvMod.Conditions.AnyDragonVein",
            () => Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().DragonVeinsVisited.Any(dv => dv));

        public static Condition FirstDragonVein = new Condition("Mods.GvMod.Conditions.SpecificDragonVein",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.DragonVeinsVisited[0]);

        public static Condition SecondDragonVein = new Condition("Mods.GvMod.Conditions.SpecificDragonVein",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.DragonVeinsVisited[1]);

        public static Condition ThirdDragonVein = new Condition("Mods.GvMod.Conditions.SpecificDragonVein",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.DragonVeinsVisited[2]);

        public static Condition FourthDragonVein = new Condition("Mods.GvMod.Conditions.SpecificDragonVein",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.DragonVeinsVisited[3]);

        public static Condition FifthDragonVein = new Condition("Mods.GvMod.Conditions.SpecificDragonVein",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.DragonVeinsVisited[4]);

        public static Condition AdeptStage2 = new Condition("Mods.GvMod.Conditions.AdeptStage2",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.Stage >= 2);

        public static Condition AdeptStage3 = new Condition("Mods.GvMod.Conditions.AdeptStage3",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.Stage >= 3);

        public static Condition AdeptStage4 = new Condition("Mods.GvMod.Conditions.AdeptStage4",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.Stage >= 4);

        public static Condition AdeptStage5 = new Condition("Mods.GvMod.Conditions.AdeptStage5",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.Stage >= 5);

        public static Condition AdeptStage6 = new Condition("Mods.GvMod.Conditions.AdeptStage6",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.Stage >= 6);

        public static Condition AdeptStage7 = new Condition("Mods.GvMod.Conditions.AdeptStage7",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.Stage >= 7);

        public static Condition AdeptStage8 = new Condition("Mods.GvMod.Conditions.AdeptStage8",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.Stage >= 8);

        public static Condition AdeptStage9 = new Condition("Mods.GvMod.Conditions.AdeptStage9",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.Stage >= 9);

        public static Condition AdeptStage10 = new Condition("Mods.GvMod.Conditions.AdeptStage10",
            () => Main.LocalPlayer.TryGetModPlayer(out SeptimaPlayer plr) && plr.Stage >= 10);
    }

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

    public class NotBeatenAnyMechBoss : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            return !NPC.downedMechBossAny;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            // TODO: Get the translations for this string
            return "Anyone if the player lacks EP.";
        }
    }
}
