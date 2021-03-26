Shader "MySRP/Blinn-Phong"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
/*
        Pass 
        {
            Tags {"LightMode" = "ShadowCaster"}

            ColorMask 0

            HLSLPROGRAM
            #pragma target 3.5
            #pragma multi_compile_instancing
            #pragma vertex ShadowCasterPassVertex
            #pragma fragment ShadowCasterPassFragment
            #include "ShadowCasterPass.hlsl"
            ENDHLSL
        }
 */ 
        Pass
        {
            Name "Blinn-Phone Shading"
            Tags{"LightMode" = "baseDraw"}
            HLSLPROGRAM
            #pragma target 5.0
            #pragma multi_compile NoLight UseMainLight UsePointLight
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VertShaderIn
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct FragShaderIn
            {
                float3 worldPos : TEXCOORD1;
                float4 shadowClipPos : TEXCOORD2;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _ShadowMap;

            float3 _LightColor;
            float3 _LightDir;
            float3 _PointLightColor;
            float3 _PointLightPos;
            float3 _EnvironmentColor;
            float4x4 _VP;
            float3 _CameraPosition;
            float4x4 _ObjToWorldMatrix;

            FragShaderIn vert (VertShaderIn v)
            {
                FragShaderIn o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                float4x4 mvp = mul(_VP, _ObjToWorldMatrix);
                o.shadowClipPos = mul(mvp, v.vertex);


                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            float4 frag (FragShaderIn i) : SV_Target
            {
                // sample the texture
                float4 col = tex2D(_MainTex, i.uv);
                float3 viewDir = normalize(_CameraPosition - i.worldPos);
                float4 specularColor = float4(0,0,0,1);
                float4 pointSpecularColor = float4(0,0,0,1);
#if defined(UseMainLight)
                float3 reflectDir = normalize(reflect(_LightDir, i.normal));
                float3 specular = _LightColor * pow(max(dot(viewDir,reflectDir),0),35.0);
                specularColor = col * float4(specular,1) * 1;
#endif

#if defined(UsePointLight)
                float3 pointLightDir = i.worldPos - _PointLightPos;
                float3 pointLightRefDir = normalize(reflect(pointLightDir, i.normal));
                float3 pointSpecular = _PointLightColor * pow(max(dot(viewDir,pointLightRefDir),0),35.0);          
                pointSpecularColor = col * float4(pointSpecular,1);
#endif

                //float4 shadowLightSpacePos = mul(_VP, float4(i.worldPos,1));
                //float4 shadowNdc = shadowLightSpacePos / shadowLightSpacePos.w;
                float4 shadowNdc = i.shadowClipPos / i.shadowClipPos.w;
                float2 shadowUV = float2((shadowNdc.x + 1)*0.5 ,1 - (shadowNdc.y + 1)*0.5);
                float shadowDepth = shadowNdc.z * 0.5 + 0.5;
                bool inShadow = shadowNdc.z > tex2D(_ShadowMap, shadowUV) + 0.01; 

                col = col * 0.5  + specularColor + pointSpecularColor;
                col *= !inShadow;
                return col;
                //return shadowNdc.z;
            }
            ENDHLSL
        }
    }
}
