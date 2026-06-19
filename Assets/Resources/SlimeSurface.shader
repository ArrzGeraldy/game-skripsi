Shader "Custom/SlimeSurface"
{
    Properties
    {
        _Color ("Slime Color", Color) = (0.2, 0.8, 0.2, 0.7)
        _Glossiness ("Smoothness", Range(0,1)) = 0.9
        _FresnelPower ("Fresnel Power", Range(0,5)) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        CGPROGRAM
        #pragma surface surf Standard alpha:fade

        struct Input
        {
            float3 viewDir;
            float3 worldPos;
        };

        fixed4 _Color;
        half _Glossiness;
        float _FresnelPower;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float fresnel = pow(1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)), _FresnelPower);

            o.Albedo = _Color.rgb + fresnel * 0.2;
            o.Smoothness = _Glossiness;
            o.Metallic = 0;
            o.Alpha = _Color.a + fresnel * 0.2;
        }
        ENDCG
    }

    FallBack "Diffuse"
}