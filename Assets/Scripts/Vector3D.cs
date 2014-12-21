using System;
using UnityEngine;

namespace FarseerPhysics
{
    public struct Vector3D : IEquatable<Vector3D>
    {
        private static Vector3D _zero = new Vector3D();
        private static Vector3D _one = new Vector3D(1f, 1f, 1f);
        private static Vector3D _unitX = new Vector3D(1f, 0.0f, 0.0f);
        private static Vector3D _unitY = new Vector3D(0.0f, 1f, 0.0f);
        private static Vector3D _unitZ = new Vector3D(0.0f, 0.0f, 1f);
        private static Vector3D _up = new Vector3D(0.0f, 1f, 0.0f);
        private static Vector3D _down = new Vector3D(0.0f, -1f, 0.0f);
        private static Vector3D _right = new Vector3D(1f, 0.0f, 0.0f);
        private static Vector3D _left = new Vector3D(-1f, 0.0f, 0.0f);
        private static Vector3D _forward = new Vector3D(0.0f, 0.0f, -1f);
        private static Vector3D _backward = new Vector3D(0.0f, 0.0f, 1f);
        /// <summary>
        /// Gets or sets the x-component of the vector.
        /// </summary>
        public double X;
        /// <summary>
        /// Gets or sets the y-component of the vector.
        /// </summary>
        public double Y;
        /// <summary>
        /// Gets or sets the z-component of the vector.
        /// </summary>
        public double Z;

        public Vector3 ToVector3()
        {
            return new Vector3((float)X, (float)Y, (float)Z);
        }

        /// <summary>
        /// Returns a Vector3D with all of its components set to zero.
        /// </summary>
        public static Vector3D Zero
        {
            get
            {
                return Vector3D._zero;
            }
        }

        /// <summary>
        /// Returns a Vector3D with ones in all of its components.
        /// </summary>
        public static Vector3D One
        {
            get
            {
                return Vector3D._one;
            }
        }

        /// <summary>
        /// Returns the x unit Vector3D (1, 0, 0).
        /// </summary>
        public static Vector3D UnitX
        {
            get
            {
                return Vector3D._unitX;
            }
        }

        /// <summary>
        /// Returns the y unit Vector3D (0, 1, 0).
        /// </summary>
        public static Vector3D UnitY
        {
            get
            {
                return Vector3D._unitY;
            }
        }

        /// <summary>
        /// Returns the z unit Vector3D (0, 0, 1).
        /// </summary>
        public static Vector3D UnitZ
        {
            get
            {
                return Vector3D._unitZ;
            }
        }

        /// <summary>
        /// Returns a unit vector designating up (0, 1, 0).
        /// </summary>
        public static Vector3D Up
        {
            get
            {
                return Vector3D._up;
            }
        }

        /// <summary>
        /// Returns a unit Vector3D designating down (0, −1, 0).
        /// </summary>
        public static Vector3D Down
        {
            get
            {
                return Vector3D._down;
            }
        }

        /// <summary>
        /// Returns a unit Vector3D pointing to the right (1, 0, 0).
        /// </summary>
        public static Vector3D Right
        {
            get
            {
                return Vector3D._right;
            }
        }

        /// <summary>
        /// Returns a unit Vector3D designating left (−1, 0, 0).
        /// </summary>
        public static Vector3D Left
        {
            get
            {
                return Vector3D._left;
            }
        }

        /// <summary>
        /// Returns a unit Vector3D designating forward in a right-handed coordinate system(0, 0, −1).
        /// </summary>
        public static Vector3D Forward
        {
            get
            {
                return Vector3D._forward;
            }
        }

        /// <summary>
        /// Returns a unit Vector3D designating backward in a right-handed coordinate system (0, 0, 1).
        /// </summary>
        public static Vector3D Backward
        {
            get
            {
                return Vector3D._backward;
            }
        }

        static Vector3D()
        {
        }

        /// <summary>
        /// Initializes a new instance of Vector3D.
        /// </summary>
        /// <param name="x">Initial value for the x-component of the vector.</param><param name="y">Initial value for the y-component of the vector.</param><param name="z">Initial value for the z-component of the vector.</param>
        public Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Creates a new instance of Vector3D.
        /// </summary>
        /// <param name="value">Value to initialize each component to.</param>
        public Vector3D(double value)
        {
            this.X = this.Y = this.Z = value;
        }


        /// <summary>
        /// Returns a vector pointing in the opposite direction.
        /// </summary>
        /// <param name="value">Source vector.</param>
        public static Vector3D operator -(Vector3D value)
        {
            Vector3D Vector3D;
            Vector3D.X = -value.X;
            Vector3D.Y = -value.Y;
            Vector3D.Z = -value.Z;
            return Vector3D;
        }

        /// <summary>
        /// Tests vectors for equality.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static bool operator ==(Vector3D value1, Vector3D value2)
        {
            if (value1.X == value2.X && value1.Y == value2.Y)
                return value1.Z == value2.Z;
            else
                return false;
        }

        /// <summary>
        /// Tests vectors for inequality.
        /// </summary>
        /// <param name="value1">Vector to compare.</param><param name="value2">Vector to compare.</param>
        public static bool operator !=(Vector3D value1, Vector3D value2)
        {
            if (value1.X == value2.X && value1.Y == value2.Y)
                return value1.Z != value2.Z;
            else
                return true;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3D operator +(Vector3D value1, Vector3D value2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X + value2.X;
            Vector3D.Y = value1.Y + value2.Y;
            Vector3D.Z = value1.Z + value2.Z;
            return Vector3D;
        }

        /// <summary>
        /// Subtracts a vector from a vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3D operator -(Vector3D value1, Vector3D value2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X - value2.X;
            Vector3D.Y = value1.Y - value2.Y;
            Vector3D.Z = value1.Z - value2.Z;
            return Vector3D;
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3D operator *(Vector3D value1, Vector3D value2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X * value2.X;
            Vector3D.Y = value1.Y * value2.Y;
            Vector3D.Z = value1.Z * value2.Z;
            return Vector3D;
        }

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector.</param><param name="scaleFactor">Scalar value.</param>
        public static Vector3D operator *(Vector3D value, double scaleFactor)
        {
            Vector3D Vector3D;
            Vector3D.X = value.X * scaleFactor;
            Vector3D.Y = value.Y * scaleFactor;
            Vector3D.Z = value.Z * scaleFactor;
            return Vector3D;
        }

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="scaleFactor">Scalar value.</param><param name="value">Source vector.</param>
        public static Vector3D operator *(double scaleFactor, Vector3D value)
        {
            Vector3D Vector3D;
            Vector3D.X = value.X * scaleFactor;
            Vector3D.Y = value.Y * scaleFactor;
            Vector3D.Z = value.Z * scaleFactor;
            return Vector3D;
        }

        /// <summary>
        /// Divides the components of a vector by the components of another vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Divisor vector.</param>
        public static Vector3D operator /(Vector3D value1, Vector3D value2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X / value2.X;
            Vector3D.Y = value1.Y / value2.Y;
            Vector3D.Z = value1.Z / value2.Z;
            return Vector3D;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value">Source vector.</param><param name="divider">The divisor.</param>
        public static Vector3D operator /(Vector3D value, double divider)
        {
            double num = 1f / divider;
            Vector3D Vector3D;
            Vector3D.X = value.X * num;
            Vector3D.Y = value.Y * num;
            Vector3D.Z = value.Z * num;
            return Vector3D;
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the Vector3D.
        /// </summary>
        /// <param name="other">The Vector3D to compare with the current Vector3D.</param>
        public bool Equals(Vector3D other)
        {
            if (this.X == other.X && this.Y == other.Y)
                return this.Z == other.Z;
            else
                return false;
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">Object to make the comparison with.</param>
        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is Vector3D)
                flag = this.Equals((Vector3D)obj);
            return flag;
        }

        /// <summary>
        /// Gets the hash code of the vector object.
        /// </summary>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode();
        }

        /// <summary>
        /// Calculates the length of the vector.
        /// </summary>
        public double Length()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
        }

        /// <summary>
        /// Calculates the length of the vector squared.
        /// </summary>
        public double LengthSquared()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static double Distance(Vector3D value1, Vector3D value2)
        {
            double num1 = value1.X - value2.X;
            double num2 = value1.Y - value2.Y;
            double num3 = value1.Z - value2.Z;
            return Math.Sqrt(num1 * num1 + num2 * num2 + num3 * num3);
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The distance between the vectors.</param>
        public static void Distance(ref Vector3D value1, ref Vector3D value2, out double result)
        {
            double num1 = value1.X - value2.X;
            double num2 = value1.Y - value2.Y;
            double num3 = value1.Z - value2.Z;
            double num4 = num1 * num1 + num2 * num2 + num3 * num3;
            result = Math.Sqrt(num4);
        }

        /// <summary>
        /// Calculates the distance between two vectors squared.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static double DistanceSquared(Vector3D value1, Vector3D value2)
        {
            double num1 = value1.X - value2.X;
            double num2 = value1.Y - value2.Y;
            double num3 = value1.Z - value2.Z;
            return num1 * num1 + num2 * num2 + num3 * num3;
        }

        /// <summary>
        /// Calculates the distance between two vectors squared.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The distance between the two vectors squared.</param>
        public static void DistanceSquared(ref Vector3D value1, ref Vector3D value2, out double result)
        {
            double num1 = value1.X - value2.X;
            double num2 = value1.Y - value2.Y;
            double num3 = value1.Z - value2.Z;
            result = num1 * num1 + num2 * num2 + num3 * num3;
        }

        /// <summary>
        /// Calculates the dot product of two vectors. If the two vectors are unit vectors, the dot product returns a doubleing point value between -1 and 1 that can be used to determine some properties of the angle between two vectors. For example, it can show whether the vectors are orthogonal, parallel, or have an acute or obtuse angle between them.
        /// </summary>
        /// <param name="vector1">Source vector.</param><param name="Vector2D">Source vector.</param>
        public static double Dot(Vector3D vector1, Vector3D Vector2D)
        {
            return vector1.X * Vector2D.X + vector1.Y * Vector2D.Y + vector1.Z * Vector2D.Z;
        }

        /// <summary>
        /// Calculates the dot product of two vectors and writes the result to a user-specified variable. If the two vectors are unit vectors, the dot product returns a doubleing point value between -1 and 1 that can be used to determine some properties of the angle between two vectors. For example, it can show whether the vectors are orthogonal, parallel, or have an acute or obtuse angle between them.
        /// </summary>
        /// <param name="vector1">Source vector.</param><param name="Vector2D">Source vector.</param><param name="result">[OutAttribute] The dot product of the two vectors.</param>
        public static void Dot(ref Vector3D vector1, ref Vector3D Vector2D, out double result)
        {
            result = vector1.X * Vector2D.X + vector1.Y * Vector2D.Y + vector1.Z * Vector2D.Z;
        }

        /// <summary>
        /// Turns the current vector into a unit vector. The result is a vector one unit in length pointing in the same direction as the original vector.
        /// </summary>
        public void Normalize()
        {
            double num = 1f / Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            this.X *= num;
            this.Y *= num;
            this.Z *= num;
        }

        /// <summary>
        /// Creates a unit vector from the specified vector. The result is a vector one unit in length pointing in the same direction as the original vector.
        /// </summary>
        /// <param name="value">The source Vector3D.</param>
        public static Vector3D Normalize(Vector3D value)
        {
            double num = 1f / Math.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z);
            Vector3D Vector3D;
            Vector3D.X = value.X * num;
            Vector3D.Y = value.Y * num;
            Vector3D.Z = value.Z * num;
            return Vector3D;
        }

        /// <summary>
        /// Creates a unit vector from the specified vector, writing the result to a user-specified variable. The result is a vector one unit in length pointing in the same direction as the original vector.
        /// </summary>
        /// <param name="value">Source vector.</param><param name="result">[OutAttribute] The normalized vector.</param>
        public static void Normalize(ref Vector3D value, out Vector3D result)
        {
            double num = 1f / Math.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z);
            result.X = value.X * num;
            result.Y = value.Y * num;
            result.Z = value.Z * num;
        }

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="vector1">Source vector.</param><param name="Vector2D">Source vector.</param>
        public static Vector3D Cross(Vector3D vector1, Vector3D Vector2D)
        {
            Vector3D Vector3D;
            Vector3D.X = vector1.Y * Vector2D.Z - vector1.Z * Vector2D.Y;
            Vector3D.Y = vector1.Z * Vector2D.X - vector1.X * Vector2D.Z;
            Vector3D.Z = vector1.X * Vector2D.Y - vector1.Y * Vector2D.X;
            return Vector3D;
        }

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="vector1">Source vector.</param><param name="Vector2D">Source vector.</param><param name="result">[OutAttribute] The cross product of the vectors.</param>
        public static void Cross(ref Vector3D vector1, ref Vector3D Vector2D, out Vector3D result)
        {
            double num1 = vector1.Y * Vector2D.Z - vector1.Z * Vector2D.Y;
            double num2 = vector1.Z * Vector2D.X - vector1.X * Vector2D.Z;
            double num3 = vector1.X * Vector2D.Y - vector1.Y * Vector2D.X;
            result.X = num1;
            result.Y = num2;
            result.Z = num3;
        }

        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal.  Reference page contains code sample.
        /// </summary>
        /// <param name="vector">Source vector.</param><param name="normal">Normal of the surface.</param>
        public static Vector3D Reflect(Vector3D vector, Vector3D normal)
        {
            double num = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;
            Vector3D Vector3D;
            Vector3D.X = vector.X - 2f * num * normal.X;
            Vector3D.Y = vector.Y - 2f * num * normal.Y;
            Vector3D.Z = vector.Z - 2f * num * normal.Z;
            return Vector3D;
        }

        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal.  Reference page contains code sample.
        /// </summary>
        /// <param name="vector">Source vector.</param><param name="normal">Normal of the surface.</param><param name="result">[OutAttribute] The reflected vector.</param>
        public static void Reflect(ref Vector3D vector, ref Vector3D normal, out Vector3D result)
        {
            double num = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;
            result.X = vector.X - 2f * num * normal.X;
            result.Y = vector.Y - 2f * num * normal.Y;
            result.Z = vector.Z - 2f * num * normal.Z;
        }

        /// <summary>
        /// Returns a vector that contains the lowest value from each matching pair of components.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3D Min(Vector3D value1, Vector3D value2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X < value2.X ? value1.X : value2.X;
            Vector3D.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
            Vector3D.Z = value1.Z < value2.Z ? value1.Z : value2.Z;
            return Vector3D;
        }

        /// <summary>
        /// Returns a vector that contains the lowest value from each matching pair of components.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The minimized vector.</param>
        public static void Min(ref Vector3D value1, ref Vector3D value2, out Vector3D result)
        {
            result.X = value1.X < value2.X ? value1.X : value2.X;
            result.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
            result.Z = value1.Z < value2.Z ? value1.Z : value2.Z;
        }

        /// <summary>
        /// Returns a vector that contains the highest value from each matching pair of components.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3D Max(Vector3D value1, Vector3D value2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X > value2.X ? value1.X : value2.X;
            Vector3D.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
            Vector3D.Z = value1.Z > value2.Z ? value1.Z : value2.Z;
            return Vector3D;
        }

        /// <summary>
        /// Returns a vector that contains the highest value from each matching pair of components.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The maximized vector.</param>
        public static void Max(ref Vector3D value1, ref Vector3D value2, out Vector3D result)
        {
            result.X = value1.X > value2.X ? value1.X : value2.X;
            result.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
            result.Z = value1.Z > value2.Z ? value1.Z : value2.Z;
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value1">The value to clamp.</param><param name="min">The minimum value.</param><param name="max">The maximum value.</param>
        public static Vector3D Clamp(Vector3D value1, Vector3D min, Vector3D max)
        {
            double num1 = value1.X;
            double num2 = num1 > max.X ? max.X : num1;
            double num3 = num2 < min.X ? min.X : num2;
            double num4 = value1.Y;
            double num5 = num4 > max.Y ? max.Y : num4;
            double num6 = num5 < min.Y ? min.Y : num5;
            double num7 = value1.Z;
            double num8 = num7 > max.Z ? max.Z : num7;
            double num9 = num8 < min.Z ? min.Z : num8;
            Vector3D Vector3D;
            Vector3D.X = num3;
            Vector3D.Y = num6;
            Vector3D.Z = num9;
            return Vector3D;
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value1">The value to clamp.</param><param name="min">The minimum value.</param><param name="max">The maximum value.</param><param name="result">[OutAttribute] The clamped value.</param>
        public static void Clamp(ref Vector3D value1, ref Vector3D min, ref Vector3D max, out Vector3D result)
        {
            double num1 = value1.X;
            double num2 = num1 > max.X ? max.X : num1;
            double num3 = num2 < min.X ? min.X : num2;
            double num4 = value1.Y;
            double num5 = num4 > max.Y ? max.Y : num4;
            double num6 = num5 < min.Y ? min.Y : num5;
            double num7 = value1.Z;
            double num8 = num7 > max.Z ? max.Z : num7;
            double num9 = num8 < min.Z ? min.Z : num8;
            result.X = num3;
            result.Y = num6;
            result.Z = num9;
        }

        /// <summary>
        /// Performs a linear interpolation between two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        public static Vector3D Lerp(Vector3D value1, Vector3D value2, double amount)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X + (value2.X - value1.X) * amount;
            Vector3D.Y = value1.Y + (value2.Y - value1.Y) * amount;
            Vector3D.Z = value1.Z + (value2.Z - value1.Z) * amount;
            return Vector3D;
        }

        /// <summary>
        /// Performs a linear interpolation between two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="amount">Value between 0 and 1 indicating the weight of value2.</param><param name="result">[OutAttribute] The result of the interpolation.</param>
        public static void Lerp(ref Vector3D value1, ref Vector3D value2, double amount, out Vector3D result)
        {
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
        }

        /// <summary>
        /// Returns a Vector3D containing the 3D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="value1">A Vector3D containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param><param name="value2">A Vector3D containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param><param name="value3">A Vector3D containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param><param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in value2).</param><param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in value3).</param>
        public static Vector3D Barycentric(Vector3D value1, Vector3D value2, Vector3D value3, double amount1, double amount2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
            Vector3D.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
            Vector3D.Z = value1.Z + amount1 * (value2.Z - value1.Z) + amount2 * (value3.Z - value1.Z);
            return Vector3D;
        }

        /// <summary>
        /// Returns a Vector3D containing the 3D Cartesian coordinates of a point specified in barycentric (areal) coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="value1">A Vector3D containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param><param name="value2">A Vector3D containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param><param name="value3">A Vector3D containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param><param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in value2).</param><param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in value3).</param><param name="result">[OutAttribute] The 3D Cartesian coordinates of the specified point are placed in this Vector3D on exit.</param>
        public static void Barycentric(ref Vector3D value1, ref Vector3D value2, ref Vector3D value3, double amount1, double amount2, out Vector3D result)
        {
            result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
            result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
            result.Z = value1.Z + amount1 * (value2.Z - value1.Z) + amount2 * (value3.Z - value1.Z);
        }

        /// <summary>
        /// Interpolates between two values using a cubic equation.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param><param name="amount">Weighting value.</param>
        public static Vector3D SmoothStep(Vector3D value1, Vector3D value2, double amount)
        {
            amount = amount > 1.0 ? 1f : (amount < 0.0 ? 0.0f : amount);
            amount = amount * amount * (3.0 - 2.0 * amount);
            Vector3D Vector3D;
            Vector3D.X = value1.X + (value2.X - value1.X) * amount;
            Vector3D.Y = value1.Y + (value2.Y - value1.Y) * amount;
            Vector3D.Z = value1.Z + (value2.Z - value1.Z) * amount;
            return Vector3D;
        }

        /// <summary>
        /// Interpolates between two values using a cubic equation.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="amount">Weighting value.</param><param name="result">[OutAttribute] The interpolated value.</param>
        public static void SmoothStep(ref Vector3D value1, ref Vector3D value2, double amount, out Vector3D result)
        {
            amount = amount > 1.0 ? 1f : (amount < 0.0 ? 0.0f : amount);
            amount = amount * amount * (3.0 - 2.0 * amount);
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param><param name="value2">The second position in the interpolation.</param><param name="value3">The third position in the interpolation.</param><param name="value4">The fourth position in the interpolation.</param><param name="amount">Weighting factor.</param>
        public static Vector3D CatmullRom(Vector3D value1, Vector3D value2, Vector3D value3, Vector3D value4, double amount)
        {
            double num1 = amount * amount;
            double num2 = amount * num1;
            Vector3D Vector3D;
            Vector3D.X = 0.5 * (2.0 * value2.X + (-value1.X + value3.X) * amount + (2.0 * value1.X - 5.0 * value2.X + 4.0 * value3.X - value4.X) * num1 + (-value1.X + 3.0 * value2.X - 3.0 * value3.X + value4.X) * num2);
            Vector3D.Y = 0.5 * (2.0 * value2.Y + (-value1.Y + value3.Y) * amount + (2.0 * value1.Y - 5.0 * value2.Y + 4.0 * value3.Y - value4.Y) * num1 + (-value1.Y + 3.0 * value2.Y - 3.0 * value3.Y + value4.Y) * num2);
            Vector3D.Z = 0.5 * (2.0 * value2.Z + (-value1.Z + value3.Z) * amount + (2.0 * value1.Z - 5.0 * value2.Z + 4.0 * value3.Z - value4.Z) * num1 + (-value1.Z + 3.0 * value2.Z - 3.0 * value3.Z + value4.Z) * num2);
            return Vector3D;
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param><param name="value2">The second position in the interpolation.</param><param name="value3">The third position in the interpolation.</param><param name="value4">The fourth position in the interpolation.</param><param name="amount">Weighting factor.</param><param name="result">[OutAttribute] A vector that is the result of the Catmull-Rom interpolation.</param>
        public static void CatmullRom(ref Vector3D value1, ref Vector3D value2, ref Vector3D value3, ref Vector3D value4, double amount, out Vector3D result)
        {
            double num1 = amount * amount;
            double num2 = amount * num1;
            result.X = 0.5 * (2.0 * value2.X + (-value1.X + value3.X) * amount + (2.0 * value1.X - 5.0 * value2.X + 4.0 * value3.X - value4.X) * num1 + (-value1.X + 3.0 * value2.X - 3.0 * value3.X + value4.X) * num2);
            result.Y = 0.5 * (2.0 * value2.Y + (-value1.Y + value3.Y) * amount + (2.0 * value1.Y - 5.0 * value2.Y + 4.0 * value3.Y - value4.Y) * num1 + (-value1.Y + 3.0 * value2.Y - 3.0 * value3.Y + value4.Y) * num2);
            result.Z = 0.5 * (2.0 * value2.Z + (-value1.Z + value3.Z) * amount + (2.0 * value1.Z - 5.0 * value2.Z + 4.0 * value3.Z - value4.Z) * num1 + (-value1.Z + 3.0 * value2.Z - 3.0 * value3.Z + value4.Z) * num2);
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">Source position vector.</param><param name="tangent1">Source tangent vector.</param><param name="value2">Source position vector.</param><param name="tangent2">Source tangent vector.</param><param name="amount">Weighting factor.</param>
        public static Vector3D Hermite(Vector3D value1, Vector3D tangent1, Vector3D value2, Vector3D tangent2, double amount)
        {
            double num1 = amount * amount;
            double num2 = amount * num1;
            double num3 = 2.0 * num2 - 3.0 * num1 + 1.0;
            double num4 = -2.0 * num2 + 3.0 * num1;
            double num5 = num2 - 2f * num1 + amount;
            double num6 = num2 - num1;
            Vector3D Vector3D;
            Vector3D.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
            Vector3D.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
            Vector3D.Z = value1.Z * num3 + value2.Z * num4 + tangent1.Z * num5 + tangent2.Z * num6;
            return Vector3D;
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">Source position vector.</param><param name="tangent1">Source tangent vector.</param><param name="value2">Source position vector.</param><param name="tangent2">Source tangent vector.</param><param name="amount">Weighting factor.</param><param name="result">[OutAttribute] The result of the Hermite spline interpolation.</param>
        public static void Hermite(ref Vector3D value1, ref Vector3D tangent1, ref Vector3D value2, ref Vector3D tangent2, double amount, out Vector3D result)
        {
            double num1 = amount * amount;
            double num2 = amount * num1;
            double num3 = 2.0 * num2 - 3.0 * num1 + 1.0;
            double num4 = -2.0 * num2 + 3.0 * num1;
            double num5 = num2 - 2f * num1 + amount;
            double num6 = num2 - num1;
            result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
            result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
            result.Z = value1.Z * num3 + value2.Z * num4 + tangent1.Z * num5 + tangent2.Z * num6;
        }




        /// <summary>
        /// Returns a vector pointing in the opposite direction.
        /// </summary>
        /// <param name="value">Source vector.</param>
        public static Vector3D Negate(Vector3D value)
        {
            Vector3D Vector3D;
            Vector3D.X = -value.X;
            Vector3D.Y = -value.Y;
            Vector3D.Z = -value.Z;
            return Vector3D;
        }

        /// <summary>
        /// Returns a vector pointing in the opposite direction.
        /// </summary>
        /// <param name="value">Source vector.</param><param name="result">[OutAttribute] Vector pointing in the opposite direction.</param>
        public static void Negate(ref Vector3D value, out Vector3D result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3D Add(Vector3D value1, Vector3D value2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X + value2.X;
            Vector3D.Y = value1.Y + value2.Y;
            Vector3D.Z = value1.Z + value2.Z;
            return Vector3D;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] Sum of the source vectors.</param>
        public static void Add(ref Vector3D value1, ref Vector3D value2, out Vector3D result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
        }

        /// <summary>
        /// Subtracts a vector from a vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3D Subtract(Vector3D value1, Vector3D value2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X - value2.X;
            Vector3D.Y = value1.Y - value2.Y;
            Vector3D.Z = value1.Z - value2.Z;
            return Vector3D;
        }

        /// <summary>
        /// Subtracts a vector from a vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The result of the subtraction.</param>
        public static void Subtract(ref Vector3D value1, ref Vector3D value2, out Vector3D result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param>
        public static Vector3D Multiply(Vector3D value1, Vector3D value2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X * value2.X;
            Vector3D.Y = value1.Y * value2.Y;
            Vector3D.Z = value1.Z * value2.Z;
            return Vector3D;
        }

        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Source vector.</param><param name="result">[OutAttribute] The result of the multiplication.</param>
        public static void Multiply(ref Vector3D value1, ref Vector3D value2, out Vector3D result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
        }

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="scaleFactor">Scalar value.</param>
        public static Vector3D Multiply(Vector3D value1, double scaleFactor)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X * scaleFactor;
            Vector3D.Y = value1.Y * scaleFactor;
            Vector3D.Z = value1.Z * scaleFactor;
            return Vector3D;
        }

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="scaleFactor">Scalar value.</param><param name="result">[OutAttribute] The result of the multiplication.</param>
        public static void Multiply(ref Vector3D value1, double scaleFactor, out Vector3D result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
        }

        /// <summary>
        /// Divides the components of a vector by the components of another vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">Divisor vector.</param>
        public static Vector3D Divide(Vector3D value1, Vector3D value2)
        {
            Vector3D Vector3D;
            Vector3D.X = value1.X / value2.X;
            Vector3D.Y = value1.Y / value2.Y;
            Vector3D.Z = value1.Z / value2.Z;
            return Vector3D;
        }

        /// <summary>
        /// Divides the components of a vector by the components of another vector.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">The divisor.</param><param name="result">[OutAttribute] The result of the division.</param>
        public static void Divide(ref Vector3D value1, ref Vector3D value2, out Vector3D result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">The divisor.</param>
        public static Vector3D Divide(Vector3D value1, double value2)
        {
            double num = 1f / value2;
            Vector3D Vector3D;
            Vector3D.X = value1.X * num;
            Vector3D.Y = value1.Y * num;
            Vector3D.Z = value1.Z * num;
            return Vector3D;
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="value1">Source vector.</param><param name="value2">The divisor.</param><param name="result">[OutAttribute] The result of the division.</param>
        public static void Divide(ref Vector3D value1, double value2, out Vector3D result)
        {
            double num = 1f / value2;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            result.Z = value1.Z * num;
        }
    }
}
