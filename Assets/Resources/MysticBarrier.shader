Shader "Unlit/MysticBarrier"
{
    Properties
    {
        _RimColor ("Rim Color", Color) = (0.0, 0.8, 1.0, 1.0)
        _CoreColor ("Core Color", Color) = (0.0, 0.4, 0.8, 0.3)
        _FresnelPower ("Fresnel Power", Range(0.1, 8.0)) = 3.0
        _FresnelIntensity ("Fresnel Intensity", Range(0, 5.0)) = 2.0
        _ScanlineSpeed ("Scanline Speed", Float) = 0.5
        _ScanlineScale ("Scanline Scale", Float) = 8.0
        _ScanlineStrength ("Scanline Strength", Range(0,1)) = 0.3
        _SparkSpeed ("Spark Speed", Float) = 1.0
        _SparkScale ("Spark Scale", Float) = 15.0
        _SparkStrength ("Spark Strength", Range(0,2)) = 0.8
        _PulseSpeed ("Pulse Speed", Float) = 1.5
        _PulseStrength ("Pulse Strength", Range(0,1)) = 0.2
        _IntersectionColor ("Intersection Color", Color) = (0.0, 1.0, 1.0, 1.0)
        _IntersectionWidth ("Intersection Width", Range(0,2)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
        Blend One One  // Additive blending — makin glow!
        ZWrite Off
        Cull Off       // Render dua sisi

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos       : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos  : TEXCOORD1;
                float3 viewDir   : TEXCOORD2;
                float2 uv        : TEXCOORD3;
                float3 localPos  : TEXCOORD4;
            };

            fixed4 _RimColor, _CoreColor, _IntersectionColor;
            float _FresnelPower, _FresnelIntensity;
            float _ScanlineSpeed, _ScanlineScale, _ScanlineStrength;
            float _SparkSpeed, _SparkScale, _SparkStrength;
            float _PulseSpeed, _PulseStrength;
            float _IntersectionWidth;

            // Hash & noise
            float hash(float2 p)
            {
                p = frac(p * float2(127.1, 311.7));
                p += dot(p, p + 19.19);
                return frac(p.x * p.y);
            }

            float smoothNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(
                    lerp(hash(i), hash(i + float2(1,0)), u.x),
                    lerp(hash(i + float2(0,1)), hash(i + float2(1,1)), u.x),
                    u.y);
            }

            float fbm(float2 p)
            {
                float v = 0.0, a = 0.5;
                for (int i = 0; i < 4; i++)
                {
                    v += smoothNoise(p) * a;
                    p = p * 2.1 + float2(1.7, 9.2);
                    a *= 0.5;
                }
                return v;
            }

            // Hexagonal grid distance
            float hexGrid(float2 p, float scale)
            {
                p *= scale;
                float2 r = float2(1.0, 1.732);
                float2 h = r * 0.5;
                float2 a = fmod(p, r) - h;
                float2 b = fmod(p - h, r) - h;
                float2 gv = dot(a,a) < dot(b,b) ? a : b;
                return length(gv);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                o.uv = v.uv;
                o.localPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                float t = _Time.y;

                // === FRESNEL ===
                float fresnel = pow(1.0 - saturate(dot(viewDir, normal)), _FresnelPower);
                fresnel *= _FresnelIntensity;

                // Pulse fresnel
                float pulse = 1.0 + sin(t * _PulseSpeed) * _PulseStrength;
                fresnel *= pulse;

                // === SCANLINES (horizontal bands) ===
                float scanline = sin(i.worldPos.y * _ScanlineScale - t * _ScanlineSpeed * 3.0);
                scanline = pow(saturate(scanline), 3.0) * _ScanlineStrength;

                // === HEX GRID PATTERN ===
                float2 uvHex = float2(i.uv.x, i.uv.y + t * 0.02);
                float hex = hexGrid(uvHex, 6.0);
                hex = 1.0 - smoothstep(0.38, 0.45, hex); // outline hex
                hex *= 0.15; // subtle

                // === SPARKS / STARS ===
                float2 uvSpark = float2(
                    i.worldPos.x + i.worldPos.z,
                    i.worldPos.y - t * _SparkSpeed * 0.3
                ) * _SparkScale * 0.08;

                float spark = fbm(uvSpark);
                float spark2 = fbm(uvSpark * 2.3 + float2(5.1, 3.7));
                spark = pow(smoothstep(0.62, 0.78, spark), 2.0);
                spark += pow(smoothstep(0.65, 0.80, spark2), 2.0) * 0.5;
                spark *= _SparkStrength;

                // === INTERSECTION GLOW (ground) ===
                // Simulasi pakai posisi Y rendah
                float groundFade = saturate(1.0 - (i.localPos.y + 0.5) / _IntersectionWidth);
                groundFade = pow(groundFade, 2.0) * 3.0;

                // === COMBINE ===
                fixed4 col = fixed4(0,0,0,0);

                // Core transparan
                col += _CoreColor * 0.4;

                // Fresnel rim
                col += _RimColor * fresnel;

                // Scanlines ikut warna rim
                col += _RimColor * scanline;

                // Hex pattern
                col += _RimColor * hex;

                // Sparks
                col += _RimColor * spark;

                // Ground intersection
                col += _IntersectionColor * groundFade;

                // Alpha
                col.a = saturate(fresnel * 0.5 + scanline + spark * 0.3 + groundFade * 0.5 + hex);

                return col;
            }
            ENDCG
        }
    }
}