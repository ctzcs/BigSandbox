Shader "Unlit/CustomGpuInstancing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)
        _Offset("Offset",Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        Pass
        {
            
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //1.增加了可选instancing的变体
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaderlibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                //2.顶点着色器输入宏
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                //3.同样的命令加入输出结构体
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float4 _MainTex_ST;
            CBUFFER_END
            //非共享属性块
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(float, _Offset)
            UNITY_INSTANCING_BUFFER_END(Props)
            

            v2f vert (appdata v)
            {
                v2f o;
                //4.instanceid在顶点的相关设置
                UNITY_SETUP_INSTANCE_ID(v);
                //5.传递instanceid顶点到片元
                UNITY_TRANSFER_INSTANCE_ID(v,o);
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                //6.同样片元中也要设置
                UNITY_SETUP_INSTANCE_ID(i);
                // sample the texture
                half4 col = tex2D(_MainTex, i.uv);
                col *= UNITY_ACCESS_INSTANCED_PROP(Props,_Color);//访问Props里面的什么值
                return col;
            }
            ENDHLSL
        }
    }
}
