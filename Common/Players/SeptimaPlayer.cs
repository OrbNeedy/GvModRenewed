using GvMod.Common.Players.Sevenths;
using GvMod.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using GvMod.Common.Utils;
using GvMod.Common.Players.Skills;

namespace GvMod.Common.Players
{
    public class SeptimaPlayer : ModPlayer
    {
        // Septima identifiers
        public SeptimaType septimaType = SeptimaType.None;
        public Septima septima = null;
        public int subType = 0;

        // Adept stats
        // Levels increase by different methods, such as using upgrade items and beating bosses
        // They are a way to gauge the player's progress
        public int Level { get; set; } = 1;
        // Stage serves a similar purpose, but is a broader qualification of the player's progress in the game
        // Used to evolve septimas
        public int Stage { get; set; } = 1;
        // This is permanent max, it's affected by permanent upgrades and limited by SeptimaUpgrades
        public int BaseMaxEP { get; set; } = 100;
        // This one is affected by equipment and other modifiers, it's reset later
        public int ModifiedMaxEP { get; set; } = 0;
        public float CurrentEP { get; set; } = 100;
        public int EPCooldownTimer { get; set; } = 0;
        // This is permanent max, it's affected by permanent upgrades and limited by SeptimaUpgrades.
        // Up to two upgrades are planned
        public int BaseMaxAP { get; set; } = 2;
        // This one is affected by equipment and other modifiers, it's reset later
        public int ModifiedMaxAP { get; set; } = 0;
        public float CurrentAP { get; set; } = 2;


        // State related
        public bool Overheated { get; set; } = false;
        public bool UsingMainSkill { get; set; } = false;
        public int MainSkillUseTime { get; set; } = 0;
        public bool UsingSecondarySkill { get; set; } = false;
        public int SecondarySkillUseTime { get; set; } = 0;
        public bool UsingSpecialSkill { get; set; } = false;
        public int SpecialSkillUseTime { get; set; } = 0;
        public NPCTags TaggedNPCs = new();

        // Stat modifiers
        // Base modifiers, septima and item modifiers get added to this
        public float EPRecoveryModifier { get; set; } = 1;
        public float EPCooldownModifier { get; set; } = 1;
        public float EPUseModifier { get; set; } = 1;
        public float OverheatRecoveryModifier { get; set; } = 1;
        public float APRecoveryModifier { get; set; } = 1;
        // Skills will only have one key to activate it, and another key to select it quickly 
        public int SelectedSkill { get; set; } = 0;
        public int SecondarySkillCooldown { get; set; } = 0;
        // Upgrades affect max EP and AP
        public bool[] SeptimaUpgrades = new bool[] { false, false, false, false };

        static SeptimaType[] _selectableSeptimas = { SeptimaType.AzureStriker };

        public override void Initialize()
        {
            if (septima == null)
            {
                septimaType = _selectableSeptimas[Main.rand.Next(0, _selectableSeptimas.Length)];
                switch (septimaType)
                {
                    case SeptimaType.AzureStriker:
                    default:
                        septima = new AzureStriker();
                        break;
                }
            }

            septima.InitializeSeptima(Player, this);
            septima.CalculateSkills(Player, this);
        }

        public override void Load()
        {
        }

        public override void SetStaticDefaults()
        {
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (UsingSpecialSkill)
            {
                //Main.NewText("Checking");
                if (!septima.AvailableSkills[SelectedSkill].AllowsMovement)
                {
                    //Main.NewText("No movement");
                    Player.controlJump = false;
                    Player.controlDown = false;
                    Player.controlLeft = false;
                    Player.controlRight = false;
                    Player.controlUp = false;
                    Player.controlUseItem = false;
                    Player.controlUseTile = false;
                    Player.controlThrow = false;
                    Player.gravDir = 1f;
                }
            }

            if (Player.DeadOrGhost || Player.CCed) return;

            if (KeybindSystem.primaryAbility.JustPressed)
            {
                UsingMainSkill = true;
            }
            if (!KeybindSystem.primaryAbility.Current)
            {
                UsingMainSkill = false;
            }

            if (KeybindSystem.secondaryAbility.JustPressed)
            {
                if (CanUseSecondarySkill())
                {
                    UsingSecondarySkill = true;
                }
            }

            if (KeybindSystem.specialAbility.JustPressed)
            {
                // Skilless adepts don't get to use specials
                if (septima.SkillList.Count <= 0) return;

                SelectedSkill = (int)MathHelper.Clamp(SelectedSkill, 0, septima.SkillList.Count - 1);
                SpecialSkill special = septima.SkillList[SelectedSkill];

                if (special.CanUse(Player, this) && CurrentAP >= special.APCost && !UsingSpecialSkill && 
                    !UsingSecondarySkill && !Player.CCed && special.CooldownTime <= 0)
                {
                    UsingSpecialSkill = special.OnSkillUse(Player, this);
                    CurrentAP -= special.APCost;
                    special.CooldownTime = special.MaxCooldownTime;
                }
            }

            if (KeybindSystem.nextAbility.JustPressed)
            {
                ChangeSkill(1);
            }
            if (KeybindSystem.previousAbility.JustPressed)
            {
                ChangeSkill(-1);
            }

            if (KeybindSystem.abilityMenu.JustPressed)
            {
                ModContent.GetInstance<UISystem>().SwitchUIVisibility();
            }
        }

        public void ChangeSkill(int displacement)
        {
            if (UsingSpecialSkill) return;

            SelectedSkill += (int)MathHelper.Clamp(displacement, -1, 1);

            if (SelectedSkill >= septima.SkillList.Count)
            {
                SelectedSkill = 0;
            }

            if (SelectedSkill < 0)
            {
                SelectedSkill = septima.SkillList.Count - 1;
            }

        }

        public override void OnEnterWorld()
        {
            CurrentEP = GetTotalMaxEP();
            CurrentAP = GetTotalMaxAP();
            EPCooldownTimer = 0;
            SecondarySkillCooldown = 0;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["SeptimaType"] = (int)septimaType;
            tag["SeptimaSubType"] = subType;

            tag["Level"] = Level;
            tag["Stage"] = Stage;
            tag["MaxEP"] = BaseMaxEP;
            tag["MaxAP"] = BaseMaxAP;
            for (int i = 0; i < SeptimaUpgrades.Length; i++)
            {
                tag[$"SeptimaUpgrades{i}"] = SeptimaUpgrades[i];
            }

            tag["SelectedSkill"] = SelectedSkill;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("SeptimaType"))
            {
                septimaType = (SeptimaType)tag.GetInt("SeptimaType");
            }
            if (tag.ContainsKey("SeptimaSubType"))
            {
                subType = tag.GetInt("SeptimaSubType");
            }

            if (tag.ContainsKey("MaxEP"))
            {
                BaseMaxEP = tag.GetInt("MaxEP");
            }
            if (tag.ContainsKey("Level"))
            {
                Level = tag.GetInt("Level");
            }
            if (tag.ContainsKey("Stage"))
            {
                Stage = tag.GetInt("Stage");
            }
            if (tag.ContainsKey("MaxAP"))
            {
                BaseMaxAP = tag.GetInt("MaxAP");
            }
            for (int i = 0; i < SeptimaUpgrades.Length; i++)
            {
                if (tag.ContainsKey($"SeptimaUpgrades{i}"))
                {
                    SeptimaUpgrades[i] = tag.GetBool($"SeptimaUpgrades{i}");
                }
            }

            if (tag.ContainsKey("SelectedSkill"))
            {
                SelectedSkill = tag.GetInt("SelectedSkill");
            }
        }

        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
        }

        public override void PreUpdateBuffs()
        {
            septima.MiscEffects(Player, this);
            base.PreUpdateBuffs();
        }

        public override void PreUpdate()
        {
            TaggedNPCs.Update(this);
            
            // Dead men have no septima
            if (Player.DeadOrGhost) return;

            // TODO: Test if this actually works for stat modifications
            // septima.MiscEffects(Player, this);

            if (UsingSpecialSkill)
            {
                SpecialSkill special = septima.SkillList[SelectedSkill];
                if (Player.CCed && !special.Invincible)
                {
                    special.ForcedSkillEnd(Player, this);
                    UsingSpecialSkill = false;
                } else
                {
                    special.CooldownTime = special.MaxCooldownTime;
                    UsingSpecialSkill = special.MiscUpdate(Player, this);
                    if (!special.AllowsMovement)
                    {
                        //Player.webbed = true;
                        if (Player.mount.Active)
                        {
                            Player.mount.Dismount(Player);
                        }
                        Player.CancelAllBootRunVisualEffects();
                    }
                    SpecialSkillUseTime++;
                }
            } else
            {
                SpecialSkillUseTime = 0;
            }

            // Main Skill logic
            if (UsingMainSkill && CanUseMainSkill())
            {
                // If using the main skill, consume EP, increase MainSkillUseTime, and set the EP cooldown timer
                if (septima.MainSkillUse(Player, this))
                {
                    CurrentEP -= septima.EPUseBase * GetTotalEPUseModifier();
                    EPCooldownTimer = (int)(septima.EPCooldownBaseTimer * GetTotalEPCooldownModifier());
                }
                MainSkillUseTime++;
            }
            else
            {
                // If not using the main skill, set MainSkillUseTime to 0 and decrease EP cooldown 
                MainSkillUseTime = 0;
                if (EPCooldownTimer > 0)
                {
                    EPCooldownTimer--;
                }
            }

            // Secondary Skill logic
            if (UsingSecondarySkill)
            {
                int cooldownRegistered = septima.SecondarySkillUse(Player, this);
                SecondarySkillUseTime++;
                if (cooldownRegistered > 0)
                {
                    UsingSecondarySkill = false;
                    SecondarySkillCooldown = cooldownRegistered;
                }
            }
            else
            {
                SecondarySkillUseTime = 0;
            }

            // Check EP before the recovery and overheat if EP is 0 or less
            if (CurrentEP <= 0 && !Overheated)
            {
                ForceOverheat();
            }

            // EP recovery, depends on the overheat state, but all recovery scales with max EP
            if (Overheated)
            {
                // When overheat, increase with OverheatRecovery stats and check EP after
                CurrentEP += GetTotalMaxEP() * septima.OverheatRecoveryBaseRate * GetTotalOverheatRecoveryModifier();
                if (CurrentEP >= GetTotalMaxEP())
                {
                    Overheated = false;
                    septima.OnOverheatRecovery(Player, this);
                }
            } else
            {
                // When not overheat, only recover when the cooldown timer is at 0 or less
                if (EPCooldownTimer <= 0)
                {
                    CurrentEP += GetTotalMaxEP() * septima.EPRecoveryBaseRate * GetTotalEPRecoveryModifier();
                }
            }

            // AP recovers the same always, unless the player is using a special Skill
            if (!UsingSpecialSkill)
            {
                CurrentAP += septima.APRecoveryBaseRate * GetTotalAPRecoveryModifier() * 10;
            }

            // Clamp EP and AP
            CurrentEP = MathHelper.Clamp(CurrentEP, 0, GetTotalMaxEP());
            CurrentAP = MathHelper.Clamp(CurrentAP, 0, GetTotalMaxAP());
        }

        public override void PostUpdateRunSpeeds()
        {
            septima.MovementEffects(Player, this);

            if (UsingSpecialSkill)
            {
                septima.SkillList[SelectedSkill].MoveUpdate(Player, this);
            }
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            // Note: Activating prevasion also causes tags on the enemy to disappear
            // CCed will bypass all forms of prevasion, for balance with other mods 
            if (Player.CCed) return false;
            return base.FreeDodge(info);
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            //Main.NewText($"Player hit");
            if (septima.NPCDamageResistances.ContainsKey(npc.type))
            {
                switch (septima.NPCDamageResistances[npc.type])
                {
                    case Resistance.Penetrate:
                        break;
                    case Resistance.Overheat:
                        ForceOverheat();
                        break;
                    case Resistance.Ignore:
                        modifiers.Cancel();
                        break;
                    case Resistance.Absorb:
                        CurrentEP += modifiers.FinalDamage.Base / 100;
                        modifiers.Cancel();
                        break;
                }
            }

            if (UsingSpecialSkill)
            {
                //Main.NewText($"Septima modifying the hurt");
                septima.SkillList[SelectedSkill].NPCHitUpdate(Player, this, npc, ref modifiers);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (septima.ProjectileDamageResistances.ContainsKey(proj.type))
            {
                switch (septima.ProjectileDamageResistances[proj.type])
                {
                    case Resistance.Penetrate:
                        break;
                    case Resistance.Overheat:
                        ForceOverheat();
                        break;
                    case Resistance.Ignore:
                        modifiers.Cancel();
                        break;
                    case Resistance.Absorb:
                        CurrentEP += modifiers.FinalDamage.Base / 100;
                        modifiers.Cancel();
                        break;
                }
            }

            if (UsingSpecialSkill)
            {
                septima.SkillList[SelectedSkill].ProjectileHitUpdate(Player, this, proj, ref modifiers);
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
        }

        public override void ResetEffects()
        {
            ModifiedMaxEP = 0;
            ModifiedMaxAP = 0;

            EPUseModifier = 1;
            EPRecoveryModifier = 1;
            EPCooldownModifier = 1;
            APRecoveryModifier = 1;

            septima.UpdateTimers();
            if (SecondarySkillCooldown > 0) SecondarySkillCooldown--;
        }

        /// <summary>
        /// Causes the adept to overheat instantly.
        /// </summary>
        /// <param name="resetBuffs">Forces buffs related to EP duration to be reset.</param>
        /// <param name="ignoreBuffs">Forces overheat even with buffs that give infinite EP.</param>
        /// <returns>False if a buff prevented the forced overheat.</returns>
        public bool ForceOverheat(bool resetBuffs = false, bool ignoreBuffs = false)
        {
            septima.OnOverheat(Player, this);
            // Visual effects
            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(Player.Center, DustID.MartianSaucerSpark);
            }
            Overheated = true;

            return true;
        }

        public int GetTotalMaxEP()
        {
            return BaseMaxEP + ModifiedMaxEP + septima.MaxEPModifier;
        }

        public int GetTotalMaxAP()
        {
            return BaseMaxAP + ModifiedMaxAP;
        }

        public float GetTotalEPUseModifier()
        {
            return EPUseModifier;
        }

        public float GetTotalEPRecoveryModifier()
        {
            return EPRecoveryModifier + septima.EPRecoveryModifier;
        }

        public float GetTotalAPRecoveryModifier()
        {
            return APRecoveryModifier + septima.APRecoveryModifier;
        }

        public float GetTotalEPCooldownModifier()
        {
            return EPCooldownModifier + septima.EPCooldownModifier;
        }

        public float GetTotalOverheatRecoveryModifier()
        {
            return OverheatRecoveryModifier;
        }

        public float GetEPPercent()
        {
            return MathHelper.Clamp(CurrentEP / (GetTotalMaxEP() + 0.0000001f), 0, 1);
        }

        public bool CanUseMainSkill()
        {
            return CurrentEP > 0 && !Overheated && septima.CanUseMainSkill(Player, this) && !UsingSpecialSkill
                && !UsingSecondarySkill && !Player.CCed;
        }

        public bool CanUseSecondarySkill()
        {
            return SecondarySkillCooldown <= 0 && septima.CanUseSecondarySkill(Player, this) && 
                !UsingSpecialSkill && !Player.CCed && !UsingSecondarySkill;
        }
    }
}
