Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        [HDR] _Color("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
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
        Tags { "Quene" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half _Glossiness;
            half _Metallic;
            half _TwirlStrength;
            half _CellDensity;
            half _Speed;
            half _Dissolve;
            fixed4 _Color;
            half _Alpha;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float cells;
                float2 uv;
                float voronoi_out;
                Unity_Twirl_float(i.uv, float2(0.5, 0.5), _TwirlStrength, _Time.y* _Speed, uv);
                Unity_Voronoi_float(pow(uv, _Dissolve), 0, _CellDensity, voronoi_out, cells);
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 result = mul(voronoi_out, col) * _Color;
                result.a = result.r;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return result;
            }
            ENDCG
        }
    }
}
