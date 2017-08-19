using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.Function;

namespace topcam3
{
    public class TopCam : BaseScript
    {
        public TopCam()
        {
            EventHandlers["onClientResourceStop"] += new Action<string>((resName) => {
                if (resName == Call<string>(Hash.GET_CURRENT_RESOURCE_NAME))
                {
                    Call(Hash.RENDER_SCRIPT_CAMS, false, true, 1000, false, false);
                    Call(Hash.DESTROY_CAM, camera.Handle);
                    Bitch.Delete();
                }
            });

            txt.Position = new PointF(1270f, 400f);
            txt.Alignment = Alignment.Right;

            txt2.Position = new PointF(1270f, 450f);
            txt2.Alignment = Alignment.Right;

            txt3.Position = new PointF(1270f, 500f);
            txt3.Alignment = Alignment.Right;

            txt4.Position = new PointF(1270f, 550f);
            txt4.Alignment = Alignment.Right;

            Tick += OnTick;
        }

        private bool inited = false;
        
        private static Text txt = new Text("", new PointF(10f, 10f), .3f);
        private static Text txt2 = new Text("", new PointF(10f, 30f), .3f);
        private static Text txt3 = new Text("", new PointF(10f, 50f), .3f);
        private static Text txt4 = new Text("", new PointF(10f, 70f), .3f);
        private static Text txt5 = new Text("lol", new PointF(10f, 90f), .3f);

        private static Text tx = new Text("x", new PointF(), .3f);
        private static Text ty = new Text("y", new PointF(), .3f);
        private static Text tz = new Text("z", new PointF(), .3f);

        private Camera camera;

        private int i = 0;

        public static Vector3 LocalPedPos;

        public Ped Bitch;
        public ParticleEffectsAsset PTFX;

        public static string Asset = "scr_rcbarry1";
        public static string ParticleEffectName = "scr_alien_teleport";

        public float offsetY = 0f;

        public async Task OnTick()
        {            
            var pos = LocalPedPos = LocalPlayer.Character.Position;

            var camPos =  Vector3.Add(pos, new Vector3(2, offsetY, 2));
            if (!inited)
            {
                inited = true;

                camera = World.CreateCamera(
                    camPos,
                    new Vector3(0f, 0f, 0f),
                    170f
                );
                Call(Hash.RENDER_SCRIPT_CAMS, true, true, 1000, false, false);

                Bitch = await World.CreatePed(new Model(PedHash.Beach01AFY), camPos);
                PTFX = new ParticleEffectsAsset(Asset);
            }
            camera.Position = camPos;
            camera.PointAt(LocalPlayer.Character);

            var camRot = Call<Vector3>(Hash.GET_CAM_ROT, camera, 2);

            var rx = Game.GetControlNormal(0, (Control)239);
            var ry = Game.GetControlNormal(0, (Control)240);
            
            var farPos = FuccProjection.ScreenToWorld(new Vector2(rx, ry), camera);
            var farWorldPos = farPos - Vector3.Normalize(camPos - farPos) * 100f;

            var wp = World.Raycast(
                camPos,
                farWorldPos,
                IntersectOptions.Map
            );

            //txt.Caption = curWorldPos.ToString();
            //txt2.Caption = farWorldPos.ToString();

            DrawLine(camPos, farWorldPos);
            DrawCube(wp.HitPosition, new Vector3(.1f), 255, 0, 0);

            //txt2.Caption = "Projected to camera: " + String.Join(", ", projected.ToArray().Select((v) => v.ToString("f04")));
            //var hitProjected = FuccProjection.WorldToScreen(wp.HitPosition, camera);
            //txt2.Caption = "Projected hit to camera: " + VectorToString(hitProjected);
            //txt2.Draw();

            txt3.Caption = "Cam rot: " + VectorToString(camera.Rotation);
            txt3.Draw();

            //txt4.Caption = "Ped pos: " + VectorToString(pos);
            //txt4.Draw();


            var camMatrix = camera.GetMatrix();
            
            DrawMatrix("cam matrix", camMatrix, 10, 10, 255, 0, 0);
            DrawMatrix("cam view matrix", FuccProjection.CamWorldToView(camMatrix), 10, 100, 215, 58, 73);
            //DrawMatrix("cam rot matrix from euler", Matrix.Invert(Matrix.RotationQuaternion(FuccQuaternion.CreateFromEuler(camRot))), 10, 200, 200, 225, 255);
            //DrawMatrix("cam rot lookat", Matrix.LookAtLH(camPos, pos, new Vector3(0,0,1)), 10, 300, 255, 0, 255);
            //DrawMatrix("cam matrix * projection.Inverse", camMatrix * Matrix.Invert(projection), 10, 400, 0, 92, 197);

            // Frustum
            DrawHelper(camera);
            //DrawLine(camPos, FuccProjection.ScreenToWorld(new Vector3(-1f, 1f, 0f), camera), 0xff, 0xaa, 0);
            //DrawLine(camPos, FuccProjection.ScreenToWorld(new Vector3(1f, 1f, .5f), camera), 0xff, 0xaa, 0);
            //DrawLine(camPos, FuccProjection.ScreenToWorld(new Vector3(1f, -1f, .5f), camera), 0xff, 0xaa, 0);
            //DrawLine(camPos, FuccProjection.ScreenToWorld(new Vector3(-1f, -1f, .5f), camera), 0xff, 0xaa, 0);

            Call(Hash._SHOW_CURSOR_THIS_FRAME, new InputArgument[0]);

            if (Game.IsControlJustPressed(0, Control.PhoneRight))
            {
                Call(Hash.RENDER_SCRIPT_CAMS, true, true, 1000, false, false);
            }
            if (Game.IsControlJustPressed(0, Control.PhoneLeft))
            {
                Call(Hash.RENDER_SCRIPT_CAMS, false, true, 1000, false, false);
            }
            if (Game.IsControlJustPressed(0, Control.Attack))
            {
                var hit = wp.HitPosition;
                Bitch.Task.GoTo(hit);

                PTFX.StartNonLoopedAtCoord(ParticleEffectName, hit + new Vector3(0, 0, 1f));
            }
            if (Game.IsControlJustPressed(0, Control.WeaponWheelNext))
            {
                offsetY += 0.1f;
            }
            if (Game.IsControlJustPressed(0, Control.WeaponWheelPrev))
            {
                offsetY -= 0.1f;
            }

            if (Call<bool>(Hash.IS_CAM_RENDERING, camera))
            {
                Game.DisableControlThisFrame(0, Control.Attack);
                Game.DisableControlThisFrame(0, Control.WeaponWheelNext);
                Game.DisableControlThisFrame(0, Control.WeaponWheelPrev);
                Screen.Hud.HideComponentThisFrame(HudComponent.WeaponWheel);
            }

            LocalPlayer.CanControlCharacter = true;

            //DrawCoords(pos, LocalPlayer.Character.Rotation, sw, sh);
            //DrawCoords(pos + new Vector3(0, 0, 1f), Vector3.Zero, sw, sh);
            //DrawCoords(camPos, camRot, sw, sh);
            //DrawCoords(camPos + new Vector3(0, 0, 1f), Vector3.Zero, sw, sh);
        }

        public static string MatrixToString(Matrix m)
        {
            string buf = "[";
            float[] temp = m.Row1.ToArray().Concat(m.Row2.ToArray()).Concat(m.Row3.ToArray()).Concat(m.Row4.ToArray()).ToArray();

            buf += String.Join(", ", temp);

            return buf + "]";
        }
        
        public static string VectorToString(Vector3 v)
        {
            string buf = "";

            buf += String.Join(", ", v.ToArray().Select((vc) => vc.ToString("f04")));

            return buf + "";
        }

        #region DRAW
        private static void DrawHelper(Camera cam)
        {
            var camPos = cam.Position;
            var camRot = cam.Rotation;

            var colorFrustum = new int[] { 0xff, 0xaa, 0 };
            var colorCone = new int[] { 0xff, 0, 0 };
            var colorUp = new int[] { 0, 0xaa, 0xff };
            var colorTarget = new int[] { 0xff, 0xff, 0xff };
            var colorCross = new int[] { 0x33, 0x33, 0x33 };

            Dictionary<string, Vector3> points = new Dictionary<string, Vector3>();

            var setPoint = new Action<string, float, float, float>((id, x, y, z) => {
                //points.Add(id, ApplyMatrix(new Vector3(x, y, z), camProjectionMatrix2));
                points.Add(id, FuccProjection.ScreenToWorld(new Vector3(x, y, z), cam));
            });
            var addLine = new Action<string, string, int[]>((id1, id2, c) => {
                var from = camPos;
                var to = camPos;

                points.TryGetValue(id1, out from);
                points.TryGetValue(id2, out to);

                DrawLine(from, to, c[0], c[1], c[2]);
            });

            var w = 1;
            var h = 1;

            // center / target

            setPoint("c", 0, 0, -1);
            setPoint("t", 0, 0, 1);

            // near

            setPoint("n1", -w, -h, -1);
            setPoint("n2", w, -h, -1);
            setPoint("n3", -w, h, -1);
            setPoint("n4", w, h, -1);

            // far

            setPoint("f1", -w, -h, 1);
            setPoint("f2", w, -h, 1);
            setPoint("f3", -w, h, 1);
            setPoint("f4", w, h, 1);

            // up

            setPoint("u1", (float)(w * 0.7), (float)(h * 1.1), -1);
            setPoint("u2", (float)(-w * 0.7), (float)(h * 1.1), -1);
            setPoint("u3", 0, h * 2, -1);

            // cross

            setPoint("cf1", -w, 0, 1);
            setPoint("cf2", w, 0, 1);
            setPoint("cf3", 0, -h, 1);
            setPoint("cf4", 0, h, 1);

            setPoint("cn1", -w, 0, -1);
            setPoint("cn2", w, 0, -1);
            setPoint("cn3", 0, -h, -1);
            setPoint("cn4", 0, h, -1);

            // near

            addLine("n1", "n2", colorFrustum);
            addLine("n2", "n4", colorFrustum);
            addLine("n4", "n3", colorFrustum);
            addLine("n3", "n1", colorFrustum);

            // far

            addLine("f1", "f2", colorFrustum);
            addLine("f2", "f4", colorFrustum);
            addLine("f4", "f3", colorFrustum);
            addLine("f3", "f1", colorFrustum);

            // sides

            addLine("n1", "f1", colorFrustum);
            addLine("n2", "f2", colorFrustum);
            addLine("n3", "f3", colorFrustum);
            addLine("n4", "f4", colorFrustum);

            // up

            addLine("u1", "u2", colorUp);
            addLine("u2", "u3", colorUp);
            addLine("u3", "u1", colorUp);

            // target

            addLine("c", "t", colorTarget);
            addLine("p", "c", colorCross);

            // cross

            addLine("cn1", "cn2", colorCross);
            addLine("cn3", "cn4", colorCross);

            addLine("cf1", "cf2", colorCross);
            addLine("cf3", "cf4", colorCross);
        }

        private static void DrawCoords(Vector3 pos, Vector3 rot, float width, float height)
        {
            var x = FuccVector3.OffsetToGlobal(pos, rot, new Vector3(1f, 0f, 0f));
            var y = FuccVector3.OffsetToGlobal(pos, rot, new Vector3(0f, 1f, 0f));
            var z = FuccVector3.OffsetToGlobal(pos, rot, new Vector3(0f, 0f, 1f));

            DrawLine(pos, x, 255, 0, 0);
            DrawLine(pos, y, 0, 255, 0);
            DrawLine(pos, z, 0, 0, 255);

            var ox = new OutputArgument();
            var oy = new OutputArgument();

            Call(Hash.GET_SCREEN_COORD_FROM_WORLD_COORD, x, ox, oy);
            tx.Position = new PointF(ox.GetResult<float>() * width, oy.GetResult<float>() * height);

            Call(Hash.GET_SCREEN_COORD_FROM_WORLD_COORD, y, ox, oy);
            ty.Position = new PointF(ox.GetResult<float>() * width, oy.GetResult<float>() * height);

            Call(Hash.GET_SCREEN_COORD_FROM_WORLD_COORD, z, ox, oy);
            tz.Position = new PointF(ox.GetResult<float>() * width, oy.GetResult<float>() * height);

            tx.Draw();
            ty.Draw();
            tz.Draw();
        }

        private static void DrawLine(Vector3 from, Vector3 to, int r = 255, int g = 255, int b = 255)
        {
            Call(Hash.DRAW_LINE, from, to, r, g, b, 255);
        }

        private static void DrawCube(Vector3 pos, Vector3 size, int r = 255, int g = 255, int b = 255, int a = 255)
        {
            Call(Hash.DRAW_BOX, pos, pos + size / 2, r, g, b, a);
        }

        public static void DrawCap2(string t, float x = 10f, float y = 10f)
        {
            (new Text(t, new PointF(x, y), .2f, Color.FromArgb(255, 255, 255, 255))).Draw();
        }

        public static void DrawMatrix(string title, Matrix m, float x = 10f, float y = 10f, int r = 255, int g = 255, int b = 255)
        {
            DrawCap2(title, x, y - 10f);

            float height = 10f;
            float width = 90f;

            var txt = new Text("", new PointF(0, 0), .3f);
            txt.Color = Color.FromArgb(r, g, b);

            void DrawRow(Vector4 row, float offsetY)
            {
                void DrawCell(float v, float ox, float oy)
                {
                    txt.Caption = v.ToString("f04");
                    
                    txt.Position = new PointF(ox, oy);
                    txt.Draw();
                }

                var offsetX = x;

                DrawCell(row.X, offsetX, offsetY);
                offsetX += width;

                DrawCell(row.Y, offsetX + 5f, offsetY);
                offsetX += width;

                DrawCell(row.Z, offsetX + 10f, offsetY);
                offsetX += width;

                DrawCell(row.W, offsetX + 15f, offsetY);
            }

            DrawRow(m.Row1, y);
            DrawRow(m.Row2, y + height + 10f);
            DrawRow(m.Row3, y + height + 10f + height + 10f);
            DrawRow(m.Row4, y + height + 10f + height + 10f + height + 10f);
        }
        #endregion
    }
}
