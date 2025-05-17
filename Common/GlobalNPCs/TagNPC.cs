using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalNPCs
{
    public class TagNPC : GlobalNPC
    {
        public int tagLevel = 0;
        public TagType lastTagType = TagType.AzureStriker;
        public bool attacked = false;
        public int framesUntilReset = 1;
        public int framesUntilTagReset = 1;

        static Asset<Texture2D> attackEffect1;
        static Asset<Texture2D> attackEffect2;
        static Asset<Texture2D> attackEffect3;

        Asset<Texture2D> LoadedTagEffect;

        Asset<Texture2D> LoadedInnerTagEffect;

        private int frame = 0;
        private int frameTimer = 0;
        private Vector2 effect1Offset = Vector2.Zero;
        private float effect1Rotation = 0;
        private Vector2 effect2Offset = Vector2.Zero;
        private float effect2Rotation = 0;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                attackEffect1 = ModContent.Request<Texture2D>("GvMod/Assets/Effects/Damage1");
                attackEffect2 = ModContent.Request<Texture2D>("GvMod/Assets/Effects/Damage2");
                attackEffect3 = ModContent.Request<Texture2D>("GvMod/Assets/Effects/Damage3");
            }
            base.Load();
        }

        public override void Unload()
        {
            attackEffect1 = null;
            attackEffect2 = null;
            attackEffect3 = null;
            base.Unload();
        }

        public override void ResetEffects(NPC npc)
        {
            if (framesUntilReset <= 0)
            {
                attacked = false;
            } else
            {
                framesUntilReset--;
            }

            if (framesUntilTagReset <= 0)
            {
                tagLevel = 0;
            }
            else
            {
                framesUntilTagReset--;
            }
        }

        public override void SetDefaults(NPC entity)
        {
            effect1Offset = new Vector2(Main.rand.NextFloat(-15, 16), Main.rand.NextFloat(-15, 16));
            effect1Rotation = MathHelper.PiOver2 * Main.rand.Next(0, 5);
            effect2Offset = new Vector2(Main.rand.NextFloat(-15, 16), Main.rand.NextFloat(-15, 16));
            effect2Rotation = MathHelper.PiOver2 * Main.rand.Next(0, 5);
            base.SetDefaults(entity);
        }

        public override bool PreAI(NPC npc)
        {
            if (attacked)
            {
                frameTimer++;
                if (frameTimer >= 8)
                {
                    effect1Offset = new Vector2(Main.rand.NextFloat(-15, 16), Main.rand.NextFloat(-15, 16));
                    effect1Rotation = MathHelper.PiOver2 * Main.rand.Next(0, 5);
                    effect2Offset = new Vector2(Main.rand.NextFloat(-15, 16), Main.rand.NextFloat(-15, 16));
                    effect2Rotation = MathHelper.PiOver2 * Main.rand.Next(0, 5);
                    frame++;
                    if (frame >= 2)
                    {
                        frame = 0;
                    }
                }
            }
            return base.PreAI(npc);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 basePosition = npc.Center - Main.screenPosition;
            if (tagLevel > 0)
            {
                LoadedTagEffect = ModContent.Request<Texture2D>($"GvMod/Assets/Effects/Tag{tagLevel}");
                LoadedInnerTagEffect = ModContent.Request<Texture2D>($"GvMod/Assets/Effects/TagMark{tagLevel}");

                float rotation = 0;

                switch (tagLevel)
                {
                    case 2:
                        rotation = (float)(Main.timeForVisualEffects * 0.01745329);
                        break;
                    case 3:
                        rotation = (float)(Main.timeForVisualEffects * -0.03490659);
                        break;
                }

                spriteBatch.Draw(
                    LoadedTagEffect.Value,
                    basePosition,
                    new Rectangle(0, 0, LoadedTagEffect.Width(), LoadedTagEffect.Height()),
                    Color.White * 0.5f,
                    rotation,
                    LoadedTagEffect.Size() / 2,
                    1f,
                    SpriteEffects.None,
                    0
                );

                spriteBatch.Draw(
                    LoadedInnerTagEffect.Value,
                    basePosition,
                    new Rectangle(0, 0, LoadedInnerTagEffect.Width(), LoadedInnerTagEffect.Height()),
                    Color.White * 0.5f,
                    0,
                    LoadedInnerTagEffect.Size() / 2,
                    1f,
                    SpriteEffects.None,
                    0
                );
            }

            if (attacked)
            {
                Texture2D effect1 = attackEffect1.Value;
                Texture2D effect2 = attackEffect2.Value;
                Texture2D effect3 = attackEffect3.Value;

                var position1 = basePosition + effect1Offset;
                Vector2 origin1 = new Vector2(effect1.Width, frame * effect1.Height / 2) / 2;
                var position2 = basePosition + effect2Offset;
                Vector2 origin2 = new Vector2(effect2.Width, frame * effect2.Height / 2) / 2;

                spriteBatch.Draw(
                    effect2,
                    position2,
                    new Rectangle(0, frame * effect2.Height / 2, effect2.Width, effect2.Height / 2),
                    Color.White * 0.5f,
                    effect2Rotation,
                    origin2,
                    1f,
                    SpriteEffects.None,
                    0
                );

                spriteBatch.Draw(
                    effect1,
                    position1,
                    new Rectangle(0, frame * effect1.Height / 2, effect1.Width, effect1.Height/2),
                    Color.White * 0.5f,
                    effect1Rotation,
                    origin1,
                    1f,
                    SpriteEffects.None,
                    0
                );

                if (frame%2 == 0)
                {
                    spriteBatch.Draw(
                        effect3,
                        basePosition,
                        new Rectangle(0, 0, effect3.Width, effect3.Height),
                        Color.White,
                        0f,
                        effect3.Size() / 2,
                        1f,
                        SpriteEffects.None,
                        0
                    );
                }
            }

            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}
