using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using GvMod.Common.Players.Sevenths;

namespace GvMod.Content.Items.Corpses
{
    class PixieCorpse : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Default;

            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1;
            Item.value = Item.sellPrice(0, 0, 0, 10);
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.ammo = Item.type;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += Rebirth.ProjectileTable[Rebirth.CorpseItemTable[Type]].baseDamage - 1;
        }

        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
        {
            knockback += Rebirth.ProjectileTable[Rebirth.CorpseItemTable[Type]].baseKnockback - 1;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PixieDust, 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
