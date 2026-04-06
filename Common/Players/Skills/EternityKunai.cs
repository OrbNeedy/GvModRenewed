using GvMod.Content.Buffs;
using GvMod.Content.Projectiles;
using GvMod.Content;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;
using System.Collections.Generic;
using GvMod.Common.Players.Sevenths;
using System.Linq;

namespace GvMod.Common.Players.Skills
{
    class EternityKunai : SpecialSkill
    {
        public override string InternalName { get; set; } = "EternityKunai";
        public override string LocalizationKey { get; set; } = "EternityKunai";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 0;
        public override int StageRequirement { get; set; } = 0;
        public override int SPCost { get; set; } = 0;
        public override int MaxCooldownTime { get; set; } = 1500;

        public bool initialSuperState = false;
        public List<KunaiSummon> initialSummonTypes = new();

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            KeepPlayerInPlace(player);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            initialSuperState = adept.SuperState;
            initialSummonTypes = new() { };
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (Rebirth.CorpseItemTable.ContainsKey(item.type))
                {
                    KunaiSummon type = Rebirth.CorpseItemTable[item.type];
                    if (!initialSummonTypes.Contains(type))
                    {
                        initialSummonTypes.Add(type);
                    }
                }
            }

            if (initialSummonTypes.Count <= 0)
            {
                initialSummonTypes.Add(KunaiSummon.None);
            }

            /*Main.NewText("Summon types: ");
            foreach (KunaiSummon summon in initialSummonTypes)
            {
                Main.NewText(summon.ToString());
            }*/
            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            if (adept.SpecialSkillUseTime % 4 == 0 && Main.myPlayer == player.whoAmI)
            {
                int projType = ModContent.ProjectileType<Kunai>();
                float baseDamage = adept.septima.
                    GetBasicSkillPower(player, adept);
                int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().
                    ApplyTo(baseDamage + (adept.Stage) + (adept.Level * 0.15f));
                
                float maxRot = MathHelper.PiOver4 / 2f;
                float super = 0;

                Vector2 direction = player.Center.DirectionTo(Main.MouseWorld) * 10;
                int maxKunai = 1;

                if (initialSuperState)
                {
                    maxRot = MathHelper.TwoPi;
                    maxKunai = 3;
                    super = 1;
                }

                KunaiSummon nextSummon = KunaiSummon.None;

                for (int i = 0; i < maxKunai; i++)
                {
                    nextSummon = Main._rand.Next<KunaiSummon>(initialSummonTypes);
                    //Main.NewText("Kunai summon: " + nextSummon.ToString());

                    Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, 
                        direction.RotatedByRandom(maxRot), projType, finalDamage, 4f, 
                        player.whoAmI, (int)nextSummon, super, 0);
                }
            }

            return adept.SpecialSkillUseTime < 180;
        }

        public override void StatUpdate(Player player, SeptimaPlayer adept)
        {
            player.statDefense += 25;
            player.endurance += 0.05f;
            // Kunai projectile is damage type MainAttackDamage which has no base penetration
            player.GetArmorPenetration<MainAttackDamage>() = 
                player.GetArmorPenetration<SpecialAttackDamage>();
        }
    }
}
