using GvMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    public class MirrorShard : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
        }

        public override void UpdateEquip(Player player)
        {
            //Main.NewText("Mirror Shard's UpdateEquip.");
            player.GetModPlayer<ResurrectionPlayer>().canResurrect = true;
            player.GetModPlayer<ResurrectionPlayer>().resurrectionPower = 1;
            base.UpdateEquip(player);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //Main.NewText("Mirror Shard's UpdateAccessory.");
            if (hideVisual) player.GetModPlayer<ResurrectionPlayer>().type = AnthemAuraType.Invisible;
            else player.GetModPlayer<ResurrectionPlayer>().type = AnthemAuraType.LumenWeak;
            base.UpdateAccessory(player, hideVisual);
        }

        // Preventively adding this to avoid allowing other resurrection items override each other
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }
    }
}
