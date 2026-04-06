using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalNPCs
{
    public class DebuffNPC : GlobalNPC
    {
        public bool soulPetrified = false;
        public int soulPetrificationImmunity = 0;
        public const int maxSoulPetrificationImmunity = 150;
        public bool previousNoTileCollideState = false;
        public SpriteBatchState prevState = null;
        public override bool InstancePerEntity => true;

        public override bool PreAI(NPC npc)
        {
            if (IsPetrified(npc))
            {
                npc.noTileCollide = false;
                npc.velocity += new Vector2(0, 2);

                npc.velocity *= 0.8f;
                if (npc.collideY && npc.velocity.Y > 0)
                {
                    npc.velocity.Y = 0;
                }
                return false;
            }
            return base.PreAI(npc);
        }

        public bool IsPetrified(NPC npc)
        {
            bool petrified = soulPetrified;

            if (npc.realLife != -1)
            {
                petrified = Main.npc[npc.realLife].GetGlobalNPC<DebuffNPC>().soulPetrified;
            }

            return petrified;
        }

        public override bool CanHitNPC(NPC npc, NPC target)
        {
            return base.CanHitNPC(npc, target) && !soulPetrified;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(npc, target, ref cooldownSlot) && !soulPetrified;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (IsPetrified(npc))
            {
                modifiers.SourceDamage += 0.1f;
                modifiers.Defense *= 0.5f;
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (IsPetrified(npc))
            {
                modifiers.SourceDamage += 0.1f;
                modifiers.Defense *= 0.5f;
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (IsPetrified(npc))
            {
                prevState = SpriteBatchExt.GetState(spriteBatch);
                SpriteBatchExt.Restart(spriteBatch, prevState, SpriteSortMode.Immediate);
                MiscShaderData shader = GameShaders.Misc["GvMod:Petrification"];

                shader.Apply();
            } else
            {
                prevState = null;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (prevState != null)
            {
                SpriteBatchExt.Restart(spriteBatch, prevState);
                prevState = null;
            }
            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void ResetEffects(NPC npc)
        {
            if (IsPetrified(npc))
            {
                npc.noTileCollide = previousNoTileCollideState;
            }
            soulPetrified = false;

            if (soulPetrificationImmunity > 0) soulPetrificationImmunity--;
        }
    }
}
