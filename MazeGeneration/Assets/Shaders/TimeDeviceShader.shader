Shader "Custom/TimeDeviceShader"
{
	Properties
	{
        _LeftTex ("Left Texture", 2D) = "white" {}
		_RightTex ("Right Texture", 2D) = "white" {}
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "UnityStandardUtils.cginc"

				struct VertIn
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct FragIn
				{
					float4 vertex : SV_POSITION;
					float2 uvL : TEXCOORD0;
					float2 uvR : TEXCOORD1;
					float3 screen_uv : TEXCOORD2;
				};

				sampler2D _LeftTex;
            	float4 _LeftTex_ST;

				sampler2D _RightTex;
            	float4 _RightTex_ST;

				FragIn vert(VertIn v)
				{
					FragIn o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.screen_uv = float3((o.vertex.xy + o.vertex.w) * 0.5, o.vertex.w);
					o.uvL = TRANSFORM_TEX(v.uv, _LeftTex);
					o.uvR = TRANSFORM_TEX(v.uv, _RightTex);
					
					return o;
				}


				fixed4 frag(FragIn i) : SV_Target
				{
					// Perspective correction for screen uv coordinate
					float2 screen_uv = i.screen_uv.xy / i.screen_uv.z;
					screen_uv.y = 1-screen_uv.y;

					// sample the texture
                	fixed4 col;
					fixed4 l = tex2D(_LeftTex, i.uvL);
					fixed4 r = tex2D(_RightTex, i.uvR);

					l = tex2D(_LeftTex, screen_uv);
					r = tex2D(_RightTex, screen_uv);
 				
					//color pixel to allow stereoscopic rendering for VR
					col = r * unity_StereoEyeIndex + l * (1 - unity_StereoEyeIndex);
					
					//color pixel based on Stencil Buffer but not stereoscopic
					//col = tex2D(_MainTex, screen_uv);

					return col;
				}
				
				ENDCG
			}
		}
}