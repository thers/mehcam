using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace topcam3
{
    public class FuccMatrix
    {
        public static Matrix Create(Vector3 pos, Vector3 rot)
        {
            Matrix te = Matrix.RotationQuaternion(FuccQuaternion.CreateFromEuler(rot));

            te.M41 = pos.X;
            te.M42 = pos.Y;
            te.M43 = pos.Z;

            return te;
        }

        public static Matrix CreateProjection(float fov, float aspect, float near, float far)
        {
            return Matrix.PerspectiveFovLH(FuccQuaternion.Deg2Rad * fov, aspect, near, far);
        }

        public static Matrix Mono_CreatePerspective(float fov, float aspect, float near, float far)
        {
            var result = Matrix.Identity;

            float num = 1f / ((float)Math.Tan(FuccQuaternion.Deg2Rad * fov * 0.5f));
            float num9 = num / aspect;

            result.M11 = num9;
            result.M12 = result.M13 = result.M14 = 0;
            result.M22 = num;
            result.M21 = result.M23 = result.M24 = 0;
            result.M31 = result.M32 = 0f;
            result.M33 = far / (near - far);
            result.M34 = -1;
            result.M41 = result.M42 = result.M44 = 0;
            result.M43 = (near * far) / (near - far);

            return result;
        }

        public static Matrix Mono_CreateRotationQuaternion(Quaternion quaternion)
        {
            Matrix result = new Matrix();

            float num9 = quaternion.X * quaternion.X;
            float num8 = quaternion.Y * quaternion.Y;
            float num7 = quaternion.Z * quaternion.Z;
            float num6 = quaternion.X * quaternion.Y;
            float num5 = quaternion.Z * quaternion.W;
            float num4 = quaternion.Z * quaternion.X;
            float num3 = quaternion.Y * quaternion.W;
            float num2 = quaternion.Y * quaternion.Z;
            float num = quaternion.X * quaternion.W;

            result.M11 = 1f - (2f * (num8 + num7));
            result.M12 = 2f * (num6 + num5);
            result.M13 = 2f * (num4 - num3);
            result.M14 = 0f;
            result.M21 = 2f * (num6 - num5);
            result.M22 = 1f - (2f * (num7 + num9));
            result.M23 = 2f * (num2 + num);
            result.M24 = 0f;
            result.M31 = 2f * (num4 + num3);
            result.M32 = 2f * (num2 - num);
            result.M33 = 1f - (2f * (num8 + num9));
            result.M34 = 0f;
            result.M41 = 0f;
            result.M42 = 0f;
            result.M43 = 0f;
            result.M44 = 1f;

            return result;
        }
    }
}
