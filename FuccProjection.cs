using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;

namespace topcam3
{
    public static class FuccProjection
    {
        public static Matrix GetForCamera(Camera cam)
        {
            return FuccMatrix.CreateProjection(
                Function.Call<float>(Hash.GET_CAM_FOV, cam),
                Screen.Width / Screen.Height,
                cam.NearClip,
                cam.FarClip
            );
        }

        public static Vector3 WorldToScreen(Vector3 pos, Camera camera) =>
            WorldToScreen(pos, camera.GetMatrix(), camera.GetProjection());

        public static Vector3 WorldToScreen(Vector3 pos, Matrix world, Matrix projection)
        {
            // Make transformation matrix
            var matrix = CamWorldToView(world) * projection;

            // Translate world pos to screen space pos
            var screenSpacePos = FuccVector3.RowMajor_ApplyMatrix(pos, matrix);

            // Screen space pos to image pos
            Vector3 imagePos = screenSpacePos;

            return imagePos;
        }

        public static Vector3 ScreenToWorld(Vector2 pos, Camera camera) =>
            ScreenToWorld(new Vector3(1f - pos.X * 2f, 1f - pos.Y * 2f, .5f), camera);

        public static Vector3 ScreenToWorld(Vector3 pos, Camera camera) =>
            ScreenToWorld(pos, camera.GetMatrix(), camera.GetProjection());

        public static Vector3 ScreenToWorld(Vector3 pos, Matrix world, Matrix projection)
        {
            // Make transformation matrix
            var matrix = Matrix.Invert(CamWorldToView(world) * projection);
            
            // Image pos to screen space
            var screenSpacePos = new Vector3(
                pos.X,
                pos.Y,
                pos.Z
            );

            return FuccVector3.RowMajor_ApplyMatrix(screenSpacePos, matrix);
        }

        public static Matrix CamWorldToView(Matrix world)
        {
            return Matrix.LookAtLH((Vector3)world.Row4, TopCam.LocalPedPos, world.Up);
        }
    }
}
