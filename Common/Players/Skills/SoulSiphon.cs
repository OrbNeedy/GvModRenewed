using GvMod.Content;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;
using GvMod.Common.Players.Sevenths;
using Terraria.ID;
using GvMod.Common.Utils;
using GvMod.Content.Projectiles;
using System;

namespace GvMod.Common.Players.Skills
{
    public class SoulSiphon : SpecialSkill
    {
        public override string InternalName { get; set; } = "SoulSiphon";
        public override string LocalizationKey { get; set; } = "SoulSiphon";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = true;
        public override int LevelRequirement { get; set; } = 68;
        public override int StageRequirement { get; set; } = 1;
        public override int SPCost { get; set; } = 3;
        public override int MaxCooldownTime { get; set; } = 600;

        private float TotalStolenLife { get; set; } = 0;
        private float Range { get; set; } = 0;
        public static int MaxSoulSiphonRange = 1800;
        public static int MaxSoulSiphonAttackTime = 180;
        private bool initialSuperState = false;

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
            TotalStolenLife = 0;
            Range = 32; // Two tiles
            initialSuperState = adept.SuperState;

            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center,
                    Vector2.Zero, ModContent.ProjectileType<SoulSiphonProjectile>(), 0,
                    0, player.whoAmI);
            }
            return true;
        }

        public override void StatUpdate(Player player, SeptimaPlayer adept)
        {
            player.GetArmorPenetration<SpecialAttackDamage>() += 200f;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            float baseDamage = 24;
            int totalSkillDuration = MaxSoulSiphonAttackTime + 60;

            if (adept.SpecialSkillUseTime < MaxSoulSiphonAttackTime)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && Main._rand.NextBool(12))
                    {
                        float npcDistance = npc.Center.Distance(player.Center);
                        // Ease-in (adept.SpecialSkillUseTime * adept.SpecialSkillUseTime * (1/18))
                        float addedRange = Easing.EaseInExponential(adept.SpecialSkillUseTime, MaxSoulSiphonAttackTime) * 
                            MaxSoulSiphonRange;
                        if (npcDistance <= Range + addedRange)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                int dustID = Dust.NewDust(npc.Center - npc.Size / 2, (int)npc.Size.X, (int)npc.Size.Y,
                                    DustID.Firework_Pink);
                                Main.dust[dustID].noGravity = true;
                            }
                            int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().
                                ApplyTo(baseDamage + (adept.Stage * 2) + (adept.Level * 0.2f));
                            int direction = 1;

                            if ((npc.Center.X - player.Center.X) < 0)
                            {
                                direction = -1;
                            }

                            bool crit = player.GetTotalCritChance<SpecialAttackDamage>() < Main.rand.NextFloat();

                            float returnDamage = Septima.ApplyDamageToNPCAndReturnFinalDamage(player, npc,
                                finalDamage, 3.5f, direction, crit, ModContent.GetInstance<SpecialAttackDamage>(),
                                true
                            );

                            if (initialSuperState)
                            {
                                returnDamage *= 1.5f;
                            }

                            TotalStolenLife += returnDamage / 8f;
                        }
                    }
                }
            }

            if (adept.SpecialSkillUseTime == totalSkillDuration)
            {
                TotalStolenLife = float.Clamp(TotalStolenLife, 0, 365 + (adept.Stage * 35) + (adept.Level * 2.85f));
                float remainingStolenHealth = TotalStolenLife - (player.statLifeMax2 - player.statLife);
                
                if (remainingStolenHealth <= 0)
                {
                    if (player.statLife < player.statLifeMax2)
                    {
                        player.Heal((int)MathF.Floor(TotalStolenLife));
                    }
                } else
                {
                    if (player.statLife < player.statLifeMax2)
                    {
                        player.Heal(player.statLifeMax2 - player.statLife);
                    }
                    // Main.NewText("Remain: " + remainingStolenHealth);
                    if (initialSuperState && Main.myPlayer == player.whoAmI)
                    {
                        int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>()
                            .ApplyTo(remainingStolenHealth * (1 + (adept.Stage * 0.05f)));
                        Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center,
                            Vector2.Zero, ModContent.ProjectileType<SoulSiphonExplosion>(), finalDamage,
                            8f, player.whoAmI);
                    }
                }
                //Main.NewText("Total life stolen: " + TotalStolenLife);
                //Main.NewText("Remain: " + remainingStolenHealth);

                TotalStolenLife = 0;
            }

            return adept.SpecialSkillUseTime < totalSkillDuration;
        }

        public override void HurtUpdate(Player player, SeptimaPlayer adept, Player.HurtInfo info)
        {
        }
    }
}
