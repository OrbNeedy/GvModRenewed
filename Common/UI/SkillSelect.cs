using System.Collections.Generic;
using GvMod.Common.Players;
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
        bool selectingSkill = false;

        UIElement area;
        UIImageButton SelectedSkillButton;
        UIImage SelectedSkillFrame;
        UIText SkillName;
        UIText SkillCost;
        UIImage SkillOverheatSeal;
        UIImageFramed SkillOverheatMeter;
        List<UIImageButton> SkillButtons = new List<UIImageButton>();

        public override void OnInitialize()
        {
            if (Main.dedServ) return;

            area = new UIElement();
            area.Width.Set(72, 0f);
            area.Height.Set(30, 0f);
            area.Left.Set(20, 0f);
            area.Top.Set(-60, 1f);

            SelectedSkillButton = new UIImageButton(ModContent.Request<Texture2D>("GvMod/Assets/Skills/Default"));
            SelectedSkillButton.Width.Set(44, 0f);
            SelectedSkillButton.Height.Set(26, 0f);
            SelectedSkillButton.Left.Set(16, 0f);
            SelectedSkillButton.Top.Set(2, 0f);

            SelectedSkillFrame = new UIImage(ModContent.Request<Texture2D>("GvMod/Common/UI/SkillFrameEmpty"));
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

            for (int i = 0; i < adept.septima.AvailableSkills.Count; i++)
            {

            }

            base.Update(gameTime);
            Recalculate();
        }
    }
}
