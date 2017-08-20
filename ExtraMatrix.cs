using CitizenFX.Core;

namespace topcam3
{
    public class ExtraMatrix
    {
        public static Matrix Create(Vector3 pos, Vector3 rot)
        {
            Matrix te = Matrix.RotationQuaternion(ExtraQuaternion.CreateFromEuler(rot));

            te.M41 = pos.X;
            te.M42 = pos.Y;
            te.M43 = pos.Z;

            return te;
        }
    }
}
