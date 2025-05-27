using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;

namespace GvMod.Common
{
    public class MirrorShardDropCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            throw new NotImplementedException();
        }

        public bool CanShowItemDropInUI()
        {
            throw new NotImplementedException();
        }

        public string GetConditionDescription()
        {
            throw new NotImplementedException();
        }
    }
}
