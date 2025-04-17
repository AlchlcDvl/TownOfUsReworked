Shader "Custom/GlitchedShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineMoveSpeed ("Scanline Move Speed", Float) = 1.0
        _ScanlineIntensity ("Scanline Intensity", Range(0.0, 1.0)) = 0.5
        _ScanlineCount ("Scanline Count", Float) = 200.0
        [MaterialToggle] _Glitches ("Enable Glitches", Float) = 1
        _GlitchTimer ("Glitch Timer", Float) = 5.0
        _GlitchDuration ("Glitch Duration", Float) = 0.1
        _GlitchIntensity ("Glitch Intensity", Range(0.0, 1.0)) = 0.5
        [IntRange] _EffectCount ("Effect Count", Range(1, 8)) = 1
        [MaterialToggle] _FlipX ("Enable X Axis Flip", Float) = 0
        [MaterialToggle] _FlipY ("Enable Y Axis Flip", Float) = 0
        [MaterialToggle] _PixelVert ("Enable Vertical Pixel Displacements", Float) = 0
        [MaterialToggle] _PixelHorz ("Enable Horizontal Pixel Displacements", Float) = 0
        [MaterialToggle] _ChunkVert ("Enable Vertical Chunk Displacements", Float) = 0
        _ChunkSizeVert ("Vertical Chunk Size", Range(0.01, 0.3)) = 0.05
        [MaterialToggle] _ChunkHorz ("Enable Horizontal Chunk Displacements", Float) = 0
        _ChunkSizeHorz ("Horizontal Chunk Size", Range(0.01, 0.3)) = 0.05
        [MaterialToggle] _ZoomX ("Enable X Axis Zoom", Float) = 0
        [MaterialToggle] _ZoomY ("Enable Y Axis Zoom", Float) = 0
        _MinXScale ("Min X Scale", Range(0.1, 2.0)) = 0.8
        _MaxXScale ("Max X Scale", Range(0.1, 2.0)) = 1.2
        _MinYScale ("Min Y Scale", Range(0.1, 2.0)) = 0.8
        _MaxYScale ("Max Y Scale", Range(0.1, 2.0)) = 1.2
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

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
            float4 _MainTex_ST;
            float _Glitches;
            float _ChunkSizeHorz;
            float _ChunkSizeVert;
            float _ScanlineMoveSpeed;
            float _ScanlineIntensity;
            float _ScanlineCount;
            float _GlitchTimer;
            float _GlitchDuration;
            float _GlitchIntensity;
            float _FlipX;
            float _FlipY;
            float _PixelVert;
            float _PixelHorz;
            float _ChunkVert;
            float _ChunkHorz;
            float _ZoomX;
            float _ZoomY;
            float _EffectCount;
            float _MinXScale;
            float _MaxXScale;
            float _MinYScale;
            float _MaxYScale;

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

            float2 verticalChunkDisplace(float2 uv, float2 chunkSeed, float intensity)
            {
                float chunkXID = floor(uv.x / _ChunkSizeVert);
                float2 displacement = float2(0, random(chunkSeed + chunkXID) - 0.5);
                return uv + (displacement * intensity);
            }

            float2 horizontalChunkDisplace(float2 uv, float2 chunkSeed, float intensity)
            {
                float chunkYID = floor(uv.y / _ChunkSizeHorz);
                float2 displacement = float2(random(chunkSeed + chunkYID) - 0.5, 0);
                return uv + (displacement * intensity);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float glitchTime = fmod(_Time.y, _GlitchTimer);
                float glitchStrength = smoothstep(_GlitchTimer - _GlitchDuration, _GlitchTimer, glitchTime) * _GlitchIntensity;
                float2 modifiedUV = i.uv;

                if (glitchStrength > 0 && _Glitches)
                {
                    float glitchPhase = floor(_Time.y / _GlitchTimer);
                    float2 glitchSeed = float2(glitchPhase, 0.0);
                    float effectSeed = random(glitchSeed);

                    int effectIndices[7];
                    float weights[7];
                    int enabledCount = 0;

                    if (_PixelHorz > 0)
                    {
                        effectIndices[enabledCount] = 0;
                        weights[enabledCount++] = 1.0;
                    }

                    if (_PixelVert > 0)
                    {
                        effectIndices[enabledCount] = 1;
                        weights[enabledCount++] = 1.0;
                    }

                    if (_ChunkHorz > 0)
                    {
                        effectIndices[enabledCount] = 2;
                        weights[enabledCount++] = 1.0;
                    }

                    if (_ChunkVert > 0)
                    {
                        effectIndices[enabledCount] = 3;
                        weights[enabledCount++] = 1.0;
                    }

                    if (_ZoomX > 0)
                    {
                        effectIndices[enabledCount] = 4;
                        weights[enabledCount++] = 1.0;
                    }

                    if (_FlipX > 0)
                    {
                        effectIndices[enabledCount] = 5;
                        weights[enabledCount++] = 1.0;
                    }

                    if (_FlipY > 0)
                    {
                        effectIndices[enabledCount] = 6;
                        weights[enabledCount++] = 1.0;
                    }

                    if (_ZoomY > 0)
                    {
                        effectIndices[enabledCount] = 7;
                        weights[enabledCount++] = 1.0;
                    }

                    if (enabledCount == 0)
                    {
                        effectIndices[enabledCount] = (int)(random(glitchSeed + float2(1.0, 0.0)) * 7);
                        weights[enabledCount++] = 1.0;
                    }

                    int effectsApplied = 0;
                    float2 usedSeed = glitchSeed;

                    for (int iter = 0; iter < _EffectCount && effectsApplied < enabledCount; iter++)
                    {
                        float totalWeight = 0.0;

                        for (int w = 0; w < enabledCount; w++)
                            totalWeight += weights[w];

                        float pick = random(usedSeed) * totalWeight;
                        usedSeed += 0.1234;

                        int selected = 0;

                        while (pick > weights[selected] && selected < enabledCount - 1)
                        {
                            pick -= weights[selected];
                            selected++;
                        }

                        int effect = effectIndices[selected];
                        weights[selected] *= 0.3;

                        if (effect == 5)
                        {
                            modifiedUV.x = 1.0 - modifiedUV.x;
                            effectsApplied++;
                        }
                        else if (effect == 6)
                        {
                            modifiedUV.y = 1.0 - modifiedUV.y;
                            effectsApplied++;
                        }
                    }

                    for (int iter = 0; iter < _EffectCount && effectsApplied < enabledCount; iter++)
                    {
                        float totalWeight = 0.0;

                        for (int w = 0; w < enabledCount; w++)
                            totalWeight += weights[w];

                        float pick = random(usedSeed) * totalWeight;
                        usedSeed += 0.1234;

                        int selected = 0;

                        while (pick > weights[selected] && selected < enabledCount - 1)
                        {
                            pick -= weights[selected];
                            selected++;
                        }

                        int effect = effectIndices[selected];
                        weights[selected] *= 0.3;

                        if (effect == 4)
                        {
                            float zoomScale = lerp(_MinXScale, _MaxXScale, random(glitchSeed + float2(1.0, 0.0)));
                            modifiedUV.x = ((modifiedUV.x - 0.5) * zoomScale) + 0.5;
                            effectsApplied++;
                        }
                        else if (effect == 7)
                        {
                            float zoomScale = lerp(_MinYScale, _MaxYScale, random(glitchSeed + float2(1.0, 0.0)));
                            modifiedUV.y = ((modifiedUV.y - 0.5) * zoomScale) + 0.5;
                            effectsApplied++;
                        }
                    }

                    for (int iter = 0; iter < _EffectCount && effectsApplied < enabledCount; iter++)
                    {
                        float totalWeight = 0.0;

                        for (int w = 0; w < enabledCount; w++)
                            totalWeight += weights[w];

                        float pick = random(usedSeed) * totalWeight;
                        usedSeed += 0.1234;

                        int selected = 0;

                        while (pick > weights[selected] && selected < enabledCount - 1)
                        {
                            pick -= weights[selected];
                            selected++;
                        }

                        int effect = effectIndices[selected];
                        weights[selected] *= 0.3;

                        if (effect == 0)
                        {
                            modifiedUV.x += (random(float2(glitchTime, modifiedUV.y)) - 0.5) * glitchStrength;
                            effectsApplied++;
                        }
                        else if (effect == 1)
                        {
                            modifiedUV.y += (random(float2(glitchTime, modifiedUV.x)) - 0.5) * glitchStrength;
                            effectsApplied++;
                        }
                        else if (effect == 2)
                        {
                            modifiedUV = verticalChunkDisplace(modifiedUV, glitchSeed, glitchStrength);
                            effectsApplied++;
                        }
                        else if (effect == 3)
                        {
                            modifiedUV = horizontalChunkDisplace(modifiedUV, glitchSeed, glitchStrength);
                            effectsApplied++;
                        }
                    }
                }

                fixed4 col = tex2D(_MainTex, modifiedUV);

                if (col.a > 0.0)
                {
                    float scanline = sin((i.uv.y + _Time.y * _ScanlineMoveSpeed) * _ScanlineCount * 3.14159);
                    scanline = (scanline * 0.5 + 0.5) * _ScanlineIntensity;
                    col.rgb *= 1.0 - scanline;
                }

                return col;
            }

            ENDCG
        }
    }

    FallBack "Sprites/Diffuse"
}