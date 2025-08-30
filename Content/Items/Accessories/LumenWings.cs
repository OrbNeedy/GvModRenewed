using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class LumenWings : ModItem
    {
        private float damageReduction = 10f;

        public override void SetStaticDefaults()
        {
            // These wings use the same values as the solar wings
            // Fly time: 180 ticks = 3 seconds
            // Fly speed: 9 (46 in the wiki)
            // Acceleration multiplier: 2.5
            // Fly time, Fly speed, Acceleration
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(150, 6.85f, 1.5f);
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageReduction);

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            /*ascentWhenFalling = 0.85f; // Falling glide speed
            ascentWhenRising = 0.1f; // Rising speed
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;*/
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PlayerBuffs effects = player.GetModPlayer<PlayerBuffs>();
            if (!hideVisual)
            {
                effects.dustType.AddRange(new List<WingDust> { WingDust.Lumen1, WingDust.Lumen2 });
                effects.specialWingType = SpecialWingEquip.Lumen;
            } else
            {
                if (player.velocity.Y != 0)
                {
                    effects.specialWingType = SpecialWingEquip.Lumen;
                }
            }

            if (player.GetModPlayer<ResurrectionPlayer>().resurrected)
            {
                player.endurance += damageReduction / 100f;
            }
            base.UpdateAccessory(player, hideVisual);
        }
        
        public override void UpdateVanity(Player player)
        {
            PlayerBuffs effects = player.GetModPlayer<PlayerBuffs>();
            effects.dustType.AddRange(new List<WingDust> { WingDust.Lumen1, WingDust.Lumen2 });
            effects.specialWingType = SpecialWingEquip.Lumen;
            base.UpdateVanity(player);
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ActinoBlancCrystal>(10)
                .AddIngredient<MorphoWings>()
                .AddIngredient(ItemID.ChlorophyteBar, 8)
                .AddTile(TileID.MythrilAnvil)
                .SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
                .Register();
        }
    }
}
