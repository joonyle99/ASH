Shader "Fire"
{
    Properties
    {
        [NoScaleOffset] _MainTex("MainTex", 2D) = "white" {}
        _Speed("Speed", Vector) = (0, -0.5, 0, 0)
        [HDR]_Color("Color", Color) = (2, 2, 2, 0)
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15

    
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "AlphaTest"
            "ShaderGraphShader" = "true"
            "ShaderGraphTargetId" = "UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
            // LightMode: <None>
        }

        // Render State
        Cull Back
        Blend One Zero
        ZTest [unity_GUIZTestMode]
        ZWrite On

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]
        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>

        // Defines

        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_COLOR
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _ALPHATEST_ON 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv2 : TEXCOORD2;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float4 texCoord2;
             float4 color;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv2;
             float4 VertexColor;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float4 interp4 : INTERP4;
             float3 interp5 : INTERP5;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

        PackedVaryings PackVaryings(Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz = input.positionWS;
            output.interp1.xyz = input.normalWS;
            output.interp2.xyzw = input.texCoord0;
            output.interp3.xyzw = input.texCoord2;
            output.interp4.xyzw = input.color;
            output.interp5.xyz = input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

        Varyings UnpackVaryings(PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.texCoord2 = input.interp3.xyzw;
            output.color = input.interp4.xyzw;
            output.viewDirectionWS = input.interp5.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }


        // --------------------------------------------------
        // Graph

        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float2 _Speed;
        float4 _MainTex_TexelSize;
        float4 _Color;
        CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_OneMinus_float(float In, out float Out)
            {
                Out = 1 - In;
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A * B;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }


            inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
            {
                float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                UV = frac(sin(mul(UV, m)));
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
                        float2 lattice = float2(x,y);
                        float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


            inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
            {
                float angle = dot(uv, float2(12.9898, 78.233));
                #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
            #endif
            return frac(sin(angle) * 43758.5453);
        }

        inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
        {
            return (1.0 - t) * a + (t * b);
        }


        inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);

            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0 = Unity_SimpleNoise_RandomValue_float(c0);
            float r1 = Unity_SimpleNoise_RandomValue_float(c1);
            float r2 = Unity_SimpleNoise_RandomValue_float(c2);
            float r3 = Unity_SimpleNoise_RandomValue_float(c3);

            float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
            float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
            float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
        {
            float t = 0.0;

            float freq = pow(2.0, float(0));
            float amp = pow(0.5, float(3 - 0));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3 - 1));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3 - 2));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

            Out = t;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }

        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif

        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
            float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
            Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
            float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
            float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
            float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
            float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
            float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
            float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
            Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
            UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
            float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
            Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
            float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
            float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
            float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
            float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
            float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
            Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
            float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
            Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
            float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
            float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
            float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
            float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
            float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
            Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
            float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
            Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
            float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
            Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
            float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
            Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
            surface.BaseColor = (_Multiply_4401e795467248a1947fb3451b629452_Out_2.xyz);
            surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
            surface.AlphaClipThreshold = 0.5;
            return surface;
        }

        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal = input.normalOS;
            output.ObjectSpaceTangent = input.tangentOS.xyz;
            output.ObjectSpacePosition = input.positionOS;

            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

        #endif







            output.uv0 = input.texCoord0;
            output.uv2 = input.texCoord2;
            output.VertexColor = input.color;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
        }

        // --------------------------------------------------
        // Main

        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif

        ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD2
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                 float3 positionOS : POSITION;
                 float3 normalOS : NORMAL;
                 float4 tangentOS : TANGENT;
                 float4 uv0 : TEXCOORD0;
                 float4 uv2 : TEXCOORD2;
                 float4 color : COLOR;
                #if UNITY_ANY_INSTANCING_ENABLED
                 uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
            struct Varyings
            {
                 float4 positionCS : SV_POSITION;
                 float4 texCoord0;
                 float4 texCoord2;
                 float4 color;
                #if UNITY_ANY_INSTANCING_ENABLED
                 uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            struct SurfaceDescriptionInputs
            {
                 float4 uv0;
                 float4 uv2;
                 float4 VertexColor;
                 float3 TimeParameters;
            };
            struct VertexDescriptionInputs
            {
                 float3 ObjectSpaceNormal;
                 float3 ObjectSpaceTangent;
                 float3 ObjectSpacePosition;
            };
            struct PackedVaryings
            {
                 float4 positionCS : SV_POSITION;
                 float4 interp0 : INTERP0;
                 float4 interp1 : INTERP1;
                 float4 interp2 : INTERP2;
                #if UNITY_ANY_INSTANCING_ENABLED
                 uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.interp0.xyzw = input.texCoord0;
                output.interp1.xyzw = input.texCoord2;
                output.interp2.xyzw = input.color;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.interp0.xyzw;
                output.texCoord2 = input.interp1.xyzw;
                output.color = input.interp2.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float2 _Speed;
            float4 _MainTex_TexelSize;
            float4 _Color;
            CBUFFER_END

                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);

                // Graph Includes
                // GraphIncludes: <None>

                // -- Property used by ScenePickingPass
                #ifdef SCENEPICKINGPASS
                float4 _SelectionID;
                #endif

                // -- Properties used by SceneSelectionPass
                #ifdef SCENESELECTIONPASS
                int _ObjectId;
                int _PassValue;
                #endif

                // Graph Functions

                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }

                void Unity_OneMinus_float(float In, out float Out)
                {
                    Out = 1 - In;
                }

                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }

                void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                {
                    Out = A * B;
                }

                void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                {
                    Out = UV * Tiling + Offset;
                }


                inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                {
                    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                    UV = frac(sin(mul(UV, m)));
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
                            float2 lattice = float2(x,y);
                            float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                {
                    float angle = dot(uv, float2(12.9898, 78.233));
                    #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                    // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                    angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                #endif
                return frac(sin(angle) * 43758.5453);
            }

            inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
            {
                return (1.0 - t) * a + (t * b);
            }


            inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                return t;
            }
            void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
            {
                float t = 0.0;

                float freq = pow(2.0, float(0));
                float amp = pow(0.5, float(3 - 0));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3 - 1));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3 - 2));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                Out = t;
            }

            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }

            void Unity_Step_float(float Edge, float In, out float Out)
            {
                Out = step(Edge, In);
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                surface.AlphaClipThreshold = 0.5;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

            #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

            #endif







                output.uv0 = input.texCoord0;
                output.uv2 = input.texCoord2;
                output.VertexColor = input.color;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                    return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif

            ENDHLSL
            }
            Pass
            {
                Name "DepthNormalsOnly"
                Tags
                {
                    "LightMode" = "DepthNormalsOnly"
                }

                // Render State
                Cull Back
                ZTest LEqual
                ZWrite On

                // Debug
                // <None>

                // --------------------------------------------------
                // Pass

                HLSLPROGRAM

                // Pragmas
                #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag

                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>

                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>

                // Defines

                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define ATTRIBUTES_NEED_TEXCOORD1
                #define ATTRIBUTES_NEED_TEXCOORD2
                #define ATTRIBUTES_NEED_COLOR
                #define VARYINGS_NEED_NORMAL_WS
                #define VARYINGS_NEED_TANGENT_WS
                #define VARYINGS_NEED_TEXCOORD0
                #define VARYINGS_NEED_TEXCOORD2
                #define VARYINGS_NEED_COLOR
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
                #define _ALPHATEST_ON 1
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                // custom interpolator pre-include
                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                // --------------------------------------------------
                // Structs and Packing

                // custom interpolators pre packing
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv0 : TEXCOORD0;
                     float4 uv1 : TEXCOORD1;
                     float4 uv2 : TEXCOORD2;
                     float4 color : COLOR;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 normalWS;
                     float4 tangentWS;
                     float4 texCoord0;
                     float4 texCoord2;
                     float4 color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                     float4 uv0;
                     float4 uv2;
                     float4 VertexColor;
                     float3 TimeParameters;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 interp0 : INTERP0;
                     float4 interp1 : INTERP1;
                     float4 interp2 : INTERP2;
                     float4 interp3 : INTERP3;
                     float4 interp4 : INTERP4;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };

                PackedVaryings PackVaryings(Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.interp0.xyz = input.normalWS;
                    output.interp1.xyzw = input.tangentWS;
                    output.interp2.xyzw = input.texCoord0;
                    output.interp3.xyzw = input.texCoord2;
                    output.interp4.xyzw = input.color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }

                Varyings UnpackVaryings(PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.normalWS = input.interp0.xyz;
                    output.tangentWS = input.interp1.xyzw;
                    output.texCoord0 = input.interp2.xyzw;
                    output.texCoord2 = input.interp3.xyzw;
                    output.color = input.interp4.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }


                // --------------------------------------------------
                // Graph

                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float2 _Speed;
                float4 _MainTex_TexelSize;
                float4 _Color;
                CBUFFER_END

                    // Object and Global properties
                    SAMPLER(SamplerState_Linear_Repeat);
                    TEXTURE2D(_MainTex);
                    SAMPLER(sampler_MainTex);

                    // Graph Includes
                    // GraphIncludes: <None>

                    // -- Property used by ScenePickingPass
                    #ifdef SCENEPICKINGPASS
                    float4 _SelectionID;
                    #endif

                    // -- Properties used by SceneSelectionPass
                    #ifdef SCENESELECTIONPASS
                    int _ObjectId;
                    int _PassValue;
                    #endif

                    // Graph Functions

                    void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                    {
                        Out = A * B;
                    }

                    void Unity_OneMinus_float(float In, out float Out)
                    {
                        Out = 1 - In;
                    }

                    void Unity_Multiply_float_float(float A, float B, out float Out)
                    {
                        Out = A * B;
                    }

                    void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                    {
                        Out = A * B;
                    }

                    void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                    {
                        Out = UV * Tiling + Offset;
                    }


                    inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                    {
                        float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                        UV = frac(sin(mul(UV, m)));
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
                                float2 lattice = float2(x,y);
                                float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                    inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                    {
                        float angle = dot(uv, float2(12.9898, 78.233));
                        #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                        // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                        angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                    #endif
                    return frac(sin(angle) * 43758.5453);
                }

                inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                {
                    return (1.0 - t) * a + (t * b);
                }


                inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                {
                    float2 i = floor(uv);
                    float2 f = frac(uv);
                    f = f * f * (3.0 - 2.0 * f);

                    uv = abs(frac(uv) - 0.5);
                    float2 c0 = i + float2(0.0, 0.0);
                    float2 c1 = i + float2(1.0, 0.0);
                    float2 c2 = i + float2(0.0, 1.0);
                    float2 c3 = i + float2(1.0, 1.0);
                    float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                    float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                    float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                    float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                    float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                    float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                    float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                    return t;
                }
                void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                {
                    float t = 0.0;

                    float freq = pow(2.0, float(0));
                    float amp = pow(0.5, float(3 - 0));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                    freq = pow(2.0, float(1));
                    amp = pow(0.5, float(3 - 1));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                    freq = pow(2.0, float(2));
                    amp = pow(0.5, float(3 - 2));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                    Out = t;
                }

                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }

                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }

                // Custom interpolators pre vertex
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };

                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }

                // Custom interpolators, pre surface
                #ifdef FEATURES_GRAPH_VERTEX
                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                {
                return output;
                }
                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                #endif

                // Graph Pixel
                struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };

                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                    float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                    Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                    float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                    float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                    float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                    float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                    float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                    float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                    Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                    UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                    float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                    float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                    Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                    float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                    float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                    Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                    float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                    float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                    float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                    Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                    float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                    Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                    float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                    float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                    float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                    float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                    float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                    Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                    float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                    Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                    float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                    Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                    float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                    Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                    surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                    surface.AlphaClipThreshold = 0.5;
                    return surface;
                }

                // --------------------------------------------------
                // Build Graph Inputs
                #ifdef HAVE_VFX_MODIFICATION
                #define VFX_SRP_ATTRIBUTES Attributes
                #define VFX_SRP_VARYINGS Varyings
                #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                #endif
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                    output.ObjectSpaceNormal = input.normalOS;
                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                    output.ObjectSpacePosition = input.positionOS;

                    return output;
                }
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                    // FragInputs from VFX come from two places: Interpolator or CBuffer.
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif







                    output.uv0 = input.texCoord0;
                    output.uv2 = input.texCoord2;
                    output.VertexColor = input.color;
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                        return output;
                }

                // --------------------------------------------------
                // Main

                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

                // --------------------------------------------------
                // Visual Effect Vertex Invocations
                #ifdef HAVE_VFX_MODIFICATION
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                #endif

                ENDHLSL
                }
                Pass
                {
                    Name "ShadowCaster"
                    Tags
                    {
                        "LightMode" = "ShadowCaster"
                    }

                    // Render State
                    Cull Back
                    ZTest LEqual
                    ZWrite On
                    ColorMask 0

                    // Debug
                    // <None>

                    // --------------------------------------------------
                    // Pass

                    HLSLPROGRAM

                    // Pragmas
                    #pragma target 4.5
                    #pragma exclude_renderers gles gles3 glcore
                    #pragma multi_compile_instancing
                    #pragma multi_compile _ DOTS_INSTANCING_ON
                    #pragma vertex vert
                    #pragma fragment frag

                    // DotsInstancingOptions: <None>
                    // HybridV1InjectedBuiltinProperties: <None>

                    // Keywords
                    #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
                    // GraphKeywords: <None>

                    // Defines

                    #define ATTRIBUTES_NEED_NORMAL
                    #define ATTRIBUTES_NEED_TANGENT
                    #define ATTRIBUTES_NEED_TEXCOORD0
                    #define ATTRIBUTES_NEED_TEXCOORD2
                    #define ATTRIBUTES_NEED_COLOR
                    #define VARYINGS_NEED_NORMAL_WS
                    #define VARYINGS_NEED_TEXCOORD0
                    #define VARYINGS_NEED_TEXCOORD2
                    #define VARYINGS_NEED_COLOR
                    #define FEATURES_GRAPH_VERTEX
                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                    #define SHADERPASS SHADERPASS_SHADOWCASTER
                    #define _ALPHATEST_ON 1
                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                    // custom interpolator pre-include
                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                    // Includes
                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                    // --------------------------------------------------
                    // Structs and Packing

                    // custom interpolators pre packing
                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                    struct Attributes
                    {
                         float3 positionOS : POSITION;
                         float3 normalOS : NORMAL;
                         float4 tangentOS : TANGENT;
                         float4 uv0 : TEXCOORD0;
                         float4 uv2 : TEXCOORD2;
                         float4 color : COLOR;
                        #if UNITY_ANY_INSTANCING_ENABLED
                         uint instanceID : INSTANCEID_SEMANTIC;
                        #endif
                    };
                    struct Varyings
                    {
                         float4 positionCS : SV_POSITION;
                         float3 normalWS;
                         float4 texCoord0;
                         float4 texCoord2;
                         float4 color;
                        #if UNITY_ANY_INSTANCING_ENABLED
                         uint instanceID : CUSTOM_INSTANCE_ID;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                        #endif
                    };
                    struct SurfaceDescriptionInputs
                    {
                         float4 uv0;
                         float4 uv2;
                         float4 VertexColor;
                         float3 TimeParameters;
                    };
                    struct VertexDescriptionInputs
                    {
                         float3 ObjectSpaceNormal;
                         float3 ObjectSpaceTangent;
                         float3 ObjectSpacePosition;
                    };
                    struct PackedVaryings
                    {
                         float4 positionCS : SV_POSITION;
                         float3 interp0 : INTERP0;
                         float4 interp1 : INTERP1;
                         float4 interp2 : INTERP2;
                         float4 interp3 : INTERP3;
                        #if UNITY_ANY_INSTANCING_ENABLED
                         uint instanceID : CUSTOM_INSTANCE_ID;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                        #endif
                    };

                    PackedVaryings PackVaryings(Varyings input)
                    {
                        PackedVaryings output;
                        ZERO_INITIALIZE(PackedVaryings, output);
                        output.positionCS = input.positionCS;
                        output.interp0.xyz = input.normalWS;
                        output.interp1.xyzw = input.texCoord0;
                        output.interp2.xyzw = input.texCoord2;
                        output.interp3.xyzw = input.color;
                        #if UNITY_ANY_INSTANCING_ENABLED
                        output.instanceID = input.instanceID;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        output.cullFace = input.cullFace;
                        #endif
                        return output;
                    }

                    Varyings UnpackVaryings(PackedVaryings input)
                    {
                        Varyings output;
                        output.positionCS = input.positionCS;
                        output.normalWS = input.interp0.xyz;
                        output.texCoord0 = input.interp1.xyzw;
                        output.texCoord2 = input.interp2.xyzw;
                        output.color = input.interp3.xyzw;
                        #if UNITY_ANY_INSTANCING_ENABLED
                        output.instanceID = input.instanceID;
                        #endif
                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                        #endif
                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                        #endif
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        output.cullFace = input.cullFace;
                        #endif
                        return output;
                    }


                    // --------------------------------------------------
                    // Graph

                    // Graph Properties
                    CBUFFER_START(UnityPerMaterial)
                    float2 _Speed;
                    float4 _MainTex_TexelSize;
                    float4 _Color;
                    CBUFFER_END

                        // Object and Global properties
                        SAMPLER(SamplerState_Linear_Repeat);
                        TEXTURE2D(_MainTex);
                        SAMPLER(sampler_MainTex);

                        // Graph Includes
                        // GraphIncludes: <None>

                        // -- Property used by ScenePickingPass
                        #ifdef SCENEPICKINGPASS
                        float4 _SelectionID;
                        #endif

                        // -- Properties used by SceneSelectionPass
                        #ifdef SCENESELECTIONPASS
                        int _ObjectId;
                        int _PassValue;
                        #endif

                        // Graph Functions

                        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                        {
                            Out = A * B;
                        }

                        void Unity_OneMinus_float(float In, out float Out)
                        {
                            Out = 1 - In;
                        }

                        void Unity_Multiply_float_float(float A, float B, out float Out)
                        {
                            Out = A * B;
                        }

                        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                        {
                            Out = A * B;
                        }

                        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                        {
                            Out = UV * Tiling + Offset;
                        }


                        inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                        {
                            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                            UV = frac(sin(mul(UV, m)));
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
                                    float2 lattice = float2(x,y);
                                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                        inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                        {
                            float angle = dot(uv, float2(12.9898, 78.233));
                            #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                            // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                            angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                        #endif
                        return frac(sin(angle) * 43758.5453);
                    }

                    inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                    {
                        return (1.0 - t) * a + (t * b);
                    }


                    inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                    {
                        float2 i = floor(uv);
                        float2 f = frac(uv);
                        f = f * f * (3.0 - 2.0 * f);

                        uv = abs(frac(uv) - 0.5);
                        float2 c0 = i + float2(0.0, 0.0);
                        float2 c1 = i + float2(1.0, 0.0);
                        float2 c2 = i + float2(0.0, 1.0);
                        float2 c3 = i + float2(1.0, 1.0);
                        float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                        float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                        float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                        float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                        float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                        float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                        float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                        return t;
                    }
                    void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                    {
                        float t = 0.0;

                        float freq = pow(2.0, float(0));
                        float amp = pow(0.5, float(3 - 0));
                        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                        freq = pow(2.0, float(1));
                        amp = pow(0.5, float(3 - 1));
                        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                        freq = pow(2.0, float(2));
                        amp = pow(0.5, float(3 - 2));
                        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                        Out = t;
                    }

                    void Unity_Add_float(float A, float B, out float Out)
                    {
                        Out = A + B;
                    }

                    void Unity_Step_float(float Edge, float In, out float Out)
                    {
                        Out = step(Edge, In);
                    }

                    // Custom interpolators pre vertex
                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                    // Graph Vertex
                    struct VertexDescription
                    {
                        float3 Position;
                        float3 Normal;
                        float3 Tangent;
                    };

                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                    {
                        VertexDescription description = (VertexDescription)0;
                        description.Position = IN.ObjectSpacePosition;
                        description.Normal = IN.ObjectSpaceNormal;
                        description.Tangent = IN.ObjectSpaceTangent;
                        return description;
                    }

                    // Custom interpolators, pre surface
                    #ifdef FEATURES_GRAPH_VERTEX
                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                    {
                    return output;
                    }
                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                    #endif

                    // Graph Pixel
                    struct SurfaceDescription
                    {
                        float Alpha;
                        float AlphaClipThreshold;
                    };

                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                    {
                        SurfaceDescription surface = (SurfaceDescription)0;
                        float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                        float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                        Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                        float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                        float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                        float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                        float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                        float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                        float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                        Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                        UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                        float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                        float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                        Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                        float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                        float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                        Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                        float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                        Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                        float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                        float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                        Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                        float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                        Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                        float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                        float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                        float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                        float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                        float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                        Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                        float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                        Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                        float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                        Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                        float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                        Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                        surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                        surface.AlphaClipThreshold = 0.5;
                        return surface;
                    }

                    // --------------------------------------------------
                    // Build Graph Inputs
                    #ifdef HAVE_VFX_MODIFICATION
                    #define VFX_SRP_ATTRIBUTES Attributes
                    #define VFX_SRP_VARYINGS Varyings
                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                    #endif
                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                    {
                        VertexDescriptionInputs output;
                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                        output.ObjectSpaceNormal = input.normalOS;
                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                        output.ObjectSpacePosition = input.positionOS;

                        return output;
                    }
                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                    {
                        SurfaceDescriptionInputs output;
                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                    #ifdef HAVE_VFX_MODIFICATION
                        // FragInputs from VFX come from two places: Interpolator or CBuffer.
                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                    #endif







                        output.uv0 = input.texCoord0;
                        output.uv2 = input.texCoord2;
                        output.VertexColor = input.color;
                        output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                    #else
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                    #endif
                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                            return output;
                    }

                    // --------------------------------------------------
                    // Main

                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

                    // --------------------------------------------------
                    // Visual Effect Vertex Invocations
                    #ifdef HAVE_VFX_MODIFICATION
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                    #endif

                    ENDHLSL
                    }
                    Pass
                    {
                        Name "SceneSelectionPass"
                        Tags
                        {
                            "LightMode" = "SceneSelectionPass"
                        }

                        // Render State
                        Cull Off

                        // Debug
                        // <None>

                        // --------------------------------------------------
                        // Pass

                        HLSLPROGRAM

                        // Pragmas
                        #pragma target 4.5
                        #pragma exclude_renderers gles gles3 glcore
                        #pragma vertex vert
                        #pragma fragment frag

                        // DotsInstancingOptions: <None>
                        // HybridV1InjectedBuiltinProperties: <None>

                        // Keywords
                        // PassKeywords: <None>
                        // GraphKeywords: <None>

                        // Defines

                        #define ATTRIBUTES_NEED_NORMAL
                        #define ATTRIBUTES_NEED_TANGENT
                        #define ATTRIBUTES_NEED_TEXCOORD0
                        #define ATTRIBUTES_NEED_TEXCOORD2
                        #define ATTRIBUTES_NEED_COLOR
                        #define VARYINGS_NEED_TEXCOORD0
                        #define VARYINGS_NEED_TEXCOORD2
                        #define VARYINGS_NEED_COLOR
                        #define FEATURES_GRAPH_VERTEX
                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                        #define SHADERPASS SHADERPASS_DEPTHONLY
                        #define SCENESELECTIONPASS 1
                        #define ALPHA_CLIP_THRESHOLD 1
                        #define _ALPHATEST_ON 1
                        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                        // custom interpolator pre-include
                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                        // Includes
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                        // --------------------------------------------------
                        // Structs and Packing

                        // custom interpolators pre packing
                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                        struct Attributes
                        {
                             float3 positionOS : POSITION;
                             float3 normalOS : NORMAL;
                             float4 tangentOS : TANGENT;
                             float4 uv0 : TEXCOORD0;
                             float4 uv2 : TEXCOORD2;
                             float4 color : COLOR;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : INSTANCEID_SEMANTIC;
                            #endif
                        };
                        struct Varyings
                        {
                             float4 positionCS : SV_POSITION;
                             float4 texCoord0;
                             float4 texCoord2;
                             float4 color;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : CUSTOM_INSTANCE_ID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                            #endif
                        };
                        struct SurfaceDescriptionInputs
                        {
                             float4 uv0;
                             float4 uv2;
                             float4 VertexColor;
                             float3 TimeParameters;
                        };
                        struct VertexDescriptionInputs
                        {
                             float3 ObjectSpaceNormal;
                             float3 ObjectSpaceTangent;
                             float3 ObjectSpacePosition;
                        };
                        struct PackedVaryings
                        {
                             float4 positionCS : SV_POSITION;
                             float4 interp0 : INTERP0;
                             float4 interp1 : INTERP1;
                             float4 interp2 : INTERP2;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : CUSTOM_INSTANCE_ID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                            #endif
                        };

                        PackedVaryings PackVaryings(Varyings input)
                        {
                            PackedVaryings output;
                            ZERO_INITIALIZE(PackedVaryings, output);
                            output.positionCS = input.positionCS;
                            output.interp0.xyzw = input.texCoord0;
                            output.interp1.xyzw = input.texCoord2;
                            output.interp2.xyzw = input.color;
                            #if UNITY_ANY_INSTANCING_ENABLED
                            output.instanceID = input.instanceID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            output.cullFace = input.cullFace;
                            #endif
                            return output;
                        }

                        Varyings UnpackVaryings(PackedVaryings input)
                        {
                            Varyings output;
                            output.positionCS = input.positionCS;
                            output.texCoord0 = input.interp0.xyzw;
                            output.texCoord2 = input.interp1.xyzw;
                            output.color = input.interp2.xyzw;
                            #if UNITY_ANY_INSTANCING_ENABLED
                            output.instanceID = input.instanceID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            output.cullFace = input.cullFace;
                            #endif
                            return output;
                        }


                        // --------------------------------------------------
                        // Graph

                        // Graph Properties
                        CBUFFER_START(UnityPerMaterial)
                        float2 _Speed;
                        float4 _MainTex_TexelSize;
                        float4 _Color;
                        CBUFFER_END

                            // Object and Global properties
                            SAMPLER(SamplerState_Linear_Repeat);
                            TEXTURE2D(_MainTex);
                            SAMPLER(sampler_MainTex);

                            // Graph Includes
                            // GraphIncludes: <None>

                            // -- Property used by ScenePickingPass
                            #ifdef SCENEPICKINGPASS
                            float4 _SelectionID;
                            #endif

                            // -- Properties used by SceneSelectionPass
                            #ifdef SCENESELECTIONPASS
                            int _ObjectId;
                            int _PassValue;
                            #endif

                            // Graph Functions

                            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                            {
                                Out = A * B;
                            }

                            void Unity_OneMinus_float(float In, out float Out)
                            {
                                Out = 1 - In;
                            }

                            void Unity_Multiply_float_float(float A, float B, out float Out)
                            {
                                Out = A * B;
                            }

                            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                            {
                                Out = A * B;
                            }

                            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                            {
                                Out = UV * Tiling + Offset;
                            }


                            inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                            {
                                float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                                UV = frac(sin(mul(UV, m)));
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
                                        float2 lattice = float2(x,y);
                                        float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                            inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                            {
                                float angle = dot(uv, float2(12.9898, 78.233));
                                #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                                // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                                angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                            #endif
                            return frac(sin(angle) * 43758.5453);
                        }

                        inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                        {
                            return (1.0 - t) * a + (t * b);
                        }


                        inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                        {
                            float2 i = floor(uv);
                            float2 f = frac(uv);
                            f = f * f * (3.0 - 2.0 * f);

                            uv = abs(frac(uv) - 0.5);
                            float2 c0 = i + float2(0.0, 0.0);
                            float2 c1 = i + float2(1.0, 0.0);
                            float2 c2 = i + float2(0.0, 1.0);
                            float2 c3 = i + float2(1.0, 1.0);
                            float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                            float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                            float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                            float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                            float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                            float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                            float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                            return t;
                        }
                        void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                        {
                            float t = 0.0;

                            float freq = pow(2.0, float(0));
                            float amp = pow(0.5, float(3 - 0));
                            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                            freq = pow(2.0, float(1));
                            amp = pow(0.5, float(3 - 1));
                            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                            freq = pow(2.0, float(2));
                            amp = pow(0.5, float(3 - 2));
                            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                            Out = t;
                        }

                        void Unity_Add_float(float A, float B, out float Out)
                        {
                            Out = A + B;
                        }

                        void Unity_Step_float(float Edge, float In, out float Out)
                        {
                            Out = step(Edge, In);
                        }

                        // Custom interpolators pre vertex
                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                        // Graph Vertex
                        struct VertexDescription
                        {
                            float3 Position;
                            float3 Normal;
                            float3 Tangent;
                        };

                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                        {
                            VertexDescription description = (VertexDescription)0;
                            description.Position = IN.ObjectSpacePosition;
                            description.Normal = IN.ObjectSpaceNormal;
                            description.Tangent = IN.ObjectSpaceTangent;
                            return description;
                        }

                        // Custom interpolators, pre surface
                        #ifdef FEATURES_GRAPH_VERTEX
                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                        {
                        return output;
                        }
                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                        #endif

                        // Graph Pixel
                        struct SurfaceDescription
                        {
                            float Alpha;
                            float AlphaClipThreshold;
                        };

                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                        {
                            SurfaceDescription surface = (SurfaceDescription)0;
                            float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                            float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                            Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                            float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                            float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                            float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                            float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                            float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                            float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                            Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                            UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                            float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                            float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                            Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                            float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                            float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                            float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                            float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                            float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                            Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                            float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                            Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                            float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                            float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                            float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                            float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                            float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                            Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                            float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                            Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                            float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                            Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                            float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                            Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                            surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                            surface.AlphaClipThreshold = 0.5;
                            return surface;
                        }

                        // --------------------------------------------------
                        // Build Graph Inputs
                        #ifdef HAVE_VFX_MODIFICATION
                        #define VFX_SRP_ATTRIBUTES Attributes
                        #define VFX_SRP_VARYINGS Varyings
                        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                        #endif
                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                        {
                            VertexDescriptionInputs output;
                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                            output.ObjectSpaceNormal = input.normalOS;
                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                            output.ObjectSpacePosition = input.positionOS;

                            return output;
                        }
                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                        {
                            SurfaceDescriptionInputs output;
                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                        #ifdef HAVE_VFX_MODIFICATION
                            // FragInputs from VFX come from two places: Interpolator or CBuffer.
                            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                        #endif







                            output.uv0 = input.texCoord0;
                            output.uv2 = input.texCoord2;
                            output.VertexColor = input.color;
                            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                        #else
                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                        #endif
                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                return output;
                        }

                        // --------------------------------------------------
                        // Main

                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                        // --------------------------------------------------
                        // Visual Effect Vertex Invocations
                        #ifdef HAVE_VFX_MODIFICATION
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                        #endif

                        ENDHLSL
                        }
                        Pass
                        {
                            Name "ScenePickingPass"
                            Tags
                            {
                                "LightMode" = "Picking"
                            }

                            // Render State
                            Cull Back

                            // Debug
                            // <None>

                            // --------------------------------------------------
                            // Pass

                            HLSLPROGRAM

                            // Pragmas
                            #pragma target 4.5
                            #pragma exclude_renderers gles gles3 glcore
                            #pragma vertex vert
                            #pragma fragment frag

                            // DotsInstancingOptions: <None>
                            // HybridV1InjectedBuiltinProperties: <None>

                            // Keywords
                            // PassKeywords: <None>
                            // GraphKeywords: <None>

                            // Defines

                            #define ATTRIBUTES_NEED_NORMAL
                            #define ATTRIBUTES_NEED_TANGENT
                            #define ATTRIBUTES_NEED_TEXCOORD0
                            #define ATTRIBUTES_NEED_TEXCOORD2
                            #define ATTRIBUTES_NEED_COLOR
                            #define VARYINGS_NEED_TEXCOORD0
                            #define VARYINGS_NEED_TEXCOORD2
                            #define VARYINGS_NEED_COLOR
                            #define FEATURES_GRAPH_VERTEX
                            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                            #define SHADERPASS SHADERPASS_DEPTHONLY
                            #define SCENEPICKINGPASS 1
                            #define ALPHA_CLIP_THRESHOLD 1
                            #define _ALPHATEST_ON 1
                            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                            // custom interpolator pre-include
                            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                            // Includes
                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                            // --------------------------------------------------
                            // Structs and Packing

                            // custom interpolators pre packing
                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                            struct Attributes
                            {
                                 float3 positionOS : POSITION;
                                 float3 normalOS : NORMAL;
                                 float4 tangentOS : TANGENT;
                                 float4 uv0 : TEXCOORD0;
                                 float4 uv2 : TEXCOORD2;
                                 float4 color : COLOR;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                 uint instanceID : INSTANCEID_SEMANTIC;
                                #endif
                            };
                            struct Varyings
                            {
                                 float4 positionCS : SV_POSITION;
                                 float4 texCoord0;
                                 float4 texCoord2;
                                 float4 color;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                #endif
                            };
                            struct SurfaceDescriptionInputs
                            {
                                 float4 uv0;
                                 float4 uv2;
                                 float4 VertexColor;
                                 float3 TimeParameters;
                            };
                            struct VertexDescriptionInputs
                            {
                                 float3 ObjectSpaceNormal;
                                 float3 ObjectSpaceTangent;
                                 float3 ObjectSpacePosition;
                            };
                            struct PackedVaryings
                            {
                                 float4 positionCS : SV_POSITION;
                                 float4 interp0 : INTERP0;
                                 float4 interp1 : INTERP1;
                                 float4 interp2 : INTERP2;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                #endif
                            };

                            PackedVaryings PackVaryings(Varyings input)
                            {
                                PackedVaryings output;
                                ZERO_INITIALIZE(PackedVaryings, output);
                                output.positionCS = input.positionCS;
                                output.interp0.xyzw = input.texCoord0;
                                output.interp1.xyzw = input.texCoord2;
                                output.interp2.xyzw = input.color;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                output.instanceID = input.instanceID;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                output.cullFace = input.cullFace;
                                #endif
                                return output;
                            }

                            Varyings UnpackVaryings(PackedVaryings input)
                            {
                                Varyings output;
                                output.positionCS = input.positionCS;
                                output.texCoord0 = input.interp0.xyzw;
                                output.texCoord2 = input.interp1.xyzw;
                                output.color = input.interp2.xyzw;
                                #if UNITY_ANY_INSTANCING_ENABLED
                                output.instanceID = input.instanceID;
                                #endif
                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                #endif
                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                #endif
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                output.cullFace = input.cullFace;
                                #endif
                                return output;
                            }


                            // --------------------------------------------------
                            // Graph

                            // Graph Properties
                            CBUFFER_START(UnityPerMaterial)
                            float2 _Speed;
                            float4 _MainTex_TexelSize;
                            float4 _Color;
                            CBUFFER_END

                                // Object and Global properties
                                SAMPLER(SamplerState_Linear_Repeat);
                                TEXTURE2D(_MainTex);
                                SAMPLER(sampler_MainTex);

                                // Graph Includes
                                // GraphIncludes: <None>

                                // -- Property used by ScenePickingPass
                                #ifdef SCENEPICKINGPASS
                                float4 _SelectionID;
                                #endif

                                // -- Properties used by SceneSelectionPass
                                #ifdef SCENESELECTIONPASS
                                int _ObjectId;
                                int _PassValue;
                                #endif

                                // Graph Functions

                                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                {
                                    Out = A * B;
                                }

                                void Unity_OneMinus_float(float In, out float Out)
                                {
                                    Out = 1 - In;
                                }

                                void Unity_Multiply_float_float(float A, float B, out float Out)
                                {
                                    Out = A * B;
                                }

                                void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                {
                                    Out = A * B;
                                }

                                void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                {
                                    Out = UV * Tiling + Offset;
                                }


                                inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                                {
                                    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                                    UV = frac(sin(mul(UV, m)));
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
                                            float2 lattice = float2(x,y);
                                            float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                                inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                                {
                                    float angle = dot(uv, float2(12.9898, 78.233));
                                    #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                                    // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                                    angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                                #endif
                                return frac(sin(angle) * 43758.5453);
                            }

                            inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                            {
                                return (1.0 - t) * a + (t * b);
                            }


                            inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                            {
                                float2 i = floor(uv);
                                float2 f = frac(uv);
                                f = f * f * (3.0 - 2.0 * f);

                                uv = abs(frac(uv) - 0.5);
                                float2 c0 = i + float2(0.0, 0.0);
                                float2 c1 = i + float2(1.0, 0.0);
                                float2 c2 = i + float2(0.0, 1.0);
                                float2 c3 = i + float2(1.0, 1.0);
                                float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                                float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                                float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                                float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                                float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                                float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                                float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                                return t;
                            }
                            void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                            {
                                float t = 0.0;

                                float freq = pow(2.0, float(0));
                                float amp = pow(0.5, float(3 - 0));
                                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                freq = pow(2.0, float(1));
                                amp = pow(0.5, float(3 - 1));
                                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                freq = pow(2.0, float(2));
                                amp = pow(0.5, float(3 - 2));
                                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                Out = t;
                            }

                            void Unity_Add_float(float A, float B, out float Out)
                            {
                                Out = A + B;
                            }

                            void Unity_Step_float(float Edge, float In, out float Out)
                            {
                                Out = step(Edge, In);
                            }

                            // Custom interpolators pre vertex
                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                            // Graph Vertex
                            struct VertexDescription
                            {
                                float3 Position;
                                float3 Normal;
                                float3 Tangent;
                            };

                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                            {
                                VertexDescription description = (VertexDescription)0;
                                description.Position = IN.ObjectSpacePosition;
                                description.Normal = IN.ObjectSpaceNormal;
                                description.Tangent = IN.ObjectSpaceTangent;
                                return description;
                            }

                            // Custom interpolators, pre surface
                            #ifdef FEATURES_GRAPH_VERTEX
                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                            {
                            return output;
                            }
                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                            #endif

                            // Graph Pixel
                            struct SurfaceDescription
                            {
                                float Alpha;
                                float AlphaClipThreshold;
                            };

                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                            {
                                SurfaceDescription surface = (SurfaceDescription)0;
                                float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                                float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                                Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                                float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                                float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                                float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                                float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                                float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                                float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                                Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                                UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                                float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                                Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                                float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                                float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                                Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                                float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                                float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                                float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                                Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                                float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                                float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                                float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                                float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                                float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                                Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                                float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                                Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                                float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                                Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                                float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                                Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                                surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                                surface.AlphaClipThreshold = 0.5;
                                return surface;
                            }

                            // --------------------------------------------------
                            // Build Graph Inputs
                            #ifdef HAVE_VFX_MODIFICATION
                            #define VFX_SRP_ATTRIBUTES Attributes
                            #define VFX_SRP_VARYINGS Varyings
                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                            #endif
                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                            {
                                VertexDescriptionInputs output;
                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                output.ObjectSpaceNormal = input.normalOS;
                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                output.ObjectSpacePosition = input.positionOS;

                                return output;
                            }
                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                            {
                                SurfaceDescriptionInputs output;
                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                            #ifdef HAVE_VFX_MODIFICATION
                                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                            #endif







                                output.uv0 = input.texCoord0;
                                output.uv2 = input.texCoord2;
                                output.VertexColor = input.color;
                                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                            #else
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                            #endif
                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                    return output;
                            }

                            // --------------------------------------------------
                            // Main

                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                            // --------------------------------------------------
                            // Visual Effect Vertex Invocations
                            #ifdef HAVE_VFX_MODIFICATION
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                            #endif

                            ENDHLSL
                            }
                            Pass
                            {
                                Name "DepthNormals"
                                Tags
                                {
                                    "LightMode" = "DepthNormalsOnly"
                                }

                                // Render State
                                Cull Back
                                ZTest LEqual
                                ZWrite On

                                // Debug
                                // <None>

                                // --------------------------------------------------
                                // Pass

                                HLSLPROGRAM

                                // Pragmas
                                #pragma target 4.5
                                #pragma exclude_renderers gles gles3 glcore
                                #pragma multi_compile_instancing
                                #pragma multi_compile _ DOTS_INSTANCING_ON
                                #pragma vertex vert
                                #pragma fragment frag

                                // DotsInstancingOptions: <None>
                                // HybridV1InjectedBuiltinProperties: <None>

                                // Keywords
                                // PassKeywords: <None>
                                // GraphKeywords: <None>

                                // Defines

                                #define ATTRIBUTES_NEED_NORMAL
                                #define ATTRIBUTES_NEED_TANGENT
                                #define ATTRIBUTES_NEED_TEXCOORD0
                                #define ATTRIBUTES_NEED_TEXCOORD2
                                #define ATTRIBUTES_NEED_COLOR
                                #define VARYINGS_NEED_NORMAL_WS
                                #define VARYINGS_NEED_TEXCOORD0
                                #define VARYINGS_NEED_TEXCOORD2
                                #define VARYINGS_NEED_COLOR
                                #define FEATURES_GRAPH_VERTEX
                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
                                #define _ALPHATEST_ON 1
                                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                // custom interpolator pre-include
                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                // Includes
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                // --------------------------------------------------
                                // Structs and Packing

                                // custom interpolators pre packing
                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                struct Attributes
                                {
                                     float3 positionOS : POSITION;
                                     float3 normalOS : NORMAL;
                                     float4 tangentOS : TANGENT;
                                     float4 uv0 : TEXCOORD0;
                                     float4 uv2 : TEXCOORD2;
                                     float4 color : COLOR;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : INSTANCEID_SEMANTIC;
                                    #endif
                                };
                                struct Varyings
                                {
                                     float4 positionCS : SV_POSITION;
                                     float3 normalWS;
                                     float4 texCoord0;
                                     float4 texCoord2;
                                     float4 color;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                    #endif
                                };
                                struct SurfaceDescriptionInputs
                                {
                                     float4 uv0;
                                     float4 uv2;
                                     float4 VertexColor;
                                     float3 TimeParameters;
                                };
                                struct VertexDescriptionInputs
                                {
                                     float3 ObjectSpaceNormal;
                                     float3 ObjectSpaceTangent;
                                     float3 ObjectSpacePosition;
                                };
                                struct PackedVaryings
                                {
                                     float4 positionCS : SV_POSITION;
                                     float3 interp0 : INTERP0;
                                     float4 interp1 : INTERP1;
                                     float4 interp2 : INTERP2;
                                     float4 interp3 : INTERP3;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                    #endif
                                };

                                PackedVaryings PackVaryings(Varyings input)
                                {
                                    PackedVaryings output;
                                    ZERO_INITIALIZE(PackedVaryings, output);
                                    output.positionCS = input.positionCS;
                                    output.interp0.xyz = input.normalWS;
                                    output.interp1.xyzw = input.texCoord0;
                                    output.interp2.xyzw = input.texCoord2;
                                    output.interp3.xyzw = input.color;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    output.instanceID = input.instanceID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    output.cullFace = input.cullFace;
                                    #endif
                                    return output;
                                }

                                Varyings UnpackVaryings(PackedVaryings input)
                                {
                                    Varyings output;
                                    output.positionCS = input.positionCS;
                                    output.normalWS = input.interp0.xyz;
                                    output.texCoord0 = input.interp1.xyzw;
                                    output.texCoord2 = input.interp2.xyzw;
                                    output.color = input.interp3.xyzw;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    output.instanceID = input.instanceID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    output.cullFace = input.cullFace;
                                    #endif
                                    return output;
                                }


                                // --------------------------------------------------
                                // Graph

                                // Graph Properties
                                CBUFFER_START(UnityPerMaterial)
                                float2 _Speed;
                                float4 _MainTex_TexelSize;
                                float4 _Color;
                                CBUFFER_END

                                    // Object and Global properties
                                    SAMPLER(SamplerState_Linear_Repeat);
                                    TEXTURE2D(_MainTex);
                                    SAMPLER(sampler_MainTex);

                                    // Graph Includes
                                    // GraphIncludes: <None>

                                    // -- Property used by ScenePickingPass
                                    #ifdef SCENEPICKINGPASS
                                    float4 _SelectionID;
                                    #endif

                                    // -- Properties used by SceneSelectionPass
                                    #ifdef SCENESELECTIONPASS
                                    int _ObjectId;
                                    int _PassValue;
                                    #endif

                                    // Graph Functions

                                    void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                    {
                                        Out = A * B;
                                    }

                                    void Unity_OneMinus_float(float In, out float Out)
                                    {
                                        Out = 1 - In;
                                    }

                                    void Unity_Multiply_float_float(float A, float B, out float Out)
                                    {
                                        Out = A * B;
                                    }

                                    void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                    {
                                        Out = A * B;
                                    }

                                    void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                    {
                                        Out = UV * Tiling + Offset;
                                    }


                                    inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                                    {
                                        float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                                        UV = frac(sin(mul(UV, m)));
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
                                                float2 lattice = float2(x,y);
                                                float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                                    inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                                    {
                                        float angle = dot(uv, float2(12.9898, 78.233));
                                        #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                                        // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                                        angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                                    #endif
                                    return frac(sin(angle) * 43758.5453);
                                }

                                inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                                {
                                    return (1.0 - t) * a + (t * b);
                                }


                                inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                                {
                                    float2 i = floor(uv);
                                    float2 f = frac(uv);
                                    f = f * f * (3.0 - 2.0 * f);

                                    uv = abs(frac(uv) - 0.5);
                                    float2 c0 = i + float2(0.0, 0.0);
                                    float2 c1 = i + float2(1.0, 0.0);
                                    float2 c2 = i + float2(0.0, 1.0);
                                    float2 c3 = i + float2(1.0, 1.0);
                                    float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                                    float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                                    float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                                    float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                                    float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                                    float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                                    float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                                    return t;
                                }
                                void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                                {
                                    float t = 0.0;

                                    float freq = pow(2.0, float(0));
                                    float amp = pow(0.5, float(3 - 0));
                                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                    freq = pow(2.0, float(1));
                                    amp = pow(0.5, float(3 - 1));
                                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                    freq = pow(2.0, float(2));
                                    amp = pow(0.5, float(3 - 2));
                                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                    Out = t;
                                }

                                void Unity_Add_float(float A, float B, out float Out)
                                {
                                    Out = A + B;
                                }

                                void Unity_Step_float(float Edge, float In, out float Out)
                                {
                                    Out = step(Edge, In);
                                }

                                // Custom interpolators pre vertex
                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                // Graph Vertex
                                struct VertexDescription
                                {
                                    float3 Position;
                                    float3 Normal;
                                    float3 Tangent;
                                };

                                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                {
                                    VertexDescription description = (VertexDescription)0;
                                    description.Position = IN.ObjectSpacePosition;
                                    description.Normal = IN.ObjectSpaceNormal;
                                    description.Tangent = IN.ObjectSpaceTangent;
                                    return description;
                                }

                                // Custom interpolators, pre surface
                                #ifdef FEATURES_GRAPH_VERTEX
                                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                {
                                return output;
                                }
                                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                #endif

                                // Graph Pixel
                                struct SurfaceDescription
                                {
                                    float Alpha;
                                    float AlphaClipThreshold;
                                };

                                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                {
                                    SurfaceDescription surface = (SurfaceDescription)0;
                                    float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                                    float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                                    Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                                    float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                                    float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                                    float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                                    float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                                    float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                                    float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                                    Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                                    UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                    float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                                    float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                                    Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                                    float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                                    float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                                    Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                                    float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                                    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                                    float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                                    float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                                    Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                                    float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                    Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                                    float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                    float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                                    float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                                    float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                                    float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                                    Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                                    float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                                    Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                                    float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                                    Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                                    float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                                    Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                                    surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                                    surface.AlphaClipThreshold = 0.5;
                                    return surface;
                                }

                                // --------------------------------------------------
                                // Build Graph Inputs
                                #ifdef HAVE_VFX_MODIFICATION
                                #define VFX_SRP_ATTRIBUTES Attributes
                                #define VFX_SRP_VARYINGS Varyings
                                #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                #endif
                                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                {
                                    VertexDescriptionInputs output;
                                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                    output.ObjectSpaceNormal = input.normalOS;
                                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                                    output.ObjectSpacePosition = input.positionOS;

                                    return output;
                                }
                                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                {
                                    SurfaceDescriptionInputs output;
                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                #ifdef HAVE_VFX_MODIFICATION
                                    // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                #endif







                                    output.uv0 = input.texCoord0;
                                    output.uv2 = input.texCoord2;
                                    output.VertexColor = input.color;
                                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                #else
                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                #endif
                                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                        return output;
                                }

                                // --------------------------------------------------
                                // Main

                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

                                // --------------------------------------------------
                                // Visual Effect Vertex Invocations
                                #ifdef HAVE_VFX_MODIFICATION
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                #endif

                                ENDHLSL
                                }
    }
        SubShader
                                {
                                    Tags
                                    {
                                        "RenderPipeline" = "UniversalPipeline"
                                        "RenderType" = "Opaque"
                                        "UniversalMaterialType" = "Unlit"
                                        "Queue" = "AlphaTest"
                                        "ShaderGraphShader" = "true"
                                        "ShaderGraphTargetId" = "UniversalUnlitSubTarget"
                                    }
                                    Pass
                                    {
                                        Name "Universal Forward"
                                        Tags
                                        {
                                        // LightMode: <None>
                                    }

                                    // Render State
                                    Cull Back
                                    Blend One Zero
                                    ZTest LEqual
                                    ZWrite On

                                    // Debug
                                    // <None>

                                    // --------------------------------------------------
                                    // Pass

                                    HLSLPROGRAM

                                    // Pragmas
                                    #pragma target 2.0
                                    #pragma only_renderers gles gles3 glcore d3d11
                                    #pragma multi_compile_instancing
                                    #pragma multi_compile_fog
                                    #pragma instancing_options renderinglayer
                                    #pragma vertex vert
                                    #pragma fragment frag

                                    // DotsInstancingOptions: <None>
                                    // HybridV1InjectedBuiltinProperties: <None>

                                    // Keywords
                                    #pragma multi_compile _ LIGHTMAP_ON
                                    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                                    #pragma shader_feature _ _SAMPLE_GI
                                    #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                                    #pragma multi_compile_fragment _ DEBUG_DISPLAY
                                    // GraphKeywords: <None>

                                    // Defines

                                    #define ATTRIBUTES_NEED_NORMAL
                                    #define ATTRIBUTES_NEED_TANGENT
                                    #define ATTRIBUTES_NEED_TEXCOORD0
                                    #define ATTRIBUTES_NEED_TEXCOORD2
                                    #define ATTRIBUTES_NEED_COLOR
                                    #define VARYINGS_NEED_POSITION_WS
                                    #define VARYINGS_NEED_NORMAL_WS
                                    #define VARYINGS_NEED_TEXCOORD0
                                    #define VARYINGS_NEED_TEXCOORD2
                                    #define VARYINGS_NEED_COLOR
                                    #define VARYINGS_NEED_VIEWDIRECTION_WS
                                    #define FEATURES_GRAPH_VERTEX
                                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                    #define SHADERPASS SHADERPASS_UNLIT
                                    #define _FOG_FRAGMENT 1
                                    #define _ALPHATEST_ON 1
                                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                    // custom interpolator pre-include
                                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                    // Includes
                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                    // --------------------------------------------------
                                    // Structs and Packing

                                    // custom interpolators pre packing
                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                    struct Attributes
                                    {
                                         float3 positionOS : POSITION;
                                         float3 normalOS : NORMAL;
                                         float4 tangentOS : TANGENT;
                                         float4 uv0 : TEXCOORD0;
                                         float4 uv2 : TEXCOORD2;
                                         float4 color : COLOR;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                         uint instanceID : INSTANCEID_SEMANTIC;
                                        #endif
                                    };
                                    struct Varyings
                                    {
                                         float4 positionCS : SV_POSITION;
                                         float3 positionWS;
                                         float3 normalWS;
                                         float4 texCoord0;
                                         float4 texCoord2;
                                         float4 color;
                                         float3 viewDirectionWS;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                         uint instanceID : CUSTOM_INSTANCE_ID;
                                        #endif
                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                        #endif
                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                        #endif
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                        #endif
                                    };
                                    struct SurfaceDescriptionInputs
                                    {
                                         float4 uv0;
                                         float4 uv2;
                                         float4 VertexColor;
                                         float3 TimeParameters;
                                    };
                                    struct VertexDescriptionInputs
                                    {
                                         float3 ObjectSpaceNormal;
                                         float3 ObjectSpaceTangent;
                                         float3 ObjectSpacePosition;
                                    };
                                    struct PackedVaryings
                                    {
                                         float4 positionCS : SV_POSITION;
                                         float3 interp0 : INTERP0;
                                         float3 interp1 : INTERP1;
                                         float4 interp2 : INTERP2;
                                         float4 interp3 : INTERP3;
                                         float4 interp4 : INTERP4;
                                         float3 interp5 : INTERP5;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                         uint instanceID : CUSTOM_INSTANCE_ID;
                                        #endif
                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                        #endif
                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                        #endif
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                        #endif
                                    };

                                    PackedVaryings PackVaryings(Varyings input)
                                    {
                                        PackedVaryings output;
                                        ZERO_INITIALIZE(PackedVaryings, output);
                                        output.positionCS = input.positionCS;
                                        output.interp0.xyz = input.positionWS;
                                        output.interp1.xyz = input.normalWS;
                                        output.interp2.xyzw = input.texCoord0;
                                        output.interp3.xyzw = input.texCoord2;
                                        output.interp4.xyzw = input.color;
                                        output.interp5.xyz = input.viewDirectionWS;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        output.instanceID = input.instanceID;
                                        #endif
                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                        #endif
                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                        #endif
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        output.cullFace = input.cullFace;
                                        #endif
                                        return output;
                                    }

                                    Varyings UnpackVaryings(PackedVaryings input)
                                    {
                                        Varyings output;
                                        output.positionCS = input.positionCS;
                                        output.positionWS = input.interp0.xyz;
                                        output.normalWS = input.interp1.xyz;
                                        output.texCoord0 = input.interp2.xyzw;
                                        output.texCoord2 = input.interp3.xyzw;
                                        output.color = input.interp4.xyzw;
                                        output.viewDirectionWS = input.interp5.xyz;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        output.instanceID = input.instanceID;
                                        #endif
                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                        #endif
                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                        #endif
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        output.cullFace = input.cullFace;
                                        #endif
                                        return output;
                                    }


                                    // --------------------------------------------------
                                    // Graph

                                    // Graph Properties
                                    CBUFFER_START(UnityPerMaterial)
                                    float2 _Speed;
                                    float4 _MainTex_TexelSize;
                                    float4 _Color;
                                    CBUFFER_END

                                        // Object and Global properties
                                        SAMPLER(SamplerState_Linear_Repeat);
                                        TEXTURE2D(_MainTex);
                                        SAMPLER(sampler_MainTex);

                                        // Graph Includes
                                        // GraphIncludes: <None>

                                        // -- Property used by ScenePickingPass
                                        #ifdef SCENEPICKINGPASS
                                        float4 _SelectionID;
                                        #endif

                                        // -- Properties used by SceneSelectionPass
                                        #ifdef SCENESELECTIONPASS
                                        int _ObjectId;
                                        int _PassValue;
                                        #endif

                                        // Graph Functions

                                        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                        {
                                            Out = A * B;
                                        }

                                        void Unity_OneMinus_float(float In, out float Out)
                                        {
                                            Out = 1 - In;
                                        }

                                        void Unity_Multiply_float_float(float A, float B, out float Out)
                                        {
                                            Out = A * B;
                                        }

                                        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                        {
                                            Out = A * B;
                                        }

                                        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                        {
                                            Out = UV * Tiling + Offset;
                                        }


                                        inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                                        {
                                            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                                            UV = frac(sin(mul(UV, m)));
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
                                                    float2 lattice = float2(x,y);
                                                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                                        inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                                        {
                                            float angle = dot(uv, float2(12.9898, 78.233));
                                            #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                                            // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                                            angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                                        #endif
                                        return frac(sin(angle) * 43758.5453);
                                    }

                                    inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                                    {
                                        return (1.0 - t) * a + (t * b);
                                    }


                                    inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                                    {
                                        float2 i = floor(uv);
                                        float2 f = frac(uv);
                                        f = f * f * (3.0 - 2.0 * f);

                                        uv = abs(frac(uv) - 0.5);
                                        float2 c0 = i + float2(0.0, 0.0);
                                        float2 c1 = i + float2(1.0, 0.0);
                                        float2 c2 = i + float2(0.0, 1.0);
                                        float2 c3 = i + float2(1.0, 1.0);
                                        float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                                        float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                                        float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                                        float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                                        float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                                        float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                                        float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                                        return t;
                                    }
                                    void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                                    {
                                        float t = 0.0;

                                        float freq = pow(2.0, float(0));
                                        float amp = pow(0.5, float(3 - 0));
                                        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                        freq = pow(2.0, float(1));
                                        amp = pow(0.5, float(3 - 1));
                                        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                        freq = pow(2.0, float(2));
                                        amp = pow(0.5, float(3 - 2));
                                        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                        Out = t;
                                    }

                                    void Unity_Add_float(float A, float B, out float Out)
                                    {
                                        Out = A + B;
                                    }

                                    void Unity_Step_float(float Edge, float In, out float Out)
                                    {
                                        Out = step(Edge, In);
                                    }

                                    // Custom interpolators pre vertex
                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                    // Graph Vertex
                                    struct VertexDescription
                                    {
                                        float3 Position;
                                        float3 Normal;
                                        float3 Tangent;
                                    };

                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                    {
                                        VertexDescription description = (VertexDescription)0;
                                        description.Position = IN.ObjectSpacePosition;
                                        description.Normal = IN.ObjectSpaceNormal;
                                        description.Tangent = IN.ObjectSpaceTangent;
                                        return description;
                                    }

                                    // Custom interpolators, pre surface
                                    #ifdef FEATURES_GRAPH_VERTEX
                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                    {
                                    return output;
                                    }
                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                    #endif

                                    // Graph Pixel
                                    struct SurfaceDescription
                                    {
                                        float3 BaseColor;
                                        float Alpha;
                                        float AlphaClipThreshold;
                                    };

                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                    {
                                        SurfaceDescription surface = (SurfaceDescription)0;
                                        float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                                        float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                                        Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                                        float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                                        float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                                        float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                                        float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                                        float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                                        float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                                        Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                                        UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                        float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                                        float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                                        Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                                        float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                                        float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                                        Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                                        float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                                        Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                                        float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                                        float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                                        Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                                        float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                        Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                                        float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                        float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                                        float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                                        float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                                        float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                                        Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                                        float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                                        Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                                        float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                                        Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                                        float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                                        Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                                        surface.BaseColor = (_Multiply_4401e795467248a1947fb3451b629452_Out_2.xyz);
                                        surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                                        surface.AlphaClipThreshold = 0.5;
                                        return surface;
                                    }

                                    // --------------------------------------------------
                                    // Build Graph Inputs
                                    #ifdef HAVE_VFX_MODIFICATION
                                    #define VFX_SRP_ATTRIBUTES Attributes
                                    #define VFX_SRP_VARYINGS Varyings
                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                    #endif
                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                    {
                                        VertexDescriptionInputs output;
                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                        output.ObjectSpaceNormal = input.normalOS;
                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                        output.ObjectSpacePosition = input.positionOS;

                                        return output;
                                    }
                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                    {
                                        SurfaceDescriptionInputs output;
                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                    #ifdef HAVE_VFX_MODIFICATION
                                        // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                    #endif







                                        output.uv0 = input.texCoord0;
                                        output.uv2 = input.texCoord2;
                                        output.VertexColor = input.color;
                                        output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                    #else
                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                    #endif
                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                            return output;
                                    }

                                    // --------------------------------------------------
                                    // Main

                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

                                    // --------------------------------------------------
                                    // Visual Effect Vertex Invocations
                                    #ifdef HAVE_VFX_MODIFICATION
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                    #endif

                                    ENDHLSL
                                    }
                                    Pass
                                    {
                                        Name "DepthOnly"
                                        Tags
                                        {
                                            "LightMode" = "DepthOnly"
                                        }

                                        // Render State
                                        Cull Back
                                        ZTest LEqual
                                        ZWrite On
                                        ColorMask 0

                                        // Debug
                                        // <None>

                                        // --------------------------------------------------
                                        // Pass

                                        HLSLPROGRAM

                                        // Pragmas
                                        #pragma target 2.0
                                        #pragma only_renderers gles gles3 glcore d3d11
                                        #pragma multi_compile_instancing
                                        #pragma vertex vert
                                        #pragma fragment frag

                                        // DotsInstancingOptions: <None>
                                        // HybridV1InjectedBuiltinProperties: <None>

                                        // Keywords
                                        // PassKeywords: <None>
                                        // GraphKeywords: <None>

                                        // Defines

                                        #define ATTRIBUTES_NEED_NORMAL
                                        #define ATTRIBUTES_NEED_TANGENT
                                        #define ATTRIBUTES_NEED_TEXCOORD0
                                        #define ATTRIBUTES_NEED_TEXCOORD2
                                        #define ATTRIBUTES_NEED_COLOR
                                        #define VARYINGS_NEED_TEXCOORD0
                                        #define VARYINGS_NEED_TEXCOORD2
                                        #define VARYINGS_NEED_COLOR
                                        #define FEATURES_GRAPH_VERTEX
                                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                        #define SHADERPASS SHADERPASS_DEPTHONLY
                                        #define _ALPHATEST_ON 1
                                        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                        // custom interpolator pre-include
                                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                        // Includes
                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                        // --------------------------------------------------
                                        // Structs and Packing

                                        // custom interpolators pre packing
                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                        struct Attributes
                                        {
                                             float3 positionOS : POSITION;
                                             float3 normalOS : NORMAL;
                                             float4 tangentOS : TANGENT;
                                             float4 uv0 : TEXCOORD0;
                                             float4 uv2 : TEXCOORD2;
                                             float4 color : COLOR;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                             uint instanceID : INSTANCEID_SEMANTIC;
                                            #endif
                                        };
                                        struct Varyings
                                        {
                                             float4 positionCS : SV_POSITION;
                                             float4 texCoord0;
                                             float4 texCoord2;
                                             float4 color;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                             uint instanceID : CUSTOM_INSTANCE_ID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                            #endif
                                        };
                                        struct SurfaceDescriptionInputs
                                        {
                                             float4 uv0;
                                             float4 uv2;
                                             float4 VertexColor;
                                             float3 TimeParameters;
                                        };
                                        struct VertexDescriptionInputs
                                        {
                                             float3 ObjectSpaceNormal;
                                             float3 ObjectSpaceTangent;
                                             float3 ObjectSpacePosition;
                                        };
                                        struct PackedVaryings
                                        {
                                             float4 positionCS : SV_POSITION;
                                             float4 interp0 : INTERP0;
                                             float4 interp1 : INTERP1;
                                             float4 interp2 : INTERP2;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                             uint instanceID : CUSTOM_INSTANCE_ID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                            #endif
                                        };

                                        PackedVaryings PackVaryings(Varyings input)
                                        {
                                            PackedVaryings output;
                                            ZERO_INITIALIZE(PackedVaryings, output);
                                            output.positionCS = input.positionCS;
                                            output.interp0.xyzw = input.texCoord0;
                                            output.interp1.xyzw = input.texCoord2;
                                            output.interp2.xyzw = input.color;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                            output.instanceID = input.instanceID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                            output.cullFace = input.cullFace;
                                            #endif
                                            return output;
                                        }

                                        Varyings UnpackVaryings(PackedVaryings input)
                                        {
                                            Varyings output;
                                            output.positionCS = input.positionCS;
                                            output.texCoord0 = input.interp0.xyzw;
                                            output.texCoord2 = input.interp1.xyzw;
                                            output.color = input.interp2.xyzw;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                            output.instanceID = input.instanceID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                            output.cullFace = input.cullFace;
                                            #endif
                                            return output;
                                        }


                                        // --------------------------------------------------
                                        // Graph

                                        // Graph Properties
                                        CBUFFER_START(UnityPerMaterial)
                                        float2 _Speed;
                                        float4 _MainTex_TexelSize;
                                        float4 _Color;
                                        CBUFFER_END

                                            // Object and Global properties
                                            SAMPLER(SamplerState_Linear_Repeat);
                                            TEXTURE2D(_MainTex);
                                            SAMPLER(sampler_MainTex);

                                            // Graph Includes
                                            // GraphIncludes: <None>

                                            // -- Property used by ScenePickingPass
                                            #ifdef SCENEPICKINGPASS
                                            float4 _SelectionID;
                                            #endif

                                            // -- Properties used by SceneSelectionPass
                                            #ifdef SCENESELECTIONPASS
                                            int _ObjectId;
                                            int _PassValue;
                                            #endif

                                            // Graph Functions

                                            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                            {
                                                Out = A * B;
                                            }

                                            void Unity_OneMinus_float(float In, out float Out)
                                            {
                                                Out = 1 - In;
                                            }

                                            void Unity_Multiply_float_float(float A, float B, out float Out)
                                            {
                                                Out = A * B;
                                            }

                                            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                            {
                                                Out = A * B;
                                            }

                                            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                            {
                                                Out = UV * Tiling + Offset;
                                            }


                                            inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                                            {
                                                float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                                                UV = frac(sin(mul(UV, m)));
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
                                                        float2 lattice = float2(x,y);
                                                        float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                                            inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                                            {
                                                float angle = dot(uv, float2(12.9898, 78.233));
                                                #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                                                // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                                                angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                                            #endif
                                            return frac(sin(angle) * 43758.5453);
                                        }

                                        inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                                        {
                                            return (1.0 - t) * a + (t * b);
                                        }


                                        inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                                        {
                                            float2 i = floor(uv);
                                            float2 f = frac(uv);
                                            f = f * f * (3.0 - 2.0 * f);

                                            uv = abs(frac(uv) - 0.5);
                                            float2 c0 = i + float2(0.0, 0.0);
                                            float2 c1 = i + float2(1.0, 0.0);
                                            float2 c2 = i + float2(0.0, 1.0);
                                            float2 c3 = i + float2(1.0, 1.0);
                                            float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                                            float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                                            float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                                            float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                                            float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                                            float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                                            float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                                            return t;
                                        }
                                        void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                                        {
                                            float t = 0.0;

                                            float freq = pow(2.0, float(0));
                                            float amp = pow(0.5, float(3 - 0));
                                            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                            freq = pow(2.0, float(1));
                                            amp = pow(0.5, float(3 - 1));
                                            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                            freq = pow(2.0, float(2));
                                            amp = pow(0.5, float(3 - 2));
                                            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                            Out = t;
                                        }

                                        void Unity_Add_float(float A, float B, out float Out)
                                        {
                                            Out = A + B;
                                        }

                                        void Unity_Step_float(float Edge, float In, out float Out)
                                        {
                                            Out = step(Edge, In);
                                        }

                                        // Custom interpolators pre vertex
                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                        // Graph Vertex
                                        struct VertexDescription
                                        {
                                            float3 Position;
                                            float3 Normal;
                                            float3 Tangent;
                                        };

                                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                        {
                                            VertexDescription description = (VertexDescription)0;
                                            description.Position = IN.ObjectSpacePosition;
                                            description.Normal = IN.ObjectSpaceNormal;
                                            description.Tangent = IN.ObjectSpaceTangent;
                                            return description;
                                        }

                                        // Custom interpolators, pre surface
                                        #ifdef FEATURES_GRAPH_VERTEX
                                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                        {
                                        return output;
                                        }
                                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                        #endif

                                        // Graph Pixel
                                        struct SurfaceDescription
                                        {
                                            float Alpha;
                                            float AlphaClipThreshold;
                                        };

                                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                        {
                                            SurfaceDescription surface = (SurfaceDescription)0;
                                            float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                                            float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                                            Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                                            float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                                            float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                                            float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                                            float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                                            float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                                            float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                                            Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                                            UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                            float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                                            float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                                            Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                                            float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                                            float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                                            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                                            float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                                            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                                            float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                                            float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                                            Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                                            float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                            Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                                            float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                            float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                                            float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                                            float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                                            float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                                            Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                                            float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                                            Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                                            float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                                            Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                                            float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                                            Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                                            surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                                            surface.AlphaClipThreshold = 0.5;
                                            return surface;
                                        }

                                        // --------------------------------------------------
                                        // Build Graph Inputs
                                        #ifdef HAVE_VFX_MODIFICATION
                                        #define VFX_SRP_ATTRIBUTES Attributes
                                        #define VFX_SRP_VARYINGS Varyings
                                        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                        #endif
                                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                        {
                                            VertexDescriptionInputs output;
                                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                            output.ObjectSpaceNormal = input.normalOS;
                                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                                            output.ObjectSpacePosition = input.positionOS;

                                            return output;
                                        }
                                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                        {
                                            SurfaceDescriptionInputs output;
                                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                        #ifdef HAVE_VFX_MODIFICATION
                                            // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                        #endif







                                            output.uv0 = input.texCoord0;
                                            output.uv2 = input.texCoord2;
                                            output.VertexColor = input.color;
                                            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                        #else
                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                        #endif
                                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                return output;
                                        }

                                        // --------------------------------------------------
                                        // Main

                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

                                        // --------------------------------------------------
                                        // Visual Effect Vertex Invocations
                                        #ifdef HAVE_VFX_MODIFICATION
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                        #endif

                                        ENDHLSL
                                        }
                                        Pass
                                        {
                                            Name "DepthNormalsOnly"
                                            Tags
                                            {
                                                "LightMode" = "DepthNormalsOnly"
                                            }

                                            // Render State
                                            Cull Back
                                            ZTest LEqual
                                            ZWrite On

                                            // Debug
                                            // <None>

                                            // --------------------------------------------------
                                            // Pass

                                            HLSLPROGRAM

                                            // Pragmas
                                            #pragma target 2.0
                                            #pragma only_renderers gles gles3 glcore d3d11
                                            #pragma multi_compile_instancing
                                            #pragma vertex vert
                                            #pragma fragment frag

                                            // DotsInstancingOptions: <None>
                                            // HybridV1InjectedBuiltinProperties: <None>

                                            // Keywords
                                            // PassKeywords: <None>
                                            // GraphKeywords: <None>

                                            // Defines

                                            #define ATTRIBUTES_NEED_NORMAL
                                            #define ATTRIBUTES_NEED_TANGENT
                                            #define ATTRIBUTES_NEED_TEXCOORD0
                                            #define ATTRIBUTES_NEED_TEXCOORD1
                                            #define ATTRIBUTES_NEED_TEXCOORD2
                                            #define ATTRIBUTES_NEED_COLOR
                                            #define VARYINGS_NEED_NORMAL_WS
                                            #define VARYINGS_NEED_TANGENT_WS
                                            #define VARYINGS_NEED_TEXCOORD0
                                            #define VARYINGS_NEED_TEXCOORD2
                                            #define VARYINGS_NEED_COLOR
                                            #define FEATURES_GRAPH_VERTEX
                                            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
                                            #define _ALPHATEST_ON 1
                                            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                            // custom interpolator pre-include
                                            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                            // Includes
                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                            // --------------------------------------------------
                                            // Structs and Packing

                                            // custom interpolators pre packing
                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                            struct Attributes
                                            {
                                                 float3 positionOS : POSITION;
                                                 float3 normalOS : NORMAL;
                                                 float4 tangentOS : TANGENT;
                                                 float4 uv0 : TEXCOORD0;
                                                 float4 uv1 : TEXCOORD1;
                                                 float4 uv2 : TEXCOORD2;
                                                 float4 color : COLOR;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                 uint instanceID : INSTANCEID_SEMANTIC;
                                                #endif
                                            };
                                            struct Varyings
                                            {
                                                 float4 positionCS : SV_POSITION;
                                                 float3 normalWS;
                                                 float4 tangentWS;
                                                 float4 texCoord0;
                                                 float4 texCoord2;
                                                 float4 color;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                                #endif
                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                #endif
                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                #endif
                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                #endif
                                            };
                                            struct SurfaceDescriptionInputs
                                            {
                                                 float4 uv0;
                                                 float4 uv2;
                                                 float4 VertexColor;
                                                 float3 TimeParameters;
                                            };
                                            struct VertexDescriptionInputs
                                            {
                                                 float3 ObjectSpaceNormal;
                                                 float3 ObjectSpaceTangent;
                                                 float3 ObjectSpacePosition;
                                            };
                                            struct PackedVaryings
                                            {
                                                 float4 positionCS : SV_POSITION;
                                                 float3 interp0 : INTERP0;
                                                 float4 interp1 : INTERP1;
                                                 float4 interp2 : INTERP2;
                                                 float4 interp3 : INTERP3;
                                                 float4 interp4 : INTERP4;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                                #endif
                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                #endif
                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                #endif
                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                #endif
                                            };

                                            PackedVaryings PackVaryings(Varyings input)
                                            {
                                                PackedVaryings output;
                                                ZERO_INITIALIZE(PackedVaryings, output);
                                                output.positionCS = input.positionCS;
                                                output.interp0.xyz = input.normalWS;
                                                output.interp1.xyzw = input.tangentWS;
                                                output.interp2.xyzw = input.texCoord0;
                                                output.interp3.xyzw = input.texCoord2;
                                                output.interp4.xyzw = input.color;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                output.instanceID = input.instanceID;
                                                #endif
                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                #endif
                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                #endif
                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                output.cullFace = input.cullFace;
                                                #endif
                                                return output;
                                            }

                                            Varyings UnpackVaryings(PackedVaryings input)
                                            {
                                                Varyings output;
                                                output.positionCS = input.positionCS;
                                                output.normalWS = input.interp0.xyz;
                                                output.tangentWS = input.interp1.xyzw;
                                                output.texCoord0 = input.interp2.xyzw;
                                                output.texCoord2 = input.interp3.xyzw;
                                                output.color = input.interp4.xyzw;
                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                output.instanceID = input.instanceID;
                                                #endif
                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                #endif
                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                #endif
                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                output.cullFace = input.cullFace;
                                                #endif
                                                return output;
                                            }


                                            // --------------------------------------------------
                                            // Graph

                                            // Graph Properties
                                            CBUFFER_START(UnityPerMaterial)
                                            float2 _Speed;
                                            float4 _MainTex_TexelSize;
                                            float4 _Color;
                                            CBUFFER_END

                                                // Object and Global properties
                                                SAMPLER(SamplerState_Linear_Repeat);
                                                TEXTURE2D(_MainTex);
                                                SAMPLER(sampler_MainTex);

                                                // Graph Includes
                                                // GraphIncludes: <None>

                                                // -- Property used by ScenePickingPass
                                                #ifdef SCENEPICKINGPASS
                                                float4 _SelectionID;
                                                #endif

                                                // -- Properties used by SceneSelectionPass
                                                #ifdef SCENESELECTIONPASS
                                                int _ObjectId;
                                                int _PassValue;
                                                #endif

                                                // Graph Functions

                                                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                                {
                                                    Out = A * B;
                                                }

                                                void Unity_OneMinus_float(float In, out float Out)
                                                {
                                                    Out = 1 - In;
                                                }

                                                void Unity_Multiply_float_float(float A, float B, out float Out)
                                                {
                                                    Out = A * B;
                                                }

                                                void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                                {
                                                    Out = A * B;
                                                }

                                                void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                                {
                                                    Out = UV * Tiling + Offset;
                                                }


                                                inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                                                {
                                                    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                                                    UV = frac(sin(mul(UV, m)));
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
                                                            float2 lattice = float2(x,y);
                                                            float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                                                inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                                                {
                                                    float angle = dot(uv, float2(12.9898, 78.233));
                                                    #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                                                    // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                                                    angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                                                #endif
                                                return frac(sin(angle) * 43758.5453);
                                            }

                                            inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                                            {
                                                return (1.0 - t) * a + (t * b);
                                            }


                                            inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                                            {
                                                float2 i = floor(uv);
                                                float2 f = frac(uv);
                                                f = f * f * (3.0 - 2.0 * f);

                                                uv = abs(frac(uv) - 0.5);
                                                float2 c0 = i + float2(0.0, 0.0);
                                                float2 c1 = i + float2(1.0, 0.0);
                                                float2 c2 = i + float2(0.0, 1.0);
                                                float2 c3 = i + float2(1.0, 1.0);
                                                float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                                                float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                                                float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                                                float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                                                float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                                                float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                                                float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                                                return t;
                                            }
                                            void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                                            {
                                                float t = 0.0;

                                                float freq = pow(2.0, float(0));
                                                float amp = pow(0.5, float(3 - 0));
                                                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                freq = pow(2.0, float(1));
                                                amp = pow(0.5, float(3 - 1));
                                                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                freq = pow(2.0, float(2));
                                                amp = pow(0.5, float(3 - 2));
                                                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                Out = t;
                                            }

                                            void Unity_Add_float(float A, float B, out float Out)
                                            {
                                                Out = A + B;
                                            }

                                            void Unity_Step_float(float Edge, float In, out float Out)
                                            {
                                                Out = step(Edge, In);
                                            }

                                            // Custom interpolators pre vertex
                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                            // Graph Vertex
                                            struct VertexDescription
                                            {
                                                float3 Position;
                                                float3 Normal;
                                                float3 Tangent;
                                            };

                                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                            {
                                                VertexDescription description = (VertexDescription)0;
                                                description.Position = IN.ObjectSpacePosition;
                                                description.Normal = IN.ObjectSpaceNormal;
                                                description.Tangent = IN.ObjectSpaceTangent;
                                                return description;
                                            }

                                            // Custom interpolators, pre surface
                                            #ifdef FEATURES_GRAPH_VERTEX
                                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                            {
                                            return output;
                                            }
                                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                            #endif

                                            // Graph Pixel
                                            struct SurfaceDescription
                                            {
                                                float Alpha;
                                                float AlphaClipThreshold;
                                            };

                                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                            {
                                                SurfaceDescription surface = (SurfaceDescription)0;
                                                float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                                                float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                                                Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                                                float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                                                float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                                                float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                                                float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                                                float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                                                float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                                                Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                                                UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                                float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                                                float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                                                Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                                                float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                                                float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                                                Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                                                float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                                                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                                                float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                                                float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                                                Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                                                float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                                Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                                                float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                                float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                                                float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                                                float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                                                float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                                                Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                                                float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                                                Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                                                float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                                                Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                                                float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                                                Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                                                surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                                                surface.AlphaClipThreshold = 0.5;
                                                return surface;
                                            }

                                            // --------------------------------------------------
                                            // Build Graph Inputs
                                            #ifdef HAVE_VFX_MODIFICATION
                                            #define VFX_SRP_ATTRIBUTES Attributes
                                            #define VFX_SRP_VARYINGS Varyings
                                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                            #endif
                                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                            {
                                                VertexDescriptionInputs output;
                                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                output.ObjectSpaceNormal = input.normalOS;
                                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                output.ObjectSpacePosition = input.positionOS;

                                                return output;
                                            }
                                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                            {
                                                SurfaceDescriptionInputs output;
                                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                            #ifdef HAVE_VFX_MODIFICATION
                                                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                            #endif







                                                output.uv0 = input.texCoord0;
                                                output.uv2 = input.texCoord2;
                                                output.VertexColor = input.color;
                                                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                            #else
                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                            #endif
                                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                    return output;
                                            }

                                            // --------------------------------------------------
                                            // Main

                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

                                            // --------------------------------------------------
                                            // Visual Effect Vertex Invocations
                                            #ifdef HAVE_VFX_MODIFICATION
                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                            #endif

                                            ENDHLSL
                                            }
                                            Pass
                                            {
                                                Name "ShadowCaster"
                                                Tags
                                                {
                                                    "LightMode" = "ShadowCaster"
                                                }

                                                // Render State
                                                Cull Back
                                                ZTest LEqual
                                                ZWrite On
                                                ColorMask 0

                                                // Debug
                                                // <None>

                                                // --------------------------------------------------
                                                // Pass

                                                HLSLPROGRAM

                                                // Pragmas
                                                #pragma target 2.0
                                                #pragma only_renderers gles gles3 glcore d3d11
                                                #pragma multi_compile_instancing
                                                #pragma vertex vert
                                                #pragma fragment frag

                                                // DotsInstancingOptions: <None>
                                                // HybridV1InjectedBuiltinProperties: <None>

                                                // Keywords
                                                #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
                                                // GraphKeywords: <None>

                                                // Defines

                                                #define ATTRIBUTES_NEED_NORMAL
                                                #define ATTRIBUTES_NEED_TANGENT
                                                #define ATTRIBUTES_NEED_TEXCOORD0
                                                #define ATTRIBUTES_NEED_TEXCOORD2
                                                #define ATTRIBUTES_NEED_COLOR
                                                #define VARYINGS_NEED_NORMAL_WS
                                                #define VARYINGS_NEED_TEXCOORD0
                                                #define VARYINGS_NEED_TEXCOORD2
                                                #define VARYINGS_NEED_COLOR
                                                #define FEATURES_GRAPH_VERTEX
                                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                #define SHADERPASS SHADERPASS_SHADOWCASTER
                                                #define _ALPHATEST_ON 1
                                                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                // custom interpolator pre-include
                                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                // Includes
                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                // --------------------------------------------------
                                                // Structs and Packing

                                                // custom interpolators pre packing
                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                struct Attributes
                                                {
                                                     float3 positionOS : POSITION;
                                                     float3 normalOS : NORMAL;
                                                     float4 tangentOS : TANGENT;
                                                     float4 uv0 : TEXCOORD0;
                                                     float4 uv2 : TEXCOORD2;
                                                     float4 color : COLOR;
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                     uint instanceID : INSTANCEID_SEMANTIC;
                                                    #endif
                                                };
                                                struct Varyings
                                                {
                                                     float4 positionCS : SV_POSITION;
                                                     float3 normalWS;
                                                     float4 texCoord0;
                                                     float4 texCoord2;
                                                     float4 color;
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                    #endif
                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                    #endif
                                                };
                                                struct SurfaceDescriptionInputs
                                                {
                                                     float4 uv0;
                                                     float4 uv2;
                                                     float4 VertexColor;
                                                     float3 TimeParameters;
                                                };
                                                struct VertexDescriptionInputs
                                                {
                                                     float3 ObjectSpaceNormal;
                                                     float3 ObjectSpaceTangent;
                                                     float3 ObjectSpacePosition;
                                                };
                                                struct PackedVaryings
                                                {
                                                     float4 positionCS : SV_POSITION;
                                                     float3 interp0 : INTERP0;
                                                     float4 interp1 : INTERP1;
                                                     float4 interp2 : INTERP2;
                                                     float4 interp3 : INTERP3;
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                    #endif
                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                    #endif
                                                };

                                                PackedVaryings PackVaryings(Varyings input)
                                                {
                                                    PackedVaryings output;
                                                    ZERO_INITIALIZE(PackedVaryings, output);
                                                    output.positionCS = input.positionCS;
                                                    output.interp0.xyz = input.normalWS;
                                                    output.interp1.xyzw = input.texCoord0;
                                                    output.interp2.xyzw = input.texCoord2;
                                                    output.interp3.xyzw = input.color;
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                    output.instanceID = input.instanceID;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                    #endif
                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                    output.cullFace = input.cullFace;
                                                    #endif
                                                    return output;
                                                }

                                                Varyings UnpackVaryings(PackedVaryings input)
                                                {
                                                    Varyings output;
                                                    output.positionCS = input.positionCS;
                                                    output.normalWS = input.interp0.xyz;
                                                    output.texCoord0 = input.interp1.xyzw;
                                                    output.texCoord2 = input.interp2.xyzw;
                                                    output.color = input.interp3.xyzw;
                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                    output.instanceID = input.instanceID;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                    #endif
                                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                    #endif
                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                    output.cullFace = input.cullFace;
                                                    #endif
                                                    return output;
                                                }


                                                // --------------------------------------------------
                                                // Graph

                                                // Graph Properties
                                                CBUFFER_START(UnityPerMaterial)
                                                float2 _Speed;
                                                float4 _MainTex_TexelSize;
                                                float4 _Color;
                                                CBUFFER_END

                                                    // Object and Global properties
                                                    SAMPLER(SamplerState_Linear_Repeat);
                                                    TEXTURE2D(_MainTex);
                                                    SAMPLER(sampler_MainTex);

                                                    // Graph Includes
                                                    // GraphIncludes: <None>

                                                    // -- Property used by ScenePickingPass
                                                    #ifdef SCENEPICKINGPASS
                                                    float4 _SelectionID;
                                                    #endif

                                                    // -- Properties used by SceneSelectionPass
                                                    #ifdef SCENESELECTIONPASS
                                                    int _ObjectId;
                                                    int _PassValue;
                                                    #endif

                                                    // Graph Functions

                                                    void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                                    {
                                                        Out = A * B;
                                                    }

                                                    void Unity_OneMinus_float(float In, out float Out)
                                                    {
                                                        Out = 1 - In;
                                                    }

                                                    void Unity_Multiply_float_float(float A, float B, out float Out)
                                                    {
                                                        Out = A * B;
                                                    }

                                                    void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                                    {
                                                        Out = A * B;
                                                    }

                                                    void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                                    {
                                                        Out = UV * Tiling + Offset;
                                                    }


                                                    inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                                                    {
                                                        float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                                                        UV = frac(sin(mul(UV, m)));
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
                                                                float2 lattice = float2(x,y);
                                                                float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                                                    inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                                                    {
                                                        float angle = dot(uv, float2(12.9898, 78.233));
                                                        #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                                                        // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                                                        angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                                                    #endif
                                                    return frac(sin(angle) * 43758.5453);
                                                }

                                                inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                                                {
                                                    return (1.0 - t) * a + (t * b);
                                                }


                                                inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                                                {
                                                    float2 i = floor(uv);
                                                    float2 f = frac(uv);
                                                    f = f * f * (3.0 - 2.0 * f);

                                                    uv = abs(frac(uv) - 0.5);
                                                    float2 c0 = i + float2(0.0, 0.0);
                                                    float2 c1 = i + float2(1.0, 0.0);
                                                    float2 c2 = i + float2(0.0, 1.0);
                                                    float2 c3 = i + float2(1.0, 1.0);
                                                    float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                                                    float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                                                    float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                                                    float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                                                    float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                                                    float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                                                    float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                                                    return t;
                                                }
                                                void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                                                {
                                                    float t = 0.0;

                                                    float freq = pow(2.0, float(0));
                                                    float amp = pow(0.5, float(3 - 0));
                                                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                    freq = pow(2.0, float(1));
                                                    amp = pow(0.5, float(3 - 1));
                                                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                    freq = pow(2.0, float(2));
                                                    amp = pow(0.5, float(3 - 2));
                                                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                    Out = t;
                                                }

                                                void Unity_Add_float(float A, float B, out float Out)
                                                {
                                                    Out = A + B;
                                                }

                                                void Unity_Step_float(float Edge, float In, out float Out)
                                                {
                                                    Out = step(Edge, In);
                                                }

                                                // Custom interpolators pre vertex
                                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                // Graph Vertex
                                                struct VertexDescription
                                                {
                                                    float3 Position;
                                                    float3 Normal;
                                                    float3 Tangent;
                                                };

                                                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                {
                                                    VertexDescription description = (VertexDescription)0;
                                                    description.Position = IN.ObjectSpacePosition;
                                                    description.Normal = IN.ObjectSpaceNormal;
                                                    description.Tangent = IN.ObjectSpaceTangent;
                                                    return description;
                                                }

                                                // Custom interpolators, pre surface
                                                #ifdef FEATURES_GRAPH_VERTEX
                                                Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                {
                                                return output;
                                                }
                                                #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                #endif

                                                // Graph Pixel
                                                struct SurfaceDescription
                                                {
                                                    float Alpha;
                                                    float AlphaClipThreshold;
                                                };

                                                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                {
                                                    SurfaceDescription surface = (SurfaceDescription)0;
                                                    float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                                                    float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                                                    Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                                                    float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                                                    float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                                                    float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                                                    float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                                                    float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                                                    float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                                                    Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                                                    UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                                    float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                                                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                                                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                                                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                                                    float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                                                    float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                                                    Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                                                    float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                                                    float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                                                    Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                                                    float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                                                    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                                                    float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                                                    float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                                                    Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                                                    float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                                    Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                                                    float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                                    float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                                                    float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                                                    float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                                                    float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                                                    Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                                                    float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                                                    Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                                                    float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                                                    Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                                                    float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                                                    Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                                                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                                                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                                                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                                                    float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                                                    surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                                                    surface.AlphaClipThreshold = 0.5;
                                                    return surface;
                                                }

                                                // --------------------------------------------------
                                                // Build Graph Inputs
                                                #ifdef HAVE_VFX_MODIFICATION
                                                #define VFX_SRP_ATTRIBUTES Attributes
                                                #define VFX_SRP_VARYINGS Varyings
                                                #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                #endif
                                                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                {
                                                    VertexDescriptionInputs output;
                                                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                    output.ObjectSpaceNormal = input.normalOS;
                                                    output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                    output.ObjectSpacePosition = input.positionOS;

                                                    return output;
                                                }
                                                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                {
                                                    SurfaceDescriptionInputs output;
                                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                #ifdef HAVE_VFX_MODIFICATION
                                                    // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                #endif







                                                    output.uv0 = input.texCoord0;
                                                    output.uv2 = input.texCoord2;
                                                    output.VertexColor = input.color;
                                                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                #else
                                                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                #endif
                                                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                        return output;
                                                }

                                                // --------------------------------------------------
                                                // Main

                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

                                                // --------------------------------------------------
                                                // Visual Effect Vertex Invocations
                                                #ifdef HAVE_VFX_MODIFICATION
                                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                #endif

                                                ENDHLSL
                                                }
                                                Pass
                                                {
                                                    Name "SceneSelectionPass"
                                                    Tags
                                                    {
                                                        "LightMode" = "SceneSelectionPass"
                                                    }

                                                    // Render State
                                                    Cull Off

                                                    // Debug
                                                    // <None>

                                                    // --------------------------------------------------
                                                    // Pass

                                                    HLSLPROGRAM

                                                    // Pragmas
                                                    #pragma target 2.0
                                                    #pragma only_renderers gles gles3 glcore d3d11
                                                    #pragma multi_compile_instancing
                                                    #pragma vertex vert
                                                    #pragma fragment frag

                                                    // DotsInstancingOptions: <None>
                                                    // HybridV1InjectedBuiltinProperties: <None>

                                                    // Keywords
                                                    // PassKeywords: <None>
                                                    // GraphKeywords: <None>

                                                    // Defines

                                                    #define ATTRIBUTES_NEED_NORMAL
                                                    #define ATTRIBUTES_NEED_TANGENT
                                                    #define ATTRIBUTES_NEED_TEXCOORD0
                                                    #define ATTRIBUTES_NEED_TEXCOORD2
                                                    #define ATTRIBUTES_NEED_COLOR
                                                    #define VARYINGS_NEED_TEXCOORD0
                                                    #define VARYINGS_NEED_TEXCOORD2
                                                    #define VARYINGS_NEED_COLOR
                                                    #define FEATURES_GRAPH_VERTEX
                                                    /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                    #define SHADERPASS SHADERPASS_DEPTHONLY
                                                    #define SCENESELECTIONPASS 1
                                                    #define ALPHA_CLIP_THRESHOLD 1
                                                    #define _ALPHATEST_ON 1
                                                    /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                    // custom interpolator pre-include
                                                    /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                    // Includes
                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                    // --------------------------------------------------
                                                    // Structs and Packing

                                                    // custom interpolators pre packing
                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                    struct Attributes
                                                    {
                                                         float3 positionOS : POSITION;
                                                         float3 normalOS : NORMAL;
                                                         float4 tangentOS : TANGENT;
                                                         float4 uv0 : TEXCOORD0;
                                                         float4 uv2 : TEXCOORD2;
                                                         float4 color : COLOR;
                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                         uint instanceID : INSTANCEID_SEMANTIC;
                                                        #endif
                                                    };
                                                    struct Varyings
                                                    {
                                                         float4 positionCS : SV_POSITION;
                                                         float4 texCoord0;
                                                         float4 texCoord2;
                                                         float4 color;
                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                         uint instanceID : CUSTOM_INSTANCE_ID;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                        #endif
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                        #endif
                                                    };
                                                    struct SurfaceDescriptionInputs
                                                    {
                                                         float4 uv0;
                                                         float4 uv2;
                                                         float4 VertexColor;
                                                         float3 TimeParameters;
                                                    };
                                                    struct VertexDescriptionInputs
                                                    {
                                                         float3 ObjectSpaceNormal;
                                                         float3 ObjectSpaceTangent;
                                                         float3 ObjectSpacePosition;
                                                    };
                                                    struct PackedVaryings
                                                    {
                                                         float4 positionCS : SV_POSITION;
                                                         float4 interp0 : INTERP0;
                                                         float4 interp1 : INTERP1;
                                                         float4 interp2 : INTERP2;
                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                         uint instanceID : CUSTOM_INSTANCE_ID;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                        #endif
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                        #endif
                                                    };

                                                    PackedVaryings PackVaryings(Varyings input)
                                                    {
                                                        PackedVaryings output;
                                                        ZERO_INITIALIZE(PackedVaryings, output);
                                                        output.positionCS = input.positionCS;
                                                        output.interp0.xyzw = input.texCoord0;
                                                        output.interp1.xyzw = input.texCoord2;
                                                        output.interp2.xyzw = input.color;
                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                        output.instanceID = input.instanceID;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                        #endif
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                        output.cullFace = input.cullFace;
                                                        #endif
                                                        return output;
                                                    }

                                                    Varyings UnpackVaryings(PackedVaryings input)
                                                    {
                                                        Varyings output;
                                                        output.positionCS = input.positionCS;
                                                        output.texCoord0 = input.interp0.xyzw;
                                                        output.texCoord2 = input.interp1.xyzw;
                                                        output.color = input.interp2.xyzw;
                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                        output.instanceID = input.instanceID;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                        #endif
                                                        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                        #endif
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                        output.cullFace = input.cullFace;
                                                        #endif
                                                        return output;
                                                    }


                                                    // --------------------------------------------------
                                                    // Graph

                                                    // Graph Properties
                                                    CBUFFER_START(UnityPerMaterial)
                                                    float2 _Speed;
                                                    float4 _MainTex_TexelSize;
                                                    float4 _Color;
                                                    CBUFFER_END

                                                        // Object and Global properties
                                                        SAMPLER(SamplerState_Linear_Repeat);
                                                        TEXTURE2D(_MainTex);
                                                        SAMPLER(sampler_MainTex);

                                                        // Graph Includes
                                                        // GraphIncludes: <None>

                                                        // -- Property used by ScenePickingPass
                                                        #ifdef SCENEPICKINGPASS
                                                        float4 _SelectionID;
                                                        #endif

                                                        // -- Properties used by SceneSelectionPass
                                                        #ifdef SCENESELECTIONPASS
                                                        int _ObjectId;
                                                        int _PassValue;
                                                        #endif

                                                        // Graph Functions

                                                        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                                        {
                                                            Out = A * B;
                                                        }

                                                        void Unity_OneMinus_float(float In, out float Out)
                                                        {
                                                            Out = 1 - In;
                                                        }

                                                        void Unity_Multiply_float_float(float A, float B, out float Out)
                                                        {
                                                            Out = A * B;
                                                        }

                                                        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                                        {
                                                            Out = A * B;
                                                        }

                                                        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                                        {
                                                            Out = UV * Tiling + Offset;
                                                        }


                                                        inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                                                        {
                                                            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                                                            UV = frac(sin(mul(UV, m)));
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
                                                                    float2 lattice = float2(x,y);
                                                                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                                                        inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                                                        {
                                                            float angle = dot(uv, float2(12.9898, 78.233));
                                                            #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                                                            // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                                                            angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                                                        #endif
                                                        return frac(sin(angle) * 43758.5453);
                                                    }

                                                    inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                                                    {
                                                        return (1.0 - t) * a + (t * b);
                                                    }


                                                    inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                                                    {
                                                        float2 i = floor(uv);
                                                        float2 f = frac(uv);
                                                        f = f * f * (3.0 - 2.0 * f);

                                                        uv = abs(frac(uv) - 0.5);
                                                        float2 c0 = i + float2(0.0, 0.0);
                                                        float2 c1 = i + float2(1.0, 0.0);
                                                        float2 c2 = i + float2(0.0, 1.0);
                                                        float2 c3 = i + float2(1.0, 1.0);
                                                        float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                                                        float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                                                        float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                                                        float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                                                        float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                                                        float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                                                        float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                                                        return t;
                                                    }
                                                    void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                                                    {
                                                        float t = 0.0;

                                                        float freq = pow(2.0, float(0));
                                                        float amp = pow(0.5, float(3 - 0));
                                                        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                        freq = pow(2.0, float(1));
                                                        amp = pow(0.5, float(3 - 1));
                                                        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                        freq = pow(2.0, float(2));
                                                        amp = pow(0.5, float(3 - 2));
                                                        t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                        Out = t;
                                                    }

                                                    void Unity_Add_float(float A, float B, out float Out)
                                                    {
                                                        Out = A + B;
                                                    }

                                                    void Unity_Step_float(float Edge, float In, out float Out)
                                                    {
                                                        Out = step(Edge, In);
                                                    }

                                                    // Custom interpolators pre vertex
                                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                    // Graph Vertex
                                                    struct VertexDescription
                                                    {
                                                        float3 Position;
                                                        float3 Normal;
                                                        float3 Tangent;
                                                    };

                                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                    {
                                                        VertexDescription description = (VertexDescription)0;
                                                        description.Position = IN.ObjectSpacePosition;
                                                        description.Normal = IN.ObjectSpaceNormal;
                                                        description.Tangent = IN.ObjectSpaceTangent;
                                                        return description;
                                                    }

                                                    // Custom interpolators, pre surface
                                                    #ifdef FEATURES_GRAPH_VERTEX
                                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                    {
                                                    return output;
                                                    }
                                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                    #endif

                                                    // Graph Pixel
                                                    struct SurfaceDescription
                                                    {
                                                        float Alpha;
                                                        float AlphaClipThreshold;
                                                    };

                                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                    {
                                                        SurfaceDescription surface = (SurfaceDescription)0;
                                                        float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                                                        float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                                                        Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                                                        float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                                                        float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                                                        float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                                                        float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                                                        float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                                                        float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                                                        Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                                                        UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                                        float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                                                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                                                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                                                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                                                        float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                                                        float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                                                        Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                                                        float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                                                        float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                                                        Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                                                        float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                                                        Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                                                        float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                                                        float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                                                        Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                                                        float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                                        Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                                                        float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                                        float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                                                        float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                                                        float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                                                        float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                                                        Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                                                        float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                                                        Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                                                        float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                                                        Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                                                        float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                                                        Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                                                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                                                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                                                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                                                        float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                                                        surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                                                        surface.AlphaClipThreshold = 0.5;
                                                        return surface;
                                                    }

                                                    // --------------------------------------------------
                                                    // Build Graph Inputs
                                                    #ifdef HAVE_VFX_MODIFICATION
                                                    #define VFX_SRP_ATTRIBUTES Attributes
                                                    #define VFX_SRP_VARYINGS Varyings
                                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                    #endif
                                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                    {
                                                        VertexDescriptionInputs output;
                                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                        output.ObjectSpaceNormal = input.normalOS;
                                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                        output.ObjectSpacePosition = input.positionOS;

                                                        return output;
                                                    }
                                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                    {
                                                        SurfaceDescriptionInputs output;
                                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                    #ifdef HAVE_VFX_MODIFICATION
                                                        // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                    #endif







                                                        output.uv0 = input.texCoord0;
                                                        output.uv2 = input.texCoord2;
                                                        output.VertexColor = input.color;
                                                        output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                    #else
                                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                    #endif
                                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                            return output;
                                                    }

                                                    // --------------------------------------------------
                                                    // Main

                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                                                    // --------------------------------------------------
                                                    // Visual Effect Vertex Invocations
                                                    #ifdef HAVE_VFX_MODIFICATION
                                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                    #endif

                                                    ENDHLSL
                                                    }
                                                    Pass
                                                    {
                                                        Name "ScenePickingPass"
                                                        Tags
                                                        {
                                                            "LightMode" = "Picking"
                                                        }

                                                        // Render State
                                                        Cull Back

                                                        // Debug
                                                        // <None>

                                                        // --------------------------------------------------
                                                        // Pass

                                                        HLSLPROGRAM

                                                        // Pragmas
                                                        #pragma target 2.0
                                                        #pragma only_renderers gles gles3 glcore d3d11
                                                        #pragma multi_compile_instancing
                                                        #pragma vertex vert
                                                        #pragma fragment frag

                                                        // DotsInstancingOptions: <None>
                                                        // HybridV1InjectedBuiltinProperties: <None>

                                                        // Keywords
                                                        // PassKeywords: <None>
                                                        // GraphKeywords: <None>

                                                        // Defines

                                                        #define ATTRIBUTES_NEED_NORMAL
                                                        #define ATTRIBUTES_NEED_TANGENT
                                                        #define ATTRIBUTES_NEED_TEXCOORD0
                                                        #define ATTRIBUTES_NEED_TEXCOORD2
                                                        #define ATTRIBUTES_NEED_COLOR
                                                        #define VARYINGS_NEED_TEXCOORD0
                                                        #define VARYINGS_NEED_TEXCOORD2
                                                        #define VARYINGS_NEED_COLOR
                                                        #define FEATURES_GRAPH_VERTEX
                                                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                        #define SHADERPASS SHADERPASS_DEPTHONLY
                                                        #define SCENEPICKINGPASS 1
                                                        #define ALPHA_CLIP_THRESHOLD 1
                                                        #define _ALPHATEST_ON 1
                                                        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                        // custom interpolator pre-include
                                                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                        // Includes
                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                        // --------------------------------------------------
                                                        // Structs and Packing

                                                        // custom interpolators pre packing
                                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                        struct Attributes
                                                        {
                                                             float3 positionOS : POSITION;
                                                             float3 normalOS : NORMAL;
                                                             float4 tangentOS : TANGENT;
                                                             float4 uv0 : TEXCOORD0;
                                                             float4 uv2 : TEXCOORD2;
                                                             float4 color : COLOR;
                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                             uint instanceID : INSTANCEID_SEMANTIC;
                                                            #endif
                                                        };
                                                        struct Varyings
                                                        {
                                                             float4 positionCS : SV_POSITION;
                                                             float4 texCoord0;
                                                             float4 texCoord2;
                                                             float4 color;
                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                             uint instanceID : CUSTOM_INSTANCE_ID;
                                                            #endif
                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                            #endif
                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                            #endif
                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                            #endif
                                                        };
                                                        struct SurfaceDescriptionInputs
                                                        {
                                                             float4 uv0;
                                                             float4 uv2;
                                                             float4 VertexColor;
                                                             float3 TimeParameters;
                                                        };
                                                        struct VertexDescriptionInputs
                                                        {
                                                             float3 ObjectSpaceNormal;
                                                             float3 ObjectSpaceTangent;
                                                             float3 ObjectSpacePosition;
                                                        };
                                                        struct PackedVaryings
                                                        {
                                                             float4 positionCS : SV_POSITION;
                                                             float4 interp0 : INTERP0;
                                                             float4 interp1 : INTERP1;
                                                             float4 interp2 : INTERP2;
                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                             uint instanceID : CUSTOM_INSTANCE_ID;
                                                            #endif
                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                            #endif
                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                            #endif
                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                            #endif
                                                        };

                                                        PackedVaryings PackVaryings(Varyings input)
                                                        {
                                                            PackedVaryings output;
                                                            ZERO_INITIALIZE(PackedVaryings, output);
                                                            output.positionCS = input.positionCS;
                                                            output.interp0.xyzw = input.texCoord0;
                                                            output.interp1.xyzw = input.texCoord2;
                                                            output.interp2.xyzw = input.color;
                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                            output.instanceID = input.instanceID;
                                                            #endif
                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                            #endif
                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                            #endif
                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                            output.cullFace = input.cullFace;
                                                            #endif
                                                            return output;
                                                        }

                                                        Varyings UnpackVaryings(PackedVaryings input)
                                                        {
                                                            Varyings output;
                                                            output.positionCS = input.positionCS;
                                                            output.texCoord0 = input.interp0.xyzw;
                                                            output.texCoord2 = input.interp1.xyzw;
                                                            output.color = input.interp2.xyzw;
                                                            #if UNITY_ANY_INSTANCING_ENABLED
                                                            output.instanceID = input.instanceID;
                                                            #endif
                                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                            #endif
                                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                            #endif
                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                            output.cullFace = input.cullFace;
                                                            #endif
                                                            return output;
                                                        }


                                                        // --------------------------------------------------
                                                        // Graph

                                                        // Graph Properties
                                                        CBUFFER_START(UnityPerMaterial)
                                                        float2 _Speed;
                                                        float4 _MainTex_TexelSize;
                                                        float4 _Color;
                                                        CBUFFER_END

                                                            // Object and Global properties
                                                            SAMPLER(SamplerState_Linear_Repeat);
                                                            TEXTURE2D(_MainTex);
                                                            SAMPLER(sampler_MainTex);

                                                            // Graph Includes
                                                            // GraphIncludes: <None>

                                                            // -- Property used by ScenePickingPass
                                                            #ifdef SCENEPICKINGPASS
                                                            float4 _SelectionID;
                                                            #endif

                                                            // -- Properties used by SceneSelectionPass
                                                            #ifdef SCENESELECTIONPASS
                                                            int _ObjectId;
                                                            int _PassValue;
                                                            #endif

                                                            // Graph Functions

                                                            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                                            {
                                                                Out = A * B;
                                                            }

                                                            void Unity_OneMinus_float(float In, out float Out)
                                                            {
                                                                Out = 1 - In;
                                                            }

                                                            void Unity_Multiply_float_float(float A, float B, out float Out)
                                                            {
                                                                Out = A * B;
                                                            }

                                                            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                                            {
                                                                Out = A * B;
                                                            }

                                                            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                                            {
                                                                Out = UV * Tiling + Offset;
                                                            }


                                                            inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                                                            {
                                                                float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                                                                UV = frac(sin(mul(UV, m)));
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
                                                                        float2 lattice = float2(x,y);
                                                                        float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                                                            inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                                                            {
                                                                float angle = dot(uv, float2(12.9898, 78.233));
                                                                #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                                                                // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                                                                angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                                                            #endif
                                                            return frac(sin(angle) * 43758.5453);
                                                        }

                                                        inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                                                        {
                                                            return (1.0 - t) * a + (t * b);
                                                        }


                                                        inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                                                        {
                                                            float2 i = floor(uv);
                                                            float2 f = frac(uv);
                                                            f = f * f * (3.0 - 2.0 * f);

                                                            uv = abs(frac(uv) - 0.5);
                                                            float2 c0 = i + float2(0.0, 0.0);
                                                            float2 c1 = i + float2(1.0, 0.0);
                                                            float2 c2 = i + float2(0.0, 1.0);
                                                            float2 c3 = i + float2(1.0, 1.0);
                                                            float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                                                            float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                                                            float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                                                            float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                                                            float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                                                            float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                                                            float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                                                            return t;
                                                        }
                                                        void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                                                        {
                                                            float t = 0.0;

                                                            float freq = pow(2.0, float(0));
                                                            float amp = pow(0.5, float(3 - 0));
                                                            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                            freq = pow(2.0, float(1));
                                                            amp = pow(0.5, float(3 - 1));
                                                            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                            freq = pow(2.0, float(2));
                                                            amp = pow(0.5, float(3 - 2));
                                                            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                            Out = t;
                                                        }

                                                        void Unity_Add_float(float A, float B, out float Out)
                                                        {
                                                            Out = A + B;
                                                        }

                                                        void Unity_Step_float(float Edge, float In, out float Out)
                                                        {
                                                            Out = step(Edge, In);
                                                        }

                                                        // Custom interpolators pre vertex
                                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                        // Graph Vertex
                                                        struct VertexDescription
                                                        {
                                                            float3 Position;
                                                            float3 Normal;
                                                            float3 Tangent;
                                                        };

                                                        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                        {
                                                            VertexDescription description = (VertexDescription)0;
                                                            description.Position = IN.ObjectSpacePosition;
                                                            description.Normal = IN.ObjectSpaceNormal;
                                                            description.Tangent = IN.ObjectSpaceTangent;
                                                            return description;
                                                        }

                                                        // Custom interpolators, pre surface
                                                        #ifdef FEATURES_GRAPH_VERTEX
                                                        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                        {
                                                        return output;
                                                        }
                                                        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                        #endif

                                                        // Graph Pixel
                                                        struct SurfaceDescription
                                                        {
                                                            float Alpha;
                                                            float AlphaClipThreshold;
                                                        };

                                                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                        {
                                                            SurfaceDescription surface = (SurfaceDescription)0;
                                                            float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                                                            float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                                                            Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                                                            float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                                                            float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                                                            float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                                                            float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                                                            float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                                                            float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                                                            Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                                                            UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                                            float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                                                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                                                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                                                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                                                            float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                                                            float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                                                            Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                                                            float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                                                            float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                                                            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                                                            float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                                                            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                                                            float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                                                            float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                                                            Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                                                            float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                                            Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                                                            float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                                            float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                                                            float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                                                            float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                                                            float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                                                            Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                                                            float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                                                            Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                                                            float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                                                            Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                                                            float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                                                            Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                                                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                                                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                                                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                                                            float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                                                            surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                                                            surface.AlphaClipThreshold = 0.5;
                                                            return surface;
                                                        }

                                                        // --------------------------------------------------
                                                        // Build Graph Inputs
                                                        #ifdef HAVE_VFX_MODIFICATION
                                                        #define VFX_SRP_ATTRIBUTES Attributes
                                                        #define VFX_SRP_VARYINGS Varyings
                                                        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                        #endif
                                                        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                        {
                                                            VertexDescriptionInputs output;
                                                            ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                            output.ObjectSpaceNormal = input.normalOS;
                                                            output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                            output.ObjectSpacePosition = input.positionOS;

                                                            return output;
                                                        }
                                                        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                        {
                                                            SurfaceDescriptionInputs output;
                                                            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                        #ifdef HAVE_VFX_MODIFICATION
                                                            // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                        #endif







                                                            output.uv0 = input.texCoord0;
                                                            output.uv2 = input.texCoord2;
                                                            output.VertexColor = input.color;
                                                            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                        #else
                                                        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                        #endif
                                                        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                return output;
                                                        }

                                                        // --------------------------------------------------
                                                        // Main

                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                                                        // --------------------------------------------------
                                                        // Visual Effect Vertex Invocations
                                                        #ifdef HAVE_VFX_MODIFICATION
                                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                        #endif

                                                        ENDHLSL
                                                        }
                                                        Pass
                                                        {
                                                            Name "DepthNormals"
                                                            Tags
                                                            {
                                                                "LightMode" = "DepthNormalsOnly"
                                                            }

                                                            // Render State
                                                            Cull Back
                                                            ZTest LEqual
                                                            ZWrite On

                                                            // Debug
                                                            // <None>

                                                            // --------------------------------------------------
                                                            // Pass

                                                            HLSLPROGRAM

                                                            // Pragmas
                                                            #pragma target 2.0
                                                            #pragma only_renderers gles gles3 glcore d3d11
                                                            #pragma multi_compile_instancing
                                                            #pragma multi_compile_fog
                                                            #pragma instancing_options renderinglayer
                                                            #pragma vertex vert
                                                            #pragma fragment frag

                                                            // DotsInstancingOptions: <None>
                                                            // HybridV1InjectedBuiltinProperties: <None>

                                                            // Keywords
                                                            // PassKeywords: <None>
                                                            // GraphKeywords: <None>

                                                            // Defines

                                                            #define ATTRIBUTES_NEED_NORMAL
                                                            #define ATTRIBUTES_NEED_TANGENT
                                                            #define ATTRIBUTES_NEED_TEXCOORD0
                                                            #define ATTRIBUTES_NEED_TEXCOORD2
                                                            #define ATTRIBUTES_NEED_COLOR
                                                            #define VARYINGS_NEED_NORMAL_WS
                                                            #define VARYINGS_NEED_TEXCOORD0
                                                            #define VARYINGS_NEED_TEXCOORD2
                                                            #define VARYINGS_NEED_COLOR
                                                            #define FEATURES_GRAPH_VERTEX
                                                            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                                            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
                                                            #define _ALPHATEST_ON 1
                                                            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                                            // custom interpolator pre-include
                                                            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                                            // Includes
                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                                            // --------------------------------------------------
                                                            // Structs and Packing

                                                            // custom interpolators pre packing
                                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                                            struct Attributes
                                                            {
                                                                 float3 positionOS : POSITION;
                                                                 float3 normalOS : NORMAL;
                                                                 float4 tangentOS : TANGENT;
                                                                 float4 uv0 : TEXCOORD0;
                                                                 float4 uv2 : TEXCOORD2;
                                                                 float4 color : COLOR;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                 uint instanceID : INSTANCEID_SEMANTIC;
                                                                #endif
                                                            };
                                                            struct Varyings
                                                            {
                                                                 float4 positionCS : SV_POSITION;
                                                                 float3 normalWS;
                                                                 float4 texCoord0;
                                                                 float4 texCoord2;
                                                                 float4 color;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                #endif
                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                #endif
                                                            };
                                                            struct SurfaceDescriptionInputs
                                                            {
                                                                 float4 uv0;
                                                                 float4 uv2;
                                                                 float4 VertexColor;
                                                                 float3 TimeParameters;
                                                            };
                                                            struct VertexDescriptionInputs
                                                            {
                                                                 float3 ObjectSpaceNormal;
                                                                 float3 ObjectSpaceTangent;
                                                                 float3 ObjectSpacePosition;
                                                            };
                                                            struct PackedVaryings
                                                            {
                                                                 float4 positionCS : SV_POSITION;
                                                                 float3 interp0 : INTERP0;
                                                                 float4 interp1 : INTERP1;
                                                                 float4 interp2 : INTERP2;
                                                                 float4 interp3 : INTERP3;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                 uint instanceID : CUSTOM_INSTANCE_ID;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                 uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                 uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                                                #endif
                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                 FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                #endif
                                                            };

                                                            PackedVaryings PackVaryings(Varyings input)
                                                            {
                                                                PackedVaryings output;
                                                                ZERO_INITIALIZE(PackedVaryings, output);
                                                                output.positionCS = input.positionCS;
                                                                output.interp0.xyz = input.normalWS;
                                                                output.interp1.xyzw = input.texCoord0;
                                                                output.interp2.xyzw = input.texCoord2;
                                                                output.interp3.xyzw = input.color;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                output.instanceID = input.instanceID;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                #endif
                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                output.cullFace = input.cullFace;
                                                                #endif
                                                                return output;
                                                            }

                                                            Varyings UnpackVaryings(PackedVaryings input)
                                                            {
                                                                Varyings output;
                                                                output.positionCS = input.positionCS;
                                                                output.normalWS = input.interp0.xyz;
                                                                output.texCoord0 = input.interp1.xyzw;
                                                                output.texCoord2 = input.interp2.xyzw;
                                                                output.color = input.interp3.xyzw;
                                                                #if UNITY_ANY_INSTANCING_ENABLED
                                                                output.instanceID = input.instanceID;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                                                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                                                #endif
                                                                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                                                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                                                #endif
                                                                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                output.cullFace = input.cullFace;
                                                                #endif
                                                                return output;
                                                            }


                                                            // --------------------------------------------------
                                                            // Graph

                                                            // Graph Properties
                                                            CBUFFER_START(UnityPerMaterial)
                                                            float2 _Speed;
                                                            float4 _MainTex_TexelSize;
                                                            float4 _Color;
                                                            CBUFFER_END

                                                                // Object and Global properties
                                                                SAMPLER(SamplerState_Linear_Repeat);
                                                                TEXTURE2D(_MainTex);
                                                                SAMPLER(sampler_MainTex);

                                                                // Graph Includes
                                                                // GraphIncludes: <None>

                                                                // -- Property used by ScenePickingPass
                                                                #ifdef SCENEPICKINGPASS
                                                                float4 _SelectionID;
                                                                #endif

                                                                // -- Properties used by SceneSelectionPass
                                                                #ifdef SCENESELECTIONPASS
                                                                int _ObjectId;
                                                                int _PassValue;
                                                                #endif

                                                                // Graph Functions

                                                                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                                                {
                                                                    Out = A * B;
                                                                }

                                                                void Unity_OneMinus_float(float In, out float Out)
                                                                {
                                                                    Out = 1 - In;
                                                                }

                                                                void Unity_Multiply_float_float(float A, float B, out float Out)
                                                                {
                                                                    Out = A * B;
                                                                }

                                                                void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                                                {
                                                                    Out = A * B;
                                                                }

                                                                void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                                                {
                                                                    Out = UV * Tiling + Offset;
                                                                }


                                                                inline float2 Unity_Voronoi_RandomVector_float(float2 UV, float offset)
                                                                {
                                                                    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                                                                    UV = frac(sin(mul(UV, m)));
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
                                                                            float2 lattice = float2(x,y);
                                                                            float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
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


                                                                inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                                                                {
                                                                    float angle = dot(uv, float2(12.9898, 78.233));
                                                                    #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                                                                    // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                                                                    angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                                                                #endif
                                                                return frac(sin(angle) * 43758.5453);
                                                            }

                                                            inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                                                            {
                                                                return (1.0 - t) * a + (t * b);
                                                            }


                                                            inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                                                            {
                                                                float2 i = floor(uv);
                                                                float2 f = frac(uv);
                                                                f = f * f * (3.0 - 2.0 * f);

                                                                uv = abs(frac(uv) - 0.5);
                                                                float2 c0 = i + float2(0.0, 0.0);
                                                                float2 c1 = i + float2(1.0, 0.0);
                                                                float2 c2 = i + float2(0.0, 1.0);
                                                                float2 c3 = i + float2(1.0, 1.0);
                                                                float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                                                                float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                                                                float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                                                                float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                                                                float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                                                                float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                                                                float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                                                                return t;
                                                            }
                                                            void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                                                            {
                                                                float t = 0.0;

                                                                float freq = pow(2.0, float(0));
                                                                float amp = pow(0.5, float(3 - 0));
                                                                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                                freq = pow(2.0, float(1));
                                                                amp = pow(0.5, float(3 - 1));
                                                                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                                freq = pow(2.0, float(2));
                                                                amp = pow(0.5, float(3 - 2));
                                                                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                                                                Out = t;
                                                            }

                                                            void Unity_Add_float(float A, float B, out float Out)
                                                            {
                                                                Out = A + B;
                                                            }

                                                            void Unity_Step_float(float Edge, float In, out float Out)
                                                            {
                                                                Out = step(Edge, In);
                                                            }

                                                            // Custom interpolators pre vertex
                                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                                            // Graph Vertex
                                                            struct VertexDescription
                                                            {
                                                                float3 Position;
                                                                float3 Normal;
                                                                float3 Tangent;
                                                            };

                                                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                                            {
                                                                VertexDescription description = (VertexDescription)0;
                                                                description.Position = IN.ObjectSpacePosition;
                                                                description.Normal = IN.ObjectSpaceNormal;
                                                                description.Tangent = IN.ObjectSpaceTangent;
                                                                return description;
                                                            }

                                                            // Custom interpolators, pre surface
                                                            #ifdef FEATURES_GRAPH_VERTEX
                                                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                                            {
                                                            return output;
                                                            }
                                                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                                            #endif

                                                            // Graph Pixel
                                                            struct SurfaceDescription
                                                            {
                                                                float Alpha;
                                                                float AlphaClipThreshold;
                                                            };

                                                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                            {
                                                                SurfaceDescription surface = (SurfaceDescription)0;
                                                                float4 _Property_82c021cc90554b33ae02ce669fc35095_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
                                                                float4 _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2;
                                                                Unity_Multiply_float4_float4(_Property_82c021cc90554b33ae02ce669fc35095_Out_0, IN.VertexColor, _Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2);
                                                                float4 _UV_7655142ea26e43d58850b72c993ef065_Out_0 = IN.uv0;
                                                                float _Split_2b12e99232594e9c867906d8cc67dd75_R_1 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[0];
                                                                float _Split_2b12e99232594e9c867906d8cc67dd75_G_2 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[1];
                                                                float _Split_2b12e99232594e9c867906d8cc67dd75_B_3 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[2];
                                                                float _Split_2b12e99232594e9c867906d8cc67dd75_A_4 = _UV_7655142ea26e43d58850b72c993ef065_Out_0[3];
                                                                float _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1;
                                                                Unity_OneMinus_float(_Split_2b12e99232594e9c867906d8cc67dd75_G_2, _OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1);
                                                                UnityTexture2D _Property_6e40b51e3c5c4711923863caccf44b01_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                                                                float4 _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6e40b51e3c5c4711923863caccf44b01_Out_0.tex, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.samplerstate, _Property_6e40b51e3c5c4711923863caccf44b01_Out_0.GetTransformedUV(IN.uv2.xy));
                                                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.r;
                                                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_G_5 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.g;
                                                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_B_6 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.b;
                                                                float _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_A_7 = _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_RGBA_0.a;
                                                                float _Multiply_878326a27ae3440881b4721edc734b7e_Out_2;
                                                                Unity_Multiply_float_float(_OneMinus_5a02bea0a37d4768ad80cc7d7f3ea062_Out_1, _SampleTexture2D_dbf5a6e8ea29413aa0c4e7d7da186a54_R_4, _Multiply_878326a27ae3440881b4721edc734b7e_Out_2);
                                                                float2 _Property_56c189412a8f497d878cb7249368e9c6_Out_0 = _Speed;
                                                                float2 _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2;
                                                                Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_56c189412a8f497d878cb7249368e9c6_Out_0, _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2);
                                                                float2 _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3;
                                                                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_9dee036a09d34a69866f45cb2b1cbed5_Out_2, _TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3);
                                                                float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3;
                                                                float _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4;
                                                                Unity_Voronoi_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 2, 7.5, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Cells_4);
                                                                float _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                                                Unity_SimpleNoise_float(_TilingAndOffset_fee91761d7c24ecb9d97eb83216ea992_Out_3, 50, _SimpleNoise_18039dada893457cace85d982b532c82_Out_2);
                                                                float _Split_e981a766084745b1a12cb9101d484e90_R_1 = _SimpleNoise_18039dada893457cace85d982b532c82_Out_2;
                                                                float _Split_e981a766084745b1a12cb9101d484e90_G_2 = 0;
                                                                float _Split_e981a766084745b1a12cb9101d484e90_B_3 = 0;
                                                                float _Split_e981a766084745b1a12cb9101d484e90_A_4 = 0;
                                                                float _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2;
                                                                Unity_Multiply_float_float(_Voronoi_b94eaaa6ee1f4ce49c0d309247782e95_Out_3, _Split_e981a766084745b1a12cb9101d484e90_R_1, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2);
                                                                float _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2;
                                                                Unity_Add_float(_Multiply_878326a27ae3440881b4721edc734b7e_Out_2, _Multiply_e9bf5e90acfd4150a479a5ab6dabe064_Out_2, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2);
                                                                float _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2;
                                                                Unity_Step_float(0.95, _Add_d387bd22f7eb4aba8b6cd0623bdc3204_Out_2, _Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2);
                                                                float4 _Multiply_4401e795467248a1947fb3451b629452_Out_2;
                                                                Unity_Multiply_float4_float4(_Multiply_4a4cc3e759ca4fb79572e86439947140_Out_2, (_Step_378cb5a4b40740b4aee7bfc0d77ffb63_Out_2.xxxx), _Multiply_4401e795467248a1947fb3451b629452_Out_2);
                                                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_R_1 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[0];
                                                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_G_2 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[1];
                                                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_B_3 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[2];
                                                                float _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4 = _Multiply_4401e795467248a1947fb3451b629452_Out_2[3];
                                                                surface.Alpha = _Split_5398459cd9c84a0e86c5fa5611d77ce2_A_4;
                                                                surface.AlphaClipThreshold = 0.5;
                                                                return surface;
                                                            }

                                                            // --------------------------------------------------
                                                            // Build Graph Inputs
                                                            #ifdef HAVE_VFX_MODIFICATION
                                                            #define VFX_SRP_ATTRIBUTES Attributes
                                                            #define VFX_SRP_VARYINGS Varyings
                                                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                                            #endif
                                                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                                            {
                                                                VertexDescriptionInputs output;
                                                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                                output.ObjectSpaceNormal = input.normalOS;
                                                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                                output.ObjectSpacePosition = input.positionOS;

                                                                return output;
                                                            }
                                                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                                            {
                                                                SurfaceDescriptionInputs output;
                                                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                            #ifdef HAVE_VFX_MODIFICATION
                                                                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                                                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                                            #endif







                                                                output.uv0 = input.texCoord0;
                                                                output.uv2 = input.texCoord2;
                                                                output.VertexColor = input.color;
                                                                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                                            #else
                                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                                            #endif
                                                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                                    return output;
                                                            }

                                                            // --------------------------------------------------
                                                            // Main

                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

                                                            // --------------------------------------------------
                                                            // Visual Effect Vertex Invocations
                                                            #ifdef HAVE_VFX_MODIFICATION
                                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                                            #endif

                                                            ENDHLSL
                                                            }
                                }
                                    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
                                                                CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
                                                                FallBack "Hidden/Shader Graph/FallbackError"
}