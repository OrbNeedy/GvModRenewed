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
using Terraria.Localization;
using GvMod.Content.Buffs;
using Terraria.Audio;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GvMod.Common.Players
{
    public class SeptimaPlayer : ModPlayer
    {
        // Cheating item check, for testing purposes
        public bool perfectionCheck = false;

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
        public const int InitialMaxEP = 100;
        public int BaseMaxEP { get; set; } = 100;
        // This one is affected by equipment and other modifiers, it's reset later
        /// <summary>
        /// A temporary modification to max EP. Not a multiplier.
        /// </summary>
        public int ModifiedMaxEP { get; set; } = 0;
        public float CurrentEP { get; set; } = 100;
        public int EPCooldownTimer { get; set; } = 0;
        // This is permanent max, it's affected by permanent upgrades and limited by SeptimaUpgrades.
        // Up to two upgrades are planned
        /// <summary>
        /// Permanent max. Use <see cref="ModifiedMaxSP"/> for a temporary upgrade.
        /// </summary>
        public int BaseMaxSP { get; set; } = 2;
        // This one is affected by equipment and other modifiers, it's reset later
        public int ModifiedMaxSP { get; set; } = 0;
        public float CurrentSP { get; set; } = 2;
        public bool PreviousDnizerState { get; set; } = false;
        public bool DnizerMode { get; set; } = false;

        // State related
        public int ChargeguardLevel { get; set; } = 0;
        public int ChargeguardCooldown { get; set; } = 0;
        public int RechargeDelay { get; set; } = 0;
        public int RechargeTimer { get; set; } = 0;
        public bool DoubleTap { get; set; } = false;
        public bool QueueStageCheck { get; set; } = false;
        public bool Overheated { get; set; } = false;
        public bool UsingMainSkill { get; set; } = false;
        public int MainSkillUseTime { get; set; } = 0;
        public bool UsingSecondarySkill { get; set; } = false;
        public int SecondarySkillUseTime { get; set; } = 0;
        public bool UsingSpecialSkill { get; set; } = false;
        public bool SuperState { get; set; } = false;
        public int SpecialSkillUseTime { get; set; } = 0;
        public NPCTags TaggedNPCs = new();
        public List<string> QueuedSkills = new();

        // Stat modifiers
        // Base modifiers, septima and item modifiers get added to this
        /// <summary>
        /// Multiplicative modifier to the recovery rate of EP after the cooldown time is over.
        /// </summary>
        public float EPRecoveryModifier { get; set; } = 1;
        /// <summary>
        /// Multiplicative modifier to the cooldown applied to the EP recovery after the main skill is used.
        /// </summary>
        public float EPCooldownModifier { get; set; } = 1;
        /// <summary>
        /// Multiplicative modifier to the use rate of EP when the main skill is used.
        /// </summary>
        public float EPUseModifier { get; set; } = 1;
        /// <summary>
        /// Multiplicative modifier to the recovery rate of EP when the player is in an <see cref="Overheated"/> state.
        /// </summary>
        public float OverheatRecoveryModifier { get; set; } = 1;
        /// <summary>
        /// Multiplicative modifier to the recovery rate of SP.
        /// </summary>
        public float SPRecoveryModifier { get; set; } = 1;
        // Skills will only have one key to activate it, and another key to select it quickly 
        public float SPSaveChance { get; set; } = 0;
        public int SelectedSkill { get; set; } = 0;
        public int SecondarySkillCooldown { get; set; } = 0;
        // Flags for the dragon veins this player already visited
        public const int MaxDragonVeins = 7;
        public bool[] DragonVeinsVisited { get; set; } = new bool[MaxDragonVeins] { false, false, false, false, false, false, 
            false };

        public override void Initialize()
        {
            if (septima == null)
            {
                SetSeptima(septimaType);
            }

            septima.InitializeSeptima(Player, this, Mod);
        }

        public override void Load()
        {
        }

        public override void SetStaticDefaults()
        {
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            SeptimaPlayer adept = (SeptimaPlayer)targetCopy;

            adept.DoubleTap = DoubleTap;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            SeptimaPlayer adept = (SeptimaPlayer)clientPlayer;

            if (adept.DoubleTap != DoubleTap)
            {
                SyncRecharge(Player.whoAmI, DoubleTap);
            }
        }

        public static void SyncRecharge(int whoAmI, bool recharge)
        {
            ModPacket packet = ModContent.GetInstance<GvMod>().GetPacket();
            packet.Write((byte)MessageType.PrevasionVisualSync); // ID
            packet.Write((byte)whoAmI); // Player
            packet.Write(recharge); // Recharge State
            packet.Send(ignoreClient: whoAmI);
        }

        public static void ReceiveRechargeSync(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            // If server, the target is the player who sent the message
            if (Main.dedServ)
            {
                player = whoAmI;
            }

            bool recharge = reader.ReadBoolean();

            // If the target is not the player who sent the message, change it locally
            if (player != Main.myPlayer)
            {
                SeptimaPlayer adept = Main.player[player].GetModPlayer<SeptimaPlayer>();
                adept.DoubleTap = recharge;
            }

            // If it's the server, send the change to everyone else
            if (Main.dedServ)
            {
                SyncRecharge(player, recharge);
                // Main.player[player].GetModPlayer<ResurrectionPlayer>().SyncRebirth(player, forcedResurrect);
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            SyncState(Player.whoAmI, Overheated, (byte)septimaType);
        }

        public static void SyncState(int whoAmI, bool overheat, byte septimaType)
        {
            ModPacket packet = ModContent.GetInstance<GvMod>().GetPacket();
            packet.Write((byte)MessageType.SeptimaStateSync); // ID
            packet.Write((byte)whoAmI); // Player
            packet.Write(overheat); // Overheat state
            packet.Write(septimaType); // Septima type
            packet.Send(ignoreClient: whoAmI);
        }

        public static void ReceiveStateSync(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            // If server, the target is the player who sent the message
            if (Main.dedServ)
            {
                player = whoAmI;
            }

            bool overheat = reader.ReadBoolean();
            byte septimaType = reader.ReadByte();

            // If the target is not the player who sent the message, change it locally
            if (player != Main.myPlayer)
            {
                SeptimaPlayer adept = Main.player[player].GetModPlayer<SeptimaPlayer>();
                adept.Overheated = overheat;
                adept.SetSeptima((SeptimaType)septimaType);
            }

            // If it's the server, send the change to everyone else
            if (Main.dedServ)
            {
                SyncState(player, overheat, septimaType);
                // Main.player[player].GetModPlayer<ResurrectionPlayer>().SyncRebirth(player, forcedResurrect);
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (UsingSpecialSkill)
            {
                //Main.NewText("Checking");
                if (!septima.AvailableSkills[SelectedSkill].AllowsMovement)
                {
                    // Main.NewText("No movement");
                    Player.controlJump = false;
                    Player.controlDown = false;
                    Player.controlLeft = false;
                    Player.controlRight = false;
                    Player.controlUp = false;
                    Player.controlUseItem = false;
                    Player.controlUseTile = false;
                    Player.controlThrow = false;
                    Player.controlHook = false;
                    Player.controlMount = false;
                    // Player.gravDir = 1f;
                }
            }

            if (KeybindSystem.abilityMenu.JustPressed)
            {
                ModContent.GetInstance<UISystem>().SwitchUIVisibility();
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
                if (septima.AvailableSkills.Count > 0)
                {
                    //ModContent.GetInstance<GvMod>().Logger.Warn($"");

                    SelectedSkill = (int)MathHelper.Clamp(SelectedSkill, 0, septima.AvailableSkills.Count - 1);
                    SpecialSkill special = septima.AvailableSkills[SelectedSkill];

                    if (CanUseSpecialSkill(special))
                    {
                        if (special.InternalName != "Default")
                        {
                            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GlobalSpecialSkillUse") with
                            {
                                PitchVariance = 0.1f,
                                Volume = 0.75f
                            }, Player.Center);
                        }
                        UsingSpecialSkill = special.OnSkillUse(Player, this);
                        if (1 - SPSaveChance >= Main.rand.NextFloat())
                        {
                            CurrentSP -= special.SPCost;
                        }
                        special.CooldownTime = special.MaxCooldownTime;
                    }
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
        }

        public void ChangeSkill(int displacement)
        {
            if (UsingSpecialSkill) return;

            SelectedSkill += (int)MathHelper.Clamp(displacement, -1, 1);

            if (SelectedSkill >= septima.AvailableSkills.Count)
            {
                SelectedSkill = 0;
            }

            if (SelectedSkill < 0)
            {
                SelectedSkill = septima.AvailableSkills.Count - 1;
            }

        }

        public override void OnEnterWorld()
        {
            CurrentEP = GetTotalMaxEP();
            CurrentSP = GetTotalMaxSP();
            EPCooldownTimer = 0;
            SecondarySkillCooldown = 0;
            StageCheck();
            septima.CalculateSkills(Player, this);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["SeptimaType"] = (int)septimaType;
            tag["SeptimaSubType"] = subType;

            tag["Level"] = Level;
            tag["Stage"] = Stage;
            tag["MaxEP"] = BaseMaxEP;
            tag["MaxSP"] = BaseMaxSP;

            for (int i = 0; i < DragonVeinsVisited.Length; i++)
            {
                tag[$"DragonVein{i}"] = DragonVeinsVisited[i];
            }

            tag["SelectedSkill"] = SelectedSkill;

            septima.PreSaveTag();

            foreach (KeyValuePair<string, int> pair in septima.SaveTags)
            {
                // Key looks like "AzureStriker:Kudos"
                tag[septimaType.ToString() + "." + pair.Key] = pair.Value;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("SeptimaType"))
            {
                septimaType = (SeptimaType)tag.GetInt("SeptimaType");
                if (septima.Type != septimaType)
                {
                    SetSeptima(septimaType);
                }
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
            if (tag.ContainsKey("MaxSP"))
            {
                BaseMaxSP = tag.GetInt("MaxSP");
            }

            DragonVeinsVisited = new bool[MaxDragonVeins];
            for (int i = 0; i < DragonVeinsVisited.Length; i++)
            {
                if (tag.ContainsKey($"DragonVein{i}"))
                {
                    DragonVeinsVisited[i] = tag.GetBool($"DragonVein{i}");
                } else
                {
                    DragonVeinsVisited[i] = false;
                }
            }

            if (tag.ContainsKey("SelectedSkill"))
            {
                SelectedSkill = (int)MathHelper.
                    Clamp(tag.GetInt("SelectedSkill"), 0, septima.AvailableSkills.Count - 1);
            }

            septima.PostLoadSeptima(Player, this);

            foreach (string key in septima.SaveTags.Keys)
            {
                string compoundedKey = septimaType.ToString() + "." + key;
                if (tag.ContainsKey(compoundedKey))
                {
                    septima.SaveTags[key] = tag.GetInt(compoundedKey);
                }
            }

            septima.PostTagLoad();
        }

        public override void PreUpdateMovement()
        {
            septima.DirectMovementEffects(Player, this);
        }

        public override void PreUpdateBuffs()
        {
            septima.MiscEffects(Player, this);

            if (UsingSpecialSkill)
            {
                septima.AvailableSkills[SelectedSkill].StatUpdate(Player, this);
            }
            base.PreUpdateBuffs();
        }

        public override void PostUpdateBuffs()
        {
            if (Player.CCed)
            {
                ForceOverheat();
            }
        }

        public override void PreUpdate()
        {
            //Main.NewText("Speed length: " + Player.velocity.Length());
            TaggedNPCs.Update(this);

            //Main.NewText("Dragon vein state: " + DragonVeinsVisited.Count(true) + " visited");

            /*int count = 0;
            foreach (SpecialSkill skill in septima.AvailableSkills)
            {
                Main.NewText($"Skill {count}: {skill.InternalName}");
                count++;
            }*/

            if (Main.rand.NextBool(1200))
            {
                // Main.NewText("Random skill calculation triggered.", Color.Red);
                septima.CalculateSkills(Player, this);
            }

            // Dead men have no septima
            if (Player.DeadOrGhost) return;

            if (QueueStageCheck)
            {
                // Main.NewText("Checking after boss death");
                StageCheck();
                QueueStageCheck = false;
            }

            SpecialSkillLogic();

            // Main Skill logic
            MainSkillLogic();

            // Secondary Skill logic
            SecondarySkillLogic();

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
                //Main.NewText("Recovery rate: " + GetTotalOverheatRecoveryModifier());
                //Main.NewText("Max EP: " + GetTotalMaxEP());
                //Main.NewText("Base recovery: " + septima.OverheatRecoveryBaseRate);
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

            // SP recovers the same always, unless the player is using a special Skill
            if (!UsingSpecialSkill)
            {
                CurrentSP += septima.SPRecoveryBaseRate * GetTotalSPRecoveryModifier();
            }

            RechargeLogic();

            // Clamp EP and SP
            CurrentEP = MathHelper.Clamp(CurrentEP, 0, GetTotalMaxEP());
            CurrentSP = MathHelper.Clamp(CurrentSP, 0, GetTotalMaxSP());
            if (perfectionCheck)
            {
                CurrentEP = GetTotalMaxEP();
                CurrentSP = GetTotalMaxSP();
                Overheated = false;
            }
        }

        private void SpecialSkillLogic()
        {
            if (UsingSpecialSkill)
            {
                SpecialSkill special = septima.AvailableSkills[SelectedSkill];
                if (Player.CCed && !special.Invincible)
                {
                    special.ForcedSkillEnd(Player, this);
                    UsingSpecialSkill = false;
                }
                else
                {
                    special.CooldownTime = special.MaxCooldownTime;
                    UsingSpecialSkill = special.MiscUpdate(Player, this);
                    if (!special.AllowsMovement)
                    {
                        //Player.webbed = true;
                        Player.CancelAllBootRunVisualEffects();
                    }
                    SpecialSkillUseTime++;

                    if (special.Invincible)
                    {
                        Player.immune = true;
                        Player.immuneTime = 2;
                    }
                }
            }
            else
            {
                SpecialSkillUseTime = 0;
            }
        }

        private void MainSkillLogic()
        {
            if (UsingMainSkill && CanUseMainSkill())
            {
                // If using the main skill, consume EP, increase MainSkillUseTime, and set the EP cooldown timer
                if (septima.MainSkillUse(Player, this))
                {
                    ConsumeEP(septima.EPUseBase);
                }

                for (int i = 0; i < TaggedNPCs.targetCount; i++)
                {
                    Tag currentTag = TaggedNPCs.GetTagByIndex(i);
                    int finalDamage = septima.TagEffect(Player, this, i, ref TaggedNPCs);
                    TryTriggerTagLifesteal(finalDamage);
                }
                
                MainSkillUseTime++;

                if (septima.CanChargeWhileAttacking && EPCooldownTimer > 0)
                {
                    EPCooldownTimer--;
                }
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
        }

        /// <summary>
        /// Consumes the passed EP applying cooldown and use modifiers.
        /// </summary>
        /// <param name="amount"></param>
        public void ConsumeEP(float amount)
        {
            CurrentEP -= amount * GetTotalEPUseModifier();
            EPCooldownTimer = (int)(septima.EPCooldownBaseTimer * GetTotalEPCooldownModifier());
        }

        public void ConsumeEP(float amount, float cooldown)
        {
            CurrentEP -= amount * GetTotalEPUseModifier();
            EPCooldownTimer = (int)(cooldown * GetTotalEPCooldownModifier());
        }

        public bool CanConsumeEP(float amount)
        {
            return CurrentEP >= (amount * GetTotalEPUseModifier()) && !Overheated && !Player.CCed;
        }

        public void TryTriggerTagLifesteal(int damage)
        {
            if (damage <= 0) return;

            if (Player.GetModPlayer<PlayerBuffs>().LifeLoupe && Player.lifeSteal > 0)
            {
                int lifeDrain = (int)(damage * 0.05f);
                if (lifeDrain <= 0) lifeDrain = 1;
                Player.Heal(lifeDrain);

                Player.lifeSteal -= lifeDrain;
            }
        }

        private void SecondarySkillLogic()
        {
            if (UsingSecondarySkill)
            {
                int cooldownRegistered = septima.SecondarySkillUse(Player, this);
                SecondarySkillUseTime++;
                UsingSecondarySkill = true;
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
        }

        private void RechargeLogic()
        {
            if (Main.myPlayer != Player.whoAmI) return;

            int maxRechargeTimer = 30;
            int maxRechargeDelay = 50;

            // Trigger
            if (RechargeDelay <= 0 && DoubleTap && !UsingMainSkill && !UsingSecondarySkill && 
                !UsingSpecialSkill && !Overheated && septima.AllowRecharge && 
                Player.GetModPlayer<PlayerPrevasion>().PrevasionIframes <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/Recharge") with
                {
                    PitchVariance = 0.1f, 
                    Volume = 0.65f
                }, Player.Center);

                RechargeDelay = maxRechargeDelay;
                RechargeTimer = maxRechargeTimer;
            }

            if (RechargeDelay > 0) RechargeDelay--;
            if (RechargeTimer > 0)
            {
                if (ChargeguardCooldown <= 0 && RechargeTimer == maxRechargeTimer)
                {
                    Player.immune = true;
                    int iframes = (int)(maxRechargeTimer * ChargeguardLevel * 0.5f);
                    //Main.NewText("[Title Card] of " + iframes);
                    Player.SetImmuneTimeForAllTypes(iframes);
                    /*Player.AddImmuneTime(ImmunityCooldownID.General, iframes);
                    Player.AddImmuneTime(ImmunityCooldownID.Bosses, iframes);
                    Player.AddImmuneTime(ImmunityCooldownID.TileContactDamage, iframes);
                    Player.AddImmuneTime(ImmunityCooldownID.Lava, iframes);
                    Player.AddImmuneTime(ImmunityCooldownID.WrongBugNet, iframes);
                    Player.AddImmuneTime(ImmunityCooldownID.DD2OgreKnockback, iframes);*/
                    ChargeguardCooldown = 300;
                }

                int limit = Main.rand.Next(5, 10);
                for (int i = 0; i < limit; i++)
                {
                    Dust.NewDust(Player.position, Player.width, Player.height, DustID.ShimmerSpark, newColor: septima.MainColor);
                }

                if (Player.GetModPlayer<PlayerPrevasion>().PrevasionIframes <= 0)
                {
                    CurrentEP += (float)GetTotalMaxEP() / maxRechargeTimer;
                    EPCooldownTimer = septima.PrevasionEPCooldownBaseTimer;
                }

                RechargeTimer--;

                if (RechargeTimer <= 0 && Player.GetModPlayer<PlayerPrevasion>().PrevasionIframes <= 0)
                {
                    EPCooldownTimer = 1;
                }
            }

            if (ChargeguardCooldown > 0) ChargeguardCooldown--;
        }

        public override void PostUpdateRunSpeeds()
        {
            septima.MovementEffects(Player, this);

            if (UsingSpecialSkill)
            {
                septima.AvailableSkills[SelectedSkill].MoveUpdate(Player, this);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            septima.ModifyHitNPC(Player, this, target, ref modifiers);
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            septima.ModifyNPCHurt(Player, this, npc, ref modifiers);

            //Main.NewText($"Player hit");
            if (SeptimaTemplates.NPCDamageResistances[septima.Type].ContainsKey(npc.type))
            {
                //Main.NewText("Resistance: " + SeptimaTemplates.NPCDamageResistances[septima.Type][npc.type]);
                switch (SeptimaTemplates.NPCDamageResistances[septima.Type][npc.type])
                {
                    case Resistance.Penetrate:
                        break;
                    case Resistance.Overheat:
                        ForceOverheat(ignoreBuffs: true);
                        break;
                    case Resistance.Ignore:
                        modifiers.Cancel();
                        break;
                    case Resistance.Absorb:
                        CurrentEP += modifiers.FinalDamage.Base / 100f;
                        modifiers.Cancel();
                        break;
                }
            }

            if (UsingSpecialSkill)
            {
                //Main.NewText($"Septima modifying the hurt");
                septima.AvailableSkills[SelectedSkill].NPCHitUpdate(Player, this, npc, ref modifiers);
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            septima.ModifyHurt(Player, this, ref modifiers);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            septima.ModifyProjectileHurt(Player, this, proj, ref modifiers);

            if (SeptimaTemplates.NPCDamageResistances[septima.Type].ContainsKey(proj.type))
            {
                switch (SeptimaTemplates.NPCDamageResistances[septima.Type][proj.type])
                {
                    case Resistance.Penetrate:
                        break;
                    case Resistance.Overheat:
                        ForceOverheat(ignoreBuffs: true);
                        break;
                    case Resistance.Ignore:
                        modifiers.Cancel();
                        break;
                    case Resistance.Absorb:
                        CurrentEP += modifiers.FinalDamage.Base / 100f;
                        modifiers.Cancel();
                        break;
                }
            }

            if (UsingSpecialSkill)
            {
                septima.AvailableSkills[SelectedSkill].ProjectileHitUpdate(Player, this, proj, ref modifiers);
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            septima.OnHurt(Player, this, info);

            if (UsingSpecialSkill)
            {
                septima.AvailableSkills[SelectedSkill].HurtUpdate(Player, this, info);
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            septima.OnHurtByNPC(Player, this, npc, hurtInfo);

            if (UsingSpecialSkill)
            {
                septima.AvailableSkills[SelectedSkill].NPCHurtUpdate(Player, this, npc, hurtInfo);
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            septima.OnHurtByProjectile(Player, this, proj, hurtInfo);

            if (UsingSpecialSkill)
            {
                septima.AvailableSkills[SelectedSkill].ProjectileHurtUpdate(Player, this, proj, hurtInfo);
            }
        }

        public override void OnRespawn()
        {
            CurrentEP = GetTotalMaxEP();
            CurrentSP = GetTotalMaxSP();
            SecondarySkillCooldown = 0;

            septima.ForceCooldownEnd();
            base.OnRespawn();
        }

        public override void UpdateDead()
        {
            perfectionCheck = false;
            UsingMainSkill = false;
            UsingSecondarySkill = false;
            UsingSpecialSkill = false;

            EPUseModifier = 0;
            EPRecoveryModifier = 0;
            EPCooldownModifier = 0;
            SPRecoveryModifier = 0;
            base.UpdateDead();
        }

        public override void ResetEffects()
        {
            SuperState = septima.GetSuperState(Player, this);
            septima.ResetEffects(Player, this);

            ModifiedMaxEP = 0;
            ModifiedMaxSP = 0;

            EPUseModifier = 1;
            EPRecoveryModifier = 1;
            EPCooldownModifier = 1;
            SPRecoveryModifier = 1;
            SPSaveChance = 0;
            OverheatRecoveryModifier = 1;
            PreviousDnizerState = DnizerMode;
            DnizerMode = false;

            septima.UpdateTimers(perfectionCheck);
            if (SecondarySkillCooldown > 0) SecondarySkillCooldown--;

            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[0] < 15)
            {
                DoubleTap = true;
            }
            else
            {
                DoubleTap = false;
            }

            ChargeguardLevel = 0;

            perfectionCheck = false;
        }

        /// <summary>
        /// Run every frame when the player is inside of a dragon vein point in the world. <br/>
        /// The flag in this septima player has not been updated yet.
        /// </summary>
        /// <param name="index">The index of the vein being visited. <br/>
        /// It's a different location for each world, but the same player will still keep the flags from other 
        /// worlds.</param>
        /// <param name="distance">The distance in tile coordinates from the player to the vein.</param>
        public void UpdateInsideDragonVein(int index, float distance)
        {
            septima.DuringVeinVisit(Player, this, index, distance);

            if (distance <= 64 && Player.HasBuff<Anthem>())
            {
                //Main.NewText("Conditions");
                // Very rarely, when the player is near a dragon vein and is in an Anthem state, increase
                // it's level up to 1000
                if (Main.rand.NextBool(18000))
                {
                    // I hope I don't regret this choice
                    if (UpgradeLevel(0, 1000))
                    {
                        int randomMessage = Main.rand.Next(0, 7);
                        Main.NewText(Language.
                            GetTextValue($"Mods.GvMod.LevelUpMessage.DragonVein{randomMessage.ToString()}"), 
                            septima.MainColor);
                    }
                }
            }
        }

        /// <summary>
        /// Increases the level of the adept and checks if the stage can increase too.
        /// </summary>
        /// <param name="minLevel">Minimum level the player needs to be able to increase it via this method (Exclusive).</param>
        /// <param name="maxLevel">Maximum level the player can get through this method (Inclusive).</param>
        /// <returns>True if the upgrade was successful, false if it wasn't.</returns>
        public bool UpgradeLevel(int minLevel, int maxLevel)
        {
            if (Level >= maxLevel || Level < minLevel) return false;

            Level++;

            StageCheck();
            septima.CalculateSkills(Player, this, true);

            return true;
        }

        /// <summary>
        /// Increases the level of the adept and checks if the stage can increase too.
        /// </summary>
        /// <param name="minLevel">Minimum level the player needs to be able to increase it via this method (Exclusive).</param>
        /// <returns>True if the upgrade was successful, false if it wasn't.</returns>
        public bool UpgradeLevel(int minLevel)
        {
            if (Level < minLevel) return false;

            Level++;

            StageCheck();
            septima.CalculateSkills(Player, this, true);

            return true;
        }

        /// <summary>
        /// Checks the state of the player and the world to determine if the stage can increase or not. <br/>
        /// Runs after a boss is defeated, the player levels up, or enters the world.
        /// </summary>
        public void StageCheck()
        {
            bool stageChanged = false;
            int checks = 0;
            // I pray to god this never makes an infinite loop
            do
            {
                checks++;
                stageChanged = false;
                switch (Stage)
                {
                    case 1:
                        if (Level >= 10 && (NPC.downedBoss1 || NPC.downedSlimeKing))
                        {
                            stageChanged = true;
                            if (BaseMaxEP < 200) BaseMaxEP += 10; // Expected: 110
                            Stage++;
                            septima.OnStageChange(Player, this);
                        }
                        break;
                    case 2:
                        if (Level >= 25 && NPC.downedBoss3)
                        {
                            stageChanged = true;
                            if (BaseMaxEP < 200) BaseMaxEP += 10; // Expected: 120
                            Stage++;
                            septima.OnStageChange(Player, this);
                        }
                        break;
                    case 3:
                        if (Level >= 30 && Main.hardMode)
                        {
                            stageChanged = true;
                            if (BaseMaxSP == 2) BaseMaxSP += 1;
                            if (BaseMaxEP < 200) BaseMaxEP += 20; // Expected: 140
                            Stage++;
                            septima.OnStageChange(Player, this);
                        }
                        break;
                    case 4:
                        if (Level >= 40 && (NPC.downedMechBossAny || NPC.downedQueenSlime))
                        {
                            stageChanged = true;
                            if (BaseMaxEP < 200) BaseMaxEP += 10; // Expected: 150
                            Stage++;
                            septima.OnStageChange(Player, this);
                        }
                        break;
                    case 5:
                        if (Level >= 50 && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                        {
                            stageChanged = true;
                            if (BaseMaxEP < 200) BaseMaxEP += 10; // Expected: 160
                            Stage++;
                            septima.OnStageChange(Player, this);
                        }
                        break;
                    case 6:
                        if (Level >= 60 && NPC.downedGolemBoss)
                        {
                            stageChanged = true;
                            if (BaseMaxEP < 200) BaseMaxEP += 10; // Expected: 170
                            Stage++;
                            septima.OnStageChange(Player, this);
                        }
                        break;
                    case 7:
                        if (Level >= 65 && NPC.downedAncientCultist)
                        {
                            stageChanged = true;
                            if (BaseMaxEP < 200) BaseMaxEP += 10; // Expected: 180
                            Stage++;
                            septima.OnStageChange(Player, this);
                        }
                        break;
                    case 8:
                        if (Level >= 75 && DragonVeinsVisited.Count(true) >= DragonVeinsVisited.Length)
                        {
                            stageChanged = true;
                            if (BaseMaxEP < 200) BaseMaxEP += 20; // Expected: 200
                            Stage++;
                            septima.OnStageChange(Player, this);
                        }
                        break;
                    case 9:
                        if (Level >= 90 && NPC.downedMoonlord)
                        {
                            stageChanged = true;
                            if (BaseMaxSP == 3) BaseMaxSP += 1;
                            if (BaseMaxEP < 300) BaseMaxEP += 100; // Expected: 300
                            Stage++;
                            septima.OnStageChange(Player, this);
                        }
                        break;
                }
            } while (stageChanged);

            if (stageChanged || checks > 1)
            {
                Main.NewText(Language.GetTextValue($"Mods.GvMod.LevelUpMessage.Regular"), septima.MainColor);
            }
            //Main.NewText($"Final checks: {checks}");
        }

        /// <summary>
        /// Causes the adept to overheat instantly.
        /// </summary>
        /// <param name="resetBuffs">Forces buffs related to EP duration to be reset.</param>
        /// <param name="ignoreBuffs">Forces overheat even with buffs that give infinite EP.</param>
        /// <returns>False if a buff prevented the forced overheat. Also returns true if the player is already overheated.</returns>
        public bool ForceOverheat(bool resetBuffs = false, bool ignoreBuffs = false)
        {
            //Main.NewText("Forcing overheat");
            if (Overheated) return true;

            bool returnValue = true;
            if (Player.HasBuff<InfiniteSurgeBuff>() || Player.HasBuff<DnizerBuff>())
            {
                //Main.NewText("Player has buffs");
                if (resetBuffs)
                {
                    //Main.NewText("Clearing buffs");
                    Player.ClearBuff(ModContent.BuffType<InfiniteSurgeBuff>());
                    Player.ClearBuff(ModContent.BuffType<DnizerBuff>());
                }
                if (ignoreBuffs)
                {
                    //Main.NewText("Buffs ignored");
                } else
                {
                    //Main.NewText("Buffs not ignored");
                    return false;
                }
            }

            Overheated = true;
            CurrentEP = 0;

            septima.OnOverheat(Player, this);

            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(Player.Center, DustID.ShimmerSpark, newColor: septima.MainColor);
            }

            //Main.NewText("Successfully overheated");
            //Main.NewText("Overheated: " + Overheated);
            //Main.NewText("CurrentEP: " + CurrentEP);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                SyncPlayer(-1, Main.myPlayer, false);
            }

            return returnValue;
        }

        public int GetTotalMaxEP()
        {
            return BaseMaxEP + ModifiedMaxEP + septima.MaxEPModifier;
        }

        public int GetTotalMaxSP()
        {
            return BaseMaxSP + ModifiedMaxSP;
        }

        public float GetTotalEPUseModifier()
        {
            float returnValue = EPUseModifier;
            if (returnValue < 0) returnValue = 0;
            return returnValue;
        }

        public float GetTotalEPRecoveryModifier()
        {
            if (Player.CCed) return 0;

            float returnValue = EPRecoveryModifier + septima.EPRecoveryModifier;
            if (returnValue < 0.01f) returnValue = 0.01f;
            return returnValue;
        }

        public float GetTotalSPRecoveryModifier()
        {
            float returnValue = SPRecoveryModifier + septima.SPRecoveryModifier;
            if (returnValue < 0.01f) returnValue = 0.01f;
            return returnValue;
        }

        public float GetTotalEPCooldownModifier()
        {
            float returnValue = EPCooldownModifier + septima.EPCooldownModifier;
            if (returnValue < 0.01f) returnValue = 0.01f;
            return returnValue;
        }

        public float GetTotalOverheatRecoveryModifier()
        {
            if (Player.CCed) return 0;

            float returnValue = OverheatRecoveryModifier;
            if (returnValue < 0.01f) returnValue = 0.01f;
            return returnValue;
        }

        public float GetEPPercent()
        {
            return MathHelper.Clamp(CurrentEP / (GetTotalMaxEP() + 0.0000001f), 0, 1);
        }

        public bool CanUseMainSkill()
        {
            bool epDiscriminator = (CurrentEP > 0 && !Overheated && septima.CanUseMainSkill(Player, this)) || 
                septima.CanUseMainSkillNoEP(Player, this);
            return epDiscriminator && !UsingSpecialSkill && !UsingSecondarySkill && !Player.CCed && 
                RechargeTimer <= 0;
        }

        public bool CanUseSecondarySkill()
        {
            return SecondarySkillCooldown <= 0 && septima.CanUseSecondarySkill(Player, this) && 
                !UsingSpecialSkill && !Player.CCed && !UsingSecondarySkill && RechargeTimer <= 0;
        }

        public bool CanUseSpecialSkill(SpecialSkill special)
        {
            return special.CanUse(Player, this) && CurrentSP >= special.SPCost && !UsingSpecialSkill &&
                        !UsingSecondarySkill && !Player.CCed && special.CooldownTime <= 0 && RechargeTimer <= 0;
        }

        public void SetSeptima(Septima septima)
        {
            if (septima.Type == this.septima?.Type) return;

            this.septima = septima;
            this.septima.InitializeSeptima(Player, this, Mod);
            septimaType = this.septima.Type;
        }

        public void SetSeptima(SeptimaType septima)
        {
            SetSeptima(SeptimaTemplates.GetNewSeptima(septimaType));
        }
    }
}
