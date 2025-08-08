using UnityEngine;
using Robotech.TBS.Core;

namespace Robotech.TBS.Debugging
{
    public class DevHotkeys : MonoBehaviour
    {
        public KeyCode endTurnKey = KeyCode.Return; // press Enter to end turn
        void Update()
        {
            if (Input.GetKeyDown(endTurnKey))
            {
                var tm = FindObjectOfType<TurnManager>();
                if (tm != null) tm.EndPhase(); // progresses through AI and starts next turn
            }
        }
    }
}
