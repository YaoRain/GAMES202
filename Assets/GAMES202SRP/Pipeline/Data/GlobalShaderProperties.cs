using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace MySRP
{
    static internal class PreFrameBuffer
    {
        public static int _EnvironmentColor;
    }
    static internal class PreCameraBuffer
    {
        public static int _CameraPosition;
        public static int _VP;
    }
    static internal class PreLightBufer
    {
        public static int _LightColor;
        public static int _LightDir;
    }

    public class GlobalShaderProperties
    {
        public static void GetShaderPropertyIDs()
        {
            PreFrameBuffer._EnvironmentColor = Shader.PropertyToID("_EnvironmentColor");

            PreCameraBuffer._CameraPosition = Shader.PropertyToID("_CameraPosition");
            PreCameraBuffer._VP = Shader.PropertyToID("_VP");

            PreLightBufer._LightColor = Shader.PropertyToID("_LightColor");
            PreLightBufer._LightDir = Shader.PropertyToID("_LightDir");
        }

        public static void SetPreFrameBuffer()
        {
            Shader.SetGlobalColor(PreFrameBuffer._EnvironmentColor, new Color(0, 0.4f, 0));
        }
        public static void SetPreCameraBuffer(Vector3 camPos, Matrix4x4 vp)
        {
            Shader.SetGlobalVector(PreCameraBuffer._CameraPosition, camPos);
            Shader.SetGlobalMatrix(PreCameraBuffer._VP, vp);
        }
        public static void SetPreLightBuffer()
        {
            Shader.SetGlobalColor(PreLightBufer._LightColor, new Color(1, 1, 1));
            Shader.SetGlobalVector(PreLightBufer._LightDir, new Vector3(0, -1, -1));
        }
    }
}