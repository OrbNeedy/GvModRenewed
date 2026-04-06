using GvMod.Content;
using GvMod.Content.Buffs;
using GvMod.Content.Items.Upgrades;
using Microsoft.Xna.Framework;
using System.IO;
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
        Djinn, 
        Muse
    }

    public class ResurrectionPlayer : ModPlayer
    {
        public float forcedRebirthResurrection = 0;
        public Vector2 rebirthResurrectionPosition = Vector2.Zero;
        public bool resurrected = false;
        public bool canResurrect = false;
        public bool wearingNecklace = false;
        public int breakNecklace = 0;
        public float resurrectionPower = 0;
        public ulong resurrectionTime = 0;
        public AnthemAuraType type = AnthemAuraType.LumenWeak;

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            SyncRebirth(Player.whoAmI, forcedRebirthResurrection);
        }

        public static void SyncRebirth(int whoAmI, float forcedResurrection)
        {
            ModPacket packet = ModContent.GetInstance<GvMod>().GetPacket();
            packet.Write((byte)MessageType.ResurrectionSync); // ID
            packet.Write((byte)whoAmI); // Player
            packet.Write((byte)forcedResurrection); // Force Resurrection
            packet.Send(ignoreClient: whoAmI);
        }

        public static void SyncResurrection(int whoAmI, bool resurrected, byte auraType)
        {
            ModPacket packet = ModContent.GetInstance<GvMod>().GetPacket();
            packet.Write((byte)MessageType.ReincarnationVisualSync); // ID
            packet.Write((byte)whoAmI); // Player
            packet.Write(resurrected); // Anthem Resurrection
            packet.Write(auraType); // Aura type
            packet.Send(ignoreClient: whoAmI);
        }

        public static void ReceiveResurrectSync(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            // If server, the target is the player who sent the message
            if (Main.dedServ)
            {
                player = whoAmI;
            }

            float forcedResurrect = reader.ReadByte();

            // If the target is not the player who sent the message, change it locally
            if (player != Main.myPlayer)
            {
                Main.player[player].GetModPlayer<ResurrectionPlayer>().forcedRebirthResurrection = forcedResurrect;
            }

            // If it's the server, send the change to everyone else
            if (Main.dedServ)
            {
                SyncRebirth(player, forcedResurrect);
            }
        }

        public static void ReceiveVisualResurrectSync(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            // If server, the target is the player who sent the message
            if (Main.dedServ)
            {
                player = whoAmI;
            }

            bool resurrect = reader.ReadBoolean();
            byte auraType = reader.ReadByte();

            // If the target is not the player who sent the message, change it locally
            if (player != Main.myPlayer)
            {
                ResurrectionPlayer resurectee = Main.player[player].GetModPlayer<ResurrectionPlayer>();
                resurectee.resurrected = resurrect;
                resurectee.type = (AnthemAuraType)auraType;
            }

            // If it's the server, send the change to everyone else
            if (Main.dedServ)
            {
                SyncResurrection(player, resurrect, auraType);
            }
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            ResurrectionPlayer clone = (ResurrectionPlayer)targetCopy;
            clone.forcedRebirthResurrection = forcedRebirthResurrection;
            clone.resurrected = resurrected;
            clone.type = type;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            ResurrectionPlayer clone = (ResurrectionPlayer)clientPlayer;

            if (forcedRebirthResurrection != clone.forcedRebirthResurrection)
            {
                SyncRebirth(Player.whoAmI, forcedRebirthResurrection);
            }

            if (resurrected != clone.resurrected || type != clone.type)
            {
                SyncResurrection(Player.whoAmI, resurrected, (byte)type);
            }
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
                if (breakNecklace > 0) return;
                // Main.NewText("Resurrection power: " + resurrectionPower);
                SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();

                adept.EPUseModifier *= MathHelper.Clamp(1 - (resurrectionPower * 0.34f), 0, 1);
                adept.EPRecoveryModifier += 0.12f * resurrectionPower;
                adept.EPCooldownModifier *= resurrectionPower >= 2 ? 2 : 1;
                adept.SPRecoveryModifier += resurrectionPower >= 3 ? 0.25f : 0;
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

            if (wearingNecklace && !Player.HasBuff<ResurrectionCooldown>() && !resurrected)
            {
                breakNecklace = 2;

                Player.Heal(Player.statLifeMax2);
                playSound = false;
                genDust = false;
                Player.AddBuff(ModContent.BuffType<Anthem>(), 4800);
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

        public override void UpdateDead()
        {
            rebirthResurrectionPosition = Player.Center;
            if (forcedRebirthResurrection > 0)
            {
                Player.respawnTimer = 0;
            }
        }

        public override void ResetEffects()
        {
            //Main.NewText("Resurrection Player's ResetEffects.");
            if (resurrected) resurrectionTime++;
            resurrected = false;
            canResurrect = false;
            wearingNecklace = false;
            if (breakNecklace > 0) breakNecklace--;
            resurrectionPower = 0;

            //Main.NewText("Rebirth position: " + rebirthResurrectionPosition);
            //Main.NewText("Player position: " + Player.Center);
            if (forcedRebirthResurrection > 0)
            {
                Player.statLife = (int)(Player.statLifeMax2 * forcedRebirthResurrection);
                Player.Center = rebirthResurrectionPosition;
                forcedRebirthResurrection = 0;
            } else
            {
                rebirthResurrectionPosition = Player.Center;
            }
        }
    }
}
