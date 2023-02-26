Shader "Custom/MyShader"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _TwirlStrength("TwirlStrength", Float) = 1
        _CellDensity("CellDensity", Float) = 1
        _Speed("Speed", Float) = 1
        _Dissolve("Dissolve", Float) = 1
        _Alpha("Alpha", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 200
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows keepalpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        half _TwirlStrength;
        half _CellDensity;
        half _Speed;
        half _Dissolve;
        fixed4 _Color;
        half _Alpha;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        
        void Unity_Twirl_float(float2 UV, float2 Center, float Strength, float2 Offset, out float2 Out)
        {
            float2 delta = UV - Center;
            float angle = Strength * length(delta);
            float x = cos(angle) * delta.x - sin(angle) * delta.y;
            float y = sin(angle) * delta.x + cos(angle) * delta.y;
            Out = float2(x + Center.x + Offset.x, y + Center.y + Offset.y);
        }

        inline float2 unity_voronoi_noise_randomVector(float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)) * 46839.32);
            return float2(sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
        }

        void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    float2 lattice = float2(x, y);
                    float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
                    if (d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }

        float4 Unity_Remap_float4(float4 In, float2 InMinMax, float2 OutMinMax, float4 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            return Out;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            float2 uv;
            float cells;
            float voronoi_out;
            Unity_Twirl_float(IN.uv_MainTex, float2(0.5, 0.5), _TwirlStrength, _Time.y * _Speed, uv);
            Unity_Voronoi_float(pow(uv, _Dissolve), 0, _CellDensity, voronoi_out, cells);
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 result = mul(voronoi_out, c);
            o.Albedo = c.rgb;
            //o.Alpha = c.a;
            o.Emission = result.rgb;
            //// Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            o.Alpha = result.a;
            //o.Alpha = _Alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
