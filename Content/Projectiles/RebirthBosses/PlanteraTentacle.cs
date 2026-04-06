using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class PlanteraTentacle : ModProjectile
    {
        public int ParentNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public float TargetTimerRate { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public float Timer { get => Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public float TimerRate { get => Projectile.localAI[1]; set => Projectile.localAI[1] = value; }
        public Vector2 parentPosition = Vector2.Zero;

        public override void SetDefaults()
        {
            Main.instance.LoadNPC(NPCID.PlanterasTentacle);
            Projectile.width = (int)TextureAssets.Npc[NPCID.PlanterasTentacle].Size().X;
            Projectile.height = (int)TextureAssets.Npc[NPCID.PlanterasTentacle].Size().Y / 4;
            Main.projFrames[Projectile.type] = 4;
            Projectile.frame = 0;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/NPC_{NPCID.PlanterasTentacle}";

        public override void AI()
        {
            if (ParentNPC != -1)
            {
                Projectile parent = Main.projectile[ParentNPC];
                if (parent.active && parent.ModProjectile is Plantera)
                {
                    parentPosition = parent.Center;
                    Vector2 offset = new Vector2(144).RotatedBy(Timer * 0.08f);
                    float distance = Projectile.Center.Distance(parentPosition + offset);

                    Projectile.velocity = Projectile.Center.DirectionTo(parentPosition + offset) * (2 + (distance / 24));

                    Projectile.rotation = Projectile.Center.DirectionTo(parent.Center).ToRotation();
                } else
                {
                    Projectile.timeLeft = -1;
                    Projectile.active = false;
                }
            } else
            {
                Projectile.timeLeft = -1;
                Projectile.active = false;
            }

            Timer += TimerRate;
            TimerRate = float.Lerp(TimerRate, TargetTimerRate, 0.02f);

            if (Main._rand.NextBool(120))
            {
                TargetTimerRate = Main._rand.NextFloat(-2, 2);
            }


            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft -= 12;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 24;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
            Vector2 additionalVel = Projectile.velocity * 0.9f;

            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 388);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 389);
        }

        public static void GetFrame(ref int timer, ref int frame, int maxFrame, int minFrame, int maxFrameTime)
        {
            if (frame < minFrame) frame = minFrame;

            if (timer >= maxFrameTime)
            {
                frame++;
                if (frame >= maxFrame)
                {
                    frame = minFrame;
                }
                timer = 0;
            }

            timer++;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> vine = TextureAssets.Chain27;
            float distance = Projectile.Center.Distance(parentPosition);
            Vector2 direction = Projectile.Center.DirectionTo(parentPosition);
            Vector2 initialDistance = direction * vine.Height() / 2f;

            for (int i = 0; i < distance; i += vine.Height())
            {
                Vector2 offset = direction * i;
                Vector2 vinePos = Projectile.Center + initialDistance + offset;
                Main.EntitySpriteDraw(
                    vine.Value,
                    vinePos - Main.screenPosition,
                    vine.Frame(),
                    Lighting.GetColor(vinePos.ToTileCoordinates()),
                    Projectile.Center.AngleTo(parentPosition) + MathHelper.PiOver2,
                    vine.Size() / 2f,
                    1,
                    SpriteEffects.None
                );
            }

            return base.PreDrawExtras();
        }
    }
}
