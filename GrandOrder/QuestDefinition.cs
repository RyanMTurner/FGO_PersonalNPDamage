using System;
using System.Collections.Generic;
using System.Text;

namespace GrandOrder {
    public class QuestDefinition {
        public List<ReleaseCondition> releaseConditions;
    }

    public class ReleaseCondition {
        public string type;
        public int value;
    }
}
