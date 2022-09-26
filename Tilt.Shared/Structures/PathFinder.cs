using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Entities;

namespace Tilt.EntityComponent.Structures
{
    public class PathFinder
    {
        private static Random mRandom = new Random();
        protected class Node
        {
            private int mX;
            private int mY;
            private Node mParent;
            private double mCost;

            /// F Score
            public Node(int x, int y)
            {
                mX = x;
                mY = y;
            }
            /// G Score
            public double Cost { get { return mCost; } set { mCost = value; } }
            /// H Score
            public int Heuristic { get; set; }

            public int X { get { return mX; } }
            public int Y { get { return mY; } }

            public Node Parent { get { return mParent; } set { mParent = value; } }
        }



        private List<Node> mOpen = new List<Node>(); 
        private List<Node> mClosed = new List<Node>();

        /// returns a path from start tile to an end tile. Set placed parameter to true 
        /// to include placed tile types as blocked
        public List<TileCoord> FindPath(int sx, int sy, int tx, int ty, bool placed = false)
        {
            mOpen.Clear();
            mClosed.Clear();
            Node startNode = new Node(sx, sy);
            startNode.Cost = 0.0;
            startNode.Parent = null;
            mOpen.Add(startNode);

            while (mOpen.Count != 0)
            {
                Node current = mOpen[0];
                if (current.X == tx && current.Y == ty)
                {
                    mClosed.Add(current);
                    break;
                }
                mOpen.Remove(current);
                mClosed.Add(current);

                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        int xp = x + current.X;
                        int yp = y + current.Y;
                        if (!(xp < 0 || yp < 0 || xp > TileMap.Tiles.GetLength(1) - 1 || yp > TileMap.Tiles.GetLength(0) - 1))
                        {
                            //TileNode tileNode = TileMap.GetTileNode(xp, yp);
                            //if (!(xp == TileMap.Base.X && yp == TileMap.Base.Y) && tileNode.Type == TileType.Occupied)
                            //    continue;

                            TileNode tileNode = TileMap.GetTileNode(xp, yp);

                            if (tileNode.Type == TileType.Impassable)
                                continue;



                            double nextStepCost = current.Cost;
                            Node neighbor = new Node(xp, yp);

                            if (nextStepCost < neighbor.Cost)
                            {
                                if (OpenListContainsTile_(neighbor))
                                {
                                    mOpen.Remove(neighbor);

                                }
                                if (ClosedListContainsTile_(neighbor))
                                {
                                    mClosed.Remove(neighbor);
                                }

                            }
                            if (!OpenListContainsTile_(neighbor) && !ClosedListContainsTile_(neighbor))
                            {
                                neighbor.Cost = nextStepCost + (mRandom.NextDouble() * Math.Sqrt(2)); ;
                                neighbor.Heuristic = Math.Abs(tx - xp) + Math.Abs(ty - yp);
                                neighbor.Parent = current;
                                mOpen.Add(neighbor);
                                mOpen = mOpen.OrderBy(n => n.Cost).ToList(); //.ThenBy(n => n.Cost).ToList();
                            }


                        }

                    }
                }

            }

            
            //check the computed path, if the last tile
            //is not the end point, we havent computed the right path
            Node destination = mClosed.Last();
            if (destination.X != tx && destination.Y != ty)
                return null;


            List<Node> finalNodes = new List<Node>();
            mClosed.Reverse();
            Node lastNode = mClosed.First();

            while (lastNode != null)
            {
                finalNodes.Add(lastNode);
                lastNode = lastNode.Parent;
            }

            List<TileCoord> finalVector = new List<TileCoord>();
            foreach (Node node in finalNodes)
            {
                finalVector.Add(new TileCoord() {X = node.X, Y =  node.Y});
            }
            
            finalVector.Reverse();
            finalVector.RemoveAt(0);
            return finalVector;
        }

        private bool OpenListContainsTile_(Node node)
        {
            return mOpen.FirstOrDefault(t => t.X == node.X && t.Y == node.Y) != null;
        }

        private bool ClosedListContainsTile_(Node node)
        {
            return mClosed.FirstOrDefault(t => t.X == node.X && t.Y == node.Y) != null;
        }

    }
}
