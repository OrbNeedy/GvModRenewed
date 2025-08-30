using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class JouleWings : ModItem
    {
        private float damageReduction = 15f;
        private float extraDamage = 20f;

        public override void SetStaticDefaults()
        {
            // These wings use the same values as the solar wings
            // Fly time: 180 ticks = 3 seconds
            // Fly speed: 9 (46 in the wiki)
            // Acceleration multiplier: 2.5
            // Fly time, Fly speed, Acceleration
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 9f, 2.5f, true, 
                hoverAccelerationMultiplier: 3f);
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Purple;

            Item.accessory = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageReduction, extraDamage);

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.8f; // Falling glide speed
            ascentWhenRising = 0.1f; // Rising speed
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PlayerBuffs effects = player.GetModPlayer<PlayerBuffs>();
            if (!hideVisual)
            {
                effects.specialWingType = SpecialWingEquip.Joule;
            }

            if (player.velocity.Y != 0)
            {
                if (hideVisual)
                {
                    effects.specialWingType = SpecialWingEquip.Joule;
                }
            }

            if (player.GetModPlayer<ResurrectionPlayer>().resurrected || 
                player.GetModPlayer<SeptimaPlayer>().DnizerMode)
            {
                player.endurance += damageReduction / 100f;
                player.GetDamage<SeptimaDamage>() += extraDamage / 100f;
            }
            base.UpdateAccessory(player, hideVisual);
        }

        public override void UpdateVanity(Player player)
        {
            PlayerBuffs effects = player.GetModPlayer<PlayerBuffs>();
            effects.specialWingType = SpecialWingEquip.Joule;
            base.UpdateVanity(player);
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PulsarFragment>(14)
                .AddIngredient<LumenWings>()
                .AddIngredient(ItemID.LunarBar, 10)
                .AddTile(TileID.LunarCraftingStation)
                .SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
                .Register();
        }
    }
}
