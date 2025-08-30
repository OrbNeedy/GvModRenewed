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
        public override int SPCost { get; set; } = 3;
        public override int MaxCooldownTime { get; set; } = 720;

        private int ChainsLeft { get; set; } = 7;
        private int ChainReleaseTimer { get; set; } = 0;
        private int BaseChainReleaseTimer { get; set; } = 8;
        private int ChainWaitTimer { get; set; } = 0;
        private int ElectrocutionTimer { get; set; } = 60;
        private int BaseChainWaitTimer { get; set; } = 0;
        private List<Projectile> ReleasedChains = new();
        private bool EarlyCancel { get; set; } = false;
        private float initialPower = 0;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            player.noFallDmg = true;
            player.velocity = new Vector2(0, 0.0000001f);
            player.position = player.oldPosition;
            player.fallStart = (int)player.Center.Y;
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            ReleasedChains.Clear();
            player.oldPosition = player.position;

            float baseDamage = 50 + (2 * adept.Stage) + (0.2f * adept.Level);
            int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().ApplyTo(baseDamage);
            ChainsLeft = 5 + (int)(adept.Stage / 3);
            // 4 frames per chain so the 12 frame timer will start after all chains have been shot
            BaseChainWaitTimer = ChainWaitTimer = (ChainsLeft * BaseChainReleaseTimer) + 
                (int)VoltaicChainProjectile.MoveTime + 8;
            BaseChainWaitTimer += ElectrocutionTimer;

            if (player.GetModPlayer<ResurrectionPlayer>().resurrected)
            {
                // Main.NewText("Resurrected");
                initialPower = player.GetModPlayer<ResurrectionPlayer>().resurrectionPower;
            }
            else
            {
                // Main.NewText("Not resurrected");
                initialPower = 0;
            }

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
                    chainInfo.Item1, chainInfo.Item2, ModContent.ProjectileType<VoltaicChainProjectile>(), 
                    finalDamage, 4f, player.whoAmI, ElectrocutionTimer, ChainWaitTimer));
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
                    + (10 * adept.Stage) + (0.75f * adept.Level);
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

            if (initialPower >= 2 && adept.SpecialSkillUseTime == BaseChainWaitTimer)
            {
                for (int i = -1; i < 2; i++)
                {
                    float baseDamage = 65 + (5 * adept.Stage) + (0.25f * adept.Level);
                    int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().ApplyTo(baseDamage);
                    Projectile.NewProjectileDirect(player.GetSource_FromThis("Septima"),
                        new Vector2(Main.MouseWorld.X + (100 * i), player.Center.Y), Vector2.Zero,
                        ModContent.ProjectileType<Thunder>(), finalDamage, 0, player.whoAmI, 15);
                }
            }

            if (initialPower >= 3) return adept.SpecialSkillUseTime < BaseChainWaitTimer + 60;

            if (initialPower >= 2) return adept.SpecialSkillUseTime < BaseChainWaitTimer + 60 && !EarlyCancel;

            return adept.SpecialSkillUseTime < BaseChainWaitTimer && !EarlyCancel;
        }

        public override void HurtUpdate(Player player, SeptimaPlayer adept, Player.HurtInfo info)
        {
            // Early cancel if the damage is too high
            if (initialPower >= 3) EarlyCancel = false;
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
                            projectile.velocity = Vector2.Zero;
                        }
                    }
                }
            }
        }
    }
}
