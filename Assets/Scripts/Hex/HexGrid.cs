using System.Collections.Generic;
using UnityEngine;

namespace Robotech.TBS.Hex
{
    public class HexGrid : MonoBehaviour
    {
        [Header("Grid")]
        public int width = 40;
        public int height = 24;
        public float hexSize = 1.0f;

        private Dictionary<(int,int), HexCoord> coords = new();

        public IEnumerable<HexCoord> AllCoords()
        {
            for (int r = 0; r < height; r++)
            {
                int qOffset = Mathf.FloorToInt(r / 2f);
                for (int q = -qOffset; q < width - qOffset; q++)
                    yield return new HexCoord(q, r);
            }
        }

        public bool InBounds(HexCoord c)
        {
            // Simple bounds check using row-based offset limits
            int qOffset = Mathf.FloorToInt(c.r / 2f);
            return c.r >= 0 && c.r < height && c.q >= -qOffset && c.q < width - qOffset;
        }

        public Vector3 CoordToWorld(HexCoord c) => c.ToWorld(hexSize);

        public IEnumerable<HexCoord> Neighbors(HexCoord c)
        {
            foreach (var d in HexCoord.Neighbors)
            {
                var n = c + d;
                if (InBounds(n)) yield return n;
            }
        }

        public List<HexCoord> Range(HexCoord center, int range)
        {
            var result = new List<HexCoord>();
            for (int dq = -range; dq <= range; dq++)
            {
                for (int dr = Mathf.Max(-range, -dq - range); dr <= Mathf.Min(range, -dq + range); dr++)
                {
                    var c = new HexCoord(center.q + dq, center.r + dr);
                    if (InBounds(c)) result.Add(c);
                }
            }
            return result;
        }
    }
}
