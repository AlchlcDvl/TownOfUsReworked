Shader "Custom/AberrationShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.1)) = 0.01
        _Speed ("Speed", Range(0, 10)) = 1.0 // Speed of the aberration change
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
            float _ChromaticAberration;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 originalColor = tex2D(_MainTex, i.uv);

                // Skip processing if the pixel is fully transparent (alpha = 0)
                if (originalColor.a == 0)
                {
                    return originalColor;
                }

                // Calculate a time-based offset using _Time.y (seconds since the scene started)
                float timeOffset = sin(_Time.y * _Speed) * 0.5 + 0.5; // Oscillates between 0 and 1
                float aberration = _ChromaticAberration * timeOffset;

                // Calculate UV offsets for each color channel
                float2 uvRed = i.uv + float2(aberration, aberration);
                float2 uvGreen = i.uv; // No offset for green
                float2 uvBlue = i.uv - float2(aberration, aberration);

                // Sample each color channel with its respective UV offset
                float red = tex2D(_MainTex, uvRed).r;
                float green = tex2D(_MainTex, uvGreen).g;
                float blue = tex2D(_MainTex, uvBlue).b;

                // Combine the channels into a final color
                fixed4 col = fixed4(red, green, blue, originalColor.a);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}