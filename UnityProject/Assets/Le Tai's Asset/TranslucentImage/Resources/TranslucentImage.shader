Shader "UI/TranslucentImage"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _BlurTex("Blur Texture", 2D) = "gray" {}

        _Vibrancy("Vibrancy", Float) = 1
        _Brightness("Brightness", Float) = 0
        _Flatten("Flatten", Float) = 0

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"= "Transparent"
            "IgnoreProjector"= "True"
            "RenderType"= "Transparent"
            "PreviewType"= "Plane"
            "CanUseSpriteAtlas"= "True"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off	
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
    
                #include "UnityCG.cginc"
                #include "UnityUI.cginc"
    
                #pragma multi_compile __ UNITY_UI_CLIP_RECT
                #pragma multi_compile __ UNITY_UI_ALPHACLIP
    
                struct appdata
                {
                    half4 vertex    : POSITION;
                    half4 color     : COLOR;
                    half2 texcoord  : TEXCOORD0;
                    half2 extraData : TEXCOORD1;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };
    
                struct v2f
                {
                    half4 vertex    : SV_POSITION;
                    half4 color     : COLOR;
                    half2 texcoord  : TEXCOORD0;
                    half4 worldPosition : TEXCOORD1;
                    float4 blurTexcoord : TEXCOORD2;
                    half2 extraData : TEXCOORD3;
                    UNITY_VERTEX_OUTPUT_STEREO
                };
    
                fixed4 _TextureSampleAdd;
                half4  _ClipRect;
                //xMin, yMin, xMax, yMax
                half4 _CropRegion;
                
                half2 getCroppedCoord(half2 screenCoord)
                {
                    return (screenCoord - _CropRegion.xy)/(_CropRegion.zw - _CropRegion.xy);
                }
    
                v2f vert(appdata IN)
                {
                    v2f OUT;
                    
                    UNITY_SETUP_INSTANCE_ID(IN);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
    
                    OUT.worldPosition = IN.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
    
                    #ifdef UNITY_HALF_TEXEL_OFFSET
                        OUT.vertex.xy += (_ScreenParams.zw - 1.0)*half2(-1,1);
                    #endif
    
                    OUT.color = IN.color;
                    OUT.texcoord = IN.texcoord;			    
                    OUT.blurTexcoord = ComputeScreenPos(OUT.vertex);
                    OUT.blurTexcoord.xy = getCroppedCoord(OUT.blurTexcoord.xy);
                    OUT.extraData = IN.extraData;
    
                    return OUT;
                }
                sampler2D _MainTex;
                sampler2D _BlurTex;
                uniform half _Vibrancy;
                uniform half _Flatten;
                uniform half _Brightness;
    
                half4 frag(v2f IN) : SV_Target
                {
                    half4 foregroundColor = tex2D(_MainTex, IN.texcoord.xy) + _TextureSampleAdd;
                    
                    //Overlay
                    half4 color = foregroundColor * IN.color;
    
    
                    half3 backgroundColor = tex2Dproj(_BlurTex, IN.blurTexcoord).rgb;                    
                    
                    //saturate help keep color in range                    
                    //Exclusion blend
                    half4 fgScaled = lerp(0, foregroundColor, _Flatten);
                    backgroundColor = saturate(backgroundColor + fgScaled - 2 * fgScaled * backgroundColor);
                    
                    //Vibrancy
                    backgroundColor = saturate(lerp(Luminance(backgroundColor), backgroundColor, _Vibrancy));
    
                    //Brightness
                    backgroundColor = saturate(backgroundColor + _Brightness);
    
    
                    //Alpha blend with backgroundColor
                    color.rgb = lerp(backgroundColor, color.rgb, IN.extraData[0]);                    
    
                    
//                    #ifdef UNITY_UI_CLIP_RECT
                        color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
//                    #endif
                    
                    #ifdef UNITY_UI_ALPHACLIP
                        clip(color.a - 0.001);
                    #endif
    
                    return color;
                }
            ENDCG
        }
    }
}
