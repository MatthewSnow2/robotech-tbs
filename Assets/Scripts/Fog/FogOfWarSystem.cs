using System.Collections.Generic;
using UnityEngine;
using Robotech.TBS.Hex;

namespace Robotech.TBS.Fog
{
    public class FogOfWarSystem : MonoBehaviour
    {
        public HexGrid grid;
        // Seen: tiles that have been revealed at least once; Visible: currently visible
        private HashSet<(int,int)> seen = new();
        private HashSet<(int,int)> visible = new();

        public bool IsSeen(HexCoord c) => seen.Contains((c.q, c.r));
        public bool IsVisible(HexCoord c) => visible.Contains((c.q, c.r));

        public void ClearVisibility()
        {
            visible.Clear();
        }

        public void RevealFrom(HexCoord source, int visionRadius)
        {
            foreach (var c in grid.Range(source, visionRadius))
            {
                seen.Add((c.q, c.r));
                visible.Add((c.q, c.r));
            }
        }
    }
}
