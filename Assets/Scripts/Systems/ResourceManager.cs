using UnityEngine;

namespace Robotech.TBS.Systems
{
    public class ResourceManager : MonoBehaviour
    {
        public int protoculture = 10;
        public int materials = 20;
        public int credits = 50;
        public int science = 0;

        public void ApplyUpkeep(int protoCost)
        {
            protoculture -= protoCost;
            if (protoculture < 0) protoculture = 0; // TODO: deficit penalties
        }

        public void AddTurnYields(int prod, int sci, int infl)
        {
            materials += prod;
            science += sci;
            // influence would drive border growth (not yet implemented)
        }
    }
}
