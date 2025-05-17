using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.Audio;
using GvMod.Content.Items.Ammo;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Weapons
{
    // Naga, Mizuchi, Technos, Orochi, Vasuki, Dullahan, and Ouroboros
    public enum DartLeaderUpgrades
    {
        Naga, 
        Mizuchi, 
        Technos, 
        Orochi, 
        Vasuki, 
        Dullahan, 
        Ouroboros
    }
    public class DartLeader : ModItem
    {
        private int orochiTimer = 0;

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 1;
            Item.knockBack = 0;
            Item.rare = ItemRarityID.LightRed;

            Item.shoot = ModContent.ProjectileType<HairDartProjectile>();
            Item.useAmmo = ModContent.ItemType<HairDart>();
            Item.shootSpeed = 10;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.UseSound = new SoundStyle("GvMod/Assets/Sfx/GunShot_G") with
            {
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
                .AddRecipeGroup(RecipeGroupID.IronBar, 10)
                .AddIngredient(ItemID.Revolver)
                .AddIngredient(ItemID.IllegalGunParts)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
