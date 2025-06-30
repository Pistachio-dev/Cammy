
using Hypostasis.Game.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace Cammy
{
    public static unsafe class CodeMovableCamera
    {
        public static bool Enabled => enabled && gameCamera != null;

        private static bool enabled = false;
        public static GameCamera* gameCamera;
        private static Vector3 SavedPosition = new Vector3();
        private static Vector3 SavedViewPosition = new Vector3();
        private static Vector3 SavedLookAtPosition = new Vector3();
        public static GameObject* Target = null;
        private static float SavedZoom = 1;
        public static float HRotation = 0;
        public static float VRotation = 0;
        public static float Zoom = 1;
        public static Vector3 Position = new Vector3();
        public static Vector3 ViewPosition = new Vector3();
        public static Vector3 LookAtPosition = new Vector3();
        public static Vector3 A4 = Vector3.One;
        public static CameraConfigPreset previousPreset = null;
        public static float SavedFoV = 0;

        public static void Enable()
        {
            gameCamera = Common.CameraManager->worldCamera;

            SavedPosition = new Vector3(gameCamera->x, gameCamera->y, gameCamera->z);
            SavedViewPosition = new Vector3(gameCamera->viewX, gameCamera->viewY, gameCamera->viewZ);
            SavedLookAtPosition = new Vector3(gameCamera->lookAtX, gameCamera->lookAtY, gameCamera->lookAtZ);
            SavedZoom = gameCamera->currentZoom;
            SavedFoV = gameCamera->currentFoV;
            gameCamera->mode = 1;
            Game.cameraNoClippyReplacer.Enable();
            previousPreset = PresetManager.ActivePreset;
            FreeCam.freeCamPreset.Apply();
            enabled = true;
        }

        public static void MovePosition(float x, float y, float z)
        {
            Position.X = x; Position.Y = y; Position.Z = z;
        }

        public static void ApplyDistance(float distance, float horizontalAngleRads, float verticalAngleRads)
        {
            var x = Position.X + (distance * Math.Cos(verticalAngleRads) * Math.Sin(horizontalAngleRads));
            var z = Position.Z + (distance * Math.Cos(verticalAngleRads) * Math.Cos(horizontalAngleRads));
            var y = Position.Y + (distance * Math.Sin(verticalAngleRads));

            Position = new Vector3((float)x, (float)y, (float)z);
        }

        public static void Update()
        {
            if (!Enabled || gameCamera == null)
            {
                return;
            }
            gameCamera->x = Position.X; gameCamera->y = Position.Y; gameCamera->z = Position.Z;
        }
        public static void Disable()
        {
            Position = SavedPosition;
            ViewPosition = SavedViewPosition;
            LookAtPosition = SavedLookAtPosition;
            Zoom = SavedZoom;

            PresetManager.DefaultPreset.Apply();
            PresetManager.DisableCameraPresets();
            PresetManager.CurrentPreset = previousPreset;
            gameCamera->currentZoom = gameCamera->interpolatedZoom = SavedZoom;
            gameCamera->currentFoV = (float)Math.Max(SavedFoV, 1.5);
            enabled = false;
            Game.cameraNoClippyReplacer.Disable();
        }

        public static void Dispose()
        {
            Disable();
        }
    }
}
