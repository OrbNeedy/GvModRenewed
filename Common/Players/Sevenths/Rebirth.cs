using GvMod.Common.Players.Skills;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using GvMod.Content;
using GvMod.Common.Utils;
using GvMod.Content.Items.Ammo;
using Terraria.Audio;
using GvMod.Content.Buffs;
using Terraria.DataStructures;
using GvMod.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using System;
using GvMod.Content.Projectiles.RebirthBosses;
using MonoMod.Cil;
using GvMod.Content.Projectiles.Hooks;
using System.Linq;
using GvMod.Content.Items.Corpses;
using GvMod.Content.Projectiles.RebirthSummons;

namespace GvMod.Common.Players.Sevenths
{
    public record BossMinionStats
    {
        public int projectileID = 0;
        public int baseDamage = 0;
        public float baseKnockback = 0;
        public int levelRequirement = 0;
        public int stageRequirement = 0;
        public float ai0 = 0;
        public float ai1 = 0;
        public float ai2 = 0;

        public BossMinionStats(int id, int damage, float knockback, float ai0 = 0, 
            float ai1 = 0, float ai2 = 0)
        {
            projectileID = id;
            baseDamage = damage;
            baseKnockback = knockback;
            this.ai0 = ai0;
            this.ai1 = ai1;
            this.ai2 = ai2;
        }

        public BossMinionStats(int id, int damage, int knockback, int level, int stage, 
            float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            projectileID = id;
            baseDamage = damage;
            baseKnockback = knockback;
            levelRequirement = level;
            stageRequirement = stage;
            this.ai0 = ai0;
            this.ai1 = ai1;
            this.ai2 = ai2;
        }
    }

    public class Rebirth : Septima
    {
        // Septima uniques
        private int MainAttackTimer { get; set; } = 0;
        private int SuperVisualTimer = 0;
        public int LastBossKilled { get; private set; } = -1;
        public bool IsInPulleyOrGrappling { get; private set; } = false;
        public int BodyDamageProjectileId { get; set; } = -1;
        public bool BodyDamageProjectileActive = false;
        public static float MinGrappleSpeed = 24f;
        public static int MaxDynamicDamageResistance = 60;
        public static Dictionary<KunaiSummon, SummonStats> ProjectileTable = new();
        public static Dictionary<int, KunaiSummon> CorpseItemTable = new();
        public static Dictionary<int, int[]> SpawnMap = new();

        public static Dictionary<int, BossMinionStats> BossDefeatTable { get; private set; } = new();
        // Tags
        public override Dictionary<string, int> SaveTags { get; set; } = new() {
            ["LastKilledBoss"] = -1
        };

        // Base values
        public override float BaseBasicAttackDamage { get; protected set; } = 5;
        public override float BaseSecondaryAttackDamage { get; protected set; } = 1;
        public override List<SpecialSkill> SkillList { get; protected set; } = new() { new SpecialSkill(), 
            new GalvanicPatch().SetLevel(0).SetLocalization("AnimaReturn"), new Resurrection(), 
            new SeptimalSurge().SetStage(7).SetLevel(62), new SoulCleanse(), new SoulSiphon(), 
            new GalvanicRenewal().SetStage(3).SetLevel(30).SetLocalization("ReAnimate"), 
            new OffensiveResurrection(), new InfiniteSurge(), new UnlimitedAnimus().SetUnlockConditions(
                CustomConditions.FirstDragonVein), new SeptimalShield(), new Gorgoneia(), 
            new EternityKunai().SetUnlockConditions(CustomConditions.FourthDragonVein)
        };
        public override List<SpecialSkill> AvailableSkills { get; protected set; } = new();
        public override float EPUseBase { get; protected set; } = 16;
        public override float EPRecoveryBaseRate { get; protected set; } = 1f / 210f;
        public override int EPCooldownBaseTimer { get; protected set; } = 150;
        public override int PrevasionEPCooldownBaseTimer { get; protected set; } = 90;
        public override float OverheatRecoveryBaseRate { get; protected set; } = 1f / 600f;
        public override float SPRecoveryBaseRate { get; protected set; } = 1f / 5400f;
        public override bool CanChargeWhileAttacking { get; protected set; } = true;
        public override bool AllowRecharge { get; protected set; } = false;
        public override bool AllowPrevasion { get; protected set; } = false;

        // Identifiers
        public override SeptimaType Type { get; protected set; } = SeptimaType.Rebirth;
        public override string InternalName { get; protected set; } = "Rebirth";
        public override Color MainColor { get; protected set; } = new Color(160, 51, 208);
        // Two separate colors so any septima can have distinct overheat and normal EP bar colors
        // This was basically made for septimas with red colors in their design
        public override Color OverheatColor { get; protected set; } = Color.DarkRed;

        // Modifiers
        public override int MaxEPModifier { get; set; } = 50;

        public override void LoadSeptima(Mod mod)
        {
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_Head_Rebirth", 
                EquipType.Head, name: "RebirthArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_Body_Rebirth",
                EquipType.Body, name: "RebirthArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_Legs_Rebirth",
                EquipType.Legs, name: "RebirthArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_Back_Rebirth",
                EquipType.Back, name: "RebirthArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_HandsOff_Rebirth",
                EquipType.HandsOff, name: "RebirthArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_HandsOn_Rebirth",
                EquipType.HandsOn, name: "RebirthArmedPhenomenon");
            base.LoadSeptima(mod);
        }

        public override void PostLoadSeptima(Mod mod)
        {
            //IL_Player.QuickGrapple += RebirthGrappleIL;
            On_Player.QuickGrapple_GetItemToUse += RebirthGrappleDetour;
            IL_Player.Update += RopeSpeedUpgrade;

            BossMinionStats kingSlime = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 50, 1);
            BossMinionStats eoc = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 25, 2);
            BossMinionStats eow = new BossMinionStats(ModContent.ProjectileType<EaterofWorlds>(), 50, 3.5f, ai1: -1, ai2: -1);
            BossMinionStats boc = new BossMinionStats(ModContent.ProjectileType<BrainOfCthulhu>(), 22, 1);
            BossMinionStats skeletron = new BossMinionStats(ModContent.ProjectileType<Skeletron>(), 15, 3);
            BossMinionStats deerclops = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 50, 1);
            BossMinionStats queenBee = new BossMinionStats(ModContent.ProjectileType<QueenBee>(), 18, 1);
            BossMinionStats wof = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 50, 1);
            BossMinionStats queenSlime = new BossMinionStats(ModContent.ProjectileType<QueenSlime>(), 35, 2.5f);
            BossMinionStats skeletronPrime = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 50, 1);
            BossMinionStats twins = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 50, 1);
            BossMinionStats destroyer = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 50, 1);
            BossMinionStats plantera = new BossMinionStats(ModContent.ProjectileType<Plantera>(), 30, 1);
            BossMinionStats duke = new BossMinionStats(ModContent.ProjectileType<DukeFishron>(), 20, 3);
            BossMinionStats golem = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 50, 1);
            BossMinionStats cultist = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 50, 1);
            BossMinionStats empress = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 50, 1);
            BossMinionStats moonlord = new BossMinionStats(ModContent.ProjectileType<EyeOfCthulhu>(), 50, 1);

            BossDefeatTable = new()
            {
                //[NPCID.KingSlime] = kingSlime,
                [NPCID.EyeofCthulhu] = eoc,
                [NPCID.EaterofWorldsHead] = eow,
                [NPCID.EaterofWorldsBody] = eow,
                [NPCID.EaterofWorldsTail] = eow,
                [NPCID.BrainofCthulhu] = boc,
                [NPCID.SkeletronHead] = skeletron,
                //[NPCID.Deerclops] = deerclops,
                [NPCID.QueenBee] = queenBee,
                //[NPCID.WallofFlesh] = wof,
                //[NPCID.WallofFleshEye] = wof,
                [NPCID.QueenSlimeBoss] = queenSlime,
                [NPCID.Plantera] = plantera,
                [NPCID.DukeFishron] = duke,
                //[NPCID.CultistBoss] = cultist,
                //[NPCID.HallowBoss] = empress,
                //[NPCID.MoonLordCore] = moonlord
            };

            int headID = EquipLoader.GetEquipSlot(mod, "RebirthArmedPhenomenon", EquipType.Head);
            //ArmorIDs.Head.Sets. // Find an option that draws only front hair
            SetTables();
        }

        public static void RegisterSummonStats(KunaiSummon key, SummonStats stats)
        {
            if (ProjectileTable == null)
            {
                ProjectileTable = new();
            } 

            ProjectileTable[key] = stats;
        }

        public void SetTables()
        {
            CorpseItemTable = new()
            {
                [ModContent.ItemType<BatCorpse>()] = KunaiSummon.Bat,
                [ModContent.ItemType<BeeCorpse>()] = KunaiSummon.Bee,
                [ModContent.ItemType<BigBatCorpse>()] = KunaiSummon.BigBat,
                [ModContent.ItemType<CrimeraCorpse>()] = KunaiSummon.Crimera,
                [ModContent.ItemType<EaterOfSoulsCorpse>()] = KunaiSummon.EaterOfSouls,
                [ModContent.ItemType<FlyingSnakeCorpse>()] = KunaiSummon.FlyingSnake,
                [ModContent.ItemType<HellBatCorpse>()] = KunaiSummon.HellBat,
                [ModContent.ItemType<HornetCorpse>()] = KunaiSummon.Hornet,
                [ModContent.ItemType<IceBatCorpse>()] = KunaiSummon.IceBat,
                [ModContent.ItemType<PigronCorpse>()] = KunaiSummon.Pigron,
                [ModContent.ItemType<PixieCorpse>()] = KunaiSummon.Pixie,
                [ModContent.ItemType<ScorpionCorpse>()] = KunaiSummon.Scorpion,
                [ModContent.ItemType<SlimeCorpse>()] = KunaiSummon.Slime,
                [ModContent.ItemType<FlyingEyeCorpse>()] = KunaiSummon.FlyingEye,
                [ModContent.ItemType<WanderingEyeCorpse>()] = KunaiSummon.WanderingEye
            };

            ProjectileTable = new()
            {
                [KunaiSummon.None] = new SummonStats(-1, -1, -1, []),
                [KunaiSummon.Bat] = new SummonStats(
                    ModContent.ProjectileType<ZombieBat>(), 2, 2, []),
                [KunaiSummon.Bee] = new SummonStats(
                    ModContent.ProjectileType<ZombieBee>(), 1, 0.5f, [Condition.DownedQueenBee]),
                [KunaiSummon.BigBat] = new SummonStats(
                    ModContent.ProjectileType<ZombieBigBat>(), 6, 2.5f, []),
                [KunaiSummon.Crimera] = new SummonStats(
                    ModContent.ProjectileType<ZombieCrimera>(), 10, 3f, []),
                [KunaiSummon.EaterOfSouls] = new SummonStats(
                    ModContent.ProjectileType<ZombieEaterOfSouls>(), 10, 3f, []),
                [KunaiSummon.FlyingSnake] = new SummonStats(
                    ModContent.ProjectileType<ZombieFlyingSnake>(), 20, 3f, []),
                [KunaiSummon.HellBat] = new SummonStats(
                    ModContent.ProjectileType<ZombieHellBat>(), 5, 2f, []),
                [KunaiSummon.Hornet] = new SummonStats(
                    ModContent.ProjectileType<ZombieHornet>(), 4, 2f, []),
                [KunaiSummon.IceBat] = new SummonStats(
                    ModContent.ProjectileType<ZombieIceBat>(), 5, 2f, []),
                [KunaiSummon.Pigron] = new SummonStats(
                    ModContent.ProjectileType<ZombiePigron>(), 14, 3.5f, []),
                [KunaiSummon.Pixie] = new SummonStats(
                    ModContent.ProjectileType<ZombiePixie>(), 12, 1.5f, []),
                [KunaiSummon.Scorpion] = new SummonStats(
                    ModContent.ProjectileType<ZombieScorpion>(), 8, 2.5f, []),
                [KunaiSummon.Slime] = new SummonStats(
                    ModContent.ProjectileType<ZombieSlime>(), 3, 3f, []),
                [KunaiSummon.FlyingEye] = new SummonStats(
                    ModContent.ProjectileType<ZombieFlyingEye>(), 2, 1f, []),
                [KunaiSummon.WanderingEye] = new SummonStats(
                    ModContent.ProjectileType<ZombieWanderingEye>(), 5, 3f, [])
            };

            SpawnMap = new()
            {
                [ModContent.ItemType<BatCorpse>()] = [NPCID.CaveBat],
                [ModContent.ItemType<BeeCorpse>()] = [NPCID.Bee, NPCID.BeeSmall],
                [ModContent.ItemType<BigBatCorpse>()] = [NPCID.GiantBat],
                [ModContent.ItemType<CrimeraCorpse>()] = [NPCID.Crimera, NPCID.BigCrimera, NPCID.LittleCrimera],
                [ModContent.ItemType<EaterOfSoulsCorpse>()] = [NPCID.EaterofSouls, NPCID.BigEater, NPCID.LittleEater],
                [ModContent.ItemType<FlyingSnakeCorpse>()] = [NPCID.FlyingSnake],
                [ModContent.ItemType<HellBatCorpse>()] = [NPCID.Hellbat],
                [ModContent.ItemType<HornetCorpse>()] = [
                    NPCID.Hornet, NPCID.HornetFatty, NPCID.HornetHoney, NPCID.HornetLeafy, NPCID.HornetSpikey,
                    NPCID.HornetStingy, NPCID.BigHornetFatty, NPCID.BigHornetHoney, NPCID.BigHornetLeafy,
                    NPCID.BigHornetSpikey, NPCID.BigHornetStingy, NPCID.BigMossHornet, NPCID.GiantMossHornet,
                    NPCID.LittleHornetFatty, NPCID.LittleHornetHoney, NPCID.LittleHornetLeafy,
                    NPCID.LittleHornetSpikey, NPCID.LittleHornetStingy, NPCID.LittleMossHornet, NPCID.MossHornet,
                    NPCID.TinyMossHornet
                ],
                [ModContent.ItemType<IceBatCorpse>()] = [NPCID.IceBat],
                [ModContent.ItemType<PigronCorpse>()] = [NPCID.PigronCorruption, NPCID.PigronCrimson,
                    NPCID.PigronHallow],
                [ModContent.ItemType<PixieCorpse>()] = [NPCID.Pixie],
                [ModContent.ItemType<ScorpionCorpse>()] = [NPCID.Scorpion, NPCID.ScorpionBlack],
                [ModContent.ItemType<FlyingEyeCorpse>()] = [
                    NPCID.CataractEye, NPCID.CataractEye2, NPCID.DemonEye, NPCID.DemonEye2, NPCID.DemonEyeOwl, 
                    NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship, NPCID.DialatedEye, NPCID.DialatedEye2, 
                    NPCID.GreenEye, NPCID.GreenEye2, NPCID.PurpleEye, NPCID.PurpleEye2, NPCID.SleepyEye,
                    NPCID.SleepyEye2
                ],
                [ModContent.ItemType<WanderingEyeCorpse>()] = [NPCID.WanderingEye]
            };
        }

        public override Dictionary<int, Resistance> GetNPCResistances()
        {
            return new()
            {
                [NPCID.Bee] = Resistance.Penetrate,
                [NPCID.BeeSmall] = Resistance.Penetrate,
                [NPCID.QueenBee] = Resistance.Penetrate,
                [NPCID.Reaper] = Resistance.Overheat,
                [NPCID.DungeonSpirit] = Resistance.Ignore,
                [NPCID.Ghost] = Resistance.Ignore,
                [NPCID.PirateGhost] = Resistance.Ignore,
                [NPCID.Poltergeist] = Resistance.Ignore,
                [NPCID.Wraith] = Resistance.Ignore,
                [NPCID.ShadowFlameApparition] = Resistance.Ignore
            };
        }

        public override Dictionary<int, Resistance> GetProjectileResistances()
        {
            return new()
            {
                [ProjectileID.Stinger] = Resistance.Penetrate,
                [ProjectileID.HornetStinger] = Resistance.Penetrate,
                [ProjectileID.QueenBeeStinger] = Resistance.Penetrate,
                [ProjectileID.MedusaHead] = Resistance.Ignore,
                [ProjectileID.MedusaHeadRay] = Resistance.Ignore,
                [ProjectileID.MagnetSphereBall] = Resistance.Overheat,
                [ProjectileID.MagnetSphereBolt] = Resistance.Overheat,
                [ProjectileID.WebSpit] = Resistance.Overheat,
                [ProjectileID.Web] = Resistance.Overheat,
                [ProjectileID.AncientDoomProjectile] = Resistance.Overheat,
                [ProjectileID.BrainOfConfusion] = Resistance.Penetrate,
                [ModContent.ProjectileType<Flashfield>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<FlashphereProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<AstrasphereProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<AstrasphereOrbits>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<LuxcaliburProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<VoltaicChainProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<GrandStrizerProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<WideThunder>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<SoulSiphonExplosion>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<GorgonGazeBeam>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<GorgoneiaBeam>()] = Resistance.Penetrate
            };
        }

        private void RopeSpeedUpgrade(ILContext il)
        {
            try
            {
                // Five calls to WorldGen.IsRope
                // Six Ldfa instructions on Vector2.Y
                ILCursor c = new ILCursor(il);
                ILLabel limitLabel = il.DefineLabel();
                ILLabel upLabel1 = il.DefineLabel();
                ILLabel upLabel2 = il.DefineLabel();
                ILLabel downLabel1 = il.DefineLabel();
                ILLabel downLabel2 = il.DefineLabel();

                Func<Player, bool> isRebirth = player => { 
                    return player.GetModPlayer<SeptimaPlayer>().septima is Rebirth; 
                };

                if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(0.2f)))
                {
                    throw new ILPatchFailureException(ModContent.GetInstance<GvMod>(), il, 
                        new Exception($"Failed first IL entrypoint {nameof(IL_Player.Update)}."));
                }

                if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(0.2f)))
                {
                    throw new ILPatchFailureException(ModContent.GetInstance<GvMod>(), il,
                        new Exception($"Failed second IL entrypoint {nameof(IL_Player.Update)}."));
                }

                if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(0.2f)))
                {
                    throw new ILPatchFailureException(ModContent.GetInstance<GvMod>(), il,
                        new Exception($"Failed third IL entrypoint {nameof(IL_Player.Update)}."));
                }

                if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(0.2f)))
                {
                    throw new ILPatchFailureException(ModContent.GetInstance<GvMod>(), il,
                        new Exception($"Failed fourth IL entrypoint {nameof(IL_Player.Update)}."));
                }

                if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(0.2f)))
                {
                    throw new ILPatchFailureException(ModContent.GetInstance<GvMod>(), il,
                        new Exception($"Failed fifth IL entrypoint {nameof(IL_Player.Update)}."));
                }

                if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(-3f)))
                {
                    throw new ILPatchFailureException(ModContent.GetInstance<GvMod>(), il,
                        new Exception($"Failed sixth IL entrypoint {nameof(IL_Player.Update)}."));
                }

                // Push player into the stack
                c.EmitLdarg0(); // Stack: -3, player
                // Return a bool based on the player's septima
                c.EmitDelegate<Func<Player, bool>>(isRebirth); // Stack: -3, bool
                // If septima is not rebirth, jump to c.MarkLabel() with the same label
                c.EmitBrfalse(limitLabel); // Stack: -3

                c.EmitLdcR4(5f);
                c.EmitSub();

                c.MarkLabel(limitLabel);

                c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(0.2f));

                // Repeat
                c.EmitLdarg0(); // Stack: 0.2, player
                c.EmitDelegate<Func<Player, bool>>(isRebirth); // Stack: 0.2, bool
                c.EmitBrfalse(upLabel1); // Stack: 0.2

                // Place 0.15 in stack
                c.EmitLdcR4(0.3f); // Stack: 0.2, 0.3
                // Add 0.15
                c.EmitAdd(); // Stack: 0.5

                c.MarkLabel(upLabel1);

                c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(0.02f));

                c.EmitLdarg0(); // Stack: 0.02, player
                c.EmitDelegate<Func<Player, bool>>(isRebirth); // Stack: 0.02, bool
                c.EmitBrfalse(upLabel2); // Stack: 0.02

                c.EmitLdcR4(0.03f); // Stack: 0.02, 0.03
                c.EmitAdd(); // Stack: 0.05

                c.MarkLabel(upLabel2);

                // Repeat for down movement
                c.TryGotoNext(MoveType.After, 
                    i => i.MatchLdcR4(0.2f));

                c.EmitLdarg0(); // Stack: 0.2, player
                c.EmitDelegate<Func<Player, bool>>(isRebirth); // Stack: 0.02, bool
                c.EmitBrfalse(downLabel1); // Stack: 0.2

                c.EmitLdcR4(0.3f); // Stack: 0.2, 0.3
                c.EmitAdd(); // Stack: 0.5

                c.MarkLabel(downLabel1);

                c.GotoNext(MoveType.After, 
                    i => i.MatchLdcR4(0.1f));

                c.EmitLdarg0(); // Stack: 0.1, player
                c.EmitDelegate<Func<Player, bool>>(isRebirth); // Stack: 0.1, bool
                c.EmitBrfalse(downLabel2); // Stack: 0.1
                
                c.EmitLdcR4(0.25f); // Stack: 0.1, 0.25
                c.EmitAdd(); // Stack: 0.35

                c.MarkLabel(downLabel2);
            }
            catch (Exception e)
            {
                ModContent.GetInstance<GvMod>().Logger.Error("Error adding IL edit on IL_Player.", e);
                MonoModHooks.DumpIL(ModContent.GetInstance<GvMod>(), il);
            }
        }

        public override void InitializeSeptima(Player player, SeptimaPlayer adept, Mod mod)
        {
        }

        private Item RebirthGrappleDetour(On_Player.orig_QuickGrapple_GetItemToUse orig, Player self)
        {
            Item item = orig(self);
            SeptimaPlayer adept = self.GetModPlayer<SeptimaPlayer>();
            PlayerBuffs buffs = self.GetModPlayer<PlayerBuffs>();

            if (adept.septima is Rebirth && buffs.ArmedPhenomenonStats > 0)
            {
                //Main.NewText("Accepted");
                float baseHookDamage = 12 + (buffs.ArmedPhenomenonStats * 8) + (adept.Stage * 5);
                item = new Item();
                item.SetDefaults(ItemID.AmethystHook);
                item.useStyle = ItemUseStyleID.None;
                item.useTime = 0;
                item.useAnimation = 0;
                item.shootSpeed = 5f;
                item.shoot = ModContent.ProjectileType<RebirthHook>();
                item.damage = (int)self.GetTotalDamage<MainAttackDamage>().ApplyTo(baseHookDamage);
                item.DamageType = ModContent.GetInstance<MainAttackDamage>();
                item.knockBack = 4f;
            }

            return item;
        }

        public override void PostLoadSeptima(Player player, SeptimaPlayer adept)
        {
        }

        public override void PostTagLoad()
        {
            LastBossKilled = SaveTags["LastKilledBoss"];
        }

        public override void PreSaveTag()
        {
            SaveTags["LastKilledBoss"] = LastBossKilled;
        }

        public override void MiscEffects(Player player, SeptimaPlayer adept)
        {
            /*Main.NewText("Sum", Color.Green);
            foreach (int num in player.grappling)
            {
                Main.NewText(num);
            }*/
            if (BodyDamageProjectileId > -1)
            {
                Projectile proj = Main.projectile[BodyDamageProjectileId];
                BodyDamageProjectileActive = proj.active && proj.ModProjectile is RebirthHitbox &&
                    proj.owner == player.whoAmI;
            }
            else
            {
                BodyDamageProjectileActive = false;
            }

            IsInPulleyOrGrappling = player.pulley || !player.grappling.Contains(-1);

            player.buffImmune[BuffID.Stoned] = true;
            player.buffImmune[ModContent.BuffType<SoulPetrification>()] = true;

            if (IsInPulleyOrGrappling)
            {
                player.maxFallSpeed *= 2f; 
                player.statDefense += 4;
                player.noKnockback = true;
            } 
        }

        public override void ArmedPhenomenonPreUpdate(Player player, SeptimaPlayer adept, int potency)
        {
            player.GetDamage<SeptimaDamage>() += 0.05f * potency;
            player.statDefense += potency;
            player.endurance += 0.02f;
            player.spikedBoots += 2;
            player.extraFall += 15;

            if (adept.SuperState)
            {
                player.endurance += 0.05f * potency;
                adept.SPRecoveryModifier += 0.15f;
            }

            if (IsInPulleyOrGrappling)
            {
                player.statDefense += 4 * potency;
                player.endurance += 0 + ((potency - 1) * 0.08f);
                
                if (player.velocity.Length() > MinGrappleSpeed)
                {
                    if ((!BodyDamageProjectileActive || BodyDamageProjectileId == -1) && 
                        Main.myPlayer == player.whoAmI)
                    {
                        SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/RopeHookContract") with
                        {
                            Volume = 0.5f,
                            PitchVariance = 0.1f
                        }, player.Center);

                        int finalDamage = (int)player.GetTotalDamage<MainAttackDamage>().
                            ApplyTo(GetBasicSkillPower(player, adept));
                        BodyDamageProjectileId = Projectile.NewProjectile(
                            player.GetSource_FromThis(), player.Center, Vector2.Zero,
                            ModContent.ProjectileType<RebirthHitbox>(), finalDamage, 3.5f,
                            player.whoAmI, -1
                            );
                    }

                    Projectile proj = Main.projectile[BodyDamageProjectileId];
                    BodyDamageProjectileActive = proj.active && proj.ModProjectile is RebirthHitbox &&
                        proj.owner == player.whoAmI;

                    if (BodyDamageProjectileActive)
                    {
                        proj.Center = player.Center;
                        proj.timeLeft = 4;
                        proj.friendly = true;
                        proj.hostile = false;
                        proj.netUpdate = true;
                    }
                }
            }
        }

        public override void SetArmedPhenomenonEquip(Player player, SeptimaPlayer adept, Mod mod)
        {
            player.head = EquipLoader.GetEquipSlot(mod, "RebirthArmedPhenomenon", EquipType.Head);
            player.body = EquipLoader.GetEquipSlot(mod, "RebirthArmedPhenomenon", EquipType.Body);
            player.legs = EquipLoader.GetEquipSlot(mod, "RebirthArmedPhenomenon", EquipType.Legs);
            player.handon = EquipLoader.GetEquipSlot(mod, "RebirthArmedPhenomenon", EquipType.HandsOn);
            player.handoff = EquipLoader.GetEquipSlot(mod, "RebirthArmedPhenomenon", EquipType.HandsOff);
            player.back = EquipLoader.GetEquipSlot(mod, "RebirthArmedPhenomenon", EquipType.Back);
        }

        public override void ArmedPhenomenonPostEquipUpdate(Player player, SeptimaPlayer adept, int potency)
        {

        }

        public override void DirectMovementEffects(Player player, SeptimaPlayer adept)
        {
        }

        public override bool CanUseMainSkillNoEP(Player player, SeptimaPlayer adept)
        {
            return (player.HeldItem.ModItem is SteelKunai || 
                Rebirth.CorpseItemTable.ContainsKey(player.HeldItem.type)) && !adept.Overheated;
        }

        public override bool MainSkillUse(Player player, SeptimaPlayer adept)
        {
            if (MainAttackTimer <= 0)
            {
                if (Main.myPlayer == player.whoAmI && !player.ItemAnimationActive)
                {
                    if (player.HeldItem.ModItem is SteelKunai)
                    {
                        bool super = false;
                        // Look for a corpse and return summon 
                        int inventoryPos = GetSummonItem(player);
                        Item summonItem = null;
                        if (inventoryPos != -1)
                        {
                            summonItem = player.inventory[inventoryPos];
                        }

                        KunaiSummon summonType = KunaiSummon.None;
                        int costMod = 1;
                        if (summonItem != null)
                        {
                            // Set extra damage based on corpse type
                            summonType = CorpseItemTable[summonItem.type];
                        }

                        int finalDamage = player.GetWeaponDamage(player.HeldItem);
                        float finalKnockback = player.GetWeaponKnockback(player.HeldItem);
                        
                        foreach (Condition condition in ProjectileTable[summonType].conditions)
                        {
                            if (!condition.IsMet())
                            {
                                summonType = KunaiSummon.None;
                            }
                        }

                        // Shoot Kunai
                        int kunaiAmount = 3;
                        float kunaiAngle = MathHelper.PiOver4 / 4f;

                        if (adept.DnizerMode)
                        {
                            kunaiAmount = 5;
                            kunaiAngle *= 2f;
                            kunaiAngle /= 3f;
                            super = true;
                            finalDamage += (int)player.GetTotalDamage<MainAttackDamage>().
                                ApplyTo(GetBasicSkillPower(player, adept));
                        }

                        Vector2 baseDirection = player.Center.DirectionTo(Main.MouseWorld);
                        bool canSummon = summonType != KunaiSummon.None;
                        float currentAngle = 0;
                        for (int i = 0; i < kunaiAmount; i++)
                        {
                            if (i % 2 == 0)
                            {
                                currentAngle -= i * kunaiAngle;
                            } else
                            {
                                currentAngle += i * kunaiAngle;
                            }

                            Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem, "Septima"),
                                player.Center, baseDirection.RotatedBy(currentAngle) * 8,
                                ModContent.ProjectileType<Kunai>(), finalDamage, finalKnockback,
                                player.whoAmI, (int)summonType, super ? 1 : 0, costMod);

                            if (canSummon)
                            {
                                if (inventoryPos != -1)
                                {
                                    if (player.inventory[inventoryPos].stack > 1)
                                    {
                                        player.inventory[inventoryPos].stack--;
                                    }
                                    else
                                    {
                                        player.inventory[inventoryPos].TurnToAir();
                                    }
                                    canSummon = player.inventory[inventoryPos].stack >= 1;
                                }
                            }

                            if (!canSummon)
                            {
                                summonType = KunaiSummon.None;
                            }
                        }

                        // Set animation time
                        MainAttackTimer = 30;
                        player.itemAnimation = player.itemAnimationMax = MainAttackTimer;

                        if (player.HeldItem.stack > 1)
                        {
                            player.HeldItem.stack--;
                        }
                        else
                        {
                            player.HeldItem.TurnToAir();
                        }
                    }

                    if (CorpseItemTable.ContainsKey(player.HeldItem.type))
                    {
                        KunaiSummon summonType = CorpseItemTable[player.HeldItem.type];
                        if (summonType != KunaiSummon.None && adept.CanConsumeEP(EPUseBase))
                        {
                            RaiseUndead(player.HeldItem, player.Center, player, adept, 2.5f);
                            
                            if (player.HeldItem.stack > 1)
                            {
                                player.HeldItem.stack--;
                            }
                            else
                            {
                                player.HeldItem.TurnToAir();
                            }

                            MainAttackTimer = 60;
                            player.itemAnimation = player.itemAnimationMax = MainAttackTimer;
                        }
                    }
                }
            }

            return false;
        }

        public override float GetBasicSkillPower(Player player, SeptimaPlayer adept)
        {
            float finalDamage = BaseBasicAttackDamage + (adept.Stage * 1.8f) + (adept.Level * 0.07f);

            if (player.GetModPlayer<PlayerBuffs>().DilationReticles)
            {
                finalDamage *= 2f;
            }

            return finalDamage;
        }

        public override float GetTagSkillPower(Player player, SeptimaPlayer adept, Tag tag, int tagCount)
        {
            float basePercent = 1f;
            float tagDivider = 6f;

            if (player.GetModPlayer<PlayerBuffs>().DilationReticles)
            {
                basePercent = 0.5f;
                tagDivider = 10f;
            }

            return basePercent + ((float)tag.tagLevel / tagDivider);
        }

        public bool RaiseUndead(KunaiSummon summonType, Vector2 position, Player owner, SeptimaPlayer adept,
            float potency = 1, float costModifier = 1)
        {
            bool fromSpecial = false;
            if (costModifier <= 0)
            {
                fromSpecial = true;
            }

            if (Main.myPlayer == owner.whoAmI && ProjectileTable.ContainsKey(summonType) &&
                ((adept.CanUseMainSkill() && !fromSpecial) || fromSpecial))
            {
                SummonStats stats = ProjectileTable[summonType];

                float baseAttack = GetBasicSkillPower(owner, adept) + stats.baseDamage;
                int finalDamage = (int)owner.GetTotalDamage<MainAttackDamage>().
                    ApplyTo(baseAttack * potency);

                for (int i = 0; i < 25; i++)
                {
                    Dust.NewDust(position - new Vector2(20), 40, 40, DustID.Stone);
                }

                Projectile.NewProjectile(owner.GetSource_FromThis("Septima"), position,
                    new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi), stats.projectileID, 
                    finalDamage, stats.baseKnockback, owner.whoAmI);

                //Main.NewText("Raised Zombie: " + summonType.ToString(), Color.Green);

                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/SmallResurrection") with
                {
                    PitchVariance = 0.1f
                }, position);

                adept.ConsumeEP(EPUseBase * costModifier);
                
                return true;
            }
            return false;
        }

        public bool RaiseUndead(Item item, Vector2 position, Player owner, SeptimaPlayer adept,
            float potency = 1, float costModifier = 1)
        {
            if (CorpseItemTable.ContainsKey(item.type))
            {
                return RaiseUndead(CorpseItemTable[item.type], position, owner, adept, potency, costModifier);
            }
            return false;
        }

        private int GetSummonItem(Player player)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (CorpseItemTable.ContainsKey(item.type))
                {
                    return i;
                }
            }
            return -1;
        }

        public override void ModifyHitNPC(Player player, SeptimaPlayer adept, NPC target, ref NPC.HitModifiers modifiers)
        {
            NPC checkedNPC = target;
            if (target.realLife != -1)
            {
                checkedNPC = Main.npc[target.realLife];
            }

            if (checkedNPC.boss || !player.GetModPlayer<SetBonusPlayer>().pulsarUpgrade ||
                checkedNPC.immortal) return;

            float lifePercent = (float)checkedNPC.life / (float)checkedNPC.lifeMax;
            if (lifePercent <= 0.5f)
            {
                float chance = 0.01f + ((0.5f - lifePercent) * 0.5f);
                if (Main._rand.NextFloat() <= chance)
                {
                    /*Main.NewText("Triggered instakill");
                    Main.NewText("Life: " + target.life);
                    Main.NewText("Max life: " + target.lifeMax);
                    Main.NewText("Percent: " + lifePercent);
                    Main.NewText("Chance: " + chance);*/
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(checkedNPC.Center - checkedNPC.Size / 2, 
                            (int)checkedNPC.Size.X, (int)checkedNPC.Size.Y, DustID.Firework_Pink);
                    }
                    modifiers.SetInstantKill();
                }
            }
        }

        public override int TagEffect(Player player, SeptimaPlayer adept, int index, ref NPCTags tags)
        {
            return 0;
        }

        public override int SecondarySkillUse(Player player, SeptimaPlayer adept)
        {
            // Notes: Mineral-based enemies (Meteor heads, golems, mimics, spike balls) are immune to
            // stoned and this particular debuff
            if (adept.SecondarySkillUseTime == 0 && Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<GorgonGazeBeam>(), 1, 0, player.whoAmI, 1);
            }

            if (adept.SecondarySkillUseTime >= GorgonGazeBeam.gorgonGazeBeamDuration)
            {
                return 600;
            }

            return 0;
        }

        public override void OnHurtByNPC(Player player, SeptimaPlayer adept, NPC npc, Player.HurtInfo info)
        {
            float armedPhenomenonStats = player.GetModPlayer<PlayerBuffs>().ArmedPhenomenonStats;
            if (adept.DnizerMode)
            {
                GorgonGazeBeam.PetrifyNPC(npc, 0.2f, ignoreCooldown: true);
            }
        }

        public override void ModifyNPCHurt(Player player, SeptimaPlayer adept, NPC npc, ref Player.HurtModifiers modifiers)
        {
            modifiers.ModifyHurtInfo += (ref Player.HurtInfo info) => {
                GraplingDamageCancellation(player, adept, ref info);
            };
            base.ModifyNPCHurt(player, adept, npc, ref modifiers);
        }

        public void GraplingDamageCancellation(Player player, SeptimaPlayer adept, ref Player.HurtInfo info)
        {
            int armedPhenomenonStats = player.GetModPlayer<PlayerBuffs>().ArmedPhenomenonStats;
            if (player.velocity.Length() < MinGrappleSpeed || armedPhenomenonStats <= 0) return;
            
            Resistance resistance = PlayerPrevasion.GetAttackResistance(info.DamageSource, adept);

            if (resistance == Resistance.None)
            {
                //Main.NewText("Received damage: " + info.Damage);
                //Main.NewText("Max: " + MaxDynamicDamageResistance);
                if (info.Damage <= (MaxDynamicDamageResistance * (armedPhenomenonStats / 3f)))
                {
                    //Main.NewText("Cancelling damage", Color.Green);
                    info.Cancelled = true;
                } else
                {
                    //Main.NewText("Proceeding with damage", Color.DarkRed);
                }
            }
        }

        public override void ModifyHurt(Player player, SeptimaPlayer adept, ref Player.HurtModifiers modifiers)
        {
            modifiers.ModifyHurtInfo += (ref Player.HurtInfo info) => {
                ModifySuperStateHurt(player, adept, ref info);
            };
            base.ModifyHurt(player, adept, ref modifiers);
        }

        private void ModifySuperStateHurt(Player player, SeptimaPlayer adept, ref Player.HurtInfo info)
        {
            Resistance resistance = PlayerPrevasion.GetAttackResistance(info.DamageSource, adept);

            PlayerBuffs buffs = player.GetModPlayer<PlayerBuffs>();
            // Ensure the interaction is neutral
            // Penetration and Overheat would ignore SuperState
            // Absorb benefits from the interaction and ignore doesn't interact
            if (resistance == Resistance.None && adept.SuperState)
            {
                // Main.NewText("Resistance is none");
                info.Knockback *= 0;

                float lifePercent = 0.08f;

                if (info.Damage <= player.statLifeMax2 * lifePercent || buffs.UnlimitedAnimus)
                {
                    // Main.NewText("Triggered the effect");
                    info.Damage = 1;
                }
            }
        }

        public override void OnHurt(Player player, SeptimaPlayer adept, Player.HurtInfo info)
        {
        }

        public override void OnDnizerActive(Player player, SeptimaPlayer adept)
        {
            if (Main.myPlayer != player.whoAmI) return;

            /*for (int i = -5; i < 6; i++)
            {
                Vector2 offset = new Vector2(120 * i, 0);
                int delay = 6 * Math.Abs(i);
                int finalDamage = (int)player.GetTotalDamage<SecondaryAttackDamage>().
                    ApplyTo(GetSecondarySkillPower(player, adept));
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center - offset,
                    Vector2.Zero, ModContent.ProjectileType<Thunder>(), finalDamage, 0, player.whoAmI,
                    delay);
            }*/
            base.OnDnizerActive(player, adept);
        }

        public override void OnBossDefeat(int bossID, Player player, SeptimaPlayer adept)
        {
            if (BossDefeatTable.ContainsKey(bossID))
            {
                if (adept.Level >= BossDefeatTable[bossID].levelRequirement &&
                    adept.Stage >= BossDefeatTable[bossID].stageRequirement)
                {
                    LastBossKilled = bossID;
                }
            }
        }

        public override bool GetSuperState(Player player, SeptimaPlayer adept)
        {
            ResurrectionPlayer resurrection = player.GetModPlayer<ResurrectionPlayer>();
            PlayerBuffs buffs = player.GetModPlayer<PlayerBuffs>(); // For PlayerBuffs, check for the buff instead
            // Because of the name, PlayerBuffs resets before this is called, and any flags will always be false

            bool gv2SuperCheck = player.HasBuff<SeptimalSurgeBuff>() || resurrection.resurrectionPower >= 2;
            bool gv3SuperCheck = adept.DnizerMode || resurrection.resurrectionPower >= 3;
            bool unlimitedAnimusSuperCheck = player.HasBuff<UnlimitedAnimusBuff>();

            return (gv2SuperCheck || gv3SuperCheck || unlimitedAnimusSuperCheck) && !adept.Overheated;
        }

        public override void DrawPassive(ref PlayerDrawSet drawInfo, Player player, SeptimaPlayer adept)
        {
            if (drawInfo.shadow != 0) return;

            if (adept.SuperState)
            {
                Rectangle playerRect = PlayerRenderTarget.
                    getPlayerTargetSourceRectangle(player.whoAmI);
                Rectangle sourceRectangle = new Rectangle(player.whoAmI * playerRect.Width, 0,
                    playerRect.Width, playerRect.Height);

                //Main.NewText(PlayerRenderTarget.Target.Size());

                Vector2 position = player.Center - Main.screenPosition;
                float XScaleVal = 1f + (MathF.Sin(SuperVisualTimer * 0.45f) * 0.145f);
                float YScaleVal = 1f + (MathF.Sin(0.1f + (SuperVisualTimer * 0.3f)) * 0.145f);

                drawInfo.DrawDataCache.Add(
                    new DrawData(
                        PlayerRenderTarget.Target,
                        position,
                        sourceRectangle,
                        adept.septima.MainColor * 0.55f,
                        0,
                        playerRect.Size() / 2 + player.Size / 2,
                        new Vector2(XScaleVal, YScaleVal),
                        SpriteEffects.None,
                        0)
                    );
            }
        }

        public override void ResetEffects(Player player, SeptimaPlayer adept)
        {
            SuperVisualTimer++;

            if (MainAttackTimer > 0) MainAttackTimer--;
        }
    }
}
