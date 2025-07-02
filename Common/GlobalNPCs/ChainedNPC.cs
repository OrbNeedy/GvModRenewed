using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GvMod.Common.GlobalNPCs
{
    public class ChainedNPC : GlobalNPC
    {
        public bool Pierced { get; set; } = false;

        public override bool InstancePerEntity => true;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(Pierced);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            Pierced = bitReader.ReadBit();
        }

        public override bool PreAI(NPC npc)
        {
            if (Pierced)
            {
                Pierced = false;
                npc.position = npc.oldPosition;
                npc.netUpdate = true;
                return false;
            }
            return base.PreAI(npc);
        }
    }
}
