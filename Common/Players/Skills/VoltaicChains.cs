using System.Collections.Generic;
using GvMod.Content.Projectiles;
using GvMod.Content;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using GvMod.Common.Utils;

namespace GvMod.Common.Players.Skills
{
    public class VoltaicChains : SpecialSkill
    {
        public override string InternalName { get; set; } = "VoltaicChains";
        public override string LocalizationKey { get; set; } = "VoltaicChains";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 55;
        public override int StageRequirement { get; set; } = 1;
        public override int APCost { get; set; } = 3;
        public override int MaxCooldownTime { get; set; } = 720;

        private int ChainsLeft { get; set; } = 7;
        private int ChainReleaseTimer { get; set; } = 0;
        private int BaseChainReleaseTimer { get; set; } = 8;
        private int ChainWaitTimer { get; set; } = 0;
        private int ElectrocutionTimer { get; set; } = 60;
        private int BaseChainWaitTimer { get; set; } = 0;
        private List<Projectile> ReleasedChains = new();
        private bool EarlyCancel { get; set; } = false;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            player.noFallDmg = true;
            player.velocity = Vector2.Zero;
            player.position = player.oldPosition;
            player.fallStart = (int)player.Center.Y;
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            ReleasedChains.Clear();
            player.oldPosition = player.position;

            int baseDamage = 50 + (2 * adept.Stage);
            int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().ApplyTo(baseDamage);
            ChainsLeft = 5 + (int)(adept.Stage / 3);
            // 4 frames per chain so the 12 frame timer will start after all chains have been shot
            BaseChainWaitTimer = ChainWaitTimer = (ChainsLeft * BaseChainReleaseTimer) + 
                (int)VoltaicChainProjectile.MoveTime + 8;
            BaseChainWaitTimer += ElectrocutionTimer;
            EarlyCancel = false;
            return true;
        }

        public override void StatUpdate(Player player, SeptimaPlayer adept)
        {
            player.statDefense *= 2;
            player.statDefense += 10;
            player.endurance += 0.4f;
            player.noKnockback = true;
            player.GetArmorPenetration<SpecialAttackDamage>() += 50f;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            if (ChainReleaseTimer >= BaseChainReleaseTimer && ChainsLeft > 0)
            {
                float baseDamage = 150;
                int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().ApplyTo(baseDamage);

                (Vector2, Vector2) chainInfo = ChainGeneration.GetPositionAndSpeed(player.Center, radius: 1200, 
                    VoltaicChainProjectile.MoveTime);
                ReleasedChains.Add(Projectile.NewProjectileDirect(player.GetSource_FromThis("Septima"), 
                    chainInfo.Item1, chainInfo.Item2, ModContent.ProjectileType<VoltaicChainProjectile>(), finalDamage, 
                    4.25f, player.whoAmI, ElectrocutionTimer, ChainWaitTimer));
                ChainReleaseTimer = 0;
                ChainsLeft--;
            }

            ChainReleaseTimer++;
            ChainWaitTimer--;

            List<int> registeredEnemies = new();
            bool electrocute = false;
            foreach (Projectile projectile in ReleasedChains)
            {
                if (projectile.ModProjectile != null)
                {
                    if (projectile.ModProjectile is VoltaicChainProjectile chain && projectile.active && 
                        projectile.owner == player.whoAmI)
                    {
                        if (chain.Electrocuting) electrocute = true;

                        List<int> collection = chain.PiercedEnemies.FindAll((i) => !registeredEnemies.Contains(i));
                        registeredEnemies.AddRange(collection);
                    }
                }
            }

            if (electrocute && adept.SpecialSkillUseTime % 4 == 0)
            {
                float baseDamage = 90 + (10 * MathHelper.Clamp(registeredEnemies.Count, 0, 11)) 
                    + (15 * adept.Stage);
                int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().ApplyTo(baseDamage);

                //Main.NewText("Electricity damage: " + finalDamage);

                foreach (int index in registeredEnemies)
                {
                    NPC target = Main.npc[index];
                    if (!target.friendly && !target.immortal)
                    {
                        // Is this how crits work? We may never know
                        bool crit = player.GetTotalCritChance<SpecialAttackDamage>() < Main.rand.NextFloat();
                        player.ApplyDamageToNPC(target, finalDamage, 0, 0, crit, 
                            ModContent.GetInstance<SpecialAttackDamage>(), true);
                    }
                }
            }

            //Main.NewText("Time elapsed: " + adept.SpecialSkillUseTime);
            //Main.NewText("Time limit: " + BaseChainWaitTimer);
            //Main.NewText("Cancel: " + EarlyCancel);

            return adept.SpecialSkillUseTime < BaseChainWaitTimer && !EarlyCancel;
        }

        public override void HurtUpdate(Player player, SeptimaPlayer adept, Player.HurtInfo info)
        {
            // Early cancel if the damage is too high
            if (info.Damage >= (player.statLifeMax2 / 7) && !EarlyCancel)
            {
                EarlyCancel = true;

                foreach (Projectile projectile in ReleasedChains)
                {
                    if (projectile.ModProjectile != null)
                    {
                        if (projectile.ModProjectile is VoltaicChainProjectile chain &&
                            projectile.owner == player.whoAmI && projectile.active)
                        {
                            // Change if break animation lasts a different amount of frames
                            projectile.timeLeft = VoltaicChainProjectile.BreakTime;
                        }
                    }
                }
            }
        }
    }
}
