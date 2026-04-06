using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Common.Players.Skills;
using GvMod.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace GvMod.Common.UI
{
    class SeptimaSelectionUI : UIState
    {
        private readonly Player _player;
        private readonly MouseEvent _return;

        private const int TotalWidth = 800;
        private const int BackgroundListWidth = 220;

        private static float BaseHeight => 554f / 1017 * Main.screenHeight;

        private UIText _statsText;
        private UIText _descText;
        private UIElement _skillsContainer;

        public SeptimaSelectionUI(Player player, MouseEvent returnAction)
        {
            _player = player;
            _return = returnAction;

            RemoveAllChildren();

            var mainElement = new UIElement
            {
                Width = StyleDimension.FromPixels(TotalWidth),
                Height = StyleDimension.FromPixels(BaseHeight),
                Top = StyleDimension.FromPixels(220f),
                HAlign = 0.5f,
                VAlign = 0f
            };
            mainElement.SetPadding(0f);
            Append(mainElement);

            Color panelColor = new Color(33, 43, 79) * 0.8f;

            var panel = new UIPanel
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                Top = StyleDimension.FromPixels(50f),
                BackgroundColor = panelColor
            };

            mainElement.Append(panel);

            UIImageButton closeButton = new(ModContent.Request<Texture2D>("GVMod/Assets/UI/Back"))
            {
                Width = StyleDimension.FromPixels(32),
                Height = StyleDimension.FromPixels(32),
                Left = StyleDimension.FromPixelsAndPercent(-40, 1f),
                Top = StyleDimension.FromPixelsAndPercent(0, 0)
            };

            closeButton.OnLeftClick += _return;
            panel.Append(closeButton);

            var descriptionPanel = new UIPanel
            {
                Width = StyleDimension.FromPixelsAndPercent(-BackgroundListWidth - 10, 1f),
                HAlign = 1f,
                Height = StyleDimension.FromPixelsAndPercent(-40, 1f),
                Top = StyleDimension.FromPixels(30),
                BackgroundColor = panelColor
            };
            panel.Append(descriptionPanel);

            BuildDescription(descriptionPanel);

            var originsListPanel = new UIPanel
            {
                Width = StyleDimension.FromPixels(BackgroundListWidth),
                HAlign = 0,
                Height = StyleDimension.FromPixelsAndPercent(-40, 1f),
                Top = StyleDimension.FromPixels(30),
                BackgroundColor = panelColor
            };
            panel.Append(originsListPanel);

            BuildBackgroundSelections(originsListPanel);
        }

        private void SetSkillList(SeptimaType type, bool resetContainer = false)
        {
            Septima data = SeptimaTemplates.GetSeptimaTemplate(type);
            var descItemHeight = StyleDimension.FromPixels(data.SkillList.Count == 0 ? 0 : 54 + 32 * (data.SkillList.Count / 12f));

            if (resetContainer)
            {
                _skillsContainer = new UIElement()
                {
                    Width = StyleDimension.FromPercent(1),
                    Height = descItemHeight,
                    Top = StyleDimension.FromPixels(-20),
                    HAlign = 0.5f,
                };
            } else
            {
                _skillsContainer.Height = descItemHeight;
                _skillsContainer.HAlign = 0.5f;
                _skillsContainer.Recalculate();
            }

            int offset = 0;
            float yOffset = 0;
            int id = 0;

            Asset<Texture2D> backTexture = ModContent.Request<Texture2D>($"GvMod/Common/UI/SkillFrame");
            foreach (SpecialSkill skill in data.SkillList)
            {
                if (skill.InternalName == "Default") continue;

                Asset<Texture2D> texture = ModContent.Request<Texture2D>($"GvMod/Assets/Skills/{skill.InternalName}");
                string skillName = Language.GetText(skill.LocalizationKey).Value;

                UIImage skillBackground = new(backTexture) {
                    Left = StyleDimension.FromPixels(-6 + offset * 42),
                    Top = StyleDimension.FromPixels(-2 + yOffset * 38),
                    ImageScale = 0.77f,
                };

                /*ModContent.GetInstance<GvMod>().Logger.Debug($"Skill: {skill.LocalizationKey}\nLeft: {skillBackground.Left.Pixels}" +
                    $"\nTop: {skillBackground.Top.Pixels}\nOffset: {offset}\nyOffset: {yOffset}");*/

                UIImage skillIcon = new(texture)
                {
                    Left = StyleDimension.FromPixels(2f),
                    Top = StyleDimension.FromPixels(2f),
                    ImageScale = 0.77f,
                }; 
                
                skillBackground.Append(skillIcon);

                float width = FontAssets.ItemStack.Value.MeasureString(skillName).X * 0.8f;

                UIText name = new(skillName, 0.8f)
                {
                    Width = StyleDimension.FromPixels(42),
                    Top = StyleDimension.FromPixels(-10),
                    HAlign = 0.5f,
                    DynamicallyScaleDownToWidth = true,
                };

                if (id == 0 || id % 5 == 1)
                {
                    name.Top = StyleDimension.FromPixels(-10);
                    name.HAlign = 0f;
                    name.Left = StyleDimension.FromPixels(6);
                }
                else if (id % 5 == 0)
                {
                    name.Top = StyleDimension.FromPixels(-10);
                    name.HAlign = 1f;
                }

                name.DynamicallyScaleDownToWidth = true;

                skillIcon.OnMouseOver += (UIMouseEvent evt, UIElement listeningElement) => skillBackground.Append(name);
                skillIcon.OnMouseOut += (UIMouseEvent evt, UIElement listeningElement) => skillBackground.RemoveChild(name);

                _skillsContainer.Append(skillBackground);

                offset++;
                //ModContent.GetInstance<GvMod>().Logger.Debug($"\nNew offset: {10 + (offset * 32)}");
                //ModContent.GetInstance<GvMod>().Logger.Debug($"\nCompare to: {TotalWidth - BackgroundListWidth + 20}");
                if (10 + (offset * 56) > TotalWidth - BackgroundListWidth + 20) //ew hardcoding but nothing works
                {
                    //ModContent.GetInstance<GvMod>().Logger.Debug("yOffset change");
                    offset = 0;
                    yOffset++;
                }

                id++;
            }
        }

        private void BuildDescription(UIPanel panel)
        {
            var list = new UIList() //List for use in the description
            {
                Width = StyleDimension.FromPercent(0.98f),
                Height = StyleDimension.FromPixelsAndPercent(0, 1f),
                PaddingLeft = 8,
                PaddingRight = 8,
                ListPadding = -20
            };
            panel.Append(list);

            var scrollBar = new UIScrollbar() //Scrollbar for above list
            {
                HAlign = 1f,
                Height = StyleDimension.FromPixelsAndPercent(-8, 1f),
                Top = StyleDimension.FromPixels(4),
            };

            list.SetScrollbar(scrollBar);
            panel.Append(scrollBar);

            var bgData = _player.GetModPlayer<SeptimaPlayer>().septima;
            _statsText = new UIText(GetStatsText(bgData))
            {
                Top = StyleDimension.FromPercent(0.1f),
                Width = StyleDimension.FromPixelsAndPercent(-8, 1f),
                Height = StyleDimension.FromPixels(44),
                IsWrapped = false,
                MarginTop = 8
            };
            list.Add(_statsText);

            _skillsContainer = new();
            SetSkillList(bgData.Type, true);
            list.Add(_skillsContainer);

            _descText = new UIText(Language.GetText("Mods.GvMod.UI.SeptimaDescription." + bgData.Type.ToString()))
            {
                Top = StyleDimension.FromPercent(0.1f),
                Width = StyleDimension.FromPixelsAndPercent(-8, 1f),
                Height = StyleDimension.FromPixels(10),
                IsWrapped = true,
                MarginTop = 8
            };
            list.Add(_descText);
        }
    
        private static string GetStatsText(Septima bgData)
        {
            int itemID = ModContent.ItemType<AffirmationItem>();
            if (!bgData.AllowPrevasion) itemID = ModContent.ItemType<NegationItem>();

            string stats = $"Prevasion: [i:{itemID}] EP: {SeptimaPlayer.InitialMaxEP + bgData.MaxEPModifier}";

            return stats;
        }

        /// <summary>Builds the origin list and buttons.</summary>
        private void BuildBackgroundSelections(UIPanel container)
        {
            UIList allBGButtons = new() //List of all background buttons
            {
                Width = StyleDimension.FromPercent(1),
                Height = StyleDimension.FromPixelsAndPercent(0, 1f),
                ListPadding = 4,
            };

            container.Append(allBGButtons);

            UIScrollbar scroll = new() //Scrollbar for above list
            {
                HAlign = 1f,
                Height = StyleDimension.FromPixelsAndPercent(-8, 1f),
                Top = StyleDimension.FromPixels(4)
            };

            allBGButtons.SetScrollbar(scroll);
            container.Append(scroll);

            List<(SeptimaType data, UIColoredImageButton button)> buttons = [];

            foreach (var item in SeptimaTemplates._selectableSeptimas) //Adds every background into the list as a button
            {
                // Copied from NewBeginnings, unused
                // But what if octimas were unlockable by beating the game with specific ones? Such as Reverie Mirror and Azure Striker
                /*if (!item.Delegates.ClearCondition())
                    continue;*/

                var asset = SeptimaTemplates.SeptimaIcons[item];
                UIColoredImageButton currentBGButton = new(asset)
                {
                    Width = StyleDimension.FromPercent(0.9f),
                    Height = StyleDimension.FromPixels(36),
                    Left = StyleDimension.FromPixels(-64),
                    Top = StyleDimension.FromPixels(8),
                    MarginRight = 4f,
                    MarginTop = 4f,
                };
                currentBGButton.SetColor(Color.Gray);

                currentBGButton.OnLeftMouseDown += (UIMouseEvent evt, UIElement listeningElement) => //Click event
                    BackgroundButtonClick(allBGButtons, item, currentBGButton);

                currentBGButton.OnMouseOver += (UIMouseEvent evt, UIElement listeningElement) =>
                {
                    currentBGButton.SetColor(new Color(220, 220, 220));
                };

                currentBGButton.OnMouseOut += (UIMouseEvent evt, UIElement listeningElement) =>
                {
                    var bgData = _player.GetModPlayer<SeptimaPlayer>().septima;

                    if (_player.GetModPlayer<SeptimaPlayer>().septimaType == SeptimaType.None ||
                        _player.GetModPlayer<SeptimaPlayer>().septima == null)
                    {
                        _player.GetModPlayer<SeptimaPlayer>().SetSeptima(bgData); //Sets the player's background.

                        currentBGButton.SetColor(Color.Gray);
                    }
                    else
                        currentBGButton.SetColor(Color.White);
                };

                float textSize = 1.2f;

                UIText bgName = new(Language.GetText($"Mods.GvMod.UI.SeptimaNames.{item.ToString()}"), textSize) // Background's name
                {
                    HAlign = 0f,
                    VAlign = 0.5f,
                    Left = StyleDimension.FromPixels(114)
                };

                currentBGButton.Append(bgName);
                buttons.Add((item, currentBGButton));
            }

            foreach (var (_, button) in buttons)
                allBGButtons.Add(button);

            /*if (UnlockSaveData.Unlocked("Renewed"))
                AddCustomBGButton(allBGButtons, buttons);*/

            //SetSort(allBGButtons, buttons, _sortButton);
        }

        // Maybe custom septimas in the future
        /*private void AddCustomBGButton(UIList allBGButtons, List<(SeptimaType, UIColoredImageButton button)> buttons)
        {
            var asset = PlayerBackgroundDatabase.backgroundIcons["Custom"];
            UIColoredImageButton customBGButton = new(asset)
            {
                Width = StyleDimension.FromPercent(0.9f),
                Height = StyleDimension.FromPixels(36),
                Left = StyleDimension.FromPixels(-64),
                Top = StyleDimension.FromPixels(8),
                MarginRight = 4f,
                MarginTop = 4f,
            };
            customBGButton.SetColor(Color.Gray);
            customBGButton.OnLeftClick += CurrentBGButton_OnClick;
            allBGButtons.Add(customBGButton);

            UIText bgName = new(Language.GetTextValue("Mods.GvMod.Origins.Custom.DisplayName"), 1.2f) //Background's name
            {
                HAlign = 0f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(114)
            };
            customBGButton.Append(bgName);

            // Dummy data for the random since it doesn't strictly have a useful data; solely sets stuff so that everything is optimal for sorting (and doesn't crash).
            PlayerBackgroundData data = new()
            {
                Misc = new(sortPriority: -500, stars: 500),
                Name = Language.GetText("Mods.NewBeginnings.Origins.Random.DisplayName")
            };
            buttons.Add((data, customBGButton));
        }*/

        /*private void CurrentBGButton_OnClick(UIMouseEvent evt, UIElement listeningElement) =>
            Main.MenuUI.SetState(new UICustomOrigin(_player, (evt, listeningElement) => Main.MenuUI.SetState(this)));*/

        private Septima BackgroundButtonClick(UIList allBGButtons, SeptimaType item, UIColoredImageButton currentBGButton)
        {
            Septima useData = SeptimaTemplates.GetNewSeptima(item); //Hardcoding for random, sucks but eh
            _descText.SetText(Language.GetText("Mods.GvMod.UI.SeptimaDescription." + item.ToString())); //Changes the UIText's value to use the bg's description

            _statsText.SetText(GetStatsText(useData));

            _skillsContainer.RemoveAllChildren();
            SetSkillList(item);

            _player.GetModPlayer<SeptimaPlayer>().SetSeptima(useData); //...and sets it.

            foreach (var button in allBGButtons.Where(x => x is UIColoredImageButton))
                (button as UIColoredImageButton).SetColor(Color.Gray);

            currentBGButton.SetColor(Color.White); //"Selects" the button visually.
            return useData; // No idea why this has a return value
        }
    }
}
