using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KSW.UI.WPF.Controls
{
    public static class MathHelpers
    {
        internal const double DoubleEpsilon = 2.2204460492503131E-16;

        internal const float FloatEpsilon = 1.1920929E-07f;

        private static (double min, double max) GetMinMax(double a, double b)
        {
            if (!(a >= b))
            {
                return (min: a, max: b);
            }

            return (min: b, max: a);
        }

        private static (float min, float max) GetMinMax(float a, float b)
        {
            if (!(a >= b))
            {
                return (min: a, max: b);
            }

            return (min: b, max: a);
        }

        private static (decimal min, decimal max) GetMinMax(decimal a, decimal b)
        {
            if (!(a >= b))
            {
                return (min: a, max: b);
            }

            return (min: b, max: a);
        }

        private static (int min, int max) GetMinMax(int a, int b)
        {
            if (a < b)
            {
                return (min: a, max: b);
            }

            return (min: b, max: a);
        }

        public static double SafeClamp(double value, double min, double max)
        {
            (min, max) = GetMinMax(min, max);
            if (value < min)
            {
                return min;
            }

            if (!(value > max))
            {
                return value;
            }

            return max;
        }

        public static decimal SafeClamp(decimal value, decimal min, decimal max)
        {
            (min, max) = GetMinMax(min, max);
            if (value < min)
            {
                return min;
            }

            if (!(value > max))
            {
                return value;
            }

            return max;
        }

        public static int SafeClamp(int value, int min, int max)
        {
            (min, max) = GetMinMax(min, max);
            if (value < min)
            {
                return min;
            }

            if (value <= max)
            {
                return value;
            }

            return max;
        }

        public static float SafeClamp(float value, float min, float max)
        {
            (min, max) = GetMinMax(min, max);
            if (value < min)
            {
                return min;
            }

            if (!(value > max))
            {
                return value;
            }

            return max;
        }

        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }

            double num = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.2204460492503131E-16;
            double num2 = value1 - value2;
            if (0.0 - num < num2)
            {
                return num > num2;
            }

            return false;
        }

        public static bool AreClose(double value1, double value2, double eps)
        {
            if (value1 == value2)
            {
                return true;
            }

            double num = value1 - value2;
            if (0.0 - eps < num)
            {
                return eps > num;
            }

            return false;
        }

        public static bool AreClose(float value1, float value2)
        {
            if (value1 == value2)
            {
                return true;
            }

            float num = (Math.Abs(value1) + Math.Abs(value2) + 10f) * 1.1920929E-07f;
            float num2 = value1 - value2;
            if (0f - num < num2)
            {
                return num > num2;
            }

            return false;
        }

        public static bool LessThan(double value1, double value2)
        {
            if (value1 < value2)
            {
                return !AreClose(value1, value2);
            }

            return false;
        }

        public static bool LessThan(float value1, float value2)
        {
            if (value1 < value2)
            {
                return !AreClose(value1, value2);
            }

            return false;
        }

        public static bool GreaterThan(double value1, double value2)
        {
            if (value1 > value2)
            {
                return !AreClose(value1, value2);
            }

            return false;
        }

        public static bool GreaterThan(float value1, float value2)
        {
            if (value1 > value2)
            {
                return !AreClose(value1, value2);
            }

            return false;
        }

        public static bool LessThanOrClose(double value1, double value2)
        {
            if (!(value1 < value2))
            {
                return AreClose(value1, value2);
            }

            return true;
        }

        public static bool LessThanOrClose(float value1, float value2)
        {
            if (!(value1 < value2))
            {
                return AreClose(value1, value2);
            }

            return true;
        }

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (!(value1 > value2))
            {
                return AreClose(value1, value2);
            }

            return true;
        }

        public static bool GreaterThanOrClose(float value1, float value2)
        {
            if (!(value1 > value2))
            {
                return AreClose(value1, value2);
            }

            return true;
        }

        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 2.2204460492503131E-15;
        }

        public static bool IsOne(float value)
        {
            return Math.Abs(value - 1f) < 1.1920929E-06f;
        }

        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 2.2204460492503131E-15;
        }

        public static bool IsZero(float value)
        {
            return Math.Abs(value) < 1.1920929E-06f;
        }

        public static double DegreeToRadians(double angle)
        {
            return angle * (Math.PI / 180.0);
        }

        public static double GradiansToRadians(double angle)
        {
            return angle * (Math.PI / 200.0);
        }

        public static double TurnToRadians(double angle)
        {
            return angle * 2.0 * Math.PI;
        }

        public static Point GetEllipsePoint(Point centre, double radiusX, double radiusY, double angle)
        {
            return new Point(radiusX * Math.Cos(angle) + centre.X, radiusY * Math.Sin(angle) + centre.Y);
        }
    }
}
