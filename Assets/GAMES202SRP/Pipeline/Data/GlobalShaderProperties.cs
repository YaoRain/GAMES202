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

        public static int _PointLightColor;
        public static int _PointLightPos;
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
            PreLightBufer._PointLightColor = Shader.PropertyToID("_PointLightColor");
            PreLightBufer._PointLightPos = Shader.PropertyToID("_PointLightPos");
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
        public static void SetMainLight(Color color, Vector3 dir)
        {
            Shader.SetGlobalColor(PreLightBufer._LightColor, color);
            Shader.SetGlobalVector(PreLightBufer._LightDir, dir);
        }
        public static void SetPointLight(Color color, Vector3 position)
        {
            Shader.SetGlobalColor(PreLightBufer._PointLightColor, color);
            Shader.SetGlobalVector(PreLightBufer._PointLightPos, position);
        }
    }
}