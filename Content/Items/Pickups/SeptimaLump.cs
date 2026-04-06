using GvMod.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Pickups
{
    public class SeptimaLump : ModItem
    {
        public override void SetDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(2, 2));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.White;
            Item.maxStack = 999;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.UseSound = SoundID.Item15; // 29, 4, 92
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.consumable = true;
        }

        public override bool OnPickup(Player player)
        {
            Rectangle r = new Rectangle((int)player.position.X - 50, (int)player.position.Y - 50, 100, 100);
            CombatText.NewText(r, Color.CadetBlue, 15 * Item.stack);
            player.GetModPlayer<SeptimaPlayer>().CurrentEP += 15f * Item.stack;
            return false;
        }

        public override bool CanStackInWorld(Item source)
        {
            return false;
        }
    }
}
