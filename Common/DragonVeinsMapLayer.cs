using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Common.Players;
using GvMod.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace GvMod.Common
{
    public class DragonVeinsMapLayer : ModMapLayer
    {
        public override Position GetDefaultPosition() => new After(IMapLayer.Pings);

        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            if (!NPC.downedAncientCultist && !Main.mapFullscreen) return;
            const float scaleIfNotSelected = 1f;
            const float scaleIfSelected = scaleIfNotSelected * 2f;

            Texture2D icon = ModContent.Request<Texture2D>("GvMod/Assets/Icons/DragonVein").Value;

            SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
            
            for (int i = 0; i < adept.DragonVeinsVisited.Length; i++)
            {
                if (i > ModContent.GetInstance<DragonVeinsSystem>().veinPoints.Count) break;
                Point16 vein = ModContent.GetInstance<DragonVeinsSystem>().veinPoints[i];

                if (context.Draw(icon, new Vector2(vein.X, vein.Y),
                    Color.White, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected,
                    Alignment.Center).IsMouseOver)
                {
                    text = adept.DragonVeinsVisited[i] ? Language.GetTextValue("Mods.GvMod.DragonVeins.Visited") : 
                        Language.GetTextValue("Mods.GvMod.DragonVeins.Unseen");
                }
            }
        }
    }
}
