﻿Shader "HyperCard/Sprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
		_Zoom("_Zoom", Float) = 1
		_DistortionMask("_DistortionMask", 2D) = "white" {}
		_DistortionFreq("_DistortionFreq", Float) = 0
		_DistortionAmp("_DistortionAmp", Float) = 0
		_DistortionSpeed("_DistortionSpeed", Float) = 0
		_DistortionDir("_DistortionDir", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }  

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Back
        Lighting Off
        ZWrite Off
        Fog { Mode Off }

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
            #include "UnityCG.cginc"

			uniform fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform float _Zoom;
			uniform float4 _TimeEditor;
			uniform sampler2D _DistortionMask;
			uniform float _DistortionFreq;
			uniform float _DistortionAmp;
			uniform float _DistortionSpeed;
			uniform float2 _DistortionDir;

			uniform int _BlackAndWhite;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };


            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
				float4 time = _Time + _TimeEditor;

				float xs = IN.texcoord.x * _DistortionFreq * _DistortionDir.x;
				float ys = IN.texcoord.y * _DistortionFreq * _DistortionDir.y;

				float uv = sin(xs + ys + _DistortionSpeed * time.g) * _DistortionAmp / 10;

				float4 c = tex2D(_MainTex, float2(IN.texcoord.x * _Zoom + uv, IN.texcoord.y * _Zoom + uv)) * IN.color;

				float4 maskTex = tex2D(_DistortionMask, IN.texcoord);

				c.a = c.a * IN.color.a * maskTex.r;

				if (_BlackAndWhite == 1)
				{
					half c2 = (c.r + c.g + c.b) / 3;
					c = fixed4(c2, c2, c2, c.a);
				};

                return c;
            }

			ENDCG
        }


    }
}