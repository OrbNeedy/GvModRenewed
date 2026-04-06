using GvMod.Common.Players.Sevenths;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace GvMod.Content.Items.Corpses
{
    class SlimeCorpse : ModItem
    {
        int colorTimer = 90;
        int oldColorIndex = 0;
        int colorIndex = 0;
        public static Color[] slimeCorpseColorList = { 
            Color.White, 
            Color.DarkCyan, 
            Color.GreenYellow, 
            Color.MediumPurple, 
            Color.DarkRed, 
            Color.DarkGray, 
            Color.Orange,
            Color.Tan
        };

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Default;

            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1;
            Item.value = Item.sellPrice(0, 0, 0, 10);
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.ammo = Item.type;
        }

        public override void UpdateInventory(Player player)
        {
            ColorUpdate();
        }

        public override void PostUpdate()
        {
            ColorUpdate();
        }

        private void ColorUpdate()
        {
            Item.color = Color.Lerp(slimeCorpseColorList[oldColorIndex], slimeCorpseColorList[colorIndex], colorTimer / 90f);

            if (colorTimer >= 90)
            {
                oldColorIndex = colorIndex;
                colorIndex = Main._rand.Next(0, slimeCorpseColorList.Length);
                colorTimer = 0;
            }
            else
            {
                colorTimer++;
            }
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
                .AddIngredient(ItemID.Gel, 4)
                .Register();

            CreateRecipe(10)
                .AddIngredient(ItemID.Gel, 4)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
