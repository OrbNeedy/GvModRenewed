using Terraria.ID;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using GvMod.Content.Buffs;
using Terraria.Audio;
using System;

namespace GvMod.Common.Players.Skills
{
    public class Resurrection : SpecialSkill
    {
        public override string InternalName { get; set; } = "Resurrection";
        public override string LocalizationKey { get; set; } = "Resurrection";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 10;
        public override int StageRequirement { get; set; } = 0;
        public override int SPCost { get; set; } = 1;
        public override int MaxCooldownTime { get; set; } = 1200;
        public bool initialSuperState = false;
        public bool initialPulsarBoost = false;

        public override string GetNewNameKey(Player player, SeptimaPlayer adept)
        {
            if (adept.SuperState)
            {
                return "Resurexionn";
            }
            return base.GetNewNameKey(player, adept);
        }

        public override string GetNewDescriptionKey(Player player, SeptimaPlayer adept)
        {
            if (adept.SuperState)
            {
                return "Resurexionn";
            }
            return base.GetNewDescriptionKey(player, adept);
        }

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            KeepPlayerInPlace(player);
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            initialSuperState = adept.SuperState;
            initialPulsarBoost = player.GetModPlayer<SetBonusPlayer>().pulsarUpgrade;
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/Resurrection") with
            {
                Volume = 0.5f,
                PitchVariance = 0.1f
            }, player.Center, (soundInstance) => { return SoundCancelLogic(soundInstance, adept); });
            return true;
        }

        private bool SoundCancelLogic(ActiveSound soundInstance, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 180;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            if (Main._rand.NextBool(3))
            {
                for (int i = 0; i < Main._rand.Next(1, 4); i++)
                {
                    Dust.NewDustPerfect(player.Center, DustID.ShimmerSpark, newColor: adept.septima.MainColor);
                }
            }

            if (adept.SpecialSkillUseTime >= 30)
            {
                float potency = 1f / 3f;

                if (initialSuperState) potency = 1f;

                ResurrectPlayers(player, adept, potency);

                if (initialPulsarBoost && adept.SpecialSkillUseTime >= 180)
                {
                    OffensiveResurrection.ResurrectBoss(player, adept, initialSuperState);
                    player.AddBuff(ModContent.BuffType<UnlimitedAnimusBuff>(), 300);
                }

                if (adept.SpecialSkillUseTime >= 180)
                {
                    return false;
                }
            }

            return true;
        }
        
        public static void ResurrectPlayers(Player player, SeptimaPlayer adept, float resurrectionPower = 1f / 3f)
        {
            foreach (Player otherPlayer in Main.ActivePlayers)
            {
                if (otherPlayer.whoAmI != player.whoAmI && otherPlayer.DeadOrGhost &&
                    !otherPlayer.InOpposingTeam(player) && otherPlayer.Distance(player.Center) <= 3600)
                {
                    if (Main._rand.NextBool(3))
                    {
                        for (int i = 0; i < Main._rand.Next(1, 4); i++)
                        {
                            Dust.NewDustPerfect(otherPlayer.Center, DustID.ShimmerSpark,
                                newColor: adept.septima.MainColor);
                        }
                    }

                    if (adept.SpecialSkillUseTime >= 180)
                    {
                        otherPlayer.GetModPlayer<ResurrectionPlayer>().forcedRebirthResurrection = resurrectionPower;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            ResurrectionPlayer.SyncRebirth(otherPlayer.whoAmI, resurrectionPower);
                        }

                        for (int i = 0; i < Main._rand.Next(10, 30); i++)
                        {
                            Dust.NewDustPerfect(otherPlayer.Center, DustID.ShimmerSpark,
                                newColor: adept.septima.MainColor);
                        }
                    }
                }
            }
        }

        public override void NPCHitUpdate(Player player, SeptimaPlayer adept, NPC npc, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage /= 2;
        }

        public override void ProjectileHitUpdate(Player player, SeptimaPlayer adept, Projectile projectile, 
            ref Player.HurtModifiers modifiers)
        {
            // Add Greed Snatcher exception
            modifiers.SourceDamage /= 2;
        }
    }
}
