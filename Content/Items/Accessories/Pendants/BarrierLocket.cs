using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Pendants
{
    [AutoloadEquip(EquipType.Neck)]
    public class BarrierLocket : ModItem
    {
        private float damageReduction = 10;
        private float upgradedDamageReduction = 5;
        private int upgradedDefense = 6;
        private int actionDefense = 8;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.defense = 2;

            Item.rare = ItemRarityID.Green;
        }

        public override LocalizedText Tooltip => base.Tooltip;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");
            if (index != -1)
            {
                if (NPC.downedGolemBoss)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.BarrierLocket.UpgradeTooltip",
                        damageReduction + upgradedDamageReduction, actionDefense));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.BarrierLocket.TrueTooltip", damageReduction));
                }
            }

            if (NPC.downedGolemBoss)
            {
                index = tooltips.FindLastIndex((x) => x.Name.StartsWith("ItemName") && x.Mod == "Terraria");
                if (index != -1)
                {
                    tooltips[index].Text = base.DisplayName.Value + " +";
                }
            }
        }

        public override void PostUpdate()
        {
            if (NPC.downedGolemBoss)
            {
                Item.rare = ItemRarityID.Cyan;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (NPC.downedGolemBoss)
            {
                Item.rare = ItemRarityID.Cyan;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            if (NPC.downedGolemBoss)
            {
                Item.rare = ItemRarityID.Cyan;
                Item.defense = upgradedDefense;

                if (adept.UsingMainSkill || adept.UsingSecondarySkill || adept.UsingSpecialSkill)
                {
                    player.noKnockback = true;
                    player.endurance += (damageReduction + upgradedDamageReduction) / 100f;
                    player.statDefense += actionDefense;
                }
            } else
            {
                if (player.GetModPlayer<SeptimaPlayer>().UsingMainSkill)
                {
                    player.noKnockback = true;
                    player.endurance += damageReduction / 100f;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Chain, 6)
                .AddIngredient(ItemID.Ruby, 8)
                .AddIngredient<Nanochip98>(10)
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddIngredient(ItemID.Bone, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
