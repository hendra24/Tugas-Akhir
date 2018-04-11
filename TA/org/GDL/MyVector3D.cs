using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.GDL
{
    public struct MyVector3D
    {
        internal double _x;
        internal double _y;
        internal double _z;

        public double Length
        {
            get
            {
                return Math.Sqrt(this._x * this._x + this._y * this._y + this._z * this._z);
            }
        }

        public double LengthSquared
        {
            get
            {
                return this._x * this._x + this._y * this._y + this._z * this._z;
            }
        }

        public double X
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }

        public double Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }

        public double Z
        {
            get
            {
                return this._z;
            }
            set
            {
                this._z = value;
            }
        }

        public MyVector3D(double x, double y, double z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }

        public static explicit operator Point3D(MyVector3D vector)
        {
            return new Point3D(vector._x, vector._y, vector._z);
        }
        /*
        public static explicit operator Size3D(MyVector3D vector)
        {
            return new Size3D(Math.Abs(vector._x), Math.Abs(vector._y), Math.Abs(vector._z));
        }*/

        public static MyVector3D operator -(MyVector3D vector)
        {
            return new MyVector3D(-vector._x, -vector._y, -vector._z);
        }

        public static MyVector3D operator +(MyVector3D vector1, MyVector3D vector2)
        {
            return new MyVector3D(vector1._x + vector2._x, vector1._y + vector2._y, vector1._z + vector2._z);
        }

        public static MyVector3D operator -(MyVector3D vector1, MyVector3D vector2)
        {
            return new MyVector3D(vector1._x - vector2._x, vector1._y - vector2._y, vector1._z - vector2._z);
        }

        public static Point3D operator +(MyVector3D vector, Point3D point)
        {
            return new Point3D(vector._x + point.X, vector._y + point.Y, vector._z + point.Z);
        }

        public static Point3D operator -(MyVector3D vector, Point3D point)
        {
            return new Point3D(vector._x - point.X, vector._y - point.Y, vector._z - point.Z);
        }

        public static MyVector3D operator *(MyVector3D vector, double scalar)
        {
            return new MyVector3D(vector._x * scalar, vector._y * scalar, vector._z * scalar);
        }

        public static MyVector3D operator *(double scalar, MyVector3D vector)
        {
            return new MyVector3D(vector._x * scalar, vector._y * scalar, vector._z * scalar);
        }

        public static MyVector3D operator /(MyVector3D vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }
        /*
        public static MyVector3D operator *(MyVector3D vector, MyVector3D matrix)
        {
            return matrix.Transform(vector);
        }*/

        public static bool operator ==(MyVector3D vector1, MyVector3D vector2)
        {
            if (vector1.X == vector2.X && vector1.Y == vector2.Y)
                return vector1.Z == vector2.Z;
            else
                return false;
        }

        public static bool operator !=(MyVector3D vector1, MyVector3D vector2)
        {
            return !(vector1 == vector2);
        }

        public void Normalize()
        {
            double num1 = Math.Abs(this._x);
            double num2 = Math.Abs(this._y);
            double num3 = Math.Abs(this._z);
            if (num2 > num1)
                num1 = num2;
            if (num3 > num1)
                num1 = num3;
            this._x /= num1;
            this._y /= num1;
            this._z /= num1;
            this /= Math.Sqrt(this._x * this._x + this._y * this._y + this._z * this._z);
        }

        public static double AngleBetween(MyVector3D vector1, MyVector3D vector2)
        {
            vector1.Normalize();
            vector2.Normalize();
            return RadiansToDegrees(MyVector3D.DotProduct(vector1, vector2) >= 0.0 ? 2.0 * Math.Asin((vector1 - vector2).Length / 2.0) : Math.PI - 2.0 * Math.Asin((-vector1 - vector2).Length / 2.0));
        }

        internal static double RadiansToDegrees(double radians)
        {
            return radians * 57.2957795130823;
        }

        public void Negate()
        {
            this._x = -this._x;
            this._y = -this._y;
            this._z = -this._z;
        }

        public static MyVector3D Add(MyVector3D vector1, MyVector3D vector2)
        {
            return new MyVector3D(vector1._x + vector2._x, vector1._y + vector2._y, vector1._z + vector2._z);
        }

        public static MyVector3D Subtract(MyVector3D vector1, MyVector3D vector2)
        {
            return new MyVector3D(vector1._x - vector2._x, vector1._y - vector2._y, vector1._z - vector2._z);
        }

        public static Point3D Add(MyVector3D vector, Point3D point)
        {
            return new Point3D(vector._x + point.X, vector._y + point.Y, vector._z + point.Z);
        }

        public static Point3D Subtract(MyVector3D vector, Point3D point)
        {
            return new Point3D(vector._x - point.X, vector._y - point.X, vector._z - point.X);
        }

        public static MyVector3D Multiply(MyVector3D vector, double scalar)
        {
            return new MyVector3D(vector._x * scalar, vector._y * scalar, vector._z * scalar);
        }

        public static MyVector3D Multiply(double scalar, MyVector3D vector)
        {
            return new MyVector3D(vector._x * scalar, vector._y * scalar, vector._z * scalar);
        }

        public static MyVector3D Divide(MyVector3D vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }
        /*
        public static MyVector3D Multiply(MyVector3D vector, MyVector3D matrix)
        {
            return matrix.Transform(vector);
        }*/

        public static double DotProduct(MyVector3D vector1, MyVector3D vector2)
        {
            return MyVector3D.DotProduct(ref vector1, ref vector2);
        }

        internal static double DotProduct(ref MyVector3D vector1, ref MyVector3D vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y + vector1._z * vector2._z;
        }

        public static MyVector3D CrossProduct(MyVector3D vector1, MyVector3D vector2)
        {
            MyVector3D result;
            MyVector3D.CrossProduct(ref vector1, ref vector2, out result);
            return result;
        }

        internal static void CrossProduct(ref MyVector3D vector1, ref MyVector3D vector2, out MyVector3D result)
        {
            result._x = vector1._y * vector2._z - vector1._z * vector2._y;
            result._y = vector1._z * vector2._x - vector1._x * vector2._z;
            result._z = vector1._x * vector2._y - vector1._y * vector2._x;
        }

        public static bool Equals(MyVector3D vector1, MyVector3D vector2)
        {
            if (vector1.X.Equals(vector2.X) && vector1.Y.Equals(vector2.Y))
                return vector1.Z.Equals(vector2.Z);
            else
                return false;
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is MyVector3D))
                return false;
            else
                return MyVector3D.Equals(this, (MyVector3D)o);
        }

        public bool Equals(MyVector3D value)
        {
            return MyVector3D.Equals(this, value);
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }
        /*
        public static Vector3D Parse(string source)
        {
            IFormatProvider formatProvider = (IFormatProvider)TypeConverterHelper.EnglishUSCulture;
            TokenizerHelper tokenizerHelper = new TokenizerHelper(source, formatProvider);
            Vector3D vector3D = new Vector3D(Convert.ToDouble(tokenizerHelper.NextTokenRequired(), formatProvider), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), formatProvider), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), formatProvider));
            tokenizerHelper.LastTokenRequired();
            return vector3D;
        }

        public override string ToString()
        {
            return this.ConvertToString((string)null, (IFormatProvider)null);
        }

        public string ToString(IFormatProvider provider)
        {
            return this.ConvertToString((string)null, provider);
        }

        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            return this.ConvertToString(format, provider);
        }
        
        internal string ConvertToString(string format, IFormatProvider provider)
        {
            char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}", (object)numericListSeparator, (object)this._x, (object)this._y, (object)this._z);
        }*/
    }
}