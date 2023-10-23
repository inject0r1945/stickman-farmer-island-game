Shader "Hidden/UnityHighlight"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.01
    }

        SubShader
        {
            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct VertIn
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertOut
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            VertOut VertexOutput(VertIn input)
            {
                VertOut output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.position = TransformObjectToHClip(input.position.xyz);
                output.uv = input.uv;
                return output;
            }
            ENDHLSL

            Tags { "RenderType" = "Opaque" }


                // #0: things that are visible (pass depth). 1 in alpha, 1 in red (SM3.0)
                Pass
                {
                // push towards camera a bit, so that coord mismatch due to dynamic batching is not affecting us
                    Offset -0.02, 0

                    HLSLPROGRAM
                    #pragma multi_compile_instancing
                    #pragma vertex PrepassVert
                    #pragma fragment PrepassFrag
                    #pragma target 3.0

                    float _ObjectId;

                    #define DRAW_COLOR float4(_ObjectId, 1, 1, 1)
                    #include "HSVHighlight.hlsl"
                    ENDHLSL

                }

                // #1: all the things, including the ones that fail the depth test. Additive blend, 1 in green, 1 in alpha
                Pass
                {
                    // push towards camera a bit, so that coord mismatch due to dynamic batching is not affecting us
                    Offset -0.02, 0

                    HLSLPROGRAM
                    #pragma multi_compile_instancing
                    #pragma vertex PrepassVert
                    #pragma fragment PrepassFrag
                    #pragma target 3.0

                    float _ObjectId;

                    #define DRAW_COLOR float4(0, 0, 1, 1)
                    #include "HSVHighlight.hlsl"
                    ENDHLSL
                }

                    // #2: separable blur pass, either horizontal or vertical
                    Pass
                    {
                        HLSLPROGRAM
                        #pragma multi_compile_instancing
                        #pragma vertex VertexOutput
                        #pragma fragment fragment
                        #pragma target 3.0

                        float2 _BlurDirection;
                        float _OutlineStrength;
                        TEXTURE2D_X(_MainTex);
                        SAMPLER(sampler_MainTex);
                        float4 _MainTex_TexelSize;

                        // 9-tap Gaussian kernel, that blurs green & blue channels,
                        // keeps red & alpha intact.
                        static const half4 kCurveWeights[9] = {
                            half4(0,0.0204001988,0.0204001988,0),
                            half4(0,0.0577929595,0.0577929595,0),
                            half4(0,0.1215916882,0.1215916882,0),
                            half4(0,0.1899858519,0.1899858519,0),
                            half4(1,0.2204586031,0.2204586031,1),
                            half4(0,0.1899858519,0.1899858519,0),
                            half4(0,0.1215916882,0.1215916882,0),
                            half4(0,0.0577929595,0.0577929595,0),
                            half4(0,0.0204001988,0.0204001988,0)
                        };

                        half4 fragment(VertOut i) : SV_Target
                        {
                            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                            float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);
                            float2 step = _MainTex_TexelSize.xy * _BlurDirection;
                            uv = uv - step * 4;
                            half4 col = 0;
                            for (int tap = 0; tap < 9; ++tap)
                            {
                                col += SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv) * kCurveWeights[tap];
                                uv += step;
                            }
                            return col * _OutlineStrength;
                        }
                        ENDHLSL
                    }

                        // #3: Compare object ids
                        Pass
                        {
                            HLSLPROGRAM
                            #pragma multi_compile_instancing
                            #pragma vertex VertexOutput
                            #pragma fragment fragment
                            #pragma target 3.0

                            TEXTURE2D_X(_MainTex);
                            SAMPLER(sampler_MainTex);
                            float4 _MainTex_TexelSize;

                            // 8 tap search around the current pixel to
                            // see if it borders with an object that has a
                            // different object id
                            static const half2 kOffsets[8] = {
                                half2(-1,-1),
                                half2(0,-1),
                                half2(1,-1),
                                half2(-1,0),
                                half2(1,0),
                                half2(-1,1),
                                half2(0,1),
                                half2(1,1)
                            };

                            half4 fragment(VertOut i) : SV_Target
                            {
                                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                                float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);
                                float4 currentTexel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv);
                                if (currentTexel.r == 0)
                                    return currentTexel;

                                // if the current texel borders with a
                                // texel that has a differnt object id
                                // set the alpha to 0. This implies an
                                // edge.
                                for (int tap = 0; tap < 8; ++tap)
                                {
                                    float id = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv + (kOffsets[tap] * _MainTex_TexelSize.xy)).r;
                                    if (id != 0 && id - currentTexel.r != 0)
                                    {
                                        currentTexel.a = 0;
                                    }
                                }

                                return currentTexel;
                            }
                            ENDHLSL
                        }

                            // #4: final postprocessing pass
                            Pass
                            {
                                HLSLPROGRAM
                                #pragma multi_compile_instancing
                                #pragma vertex VertexOutput
                                #pragma fragment fragment
                                #pragma target 3.0

                                TEXTURE2D_X(_MainTex);
                                SAMPLER(sampler_MainTex);
                                TEXTURE2D_X(_PrepassRT);
                                SAMPLER(sampler_PrepassRT);
                                TEXTURE2D_X(_BlurredRT);
                                SAMPLER(sampler_BlurredRT);
                                float4 _MainTex_TexelSize;
                                half4 _OutlineColor;

                                half4 fragment(VertOut i) : SV_Target
                                {
                                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                                    float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);

                                    float4 outline = saturate(SAMPLE_TEXTURE2D_X(_BlurredRT, sampler_BlurredRT, uv) - SAMPLE_TEXTURE2D_X(_PrepassRT, sampler_PrepassRT, uv));

                                    half4 col = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv);

                                    return lerp(col, _OutlineColor, outline.b);
                                }
                                ENDHLSL
                            }
        }
}