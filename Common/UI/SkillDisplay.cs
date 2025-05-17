using System.Collections.Generic;
using GvMod.Common.Players;
using GvMod.Common.Players.Skills;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace GvMod.Common.UI
{
    public class SkillDisplay : UIState
    {
        Asset<Texture2D> DefaultFrame;
        Asset<Texture2D> ActiveFrame;
        Asset<Texture2D> CooldownFrame;
        Asset<Texture2D> DefaultSkill;

        UIElement area;
        UIImageButton SelectedSkillButton;
        UIImage SelectedSkillFrame;
        UIText SkillName;
        UIText SkillCost;
        UIImage SkillOverheatSeal;
        UIImageFramed SkillOverheatMeter;

        public override void OnInitialize()
        {
            if (Main.dedServ) return;

            DefaultFrame = ModContent.Request<Texture2D>("GvMod/Common/UI/SkillFrameEmpty");
            ActiveFrame = ModContent.Request<Texture2D>("GvMod/Common/UI/SkillFrame");
            CooldownFrame = ModContent.Request<Texture2D>("GvMod/Common/UI/SkillFrameCooldown");
            DefaultSkill = ModContent.Request<Texture2D>("GvMod/Assets/Skills/Default");

            area = new UIElement();
            area.Width.Set(72, 0f);
            area.Height.Set(30, 0f);
            area.Left.Set(20, 0f);
            area.Top.Set(-60, 1f);

            SelectedSkillButton = new UIImageButton(DefaultSkill);
            SelectedSkillButton.Width.Set(44, 0f);
            SelectedSkillButton.Height.Set(26, 0f);
            SelectedSkillButton.Left.Set(16, 0f);
            SelectedSkillButton.Top.Set(2, 0f);

            SelectedSkillFrame = new UIImage(DefaultFrame);
            SelectedSkillFrame.Width.Set(48, 0f);
            SelectedSkillFrame.Height.Set(30, 0f);
            SelectedSkillFrame.Left.Set(14, 0f);
            SelectedSkillFrame.Top.Set(0, 0f);

            SkillOverheatSeal = new UIImage(ModContent.Request<Texture2D>("GvMod/Common/UI/SkillCooldownSeal"));
            SkillOverheatSeal.Width.Set(72, 0f);
            SkillOverheatSeal.Height.Set(24, 0f);
            SkillOverheatSeal.Left.Set(0, 0f);
            SkillOverheatSeal.Top.Set(4, 0f);

            Asset<Texture2D> meterAsset = ModContent.Request<Texture2D>("GvMod/Common/UI/SkillCooldownSeal");

            SkillOverheatMeter = new UIImageFramed(meterAsset, meterAsset.Value.Bounds);
            SkillOverheatMeter.Width.Set(54, 0f);
            SkillOverheatMeter.Height.Set(6, 0f);
            SkillOverheatMeter.Left.Set(8, 0f);
            SkillOverheatMeter.Top.Set(20, 0f);

            SkillName = new UIText("None");
            SkillName.Width.Set(72, 0f);
            SkillName.Height.Set(12, 0f);
            SkillName.Left.Set(0, 0f);
            SkillName.Top.Set(-16, 0f);
            SkillName.HAlign = 0.5f;

            SkillCost = new UIText("0 AP");
            SkillCost.Width.Set(72, 0f);
            SkillCost.Height.Set(12, 0f);
            SkillCost.Left.Set(0, 0f);
            SkillCost.Top.Set(36, 0f);
            SkillCost.HAlign = 0.5f;

            area.Append(SelectedSkillFrame);
            area.Append(SelectedSkillButton);
            Append(area);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().septimaType == Players.Sevenths.SeptimaType.None)
            {
                return;
            }

            SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();

            // Fail cases
            if (adept.septima.AvailableSkills.Count <= 0 || adept.septima.AvailableSkills == null ||
                adept.septima.AvailableSkills[adept.SelectedSkill].InternalName == "Default")
            {
                SelectedSkillButton.SetImage(DefaultSkill);
                SelectedSkillFrame.SetImage(DefaultFrame);

                base.Update(gameTime);
                Recalculate();
                return;
            }

            SpecialSkill skill = adept.septima.AvailableSkills[adept.SelectedSkill];

            // Add the cooldown seal and it's meter if the skill is on cooldown
            if (skill.CooldownTime > 0)
            {
                SelectedSkillFrame.SetImage(CooldownFrame);
                area.Append(SkillOverheatSeal);
                SelectedSkillButton.Remove();

                base.Update(gameTime);
                Recalculate();
                return;
            } else
            {
                area.Append(SelectedSkillButton);
                SkillOverheatSeal.Remove();
            }

            // Change the frame depending on the player's AP and skill cost
            if (skill.APCost > adept.CurrentAP)
            {
                SelectedSkillFrame.SetImage(CooldownFrame);
            } else
            {
                SelectedSkillFrame.SetImage(ActiveFrame);
            }

            // Consider using lazy loading?
            SelectedSkillButton.SetImage(
                ModContent.Request<Texture2D>($"GvMod/Assets/Skills/{skill.InternalName}"));

            base.Update(gameTime);
            Recalculate();
        }
    }
}
