using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Content;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.Players
{
    public class AltTagPlayer : ModPlayer
    {
        public List<int> TaggedNPCs { get; set; }
        public bool DamageTags { get; set; }

        public override void PreUpdate()
        {
            if (DamageTags)
            {
                foreach (var item in TaggedNPCs)
                {
                    NPC target = Main.npc[item];
                    
                    if (!target.active) continue;

                    Player.ApplyDamageToNPC(Main.npc[item], 40, 0, (int)Player.DirectionTo(target.Center).X, false, 
                        ModContent.GetInstance<SpecialAttackDamage>(), true);
                }
            }
        }
    }
}
