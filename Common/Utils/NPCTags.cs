using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Common.GlobalNPCs;
using GvMod.Common.Players;
using Terraria;

namespace GvMod.Common.Utils
{
    public enum TagType
    {
        AzureStriker, 
        LuminousAvenger, 
        ShadowYakumo, 
        GoldenTrillion
    }
    public struct Tag(int targetIndex, int tagLevel, int timeLeft, int damageTimer)
    {
        public int targetIndex = targetIndex;
        public int tagLevel = tagLevel;
        public int timeLeft = timeLeft;
        public int damageTimer = damageTimer;
    }
    public class NPCTags
    {
        // Count
        public int targetCount { get; private set; } = 0;
        // Indexes
        public List<int> taggedTargets { get; private set; } = new();
        // Tag levels, up to 3
        public List<int> tagLevel { get; private set; } = new();
        // Tag timer, when reaching 0 will be removed
        public List<int> tagTimer { get; private set; } = new();
        // Damage timer, specific time depends on the septima
        public List<int> damageTimer { get; private set; } = new();

        public void Update(SeptimaPlayer adept)
        {
            for (int i = 0; i < targetCount; i++)
            {
                damageTimer[i]--;
                tagTimer[i]--;
                if (tagTimer[i] <= 0)
                {
                    RemoveTag(i);
                    i--;
                    return;
                }

                // Remove if the enemy is not active, it's health is 0 or less, it's immportal (Dummy), or it's
                // friendly, remove tag
                if (!Main.npc[taggedTargets[i]].active || Main.npc[taggedTargets[i]].life <= 0 ||
                    Main.npc[taggedTargets[i]].immortal || Main.npc[taggedTargets[i]].friendly)
                {
                    RemoveTag(i);
                    i--;
                    return;
                }

                NPC target = Main.npc[taggedTargets[i]];
                target.GetGlobalNPC<TagNPC>().tagLevel = tagLevel[i];
                target.GetGlobalNPC<TagNPC>().framesUntilTagReset = 2;
                switch (adept.septima.Type)
                {
                    case Players.Sevenths.SeptimaType.AzureStriker:
                    default:
                        target.GetGlobalNPC<TagNPC>().lastTagType = TagType.AzureStriker;
                        break;
                }
            }
        }

        public void RemoveTag(int index)
        {
            taggedTargets.RemoveAt(index);
            tagLevel.RemoveAt(index);
            tagTimer.RemoveAt(index);
            damageTimer.RemoveAt(index);
            targetCount--;
        }

        public void AddTag(int index, int timer = 600)
        {
            for (int i = 0; i < targetCount; i++)
            {
                // If the tagged entity is already registered, increase tag and update timer
                if (taggedTargets[i] == index)
                {
                    if (tagLevel[i] < 3)
                    {
                        tagLevel[i]++;
                    }
                    tagTimer[i] = timer;
                    return;
                }
            }

            // Else, add a new tag
            taggedTargets.Add(index);
            tagLevel.Add(1);
            tagTimer.Add(timer);
            damageTimer.Add(0);
            targetCount++;
        }

        public Tag GetTag(int index)
        {
            for (int i = 0; i < targetCount; i++)
            {
                // If the tagged entity is already registered, increase tag and update timer
                if (taggedTargets[i] == index)
                {
                    return new Tag(taggedTargets[i], tagLevel[i], tagTimer[i], damageTimer[i]);
                }
            }
            return new Tag(-1, -1, -1, -1);
        }
    }
}
