using GvMod.Content;
using GvMod.Content.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Common.Players
{
    // Also can be used as a reference to the potency of an anthem
    public enum AnthemAuraType
    {
        Invisible,
        LumenWeak,
        Lumen,
        Djin, 
        Muse
    }

    public class ResurrectionPlayer : ModPlayer
    {
        public bool resurrected = false;
        public bool canResurrect = false;
        public float resurrectionPower = 0;
        public AnthemAuraType type = AnthemAuraType.LumenWeak;

        public override void ResetEffects()
        {
            //Main.NewText("Resurrection Player's ResetEffects.");
            resurrected = false;
            canResurrect = false;
            resurrectionPower = 0;
            base.ResetEffects();
        }

        public override void PreUpdate()
        {
            //Main.NewText("Resurrection Player's PreUpdate.");
            if (Player.HasBuff<Anthem>())
            {
                SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();

                adept.EPUseModifier -= MathHelper.Clamp((resurrectionPower - 1) * 0.5f, 0, 1);
                adept.APRecoveryModifier += resurrectionPower > 3 ? 0.25f : 0;
                adept.EPRecoveryModifier += 0.1f;
                Player.GetDamage<SeptimaDamage>() += 0.15f * resurrectionPower;

                if (Player.HasBuff(ModContent.BuffType<ResurrectionCooldown>()) || resurrectionPower <= 0 ||
                    !canResurrect)
                {
                    resurrected = false;

                    Player.ClearBuff(ModContent.BuffType<Anthem>());
                    if (!Player.HasBuff(ModContent.BuffType<ResurrectionCooldown>()))
                    {
                        Player.AddBuff(ModContent.BuffType<ResurrectionCooldown>(), 18000);
                    }
                }
            }
            base.PreUpdate();
        }

        public override void PostUpdate()
        {
            //Main.NewText("Resurrection Player's PostUpdate.");
            base.PostUpdate();
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            //Main.NewText("Resurrection Player's PreKill.");
            if (resurrectionPower > 0 && !Player.HasBuff<ResurrectionCooldown>() && canResurrect && 
                !Player.HasBuff<Anthem>())
            {
                Player.Heal(Player.statLifeMax2);
                playSound = false;
                genDust = false;
                Player.AddBuff(ModContent.BuffType<Anthem>(), 3600 + (int)(1200 * (resurrectionPower - 1)));
                resurrected = true;
                return false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
    }
}
