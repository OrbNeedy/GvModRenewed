using GvMod.Content.Buffs;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
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
        public bool FreeFloat { get; set; } = false;
        private Vector2 flyingVelocity = Vector2.Zero;

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            IL_PlayerDrawLayers.DrawPlayer_21_Head_TheFace += ModifyEyes_DrawPlayer_21_Head_TheFace;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
        }

        public override void PreUpdateMovement()
        {
            if (FreeFloat)
            {
                if (Player.controlLeft)
                {
                    flyingVelocity.X -= Player.runAcceleration;
                }
                if (Player.controlRight)
                {
                    flyingVelocity.X += Player.runAcceleration;
                }
                if (Player.controlUp)
                {
                    flyingVelocity.Y -= Player.runAcceleration;
                }
                if (Player.controlDown)
                {
                    flyingVelocity.Y += Player.runAcceleration;
                }

                if (flyingVelocity.Length() > 0)
                {
                    flyingVelocity.Normalize();
                }

                Player.velocity *= 0.9f;
                Player.velocity += flyingVelocity;
            }
            base.PreUpdateMovement();
        }

        public override void PreUpdate()
        {
            if (ArmedPhenomenonStats > 0)
            {
                SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();
                adept.septima.ArmedPhenomenonPreUpdate(Player, adept, ArmedPhenomenonStats);
            }
            if (FreeFloat)
            {
                Player.gravity = 0;
                Player.fallStart = (int)Player.Center.Y;
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
            FreeFloat = false;
            flyingVelocity = Vector2.Zero;
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
