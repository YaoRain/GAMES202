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
            ZTest GEqual
            // Cull Front
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
            float4x4 _ObjToWorldMatrix;

            FragShaderIn vert (VertShaderIn v)
            {
                FragShaderIn o;
                float4x4 mvp = mul(_VP, _ObjToWorldMatrix);
                o.postion = mul(mvp, v.vertex);
                return o;
            }

            float4 frag (FragShaderIn i) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}
