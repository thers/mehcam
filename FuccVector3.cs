using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace topcam3
{
    class FuccVector3
    {
        public static Vector3 OffsetToGlobal(Vector3 parentPos, Vector3 parentRot, Vector3 childPos)
        {
            return RowMajor_ApplyMatrix(childPos, FuccMatrix.Create(parentPos, parentRot));
        }

        public static Vector3 RowMajor_MonoTransform(Vector3 vec, Matrix e)
        {
            var result = new Vector3();

            var x = (vec.X * e.M11) + (vec.Y * e.M21) + (vec.Z * e.M31) + e.M41;
            var y = (vec.X * e.M12) + (vec.Y * e.M22) + (vec.Z * e.M32) + e.M42;
            var z = (vec.X * e.M13) + (vec.Y * e.M23) + (vec.Z * e.M33) + e.M43;

            result.X = x;
            result.Y = y;
            result.Z = z;

            return result;
        }

        public static Vector3 RowMajor_ApplyMatrix(Vector3 vec, Matrix e)
        {
            var x = vec.X;
            var y = vec.Y;
            var z = vec.Z;

            var w = 1f / (e.M14 * x + e.M24 * y + e.M34 * z + e.M44);

            return new Vector3(
                (e.M11 * x + e.M21 * y + e.M31 * z + e.M41) * w,
                (e.M12 * x + e.M22 * y + e.M32 * z + e.M42) * w,
                (e.M13 * x + e.M23 * y + e.M33 * z + e.M43) * w
            );
        }

        public static Vector3 ColumnMajor_ApplyMatrix(Vector3 vec, Matrix e)
        {
            var x = vec.X;
            var y = vec.Y;
            var z = vec.Z;

            var w = 1f / (e.M41 * x + e.M42 * y + e.M43 * z + e.M44);

            return new Vector3(
                (e.M11 * x + e.M12 * y + e.M13 * z + e.M14) * w,
                (e.M21 * x + e.M22 * y + e.M23 * z + e.M24) * w,
                (e.M31 * x + e.M32 * y + e.M33 * z + e.M34) * w
            );
        }
    }
}
