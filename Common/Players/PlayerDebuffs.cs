using GvMod.Content.Buffs;
using Terraria.ModLoader;

namespace GvMod.Common.Players
{
    public class PlayerDebuffs : ModPlayer
    {
        public bool chaffed = false;
        public bool soulPetrified = false;
        public int soulPetrifiedImmunity = 0;

        bool oldControlLeft = false;
        bool oldControlRight = false;

        public override void PreUpdate()
        {
            if (soulPetrified)
            {
                Player.stoned = true;
                Player.statDefense *= 0.5f;

                soulPetrifiedImmunity = 180;
                int index = Player.FindBuffIndex(ModContent.BuffType<SoulPetrification>());

                if (Player.controlLeft && !oldControlLeft)
                {
                    Player.buffTime[index] -= 10;
                }
                if (Player.controlRight && !oldControlRight)
                {
                    Player.buffTime[index] -= 10;
                }
            }
        }

        public override void ResetEffects()
        {
            chaffed = false;
            soulPetrified = false;

            oldControlLeft = Player.controlLeft;
            oldControlRight = Player.controlRight;

            if (soulPetrifiedImmunity > 0) soulPetrifiedImmunity--;
        }
    }
}
