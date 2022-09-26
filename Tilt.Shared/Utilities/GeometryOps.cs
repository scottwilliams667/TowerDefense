using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Utilities
{
    public static class GeometryOps
    {
        public static double Vector2Angle(Vector2 vector)
        {
            Vector2 normalizedVector = Vector2.Normalize(vector);
            return Math.Atan2(normalizedVector.Y, normalizedVector.X);
        }

        public static Vector2 Angle2Vector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static double AngleBetweenTwoVectors(Vector2 vector1, Vector2 vector2)
        {
            Vector2 delta = new Vector2(vector1.X - vector2.X, vector1.Y - vector2.Y);
            return Math.Atan2(delta.Y, delta.X);
        }

        public static Vector2 PointToVector2(Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static Point Vector2ToPoint(Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        public static bool IsWithinDistance(Vector2 first, Vector2 second, double distance)
        {
            double dist = Math.Sqrt(Math.Pow((first.X - second.X), 2) + Math.Pow((first.Y - second.Y), 2));
            return dist < distance;
        }

        public static TileCoord PositionToTileCoord(Vector2 position)
        {
            return new TileCoord()
            {
                X = (int)Math.Floor((position.X - Tuner.MapStartPosX) / TileMap.TileWidth),
                Y = (int)Math.Floor((position.Y - Tuner.MapStartPosY) / TileMap.TileHeight)
            };
        }

        public static Vector2 TileCoordToPosition(TileCoord tileCoord)
        {
            return new Vector2(Tuner.MapStartPosX + (tileCoord.X * TileMap.TileWidth), Tuner.MapStartPosY + (tileCoord.Y * TileMap.TileHeight));
        }

        public static bool Intersects(Vector2[] points, Rectangle rectangle2)
        {
            Vector2 topLeft = points[0];
            Vector2 topRight = points[1];
            Vector2 bottomLeft = points[2];
            Vector2 bottomRight = points[3];

            Vector2 rTopLeft = new Vector2(rectangle2.X, rectangle2.Y);
            Vector2 rTopRight = new Vector2(rectangle2.X + rectangle2.Width, rectangle2.Y);
            Vector2 rBottomLeft = new Vector2(rectangle2.X, rectangle2.Y + rectangle2.Height);
            Vector2 rbottomRight = new Vector2(rectangle2.X + rectangle2.Width, rectangle2.Y + rectangle2.Height);


            Vector2 axis1 = topRight - topLeft;
            Vector2 axis2 = topRight - bottomRight;
            Vector2 axis3 = rTopLeft - rBottomLeft;
            Vector2 axis4 = rTopLeft - rTopRight;

            List<Vector2> axes = new List<Vector2>() { axis1, axis2, axis3, axis4 };

            foreach (Vector2 axis in axes)
            {
                Vector2 aTopLeft = ProjectOnAxis_(topLeft, axis);
                Vector2 aTopRight = ProjectOnAxis_(topRight, axis);
                Vector2 aBottomLeft = ProjectOnAxis_(bottomLeft, axis);
                Vector2 aBottomRight = ProjectOnAxis_(bottomRight, axis);

                Vector2 bTopLeft = ProjectOnAxis_(rTopLeft, axis);
                Vector2 bTopRight = ProjectOnAxis_(rTopRight, axis);
                Vector2 bBottomLeft = ProjectOnAxis_(rBottomLeft, axis);
                Vector2 bBottomRight = ProjectOnAxis_(rbottomRight, axis);

                List<int> aPts = new List<int>();
                aPts.Add(GetScalar_(aTopLeft, topLeft));
                aPts.Add(GetScalar_(aTopRight, topRight));
                aPts.Add(GetScalar_(aBottomLeft, bottomLeft));
                aPts.Add(GetScalar_(aBottomRight, bottomRight));

                List<int> bPts = new List<int>();
                bPts.Add(GetScalar_(bTopLeft, rTopLeft));
                bPts.Add(GetScalar_(bTopRight, rTopRight));
                bPts.Add(GetScalar_(bBottomLeft, rBottomLeft));
                bPts.Add(GetScalar_(bBottomRight, rbottomRight));

                int aMin = aPts.Min();
                int aMax = aPts.Max();

                int bMin = bPts.Min();
                int bMax = bPts.Max();

                if (bMin >= aMax || aMin >= bMax)
                    return false;


            }


            return true;
        }

        private static int GetScalar_(Vector2 projection, Vector2 vector)
        {
            return (int)(projection.X * vector.X) + (int)(projection.Y + vector.Y);
        }

        private static Vector2 ProjectOnAxis_(Vector2 vector, Vector2 axis)
        {
            double projection = (vector.X * axis.X + vector.Y * axis.Y) / (Math.Pow(axis.X, 2) + Math.Pow(axis.Y, 2));

            return new Vector2((float)(projection * axis.X), (float)(projection * axis.Y));
        }

        public static Vector2 RotateAroundPoint(Vector2 vectorToRotate, Vector2 rotationVector, float angle)
        {
            Matrix transform = Matrix.CreateTranslation(-new Vector3(rotationVector, 0)) * Matrix.CreateRotationZ(angle) *
                               Matrix.CreateTranslation(new Vector3(rotationVector, 0));

            Vector3 rotated = Vector3.Transform(new Vector3(vectorToRotate, 0), transform);

            return new Vector2(rotated.X, rotated.Y);
        }

        public static bool Intersects(Vector2[] points1, Vector2[] poinst2)
        {

            return false;
        }

        public static bool Intersects(Rectangle rectangle1, Rectangle rectangle2)
        {
            return rectangle1.Intersects(rectangle2);
        }

        public static bool TryGetValue<T>(this List<T> list, int index, ref T value)
        {
            if (index < list.Count)
            {
                value = list.ElementAt(index);
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
            
        }
    }
}
