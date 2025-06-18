using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace GvMod.Common.UI
{
    public class EPBar : UIState
    {
        private UIElement area;
        private UIText percentText;
        private UIText levelText;
        private UIImageFramed barFill;
        private UIImageFramed barEffect;
        private UIImage barFrame;

        public override void OnInitialize()
        {
            if (Main.dedServ) return;

            area = new UIElement();
            area.Width.Set(124, 0f);
            area.Height.Set(22, 0f);
            area.Left.Set(-62, 0.5f); // -43
            area.Top.Set(38, 0f); // 25

            Asset<Texture2D> fillTexture = ModContent.Request<Texture2D>("GvMod/Common/UI/EPBarFill");
            Asset<Texture2D> effectTexture = ModContent.Request<Texture2D>("GvMod/Common/UI/EPBarFillOver");

            barFill = new UIImageFramed(fillTexture, new Rectangle(0, 0, 82, 14));
            barFill.Left.Set(6, 0f);
            barFill.Top.Set(2, 0f);
            barFill.Width.Set(112, 0f);
            barFill.Height.Set(18, 0f);

            barEffect = new UIImageFramed(effectTexture, new Rectangle(0, 0, 82, 14));
            barEffect.Left.Set(6, 0f);
            barEffect.Top.Set(2, 0f);
            barEffect.Width.Set(112, 0f);
            barEffect.Height.Set(18, 0f);

            barFrame = new UIImage(ModContent.Request<Texture2D>("GvMod/Common/UI/EPBarFrame"));
            barFrame.Left.Set(0, 0f);
            barFrame.Top.Set(0, 0f);
            barFrame.Width.Set(124, 0f);
            barFrame.Height.Set(22, 0f);

            percentText = new UIText("100%");
            percentText.Left.Set(0, 0f);
            percentText.Top.Set(6, 0f);
            percentText.Width.Set(16, 0f);
            percentText.Height.Set(28, 0f);
            percentText.HAlign = 0.5f;

            levelText = new UIText("Lvl 1");
            levelText.Left.Set(0, 0f);
            levelText.Top.Set(28, 0f);
            levelText.Width.Set(16, 0f);
            levelText.Height.Set(28, 0f);

            area.Append(barFrame);
            area.Append(barFill);
            area.Append(barEffect);
            area.Append(percentText);
            area.Append(levelText);
            Append(area);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().septimaType == SeptimaType.None || Main.dedServ)
            {
                return;
            }

            SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();

            float percent = adept.GetEPPercent();

            barFill.SetFrame(new Rectangle(0, 0, (int)(112 * percent), 18));
            if (adept.Overheated)
            {
                barFill.Color = adept.septima.OverheatColor;
            } else
            {
                barFill.Color = adept.septima.MainColor;
            }
            barEffect.SetFrame(new Rectangle(0, 0, (int)(112 * percent), 18));

            percentText.SetText($"{(int)(percent * 100)}%");
            levelText.SetText($"Lvl {adept.Level}\nStage {adept.Stage}");
            if (area.IsMouseHovering)
            {
                area.Append(percentText);
            } else
            {
                percentText.Remove();
            }

            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Don't draw anything when the player has no septima
            if (Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().septimaType == SeptimaType.None || Main.dedServ)
            {
                return;
            }

            base.Draw(spriteBatch);
        }
    }
}
