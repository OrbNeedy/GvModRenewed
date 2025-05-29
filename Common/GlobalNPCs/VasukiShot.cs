using Microsoft.Xna.Framework;
using GvMod.Content.Items.Weapons;
using GvMod.Content.Projectiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.IO;

namespace GvMod.Common.GlobalNPCs
{
    public class VasukiShot : GlobalNPC
    {
        public bool vasukiShoot = false;
        private int previousDartOwner = 0;
        private int previousDartDamage = 0;
        private float previousDartKnockback = 0;
        private int[] previousDartAis = new int[3];
        public int vasukiTimer = 0;

        public override bool InstancePerEntity => true;

        /*public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(vasukiShoot);
            binaryWriter.Write(vasukiTimer);
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            vasukiShoot = bitReader.ReadBit();
            vasukiTimer = binaryReader.ReadInt32();
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }*/

        public override void AI(NPC npc)
        {
            if (vasukiTimer > 0)
            {
                vasukiTimer--;
            } else
            {
                if (vasukiShoot)
                {
                    if (Main.myPlayer == previousDartOwner)
                    {
                        NPC target = GetClosestNPC(npc.position, npc);
                        Vector2 velocity = new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi);
                        if (target != null) velocity = npc.DirectionTo(target.Center);
                        Projectile.NewProjectile(Main.player[previousDartOwner].GetSource_FromThis(), npc.Center,
                            velocity * 10, ModContent.ProjectileType<HairDartProjectile>(),
                            previousDartDamage, previousDartKnockback, previousDartOwner, previousDartAis[0],
                            previousDartAis[1], previousDartAis[2]);
                    }
                    vasukiShoot = false;
                }
            }
            base.AI(npc);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!vasukiShoot && projectile.ModProjectile is HairDartProjectile && npc.active && npc.life > 0 &&
                !npc.immortal && !npc.friendly)
            {
                previousDartOwner = projectile.owner;
                previousDartDamage = projectile.damage;
                previousDartKnockback = projectile.knockBack;
                for (int i = 0; i < 3; i++)
                {
                    previousDartAis[i] = (int)projectile.ai[i];
                    if (projectile.ai[i] == (int)DartLeaderUpgrades.Vasuki)
                    {
                        vasukiShoot = true;
                        vasukiTimer = 90;
                        previousDartAis[i] = (int)DartLeaderUpgrades.VasukiVisuals;
                        // npc.netUpdate = true;
                    }
                }
            }
            base.OnHitByProjectile(npc, projectile, hit, damageDone);
        }

        public NPC GetClosestNPC(Vector2 position, NPC ommit)
        {
            int closestDistance = int.MaxValue;
            NPC returnNPC = null;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.Distance(position) < closestDistance && npc != ommit && npc.active && npc.life > 0 && 
                    !npc.immortal && !npc.friendly)
                {
                    returnNPC = npc;
                }
            }
            return returnNPC;
        }
    }
}
