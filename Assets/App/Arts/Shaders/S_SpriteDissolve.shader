Shader "Custom/S_SpriteDissolve"
{
    Properties
    {
        [NoScaleOffset]_Texture("MainTex", 2D) = "white" {}
        [HDR]_Color("Color", Color) = (0, 0, 0, 1)
        _DissolveSharpness("DissolveSharpness", Float) = 1
        _Dissolve("Dissolve", Range(0, 1)) = 0
        _DissolveScale("DissolveScale", Float) = 20
        _PixelPerUnit("PixelPerUnit", Float) = 16
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        _StencilComp ("Stencil Comparison", Float) = 8
_Stencil ("Stencil ID", Float) = 0
_StencilOp ("Stencil Operation", Float) = 0
_StencilWriteMask ("Stencil Write Mask", Float) = 255
_StencilReadMask ("Stencil Read Mask", Float) = 255
_ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        ColorMask [_ColorMask]
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteLitSubTarget"
        }
        Pass
        {
            Name "Sprite Lit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
            
            Stencil
            {
                Ref 1
    Comp Equal
                Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask 255
WriteMask 255
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZTest LEqual
        ZTest LEqual
        ZTest LEqual
        ZWrite On
        ZWrite On
        ZWrite On
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_0
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_1
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_2
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define VARYINGS_NEED_SCREENPOSITION
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITELIT
        #define _FOG_FRAGMENT 1
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
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
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
             float4 screenPosition;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
             float3 WorldSpacePosition;
             float4 VertexColor;
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
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float4 screenPosition : INTERP2;
             float3 positionWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.screenPosition.xyzw = input.screenPosition;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.screenPosition = input.screenPosition.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        float4 _Texture_TexelSize;
        half4 _Color;
        half _DissolveSharpness;
        half _Dissolve;
        half _DissolveScale;
        half _PixelPerUnit;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture);
        SAMPLER(sampler_Texture);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
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
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Floor_float3(float3 In, out float3 Out)
        {
            Out = floor(In);
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Clamp_half(half In, half Min, half Max, out half Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        float Unity_SimpleNoise_ValueNoise_Deterministic_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);
            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0; Hash_Tchou_2_1_float(c0, r0);
            float r1; Hash_Tchou_2_1_float(c1, r1);
            float r2; Hash_Tchou_2_1_float(c2, r2);
            float r3; Hash_Tchou_2_1_float(c3, r3);
            float bottomOfGrid = lerp(r0, r1, f.x);
            float topOfGrid = lerp(r2, r3, f.x);
            float t = lerp(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        
        void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
        {
            float freq, amp;
            Out = 0.0f;
            freq = pow(2.0, float(0));
            amp = pow(0.5, float(3-0));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        void Unity_Lerp_half(half A, half B, half T, out half Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
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
            half4 SpriteMask;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            half4 _Property_a5b7b3643e334b1c80b4d0ff85d392b7_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
            UnityTexture2D _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture);
            half _Property_6e37f6ff933649c486628084f5cfd9be_Out_0_Float = _PixelPerUnit;
            half _Float_b7de10816de747d0851f2f95945af895_Out_0_Float = _Property_6e37f6ff933649c486628084f5cfd9be_Out_0_Float;
            float3 _Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.WorldSpacePosition, (_Float_b7de10816de747d0851f2f95945af895_Out_0_Float.xxx), _Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3);
            float3 _Floor_23dec4001434472a93783df00012a001_Out_1_Vector3;
            Unity_Floor_float3(_Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3, _Floor_23dec4001434472a93783df00012a001_Out_1_Vector3);
            float3 _Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3;
            Unity_Divide_float3(_Floor_23dec4001434472a93783df00012a001_Out_1_Vector3, (_Float_b7de10816de747d0851f2f95945af895_Out_0_Float.xxx), _Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3);
            float4 _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.tex, _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.samplerstate, _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.GetTransformedUV((_Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3.xy)) );
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_R_4_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.r;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_G_5_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.g;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_B_6_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.b;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_A_7_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.a;
            float4 _Multiply_fabefb38c282461d93a02bbe649631cc_Out_2_Vector4;
            Unity_Multiply_float4_float4(IN.VertexColor, (_SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_R_4_Float.xxxx), _Multiply_fabefb38c282461d93a02bbe649631cc_Out_2_Vector4);
            float4 _Saturate_2664eb9ef856431d892f5ffea72d39a1_Out_1_Vector4;
            Unity_Saturate_float4(_Multiply_fabefb38c282461d93a02bbe649631cc_Out_2_Vector4, _Saturate_2664eb9ef856431d892f5ffea72d39a1_Out_1_Vector4);
            float4 _Multiply_14346f6b50c84c34908d239994b31751_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_a5b7b3643e334b1c80b4d0ff85d392b7_Out_0_Vector4, _Saturate_2664eb9ef856431d892f5ffea72d39a1_Out_1_Vector4, _Multiply_14346f6b50c84c34908d239994b31751_Out_2_Vector4);
            half _Split_f757f961ec084f78b9041554db006442_R_1_Float = IN.VertexColor[0];
            half _Split_f757f961ec084f78b9041554db006442_G_2_Float = IN.VertexColor[1];
            half _Split_f757f961ec084f78b9041554db006442_B_3_Float = IN.VertexColor[2];
            half _Split_f757f961ec084f78b9041554db006442_A_4_Float = IN.VertexColor[3];
            half _Property_2e71d22f8bfa4785ac51531a304627da_Out_0_Float = _DissolveSharpness;
            half _Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float;
            Unity_Clamp_half(_Property_2e71d22f8bfa4785ac51531a304627da_Out_0_Float, half(0), half(1), _Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float);
            half _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float;
            Unity_Multiply_half_half(_Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float, 0.5, _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float);
            half _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float;
            Unity_OneMinus_half(_Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float);
            half _Property_291889a9570c490ab71f5c070018deba_Out_0_Float = _DissolveScale;
            half _Float_dda53c2131f846acbd68456e78352a1a_Out_0_Float = _Property_291889a9570c490ab71f5c070018deba_Out_0_Float;
            float _SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float;
            Unity_SimpleNoise_Deterministic_float((_Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3.xy), _Float_dda53c2131f846acbd68456e78352a1a_Out_0_Float, _SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float);
            float _OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float;
            Unity_OneMinus_float(_SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float, _OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float);
            float _Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float;
            Unity_Saturate_float(_OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float, _Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float);
            half _Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float;
            Unity_Add_half(half(-1), _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float);
            half _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float;
            Unity_Subtract_half(half(1), _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float);
            half _Property_61e8f2f98d0047c188ec9d7787ce2d33_Out_0_Float = _Dissolve;
            half _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float;
            Unity_Lerp_half(_Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float, _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float, _Property_61e8f2f98d0047c188ec9d7787ce2d33_Out_0_Float, _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float);
            float _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float;
            Unity_Add_float(_Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float, _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float, _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float);
            float _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float;
            Unity_Smoothstep_float(_Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float, _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float, _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float);
            float _Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float;
            Unity_Subtract_float(_SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_A_7_Float, _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float, _Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float);
            float _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float;
            Unity_Saturate_float(_Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float, _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float);
            float _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float;
            Unity_Multiply_float_float(_Split_f757f961ec084f78b9041554db006442_A_4_Float, _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float, _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float);
            surface.BaseColor = (_Multiply_14346f6b50c84c34908d239994b31751_Out_2_Vector4.xyz);
            surface.Alpha = _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float;
            surface.SpriteMask = IsGammaSpace() ? half4(1, 1, 1, 1) : half4 (SRGBToLinear(half3(1, 1, 1)), 1);
            surface.AlphaClipThreshold = half(0);
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
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteLitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Normal"
            Tags
            {
                "LightMode" = "NormalsRendering"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZTest LEqual
        ZTest LEqual
        ZTest LEqual
        ZWrite On
        ZWrite On
        ZWrite On
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITENORMAL
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"
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
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
             float3 TangentSpaceNormal;
             float3 WorldSpacePosition;
             float4 VertexColor;
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
             float4 tangentWS : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        float4 _Texture_TexelSize;
        half4 _Color;
        half _DissolveSharpness;
        half _Dissolve;
        half _DissolveScale;
        half _PixelPerUnit;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture);
        SAMPLER(sampler_Texture);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
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
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Floor_float3(float3 In, out float3 Out)
        {
            Out = floor(In);
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Clamp_half(half In, half Min, half Max, out half Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        float Unity_SimpleNoise_ValueNoise_Deterministic_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);
            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0; Hash_Tchou_2_1_float(c0, r0);
            float r1; Hash_Tchou_2_1_float(c1, r1);
            float r2; Hash_Tchou_2_1_float(c2, r2);
            float r3; Hash_Tchou_2_1_float(c3, r3);
            float bottomOfGrid = lerp(r0, r1, f.x);
            float topOfGrid = lerp(r2, r3, f.x);
            float t = lerp(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        
        void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
        {
            float freq, amp;
            Out = 0.0f;
            freq = pow(2.0, float(0));
            amp = pow(0.5, float(3-0));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        void Unity_Lerp_half(half A, half B, half T, out half Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
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
            half3 NormalTS;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            half4 _Property_a5b7b3643e334b1c80b4d0ff85d392b7_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
            UnityTexture2D _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture);
            half _Property_6e37f6ff933649c486628084f5cfd9be_Out_0_Float = _PixelPerUnit;
            half _Float_b7de10816de747d0851f2f95945af895_Out_0_Float = _Property_6e37f6ff933649c486628084f5cfd9be_Out_0_Float;
            float3 _Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.WorldSpacePosition, (_Float_b7de10816de747d0851f2f95945af895_Out_0_Float.xxx), _Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3);
            float3 _Floor_23dec4001434472a93783df00012a001_Out_1_Vector3;
            Unity_Floor_float3(_Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3, _Floor_23dec4001434472a93783df00012a001_Out_1_Vector3);
            float3 _Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3;
            Unity_Divide_float3(_Floor_23dec4001434472a93783df00012a001_Out_1_Vector3, (_Float_b7de10816de747d0851f2f95945af895_Out_0_Float.xxx), _Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3);
            float4 _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.tex, _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.samplerstate, _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.GetTransformedUV((_Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3.xy)) );
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_R_4_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.r;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_G_5_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.g;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_B_6_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.b;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_A_7_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.a;
            float4 _Multiply_fabefb38c282461d93a02bbe649631cc_Out_2_Vector4;
            Unity_Multiply_float4_float4(IN.VertexColor, (_SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_R_4_Float.xxxx), _Multiply_fabefb38c282461d93a02bbe649631cc_Out_2_Vector4);
            float4 _Saturate_2664eb9ef856431d892f5ffea72d39a1_Out_1_Vector4;
            Unity_Saturate_float4(_Multiply_fabefb38c282461d93a02bbe649631cc_Out_2_Vector4, _Saturate_2664eb9ef856431d892f5ffea72d39a1_Out_1_Vector4);
            float4 _Multiply_14346f6b50c84c34908d239994b31751_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_a5b7b3643e334b1c80b4d0ff85d392b7_Out_0_Vector4, _Saturate_2664eb9ef856431d892f5ffea72d39a1_Out_1_Vector4, _Multiply_14346f6b50c84c34908d239994b31751_Out_2_Vector4);
            half _Split_f757f961ec084f78b9041554db006442_R_1_Float = IN.VertexColor[0];
            half _Split_f757f961ec084f78b9041554db006442_G_2_Float = IN.VertexColor[1];
            half _Split_f757f961ec084f78b9041554db006442_B_3_Float = IN.VertexColor[2];
            half _Split_f757f961ec084f78b9041554db006442_A_4_Float = IN.VertexColor[3];
            half _Property_2e71d22f8bfa4785ac51531a304627da_Out_0_Float = _DissolveSharpness;
            half _Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float;
            Unity_Clamp_half(_Property_2e71d22f8bfa4785ac51531a304627da_Out_0_Float, half(0), half(1), _Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float);
            half _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float;
            Unity_Multiply_half_half(_Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float, 0.5, _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float);
            half _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float;
            Unity_OneMinus_half(_Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float);
            half _Property_291889a9570c490ab71f5c070018deba_Out_0_Float = _DissolveScale;
            half _Float_dda53c2131f846acbd68456e78352a1a_Out_0_Float = _Property_291889a9570c490ab71f5c070018deba_Out_0_Float;
            float _SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float;
            Unity_SimpleNoise_Deterministic_float((_Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3.xy), _Float_dda53c2131f846acbd68456e78352a1a_Out_0_Float, _SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float);
            float _OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float;
            Unity_OneMinus_float(_SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float, _OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float);
            float _Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float;
            Unity_Saturate_float(_OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float, _Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float);
            half _Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float;
            Unity_Add_half(half(-1), _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float);
            half _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float;
            Unity_Subtract_half(half(1), _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float);
            half _Property_61e8f2f98d0047c188ec9d7787ce2d33_Out_0_Float = _Dissolve;
            half _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float;
            Unity_Lerp_half(_Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float, _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float, _Property_61e8f2f98d0047c188ec9d7787ce2d33_Out_0_Float, _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float);
            float _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float;
            Unity_Add_float(_Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float, _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float, _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float);
            float _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float;
            Unity_Smoothstep_float(_Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float, _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float, _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float);
            float _Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float;
            Unity_Subtract_float(_SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_A_7_Float, _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float, _Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float);
            float _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float;
            Unity_Saturate_float(_Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float, _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float);
            float _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float;
            Unity_Multiply_float_float(_Split_f757f961ec084f78b9041554db006442_A_4_Float, _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float, _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float);
            surface.BaseColor = (_Multiply_14346f6b50c84c34908d239994b31751_Out_2_Vector4.xyz);
            surface.Alpha = _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float;
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.AlphaClipThreshold = half(0);
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
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.WorldSpacePosition = input.positionWS;
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteNormalPass.hlsl"
        
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
        #pragma exclude_renderers d3d11_9x
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
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
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
             float3 WorldSpacePosition;
             float4 VertexColor;
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
             float4 color : INTERP0;
             float3 positionWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        float4 _Texture_TexelSize;
        half4 _Color;
        half _DissolveSharpness;
        half _Dissolve;
        half _DissolveScale;
        half _PixelPerUnit;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture);
        SAMPLER(sampler_Texture);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
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
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Floor_float3(float3 In, out float3 Out)
        {
            Out = floor(In);
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_Clamp_half(half In, half Min, half Max, out half Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        float Unity_SimpleNoise_ValueNoise_Deterministic_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);
            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0; Hash_Tchou_2_1_float(c0, r0);
            float r1; Hash_Tchou_2_1_float(c1, r1);
            float r2; Hash_Tchou_2_1_float(c2, r2);
            float r3; Hash_Tchou_2_1_float(c3, r3);
            float bottomOfGrid = lerp(r0, r1, f.x);
            float topOfGrid = lerp(r2, r3, f.x);
            float t = lerp(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        
        void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
        {
            float freq, amp;
            Out = 0.0f;
            freq = pow(2.0, float(0));
            amp = pow(0.5, float(3-0));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        void Unity_Lerp_half(half A, half B, half T, out half Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
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
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            half _Split_f757f961ec084f78b9041554db006442_R_1_Float = IN.VertexColor[0];
            half _Split_f757f961ec084f78b9041554db006442_G_2_Float = IN.VertexColor[1];
            half _Split_f757f961ec084f78b9041554db006442_B_3_Float = IN.VertexColor[2];
            half _Split_f757f961ec084f78b9041554db006442_A_4_Float = IN.VertexColor[3];
            UnityTexture2D _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture);
            half _Property_6e37f6ff933649c486628084f5cfd9be_Out_0_Float = _PixelPerUnit;
            half _Float_b7de10816de747d0851f2f95945af895_Out_0_Float = _Property_6e37f6ff933649c486628084f5cfd9be_Out_0_Float;
            float3 _Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.WorldSpacePosition, (_Float_b7de10816de747d0851f2f95945af895_Out_0_Float.xxx), _Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3);
            float3 _Floor_23dec4001434472a93783df00012a001_Out_1_Vector3;
            Unity_Floor_float3(_Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3, _Floor_23dec4001434472a93783df00012a001_Out_1_Vector3);
            float3 _Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3;
            Unity_Divide_float3(_Floor_23dec4001434472a93783df00012a001_Out_1_Vector3, (_Float_b7de10816de747d0851f2f95945af895_Out_0_Float.xxx), _Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3);
            float4 _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.tex, _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.samplerstate, _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.GetTransformedUV((_Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3.xy)) );
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_R_4_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.r;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_G_5_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.g;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_B_6_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.b;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_A_7_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.a;
            half _Property_2e71d22f8bfa4785ac51531a304627da_Out_0_Float = _DissolveSharpness;
            half _Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float;
            Unity_Clamp_half(_Property_2e71d22f8bfa4785ac51531a304627da_Out_0_Float, half(0), half(1), _Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float);
            half _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float;
            Unity_Multiply_half_half(_Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float, 0.5, _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float);
            half _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float;
            Unity_OneMinus_half(_Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float);
            half _Property_291889a9570c490ab71f5c070018deba_Out_0_Float = _DissolveScale;
            half _Float_dda53c2131f846acbd68456e78352a1a_Out_0_Float = _Property_291889a9570c490ab71f5c070018deba_Out_0_Float;
            float _SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float;
            Unity_SimpleNoise_Deterministic_float((_Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3.xy), _Float_dda53c2131f846acbd68456e78352a1a_Out_0_Float, _SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float);
            float _OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float;
            Unity_OneMinus_float(_SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float, _OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float);
            float _Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float;
            Unity_Saturate_float(_OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float, _Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float);
            half _Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float;
            Unity_Add_half(half(-1), _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float);
            half _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float;
            Unity_Subtract_half(half(1), _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float);
            half _Property_61e8f2f98d0047c188ec9d7787ce2d33_Out_0_Float = _Dissolve;
            half _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float;
            Unity_Lerp_half(_Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float, _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float, _Property_61e8f2f98d0047c188ec9d7787ce2d33_Out_0_Float, _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float);
            float _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float;
            Unity_Add_float(_Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float, _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float, _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float);
            float _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float;
            Unity_Smoothstep_float(_Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float, _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float, _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float);
            float _Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float;
            Unity_Subtract_float(_SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_A_7_Float, _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float, _Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float);
            float _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float;
            Unity_Saturate_float(_Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float, _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float);
            float _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float;
            Unity_Multiply_float_float(_Split_f757f961ec084f78b9041554db006442_A_4_Float, _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float, _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float);
            surface.Alpha = _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float;
            surface.AlphaClipThreshold = half(0);
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
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
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
        Cull [_Cull]
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
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
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
             float3 WorldSpacePosition;
             float4 VertexColor;
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
             float4 color : INTERP0;
             float3 positionWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        float4 _Texture_TexelSize;
        half4 _Color;
        half _DissolveSharpness;
        half _Dissolve;
        half _DissolveScale;
        half _PixelPerUnit;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture);
        SAMPLER(sampler_Texture);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
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
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Floor_float3(float3 In, out float3 Out)
        {
            Out = floor(In);
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_Clamp_half(half In, half Min, half Max, out half Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        float Unity_SimpleNoise_ValueNoise_Deterministic_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);
            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0; Hash_Tchou_2_1_float(c0, r0);
            float r1; Hash_Tchou_2_1_float(c1, r1);
            float r2; Hash_Tchou_2_1_float(c2, r2);
            float r3; Hash_Tchou_2_1_float(c3, r3);
            float bottomOfGrid = lerp(r0, r1, f.x);
            float topOfGrid = lerp(r2, r3, f.x);
            float t = lerp(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        
        void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
        {
            float freq, amp;
            Out = 0.0f;
            freq = pow(2.0, float(0));
            amp = pow(0.5, float(3-0));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        void Unity_Lerp_half(half A, half B, half T, out half Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
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
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            half _Split_f757f961ec084f78b9041554db006442_R_1_Float = IN.VertexColor[0];
            half _Split_f757f961ec084f78b9041554db006442_G_2_Float = IN.VertexColor[1];
            half _Split_f757f961ec084f78b9041554db006442_B_3_Float = IN.VertexColor[2];
            half _Split_f757f961ec084f78b9041554db006442_A_4_Float = IN.VertexColor[3];
            UnityTexture2D _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture);
            half _Property_6e37f6ff933649c486628084f5cfd9be_Out_0_Float = _PixelPerUnit;
            half _Float_b7de10816de747d0851f2f95945af895_Out_0_Float = _Property_6e37f6ff933649c486628084f5cfd9be_Out_0_Float;
            float3 _Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.WorldSpacePosition, (_Float_b7de10816de747d0851f2f95945af895_Out_0_Float.xxx), _Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3);
            float3 _Floor_23dec4001434472a93783df00012a001_Out_1_Vector3;
            Unity_Floor_float3(_Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3, _Floor_23dec4001434472a93783df00012a001_Out_1_Vector3);
            float3 _Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3;
            Unity_Divide_float3(_Floor_23dec4001434472a93783df00012a001_Out_1_Vector3, (_Float_b7de10816de747d0851f2f95945af895_Out_0_Float.xxx), _Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3);
            float4 _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.tex, _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.samplerstate, _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.GetTransformedUV((_Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3.xy)) );
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_R_4_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.r;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_G_5_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.g;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_B_6_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.b;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_A_7_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.a;
            half _Property_2e71d22f8bfa4785ac51531a304627da_Out_0_Float = _DissolveSharpness;
            half _Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float;
            Unity_Clamp_half(_Property_2e71d22f8bfa4785ac51531a304627da_Out_0_Float, half(0), half(1), _Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float);
            half _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float;
            Unity_Multiply_half_half(_Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float, 0.5, _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float);
            half _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float;
            Unity_OneMinus_half(_Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float);
            half _Property_291889a9570c490ab71f5c070018deba_Out_0_Float = _DissolveScale;
            half _Float_dda53c2131f846acbd68456e78352a1a_Out_0_Float = _Property_291889a9570c490ab71f5c070018deba_Out_0_Float;
            float _SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float;
            Unity_SimpleNoise_Deterministic_float((_Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3.xy), _Float_dda53c2131f846acbd68456e78352a1a_Out_0_Float, _SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float);
            float _OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float;
            Unity_OneMinus_float(_SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float, _OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float);
            float _Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float;
            Unity_Saturate_float(_OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float, _Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float);
            half _Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float;
            Unity_Add_half(half(-1), _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float);
            half _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float;
            Unity_Subtract_half(half(1), _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float);
            half _Property_61e8f2f98d0047c188ec9d7787ce2d33_Out_0_Float = _Dissolve;
            half _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float;
            Unity_Lerp_half(_Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float, _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float, _Property_61e8f2f98d0047c188ec9d7787ce2d33_Out_0_Float, _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float);
            float _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float;
            Unity_Add_float(_Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float, _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float, _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float);
            float _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float;
            Unity_Smoothstep_float(_Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float, _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float, _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float);
            float _Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float;
            Unity_Subtract_float(_SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_A_7_Float, _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float, _Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float);
            float _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float;
            Unity_Saturate_float(_Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float, _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float);
            float _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float;
            Unity_Multiply_float_float(_Split_f757f961ec084f78b9041554db006442_A_4_Float, _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float, _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float);
            surface.Alpha = _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float;
            surface.AlphaClipThreshold = half(0);
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
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
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
            Name "Sprite Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZTest LEqual
        ZTest LEqual
        ZTest LEqual
        ZWrite On
        ZWrite On
        ZWrite On
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        #define _FOG_FRAGMENT 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
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
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
             float3 TangentSpaceNormal;
             float3 WorldSpacePosition;
             float4 VertexColor;
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
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
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
        float4 _Texture_TexelSize;
        half4 _Color;
        half _DissolveSharpness;
        half _Dissolve;
        half _DissolveScale;
        half _PixelPerUnit;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture);
        SAMPLER(sampler_Texture);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
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
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Floor_float3(float3 In, out float3 Out)
        {
            Out = floor(In);
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Clamp_half(half In, half Min, half Max, out half Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        float Unity_SimpleNoise_ValueNoise_Deterministic_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);
            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0; Hash_Tchou_2_1_float(c0, r0);
            float r1; Hash_Tchou_2_1_float(c1, r1);
            float r2; Hash_Tchou_2_1_float(c2, r2);
            float r3; Hash_Tchou_2_1_float(c3, r3);
            float bottomOfGrid = lerp(r0, r1, f.x);
            float topOfGrid = lerp(r2, r3, f.x);
            float t = lerp(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        
        void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
        {
            float freq, amp;
            Out = 0.0f;
            freq = pow(2.0, float(0));
            amp = pow(0.5, float(3-0));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        void Unity_Lerp_half(half A, half B, half T, out half Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
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
            half3 NormalTS;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            half4 _Property_a5b7b3643e334b1c80b4d0ff85d392b7_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
            UnityTexture2D _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture);
            half _Property_6e37f6ff933649c486628084f5cfd9be_Out_0_Float = _PixelPerUnit;
            half _Float_b7de10816de747d0851f2f95945af895_Out_0_Float = _Property_6e37f6ff933649c486628084f5cfd9be_Out_0_Float;
            float3 _Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.WorldSpacePosition, (_Float_b7de10816de747d0851f2f95945af895_Out_0_Float.xxx), _Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3);
            float3 _Floor_23dec4001434472a93783df00012a001_Out_1_Vector3;
            Unity_Floor_float3(_Multiply_8cb96d59e3c443f7aeed870e8423b360_Out_2_Vector3, _Floor_23dec4001434472a93783df00012a001_Out_1_Vector3);
            float3 _Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3;
            Unity_Divide_float3(_Floor_23dec4001434472a93783df00012a001_Out_1_Vector3, (_Float_b7de10816de747d0851f2f95945af895_Out_0_Float.xxx), _Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3);
            float4 _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.tex, _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.samplerstate, _Property_d9aea57191ba405d9b1a518109efc518_Out_0_Texture2D.GetTransformedUV((_Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3.xy)) );
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_R_4_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.r;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_G_5_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.g;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_B_6_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.b;
            float _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_A_7_Float = _SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_RGBA_0_Vector4.a;
            float4 _Multiply_fabefb38c282461d93a02bbe649631cc_Out_2_Vector4;
            Unity_Multiply_float4_float4(IN.VertexColor, (_SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_R_4_Float.xxxx), _Multiply_fabefb38c282461d93a02bbe649631cc_Out_2_Vector4);
            float4 _Saturate_2664eb9ef856431d892f5ffea72d39a1_Out_1_Vector4;
            Unity_Saturate_float4(_Multiply_fabefb38c282461d93a02bbe649631cc_Out_2_Vector4, _Saturate_2664eb9ef856431d892f5ffea72d39a1_Out_1_Vector4);
            float4 _Multiply_14346f6b50c84c34908d239994b31751_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_a5b7b3643e334b1c80b4d0ff85d392b7_Out_0_Vector4, _Saturate_2664eb9ef856431d892f5ffea72d39a1_Out_1_Vector4, _Multiply_14346f6b50c84c34908d239994b31751_Out_2_Vector4);
            half _Split_f757f961ec084f78b9041554db006442_R_1_Float = IN.VertexColor[0];
            half _Split_f757f961ec084f78b9041554db006442_G_2_Float = IN.VertexColor[1];
            half _Split_f757f961ec084f78b9041554db006442_B_3_Float = IN.VertexColor[2];
            half _Split_f757f961ec084f78b9041554db006442_A_4_Float = IN.VertexColor[3];
            half _Property_2e71d22f8bfa4785ac51531a304627da_Out_0_Float = _DissolveSharpness;
            half _Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float;
            Unity_Clamp_half(_Property_2e71d22f8bfa4785ac51531a304627da_Out_0_Float, half(0), half(1), _Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float);
            half _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float;
            Unity_Multiply_half_half(_Clamp_f06e9754528542f3ac4d7486b21b7cb4_Out_3_Float, 0.5, _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float);
            half _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float;
            Unity_OneMinus_half(_Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float);
            half _Property_291889a9570c490ab71f5c070018deba_Out_0_Float = _DissolveScale;
            half _Float_dda53c2131f846acbd68456e78352a1a_Out_0_Float = _Property_291889a9570c490ab71f5c070018deba_Out_0_Float;
            float _SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float;
            Unity_SimpleNoise_Deterministic_float((_Divide_7ea60dd3bad3449f8267aaa01dbb8d0e_Out_2_Vector3.xy), _Float_dda53c2131f846acbd68456e78352a1a_Out_0_Float, _SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float);
            float _OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float;
            Unity_OneMinus_float(_SimpleNoise_4bcf176f1d4f4e069cac0b793015b81c_Out_2_Float, _OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float);
            float _Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float;
            Unity_Saturate_float(_OneMinus_3daddcdc5ebf43ae8b1770f24f0af63f_Out_1_Float, _Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float);
            half _Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float;
            Unity_Add_half(half(-1), _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float);
            half _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float;
            Unity_Subtract_half(half(1), _Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float);
            half _Property_61e8f2f98d0047c188ec9d7787ce2d33_Out_0_Float = _Dissolve;
            half _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float;
            Unity_Lerp_half(_Add_baf5a6f21fe04d5ab218ba9316ab7502_Out_2_Float, _Subtract_b1183fdc70c44fc5b7cebe60e3d3bc20_Out_2_Float, _Property_61e8f2f98d0047c188ec9d7787ce2d33_Out_0_Float, _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float);
            float _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float;
            Unity_Add_float(_Saturate_26149c3588784ee398d3500bc493ea9a_Out_1_Float, _Lerp_99d75bffb7f04fa9bd073982956be4a0_Out_3_Float, _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float);
            float _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float;
            Unity_Smoothstep_float(_Multiply_f94e9a775d944802a8a98210b2924a79_Out_2_Float, _OneMinus_b457c17dcff24954a6d33bc5bdef1aa1_Out_1_Float, _Add_dc9bdc97d8ac420a89bbec2fbdcde921_Out_2_Float, _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float);
            float _Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float;
            Unity_Subtract_float(_SampleTexture2D_b716ffc9492244a1af58108111e2b0dd_A_7_Float, _Smoothstep_d23fcd44fa2f4b8db703ae30f69ed1b4_Out_3_Float, _Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float);
            float _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float;
            Unity_Saturate_float(_Subtract_8dca24e80e99494daec20825f3b19ffa_Out_2_Float, _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float);
            float _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float;
            Unity_Multiply_float_float(_Split_f757f961ec084f78b9041554db006442_A_4_Float, _Saturate_40f9b6f48fe648fdaa22994e6230920e_Out_1_Float, _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float);
            surface.BaseColor = (_Multiply_14346f6b50c84c34908d239994b31751_Out_2_Vector4.xyz);
            surface.Alpha = _Multiply_d33a7e9553b1496b9811e29356136fed_Out_2_Float;
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.AlphaClipThreshold = half(0);
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
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.WorldSpacePosition = input.positionWS;
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteForwardPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphSpriteGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}