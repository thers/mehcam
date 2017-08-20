using CitizenFX.Core;

namespace topcam3
{
    public static class Extensions
    {
        public static Matrix GetMatrix(this Entity ent)
        {
            var pos = ent.Position;
            var rot = ent.Rotation;

            return ExtraMatrix.Create(pos, rot);
        }

        public static Matrix GetProjection(this Camera cam)
            => Projection.GetForCamera(cam);


        public static Vector3 GlobalPositionFromOffset(this Camera cam, Vector3 offset)
        {
            return Vector3.TransformCoordinate(offset, ExtraMatrix.Create(cam.Position, cam.Rotation));
        }
    }
}
