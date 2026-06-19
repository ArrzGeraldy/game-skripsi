Shader "Unlit/WeaponTrail"
{
    Properties
    {
        _BaseColor0("Base Color 0", Color) = (1,1,1,1)
        _TipColor0("Tip Color 0", Color) = (1,1,1,1)
        _BaseColor1("Base Color 1", Color) = (1,1,1,1)
        _TipColor1("Tip Color 1", Color) = (1,1,1,1)
        _NoiseTex("Noise Texture", 2D) = "bump" {}
        _Type("Type", range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Cull off
        ZWrite off
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float4 _BaseColor0;
            float4 _TipColor0;
            float4 _BaseColor1;
            float4 _TipColor1;
            float _Type;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // noise
                fixed4 noise = tex2D(_NoiseTex, i.uv);
                
                // mask A
                float maskA = (i.uv.y + 0.4) * i.uv.x * noise.r;
                maskA = smoothstep(0.2, 0.7, maskA);

                // mask B
                float maskB = (i.uv.y + 0.2) * i.uv.x;
                maskB = smoothstep(0.1, 0.8, maskB);

                // get mask by type
                float mask = lerp(maskB, maskA, _Type);

                // get color by type
                float4 baseColor = lerp(_BaseColor0, _BaseColor1, _Type);
                float4 tipColor = lerp(_TipColor0, _TipColor1, _Type);

                float4 col = lerp(baseColor, tipColor, mask);
                float intensity = lerp(1, 2.5, _Type);
                col.rgb *= intensity;
                col.a =  mask;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            // fixed4 frag (v2f i) : SV_Target
            // {
            //     float2 uv = i.uv;
            //     // uv *= 2;
            //     fixed4 noise = tex2D(_NoiseTex, uv);
            //     float mask = ((i.uv.y + 0.8) * i.uv.x) * noise.r;
            //     // mask = smoothstep(0.1, 0.8, mask);
            //     mask = smoothstep(0.2, 0.7, mask);

            //     float4 lerpColor =  lerp(_BaseColor, _TipColor, mask);
            //     float4 col = lerpColor;

            //     col.a =  mask;
            //     col.rgb *= 2.5;
            //     // apply fog
            //     UNITY_APPLY_FOG(i.fogCoord, col);
            //     return col;
            // }
            ENDCG
        }
    }
}
