using System;
using UnityEngine;

namespace Robotech.TBS.Hex
{
    // Axial coordinates (q, r) for pointy-top hexes
    [Serializable]
    public struct HexCoord
    {
        public int q; // column
        public int r; // row

        public HexCoord(int q, int r)
        {
            this.q = q; this.r = r;
        }

        public int S => -q - r;

        public static readonly HexCoord[] Neighbors = new HexCoord[]
        {
            new HexCoord(+1, 0), new HexCoord(+1, -1), new HexCoord(0, -1),
            new HexCoord(-1, 0), new HexCoord(-1, +1), new HexCoord(0, +1)
        };

        public static HexCoord operator +(HexCoord a, HexCoord b) => new HexCoord(a.q + b.q, a.r + b.r);
        public static HexCoord operator -(HexCoord a, HexCoord b) => new HexCoord(a.q - b.q, a.r - b.r);

        public int Distance(HexCoord other)
        {
            var d = this - other;
            return (Mathf.Abs(d.q) + Mathf.Abs(d.r) + Mathf.Abs(d.S)) / 2;
        }

        public override string ToString() => $"({q},{r})";

        public Vector3 ToWorld(float hexSize)
        {
            // Pointy-top axial to world (flat x, z)
            float x = hexSize * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f)/2f * r);
            float z = hexSize * (3f/2f * r);
            return new Vector3(x, 0f, z);
        }
    }
}
