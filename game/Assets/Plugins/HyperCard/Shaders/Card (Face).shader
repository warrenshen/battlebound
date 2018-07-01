Shader "HyperCard/Card (Face)"
{
 Properties
 {
     _CardForeground("_CardForeground", 2D) = "black" {}
     _CardBackground("_CardBackground", 2D) = "black" {}
     [HideInInspector] _CardFrame("_CardFrame", 2D) = "black" {}
     [HideInInspector] _CardFrameColor("_CardFrameColor", Color) = (1,1,1,1)
     [HideInInspector] _CardMask("_CardMask", 2D) = "black" {}
     [HideInInspector] _CardAlpha("_CardAlpha", 2D) = "black" {}
     [HideInInspector] _PriceTexture("_PriceTexture", 2D) = "white" {}
     [HideInInspector] _PriceRotSpeed("_PriceRotSpeed", Float ) = 1
     [HideInInspector] _PriceColor("_PriceColor", 2D) = "white" {}
     [HideInInspector] _PriceAlphaMult("_PriceAlphaMult", Float ) = 1
     [HideInInspector] _GemColor("_GemColor", Color) = (0,0,0,0)
     [HideInInspector] _Dist0_Enabled("_Dist0_Enabled", Int) = 0
     [HideInInspector] _CardDist0Mask("Card Dist 0 Mask", 2D) = "black" {}
     [HideInInspector] _Dist0Freq_Red("Dist Freq", Float) = 100
     [HideInInspector] _Dist0Amp_Red("Amp. Mult", Float) = 1
     [HideInInspector] _Dist0Speed_Red("Dist Speed", Float) = 1
     [HideInInspector] _Dist0Pos_Red("Dist strength (x,y)", Vector) = (0,0,0,0)
     [HideInInspector] _Dist0Freq_Green("Dist Freq", Float) = 100
     [HideInInspector] _Dist0Amp_Green("Amp. Mult", Float) = 1
     [HideInInspector] _Dist0Speed_Green("Dist Speed", Float) = 1
     [HideInInspector] _Dist0Pos_Green("Dist strength (x,y)", Vector) = (0,0,0,0)
     [HideInInspector] _Dist0Freq_Blue("Dist Freq", Float) = 100
     [HideInInspector] _Dist0Amp_Blue("Amp. Mult", Float) = 1
     [HideInInspector] _Dist0Speed_Blue("Dist Speed", Float) = 1
     [HideInInspector] _Dist0Pos_Blue("Dist strength (x,y)", Vector) = (0,0,0,0)
     [HideInInspector] _CardDist1Mask("Card Dist Mask", 2D) = "black" {}
     [HideInInspector] _CardDist1Tex("Card Dist Tex", 2D) = "black" {}
     [HideInInspector] _Dist1SpeedX("Dist Speed X", Float) = 0
     [HideInInspector] _Dist1SpeedY("Dist Speed Y", Float) = 0
     [HideInInspector] _Dist1AlphaMult("Dist Alpha", Float) = 0
     [HideInInspector] _Dist1Color("Dist Color", Color) = (0,0,0,0)
     [HideInInspector] _Dist1Freq("Dist Freq", Float) = 100
     [HideInInspector] _Dist1Amp("Dist Amplitude", Float) = 0.003
     [HideInInspector] _Dist1Speed("Dist Speed", Float) = 1
     [HideInInspector] _Dist1Pos("Dist Vector Pos (x,y)", Vector) = (0,0,0,0)
     [HideInInspector] _PeriodicalFxTex("_FXTex", 2D) = "black" {}
     [HideInInspector] _PeriodicalFxColor("_FXColor", Color) = (0,0,0,0)
     [HideInInspector] _PeriodicalFxAlpha("_FXAlpha", Float) = 1
     [HideInInspector] _PeriodicalFxMask("_FXMask", 2D) = "white" {}     
     [HideInInspector] _BlackAndWhite("_BlackAndWhite", Int) = 0
     [HideInInspector] _BurningMap("_BurningMap", 2D) = "black" {}
     [HideInInspector] _BurningTexture("_BurningTexture", 2D) = "black" {}
     [HideInInspector] _BurningAmount("_BurningAmount", Float) = 1
     [HideInInspector] _BurningRotateSpeed("_BurningRotateSpeed", Float) = 0
     [HideInInspector] _BurningRotateSpeed("_BurnAlphaCut", Float) = 0   
     [HideInInspector] _BurningOutline("_BurningOutline", Float) = 0
     [HideInInspector] _BurnColor("_BurnColor", Color) = (0,0,0,0)
     [HideInInspector] _SpriteSheet_Enabled("_SpriteSheet_Enabled", Int) = 0
     [HideInInspector] _SpriteSheetTex("_SpriteSheetTex", 2D) = "black" {}
     [HideInInspector] _SpriteSheetOffset("_SpriteSheetOffset", Vector) = (1,1,0,0)
     [HideInInspector] _SpriteSheetScale("_SpriteSheetScale", Vector) = (1,1,0,0)
     [HideInInspector] _SpriteSheetIndex("_SpriteSheetIndex", Int) = 0
     [HideInInspector] _SpriteSheetCols("_SpriteSheetCols", Float) = 1
     [HideInInspector] _SpriteSheetRows("_SpriteSheetRows", Float) = 1
     //Holo/Cubemap support removed by nick 6/30/18
     [HideInInspector] _MixTexture_Enabled("_MixTexture_Enabled", Int) = 0
     [HideInInspector] _OverlayTexture("_OverlayTexture", 2D) = "black" {}
     [HideInInspector] _Stencil("Stencil ID", Int) = 0
     [HideInInspector] _OutlineTrimOffset("_OutlineTrimOffset", Float) = 1
     [HideInInspector] _CardOpacity("_CardOpacity", Float) = 1
     [HideInInspector] _CanvasOffsetX("_CanvasOffsetX", Float) = 1
     [HideInInspector] _CanvasOffsetY("_CanvasOffsetY", Float) = 1
     [HideInInspector] _Seed("_Seed", Float) = 1
     [HideInInspector] _Glitter_Enabled("_Glitter_Enabled", Int) = 0
     [HideInInspector] _GlitterMap("_GlitterMap", 2D) = "white" {}
     [HideInInspector] _GlitterMask("_GlitterMask", 2D) = "black" {}
     [HideInInspector] _GlitterColor("_GlitterColor", Color) = (1,1,1,1)
     [HideInInspector] _GlitterPower("_GlitterPower", Float) = 2
     [HideInInspector] _GlitterContrast("_GlitterContrast", Float) = 1
     [HideInInspector] _GlitterSpeed("_GlitterSpeed", Float) = 1
     [HideInInspector] _GlitterSpecMap("_GlitterSpecMap", 2D) = "white" {}
     [HideInInspector] _GlitterSpecPower("_GlitterPower", Float) = 2
     [HideInInspector] _GlitterSpecContrast("_GlitterContrast", Float) = 1
 }

 SubShader
 {
     Tags
     {
         "IgnoreProjector" = "True"
         "Queue" = "Transparent"
         "RenderType" = "Transparent"
         "DisableBatching" = "True"
     }

     Pass
     {
         Stencil {
             Ref [_Stencil]
             Comp notequal
             Pass keep 
         }

         Name "Outline"
     
         Lighting Off
         Fog { Mode Off }
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
             VertexOutput o = (VertexOutput) 0;
             o.uv0 = v.texcoord0;
     
             if(_CanvasMode == 0)
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
             
                 if(i.uv0.y < 0.01) discard;

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
             Pass Replace
            } 

         Cull Back
         Lighting Off
         Fog { Mode Off }
         Blend SrcAlpha OneMinusSrcAlpha

         CGPROGRAM
         #pragma target 3.0
         #pragma vertex vert 
         #pragma fragment frag

         #include "noiseSimplex.cginc"
         #include "UnityCG.cginc"

         uniform float4 _TimeEditor;
         uniform sampler2D _CardMask;
         uniform sampler2D _CardAlpha;
         uniform sampler2D _CardFrame;
         uniform float4 _CardFrameColor;
         uniform sampler2D _CardForeground;
         uniform float4 _CardForeground_ST;
         uniform sampler2D _CardBackground;
         uniform float4 _CardBackground_ST;

         uniform int _Dist0_Enabled;
         uniform sampler2D _CardDist0Mask;

         uniform float _Dist0Freq_Red;
         uniform float _Dist0Amp_Red;
         uniform float _Dist0Speed_Red;
         uniform float4 _Dist0Pos_Red;

         uniform float _Dist0Freq_Green;
         uniform float _Dist0Amp_Green;
         uniform float _Dist0Speed_Green;
         uniform float4 _Dist0Pos_Green;

         uniform float _Dist0Freq_Blue;
         uniform float _Dist0Amp_Blue;
         uniform float _Dist0Speed_Blue;
         uniform float4 _Dist0Pos_Blue;

         uniform sampler2D _CardDist1Mask;
         uniform sampler2D _CardDist1Tex;

         uniform float4 _CardDist1Tex_ST;

         uniform float _Dist1SpeedX;
         uniform float _Dist1SpeedY;
         uniform float _Dist1AlphaMult;
         uniform float4 _Dist1Color;
         uniform float _Dist1Freq;
         uniform float _Dist1Amp;
         uniform float _Dist1Speed;
         uniform float4 _Dist1Pos;

         uniform sampler2D _PeriodicalFxTex;
         uniform sampler2D _PeriodicalFxMask;
         uniform float _PeriodicalFxAlpha;
         uniform float4 _PeriodicalFxColor;

         uniform int _BlackAndWhite;

         uniform float _BurnNoiseFreq;
         uniform float _BurningAmount;
         uniform float _BurningRotateSpeed;
         uniform float _BurningOutline;
         uniform float4 _BurnColor;
         uniform float4 _BurnEndColor;
         uniform float _BurnAlphaCut;
         uniform float _BurnExposure;

         uniform int _SpriteSheet_Enabled;
         uniform sampler2D _SpriteSheetTex;
         uniform float4 _SpriteSheetTex_ST;
         uniform float2 _SpriteSheetOffset;
         uniform float2 _SpriteSheetScale;

         uniform float _SpriteSheetIndex;
         uniform float _SpriteSheetCols;
         uniform float _SpriteSheetRows;

         /*
         uniform int _Holo_Enabled;
         uniform sampler2D _HoloMask; 
         uniform sampler2D _HoloMap;
         uniform float2 _HoloMap_Scale;
         uniform samplerCUBE _HoloCube; 
         uniform float4 _HoloCubeColor;
         uniform float _HoloCubeRotation;
         uniform float _HoloPower;
         uniform float _HoloAlpha;
         uniform float3 _HoloBBoxMin;
            uniform float3 _HoloBBoxMax;
            uniform float3 _HoloEnviCubeMapPos;
         uniform int _Holo_Debug;*/

         uniform int _Glitter_Enabled;
         uniform sampler2D _GlitterMask; 
         uniform float4 _GlitterMask_ST;
         uniform sampler2D _GlitterMap; 
         uniform float4 _GlitterMap_ST;
         uniform float4 _GlitterColor;
         uniform float _GlitterPower;
         uniform float _GlitterContrast;
         uniform float _GlitterSpeed;
         uniform sampler2D _GlitterSpecMap;
         uniform float _GlitterSpecPower;
         uniform float _GlitterSpecContrast;

         uniform int _MixTexture_Enabled;
     
         uniform float _MixNoiseFreq;
         uniform float _MixNoiseSpeed;
         uniform float _MixNoiseMult;
         uniform float _MixNoiseOffset;
         uniform float _MixNoiseThreshold;
         uniform float2 _MixNoiseMoveDir;

         uniform sampler2D _MixNoiseTextureMask;
         uniform float4 _MixNoiseTextureMask_ST;

         uniform float4 _MixNoiseStartColor, _MixNoiseEndColor;
         uniform float _MixNoiseThresholdInvert;
         uniform float _MixNoiseAlpha;

         uniform float _MixNoiseDistFreq;
         uniform float _MixNoiseDistAmp;
         uniform float2 _MixNoiseDistSpeed;
         uniform float2 _MixNoiseDistDir;
         uniform float _MixNoiseColorExposure;

         uniform float _CardOpacity;
         
         uniform float _Seed;

         struct appdata_t
         {
             float4 vertex : POSITION;
             float2 texcoord : TEXCOORD0;
             float3 normal : NORMAL;
             float4 tangent : TANGENT;
         };

         struct v2f
         {
             float4 vertex : SV_POSITION;
             float2 texcoord : TEXCOORD0;
             float3 vertexInWorld : TEXCOORD1;
                float3 viewDirInWorld : TEXCOORD2;
                float3 normalInWorld : TEXCOORD3;

             float3 tangentDir : TEXCOORD4;
             float3 bitangentDir : TEXCOORD5;
         };

         v2f vert(appdata_t v)
         {
             v2f o;
             o.vertex = UnityObjectToClipPos(v.vertex);
             o.texcoord = v.texcoord;
 
             // Transform vertex coordinates from local to world.
             float4 vertexWorld = mul(unity_ObjectToWorld, v.vertex);

             // Transform normal to world coordinates.
             float4 normalWorld = mul(float4(v.normal, 0.0), unity_WorldToObject);

             o.vertexInWorld = vertexWorld.xyz;
             o.viewDirInWorld = vertexWorld.xyz - _WorldSpaceCameraPos;
             o.normalInWorld = normalWorld.xyz;


             o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
             o.bitangentDir = normalize(cross(o.normalInWorld, o.tangentDir) * v.tangent.w);

             return o;
         }

         float3 LocalCorrect(float3 origVec, float3 bboxMin, float3 bboxMax, float3 vertexPos, float3 cubemapPos)
         {
             float3 invOrigVec = float3(1.0,1.0,1.0)/origVec;
             float3 intersecAtMaxPlane = (bboxMax - vertexPos) * invOrigVec;
             float3 intersecAtMinPlane = (bboxMin - vertexPos) * invOrigVec;
             float3 largestIntersec = max(intersecAtMaxPlane, intersecAtMinPlane);
             float Distance = min(min(largestIntersec.x, largestIntersec.y), largestIntersec.z);
             float3 IntersectPositionWS = vertexPos + origVec * Distance;
             float3 localCorrectedVec = IntersectPositionWS - cubemapPos;

             return localCorrectedVec;
         }

         float4 RotateAroundYInDegrees (float4 vertex, float degrees)
         {
             float alpha = degrees * UNITY_PI / 180.0;
             float sina, cosa;
             sincos(alpha, sina, cosa);
             float2x2 m = float2x2(cosa, -sina, sina, cosa);
             return float4(mul(m, vertex.xz), vertex.yw).xzyw;
         }

         float4 frag(v2f IN) : SV_TARGET
         {
             float4 time = _Time + _TimeEditor;

             float4 cardmask_col = tex2D(_CardMask, IN.texcoord);
             float4 cardframe_col = tex2D(_CardFrame, IN.texcoord) * _CardFrameColor;

             float4 alpha_col = tex2D(_CardAlpha, IN.texcoord);
         
             if(alpha_col.a < 0.1) 
             {
                 clip(-1);
             }

             /* ---------------------------------------------------------------------------------------*/
             /* DISTORTION */
             /* ---------------------------------------------------------------------------------------*/
             float2 uv = TRANSFORM_TEX(IN.texcoord, _CardForeground);
             float2 uv_bg = TRANSFORM_TEX(IN.texcoord, _CardBackground);

             float4 cardpicture_col;
             float4 fore;
             float4 back;

             if (_Dist0_Enabled == 1) 
             {
                 // red
                 float x_strength_red = uv.x * _Dist0Freq_Red * _Dist0Pos_Red.x;
                 float y_strength_red = uv.y * _Dist0Freq_Red * _Dist0Pos_Red.y;

                 float dist0_map_red = tex2D(_CardDist0Mask, uv).r;
                 float dist0_red = sin(x_strength_red + y_strength_red + _Dist0Speed_Red * time.g) * dist0_map_red * _Dist0Amp_Red / 10;

                 float dist0_red_c = dist0_red * dist0_map_red;

                 // green
                 float x_strength_green = uv.x * _Dist0Freq_Green * _Dist0Pos_Green.x;
                 float y_strength_green = uv.y * _Dist0Freq_Green * _Dist0Pos_Green.y;

                 float dist0_map_green = tex2D(_CardDist0Mask, uv).g;
                 float dist0_green = sin(x_strength_green + y_strength_green + _Dist0Speed_Green * time.g) * dist0_map_green * _Dist0Amp_Green / 10;

                 float dist0_green_c = dist0_green * dist0_map_green;

                 // blue
                 float x_strength_blue = uv.x * _Dist0Freq_Blue * _Dist0Pos_Blue.x;
                 float y_strength_blue = uv.y * _Dist0Freq_Blue * _Dist0Pos_Blue.y;

                 float dist0_map_blue = tex2D(_CardDist0Mask, uv).b;
                 float dist0_blue = sin(x_strength_blue + y_strength_blue + _Dist0Speed_Blue * time.g) * dist0_map_blue * _Dist0Amp_Blue / 10;

                 float dist0_blue_c = dist0_blue * dist0_map_blue;

                 fore = tex2D(_CardForeground, float2(uv.x + dist0_red_c + dist0_green_c + dist0_blue_c,
                     uv.y + dist0_red_c + dist0_green_c + dist0_blue_c));
                 back = tex2D(_CardBackground, float2(uv_bg.x + dist0_red_c + dist0_green_c + dist0_blue_c,
                     uv_bg.y + dist0_red_c + dist0_green_c + dist0_blue_c));
             }
             else 
             {
                 fore = tex2D(_CardForeground, float2(uv.x, uv.y));
                 back = tex2D(_CardBackground, float2(uv_bg.x, uv_bg.y));
             }
             cardpicture_col = lerp(back, fore, fore.a);
             
             /* ---------------------------------------------------------------------------------------*/

             // Dist 1
             float dist1 = sin((uv.x * _Dist1Pos.x) * _Dist1Freq + (uv.y * _Dist1Pos.y) * _Dist1Freq + _Dist1Speed * time.g) * _Dist1Amp;

             float2 dist1_coord = float2(IN.texcoord.x + _Dist1SpeedX * time.g, IN.texcoord.y + _Dist1SpeedY * time.g);

             float4 dist1_col = tex2D(_CardDist1Tex, float2(dist1_coord.x + dist1, dist1_coord.y + dist1));
             float4 dist1_alpha = tex2D(_CardDist1Mask, uv);

             float4 dist1_full = (float4(dist1_col.r, dist1_col.g, dist1_col.b, dist1_alpha.r) * _Dist1Color) * dist1_alpha.r * _Dist1AlphaMult;

             cardpicture_col += dist1_full;

             // Periodical
             float4 fx_col = tex2D(_PeriodicalFxTex, uv);
             float4 fx_col_mask = tex2D(_PeriodicalFxMask, uv);
             
             fx_col = float4(fx_col.r, fx_col.g, fx_col.b, fx_col_mask.r) * fx_col_mask.r;       
             fx_col *= _PeriodicalFxColor;
             fx_col = float4(fx_col.r, fx_col.g, fx_col.b, _PeriodicalFxAlpha) * _PeriodicalFxAlpha;

             cardpicture_col += fx_col;

             // Holo (removed by nick 6/30/18)

             // Blend
             float3 col = lerp(cardframe_col.rgb, cardpicture_col.rgb, cardmask_col.rgb.g);
             
         
             // ...

             float4 finalColor = float4(col.rgb, alpha_col.a);

             // Mix/Blend layer
             if (_MixTexture_Enabled) 
             {
                 float2 c = IN.texcoord * _MixNoiseFreq;
                 c.x += _Time.y * _MixNoiseMoveDir.x + _Seed;
                 c.y += _Time.y * _MixNoiseMoveDir.y + _Seed;

                 // ----
                 float2 mix_noise_uv = TRANSFORM_TEX(IN.texcoord, _MixNoiseTextureMask);

                 float mix_noise_dist = sin((mix_noise_uv.x * _MixNoiseDistDir.x) * _MixNoiseDistFreq + (mix_noise_uv.y * _MixNoiseDistDir.y) * _MixNoiseDistFreq + _MixNoiseDistSpeed * time.g) * _MixNoiseDistAmp;
                 float2 mix_noise_coords = float2(IN.texcoord.x + _MixNoiseDistSpeed.x * time.g, IN.texcoord.y + _MixNoiseDistSpeed.y * time.g);

                 float4 mix_noise_mask = tex2D(_MixNoiseTextureMask, float2(mix_noise_coords.x + mix_noise_dist, mix_noise_coords.y + mix_noise_dist));

                 //float4 mix_noise_mask = tex2D(_MixNoiseTextureMask, TRANSFORM_TEX(IN.texcoord, _MixNoiseTextureMask));

                 float ns = snoise(c + IN.texcoord) * _MixNoiseMult + _MixNoiseOffset;

                 float trim = smoothstep(0.0, _MixNoiseThreshold, IN.texcoord.x) * (1 - smoothstep(1 - _MixNoiseThreshold, 1.0, IN.texcoord.x)) * smoothstep(0.0, _MixNoiseThreshold, IN.texcoord.y) * (1 - smoothstep(1 - _MixNoiseThreshold, 1.0, IN.texcoord.y));

                 if(_MixNoiseThresholdInvert == 1)
                 {
                     trim = 1 - trim;
                 }
                 
                 float4 nc = lerp(_MixNoiseStartColor, _MixNoiseEndColor, ns);

                    nc *= _MixNoiseColorExposure;

                 finalColor.rgb = lerp(finalColor.rgb, nc, _MixNoiseAlpha * mix_noise_mask * trim);
             }

             // glitter fx
             if (_Glitter_Enabled == 1)
             {
                 float3x3 tangentTransform = float3x3(IN.tangentDir, IN.bitangentDir, IN.normalInWorld);
                 float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - IN.vertexInWorld.xyz);

                 float2 uv_glitter1 = (1 + _GlitterSpeed/2) * (0.25 * _GlitterSpeed * mul(tangentTransform, viewDirection).xy + IN.texcoord).rg;
                 float4 glitter_col1 = tex2D(_GlitterMap, TRANSFORM_TEX(uv_glitter1, _GlitterMap));
                 glitter_col1 *= _GlitterColor;

                 float2 uv_glitter2 = (-0.25 * _GlitterSpeed *  mul(tangentTransform, viewDirection).xy + IN.texcoord).rg;

                 float4 glitter_col2 = tex2D(_GlitterMap, TRANSFORM_TEX(uv_glitter2, _GlitterMap));

                 float4 glitter_mask_col = tex2D(_GlitterMask, TRANSFORM_TEX(IN.texcoord, _GlitterMask));

                 float3 glitter_fcolor = lerp(float3(0, 0, 0), _GlitterPower * pow(glitter_col1, 1 / _GlitterContrast), glitter_col2.rgb);

                 float4 glitter_spec = tex2D(_GlitterSpecMap, TRANSFORM_TEX(uv_glitter2, _GlitterMap));
                 float3 glitter_spec_color = lerp(float3(0, 0, 0), _GlitterSpecPower * pow(glitter_spec, 1 / _GlitterSpecContrast), glitter_mask_col);

                 glitter_fcolor *= glitter_mask_col;

                 finalColor.rgb += glitter_fcolor + glitter_spec_color;

                 //finalColor.rbg = glitter_fcolor.rgb;
             }

 
             // dissolve Fx
             float cosburn = cos(0.5 * time.g * _BurningRotateSpeed);
             float sinburn = sin(0.5* time.g * _BurningRotateSpeed);
             float2 pivotburn = float2(0.5, 0.5);
             float2 burn_uv = (mul(IN.texcoord -pivotburn, float2x2(cosburn, -sinburn, sinburn, cosburn)) + pivotburn);          

             if(_BurningAmount > 0)
             {
                 float burn_map = snoise(burn_uv * _BurnNoiseFreq + _Seed) * 0.5 + 0.5;          
             
                 float smo = saturate((_BurningAmount * (1 + _BurningOutline) - burn_map) / _BurningOutline);

                 float smb = smo - smoothstep(0.0, smo, IN.texcoord.x) * (1 - smoothstep(1 - smo, 1.0, IN.texcoord.x)) * smoothstep(0.0, smo, IN.texcoord.y) * (1 - smoothstep(1 - smo, 1.0, IN.texcoord.y));

                 float burnBorder = lerp(0, 1, 1 - smb);

                 float alphaBurn = min(alpha_col.a, burnBorder);         
     
                 finalColor = float4(lerp(col.rgb, col.rgb * (lerp(_BurnColor, _BurnEndColor, smo) * _BurnExposure * burnBorder), smo), alphaBurn);
                         
                 if(alphaBurn < _BurnAlphaCut) 
                 {
                     clip(-1);
                 }
             }

             // Materialization

             if (_BlackAndWhite == 1) 
             {
                 half c = (finalColor.r + finalColor.g + finalColor.b) / 3;
                 finalColor = fixed4(c,c,c, alpha_col.a);
             };

             finalColor.a *= _CardOpacity;

             return finalColor;
         }
         
         ENDCG
     }
     
     Pass
     {
         Stencil {
             Ref [_Stencil]
             Comp equal
             Pass keep 
         }

         Blend SrcAlpha OneMinusSrcAlpha 

         CGPROGRAM
         #pragma vertex vert 
         #pragma fragment frag

         #include "UnityCG.cginc"

         uniform sampler2D _CardMask;

         uniform int _SpriteSheet_Enabled;
         uniform sampler2D _SpriteSheetTex;
         uniform float4 _SpriteSheetTex_ST;
         uniform float2 _SpriteSheetOffset;
         uniform float2 _SpriteSheetScale;

         uniform float _SpriteSheetIndex;
         uniform float _SpriteSheetCols;
         uniform float _SpriteSheetRows;
         uniform float4 _SpriteSheetColor;
         uniform int _SpriteSheet_RmvBlackBg;

         uniform int _BlackAndWhite;

         uniform float _CardOpacity;

         uniform sampler2D _CardAlpha;

         struct appdata_t
         {
             float4 vertex : POSITION;
             float2 texcoord0 : TEXCOORD0;
             float2 texcoord : TEXCOORD1;
         };

         struct v2f
         {
             float4 vertex : SV_POSITION;
             float texcoord0 : TEXCOORD0;
             float2 texcoord : TEXCOORD1;
         };

         v2f vert(appdata_t v)
         {
             v2f o;
             o.vertex = UnityObjectToClipPos(v.vertex);
             o.texcoord0 = v.texcoord0;
             o.texcoord = (v.texcoord - _SpriteSheetOffset) * _SpriteSheetScale;

             return o;
         }

         float4 frag(v2f IN) : SV_TARGET
         {
             // Sprite Sheet
             if (_SpriteSheet_Enabled == 1)
             {
                 float ss_row = floor((_SpriteSheetIndex / _SpriteSheetCols)); // Row
                 float2 ss_v = float2((_SpriteSheetIndex - (_SpriteSheetCols * ss_row)), ss_row).gr;
                 float3 ss_t = ((float3(ss_v.g, (abs((1.0 - _SpriteSheetRows)).rr - ss_v)) + float3(IN.texcoord, 0.0)) * float3((float2(1, 1) / float2(_SpriteSheetCols, _SpriteSheetRows)), 0.0));

                 float4 ss_col = tex2D(_SpriteSheetTex, TRANSFORM_TEX(ss_t, _SpriteSheetTex));

                 if (_SpriteSheet_RmvBlackBg == 1) 
                 {
                     float bg = (ss_col.r + ss_col.g + ss_col.b) / 3.0;

                     ss_col = float4(ss_col.rgb, bg);
                 }

                 // x
                 if (((IN.texcoord.r / _SpriteSheetScale.r) > (1 / _SpriteSheetScale.r)) || ((IN.texcoord.r / _SpriteSheetScale.r) < 0))
                 {
                     discard;
                 }

                 // y
                 if (((IN.texcoord.g / _SpriteSheetScale.g) > (1 / _SpriteSheetScale.g)) || ((IN.texcoord.g / _SpriteSheetScale.g) < 0))
                 {
                     discard;
                 }

                 if (ss_col.a < 0.1) discard;

                                     
                 if (_BlackAndWhite == 1) {
                     ss_col = (ss_col.r + ss_col.g + ss_col.b) / 3;
                 };


                 ss_col *= _SpriteSheetColor;

                 float2 card_mask =  tex2D(_CardMask, (IN.texcoord / _SpriteSheetScale) + _SpriteSheetOffset);

                 ss_col.a *= card_mask.g;

                 ss_col.a *= _CardOpacity;

                 if (ss_col.a < 0.1) discard;

                 return ss_col;
             }

             return 0;
         }

         ENDCG
     }
 }
}