using GvMod.Content.Projectiles;
using GvMod.Content;
using System;
using System.Linq;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace GvMod.Common.Players.Skills
{
    public class Electroshock : SpecialSkill
    {
        public override string InternalName { get; set; } = "Electroshock";
        public override string LocalizationKey { get; set; } = "Electroshock";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = true;
        public override int LevelRequirement { get; set; } = 0;
        public override int StageRequirement { get; set; } = 0;
        public override int SPCost { get; set; } = 0;
        public override int MaxCooldownTime { get; set; } = 1200;

        private float initialEP = 0;
        private int finalDamage = 0;
        private float initialRotation = 0;

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
            player.oldPosition = player.position;

            //Main.NewText("Player EP: " + adept.CurrentEP);
            if (adept.Overheated)
            {
                initialEP = 1;
            } else
            {
                initialEP = adept.CurrentEP;
                adept.ForceOverheat(true, false);
            }

            //Main.NewText("EP: " + initialEP);
            //Main.NewText("Multiplier: " + (1 + (adept.Stage / 60) + (adept.Level / 750)));

            finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().
                    ApplyTo(initialEP * (1 + (adept.Stage / 60) + (adept.Level / 750)));

            Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<Thunder>(), finalDamage, 0, player.whoAmI, 15);

            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<AstrasphereOrbits>(), finalDamage, 2, player.whoAmI,
                    -1, initialRotation + (MathHelper.TwoPi * i / 3), (int)OrbitsBehavior.ElectroshockCounterclock);
            }

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            if (adept.SpecialSkillUseTime != 0)
            {
                if (adept.SpecialSkillUseTime % 12 == 0)
                {
                    float distance = adept.SpecialSkillUseTime / 12f;
                    Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center + new Vector2(distance * 90, 0), 
                        Vector2.Zero, ModContent.ProjectileType<Thunder>(), finalDamage, 0, 
                        player.whoAmI, 1);
                    Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center - new Vector2(distance * 90, 0),
                        Vector2.Zero, ModContent.ProjectileType<Thunder>(), finalDamage, 0,
                        player.whoAmI, 1);
                }

                if (adept.SpecialSkillUseTime % 20 == 0)
                {
                    int behavior = adept.SpecialSkillUseTime % 40 == 0 ? 
                        (int)OrbitsBehavior.Electroshock : (int)OrbitsBehavior.ElectroshockCounterclock;
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero,
                            ModContent.ProjectileType<AstrasphereOrbits>(), finalDamage, 2, player.whoAmI,
                            -1, initialRotation + (MathHelper.TwoPi * i / 3), behavior);
                    }
                }
            }

            initialRotation += MathHelper.TwoPi / 40;

            return adept.SpecialSkillUseTime < 120;
        }

        public override bool? CustomUnlockCondition(Player player, SeptimaPlayer adept)
        {
            return adept.DragonVeinsVisited[4];
        }
    }
}
