Shader "Custom/SlimeLiquid2"
{
    Properties
    {
        _ColorShallow ("Shallow Color", Color) = (0.3, 1.0, 0.3, 0.5)
        _ColorDeep ("Deep Color", Color) = (0.0, 0.4, 0.0, 0.9)
        _RimColor ("Rim Color", Color) = (0.7, 1.0, 0.7, 1.0)
        _FresnelPower ("Fresnel Power", Range(0.1, 5.0)) = 1.8
        _WobbleSpeed ("Wobble Speed", Float) = 1.2
        _WobbleAmount ("Wobble Amount", Float) = 0.08
        _BubbleSpeed ("Bubble Speed", Float) = 0.8
        _BubbleStrength ("Bubble Strength", Range(0,1)) = 0.3
        _Glossiness ("Gloss", Range(0,1)) = 0.95
        _SpecPower ("Spec Power", Range(1,128)) = 64.0
        _GravityPull ("Gravity Pull", Range(0,1)) = 0.4
        _SurfaceTension ("Surface Tension", Range(0,1)) = 0.6
        _InternalGlow ("Internal Glow", Range(0,2)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

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
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float3 localPos : TEXCOORD3;
                float3 displaceNormal : TEXCOORD4;
            };

            fixed4 _ColorShallow, _ColorDeep, _RimColor;
            float _FresnelPower;
            float _WobbleSpeed, _WobbleAmount;
            float _BubbleSpeed, _BubbleStrength;
            float _Glossiness, _SpecPower;
            float _GravityPull, _SurfaceTension, _InternalGlow;

            // Hash noise
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
                    u.y
                );
            }

            float fbm(float2 p)
            {
                float v = 0.0; float a = 0.5;
                for (int i = 0; i < 5; i++)
                {
                    v += smoothNoise(p) * a;
                    p = p * 2.0 + float2(1.7, 9.2);
                    a *= 0.5;
                }
                return v;
            }

            // Hitung displacement amount saja (untuk normal reconstruction)
            float getDisplace(float3 lp, float t)
            {
                float2 uv1 = float2(lp.x + lp.z, lp.y) * 0.6;
                float2 uv2 = float2(lp.z - lp.x, lp.y + lp.x) * 0.4;

                float n1 = fbm(uv1 + t * _WobbleSpeed * 0.3);
                float n2 = fbm(uv2 - t * _WobbleSpeed * 0.2);
                float n3 = fbm(float2(lp.x, lp.z) * 0.8 + t * _WobbleSpeed * 0.15);

                // Gravity — bagian bawah lebih tertarik ke bawah
                float gravityMask = saturate(0.5 - lp.y);
                float gravity = sin(lp.x * 3.0 + t * 0.8) * cos(lp.z * 2.5 + t * 0.6);
                gravity *= gravityMask * _GravityPull;

                // Surface tension — tepi lebih "kencang"
                float edgeMask = abs(lp.x) + abs(lp.z);
                float tension = smoothstep(0.3, 0.8, edgeMask) * _SurfaceTension * 0.5;

                float organic = (n1 - 0.5) * 2.0 + (n2 - 0.5) * 0.8 + (n3 - 0.5) * 0.5;
                return (organic + gravity) * _WobbleAmount - tension * _WobbleAmount * 0.3;
            }

            v2f vert(appdata v)
            {
                v2f o;
                float t = _Time.y;
                float3 lp = v.vertex.xyz;

                // Displace vertex
                float disp = getDisplace(lp, t);
                float3 displaced = lp + v.normal * disp;
                v.vertex.xyz = displaced;

                // Reconstruct normal dari finite difference
                float eps = 0.01;
                float3 tangent  = normalize(float3(1, 0, 0));
                float3 bitangent = normalize(float3(0, 0, 1));

                float3 p1 = lp + tangent * eps;
                float3 p2 = lp + bitangent * eps;

                float d0 = disp;
                float d1 = getDisplace(p1, t);
                float d2 = getDisplace(p2, t);

                float3 dp1 = (p1 + v.normal * d1) - displaced;
                float3 dp2 = (p2 + v.normal * d2) - displaced;
                float3 reconstructedNormal = normalize(cross(dp1, dp2));

                // Blend normal asli dengan reconstructed
                float3 finalNormal = normalize(lerp(v.normal, reconstructedNormal, 0.7));

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(finalNormal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                o.localPos = lp;
                o.displaceNormal = finalNormal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                float t = _Time.y;

                // Fresnel
                float fresnel = pow(1.0 - saturate(dot(viewDir, normal)), _FresnelPower);

                // Depth color
                float depthFactor = saturate(i.localPos.y * 0.5 + 0.5);
                fixed4 baseColor = lerp(_ColorDeep, _ColorShallow, depthFactor);

                // Animated bubble highlight
                float2 uvB = float2(i.worldPos.x + i.worldPos.z, i.worldPos.y);
                float bubble = fbm(uvB * 1.2 + t * _BubbleSpeed * 0.15);
                float bubble2 = fbm(uvB * 2.5 - t * _BubbleSpeed * 0.1);
                bubble = smoothstep(0.58, 0.72, bubble) + smoothstep(0.62, 0.74, bubble2) * 0.5;

                // Internal glow — simulasi cahaya dari dalam
                float internalNoise = fbm(float2(i.worldPos.x, i.worldPos.z) * 0.8 + t * 0.2);
                float glow = smoothstep(0.4, 0.7, internalNoise) * _InternalGlow;

                // Multi-specular
                float3 lightDir1 = normalize(float3(1.0, 1.5, -1.0));
                float3 lightDir2 = normalize(float3(-0.5, 1.0, 0.8));
                float spec1 = pow(saturate(dot(normal, normalize(lightDir1 + viewDir))), _SpecPower) * _Glossiness;
                float spec2 = pow(saturate(dot(normal, normalize(lightDir2 + viewDir))), 16.0) * _Glossiness * 0.4;

                // Combine
                fixed4 col = baseColor;
                col.rgb += _RimColor.rgb * fresnel * 0.9;
                col.rgb += bubble * _BubbleStrength * _ColorShallow.rgb;
                col.rgb += glow * _ColorShallow.rgb;
                col.rgb += spec1 + spec2;

                col.a = saturate(baseColor.a + fresnel * 0.35 + bubble * 0.1);

                return col;
            }
            ENDCG
        }
    }
}