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
