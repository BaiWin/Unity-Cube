Shader "Custom/Trampoline"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Radius("Radius", Float) = 1
        _Offset("Offset", Vector) = (0,1,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 tangent : TANGENT;
            float2 texcoord : TEXCOORD0;
        };

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half _Radius;
        float4 _Offset;
        uniform float4 GLOBAL_ObjWorldPos;
        uniform int GLOBAL_Attachment;
        uniform float4 GLOBAL_Direction;
        uniform float GLOBAL_ObjDistance;

        void vert(inout appdata v)
        {
            float3 worldSpacePosition = mul(unity_ObjectToWorld, v.vertex.xyz);
            float total_Dist = distance(worldSpacePosition.xyz, GLOBAL_ObjWorldPos); //Dist only used for specify which shader is currently working
            if (total_Dist < 3 && GLOBAL_Attachment == 1) {
                /*float4 new_Dir = float4(1, 1, 1, 1) - abs(GLOBAL_Direction);
                float4 difference = worldSpacePosition - GLOBAL_ObjWorldPos;
                float4 limitedAxisDifference = mul(new_Dir, difference);
                float vertex_Dist = length(limitedAxisDifference.xyz);

                if (vertex_Dist <= _Radius) {
                    v.vertex += (1 - (vertex_Dist / _Radius)) * -GLOBAL_Direction;
                }*/
                float vertex_Dist = length(v.vertex);
                vertex_Dist = distance(v.vertex.xyz, _Offset.xyz);
                if (vertex_Dist <= _Radius) {
                    v.vertex += -GLOBAL_Direction * (1 - saturate(vertex_Dist / _Radius)) * GLOBAL_ObjDistance;
                    //v.vertex += -GLOBAL_Direction;
                }
            }
            //_Offset = v.vertex;
            v.vertex.x += (1 - saturate(distance(v.vertex, _Offset) / _Radius));
            
        }

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
