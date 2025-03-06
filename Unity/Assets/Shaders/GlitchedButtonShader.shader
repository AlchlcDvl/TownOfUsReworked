Shader "Custom/GlitchedButtonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitchTimer ("Glitch Timer", Float) = 2.5
        _GlitchDuration ("Glitch Duration", Float) = 0.1
        _GlitchIntensity ("Glitch Intensity", Range(0.0, 1.0)) = 0.5
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
            float _GlitchTimer;
            float _GlitchDuration;
            float _GlitchIntensity;
            float _Percent;
            float _Desat;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float random (float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate time-based glitch effect
                float glitchTime = fmod(_Time.y, _GlitchTimer);
                float glitchStrength = smoothstep(_GlitchTimer - _GlitchDuration, _GlitchTimer, glitchTime) * _GlitchIntensity;

                // Alternate between horizontal and vertical displacement
                float displacementDirection = sin(_Time.y * 2.0) > 0.0 ? 1.0 : -1.0; // Alternates over time
                float2 glitchUV = i.uv;

                if (glitchStrength > 0.0)
                {
                    float2 newUV = glitchUV; // Temporary variable to store the new UV coordinates

                    if (displacementDirection > 0.0)
                    {
                        // Calculate horizontal displacement
                        newUV.x += (random(float2(glitchTime, i.uv.y)) - 0.5) * glitchStrength;
                    }
                    else
                    {
                        // Calculate vertical displacement
                        newUV.y += (random(float2(glitchTime, i.uv.x)) - 0.5) * glitchStrength;
                    }

                    // Only apply the displacement if the new UV coordinates are within bounds
                    if (newUV.x >= 0.0 && newUV.x <= 1.0 && newUV.y >= 0.0 && newUV.y <= 1.0)
                    {
                        glitchUV = newUV;
                    }
                }

                fixed4 col = tex2D(_MainTex, glitchUV);
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