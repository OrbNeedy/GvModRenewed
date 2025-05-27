using GvMod.Content;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Skills
{
    public class Astrasphere : SpecialSkill
    {
        public override string InternalName { get; set; } = "Astrasphere";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 1;
        public override int StageRequirement { get; set; } = 1;
        public override int APCost { get; set; } = 1;
        public override int MaxCooldownTime { get; set; } = 900;

        private int fieldIndex = -1;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            player.noFallDmg = true;
            player.velocity = Vector2.Zero;
            player.position = player.oldPosition;
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            player.oldPosition = player.position;
            int finalFieldDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().
                    ApplyTo(66);
            int finalSphereDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().
                    ApplyTo(33);
            fieldIndex = Projectile.NewProjectile(player.GetSource_Misc("Septima"), player.Center, Vector2.Zero, 
                ModContent.ProjectileType<Flashfield>(), finalFieldDamage, 3, player.whoAmI, 
                (int)FlashfieldBehavior.Astrasphere);
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(player.GetSource_Misc("Septima"), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<AstrasphereOrbits>(), finalSphereDamage, 1, player.whoAmI,
                    fieldIndex, MathHelper.Pi + (MathHelper.TwoPi * i / 3));
            }
            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            /*field.timeLeft = 3;
            field.friendly = true;
            field.hostile = false;
            field.netUpdate = true;*/
            /*Main.NewText($"Field index: {fieldIndex}");
            if (fieldIndex >= 0)
            {
                Projectile field = Main.projectile[fieldIndex];
                Main.NewText($"Field active: {field.active}");
                Main.NewText($"Field valid: {field.ModProjectile is Flashfield}");
                Main.NewText($"Field owner: {field.owner}");
                Main.NewText($"Player index: {player.whoAmI}");
            }*/

            return adept.SpecialSkillUseTime < 150;
        }

        public override void NPCHitUpdate(Player player, SeptimaPlayer adept, NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (fieldIndex < 0)
            {
                return;
            }

            Projectile field = Main.projectile[fieldIndex];
            if (field.active && field.ModProjectile is Flashfield &&
                field.owner == player.whoAmI)
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
            if (field.active && field.ModProjectile is Flashfield &&
                field.owner == player.whoAmI)
            {
                modifiers.Cancel();
            }
        }
    }
}
