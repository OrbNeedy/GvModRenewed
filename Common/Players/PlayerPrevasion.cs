using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using GvMod.Common.Players.Sevenths;

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

        public override bool FreeDodge(Player.HurtInfo info)
        {
            //BasePrevasionIframes = 45;
            // Note: Activating prevasion also causes tags on the enemy to disappear
            // CCed will bypass all forms of prevasion, for balance with other mods 
            // Overheat will also prevent prevasion from happening
            SeptimaPlayer adept = Player.GetModPlayer<SeptimaPlayer>();
            if (Player.CCed || adept.Overheated || !info.Dodgeable ||
                info.CooldownCounter == ImmunityCooldownID.DD2OgreKnockback || 
                !adept.septima.AllowPrevasion) return false;

            Resistance resistance = Resistance.None;
            Entity source;
            
            bool hasInfo = info.DamageSource.TryGetCausingEntity(out source);

            if (hasInfo)
            {
                if (source is NPC npc)
                {
                    if (adept.septima.NPCDamageResistances.ContainsKey(npc.type))
                    {
                        resistance = adept.septima.NPCDamageResistances[npc.type];
                    }
                }

                if (source is Projectile projectile)
                {
                    if (adept.septima.ProjectileDamageResistances.ContainsKey(projectile.type))
                    {
                        resistance = adept.septima.NPCDamageResistances[projectile.type];
                    }
                }
            }

            // All resistances have a special interaction that needs the player to get hurt, so 
            // anything that is not neutral will ignore prevasion.
            if (resistance != Resistance.None)
            {
                return base.FreeDodge(info);
            }

            int limit = (int)(Player.statLifeMax2 * PrevasionLifeLimit) + PrevasionDamageLimit;

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

                return true;
            }
            //Main.NewText("Prevasion ignored");

            return base.FreeDodge(info);
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
