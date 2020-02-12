﻿Shader "BasicStereoTexture"
{
    Properties
    {
        _LeftTex ("Left Texture", 2D) = "white" {}
        _RightTex ("Right Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
  
            #include "UnityCG.cginc"

            struct vertexInput
            {
                float4 vertex : POSITION; //object space position of vertex
                float2 uv : TEXCOORD0; //UV coordinates for the images _LeftTex, _RightTex
            };

            struct vertexOutput
            {
                float2 uvLeft : TEXCOORD0; //differentiate between the two images for each eye by having UV for each
				float2 uvRight : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _LeftTex;
            uniform float4 _LeftTex_ST;
            sampler2D _RightTex;
            uniform float4 _RightTex_ST;

            vertexOutput vert (vertexInput i)
            {
                vertexOutput o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uvLeft = TRANSFORM_TEX(i.uv, _LeftTex);
                o.uvRight = TRANSFORM_TEX(i.uv, _RightTex);
                return o;
            }

            float4 frag (vertexOutput i) : SV_Target
            {
                return lerp(tex2D(_LeftTex, i.uvLeft), 
                    tex2D(_RightTex, i.uvRight), 
                    unity_StereoEyeIndex); // selecting which eye to render to.
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}