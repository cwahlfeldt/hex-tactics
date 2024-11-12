using Godot;
using System.Collections.Generic;

namespace HexTactics.Core
{
    public static class HexGrid
    {
        public static readonly Dictionary<string, Vector3I> Directions = new()
        {
            { "NorthWest", new Vector3I(-1, 0, 1) },
            { "North", new Vector3I(0, -1, 1) },
            { "NorthEast", new Vector3I(1, -1, 0) },
            { "SouthWest", new Vector3I(-1, 1, 0) },
            { "South", new Vector3I(0, 1, -1) },
            { "SouthEast", new Vector3I(1, 0, -1) }
        };

        public static Vector3 HexToWorld(Vector3I hexCoord, float hexSize)
        {
            float x = hexSize * (1.5f * hexCoord.X);
            float z = hexSize * (Mathf.Sqrt(3.0f) * (hexCoord.Y + hexCoord.X * 0.5f));
            return new Vector3(x, 0, z);
        }

        public static int GetDistance(Vector3I a, Vector3I b)
        {
            var diff = a - b;
            return (Mathf.Abs(diff.X) + Mathf.Abs(diff.Y) + Mathf.Abs(diff.Z)) / 2;
        }

        public static List<Vector3I> GenerateHexCoordinates(int mapSize)
        {
            var coords = new List<Vector3I>();
            
            for (int q = -mapSize; q <= mapSize; q++)
            {
                int r1 = Mathf.Max(-mapSize, -q - mapSize);
                int r2 = Mathf.Min(mapSize, -q + mapSize);
                
                for (int r = r1; r <= r2; r++)
                {
                    int s = -q - r;
                    coords.Add(new Vector3I(q, r, s));
                }
            }
            
            coords.Sort((a, b) =>
            {
                if (a.Y != b.Y)
                    return b.Y.CompareTo(a.Y);
                return b.X.CompareTo(a.X);
            });
            
            return coords;
        }

        public static List<Vector3I> GetNeighbors(Vector3I center)
        {
            var neighbors = new List<Vector3I>();
            foreach (var dir in Directions.Values)
            {
                neighbors.Add(center + dir);
            }
            return neighbors;
        }

        public static List<Vector3I> GetRing(Vector3I center, int radius)
        {
            var results = new List<Vector3I>();
            if (radius <= 0) return results;

            // Start at the top-right hex of the ring
            var hex = center + new Vector3I(radius, -radius, 0);
            
            // Move in each of the 6 directions
            foreach (var direction in Directions.Values)
            {
                // Move radius times in each direction
                for (int i = 0; i < radius; i++)
                {
                    results.Add(hex);
                    hex += direction;
                }
            }
            
            return results;
        }
    }
}