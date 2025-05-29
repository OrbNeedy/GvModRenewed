using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace GvMod.Common.Systems
{
    public class ItemGroups : ModSystem
    {
        // Copied from the Len'en mod (I am the author (Check that one out too))
        public override void AddRecipeGroups()
        {
            RecipeGroup group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} " +
                $"{Lang.GetItemNameValue(ItemID.IronBar)}", ItemID.IronBar, ItemID.LeadBar);

            RecipeGroup.RegisterGroup("IronBar", group);

            RecipeGroup group2 = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} " +
                $"{Lang.GetItemNameValue(ItemID.PlatinumBar)}", ItemID.PlatinumBar, ItemID.GoldBar);

            RecipeGroup.RegisterGroup("GoldBar", group2);

            RecipeGroup group3 = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} " +
                $"{Lang.GetItemNameValue(ItemID.CrimtaneBar)}", ItemID.CrimtaneBar, ItemID.DemoniteBar);

            RecipeGroup.RegisterGroup("CrimtaneBar", group3);

            RecipeGroup group4 = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} " +
                $"{Lang.GetItemNameValue(ItemID.BloodButcherer)}", ItemID.BloodButcherer, ItemID.LightsBane);

            RecipeGroup.RegisterGroup("BloodButcherer", group4);

            RecipeGroup group5 = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} " +
                $"{Lang.GetItemNameValue(ItemID.MythrilBar)}", ItemID.MythrilBar, ItemID.OrichalcumBar);

            RecipeGroup.RegisterGroup("MythrilBar", group5);

            RecipeGroup group6 = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} " +
                $"{Lang.GetItemNameValue(ItemID.Ichor)}", ItemID.Ichor, ItemID.CursedFlame);

            RecipeGroup.RegisterGroup("EvilBiomeHardmodeMaterial", group6);
        }
    }
}
