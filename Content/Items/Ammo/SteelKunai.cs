using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Ammo
{
    public class SteelKunai : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = DamageClass.Default;
            Item.shoot = ModContent.ProjectileType<Kunai>();
            Item.shootSpeed = 8;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.noMelee = true;

            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.value = Item.sellPrice(0, 0, 0, 5);
            Item.rare = ItemRarityID.LightRed;
        }

        public override string Texture => GetTextureName();

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            if (player.GetModPlayer<SeptimaPlayer>().septima is Rebirth &&
                player.GetModPlayer<SeptimaPlayer>().UsingMainSkill)
            {
                return false;
            }
            for (int i = -1; i < 2; i++)
            {
                Projectile.NewProjectile(source, position,
                    velocity.RotatedBy(i * MathHelper.PiOver4 / 4f), type, damage,
                    knockback, player.whoAmI);
            }
            return false;
        }

        public override bool ConsumeItem(Player player)
        {
            if (player.GetModPlayer<SeptimaPlayer>().septima is Rebirth && 
                player.GetModPlayer<SeptimaPlayer>().UsingMainSkill)
            {
                return false;
            }
            return base.ConsumeItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe(35)
                .AddRecipeGroup(RecipeGroupID.IronBar)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void OnCreated(ItemCreationContext context)
        {
        }

        public string GetTextureName()
        {
            return "GvMod/Content/Projectiles/SnakeKunai";
        }
    }
}
