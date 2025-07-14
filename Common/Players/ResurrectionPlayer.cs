using GvMod.Content;
using GvMod.Content.Buffs;
using GvMod.Content.Items.Upgrades;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GvMod.Common.Players
{
    // Also can be used as a reference to the potency of an anthem
    public enum AnthemAuraType
    {
        Invisible,
        LumenWeak,
        Lumen,
        Djin, 
        Muse
    }

    public class ResurrectionPlayer : ModPlayer
    {
        public bool resurrected = false;
        public bool canResurrect = false;
        public float resurrectionPower = 0;
        public ulong resurrectionTime = 0;
        public AnthemAuraType type = AnthemAuraType.LumenWeak;

        public override void ResetEffects()
        {
            //Main.NewText("Resurrection Player's ResetEffects.");
            if (resurrected) resurrectionTime++;
            resurrected = false;
            canResurrect = false;
            resurrectionPower = 0;
            base.ResetEffects();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["ResurrectionTime"] = resurrectionTime;
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("ResurrectionTime"))
            {
                resurrectionTime = (ulong)tag.GetLong("ResurrectionTime");
            }
            base.LoadData(tag);
        }

        public override void PreUpdate()
        {
            //Main.NewText("Resurrection Player's PreUpdate.");
            if (resurrected)
            {
                SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();

                adept.EPUseModifier -= MathHelper.Clamp((resurrectionPower - 1) * 0.5f, 0, 1);
                adept.APRecoveryModifier += resurrectionPower > 3 ? 0.25f : 0;
                adept.EPRecoveryModifier += 0.1f;
                Player.GetDamage<SeptimaDamage>() += 0.15f * resurrectionPower;

                if (Player.HasBuff(ModContent.BuffType<ResurrectionCooldown>()) || resurrectionPower <= 0 ||
                    !canResurrect)
                {
                    resurrected = false;

                    Player.ClearBuff(ModContent.BuffType<Anthem>());
                    if (!Player.HasBuff(ModContent.BuffType<ResurrectionCooldown>()))
                    {
                        Player.AddBuff(ModContent.BuffType<ResurrectionCooldown>(), 18000);
                    }
                }
            }
            base.PreUpdate();
        }

        public override void PostUpdate()
        {
            //Main.NewText("Resurrection Player's PostUpdate.");
            base.PostUpdate();
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            //Main.NewText("Resurrection Player's PreKill.");
            if (resurrectionPower > 0 && !Player.HasBuff<ResurrectionCooldown>() && canResurrect && !resurrected)
            {
                Player.Heal(Player.statLifeMax2);
                playSound = false;
                genDust = false;
                Player.AddBuff(ModContent.BuffType<Anthem>(), 3600 + (int)(1200 * (resurrectionPower - 1)));
                resurrected = true;

                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/AnthemActive") with
                {
                    PitchVariance = 0.1f
                }, Player.Center);
                return false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Player.GetModPlayer<SeptimaPlayer>().Stage >= 7)
            {
                if (Main.rand.NextBool(10))
                {
                    int max = Main.rand.Next(0, 6);
                    for (int i = 0; i < max; i++)
                    {
                        Item.NewItem(Player.GetSource_Death(), new Rectangle((int)Player.position.X,
                            (int)Player.position.Y, Player.width, Player.height),
                            ModContent.ItemType<Stage4Upgrade>());
                    }
                }
            }

            if (Player.GetModPlayer<SeptimaPlayer>().Stage >= 9)
            {
                if (Main.rand.NextBool(10))
                {
                    int max = Main.rand.Next(0, 6);
                    for (int i = 0; i < max; i++)
                    {
                        Item.NewItem(Player.GetSource_Death(), new Rectangle((int)Player.position.X,
                            (int)Player.position.Y, Player.width, Player.height),
                            ModContent.ItemType<Stage5Upgrade>());
                    }
                }
            }

            base.Kill(damage, hitDirection, pvp, damageSource);
        }
    }
}
