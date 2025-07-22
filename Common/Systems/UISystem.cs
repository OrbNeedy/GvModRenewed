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

        private UserInterface SPBarUserInterface;
        private SPBar SPBar;

        private UserInterface SkillUserInterface;
        private SkillDisplay SkillDisplay;
        private SkillSelect SkillSelect;

        private UserInterface SkillNotificationsUserInterface;
        private SkillNotice SkillNotifications;

        private bool lastSkillUI;
        private bool hidingUI = false;

        public override void Load()
        {
            if (Main.dedServ) return;

            EPBarUserInterface = new UserInterface();

            EPBar = new EPBar();
            EPBar.Activate();
            EPBarUserInterface.SetState(EPBar);

            SPBarUserInterface = new UserInterface();

            SPBar = new SPBar();
            SPBar.Activate();
            SPBarUserInterface.SetState(SPBar);

            SkillUserInterface = new UserInterface();

            SkillDisplay = new SkillDisplay();
            SkillDisplay.Activate(); 
            SkillSelect = new SkillSelect();
            SkillSelect.Activate();
            SkillUserInterface.SetState(SkillDisplay);

            SkillNotificationsUserInterface = new UserInterface();
            SkillNotifications = new SkillNotice();
            SkillNotifications.Activate();
            SkillNotificationsUserInterface.SetState(SkillNotifications);

            lastSkillUI = false;
        }

        public override void Unload()
        {
            EPBar = null;
            SPBar = null;
            SkillDisplay = null;
            SkillSelect = null;
            SkillNotifications = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdatedGameTime = gameTime;

            if (EPBarUserInterface?.CurrentState != null)
            {
                EPBarUserInterface.Update(gameTime);
            }

            if (SPBarUserInterface?.CurrentState != null)
            {
                SPBarUserInterface.Update(gameTime);
            }

            if (SkillUserInterface?.CurrentState != null)
            {
                SkillUserInterface.Update(gameTime);
            }

            if (SkillNotificationsUserInterface?.CurrentState != null)
            {
                SkillNotificationsUserInterface.Update(gameTime);
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
                    "Gunvolt Mod: SP Bar",
                    delegate {
                        if (_lastUpdatedGameTime != null && SPBarUserInterface?.CurrentState != null)
                        {
                            SPBarUserInterface.Draw(Main.spriteBatch, _lastUpdatedGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );

                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Gunvolt Mod: Skill UI",
                    delegate {
                        if (_lastUpdatedGameTime != null)
                        {
                            if (SkillUserInterface?.CurrentState != null)
                            {
                                SkillUserInterface.Draw(Main.spriteBatch, _lastUpdatedGameTime);
                            }

                            if (SkillNotificationsUserInterface?.CurrentState != null)
                            {
                                SkillNotificationsUserInterface.Draw(Main.spriteBatch, _lastUpdatedGameTime);
                            }
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
                SPBarUserInterface?.SetState(SPBar);
                SwitchSkillScreenState(lastSkillUI);
                hidingUI = false;
            } else
            {
                EPBarUserInterface?.SetState(null);
                SPBarUserInterface?.SetState(null);
                SkillUserInterface?.SetState(null);
                hidingUI = true;
            }
        }
        public void ShowUI()
        {
            EPBarUserInterface?.SetState(EPBar);
            SPBarUserInterface?.SetState(SPBar);
            SwitchSkillScreenState(lastSkillUI);
        }

        public void HideUI()
        {
            EPBarUserInterface?.SetState(null);
            SPBarUserInterface?.SetState(null);
            SkillUserInterface?.SetState(null);
        }
    }
}
