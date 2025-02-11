Shader "KriptoFX/RFX1/Decal/WorldMask" {
Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_Cutoff("Cutoff", Range(0, 1.1)) = 1.1
	_MainTex ("Main Texture", 2D) = "white" {}
	_Mask ("Mask", 2D) = "white" {}
	_MaskPow("Mask pow", Float) = 1
	_AlphaPow("Alpha pow", Float) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off
	Offset -1, -1

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Mask;
			half4 _TintColor;
			half _Cutoff;
			half _MaskPow;
			half _AlphaPow;
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float3 uvMainTex : TEXCOORD0;
				float2 uvMask : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 normal : NORMAL;
				UNITY_FOG_COORDS(3)

			};
			
			float4 _MainTex_ST;
			float4 _Mask_ST;

			half4 tex2DTriplanar(sampler2D tex, float3 worldPos, float3 normal)
			{
				half2 yUV = worldPos.xz * _MainTex_ST.xy;
				half2 xUV = worldPos.zy * _MainTex_ST.xy;
				half2 zUV = worldPos.xy * _MainTex_ST.xy;

				half4 yDiff = tex2D(tex, yUV);
				half4 xDiff = tex2D(tex, xUV);
				half4 zDiff = tex2D(tex, zUV);

				half3 blendWeights = pow(abs(normal), 1);

				blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

				return xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;
			}


			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0));
				o.uvMainTex = mul(unity_Projector, v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				o.uvMask = TRANSFORM_TEX(o.uvMainTex.xyz, _Mask);

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half4 tex = tex2DTriplanar(_MainTex, i.worldPos / 10, i.normal);

				half mask = tex2D(_Mask, i.uvMask).a;
				mask = pow(mask, _MaskPow);
				
				half4 col = 2.0f * _TintColor * tex;

				UNITY_APPLY_FOG(i.fogCoord, col);
				
				half m = saturate(mask - _Cutoff);
				half alpha = saturate(tex.a * m * _TintColor.a * 2);

				half clampMutliplier = 1 - step(i.uvMainTex.x, 0);
				clampMutliplier *= 1 - step(1, i.uvMainTex.x);
				clampMutliplier *= 1 - step(i.uvMainTex.y, 0);
				clampMutliplier *= 1 - step(1, i.uvMainTex.y);
				float projectedCordZ = i.uvMainTex.z;
				clampMutliplier *= step(projectedCordZ, 1);
				clampMutliplier *= step(-1, projectedCordZ);
				col.a = alpha * clampMutliplier;
				col.rgb = col.rgb + saturate(pow(dot(col.rgb, 0.3), 10)/2)*2;
				//clip(col.a - 0.02);
				//return float4(lerp(col.rgb * pow(alpha, _AlphaPow), 1, (1-col.a)), 1);
				return float4(col.rgb * pow(alpha, _AlphaPow), col.a);
			}
			ENDCG 
		}
	}	
}
}
