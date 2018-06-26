Shader "HyperCard/Card (Back)" 
{
	Properties
	{
		[HideInInspector] _CardFrame("_BackTexture", 2D) = "black" {}
		[HideInInspector] _CardAlpha("_CardAlpha", 2D) = "black" {}
		[HideInInspector] _Stencil("Stencil ID", Int) = 0
	}

    SubShader 
	{
        Tags 
		{
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

		Pass
		{
			Stencil
			{
				Ref[_Stencil]
				Comp notequal
				Pass keep
			}

			Name "Outline"

			Lighting Off
			Fog{ Mode Off }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "noiseSimplex.cginc"
			#include "UnityCG.cginc"

			uniform float4 _TimeEditor;

			uniform float _OutlineNoiseFreq;
			uniform float _OutlineNoiseSpeed;
			uniform float _OutlineNoiseMult;
			uniform float _OutlineNoiseDistance;
			uniform float _OutlineNoiseOffset;
			uniform float _OutlineNoiseThreshold;
			uniform float _OutlineNoiseVerticalAjust;
			uniform float _OutlineWidth;
			uniform float _OutlineHeight;
			uniform float _OutlineSmooth;
			uniform float _OutlineSmoothSpeed;
			uniform float _OutlineAlphaMult;
			uniform float _OutlineTrimOffset;
			uniform float4 _OutlineColor;
			uniform float4 _OutlineEndColor;
			uniform float _OutlineEndColorDistance;
			uniform float2 _OutlinePosOffset;
			uniform float _CardOpacity;

			uniform float _CanvasMode;
			uniform float _CanvasOffsetX;
			uniform float _CanvasOffsetY;

			uniform int _ShowOutline;

			uniform float _Seed;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
			};

			struct VertexOutput {
				float4 vertex : SV_POSITION;
				float2 uv0 : TEXCOORD0;
			};

			VertexOutput vert(VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;

				if (_CanvasMode == 0)
				{
					float4 w = v.vertex;
					w.x += normalize(v.vertex).x * _OutlineWidth + _OutlinePosOffset.x;
					w.y += normalize(v.vertex).y * _OutlineHeight + _OutlinePosOffset.y;

					o.vertex = UnityObjectToClipPos(w - float4(0, 0, 0.0001, 0));
				}
				else
				{
					v.vertex.x *= _OutlineWidth * 10;
					v.vertex.y *= _OutlineHeight * 10;
					v.vertex.x += _CanvasOffsetX;
					v.vertex.y += _CanvasOffsetY;
					v.vertex.z -= 0.0001;

					o.vertex = UnityObjectToClipPos(v.vertex);
				}

				return o;
			}

			float4 frag(VertexOutput i) : SV_TARGET
			{
				float4 time = _Time + _TimeEditor;
				float4 col = float4(1, 1, 1, 1.0f);

				if (_ShowOutline == 1)
				{

					if (i.uv0.y < 0.01) discard;

					float smc = 1 - smoothstep(0.0, _OutlineEndColorDistance, i.uv0.x) * (1 - smoothstep(1 - _OutlineEndColorDistance, 1.0, i.uv0.x)) * smoothstep(0.0, _OutlineEndColorDistance, i.uv0.y) * (1 - smoothstep(1 - _OutlineEndColorDistance, 1.0, i.uv0.y));

					col.xyz = lerp(_OutlineColor.xyz, _OutlineEndColor.xyz, smc) * (_OutlineAlphaMult * pow(_CardOpacity, 6));

					float trim = 1 - smoothstep(0.0, _OutlineTrimOffset, i.uv0.x) * (1 - smoothstep(1 - _OutlineTrimOffset, 1.0, i.uv0.x)) * smoothstep(0.0, _OutlineTrimOffset, i.uv0.y) * (1 - smoothstep(1 - _OutlineTrimOffset, 1.0, i.uv0.y));

					col.a = lerp(0, 1, trim);

					float sm = 1 - smoothstep(0.0, _OutlineSmooth, i.uv0.x) * (1 - smoothstep(1 - _OutlineSmooth, 1.0, i.uv0.x)) * smoothstep(0.0, _OutlineSmooth, i.uv0.y) * (1 - smoothstep(1 - _OutlineSmooth, 1.0, i.uv0.y));

					col.a = max(0, col.a - sm * _OutlineSmoothSpeed);

					float2 c = i.uv0 * _OutlineNoiseFreq;

					c.y += _Time.y * _OutlineNoiseSpeed + _Seed;

					float ns = snoise(c + i.uv0) * _OutlineNoiseMult + _OutlineNoiseOffset;

					ns = lerp(0, _OutlineNoiseThreshold, ns);

					ns *= pow(i.uv0.y, _OutlineNoiseVerticalAjust);

					float noise = 1 - smoothstep(0.0, _OutlineNoiseDistance, i.uv0.x) * (1 - smoothstep(1 - _OutlineNoiseDistance, 1.0, i.uv0.x)) * smoothstep(0.0, _OutlineNoiseDistance, i.uv0.y) * (1 - smoothstep(1 - _OutlineNoiseDistance, 1.0, i.uv0.y));

					col *= lerp(1, ns, noise);
				}
				else
				{
					discard;
				}

				col.a *= _CardOpacity;

				return col;
			}

			ENDCG
		}

        Pass 
		{
			Stencil {
                Ref [_Stencil]
                Comp always
                Pass replace
            }

            Blend SrcAlpha OneMinusSrcAlpha
            //ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _BackTexture;
			uniform sampler2D _CardAlpha;

            struct VertexInput 
			{
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput 
			{
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };

            VertexOutput vert (VertexInput v) 
			{
                VertexOutput o = (VertexOutput) 0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(VertexOutput i) : COLOR 
			{
                float4 _BackTexture_var = tex2D(_BackTexture, i.uv0);
				float4 alpha_col = tex2D(_CardAlpha, i.uv0);

				if(alpha_col.a < 0.1) {
					clip(-1);
				}

                return float4(_BackTexture_var.rgb, alpha_col.a);
            }

            ENDCG
        }
    }
}
