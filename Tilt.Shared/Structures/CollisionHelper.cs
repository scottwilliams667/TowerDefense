using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.CSharp;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Structures
{
    /*
     * The CollisionHelper is responsible for partitioning the grid into subsections and grouping 
     * together objects that are in the same partition. As each object is bounds checked, it is placed
     * into a "bin" or partition along with other objects that are in the same partition.
     * 
     * In order to figure out what bins a object is in, you call GetCells on the bounds of that particular 
     * object. Once all the objects have been sorted into partitions, we can then use the GetNearby method
     * to return all objects within that bin.
     * 
     * 
     * 
     * Each Bin is seperated into a 256x256 pixel area. Bins indices are ordered as follows:
     * |  0 |  1 |  2  | 3 | 4 | 5 | 6 | 7 | 8 | 9 |
     * | 10 | 11 | 12 ...
     * ...
     * | 50 | 51 ....                     ... | 59
     * |
     */
    public static class CollisionHelper
    {
        private static Dictionary<int, List<CollisionComponent>> mBins = new Dictionary<int, List<CollisionComponent>>();
        private static int kQuadsInRow = 10; //(2560/128 for 32 sized
        private static int kQuadSize = 256;

        public static List<int> Register(CollisionComponent component)
        {
            List<int> cells = new List<int>();
            if(component is BoundsCollisionComponent)
            {
                BoundsCollisionComponent boundsCollision = component as BoundsCollisionComponent;
                cells = GetCells(boundsCollision);
            } 
            else
            {
                PointCollisionComponent pointCollision = component as PointCollisionComponent;
                cells = GetCells(pointCollision);
            }
            foreach (int cell in cells)
            {
                if (!mBins.ContainsKey(cell))
                    mBins.Add(cell, new List<CollisionComponent>());
                mBins[cell].Add(component);
            }
            return cells;
        }

        public static void ClearCells()
        {
            mBins = new Dictionary<int, List<CollisionComponent>>();
        }

        public static List<CollisionComponent> GetNearby(int bin)
        {
            List<CollisionComponent> components;
            mBins.TryGetValue(bin, out components);
            return components ?? new List<CollisionComponent>();
        }

        public static List<int> GetSurroundingCells(int cell)
        {
            return new List<int>()
            {
                //current cell
                cell,
                //cell to left
                cell - 1,
                //cell to right
                cell + 1,
                //cell below
                cell + kQuadsInRow,
                //cell above
                cell - kQuadsInRow,
                //diagonals
                //bottom right
                cell + kQuadsInRow + 1,
                //bottom left
                cell + kQuadsInRow - 1,
                //top right
                cell - kQuadsInRow + 1,
                //top left
                cell - kQuadsInRow - 1

                
            };
        }

        public static List<int> GetCells(PointCollisionComponent collisionComponent)
        {

            Vector2 point1 = collisionComponent.Point1;
            Vector2 point2 = collisionComponent.Point2;
            Vector2 point3 = collisionComponent.Point3;
            Vector2 point4 = collisionComponent.Point4;

            List<int> cells = new List<int>();

            int quadSize = kQuadSize;

            Vector2 point1snappedQuad = new Vector2((float)Math.Floor((double)(point1.X / quadSize)), (float)Math.Floor((double)(point1.Y / quadSize)));
            Vector2 point2snappedQuad = new Vector2((float)Math.Floor((double)(point2.X / quadSize)), (float)Math.Floor((double)(point2.Y / quadSize)));
            Vector2 point3snappedQuad = new Vector2((float)Math.Floor((double)(point3.X / quadSize)), (float)Math.Floor((double)(point3.Y / quadSize)));
            Vector2 point4snappedQuad = new Vector2((float)Math.Floor((double)(point4.X / quadSize)), (float)Math.Floor((double)(point4.Y / quadSize)));

            int point1QuadIndex = (int)((point1snappedQuad.Y * kQuadsInRow + point1snappedQuad.X));
            int point2QuadIndex = (int)((point2snappedQuad.Y * kQuadsInRow + point2snappedQuad.X));
            int point3QuadIndex = (int)((point3snappedQuad.Y * kQuadsInRow + point3snappedQuad.X));
            int point4QuadIndex = (int)((point4snappedQuad.Y * kQuadsInRow + point4snappedQuad.X));

            if(!cells.Contains(point1QuadIndex))
                cells.Add(point1QuadIndex);
            if (!cells.Contains(point2QuadIndex))
                cells.Add(point2QuadIndex);
            if (!cells.Contains(point3QuadIndex))
                cells.Add(point3QuadIndex);
            if (!cells.Contains(point4QuadIndex))
                cells.Add(point4QuadIndex); 

            return cells;
        }

        public static List<int> GetCells(BoundsCollisionComponent collisionComponent)
        {
            Rectangle bounds = collisionComponent.Bounds;
            return GetCells(bounds);
        }

        public static List<int> GetCells(Rectangle bounds)
        {
            List<int> cells = new List<int>();
            int quadSize = kQuadSize;
            Vector2 quad = new Vector2((float)Math.Floor((double)(bounds.X / quadSize)), (float)Math.Floor((double)(bounds.Y / quadSize)));
            Rectangle currentQuad = new Rectangle((int)(quad.X * quadSize), (int)(quad.Y * quadSize), quadSize, quadSize);

            int quadIndex = (int)(quad.Y * kQuadsInRow + quad.X);
            if (currentQuad.Intersects(bounds))
            {
                cells.Add(quadIndex);
            }
            int topQuadIndex = (int)((quad.Y - 1) * kQuadsInRow + quad.X);
            if (topQuadIndex >= 0)
            {
                Rectangle topQuad = new Rectangle((int)(quad.X * quadSize), (int)(((quad.Y - 1) * kQuadsInRow) * quadSize), quadSize, quadSize);
                if (topQuad.Intersects(bounds))
                {
                    cells.Add(topQuadIndex);
                }
            }
            int bottomQuadIndex = (int)((quad.Y + 1) * kQuadsInRow + quad.X);
            if (bottomQuadIndex < TileMap.Height)
            {
                Rectangle bottomQuad = new Rectangle((int)(quad.X * quadSize), (int)((quad.Y + 1) * quadSize), quadSize, quadSize);
                if (bottomQuad.Intersects(bounds))
                {
                    cells.Add(bottomQuadIndex);
                }
            }
            int leftQuadIndex = (int)((quad.X - 1) + quad.Y * kQuadsInRow);
            int leftMost = (int)(quad.Y * kQuadsInRow);
            if (leftQuadIndex >= leftMost)
            {
                Rectangle leftQuad = new Rectangle((int)((quad.X - 1) * quadSize), (int)(quad.Y * quadSize), quadSize, quadSize);
                if (leftQuad.Intersects(bounds))
                {
                    cells.Add(leftQuadIndex);
                }
            }
            int rightQuadIndex = (int)((quad.X + 1) + quad.Y * kQuadsInRow);
            int rightMost = (int)((quad.Y + 1) * kQuadsInRow);
            if (rightQuadIndex < rightMost)
            {
                Rectangle rightQuad = new Rectangle((int)((quad.X + 1) * quadSize), (int)(quad.Y * quadSize), quadSize, quadSize);
                if (rightQuad.Intersects(bounds))
                {
                    cells.Add(rightQuadIndex);
                }
            }


            int topLeftQuadIndex = (int)((quad.X - 1) + (quad.Y - 1) * kQuadsInRow);
            leftMost = (int)((quad.Y - 1) * kQuadsInRow);
            if (topLeftQuadIndex >= 0 && topLeftQuadIndex >= leftMost)
            {
                Rectangle topLeftQuad = new Rectangle((int)((quad.X - 1) * quadSize), (int)((quad.Y - 1) * quadSize), quadSize, quadSize);
                if (topLeftQuad.Intersects(bounds))
                {
                    cells.Add(topLeftQuadIndex);
                }
            }
            int bottomLeftQuadIndex = (int)((quad.X - 1) + (quad.Y + 1) * kQuadsInRow);
            leftMost = (int)((quad.Y + 1) * kQuadsInRow);
            if (bottomLeftQuadIndex < TileMap.Height && bottomLeftQuadIndex >= leftMost)
            {
                Rectangle bottomLeftQuad = new Rectangle((int)((quad.X - 1) * quadSize), (int)((quad.Y + 1) * quadSize), quadSize, quadSize);
                if (bottomLeftQuad.Intersects(bounds))
                {
                    cells.Add(bottomLeftQuadIndex);
                }
            }
            int topRightQuadIndex = (int)((quad.X + 1) + (quad.Y - 1) * kQuadsInRow);
            rightMost = (int)(quad.Y) * kQuadsInRow;
            if (topRightQuadIndex >= 0 && topRightQuadIndex < rightMost)
            {
                Rectangle topRightQuad = new Rectangle((int)((quad.X + 1) * quadSize), (int)((quad.Y - 1) * quadSize), quadSize, quadSize);
                if (topRightQuad.Intersects(bounds))
                {
                    cells.Add(topRightQuadIndex);
                }
            }
            int bottomRightQuadIndex = (int)((quad.X + 1) + (quad.Y + 1) * kQuadsInRow);
            rightMost = (int)(quad.Y + 1) * kQuadsInRow;
            if (bottomRightQuadIndex < TileMap.Height && bottomRightQuadIndex > rightMost)
            {
                Rectangle bottomRightQuad = new Rectangle((int)((quad.X + 1) * quadSize), (int)((quad.Y + 1) * quadSize), quadSize, quadSize);
                if (bottomRightQuad.Intersects(bounds))
                {
                    cells.Add(bottomRightQuadIndex);
                }
            }




            return cells;
        }
    }
}
