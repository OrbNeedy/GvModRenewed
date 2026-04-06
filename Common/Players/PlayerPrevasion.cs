using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using GvMod.Common.Players.Sevenths;
using System.IO;
using GvMod.Content.Dusts;
using Terraria.WorldBuilding;
using Terraria.DataStructures;

namespace GvMod.Common.Players
{
    public class PlayerPrevasion : ModPlayer
    {
        public float PrevasionCostAvoidanceChance { get; set; } = 0;
        public float PrevasionCost { get; set; } = 0;
        public float PrevasionCostModifier { get; set; } = 1;
        /// <summary>
        /// The maximum percent of damage relative to the player's max life that can be prevaded.
        /// <br/>If the damage is less than this plus <see cref="PrevasionDamageLimit"/>, it can be prevaded.
        /// <br/>The only exception is attacks that penetrate the adept's septima.
        /// </summary>
        public float PrevasionLifeLimit { get; set; } = 0;
        public int PrevasionDamageLimit { get; set; } = 0;
        public int PrevasionIframes { get; set; } = 0;
        public int BasePrevasionIframes = 45;

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            SyncPrevasion(Player.whoAmI, PrevasionIframes);
        }

        public static void SyncPrevasion(int whoAmI, int iFrames)
        {
            ModPacket packet = ModContent.GetInstance<GvMod>().GetPacket();
            packet.Write((byte)MessageType.PrevasionVisualSync); // ID
            packet.Write((byte)whoAmI); // Player
            packet.Write((byte)iFrames); // Prevasion iFrames
            packet.Send(ignoreClient: whoAmI);
        }

        public static void ReceivePrevasionSync(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            // If server, the target is the player who sent the message
            if (Main.dedServ)
            {
                player = whoAmI;
            }

            int iFrames = reader.ReadByte();

            // If the target is not the player who sent the message, change it locally
            if (player != Main.myPlayer)
            {
                PlayerPrevasion adept = Main.player[player].GetModPlayer<PlayerPrevasion>();
                adept.PrevasionIframes = iFrames;
            }

            // If it's the server, send the change to everyone else
            if (Main.dedServ)
            {
                SyncPrevasion(player, iFrames);
                // Main.player[player].GetModPlayer<ResurrectionPlayer>().SyncRebirth(player, forcedResurrect);
            }
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            // Note: Activating prevasion also causes tags on the enemy to disappear
            // CCed will bypass all forms of prevasion, for balance with other mods 
            // Overheat will also prevent prevasion from happening
            SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();
            if (Player.CCed || adept.Overheated || !info.Dodgeable ||
                info.CooldownCounter == ImmunityCooldownID.DD2OgreKnockback || 
                !adept.septima.AllowPrevasion) return false;

            Resistance resistance = GetAttackResistance(info.DamageSource, adept);

            // All resistances have a special interaction that needs the player to get hurt, so 
            // anything that is not neutral will ignore prevasion.
            if (resistance != Resistance.None)
            {
                return base.FreeDodge(info);
            }

            float limit = (Player.statLifeMax2 * PrevasionLifeLimit) + PrevasionDamageLimit;

            //Main.NewText("Prevasion limit: " + limit);
            //Main.NewText("Hit damage: " + info.Damage);

            if (info.Damage <= limit)
            {
                //Main.NewText("Prevasion activated");

                if (1 - PrevasionCostAvoidanceChance >= Main.rand.NextFloat())
                {
                    float finalEPCost = PrevasionCost * PrevasionCostModifier;
                    
                    if (finalEPCost < 0) finalEPCost = 0;

                    adept.CurrentEP -= finalEPCost;
                    if (adept.CurrentEP <= 0)
                    {
                        adept.ForceOverheat(ignoreBuffs: true);
                    }
                }

                Player.immune = true;
                Player.AddImmuneTime(ImmunityCooldownID.General, BasePrevasionIframes);
                Player.AddImmuneTime(ImmunityCooldownID.Bosses, BasePrevasionIframes);
                Player.AddImmuneTime(ImmunityCooldownID.TileContactDamage, BasePrevasionIframes);
                Player.AddImmuneTime(ImmunityCooldownID.Lava, BasePrevasionIframes);
                Player.AddImmuneTime(ImmunityCooldownID.WrongBugNet, BasePrevasionIframes);
                
                //Main.NewText("Iframes: " + BasePrevasionIframes);

                adept.EPCooldownTimer = adept.septima.PrevasionEPCooldownBaseTimer;
                PrevasionIframes = BasePrevasionIframes;

                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/Prevasion1") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.5f
                }, Player.Center);

                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/Prevasion2") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.5f
                }, Player.Center);

                for (int i = 0; i < 10; i++)
                {
                    float velX = Main._rand.NextFloat(1, 2);
                    float velY = -Main._rand.NextFloat(1, 2);
                    if (Main._rand.NextBool()) velX *= -1;
                    Dust.NewDust(Player.position, Player.width, Player.height, 
                        ModContent.DustType<PrevasionFeathers>(), velX, velY, 
                        newColor: adept.septima.MainColor, 
                        Scale: Main._rand.NextFloat(0.95f, 1.05f));
                }

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    SyncPlayer(-1, Main.myPlayer, false);
                }

                return true;
            }
            //Main.NewText("Prevasion ignored");

            return base.FreeDodge(info);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();

            if (adept.Overheated || adept.septima.AllowPrevasion) return;
            float limit = (Player.statLifeMax2 * PrevasionLifeLimit) + PrevasionDamageLimit;

            Resistance resistance = GetAttackResistance(modifiers.DamageSource, adept);
            // All resistances have a special interaction that needs the player to get hurt, so 
            // anything that is not neutral will ignore prevasion.
            if (resistance != Resistance.None)
            {
                return;
            }

            //Main.NewText("Prevasion limit: " + limit);
            //Main.NewText("Hit damage: ");

            if (limit > 0)
            {
                //Main.NewText("Prevasion activated");

                if (1 - PrevasionCostAvoidanceChance >= Main.rand.NextFloat())
                {
                    float finalEPCost = PrevasionCost * PrevasionCostModifier;

                    finalEPCost *= 0.4f;

                    if (finalEPCost < 0) finalEPCost = 0;

                    adept.CurrentEP -= finalEPCost;
                    if (adept.CurrentEP <= 0)
                    {
                        adept.ForceOverheat(ignoreBuffs: true);
                    }
                }

                // modifiers.SourceDamage -= 0.5f;
                modifiers.IncomingDamageMultiplier *= 0.8f;
                //Main.NewText("Iframes: " + BasePrevasionIframes);

                adept.EPCooldownTimer = adept.septima.PrevasionEPCooldownBaseTimer;

                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/Prevasion1") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.5f
                }, Player.Center);

                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/Prevasion2") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.5f
                }, Player.Center);

                return;
            }
        }

        public static Resistance GetAttackResistance(PlayerDeathReason damageSource, SeptimaPlayer adept)
        {
            Resistance resistance = Resistance.None;
            Entity source;

            bool hasInfo = damageSource.TryGetCausingEntity(out source);

            if (hasInfo)
            {
                if (source is NPC npc)
                {
                    if (SeptimaTemplates.NPCDamageResistances[adept.septima.Type].ContainsKey(npc.type))
                    {
                        resistance = SeptimaTemplates.NPCDamageResistances[adept.septima.Type][npc.type];
                    }
                }

                if (source is Projectile projectile)
                {
                    if (SeptimaTemplates.NPCDamageResistances[adept.septima.Type].ContainsKey(projectile.type))
                    {
                        resistance = SeptimaTemplates.NPCDamageResistances[adept.septima.Type][projectile.type];
                    }
                }
            }

            return resistance;
        }

        public override void ResetEffects()
        {
            if (PrevasionIframes > 0) PrevasionIframes--;

            PrevasionLifeLimit = 0;
            PrevasionDamageLimit = 0;
            PrevasionCostAvoidanceChance = 0;
            PrevasionCostModifier = 1;
            base.ResetEffects();
        }
    }
}
