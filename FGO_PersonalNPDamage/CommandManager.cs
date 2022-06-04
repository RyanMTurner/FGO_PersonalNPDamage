using GrandOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FGO_PersonalNPDamage {
    public class CommandManager {
        AtlasClient client = new AtlasClient();
        List<ServantDefinition> servantDefinitions;

        public CommandManager() {
            Task.Run(async () => { servantDefinitions = await client.GetAllServants(); servantDefinitions.RemoveUnplayable(); });
        }
    }
}
