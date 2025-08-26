using GvMod.Content;
using GvMod.Content.Buffs;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Skills
{
    public class Astrasphere : SpecialSkill
    {
        public override string InternalName { get; set; } = "Astrasphere";
        public override string LocalizationKey { get; set; } = "Astrasphere";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 1;
        public override int StageRequirement { get; set; } = 1;
        public override int SPCost { get; set; } = 1;
        public override int MaxCooldownTime { get; set; } = 900;

        private int fieldIndex = -1;
        private float initialPower = 0;

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
            player.oldPosition = player.position;
            int finalFieldDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().
                    ApplyTo(71.4f + (1.6f * adept.Stage));
            int finalSphereDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().
                    ApplyTo(37.4f + (1.6f * adept.Stage));
            int fieldAI = (int)AstraspheredBehavior.Default;
            int orbitAI = (int)OrbitsBehavior.Default;

            ResurrectionPlayer resurrectionState = player.GetModPlayer<ResurrectionPlayer>();
            if (resurrectionState.resurrected)
            {
                if (resurrectionState.resurrectionPower >= 2)
                {
                    fieldAI = (int)AstraspheredBehavior.Launch;
                    orbitAI = (int)OrbitsBehavior.Launch;
                }

                if (resurrectionState.resurrectionPower >= 3 || player.HasBuff<SeptimalSurgeBuff>())
                {
                    orbitAI = (int)OrbitsBehavior.Spread;
                }

                initialPower = resurrectionState.resurrectionPower;
            } else 
            {
                initialPower = 0;
            }

            fieldIndex = Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero,
                ModContent.ProjectileType<AstrasphereProjectile>(), finalFieldDamage, 3, player.whoAmI,
                fieldAI);
            
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<AstrasphereOrbits>(), finalSphereDamage, 1, player.whoAmI,
                    fieldIndex, MathHelper.Pi + (MathHelper.TwoPi * i / 3), orbitAI);
            }
            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            if (initialPower >= 3) return adept.SpecialSkillUseTime < 330;

            if (initialPower >= 2) return adept.SpecialSkillUseTime < 230;

            return adept.SpecialSkillUseTime < 130;
        }

        public override void NPCHitUpdate(Player player, SeptimaPlayer adept, NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (fieldIndex < 0)
            {
                return;
            }

            Projectile field = Main.projectile[fieldIndex];
            if (field.active && field.ModProjectile is AstrasphereProjectile &&
                field.owner == player.whoAmI && field.Center.Distance(player.Center) <= 90)
            {
                modifiers.Cancel();
            }
        }

        public override void ProjectileHitUpdate(Player player, SeptimaPlayer adept, Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            if (fieldIndex < 0)
            {
                return;
            }

            Projectile field = Main.projectile[fieldIndex];
            if (field.active && field.ModProjectile is AstrasphereProjectile &&
                field.owner == player.whoAmI && field.Center.Distance(player.Center) <= 90)
            {
                modifiers.Cancel();
            }
        }
    }
}
