using System.Collections;
using System.Collections.Generic;

namespace Zenko.Entities
{
    public class Condition
    {
        public ConditionType type;
        public int amount;
        public Condition(ConditionType type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }
}
