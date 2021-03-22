using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace MySRP
{
    public class MainLight
    {
        [Range(0, 10)]
        public Vector3 dir;
        public Color finalColor;
        static public MainLight mainLight;

        public static void SetLightData(VisibleLight visibleLight)
        {
            if(visibleLight.lightType == LightType.Directional)
            {
                //Shader.EnableKeyword("UseMainLight");
                Light light = visibleLight.light;
                if(mainLight is null)
                {
                    mainLight = new MainLight();
                }
                mainLight.dir = light.transform.forward;    
                mainLight.finalColor = visibleLight.finalColor;

                GlobalShaderProperties.SetMainLight(mainLight.finalColor, mainLight.dir);
            }
        }
    }

    
}