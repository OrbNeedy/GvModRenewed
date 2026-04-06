using GvMod.Content;
using Microsoft.Xna.Framework;
using Terraria;
using GvMod.Common.Players.Sevenths;
using Terraria.ID;
using Terraria.Localization;
using GvMod.Content.Buffs;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Skills
{
    class OffensiveResurrection : SpecialSkill
    {
        public override string InternalName { get; set; } = "OffensiveResurrection";
        public override string LocalizationKey { get; set; } = "OffensiveResurrection";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 10;
        public override int StageRequirement { get; set; } = 0;
        public override int SPCost { get; set; } = 1;
        public override int MaxCooldownTime { get; set; } = 1200;
        public bool initialSuperState = false;
        public bool initialPulsarBoost = false;

        public override string GetFinalName(Player player, SeptimaPlayer adept)
        {
            if (adept.septima is Rebirth rebirth)
            {
                // Boss ID is not -1 and is in the list
                bool validBossID = rebirth.LastBossKilled != -1 &&
                    Rebirth.BossDefeatTable.ContainsKey(rebirth.LastBossKilled);
                if (validBossID)
                {
                    string bossName = Lang.GetNPCName(rebirth.LastBossKilled).Value;
                    return Language.GetTextValue($"Mods.GvMod.Skills.{LocalizationKey}.DisplayName") + "\n" + bossName;
                }
            }
            return base.GetFinalName(player, adept);
        }

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            KeepPlayerInPlace(player);
            base.MoveUpdate(player, adept);
        }

        public override bool CanUse(Player player, SeptimaPlayer adept)
        {
            if (adept.septima is Rebirth rebirth)
            {
                // Boss ID is not -1 and is in the list
                bool validBossID = rebirth.LastBossKilled != -1 && 
                    Rebirth.BossDefeatTable.ContainsKey(rebirth.LastBossKilled);
                // Boss was previously killed in the world
                bool killedInWorld = Main.BestiaryTracker.Kills.GetKillCount(
                    ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[rebirth.LastBossKilled]
                    ) >= 1;
                bool maxSpawn = false;
                if (validBossID)
                {
                    maxSpawn = player.ownedProjectileCounts[Rebirth.BossDefeatTable[rebirth.LastBossKilled].projectileID] < 1;
                }
                return killedInWorld && maxSpawn;
            }
            return false;
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            initialSuperState = adept.SuperState;
            initialPulsarBoost = player.GetModPlayer<SetBonusPlayer>().pulsarUpgrade;
            return true;
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

            if (adept.SpecialSkillUseTime >= 30 && Main.myPlayer == player.whoAmI)
            {
                ResurrectBoss(player, adept, initialSuperState);

                if (initialPulsarBoost)
                {
                    Resurrection.ResurrectPlayers(player, adept, initialSuperState ? 1f : 1f / 3f);
                    player.AddBuff(ModContent.BuffType<UnlimitedAnimusBuff>(), 300);
                }

                if (adept.SpecialSkillUseTime >= 180)
                {
                    return false;
                }
            }

            return true;
        }

        public static void ResurrectBoss(Player player, SeptimaPlayer adept, bool superState = false)
        {
            Vector2 mouse = Main.MouseWorld;
            if (adept.septima is Rebirth rebirth && rebirth.LastBossKilled != -1)
            {
                // Boss ID is not -1 and is in the list
                bool validBossID = rebirth.LastBossKilled != -1 &&
                    Rebirth.BossDefeatTable.ContainsKey(rebirth.LastBossKilled);
                // Boss was previously killed in the world
                bool killedInWorld = Main.BestiaryTracker.Kills.GetKillCount(
                    ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[rebirth.LastBossKilled]
                    ) >= 1;
                // Max of one per boss
                bool maxSpawn = player.ownedProjectileCounts[Rebirth.BossDefeatTable[rebirth.LastBossKilled].projectileID] < 1;

                if (!validBossID || !killedInWorld || !maxSpawn) return;

                if (Main._rand.NextBool(3))
                {
                    for (int i = 0; i < Main._rand.Next(1, 4); i++)
                    {
                        Dust.NewDustPerfect(mouse, DustID.ShimmerSpark,
                            newColor: adept.septima.MainColor);
                    }
                }

                if (adept.SpecialSkillUseTime >= 180)
                {
                    BossMinionStats stats = Rebirth.BossDefeatTable[rebirth.LastBossKilled];
                    int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().
                        ApplyTo(stats.baseDamage * (1 + (adept.Stage * 0.02f) + (adept.Level * 0.003f)));
                    // Spawn Projectile Boss
                    string super = superState ? ":Super" : "";
                    Projectile.NewProjectile(player.GetSource_FromThis("Septima" + super), mouse,
                        Vector2.Zero, stats.projectileID, finalDamage, stats.baseKnockback,
                        player.whoAmI, stats.ai0, stats.ai1, stats.ai2);

                    for (int i = 0; i < Main._rand.Next(10, 30); i++)
                    {
                        Dust.NewDustPerfect(mouse, DustID.ShimmerSpark,
                            newColor: adept.septima.MainColor);
                    }
                }
            }
        }

        public override void StatUpdate(Player player, SeptimaPlayer adept)
        {
            player.statDefense += 15;
            player.endurance += 0.15f;
        }
    }
}
