using CitizenFX.Core;

namespace topcam3
{
    class ExtraVector3
    {
        public static Vector3 OffsetToGlobal(Matrix parentMatrix, Vector3 childPos)
        {
            return Vector3.TransformCoordinate(childPos, parentMatrix);
        }

        public static Vector3 OffsetToGlobal(Vector3 parentPos, Vector3 parentRot, Vector3 childPos)
        {
            return Vector3.TransformCoordinate(childPos, ExtraMatrix.Create(parentPos, parentRot));
        }
    }
}
