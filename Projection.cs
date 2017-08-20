using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;

namespace topcam3
{
    public static class Projection
    {
        public static Matrix GetForCamera(Camera cam)
        {
            return Matrix.PerspectiveFovLH(
                ExtraQuaternion.Deg2Rad * GetCamFov(cam.Handle),
                Screen.Width / Screen.Height,
                cam.NearClip,
                cam.FarClip
            );
        }

        public static Vector3 WorldToScreen(Vector3 pos, Camera camera) =>
            WorldToScreen(pos, CamWorldViewMatrix(camera), camera.GetProjection());

        public static Vector3 WorldToScreen(Vector3 pos, Matrix world, Matrix projection)
        {
            // Make transformation matrix
            var matrix = world * projection;

            // Translate world pos to screen space pos
            var screenSpacePos = Vector3.TransformCoordinate(pos, matrix);

            // Screen space pos to image pos
            Vector3 imagePos = screenSpacePos;

            return imagePos;
        }

        public static Vector3 ScreenToWorld(Vector2 pos, Camera camera) =>
            ScreenToWorld(new Vector3(1f - pos.X * 2f, 1f - pos.Y * 2f, .5f), camera);

        public static Vector3 ScreenToWorld(Vector3 pos, Camera camera) =>
            ScreenToWorld(pos, CamWorldViewMatrix(camera), camera.GetProjection());

        public static Vector3 ScreenToWorld(Vector3 pos, Matrix world, Matrix projection)
        {
            // Make transformation matrix
            var matrix = Matrix.Invert(world * projection);
            
            // Image pos to screen space
            var screenSpacePos = new Vector3(
                pos.X,
                pos.Y,
                pos.Z
            );

            return Vector3.TransformCoordinate(screenSpacePos, matrix);
        }

        public static Matrix CamWorldViewMatrix(Camera camera)
        {
            return Matrix.LookAtLH(camera.Position, Vector3.TransformCoordinate(Vector3.UnitY, camera.Matrix), Vector3.ForwardLH);
        }
    }
}
