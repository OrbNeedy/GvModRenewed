using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Common.Players.Skills;
using GvMod.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace GvMod.Common.UI
{
    public class SkillNotice : UIState
    {
        UIElement area;
        UIImage SkillIcon;
        UIText Text;
        int notificationTimer = 0;

        public override void OnInitialize()
        {
            if (Main.dedServ) return;

            area = new UIElement();
            area.Width.Set(18, 0f);
            area.Height.Set(32, 0f);
            area.Left.Set(20, 0f);
            area.Top.Set(-46, 1f);

            SkillIcon = new UIImage(ModContent.Request<Texture2D>("GvMod/Assets/Skills/Default"));
            SkillIcon.Width.Set(44, 0f);
            SkillIcon.Height.Set(26, 0f); 
            SkillIcon.Left.Set(-44, 0.5f);
            SkillIcon.Top.Set(-50, 0.5f);
            SkillIcon.HAlign = 0.5f;

            Text = new UIText("Skill Unlocked!");
            Text.Width.Set(44, 0f);
            Text.Height.Set(26, 0f);
            Text.Left.Set(-22, 0.5f);
            Text.Top.Set(-13, 0.5f);
            Text.HAlign = 0.5f;

            area.Append(SkillIcon);
            area.Append(Text);
            Append(area);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().septimaType == SeptimaType.None || Main.dedServ)
            {
                return;
            }

            SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();

            if (adept.QueuedSkills.Count > 0 && notificationTimer <= 0)
            {
                SkillIcon.SetImage(ModContent.Request<Texture2D>($"GvMod/Assets/Skills/{adept.QueuedSkills[0]}"));
                adept.QueuedSkills.RemoveAt(0);
                if (adept.QueuedSkills.Count > 0)
                {
                    notificationTimer = 120;
                } else
                {
                    notificationTimer = 240;
                }

                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/SkillUnlock") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.75f
                });
            }


            if (notificationTimer > 0)
            {
                if (area.Left.Pixels < 80)
                {
                    area.Left.Set(area.Left.Pixels + 14, 0f);
                }
                notificationTimer--;
            } else
            {
                if (area.Left.Pixels > -120)
                {
                    area.Left.Set(area.Left.Pixels - 14, 0f);
                }
            }

            Recalculate();
        }
    }
}
