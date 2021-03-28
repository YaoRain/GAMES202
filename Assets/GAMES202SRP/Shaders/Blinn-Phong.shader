Shader "MySRP/Blinn-Phong"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("BaseColor", Color) = (1,1,1,1)
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
            float4 _BaseColor;
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
                float4 col = tex2D(_MainTex, i.uv) * _BaseColor;
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
                float clipW = i.shadowClipPos.w;
                float2 shadowUV = float2((shadowNdc.x + 1)*0.5 ,1 - (shadowNdc.y + 1)*0.5);

                float dBlocker = tex2D(_ShadowMap, shadowUV);

                // block search
                float2 offSet = float2(1.0/1024, 1.0/1024);
                int blockerSize = (1.0-dBlocker) * 20;
                blockerSize = 1;
                float w11 = 1;
                float wOther = (1-w11)/8;
                float3x3 wight = {
                    {wOther, wOther, wOther},
                    {wOther, w11   , wOther},
                    {wOther, wOther, wOther}
                };
                int MAX_LOOP = 9;
                blockerSize = clamp(blockerSize, 1, MAX_LOOP);
                int2 blockSearchSize = int2(blockerSize, blockerSize);

                
                float2 blockSearchUV = shadowUV - offSet * (blockerSize/2);
                float avgBlockDepth = 0.0;
                int blockerNums = 0;

                [unroll(MAX_LOOP)]
                for(int i = 0; i < blockSearchSize.x; i++)
                {
                    [unroll(MAX_LOOP)]
                    for(int j = 0; j < blockSearchSize.y; j++)
                    {
                        float blockerPointDepth = tex2D(_ShadowMap, blockSearchUV);
                        bool isBlock = shadowNdc.z > blockerPointDepth + 0.005;
                        if(isBlock)
                        {
                            avgBlockDepth += blockerPointDepth;
                            blockerNums++;
                        }
                        blockSearchUV += float2(0, offSet.y);
                    }
                    blockSearchUV += float2(offSet.x, 0);
                }

                if(blockerNums > 0)
                {
                    avgBlockDepth /= blockerNums;
                } 
                else
                {
                    avgBlockDepth = 1.0;
                }          


                // 按照Texile做偏移；
                
                // filterSize 根据像素到阴影到距离
                float lightW = 40;
                int w = lightW * (shadowNdc.z - avgBlockDepth)/avgBlockDepth;
                // w = 9;
                w = clamp(w, 1, MAX_LOOP);
                int2 filterSize = int2(w,w);
                float2 filterUV = shadowUV - offSet * (w/2);
                int inShadowSum = 0;

                //float clipW = i.shadowClipPos.w;
                [unroll(MAX_LOOP)]
                for(int x = 0; x < filterSize.x; x++)
                {
                    [unroll(MAX_LOOP)]
                    for(int j = 0; j < filterSize.y; j++)
                    {
                        bool inShadow = (shadowNdc.z > tex2D(_ShadowMap, filterUV) + 0.005)&&(clipW > 0);
                        if(inShadow)
                        {
                            inShadowSum++;
                        }   
                        filterUV = filterUV + float2(0, offSet.y);
                    };
                    filterUV = filterUV + float2(offSet.x, 0);
                };
                // inShadowSum = 50;
                float shadowAtten = 1.0 - (inShadowSum * 1.0) / (filterSize.x * filterSize.y);
                col = col * 0.5  + (specularColor + pointSpecularColor);
                col *= shadowAtten;
                // col *= !inShadow;
                return col;
                //return shadowNdc.z;
            }
            ENDHLSL
        }
    }
}
