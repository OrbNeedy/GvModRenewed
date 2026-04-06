using GvMod.Common.Players;
using GvMod.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalPlayers
{
    public class PrevasionDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.BeetleBuff, PlayerDrawLayers.EyebrellaCloud);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0) return;

            Player player = drawInfo.drawPlayer;
            if (player.DeadOrGhost) return;

            PlayerPrevasion prevasion = player.GetModPlayer<PlayerPrevasion>();

            if (prevasion.PrevasionIframes > 0)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();

                int time = prevasion.BasePrevasionIframes - prevasion.PrevasionIframes;
                float displacement = (float)Math.Sin(time / 8) * prevasion.PrevasionIframes;

                Rectangle playerRect = PlayerRenderTarget.
                    getPlayerTargetSourceRectangle(player.whoAmI);
                Rectangle sourceRectangle = new Rectangle(player.whoAmI * playerRect.Width, 0,
                    playerRect.Width, playerRect.Height);

                // GameShaders.Misc["Prevasion"].UseColor(adept.septima.MainColor).UseOpacity(1).Apply();
                Vector2 position = player.position - Main.screenPosition - playerRect.Size() / 2;
                Main.spriteBatch.Draw(
                    PlayerRenderTarget.Target, 
                    position + new Vector2(displacement, 0), 
                    sourceRectangle, 
                    adept.septima.MainColor * 0.5f);

                Main.spriteBatch.Draw(
                    PlayerRenderTarget.Target,
                    position + new Vector2(-displacement, 0),
                    sourceRectangle,
                    adept.septima.MainColor * 0.5f);
            }
        }
    }
}
