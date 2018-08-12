Shader "HyperCard/Sprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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
            Ref 1
            Comp notequal
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
             float4 c = tex2D(_MainTex, float2(IN.texcoord.x, IN.texcoord.y)) * IN.color;
             c.a = c.a * IN.color.a;

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