Shader "QFSW/Blur"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _OverlayColor("Overlay Color", Color) = (1,1,1,1)
        _MainTex("Tint Color (RGB)", 2D) = "white" {}
        _Radius("Radius", Range(0, 20)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        // ===== Pass 1: Horizontal Blur =====
        Pass
        {
            Name "HorizontalBlur"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 screenUV : TEXCOORD0;
                float4 color : COLOR;
            };

            float _Radius;
            float _BlurMultiplier;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);

                float4 screenPos = ComputeScreenPos(o.vertex);
                o.screenUV = screenPos.xy / screenPos.w;

                o.color = v.color;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float radius = _Radius * _BlurMultiplier * i.color.a;
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);

                half4 sum = half4(0,0,0,0);

                half3 c = half3(0,0,0);
                c += SampleSceneColor(float2(i.screenUV.x + texelSize.x * -4.0 * radius, i.screenUV.y)) * 0.05;
                c += SampleSceneColor(float2(i.screenUV.x + texelSize.x * -3.0 * radius, i.screenUV.y)) * 0.09;
                c += SampleSceneColor(float2(i.screenUV.x + texelSize.x * -2.0 * radius, i.screenUV.y)) * 0.12;
                c += SampleSceneColor(float2(i.screenUV.x + texelSize.x * -1.0 * radius, i.screenUV.y)) * 0.15;
                c += SampleSceneColor(float2(i.screenUV.x, i.screenUV.y)) * 0.18;
                c += SampleSceneColor(float2(i.screenUV.x + texelSize.x * 1.0 * radius, i.screenUV.y)) * 0.15;
                c += SampleSceneColor(float2(i.screenUV.x + texelSize.x * 2.0 * radius, i.screenUV.y)) * 0.12;
                c += SampleSceneColor(float2(i.screenUV.x + texelSize.x * 3.0 * radius, i.screenUV.y)) * 0.09;
                c += SampleSceneColor(float2(i.screenUV.x + texelSize.x * 4.0 * radius, i.screenUV.y)) * 0.05;

                return half4(c, 1.0);
            }
            ENDHLSL
        }

        // ===== Pass 2: Vertical Blur =====
        Pass
        {
            Name "VerticalBlur"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 screenUV : TEXCOORD0;
                float4 color : COLOR;
            };

            float _Radius;
            float _BlurMultiplier;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);

                float4 screenPos = ComputeScreenPos(o.vertex);
                o.screenUV = screenPos.xy / screenPos.w;

                o.color = v.color;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float radius = _Radius * _BlurMultiplier * i.color.a;
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);

                half4 sum = half4(0,0,0,0);

                half3 c = half3(0,0,0);
                c += SampleSceneColor(float2(i.screenUV.x, i.screenUV.y + texelSize.y * -4.0 * radius)) * 0.05;
                c += SampleSceneColor(float2(i.screenUV.x, i.screenUV.y + texelSize.y * -3.0 * radius)) * 0.09;
                c += SampleSceneColor(float2(i.screenUV.x, i.screenUV.y + texelSize.y * -2.0 * radius)) * 0.12;
                c += SampleSceneColor(float2(i.screenUV.x, i.screenUV.y + texelSize.y * -1.0 * radius)) * 0.15;
                c += SampleSceneColor(float2(i.screenUV.x, i.screenUV.y)) * 0.18;
                c += SampleSceneColor(float2(i.screenUV.x, i.screenUV.y + texelSize.y * 1.0 * radius)) * 0.15;
                c += SampleSceneColor(float2(i.screenUV.x, i.screenUV.y + texelSize.y * 2.0 * radius)) * 0.12;
                c += SampleSceneColor(float2(i.screenUV.x, i.screenUV.y + texelSize.y * 3.0 * radius)) * 0.09;
                c += SampleSceneColor(float2(i.screenUV.x, i.screenUV.y + texelSize.y * 4.0 * radius)) * 0.05;

                return half4(c, 1.0);
            }
            ENDHLSL
        }

        // ===== Pass 3: Tint & Overlay =====
        Pass
        {
            Name "TintOverlay"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 screenUV : TEXCOORD0;
                float2 uvmain : TEXCOORD1;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            half4 _Color;
            half4 _OverlayColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);

                float4 screenPos = ComputeScreenPos(o.vertex);
                o.screenUV = screenPos.xy / screenPos.w;

                o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 blurredCol = half4(SampleSceneColor(i.screenUV), 1.0);
                half4 texCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uvmain);
                half4 tint = texCol * _Color;

                half4 col = blurredCol * tint;
                col = col * (1 - _OverlayColor.a) + _OverlayColor * _OverlayColor.a;
                col *= i.color;
                col = blurredCol * (1 - texCol.a) + col * texCol.a;

                return col;
            }
            ENDHLSL
        }
    }

    // Fallback cho Built-in pipeline
    SubShader
    {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _OverlayColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 texCol = tex2D(_MainTex, i.uv);
                half4 col = texCol * _Color * i.color;
                col = col * (1 - _OverlayColor.a) + _OverlayColor * _OverlayColor.a;
                return col;
            }
            ENDCG
        }
    }
}