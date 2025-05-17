using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Common.UI;
using Terraria.ID;
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

        public override void Load()
        {
            if (Main.dedServ) return;

            EPBarUserInterface = new UserInterface();

            EPBar = new EPBar();
            EPBar.Activate();
            EPBarUserInterface.SetState(EPBar);
        }

        public override void Unload()
        {
            EPBar = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdatedGameTime = gameTime;

            if (EPBarUserInterface?.CurrentState != null)
            {
                EPBarUserInterface.Update(gameTime);
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
            }
        }

        public void ShowUI()
        {
            EPBarUserInterface?.SetState(EPBar);
        }

        public void HideUI()
        {
            EPBarUserInterface?.SetState(null);
        }
    }
}
