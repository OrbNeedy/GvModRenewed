using GvMod.Content.Buffs;
using Microsoft.Xna.Framework;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.Players
{
    public class SetBonusPlayer : ModPlayer
    {
        public static int DnizerModeMaxTimer = 2400;
        public static int DnizerModeActivateMaxTimer = 60;
        public bool oldDoubleTapAction = false;
        public bool doubleTapAction = false;
        public bool DragonSaviorsBonus = false;
        public int DnizerModeTriggerCooldown = 0;
        public int DnizerModeTimer = 0;
        public int DnizerModeDamageTimer = 0;
        public int OldDnizerModeActivateTimer = 0;
        public int DnizerModeActivateTimer = 0;

        public override void PostUpdate()
        {
            SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();
            PlayerPrevasion prevasion = Player.GetModPlayer<PlayerPrevasion>();
            float dnizerTimerPercent = (float)DnizerModeTimer / (float)DnizerModeMaxTimer;
            DnizerModeActivateMaxTimer = 60 + (int)(dnizerTimerPercent * 120);
            if (adept.DnizerMode)
            {
                Player.AddBuff(ModContent.BuffType<DnizerBuff>(), 3);
                if (DnizerModeTimer < DnizerModeMaxTimer)
                {
                    DnizerModeTimer++;
                    if (adept.UsingMainSkill && adept.MainSkillUseTime % 2 == 0)
                    {
                        DnizerModeTimer++;
                    }
                    if (adept.UsingSpecialSkill && adept.MainSkillUseTime % 3 == 0)
                    {
                        DnizerModeTimer += 2;
                    }
                    if (prevasion.PrevasionIframes > 0)
                    {
                        DnizerModeTimer += 2;
                    }
                } else
                {
                    DnizerModeDamageTimer++;
                }
            } else
            {
                if (adept.PreviousDnizerState == true)
                {
                    DnizerModeTriggerCooldown = 300;
                    DnizerModeDamageTimer = 0;
                }
                if (DnizerModeTimer > 0 && DnizerModeActivateTimer <= 0)
                {
                    DnizerModeTimer--;
                    if (adept.perfectionCheck)
                    {
                        DnizerModeTimer -= 11;
                    }
                }
            }
            base.PostUpdate();
        }

        public override void UpdateBadLifeRegen()
        {
            if (DnizerModeDamageTimer > 0)
            {
                Player.lifeRegen -= (2 * DnizerModeDamageTimer / 120);
            }
        }

        public override void ArmorSetBonusActivated()
        {
            SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();
            if (DragonSaviorsBonus && DnizerModeTriggerCooldown <= 0 &&
                DnizerModeTimer < DnizerModeMaxTimer && !adept.DnizerMode)
            {
                doubleTapAction = true;
            }
        }

        public override void ArmorSetBonusHeld(int holdTime)
        {
            SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();
            if (DragonSaviorsBonus && DnizerModeTriggerCooldown <= 0 &&
                DnizerModeTimer < DnizerModeMaxTimer && !adept.DnizerMode && doubleTapAction)
            {
                DnizerModeActivateTimer = holdTime;
                if (DnizerModeActivateTimer >= DnizerModeActivateMaxTimer)
                {
                    Player.AddBuff(ModContent.BuffType<DnizerBuff>(), 3);
                    adept.septima.OnDnizerActive(Player, adept);
                    DnizerModeTriggerCooldown = 300;
                }
            }
        }

        public override void ResetEffects()
        {
            DragonSaviorsBonus = false;
            if (DnizerModeTriggerCooldown > 0)
            {
                DnizerModeTriggerCooldown--;
            }
            if (DnizerModeActivateTimer == OldDnizerModeActivateTimer)
            {
                DnizerModeActivateTimer = 0;
                doubleTapAction = false;
            }
            DnizerModeTimer = (int)MathHelper.Clamp(DnizerModeTimer, 0, DnizerModeMaxTimer);
            OldDnizerModeActivateTimer = DnizerModeActivateTimer;
        }
    }
}
