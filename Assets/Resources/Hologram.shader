Shader "Custom/Hologram"
{
    Properties
    {
        _Color          ("Hologram Color",          Color)          = (0.0, 0.5, 1.0, 1.0)
        _EmissionPower  ("Emission Power",          Range(1, 6))    = 2.5

        _ScanlineColor  ("Scanline Bright Color",   Color)          = (0.1, 0.8, 1.0, 1.0)
        _ScanlineDark   ("Scanline Dark Color",     Color)          = (0.0, 0.15, 0.35, 1.0)
        _ScanlineDensity("Scanline Density",        Range(10, 300)) = 90
        _ScanlineSpeed  ("Scanline Scroll Speed",   Range(0, 5))    = 1.2
        _ScanlineSharp  ("Scanline Sharpness",      Range(1, 20))   = 8.0
        _ScanlineBrightWidth("Bright Band Width",   Range(0.01, 0.5)) = 0.18

        _NoiseScale     ("Noise Scale",             Range(10, 200)) = 60.0
        _NoiseStrength  ("Noise Strength",          Range(0, 1))    = 0.25
        _NoiseSpeed     ("Noise Speed",             Range(0, 5))    = 0.8

        _FresnelPower   ("Fresnel Power",           Range(0.1, 8))  = 2.5
        _FresnelBoost   ("Fresnel Boost",           Range(0, 3))    = 1.2

        _Opacity        ("Base Opacity",            Range(0, 1))    = 0.82
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        // ─── Back face pass (inner glow depth) ───────────────────────────
        Pass
        {
            Name "BACK"
            Cull   Front
            ZWrite Off
            Blend  SrcAlpha One

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4  _Color;
            float   _EmissionPower;
            float   _FresnelPower;
            float   _FresnelBoost;
            float   _Opacity;

            struct a2v { float4 vertex:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float3 wNorm:TEXCOORD0; float3 wPos:TEXCOORD1; float2 uv:TEXCOORD2; };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.wNorm = UnityObjectToWorldNormal(v.normal);
                o.wPos  = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv    = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 V       = normalize(_WorldSpaceCameraPos - i.wPos);
                float  NdotV   = saturate(dot(normalize(i.wNorm), V));
                float  fresnel = pow(1.0 - NdotV, _FresnelPower) * _FresnelBoost;
                float  alpha   = fresnel * _Opacity * 0.45;
                return fixed4(_Color.rgb * _EmissionPower * 0.6, alpha);
            }
            ENDCG
        }

        // ─── Front face pass ─────────────────────────────────────────────
        Pass
        {
            Name "FRONT"
            Cull   Back
            ZWrite Off
            Blend  SrcAlpha One

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4  _Color;
            fixed4  _ScanlineColor;
            fixed4  _ScanlineDark;
            float   _EmissionPower;
            float   _ScanlineDensity;
            float   _ScanlineSpeed;
            float   _ScanlineSharp;
            float   _ScanlineBrightWidth;
            float   _NoiseScale;
            float   _NoiseStrength;
            float   _NoiseSpeed;
            float   _FresnelPower;
            float   _FresnelBoost;
            float   _Opacity;

            struct a2v { float4 vertex:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; };
            struct v2f
            {
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                float3 wNorm : TEXCOORD1;
                float3 wPos  : TEXCOORD2;
            };

            // ── Minimal hash noise ────────────────────────────────────────
            float hash2(float2 p)
            {
                p = frac(p * float2(127.1, 311.7));
                p += dot(p, p + 19.19);
                return frac(p.x * p.y);
            }

            float noise2(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(
                    lerp(hash2(i),          hash2(i + float2(1,0)), f.x),
                    lerp(hash2(i + float2(0,1)), hash2(i + float2(1,1)), f.x),
                    f.y);
            }

            v2f vert(a2v v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.uv    = v.uv;
                o.wNorm = UnityObjectToWorldNormal(v.normal);
                o.wPos  = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // ── Fresnel ───────────────────────────────────────────────
                float3 V     = normalize(_WorldSpaceCameraPos - i.wPos);
                float  NdotV = saturate(dot(normalize(i.wNorm), V));
                float  fres  = pow(1.0 - NdotV, _FresnelPower) * _FresnelBoost;

                // ── Scanline bands ────────────────────────────────────────
                // scroll in world-Y so lines are consistent across the mesh
                float  scanCoord = i.wPos.y * _ScanlineDensity + _Time.y * _ScanlineSpeed;
                float  t         = frac(scanCoord);

                // Smooth bright band of width _ScanlineBrightWidth
                float  band      = smoothstep(0.0, _ScanlineBrightWidth, t)
                                 * smoothstep(_ScanlineBrightWidth * 2.0, _ScanlineBrightWidth, t);
                band = pow(band, 1.0 / _ScanlineSharp); // sharpen

                // ── Noise overlay (surface texture) ───────────────────────
                float2 noiseUV = uv * _NoiseScale + float2(0, _Time.y * _NoiseSpeed);
                float  n       = noise2(noiseUV);
                n = n * 2.0 - 1.0; // -1..1

                // ── Color blend: dark base + bright scanline ──────────────
                float3 baseCol  = _ScanlineDark.rgb;
                float3 brightCol = _ScanlineColor.rgb * _EmissionPower;
                float3 col = lerp(baseCol, brightCol, band);

                // Add noise into the mix (subtle texture)
                col += _Color.rgb * n * _NoiseStrength * 0.5;

                // Fresnel rim adds bright cyan edge
                float3 rimCol = _ScanlineColor.rgb * fres * _EmissionPower;
                col += rimCol;

                // ── Alpha ──────────────────────────────────────────────────
                // Dark bands are nearly transparent, bright bands opaque
                float alpha = lerp(_Opacity * 0.55, _Opacity, band);
                alpha = lerp(alpha, 1.0, fres * 0.6);   // rim always visible

                // Noise adds slight alpha variation
                alpha += n * _NoiseStrength * 0.15;
                alpha  = saturate(alpha);

                return fixed4(col, alpha);
            }
            ENDCG
        }
    }

    FallBack "Unlit/Transparent"
}