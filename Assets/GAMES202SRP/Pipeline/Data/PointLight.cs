using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace MySRP
{
    public class PointLight
    {
        [Range(0, 10)]
        public Vector3 position;
        public Color finalColor;
        static public PointLight pointLight;

        public static void SetLightData(VisibleLight visibleLight)
        {
            if(visibleLight.lightType == LightType.Point)
            {
                //Debug.Log("get point light");
                //Shader.EnableKeyword("UsePointLight");
                Light light = visibleLight.light;
                if(pointLight is null)
                {
                    pointLight = new PointLight();
                }
                pointLight.position = light.transform.position;
                pointLight.finalColor = visibleLight.finalColor;

                GlobalShaderProperties.SetPointLight(pointLight.finalColor, pointLight.position);
            }
        }
    }

    
}