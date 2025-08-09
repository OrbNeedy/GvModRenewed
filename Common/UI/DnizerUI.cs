using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteelSeries.GameSense;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace GvMod.Common.UI
{
    public class DnizerUI : UIState
    {
        private UIElement area;
        private UIText percentText;

        public override void OnInitialize()
        {
            if (Main.dedServ) return;

            area = new UIElement();
            area.Width.Set(100, 0f);
            area.Height.Set(22, 0f);
            area.Left.Set(-50, 0.5f);
            area.Top.Set(58, 0.5f);

            percentText = new UIText("0%");
            percentText.Left.Set(0, 0f);
            percentText.Top.Set(0, 0f);
            percentText.Width.Set(16, 0f);
            percentText.Height.Set(28, 0f);
            percentText.HAlign = 0.5f;

            area.Append(percentText);
            Append(area);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().septimaType == SeptimaType.None || 
                Main.dedServ)
            {
                return;
            }

            SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
            SetBonusPlayer dnizerStats = Main.LocalPlayer.GetModPlayer<SetBonusPlayer>();

            if (adept.DnizerMode || dnizerStats.DnizerModeTimer > 0)
            {
                area.Append(percentText);
                float percent = (float)dnizerStats.DnizerModeTimer / 
                    (float)SetBonusPlayer.DnizerModeMaxTimer;
                float finalPercent = percent * 100;
                percentText.SetText($"{finalPercent.ToString("n2")}%");
                percentText.TextColor = Color.Lerp(adept.septima.MainColor, adept.septima.OverheatColor, 
                    percent);
            }  else
            {
                percentText.Remove();
            }

            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().septimaType == SeptimaType.None ||
                Main.dedServ)
            {
                return;
            }

            base.Draw(spriteBatch);

            SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
            SetBonusPlayer dnizerStats = Main.LocalPlayer.GetModPlayer<SetBonusPlayer>();

            if (dnizerStats.DnizerModeActivateTimer > 1)
            {
                float percent = (float)dnizerStats.DnizerModeActivateTimer /
                    (float)SetBonusPlayer.DnizerModeActivateMaxTimer;

                Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
                hitbox.Y -= 12;
                hitbox.Width = (int)(100f * percent);
                hitbox.Height = 8;
                /*new Rectangle((int)percentText.Left.Pixels, (int)percentText.Top.Pixels,
                (int)(50 * percent), 10);*/
                // spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                // new Rectangle(left + 2, (int)(hitbox.Y + 28 - (bars[0] * 28)),
                // 12, (int)(bars[0] * 28)), new Color(183, 113, 34));
                spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                    hitbox, adept.septima.MainColor);
            }
        }

        
    }
}
