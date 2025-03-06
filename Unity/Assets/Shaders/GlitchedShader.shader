Shader "Custom/GlitchedShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineMoveSpeed ("Scanline Move Speed", Float) = 1.0
        _ScanlineIntensity ("Scanline Intensity", Range(0.0, 1.0)) = 0.5
        _ScanlineCount ("Scanline Count", Float) = 200.0
        _GlitchTimer ("Glitch Timer", Float) = 5.0
        _GlitchDuration ("Glitch Duration", Float) = 0.1
        _GlitchIntensity ("Glitch Intensity", Range(0.0, 1.0)) = 0.5
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
            float _ScanlineMoveSpeed;
            float _ScanlineIntensity;
            float _ScanlineCount;
            float _GlitchTimer;
            float _GlitchDuration;
            float _GlitchIntensity;

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
                float2 glitchUV = i.uv;

                if (glitchStrength > 0.0)
                {
                    if (sin(_Time.y * 2.0) > 0.0)
                    {
                        // Horizontal displacement
                        glitchUV.x += (random(float2(glitchTime, i.uv.y)) - 0.5) * glitchStrength;
                    }
                    else
                    {
                        // Vertical displacement
                        glitchUV.y += (random(float2(glitchTime, i.uv.x)) - 0.5) * glitchStrength;
                    }
                }

                // Sample the texture with the glitchy UV
                fixed4 col = tex2D(_MainTex, glitchUV);

                // Skip processing for fully transparent pixels
                if (col.a == 0.0)
                {
                    return col;
                }

                // Scanlines effect
                float scanline = sin((i.uv.y + _Time.y * _ScanlineMoveSpeed) * _ScanlineCount * 3.14159);
                scanline = (scanline * 0.5 + 0.5) * _ScanlineIntensity;
                col.rgb *= 1.0 - scanline;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}