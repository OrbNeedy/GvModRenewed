using GvMod.Content.Items.Ammo;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Weapons
{
    public class RuinedDartGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new Vector2(30, 24);
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 1;
            Item.knockBack = 0;
            Item.rare = ItemRarityID.Green;

            Item.shoot = ModContent.ProjectileType<HairDartProjectile>();
            Item.useAmmo = ModContent.ItemType<HairDart>();
            Item.shootSpeed = 10;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.UseSound = new SoundStyle("GvMod/Assets/Sfx/GunShot_G") with
            {
                Volume = 0.5f,
                PitchVariance = 0.1f
            };
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = 1;
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 18)
                .AddIngredient(ItemID.IllegalGunParts)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddRecipeGroup("CopperBar", 7)
                .AddRecipeGroup(RecipeGroupID.IronBar, 18)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddRecipeGroup("CopperBar", 7)
                .AddRecipeGroup(RecipeGroupID.IronBar, 18)
                .AddRecipeGroup("GoldBar", 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
