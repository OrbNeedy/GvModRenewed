using System.Collections.Generic;
using GvMod.Common.Players;
using GvMod.Common.Players.Skills;
using GvMod.Common.Systems;
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
        UIImage SelectedSkillIcon;
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
            area.Width.Set(160, 0f);
            area.Height.Set(64, 0f);
            area.Left.Set(20, 0f);
            area.Top.Set(-80, 0.5f);


            SelectedSkillIcon = new UIImage(DefaultSkill);
            SelectedSkillIcon.Width.Set(44, 0f);
            SelectedSkillIcon.Height.Set(26, 0f);
            SelectedSkillIcon.Left.Set(-22, 0.5f);
            SelectedSkillIcon.Top.Set(-13, 0.5f); ;

            SelectedSkillButton = new UIImageButton(DefaultSkill);
            SelectedSkillButton.Width.Set(44, 0f);
            SelectedSkillButton.Height.Set(26, 0f);
            SelectedSkillButton.Left.Set(-22, 0.5f);
            SelectedSkillButton.Top.Set(-13, 0.5f);
            SelectedSkillButton.OnLeftClick += (mouseEvent, uiElement) =>
            {
                ModContent.GetInstance<UISystem>().SwitchSkillScreenState(true);
            };

            SelectedSkillFrame = new UIImage(DefaultFrame);
            SelectedSkillFrame.Width.Set(48, 0f);
            SelectedSkillFrame.Height.Set(30, 0f);
            SelectedSkillFrame.Left.Set(-24, 0.5f);
            SelectedSkillFrame.Top.Set(-15, 0.5f);

            SkillOverheatSeal = new UIImage(ModContent.Request<Texture2D>("GvMod/Common/UI/SkillCooldownSeal"));
            SkillOverheatSeal.Width.Set(72, 0f);
            SkillOverheatSeal.Height.Set(24, 0f);
            SkillOverheatSeal.Left.Set(40, 0f);
            SkillOverheatSeal.Top.Set(20, 0f);

            Asset<Texture2D> meterAsset = ModContent.Request<Texture2D>("GvMod/Common/UI/CooldownSealMeter");

            SkillOverheatMeter = new UIImageFramed(meterAsset, meterAsset.Value.Bounds);
            SkillOverheatMeter.Width.Set(54, 0f);
            SkillOverheatMeter.Height.Set(6, 0f);
            SkillOverheatMeter.Left.Set(48, 0f);
            SkillOverheatMeter.Top.Set(36, 0f);

            SkillName = new UIText("None");
            SkillName.Width.Set(72, 0f);
            SkillName.Height.Set(12, 0f);
            SkillName.Left.Set(0, 0f);
            SkillName.Top.Set(-2, 0f);
            SkillName.HAlign = 0.5f;

            SkillCost = new UIText("0 AP");
            SkillCost.Width.Set(72, 0f);
            SkillCost.Height.Set(12, 0f);
            SkillCost.Left.Set(0, 0f);
            SkillCost.Top.Set(48, 0f);
            SkillCost.HAlign = 0.5f;

            area.Append(SelectedSkillFrame);
            area.Append(SelectedSkillIcon);
            area.Append(SkillName);
            area.Append(SkillCost);
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
                SkillOverheatSeal.Remove();
                SkillOverheatMeter.Remove();
                SelectedSkillIcon.SetImage(DefaultSkill);
                SelectedSkillFrame.SetImage(DefaultFrame);

                SkillName.SetText("None"); // Change to use localizations
                SkillCost.SetText(""); // Localization unecessary 

                base.Update(gameTime);
                Recalculate();
                return;
            }

            SpecialSkill skill = adept.septima.AvailableSkills[adept.SelectedSkill];

            SkillName.SetText(skill.InternalName); // Change to use localizations
            SkillCost.SetText($"{skill.APCost} AP"); // Localization unecessary 

            // Add the cooldown seal and it's meter if the skill is on cooldown
            if (skill.CooldownTime > 0)
            {
                float apPercent = skill.CooldownTime / (skill.MaxCooldownTime + 0.00001f);

                SelectedSkillFrame.SetImage(CooldownFrame);
                SkillOverheatMeter.SetFrame(new Rectangle(0, 0, (int)(54 * apPercent), 6));
                area.Append(SkillOverheatSeal);
                area.Append(SkillOverheatMeter);
                SelectedSkillIcon.Remove();

                base.Update(gameTime);
                Recalculate();
                return;
            } else
            {
                area.Append(SelectedSkillIcon);
                SkillOverheatSeal.Remove();
                SkillOverheatMeter.Remove();
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
            SelectedSkillIcon.SetImage(
                ModContent.Request<Texture2D>($"GvMod/Assets/Skills/{skill.InternalName}"));
            
            area.Append(SelectedSkillButton);

            base.Update(gameTime);
            Recalculate();
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
            if (SelectedSkillButton.ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}
