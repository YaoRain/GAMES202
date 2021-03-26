Shader "MySRP/ShadowCast"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "Shadow Map"
            Tags{"LightMode" = "ShadowCast"}
            ZWrite On
            ZTest LEqual
    
            HLSLPROGRAM
            #pragma target 5.0
            #pragma multi_compile UseShadow
            #pragma vertex vert
            #pragma fragment frag

            

            #include "UnityCG.cginc"

            struct VertShaderIn
            {
                float4 vertex : POSITION;
            };

            struct FragShaderIn
            {
                float4 postion : SV_POSITION;
            };

            sampler2D _ShadowMap;
            float3 _PointLightPos;
 
            float4x4 _VP;
            float4x4 _M;
            float4x4 _MVP;
            // float4x4 unity_ObjectToWorld;

            FragShaderIn vert (VertShaderIn v)
            {
                FragShaderIn o;
                float4x4 mvp = mul(_VP, _M);
                o.postion = mul(mvp, v.vertex);
                o.postion.z = 1 - o.postion.z;
                //o.postion = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (FragShaderIn i) : SV_Target
            {
                float depth = i.postion.z / i.postion.w;
                depth = depth * 0.5 + 0.5;
                depth = 1 - depth;
                return depth;
                //return 0;
            }
            ENDHLSL
        }
    }
}
