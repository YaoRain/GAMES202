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
            Name "Blinn-Phone Shading"
            Tags{"LightMode" = "baseDraw"}
            HLSLPROGRAM
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
                float4 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float3 _LightColor;
            float3 _LightDir;
            float _LightIntensity;
            float3 _EnvironmentColor;
            float4x4 _VP;
            float3 _CameraPosition;

            // float4x4 unity_ObjectToWorld;

            FragShaderIn vert (VertShaderIn v)
            {
                FragShaderIn o;
                //o.vertex = mul(_VP,mul(unity_ObjectToWorld, v.vertex));
                o.worldPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            float4 frag (FragShaderIn i) : SV_Target
            {
                // sample the texture
                float4 col = tex2D(_MainTex, i.uv);

                float3 reflectDir = normalize(reflect(-_LightDir, i.normal));
                float3 viewDir = normalize(_CameraPosition - i.worldPos.xyz);
                float3 specular = _LightColor*pow(max(dot(viewDir,reflectDir),0),35.0);
                col = col * float4(1,1,1,1)*0.2 + col * float4(specular,1) * 1;

                return col;
            }
            ENDHLSL
        }
    }
}
