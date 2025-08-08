using UnityEngine;

namespace Robotech.TBS.Hex
{
    public static class HexMath
    {
        // Pointy-top axial rounding
        public static HexCoord AxialFromWorld(Vector3 world, float hexSize)
        {
            float qf = (Mathf.Sqrt(3f)/3f * world.x - 1f/3f * world.z) / hexSize;
            float rf = (2f/3f * world.z) / hexSize;
            return CubeRound(qf, rf);
        }

        private static HexCoord CubeRound(float q, float r)
        {
            float x = q;
            float z = r;
            float y = -x - z;

            int rx = Mathf.RoundToInt(x);
            int ry = Mathf.RoundToInt(y);
            int rz = Mathf.RoundToInt(z);

            float x_diff = Mathf.Abs(rx - x);
            float y_diff = Mathf.Abs(ry - y);
            float z_diff = Mathf.Abs(rz - z);

            if (x_diff > y_diff && x_diff > z_diff)
                rx = -ry - rz;
            else if (y_diff > z_diff)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new HexCoord(rx, rz);
        }
    }
}
