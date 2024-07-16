
Shader "Universal Render Pipeline/Unlit Indirect Shader"
{
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.5

        // BlendMode
        _Surface("__surface", Float) = 0.0
        _Blend("__mode", Float) = 0.0
        _Cull("__cull", Float) = 2.0
        [ToggleUI] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _BlendOp("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0

        // Editmode props
        _QueueOffset("Queue offset", Float) = 0.0

        // ObsoleteProperties
        [HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
        [HideInInspector] _Color("Base Color", Color) = (0.5, 0.5, 0.5, 1)
        [HideInInspector] _SampleGI("SampleGI", float) = 0.0 // needed from bakedlit
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel"="4.5"}
        LOD 100

        Blend [_SrcBlend][_DstBlend]
        ZWrite [_ZWrite]
        Cull [_Cull]

        Pass
        {
            Name "UnlitExample"

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            //Unity Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile _ DEBUG_DISPLAY

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitForwardPass.hlsl"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"

            //暴露的属性块
            CBUFFER_START(UnityPerMaterial)
            float4x4 _ObjectToWorld;
            // StructuredBuffer<float4x4> _Matrices;
            StructuredBuffer<float3> _Float3Pos;
            StructuredBuffer<float3> _Colors;
            CBUFFER_END
            
            struct Appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                
                #if defined(DEBUG_DISPLAY)
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #endif

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float fogCoord : TEXCOORD1;
                float4 positionCS : SV_POSITION;
                uint instanceId:INSTANCEID_SEMANTIC;
                #if defined(DEBUG_DISPLAY)
                float3 positionWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                #endif

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            void InitializeInputData(v2f input, out InputData inputData)
            {
                inputData = (InputData)0;

                #if defined(DEBUG_DISPLAY)
                inputData.positionWS = input.positionWS;
                inputData.normalWS = input.normalWS;
                inputData.viewDirectionWS = input.viewDirWS;
                #else
                inputData.positionWS = float3(0, 0, 0);
                inputData.normalWS = half3(0, 0, 1);
                inputData.viewDirectionWS = half3(0, 0, 1);
                #endif
                inputData.shadowCoord = 0;
                inputData.fogCoord = 0;
                inputData.vertexLighting = half3(0, 0, 0);
                inputData.bakedGI = half3(0, 0, 0);
                inputData.normalizedScreenSpaceUV = 0;
                inputData.shadowMask = half4(1, 1, 1, 1);
            }

            v2f vert(Appdata input,uint instanceID:SV_InstanceID)
            {
                InitIndirectDrawArgs(0);

                VertexPositionInputs vertexInput; //= GetVertexPositionInputs(input.positionOS.xyz);
                float4x4 temp = float4x4(
                    1,0,0,instanceID,
                    0,1,0,instanceID,
                    0,0,1,instanceID,
                    0,0,0,1
                );
                
                //float3 worldPos = input.positionOS.xyz + transpose(_Matrices[instanceID])[3].xyz;//这句是使用matrices得时候用的
                float3 worldPos = input.positionOS.xyz + _Float3Pos[instanceID].xyz;
                
                v2f output = (v2f)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                vertexInput.positionWS = TransformObjectToWorld(worldPos);
                vertexInput.positionVS = TransformWorldToView(vertexInput.positionWS);
                vertexInput.positionCS = TransformWorldToHClip(vertexInput.positionWS);

                float4 ndc = vertexInput.positionCS * 0.5f;
                vertexInput.positionNDC.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
                vertexInput.positionNDC.zw = vertexInput.positionCS.zw;
                
                output.positionCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                
                #if defined(_FOG_FRAGMENT)
                output.fogCoord = vertexInput.positionVS.z;
                #else
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                #endif

                #if defined(DEBUG_DISPLAY)
                // normalWS and tangentWS already normalize.
                // this is required to avoid skewing the direction during interpolation
                // also required for per-vertex lighting and SH evaluation
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

                // already normalized from normal transform to WS.
                
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = viewDirWS;
                #endif

                output.instanceId = instanceID;
                
                return output;
            }

            
            half4 frag(v2f input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                half2 uv = input.uv;
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                half3 color = texColor.rgb * _BaseColor.rgb;
                
                
                half alpha = texColor.a * _BaseColor.a;

                AlphaDiscard(alpha, _Cutoff);

                #if defined(_ALPHAPREMULTIPLY_ON)
                color *= alpha;
                #endif

                InputData inputData;
                InitializeInputData(input, inputData);
                SETUP_DEBUG_TEXTURE_DATA(inputData, input.uv, _BaseMap);
                
            #ifdef _DBUFFER
                ApplyDecalToBaseColor(input.positionCS, color);
            #endif

                #if defined(_FOG_FRAGMENT)
                    #if (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))
                    float viewZ = -input.fogCoord;
                    float nearToFarZ = max(viewZ - _ProjectionParams.y, 0);
                    half fogFactor = ComputeFogFactorZ0ToFar(nearToFarZ);
                    #else
                    half fogFactor = 0;
                    #endif
                #else
                half fogFactor = input.fogCoord;
                #endif
                half4 finalColor = UniversalFragmentUnlit(inputData, color, alpha);

            #if defined(_SCREEN_SPACE_OCCLUSION) && !defined(_SURFACE_TYPE_TRANSPARENT)
                float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(normalizedScreenSpaceUV);
                finalColor.rgb *= aoFactor.directAmbientOcclusion;
            #endif
                
                 
                float3 matColor = _Colors[input.instanceId];
                finalColor.rgb *= matColor;
                finalColor.rgb = MixFog(finalColor.rgb , fogFactor);
                
                return finalColor ;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormalsOnly"
            Tags{"LightMode" = "DepthNormalsOnly"}

            ZWrite On

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT // forward-only variant

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitDepthNormalsPass.hlsl"
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaUnlit
            #pragma shader_feature EDITOR_VISUALIZATION

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitMetaPass.hlsl"
            ENDHLSL
        }
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel"="2.0"}
        LOD 100

        Blend [_SrcBlend][_DstBlend]
        ZWrite [_ZWrite]
        Cull [_Cull]

        Pass
        {
            Name "Unlit"

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ DEBUG_DISPLAY

            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitForwardPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormalsOnly"
            Tags{"LightMode" = "DepthNormalsOnly"}

            ZWrite On

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore
            #pragma target 2.0

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT // forward-only variant

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitDepthNormalsPass.hlsl"
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaUnlit
            #pragma shader_feature EDITOR_VISUALIZATION

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitMetaPass.hlsl"

            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitShader"
}
