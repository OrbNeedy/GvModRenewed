using System.Collections.Generic;
using GvMod.Common.Players.Skills;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using System.Linq;

namespace GvMod.Common.Players.Sevenths
{
    public enum Resistance
    {
        None, 
        Penetrate, // Penetrates prevasion
        Overheat, // Overheats upon contact
        Ignore, // Deals no damage
        Absorb // Recovers EP when hit
    }

    public class Septima
    {
        // For specific value storing
        public virtual Dictionary<string, int> SaveTags { get; set; } = new();

        // Base values
        public virtual float BaseBasicAttackDamage { get; protected set; } = 0;
        public virtual float BaseSecondaryAttackDamage { get; protected set; } = 0;
        public virtual List<SpecialSkill> SkillList { get; protected set; } = new() { new SpecialSkill() };
        public virtual List<SpecialSkill> AvailableSkills { get; protected set; } = new();
        public virtual float EPUseBase { get; protected set; } = 0;
        public virtual float EPRecoveryBaseRate { get; protected set; } = 0; 
        public virtual int EPCooldownBaseTimer { get; protected set; } = 0;
        public virtual float OverheatRecoveryBaseRate { get; protected set; } = 0;
        public virtual float SPRecoveryBaseRate { get; protected set; } = 0f;
        public virtual bool CanChargeWhileAttacking { get; protected set; } = false;
        public virtual bool AllowRecharge { get; protected set; } = true;
        public virtual bool AllowPrevasion { get; protected set; } = true;
        public virtual int PrevasionEPCooldownBaseTimer { get; protected set; } = 0;

        // Identifiers
        public virtual SeptimaType Type { get; protected set; } = SeptimaType.None;
        public virtual string InternalName { get; protected set; } = "None";
        public virtual Color MainColor { get; protected set; } = Color.White;
        // Two separate colors so any septima can have distinct overheat and normal EP bar colors
        // This was basically made for septimas with red colors in their design
        public virtual Color OverheatColor { get; protected set; } = Color.DarkRed;

        // Modifiers
        public virtual int MaxEPModifier { get; set; } = 0;
        public virtual float EPRecoveryModifier { get; set; } = 0;
        public virtual float EPCooldownModifier { get; set; } = 0;
        public virtual float SPRecoveryModifier { get; set; } = 0;
        public virtual float APCooldownModifier { get; set; } = 0;

        public void CalculateSkills(Player player, SeptimaPlayer adept, bool queue = false)
        {
            // TODO: Return the displacement of the index so the player's selected skill won't change after sorting
            List<SpecialSkill> SkillsToAdd = SkillList.FindAll((skill) =>
            {
                bool baseRequirements = skill.LevelRequirement <= adept.Level &&
                    skill.StageRequirement <= adept.Stage;
                // Needs this to unlock, regardless of other conditions
                bool forcedRequirement = skill.UnlockConditions.All(c => c.IsMet());
                // Alternative to the stage and level requirements
                bool alternateCondition = skill.UnlockConditions.Count > 0 && skill.UnlockConditions.All(c => c.IsMet());
                //Main.NewText("Base: " + baseRequirements);
                //Main.NewText("Forced: " + forcedRequirement);
                //Main.NewText("Alternate: " + alternateCondition);
                // Add all skills under the level and stage requirements that are also not included already
                return (baseRequirements || alternateCondition) && !AvailableSkills.Contains(skill) && forcedRequirement;
            });

            AvailableSkills.AddRange(SkillsToAdd);

            if (SkillsToAdd.Count > 0)
            {
                AvailableSkills.Sort(new SkillComparer(4));
            }

            foreach (SpecialSkill skill in AvailableSkills)
            {
                skill.OnSetup(player, adept);
            }

            if (queue)
            {
                foreach (SpecialSkill skill in SkillsToAdd)
                {
                    adept.QueuedSkills.Add(skill.InternalName);
                }
            }
        }

        /// <summary>
        /// Used to set the resistances of the septima to specific NPCs. 
        /// Called after the lookup tables finished loading, so it's safe to request NPC types.
        /// </summary>
        /// <returns>List of resistances.</returns>
        public virtual Dictionary<int, Resistance> GetNPCResistances()
        {
            return new();
        }

        /// <summary>
        /// Used to set the resistances of the septima to specific Projectiles. 
        /// Called after the lookup tables finished loading, so it's safe to request Projectile types.
        /// </summary>
        /// <returns>List of resistances.</returns>
        public virtual Dictionary<int, Resistance> GetProjectileResistances()
        {
            return new();
        }

        /// <summary>
        /// Used to load static values for the septima.
        /// ID tables are not populated yet.
        /// </summary>
        /// <param name="mod"></param>
        public virtual void LoadSeptima(Mod mod)
        {

        }

        /// <summary>
        /// Used to load static values for the septima after tables were populated.
        /// </summary>
        /// <param name="mod"></param>
        public virtual void PostLoadSeptima(Mod mod)
        {

        }

        /// <summary>
        /// Run whenever the septima is assigned to a player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <param name="mod"></param>
        public virtual void InitializeSeptima(Player player, SeptimaPlayer adept, Mod mod)
        {
        }

        public virtual void PostLoadSeptima(Player player, SeptimaPlayer adept)
        {
        }

        /// <summary>
        /// Run after the <see cref="SeptimaPlayer"/> has finished loading the tags, use to assign values.
        /// </summary>
        public virtual void PostTagLoad()
        {
        }

        /// <summary>
        /// Run before the <see cref="SeptimaPlayer"/> has saved data to the tag.
        /// </summary>
        public virtual void PreSaveTag()
        {
        }

        /// <summary>
        /// Use to modify running speeds.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        public virtual void MovementEffects(Player player, SeptimaPlayer adept)
        {

        }

        /// <summary>
        /// Use to modify other speeds.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        public virtual void DirectMovementEffects(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void MiscEffects(Player player, SeptimaPlayer adept)
        {

        }

        public virtual bool CanUseMainSkill(Player player, SeptimaPlayer adept)
        {
            return true;
        }

        /// <summary>
        /// Used to override <see cref="CanUseMainSkill(Player, SeptimaPlayer)"/> so the player can use their main 
        /// skill even if they have no EP.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns>True to allow use of the main skill even if <see cref="CanUseMainSkill(Player, SeptimaPlayer)"/> 
        /// returns false or the player has no EP.</returns>
        public virtual bool CanUseMainSkillNoEP(Player player, SeptimaPlayer adept)
        {
            return false;
        }

        // Return value determines if the use spends any EP
        // Used with a septima timer to determine different use types
        /// <summary>
        /// Run while the player uses the main skill.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns>True if the EP cost will happen.</returns>
        public virtual bool MainSkillUse(Player player, SeptimaPlayer adept)
        {
            return true;
        }

        /// <summary>
        /// Run once for every tag target while the player uses the main skill, regardless of EP consumption.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <param name="index"></param>
        /// <param name="tags"></param>
        /// <returns>The damage dealt by the tag attack.</returns>
        public virtual int TagEffect(Player player, SeptimaPlayer adept, int index, ref NPCTags tags)
        {
            return 0;
        }

        public virtual bool CanUseSecondarySkill(Player player, SeptimaPlayer adept)
        {
            return true;
        }

        public virtual int SecondarySkillUse(Player player, SeptimaPlayer adept)
        {
            return 0;
        }

        /// <summary>
        /// Modifies the hurt parameters before any resistance checks are made.
        /// </summary>
        public virtual void ModifyNPCHurt(Player player, SeptimaPlayer adept, NPC npc, ref Player.HurtModifiers modifiers)
        {

        }

        /// <summary>
        /// Modifies the hurt parameters before any resistance checks are made.
        /// </summary>
        public virtual void ModifyHurt(Player player, SeptimaPlayer adept, ref Player.HurtModifiers modifiers)
        {

        }

        /// <summary>
        /// Modifies the hurt parameters before any resistance checks are made.
        /// </summary>
        public virtual void ModifyProjectileHurt(Player player, SeptimaPlayer adept, Projectile proj, ref Player.HurtModifiers modifiers)
        {

        }

        /// <summary>
        /// Happens after the player is hurt, this means prevasion failed and the septima was not immune to the damage. <br/>
        /// This runs regardless of the hurt type.
        /// </summary>
        /// <param name="info"></param>
        public virtual void OnHurt(Player player, SeptimaPlayer adept, Player.HurtInfo info)
        {

        }

        /// <summary>
        /// Happens after the player is hurt, this means prevasion failed and the septima was not immune to the npc. <br/>
        /// Only runs after NPC hit.
        /// </summary>
        /// <param name="info"></param>
        public virtual void OnHurtByNPC(Player player, SeptimaPlayer adept, NPC npc, Player.HurtInfo info)
        {

        }

        /// <summary>
        /// Happens after the player is hurt, this means prevasion failed and the septima was not immune to the projectile. <br/>
        /// Only runs after projectile hit.
        /// </summary>
        /// <param name="info"></param>
        public virtual void OnHurtByProjectile(Player player, SeptimaPlayer adept, Projectile proj, Player.HurtInfo info)
        {

        }

        public virtual void OnOverheat(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void OnOverheatRecovery(Player player, SeptimaPlayer adept)
        {

        }

        public virtual bool GetSuperState(Player player, SeptimaPlayer adept)
        {
            return false;
        }

        public void UpdateTimers(bool perfectionFlag)
        {
            // For balance purposes, cooldown time reduction will not be modified regardless of septima or items
            foreach (SpecialSkill skill in AvailableSkills)
            {
                if (skill.CooldownTime > 0)
                {
                    if (perfectionFlag)
                    {
                        skill.CooldownTime -= 10;
                    } else
                    {
                        skill.CooldownTime--;
                    }
                }
            }
        }

        public virtual void ResetEffects(Player player, SeptimaPlayer adept)
        {

        }

        public void ForceCooldownEnd()
        {
            foreach (SpecialSkill skill in AvailableSkills)
            {
                skill.CooldownTime = 0;
            }
        }

        public virtual void OnLevelUp(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void OnStageChange(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void OnVeinVisit(Player player, SeptimaPlayer adept, int index)
        {

        }

        public virtual void DuringVeinVisit(Player player, SeptimaPlayer adept, int index, float distance)
        {

        }

        /// <summary>
        /// Runs after any enemy with the boss flag on is defeated, the boss' defeat flag is still not set here.
        /// </summary>
        /// <param name="bossID"></param>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        public virtual void OnBossDefeat(int bossID, Player player, SeptimaPlayer adept)
        {

        }

        /// <summary>
        /// Draws effects passively in a player layer before the BeetleBuff layer. Isn't called if the <br/>
        /// player is dead or has no septima, but will be called on all shadows
        /// </summary>
        /// <param name="drawInfo"></param>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        public virtual void DrawPassive(ref PlayerDrawSet drawInfo, Player player, SeptimaPlayer adept)
        {
        }

        public virtual void DrawAttack(ref PlayerDrawSet drawInfo, Player player, SeptimaPlayer adept)
        {
        }

        /// <summary>
        /// Draws effects passively in a player layer before the BeetleBuff. Isn't called if the player is dead or <br/>
        /// has no septima, but will be called on all shadows
        /// </summary>
        /// <param name="drawInfo"></param>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        public virtual void DrawPassiveBack(ref PlayerDrawSet drawInfo, Player player, SeptimaPlayer adept)
        {
        }

        public virtual void DrawAttackBack(ref PlayerDrawSet drawInfo, Player player, SeptimaPlayer adept)
        {
        }

        public virtual void OnDnizerActive(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void SetArmedPhenomenonEquip(Player player, SeptimaPlayer adept, Mod mod)
        {

        }

        public virtual void ArmedPhenomenonPreUpdate(Player player, SeptimaPlayer adept, int potency)
        {

        }

        public virtual void ArmedPhenomenonPostEquipUpdate(Player player, SeptimaPlayer adept, int potency)
        {

        }

        public virtual void ItemUse(Player player, SeptimaPlayer adept, Item item)
        {

        }

        /// <summary>
        /// Modifies the basic attack power based on septima specifics.
        /// Usually used to modify based on stage and level.
        /// Does not apply to tagged strikes.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns></returns>
        public virtual float GetBasicSkillPower(Player player, SeptimaPlayer adept)
        {
            return BaseBasicAttackDamage;
        }

        /// <summary>
        /// Modifies the attack power of tagged strikes based on septima specifics.
        /// Only applies to tagged strikes.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <param name="tag"></param>
        /// <param name="tagCount"></param>
        /// <returns></returns>
        public virtual float GetTagSkillPower(Player player, SeptimaPlayer adept, Tag tag, int tagCount = 1)
        {
            return BaseBasicAttackDamage;
        }

        public virtual float GetSecondarySkillPower(Player player, SeptimaPlayer adept)
        {
            return BaseSecondaryAttackDamage;
        }

        /// <summary>
        /// Runs whenever a player hits an NPC with either an item or a projectile.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="hit"></param>
        /// <param name="damageDone"></param>
        public virtual void OnHitNPC(Player player, SeptimaPlayer adept, NPC target, 
            NPC.HitInfo hit, int damageDone)
        {
        }

        public virtual void ModifyHitNPC(Player player, SeptimaPlayer adept, NPC target, 
            ref NPC.HitModifiers modifiers)
        {
        }

        // Essentially a duplicate of Player.ApplyDamageToNPC, but returns the final damage
        public static int ApplyDamageToNPCAndReturnFinalDamage(Player player, NPC npc, int damage, 
            float knockback, int direction, bool crit = false, DamageClass? damageType = null, 
            bool damageVariation = false)
        {
            if (!PlayerLoader.CanHitNPC(player, npc))
                return 0;

            var modifiers = npc.GetIncomingStrikeModifiers(damageType ?? DamageClass.Default, 0);
            PlayerLoader.ModifyHitNPC(player, npc, ref modifiers);

            player.ApplyBannerOffenseBuff(npc, ref modifiers);

            modifiers.ArmorPenetration += player.GetTotalArmorPenetration(damageType ?? DamageClass.Generic);

            player.OnHit(npc.Center.X, npc.Center.Y, npc);
            
            NPCKillAttempt attempt = new NPCKillAttempt(npc);
            NPC.HitInfo hit = modifiers.ToHitInfo(damage, crit, knockback, damageVariation, player.luck);
            int dmg = npc.StrikeNPC(hit);
            PlayerLoader.OnHitNPC(player, npc, hit, dmg);

            if (player.accDreamCatcher && !npc.HideStrikeDamage)
                player.addDPS(dmg);

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendStrikeNPC(npc, hit);

            int num2 = Item.NPCtoBanner(npc.BannerID());
            if (num2 >= 0)
                player.lastCreatureHit = num2;

            if (attempt.DidNPCDie())
                player.OnKillNPC(ref attempt, null);

            return dmg;
        }
    }

    public class SkillComparer : IComparer<SpecialSkill>
    {
        float stageRelevance = 1;

        public SkillComparer(float stageRelevance)
        {
            this.stageRelevance = stageRelevance;
        }

        public int Compare(SpecialSkill x, SpecialSkill y)
        {
            float result = 0;

            if (x.LevelRequirement > y.LevelRequirement) result = 1;
            if (x.LevelRequirement < y.LevelRequirement) result = -1;

            return (int)result;
        }
    }
}
