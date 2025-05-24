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
    public class SkillSelect : UIState
    {
        // Assets
        Asset<Texture2D> DefaultFrame;
        Asset<Texture2D> ActiveFrame;
        Asset<Texture2D> CooldownFrame;
        Asset<Texture2D> DefaultSkill;

        // General selection elements
        UIElement area;
        UIImage SelectionAreaBackground;
        UIImageButton LeftArrowButton;
        UIImageButton RightArrowButton;

        // Selected skill elements
        UIImageButton SelectedSkillButton;
        UIImage SelectedSkillFrame;
        UIText SelectedSkillName;
        UIText SelectedSkillCost;

        // Adjacent skills elements
        UIImage[] AdjacentSkillButtons = new UIImage[2];
        UIImage[] AdjacentSkillFrames = new UIImage[2];
        UIText[] AdjacentSkillNames = new UIText[2];
        UIText[] AdjacentSkillCosts = new UIText[2];

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

            SelectionAreaBackground = new UIImage(
                ModContent.Request<Texture2D>("GvMod/Common/UI/SkillSelectionBackground"));
            SelectionAreaBackground.Width.Set(160, 0f);
            SelectionAreaBackground.Height.Set(64, 0f);
            SelectionAreaBackground.Top.Set(0, 0f);
            SelectionAreaBackground.Left.Set(0, 0f);

            LeftArrowButton = new UIImageButton(
                ModContent.Request<Texture2D>("GvMod/Common/UI/LeftSkillSelectionArrow"));
            LeftArrowButton.Width.Set(14, 0f);
            LeftArrowButton.Height.Set(20, 0f);
            LeftArrowButton.Top.Set(0, 0f);
            LeftArrowButton.Left.Set(10, 0f);
            LeftArrowButton.VAlign = 0.5f;
            LeftArrowButton.OnLeftClick += (mouseEvent, uiElement) =>
            {
                Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().ChangeSkill(-1);
            };

            RightArrowButton = new UIImageButton(
                ModContent.Request<Texture2D>("GvMod/Common/UI/RightSkillSelectionArrow"));
            RightArrowButton.Width.Set(14, 0f);
            RightArrowButton.Height.Set(20, 0f);
            RightArrowButton.Top.Set(0, 0f);
            RightArrowButton.Left.Set(-24, 1f);
            RightArrowButton.VAlign = 0.5f;
            RightArrowButton.OnLeftClick += (mouseEvent, uiElement) =>
            {
                Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().ChangeSkill(1);
            };

            SelectedSkillButton = new UIImageButton(DefaultSkill);
            SelectedSkillButton.Width.Set(44, 0f);
            SelectedSkillButton.Height.Set(26, 0f);
            SelectedSkillButton.Left.Set(-22, 0.5f);
            SelectedSkillButton.Top.Set(-13, 0.5f);
            SelectedSkillButton.OnLeftDoubleClick += (mouseEvent, uiElement) =>
            {
                ModContent.GetInstance<UISystem>().SwitchSkillScreenState(false);
            };

            SelectedSkillFrame = new UIImage(DefaultFrame);
            SelectedSkillFrame.Width.Set(48, 0f);
            SelectedSkillFrame.Height.Set(30, 0f);
            SelectedSkillFrame.Left.Set(-24, 0.5f);
            SelectedSkillFrame.Top.Set(-15, 0.5f);

            SelectedSkillName = new UIText("None", 0.75f);
            SelectedSkillName.Width.Set(160, 0f);
            SelectedSkillName.Height.Set(12, 0f);
            SelectedSkillName.Left.Set(0, 0f);
            SelectedSkillName.Top.Set(-28, 0.5f);
            SelectedSkillName.HAlign = 0.5f;

            SelectedSkillCost = new UIText("0 AP", 0.75f);
            SelectedSkillCost.Width.Set(160, 0f);
            SelectedSkillCost.Height.Set(12, 0f);
            SelectedSkillCost.Left.Set(0, 0f);
            SelectedSkillCost.Top.Set(18, 0.5f);
            SelectedSkillCost.HAlign = 0.5f;

            area.Append(SelectionAreaBackground);

            for (int i = 0; i < 2; i++)
            {
                AdjacentSkillButtons[i] = new UIImage(DefaultSkill);
                AdjacentSkillButtons[i].Width.Set(22, 0f);
                AdjacentSkillButtons[i].Height.Set(13, 0f);
                AdjacentSkillButtons[i].Left.Set(22 - (86 * i), i);
                AdjacentSkillButtons[i].Top.Set(-12, 0.5f);
                AdjacentSkillButtons[i].ScaleToFit = true;
                AdjacentSkillButtons[i].ImageScale = 0.5f;
                AdjacentSkillButtons[i].Color = new Color(170, 170, 170);

                AdjacentSkillFrames[i] = new UIImage(DefaultFrame);
                AdjacentSkillFrames[i].Width.Set(24, 0f);
                AdjacentSkillFrames[i].Height.Set(15, 0f);
                AdjacentSkillFrames[i].Left.Set(20 - (86 * i), i);
                AdjacentSkillFrames[i].Top.Set(-14, 0.5f);
                AdjacentSkillFrames[i].ScaleToFit = true;
                AdjacentSkillFrames[i].ImageScale = 0.5f;
                AdjacentSkillFrames[i].Color = new Color(170, 170, 170);

                AdjacentSkillNames[i] = new UIText("None", 0.5f);
                AdjacentSkillNames[i].Width.Set(72, 0f);
                AdjacentSkillNames[i].Height.Set(12, 0f);
                AdjacentSkillNames[i].Left.Set(0 - (86 * i), i);
                AdjacentSkillNames[i].Top.Set(-26, 0.5f);
                AdjacentSkillNames[i].TextColor = new Color(170, 170, 170);

                AdjacentSkillCosts[i] = new UIText("", 0.5f);
                AdjacentSkillCosts[i].Width.Set(72, 0f);
                AdjacentSkillCosts[i].Height.Set(12, 0f);
                AdjacentSkillCosts[i].Left.Set(0 - (86 * i), i);
                AdjacentSkillCosts[i].Top.Set(18, 0.5f);
                AdjacentSkillCosts[i].TextColor = new Color(170, 170, 170);

                area.Append(AdjacentSkillFrames[i]);
                area.Append(AdjacentSkillButtons[i]);
                area.Append(AdjacentSkillNames[i]);
                area.Append(AdjacentSkillCosts[i]);
            }

            area.Append(LeftArrowButton);
            area.Append(RightArrowButton);

            area.Append(SelectedSkillFrame);
            area.Append(SelectedSkillButton);
            area.Append(SelectedSkillName);
            area.Append(SelectedSkillCost);
            Append(area);
        }

        public override void Update(GameTime gameTime)
        {



            for (int i = 0; i < 2; i++)
            {



            }

            Recalculate();

            if (Main.LocalPlayer.GetModPlayer<SeptimaPlayer>().septimaType == Players.Sevenths.SeptimaType.None)
            {
                return;
            }

            SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();

            // Previous and next skills go first so they are added before and appear behind the selected one
            int index = 0;
            for (int i = -1; i <= 1; i += 2)
            {
                UIImage evaluatedFrame = AdjacentSkillFrames[index];
                UIImage evaluatedIcon = AdjacentSkillButtons[index];
                UIText evaluatedName = AdjacentSkillNames[index];
                UIText evaluatedCost = AdjacentSkillCosts[index];

                int evaluatedSkillIndex = adept.SelectedSkill + i;
                if (evaluatedSkillIndex < 0)
                {
                    evaluatedSkillIndex = adept.septima.AvailableSkills.Count - 1;
                }
                if (evaluatedSkillIndex >= adept.septima.AvailableSkills.Count)
                {
                    evaluatedSkillIndex = 0;
                }

                if (adept.septima.AvailableSkills.Count <= 0 || adept.septima.AvailableSkills == null ||
                    adept.septima.AvailableSkills[evaluatedSkillIndex].InternalName == "Default")
                {
                    evaluatedIcon.SetImage(DefaultSkill);
                    evaluatedFrame.SetImage(DefaultFrame);

                    evaluatedName.SetText("None"); // Change to use localizations
                    evaluatedCost.SetText(""); // Localization unecessary 

                    base.Update(gameTime);
                    Recalculate();
                    index++;
                    continue;
                }

                SpecialSkill evaluatedSkill = adept.septima.AvailableSkills[evaluatedSkillIndex];

                evaluatedName.SetText(evaluatedSkill.InternalName); // Change to use localizations
                evaluatedCost.SetText($"{evaluatedSkill.APCost} AP"); // Localization unecessary 

                if (evaluatedSkill.CooldownTime > 0)
                {
                    float apPercent = evaluatedSkill.CooldownTime / (evaluatedSkill.MaxCooldownTime + 0.00001f);

                    evaluatedFrame.SetImage(CooldownFrame);
                }
                else
                {
                    evaluatedFrame.SetImage(ActiveFrame);
                }

                // Consider using lazy loading?
                evaluatedIcon.SetImage(
                    ModContent.Request<Texture2D>($"GvMod/Assets/Skills/{evaluatedSkill.InternalName}"));

                index++;
            }

            // Fail cases
            if (adept.septima.AvailableSkills.Count <= 0 || adept.septima.AvailableSkills == null ||
                adept.septima.AvailableSkills[adept.SelectedSkill].InternalName == "Default")
            {
                SelectedSkillButton.SetImage(DefaultSkill);
                SelectedSkillFrame.SetImage(DefaultFrame);

                SelectedSkillName.SetText("None"); // Change to use localizations
                SelectedSkillCost.SetText(""); // Localization unecessary 

                base.Update(gameTime);
                Recalculate();
                return;
            }

            SpecialSkill skill = adept.septima.AvailableSkills[adept.SelectedSkill];

            SelectedSkillName.SetText(skill.InternalName); // Change to use localizations
            SelectedSkillCost.SetText($"{skill.APCost} AP"); // Localization unecessary 

            if (skill.CooldownTime > 0)
            {
                float apPercent = skill.CooldownTime / (skill.MaxCooldownTime + 0.00001f);

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

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
            if (SelectedSkillButton.ContainsPoint(Main.MouseScreen) || 
                LeftArrowButton.ContainsPoint(Main.MouseScreen) || RightArrowButton.ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}
