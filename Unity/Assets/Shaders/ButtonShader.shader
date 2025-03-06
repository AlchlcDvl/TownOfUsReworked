Shader "Custom/ButtonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Percent ("Percent", Range(0.0, 1.0)) = 1.0
        _Desat ("Desaturation", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Percent;
            float _Desat;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float t = i.uv.y < 1 - _Percent ? 1 : _Desat; // Do a position check as well

                // Calculate the luminance (grayscale value)
                float luminance = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // Interpolate between the original color and the grayscale color based on t
                col.rgb = lerp(col.rgb, luminance, t);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}