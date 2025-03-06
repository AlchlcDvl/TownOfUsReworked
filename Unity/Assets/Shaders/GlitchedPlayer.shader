Shader "Custom/GlitchedPlayer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull front
        LOD 100

        Pass
        {
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float rand(float x, float y) {
                return frac(sin(x * 12.9898 + y * 78.233) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float intervalTime = floor(_Time.y / 0.02f) * 0.02f;
                float intervalTime2 = intervalTime + 2.793;

                float timePositionVal = intervalTime + UNITY_MATRIX_MV[0][3] + UNITY_MATRIX_MV[1][3];
                float timePositionVal2 = intervalTime2 + UNITY_MATRIX_MV[0][3] + UNITY_MATRIX_MV[1][3];

                float dispGlitchRandom = rand(timePositionVal, -timePositionVal);
                float colorGlitchRandom = rand(timePositionVal, timePositionVal);

                float rShiftRandom = (rand(-timePositionVal, timePositionVal) - 0.5) * 2;//0.07f;
                float gShiftRandom = (rand(-timePositionVal, -timePositionVal) - 0.5) * 2;//0.07f;
                float shiftLineOffset = float((rand(timePositionVal2, timePositionVal2) - 0.5) / 50);

                if (dispGlitchRandom < 1) {
                    i.uv.x += (rand(floor(i.uv.y / (0.2 + shiftLineOffset)) - timePositionVal, floor(i.uv.y / (0.2 + shiftLineOffset)) + timePositionVal) - 0.5) * 0.09f;
                    i.uv.x = fmod(i.uv.x, 1);

                    i.uv.x += (rand(floor(i.uv.y / 0.2) - intervalTime, floor(i.uv.y / 0.2) + intervalTime) - 0.5) * 0.09f;
                    i.uv.x = fmod(i.uv.x, 1);
                }

                fixed4 normalC = tex2D(_MainTex, i.uv);
                fixed4 rShifted = tex2D(_MainTex, float2(i.uv.x + rShiftRandom, i.uv.y + rShiftRandom));
                fixed4 gShifted = tex2D(_MainTex, float2(i.uv.x + gShiftRandom, i.uv.y + gShiftRandom));

                fixed4 c = fixed4(0.0, 0.0, 0.0, 0.0);
                c.r = rShifted.r;
                c.g = gShifted.g;
                c.b = 0;
                c.a = normalC.a;

                return c;
            }
            ENDCG
        }
    }
}
