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
    public class APBar : UIState
    {
        private UIElement area;
        // Planned max AP of 4
        private UIImageFramed[] barFilling = new UIImageFramed[4];
        private UIImage[] barFilled = new UIImage[4];
        private UIImage[] barFrame = new UIImage[4];
        private UIImage[] barIcon = new UIImage[4];

        public override void OnInitialize()
        {
            if (Main.dedServ) return;

            area = new UIElement();
            area.Width.Set(18, 0f);
            area.Height.Set(32, 0f);
            area.Left.Set(20, 0f);
            area.Top.Set(-46, 1f);

            Asset<Texture2D> fillingTexture = ModContent.Request<Texture2D>("GvMod/Common/UI/APBarFill");
            Asset<Texture2D> fullTexture = ModContent.Request<Texture2D>("GvMod/Common/UI/APBarFull");

            for (int i = 0; i < barFilling.Length; i++)
            {
                barFilling[i] = new UIImageFramed(fillingTexture, new Rectangle(0, 0, 14, 24));
                barFilling[i].Left.Set(3 + (28*i), 0f);
                barFilling[i].Top.Set(5, 0f);
                barFilling[i].Width.Set(14, 0f);
                barFilling[i].Height.Set(24, 0f);

                barFilled[i] = new UIImage(fullTexture);
                barFilled[i].Left.Set(2 + (30 * i), 0f);
                barFilled[i].Top.Set(2, 0f);
                barFilled[i].Width.Set(14, 0f);
                barFilled[i].Height.Set(28, 0f);

                barFrame[i] = new UIImage(ModContent.Request<Texture2D>("GvMod/Common/UI/APBarFrame"));
                barFrame[i].Left.Set(0 + (30 * i), 0f);
                barFrame[i].Top.Set(0, 0f);
                barFrame[i].Width.Set(18, 0f);
                barFrame[i].Height.Set(32, 0f);

                barIcon[i] = new UIImage(ModContent.Request<Texture2D>("GvMod/Assets/UI/AzureStrikerAPBarIcon"));
                barIcon[i].Left.Set(0 + (30 * i), 0f);
                barIcon[i].Top.Set(0, 0f);
                barIcon[i].Width.Set(18, 0f);
                barIcon[i].Height.Set(32, 0f);
            }
            Append(area);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().septimaType == SeptimaType.None || Main.dedServ)
            {
                return;
            }

            SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();

            int maxAP = adept.GetTotalMaxAP();

            for (int i = 0; i < barFilling.Length; i++)
            {
                if (i >= maxAP)
                {
                    barFrame[i].Remove();
                    barFilling[i].Remove();
                    barFilled[i].Remove();
                    barIcon[i].Remove();
                    continue;
                }

                float barAP = adept.CurrentAP - i;

                if (barAP >= 1)
                {
                    barFilling[i].Remove();
                    area.Append(barFilled[i]);
                    area.Append(barIcon[i]);
                } else
                {
                    barFilling[i].SetFrame(new Rectangle(0, 2 + (int)(20 * barAP) - 20, 14, 20));
                    barFilled[i].Remove();
                    barIcon[i].Remove();
                    area.Append(barFilling[i]);
                }

                area.Append(barFrame[i]);
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
