Shader "Unlit/SlashRibbon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color1 ("Color 1", Color) = (1,1,1,1)
        [HDR]_Color2 ("Color 2", Color) = (1,1,1,1)
        _Type ("Type", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="transparent" "Queue"="transparent" }
        LOD 100
        ZWrite off
        Cull off
        Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal: TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color1;
            float4 _Color2;
            float _Type;

            v2f vert (appdata v)
            {
                v2f o;
                // float waveFreq = 1;
                // float waveAmp = 0.1;
                // float waveSpeed = 1;
                // float wave = sin(v.uv.x * waveFreq + _Time.y * waveSpeed ) * waveAmp;

                // v.vertex.xyz += v.normal * wave; // fade di ujung
                // v.vertex.y += (sin(_Time.y * 2 + v.uv.x * 2) * 0.1);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = lerp(_Color1, _Color2, _Type);
               
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
