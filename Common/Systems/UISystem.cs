using System.Collections.Generic;
using GvMod.Common.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;

namespace GvMod.Common.Systems
{
    public class UISystem : ModSystem
    {
        private GameTime _lastUpdatedGameTime;
        private UserInterface EPBarUserInterface;
        private EPBar EPBar;
        private UserInterface APBarUserInterface;
        private APBar APBar;
        private UserInterface SkillUserInterface;
        private SkillDisplay SkillDisplay;
        private SkillSelect SkillSelect;
        private bool lastSkillUI;
        private bool hidingUI = false;

        public override void Load()
        {
            if (Main.dedServ) return;

            EPBarUserInterface = new UserInterface();

            EPBar = new EPBar();
            EPBar.Activate();
            EPBarUserInterface.SetState(EPBar);

            APBarUserInterface = new UserInterface();

            APBar = new APBar();
            APBar.Activate();
            APBarUserInterface.SetState(APBar);

            SkillUserInterface = new UserInterface();

            SkillDisplay = new SkillDisplay();
            SkillDisplay.Activate(); 
            SkillSelect = new SkillSelect();
            SkillSelect.Activate();
            SkillUserInterface.SetState(SkillDisplay);
            lastSkillUI = false;
        }

        public override void Unload()
        {
            EPBar = null;
            APBar = null;
            SkillDisplay = null;
            SkillSelect = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdatedGameTime = gameTime;

            if (EPBarUserInterface?.CurrentState != null)
            {
                EPBarUserInterface.Update(gameTime);
            }

            if (APBarUserInterface?.CurrentState != null)
            {
                APBarUserInterface.Update(gameTime);
            }

            if (SkillUserInterface?.CurrentState != null)
            {
                SkillUserInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Gunvolt Mod: EP Bar",
                    delegate {
                        if (_lastUpdatedGameTime != null && EPBarUserInterface?.CurrentState != null)
                        {
                            EPBarUserInterface.Draw(Main.spriteBatch, _lastUpdatedGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );

                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Gunvolt Mod: AP Bar",
                    delegate {
                        if (_lastUpdatedGameTime != null && APBarUserInterface?.CurrentState != null)
                        {
                            APBarUserInterface.Draw(Main.spriteBatch, _lastUpdatedGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );

                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Gunvolt Mod: Skill UI",
                    delegate {
                        if (_lastUpdatedGameTime != null && SkillUserInterface?.CurrentState != null)
                        {
                            SkillUserInterface.Draw(Main.spriteBatch, _lastUpdatedGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public void SwitchSkillScreenState(bool selecting)
        {
            lastSkillUI = selecting;
            if (selecting)
            {
                SkillUserInterface?.SetState(SkillSelect);
            } else
            {
                SkillUserInterface?.SetState(SkillDisplay);
            }
        }


        public void SwitchUIVisibility()
        {
            if (hidingUI)
            {
                EPBarUserInterface?.SetState(EPBar);
                APBarUserInterface?.SetState(APBar);
                SwitchSkillScreenState(lastSkillUI);
                hidingUI = false;
            } else
            {
                EPBarUserInterface?.SetState(null);
                APBarUserInterface?.SetState(null);
                SkillUserInterface?.SetState(null);
                hidingUI = true;
            }
        }
        public void ShowUI()
        {
            EPBarUserInterface?.SetState(EPBar);
            APBarUserInterface?.SetState(APBar);
            SwitchSkillScreenState(lastSkillUI);
        }

        public void HideUI()
        {
            EPBarUserInterface?.SetState(null);
            APBarUserInterface?.SetState(null);
            SkillUserInterface?.SetState(null);
        }
    }
}
