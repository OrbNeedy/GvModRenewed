using GvMod.Common.Players.Sevenths;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Corpses
{
    class BatCorpse : ModItem
    {
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
    }
}
