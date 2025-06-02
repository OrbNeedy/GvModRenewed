using GvMod.Common.Players;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using GvMod.Content.Buffs;

namespace GvMod.Common.GlobalPlayers
{
    public class ResurrectionEffectLayer : PlayerDrawLayer
    {
        private int frame = 0;
        private int frameTimer = 0;

        public override Position GetDefaultPosition()
        {
            return PlayerDrawLayers.AfterLastVanillaLayer;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0) return;

            Player player = drawInfo.drawPlayer;
            if (player.DeadOrGhost) return;

            ResurrectionPlayer resurrection = player.GetModPlayer<ResurrectionPlayer>();
            if (!player.HasBuff<Anthem>()) return;

            Asset<Texture2D> aura = ModContent.
                    Request<Texture2D>($"GvMod/Assets/Effects/{resurrection.type.ToString()}");
            Vector2 boundSize = new Vector2(82, 82);
            Vector2 offset = new Vector2(0, -10);
            int maxFrames = 4;

            switch (resurrection.type)
            {
                case AnthemAuraType.LumenWeak:
                case AnthemAuraType.Lumen:
                    boundSize = new Vector2(82, 82);
                    offset = new Vector2(0, -10);
                    maxFrames = 4;
                    break;
            }

            frameTimer++;
            if (frameTimer >= 4)
            {
                frame++;
                frameTimer = 0;
                if (frame >= maxFrames)
                {
                    frame = 0;
                }
            }

            drawInfo.DrawDataCache.Add(new DrawData(
                aura.Value,
                player.MountedCenter - Main.screenPosition + offset,
                new Rectangle(0, (int)(boundSize.Y * frame), (int)boundSize.X, (int)boundSize.Y),
                Color.White * 0.5f,
                0,
                boundSize / 2,
                1f,
                SpriteEffects.None
                ));
        }
    }
}
