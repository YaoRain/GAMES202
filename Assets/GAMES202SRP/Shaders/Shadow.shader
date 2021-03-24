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
        Pass
        {
            Name "Shadow"
            Tags{"LightMode" = "Shadow"}
            HLSLPROGRAM
            #pragma target 5.0
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
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float3 _LightColor;
            float3 _LightDir;
            float3 _PointLightColor;
            float3 _PointLightPos;
            float3 _EnvironmentColor;
            float4x4 _VP;
            float3 _CameraPosition;

            sampler2D _ShadowMap;

            // float4x4 unity_ObjectToWorld;

            FragShaderIn vert (VertShaderIn v)
            {
                FragShaderIn o;
                //o.vertex = mul(_VP,mul(unity_ObjectToWorld, v.vertex));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            float4 frag (FragShaderIn i) : SV_Target
            {
                // sample the texture
                float4 col;

                return col;
            }
            ENDHLSL
        }
    }
}
