using System.Collections.Generic;

namespace GrandOrder {
    public class HTTPSniffResponse {
        public FGOCache cache;
    }

    public class FGOCache {
        public FGOCacheReplace replaced;
    }

    public class FGOCacheReplace {
        public List<ServantInstance> userSvt;

        public ServantInstance GetServantByID(int svtId) {
            if (userSvt == null) {
                return null;
            }
            foreach (ServantInstance servantInstance in userSvt) {
                if (servantInstance.svtId == svtId) {
                    return servantInstance;
                }
            }
            return null;
        }
    }

    public class ServantInstance {
        public int svtId;

        public int atk;
        public int lv;

        public int adjustAtk; //This is the fou value, divided by 10 for some reason

        public int skillLv1;
        public int skillLv2;
        public int skillLv3;
        public int treasureDeviceLv1;

        public override string ToString() {
            return $"Lv. {lv}, ATK {atk} (includes {adjustAtk * 10} from fous), NP {treasureDeviceLv1}, Skills {skillLv1} {skillLv2} {skillLv3}";
        }
    }
}
