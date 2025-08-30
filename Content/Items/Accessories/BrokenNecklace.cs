using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GvMod.Content.Items.Accessories
{
    public class BrokenNecklace : ModItem
    {
        public bool wasBroken = false;

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            EquipLoader.AddEquipTexture(Mod, $"GvMod/Content/Items/Accessories/GemstoneNecklace_Neck", 
                EquipType.Neck, this, name: "GemstoneNecklace");
            EquipLoader.AddEquipTexture(Mod, $"GvMod/Content/Items/Accessories/BrokenNecklace_Neck",
                EquipType.Neck, name: "BrokenNecklace");
            base.Load();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Gray;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["UpgradeFlag"] = wasBroken;
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            int equipSlotGemstoneNeck = EquipLoader.GetEquipSlot(Mod, "GemstoneNecklace",
                EquipType.Neck);
            Item.neckSlot = equipSlotGemstoneNeck;

            if (tag.ContainsKey("UpgradeFlag"))
            {
                wasBroken = tag.GetBool("UpgradeFlag");
                
                if (wasBroken)
                {
                    equipSlotGemstoneNeck = EquipLoader.GetEquipSlot(Mod, "BrokenNecklace",
                        EquipType.Neck);
                    Item.neckSlot = equipSlotGemstoneNeck;

                    Item.rare = ItemRarityID.Lime;
                }
            }
            base.LoadData(tag);
        }

        public override LocalizedText Tooltip => base.Tooltip;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");
            if (index != -1)
            {
                if (wasBroken)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.BrokenNecklace.UpgradeTooltip"));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.BrokenNecklace.TrueTooltip"));
                }
            }

            if (wasBroken)
            {
                index = tooltips.FindLastIndex((x) => x.Name.StartsWith("ItemName") && x.Mod == "Terraria");
                if (index != -1)
                {
                    tooltips[index].Text = Language.
                        GetTextValue("Mods.GvMod.Items.BrokenNecklace.TrueName");
                }
            }
        }

        public override void PostUpdate()
        {
            if (wasBroken)
            {
                //Item.rare = ItemRarityID.Lime;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (wasBroken)
            {
                //Item.rare = ItemRarityID.Lime;
            }
        }

        public override void UpdateEquip(Player player)
        {
            //Main.NewText("Mirror Shard's UpdateEquip.");
            //Main.NewText("Broken necklace: " + wasBroken);
            ResurrectionPlayer resurrection = player.GetModPlayer<ResurrectionPlayer>();
            if (wasBroken)
            {
                resurrection.canResurrect = true;
                resurrection.resurrectionPower = 2;
            } else
            {
                resurrection.wearingNecklace = true;
                wasBroken = resurrection.breakNecklace > 0;

                if (wasBroken)
                {
                    int equipSlotGemstoneNeck = EquipLoader.GetEquipSlot(Mod, "BrokenNecklace",
                        EquipType.Neck);
                    Item.neckSlot = equipSlotGemstoneNeck;

                    Item.rare = ItemRarityID.Lime;
                }

                /*Main.NewText("Wearing Necklace: " + resurrection.wearingNecklace);
                Main.NewText("Recently broken: " + wasBroken);
                Main.NewText("Player's perspective on that: " + resurrection.breakNecklace);*/
            }
            base.UpdateEquip(player);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //Main.NewText("Mirror Shard's UpdateAccessory.");
            if (wasBroken)
            {
                /*int equipSlotGemstoneNeck = EquipLoader.GetEquipSlot(Mod, "BrokenNecklace",
                    EquipType.Neck);
                Item.neckSlot = equipSlotGemstoneNeck;*/

                if (hideVisual) player.GetModPlayer<ResurrectionPlayer>().type = AnthemAuraType.Invisible;
                else player.GetModPlayer<ResurrectionPlayer>().type = AnthemAuraType.Lumen;
            } else
            {
            }
            base.UpdateAccessory(player, hideVisual);
        }

        public override void UpdateVanity(Player player)
        {
            /*if (wasBroken)
            {
                int equipSlotGemstoneNeck = EquipLoader.GetEquipSlot(Mod, "BrokenNecklace",
                    EquipType.Neck);
                Item.neckSlot = equipSlotGemstoneNeck;
            } else
            {
                int equipSlotGemstoneNeck = EquipLoader.GetEquipSlot(Mod, "GemstoneNecklace",
                    EquipType.Neck);
                Item.neckSlot = equipSlotGemstoneNeck;
            }*/
            base.UpdateVanity(player);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!wasBroken)
            {
                Asset<Texture2D> glowmask = ModContent.Request<Texture2D>("GvMod/Content/Items/" +
                    "Accessories/GemstoneNecklace");

                spriteBatch.Draw(
                    glowmask.Value,
                    position,
                    frame,
                    Color.White,
                    0f,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0
                );
                return false;
            }
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (!wasBroken)
            {
                Asset<Texture2D> glowmask = ModContent.Request<Texture2D>("GvMod/Content/Items/" +
                    "Accessories/GemstoneNecklace");

                spriteBatch.Draw(
                    glowmask.Value,
                    new Vector2
                    (
                        Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                        Item.position.Y - Main.screenPosition.Y - glowmask.Height() * 0.5f
                    ),
                    new Rectangle(0, 0, glowmask.Width(), glowmask.Height()),
                    lightColor,
                    rotation,
                    glowmask.Size() * 0.5f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
                return false;
            }
            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        // Preventively adding this to avoid allowing other resurrection items override each other
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not DjinnBunny && equippedItem.ModItem is not MirrorShard &&
                incomingItem.ModItem is not DjinnBunny && incomingItem.ModItem is not MirrorShard;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 20)
                .AddIngredient<MysteriousGemstone>(7)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
