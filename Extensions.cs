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
    public static class Extensions
    {
        public static Matrix GetMatrix(this Entity ent)
        {
            var pos = ent.Position;
            var rot = ent.Rotation;

            return FuccMatrix.Create(pos, rot);
        }


        public static Matrix GetMatrix(this Camera cam)
        {
            var pos = cam.Position;
            var rot = Function.Call<Vector3>(Hash.GET_CAM_ROT, cam, 2); ;

            return FuccMatrix.Create(pos, rot);
        }


        public static Matrix Projection(this Camera cam) => FuccProjection.GetForCamera(cam);
    }
}
