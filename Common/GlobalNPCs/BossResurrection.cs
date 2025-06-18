using GvMod.Common.Configs;
using GvMod.Common.Systems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalNPCs
{
    public class BossResurrection : GlobalNPC
    {
        public bool resurrected = false;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return (lateInstantiation && (entity.boss || entity.type == NPCID.EaterofWorldsHead || 
                entity.type == NPCID.EaterofWorldsBody || entity.type == NPCID.EaterofWorldsTail));
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source.Context == "Resurrected")
            {
                resurrected = true;
                npc.takenDamageMultiplier += 5;
                //npc.damage *= 1.5f;
            }
            base.OnSpawn(npc, source);
        }

        public override bool PreAI(NPC npc)
        {
            if (resurrected) ModContent.GetInstance<TimeModificationSystem>().stoppingTime = true;
            return base.PreAI(npc);
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (resurrected)
            {
                modifiers.FinalDamage *= 1.5f;
            }
            base.ModifyHitPlayer(npc, target, ref modifiers);
        }

        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (resurrected)
            {
                modifiers.FinalDamage *= 2f;
            }
            base.ModifyHitNPC(npc, target, ref modifiers);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            base.ModifyIncomingHit(npc, ref modifiers);
        }

        public override bool PreKill(NPC npc)
        {
            // npc.boss is checked here instead of AppliesToEntity because of the eater of worlds, whose .boss is 
            // only set to true before death
            if (!resurrected && ModContent.GetInstance<GameplayConfig>().BossResurrection &&
                Main.rand.NextBool(6) && npc.boss && Main.BestiaryTracker.Kills.GetKillCount(npc) >= 2)
            {
                NPC resurrectedNPC = NPC.NewNPCDirect(npc.GetSource_FromThis("Resurrected"), npc.Center, 
                    npc.type, ai0: npc.ai[0], ai1: npc.ai[1], ai2: npc.ai[2], ai3: npc.ai[3], target: npc.target);
                resurrectedNPC.localAI = npc.localAI;
            }
            return base.PreKill(npc);
        }

        /*public override bool CheckDead(NPC npc)
        {
            if (!resurrected && ModContent.GetInstance<GameplayConfig>().BossResurrection && 
                Main.rand.NextBool(4))
            {
                resurrected = true;
                return false;
            }
            return base.CheckDead(npc);
        }*/
    }
}
