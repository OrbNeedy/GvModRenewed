using GvMod.Content;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Skills
{
    class Gorgoneia : SpecialSkill
    {
        public override string InternalName { get; set; } = "Gorgoneia";
        public override string LocalizationKey { get; set; } = "Gorgoneia";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 16;
        public override int StageRequirement { get; set; } = 0;
        public override int SPCost { get; set; } = 2;
        public override int MaxCooldownTime { get; set; } = 1800;
        public float currentRotation = 0;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            KeepPlayerInPlace(player);
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            currentRotation = 0;
            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            float rotationPerBeam = MathHelper.TwoPi / 10f;

            if (adept.SpecialSkillUseTime % 12 == 0 && adept.SpecialSkillUseTime <= 120)
            {
                Vector2 vel = new Vector2(0, -1).RotatedBy(currentRotation);
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, 
                    vel, ModContent.ProjectileType<GorgoneiaBeam>(), 1, 0, 
                    player.whoAmI, 1.25f, 1, 1);
                currentRotation += rotationPerBeam;
            }

            return adept.SpecialSkillUseTime < 180;
        }

        public override void StatUpdate(Player player, SeptimaPlayer adept)
        {
            player.statDefense += 10;
            player.endurance += 0.5f;
        }
    }
}
