using GvMod.Common.Players;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using GvMod.Common.Players.Sevenths;

namespace GvMod.Content.Items.Tools
{
    class CorpsePreservingGuide : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 1;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.UseSound = SoundID.Item15; // 29, 4, 92
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.consumable = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                if (Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().septima is Rebirth)
                {
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "RebirthSadism",
                        Language.GetTextValue("Mods.GvMod.RebirthSadism")));

                    if (Main.LocalPlayer.GetModPlayer<PlayerBuffs>().CorpseCollectionAllowed)
                    {
                        tooltips.Insert(index + 2, new TooltipLine(Mod, "CorpseCollectionState",
                            Language.GetTextValue("Mods.GvMod.CorpseCollection.Allowed")));
                    }
                    else
                    {
                        tooltips.Insert(index + 2, new TooltipLine(Mod, "CorpseCollectionState",
                            Language.GetTextValue("Mods.GvMod.CorpseCollection.Disabled")));
                    }
                }
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                return player.GetModPlayer<SeptimaPlayer>().septimaType == SeptimaType.Rebirth;
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                PlayerBuffs adept = player.GetModPlayer<PlayerBuffs>();
                adept.CorpseCollectionAllowed = !adept.CorpseCollectionAllowed;
            }
            return null;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book)
                .Register();
        }
    }
}
