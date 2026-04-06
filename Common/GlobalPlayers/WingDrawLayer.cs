using GvMod.Common.Players;
using GvMod.Content.Items;
using GvMod.Content.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalPlayers
{
    public class WingDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return PlayerDrawLayers.Wings.GetDefaultPosition();
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead || drawInfo.hideEntirePlayer || (drawInfo.drawPlayer.wings !=
                EquipLoader.GetEquipSlot(Mod, "LumenWings", EquipType.Wings) && drawInfo.drawPlayer.wings !=
                EquipLoader.GetEquipSlot(Mod, "MorphoWings", EquipType.Wings) && drawInfo.drawPlayer.wings !=
                EquipLoader.GetEquipSlot(Mod, "JouleWings", EquipType.Wings)))
            {
                return;
            }
            Vector2 directions = drawInfo.drawPlayer.Directions;
            Vector2 vector = drawInfo.Position - Main.screenPosition + drawInfo.drawPlayer.Size / 2f;
            Vector2 vector2 = new Vector2(0f, 7f);
            vector = drawInfo.Position - Main.screenPosition + new Vector2(drawInfo.drawPlayer.width / 2, 
                drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height / 2) + vector2;

            SpecialWingEquip specialWingType = drawInfo.drawPlayer.GetModPlayer<PlayerBuffs>().
                specialWingType;

            DrawData item;
            Asset<Texture2D> sprite;

            switch (specialWingType)
            {
                case SpecialWingEquip.Morpho:
                    Vector2 vec2 = vector + new Vector2(-10, -10) * directions;
                    Color color3 = Color.White * (1f - drawInfo.shadow);

                    sprite = ModContent.Request<Texture2D>("GvMod/Content/Items/Accessories/MorphoWings_Custom");
                    item = new DrawData(
                        sprite.Value, 
                        vec2.Floor(), 
                        new Rectangle(0, 0, sprite.Width(), sprite.Height()), 
                        color3, 
                        drawInfo.drawPlayer.bodyRotation, 
                        new Vector2(sprite.Width() / 2, sprite.Height() / 2), 
                        1f, 
                        drawInfo.playerEffect);
                    item.shader = drawInfo.cWings;
                    drawInfo.DrawDataCache.Add(item);
                    break;
                case SpecialWingEquip.Lumen:
                    vec2 = vector + new Vector2(-10, -16) * directions;
                    color3 = Color.White * (1f - drawInfo.shadow);

                    sprite = ModContent.Request<Texture2D>("GvMod/Content/Items/Accessories/LumenWings_Custom");
                    item = new DrawData(
                        sprite.Value,
                        vec2.Floor(),
                        new Rectangle(0, 0, sprite.Width(), sprite.Height()),
                        color3,
                        drawInfo.drawPlayer.bodyRotation,
                        new Vector2(sprite.Width() / 2, sprite.Height() / 2),
                        1f,
                        drawInfo.playerEffect);
                    item.shader = drawInfo.cWings;
                    drawInfo.DrawDataCache.Add(item);
                    break;
                case SpecialWingEquip.Joule:
                    vec2 = vector + new Vector2(-10, -24) * directions;
                    color3 = Color.White * (1f - drawInfo.shadow);

                    sprite = ModContent.Request<Texture2D>("GvMod/Content/Items/Accessories/JouleWings_Custom");
                    
                    item = new DrawData(
                        sprite.Value,
                        vec2.Floor(),
                        new Rectangle(0, 0, sprite.Width(), sprite.Height()),
                        color3,
                        drawInfo.drawPlayer.bodyRotation,
                        new Vector2(sprite.Width() / 2, sprite.Height() / 2),
                        1f,
                        drawInfo.playerEffect);

                    if (drawInfo.drawPlayer.GetModPlayer<SeptimaPlayer>().SuperState)
                    {
                        item.shader = GameShaders.Armor.GetShaderIdFromItemId(
                            ModContent.ItemType<PowerfulDye>());
                    } else
                    {
                        item.shader = drawInfo.cWings;
                    }

                    drawInfo.DrawDataCache.Add(item);
                    break;
                default:
                case SpecialWingEquip.None:
                    break;
            }
        }
    }
}
