using GvMod.Common.Players.Sevenths;
using GvMod.Content.Buffs;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.Players
{
    public class PlayerBuffs : ModPlayer
    {
        public bool AlchemicalField { get; set; } = false;
        public bool InfiniteSurge { get; set; } = false;
        public bool ArmedPhenomenonVisuals { get; set; } = false;
        public int ArmedPhenomenonStats { get; set; } = 0;

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            IL_PlayerDrawLayers.DrawPlayer_21_Head_TheFace += ModifyEyes_DrawPlayer_21_Head_TheFace;
        }

        public override void PreUpdate()
        {
            if (ArmedPhenomenonStats > 0)
            {
                SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();
                adept.septima.ArmedPhenomenonPreUpdate(Player, adept, ArmedPhenomenonStats);
            }
            base.PreUpdate();
        }

        public override void PostUpdateEquips()
        {
            if (ArmedPhenomenonStats > 0)
            {
                Player.AddBuff(ModContent.BuffType<ArmedPhenomenonBuff>(), 3);
                SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();
                adept.septima.ArmedPhenomenonPostEquipUpdate(Player, adept, ArmedPhenomenonStats);
            }
            base.PostUpdateEquips();
        }

        public override void FrameEffects()
        {
            if (ArmedPhenomenonVisuals)
            {
                SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();
                adept.septima.SetArmedPhenomenonEquip(Player, adept, Mod);
            }
            base.FrameEffects();
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            base.ModifyDrawInfo(ref drawInfo);
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            if (AlchemicalField)
            {
                healValue *= 2;
            }
        }

        public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
        {
            if (AlchemicalField)
            {
                healValue *= 2;
            }
        }

        public override bool OnPickup(Item item)
        {
            return base.OnPickup(item);
        }

        public override void ResetEffects()
        {
            AlchemicalField = false;
            InfiniteSurge = false;
            ArmedPhenomenonVisuals = false;
            ArmedPhenomenonStats = 0;
        }

        private void ModifyEyes_DrawPlayer_21_Head_TheFace(ILContext il)
        {
            try
            {
                var c = new ILCursor(il);

                c.GotoNext(MoveType.After,
                    i => i.MatchLdfld(typeof(PlayerDrawSet), "colorEyeWhites"));

                c.EmitLdarg0();
                c.EmitLdfld(typeof(PlayerDrawSet).GetField("drawPlayer"));

                c.EmitDelegate((Color originalColor, Player player) => 
                {
                    if (!player.ghost && 
                        player.GetModPlayer<PlayerBuffs>().ArmedPhenomenonVisuals)
                    {
                        return Color.Black;
                    }
                    return originalColor;
                });

                c.GotoNext(MoveType.After,
                    i => i.MatchLdfld(typeof(PlayerDrawSet), "colorEyes"));

                c.EmitLdarg0();
                c.EmitLdfld(typeof(PlayerDrawSet).GetField("drawPlayer"));

                c.EmitDelegate((Color originalColor, Player player) =>
                {
                    if (!player.ghost &&
                        player.GetModPlayer<PlayerBuffs>().ArmedPhenomenonVisuals)
                    {
                        return player.GetModPlayer<SeptimaPlayer>().septima.MainColor;
                    }
                    return originalColor;
                });
            }
            catch (Exception e)
            {
                ModContent.GetInstance<GvMod>().Logger.Error("Error adding IL edit on IL_PlayerDrawLayers.",
                    e);
                MonoModHooks.DumpIL(ModContent.GetInstance<GvMod>(), il);
            }
        }
    }
}
