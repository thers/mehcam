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
using static CitizenFX.Core.Native.API;

namespace topcam3
{
    public class TopCam : BaseScript
    {
        public TopCam()
        {
            EventHandlers["onClientResourceStop"] += new Action<string>((resName) => {
                if (resName == GetCurrentResourceName())
                {
                    RenderScriptCams(false, true, 1000, false, false);
                    DestroyCam(camera.Handle, false);
                    Bitch.Delete();
                }
            });

            Tick += OnTick;
        }

        private bool inited = false;

        private Camera camera;

        public Ped Bitch;
        public ParticleEffectsAsset PTFX;

        public static string Asset = "scr_rcbarry1";
        public static string ParticleEffectName = "scr_alien_teleport";

        public float offsetY = 0f;

        public async Task OnTick()
        {            
            var pos = LocalPlayer.Character.Position;

            var camPos =  Vector3.Add(pos, new Vector3(2, offsetY, 2));
            if (!inited)
            {
                inited = true;

                camera = World.CreateCamera(
                    camPos,
                    new Vector3(0f, 0f, 0f),
                    60f
                );
                RenderScriptCams(true, true, 1000, false, false);

                Bitch = await World.CreatePed(new Model(PedHash.Beach01AFY), camPos);
                PTFX = new ParticleEffectsAsset(Asset);
            }

            camera.Position = camPos;
            camera.PointAt(LocalPlayer.Character);

            var rx = Game.GetControlNormal(0, (Control)239);
            var ry = Game.GetControlNormal(0, (Control)240);
            
            var farPos = Projection.ScreenToWorld(new Vector2(rx, ry), camera);
            var farWorldPos = farPos - Vector3.Normalize(camPos - farPos) * 100f;

            var wp = World.Raycast(
                camPos,
                farWorldPos,
                IntersectOptions.Map
            );

            Call(Hash._SHOW_CURSOR_THIS_FRAME, new InputArgument[0]);

            if (Game.IsControlJustPressed(0, Control.PhoneRight))
            {
                RenderScriptCams(true, true, 1000, false, false);
            }
            if (Game.IsControlJustPressed(0, Control.PhoneLeft))
            {
                RenderScriptCams(false, true, 1000, false, false);
            }
            if (Game.IsControlJustPressed(0, Control.Attack))
            {
                var hit = wp.HitPosition;
                Bitch.Task.GoTo(hit);

                PTFX.StartNonLoopedAtCoord(ParticleEffectName, hit + new Vector3(0, 0, 1f));
            }
            if (Game.IsControlJustPressed(0, Control.WeaponWheelNext))
            {
                offsetY += 0.2f;
            }
            if (Game.IsControlJustPressed(0, Control.WeaponWheelPrev))
            {
                offsetY -= 0.2f;
            }

            if (IsCamRendering(camera.Handle))
            {
                Game.DisableControlThisFrame(0, Control.Attack);
                Game.DisableControlThisFrame(0, Control.WeaponWheelNext);
                Game.DisableControlThisFrame(0, Control.WeaponWheelPrev);
                Screen.Hud.HideComponentThisFrame(HudComponent.WeaponWheel);
            }
        }
    }
}
