// An A* search algorithm that finds the path towards the real player


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AStar
{
    class Location
    {
        public int X;
        public int Z;
        public int H;
        public int G;
        public int F;
        public Location Parent;
    }

    class Program
    {


        public static int[,] grid = ConvertToWorld.makeMap();
        // 0 = empty space
        // 1 = indestructable box
        // 2 = destructable box
        // 3 = AI
        // 4 = actual player

        static List<Location> path = new List<Location>();


        // simple heuristic that calculates path distance
        static int heuristic(int x, int y, int targetX, int targetY)
        {

            // originally modified the heuristic to account for destructable
            // boxes but did not get to actually implementing bomb placement
            // so it was pointless
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);

        }


        public static Location ConvertFromWorld(Vector3 position)
        {
            float locX = position.x;
            float locZ = position.z;

            Location AILocation = new Location { X = ((int)locX - 1) / 2, Z = Math.Abs(((int)locZ + 1) / 2) };

            return AILocation;
        }


        public static List<Location> Search( Location PlayerLocation)
        {


            Location newLocation = ConvertFromWorld(ConvertToWorld.AIposition);

            Location current = null;
            var openList = new List<Location>();
            var closedList = new List<Location>();
            int g = 0;

            openList.Add(newLocation);

            while (openList.Count > 0)
            {

                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);
                // returns first element of list

                closedList.Add(current);
                openList.Remove(current);


                // have found a path so can end the loop
                if (closedList.FirstOrDefault(l => l.X == PlayerLocation.X && l.Z == PlayerLocation.Z) != null)
                {
                    // returns first element of list of default value
                    break;
                }

                var adjacentSquares = GetAdjacentSquares(current.X, current.Z, grid);

                g++;

                foreach (var adj in adjacentSquares)
                {
                    // already in closed list, so can ignore it
                    if (closedList.FirstOrDefault(l => l.X == adj.X && l.Z == adj.Z) != null)
                    {
                        continue;
                    }

                    // not in open list
                    if (openList.FirstOrDefault(l => l.X == adj.X && l.Z == adj.Z) == null)
                    {
                        //compute score, reset parent
                        adj.G = g;
                        adj.H = heuristic(adj.X, adj.Z, PlayerLocation.X, PlayerLocation.Z);
                        adj.F = adj.G + adj.H;
                        adj.Parent = current;

                        openList.Insert(0, adj);
                    }
                    else
                    {
                        //this is a check to see if the g score gets better, and updates if it is
                        if (g + adj.H < adj.F)
                        {
                            adj.G = g;
                            adj.F = adj.G + adj.H;
                            adj.Parent = current;
                        }
                    }
                }
            }

            //path has been found, put the locations in a list
            while (current != null)
            {
                path.Add(current);
                current = current.Parent;
            }
            path.Reverse();
            return path;
        }


        static List<Location> GetAdjacentSquares(int x, int z, int[,] grid)
        {
            var possibleLocations = new List<Location>()
            {
                new Location { X = x, Z = z - 1},
                new Location { X = x, Z = z + 1},
                new Location { X = x - 1, Z = z},
                new Location { X = x + 1, Z = z},
            };

            return possibleLocations.Where(l => grid[l.X, l.Z] == 0 || grid[l.X, l.Z] == 4).ToList();
        }
    }
}
