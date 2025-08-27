Shader "UI/RoundedRectOuterBorder"
{
    Properties
    {
        [PerRendererData]_MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _Radius ("Corner Radius (px)", Float) = 24
        _BorderWidth ("Border Width (px)", Float) = 6
        _BorderColor ("Border Color", Color) = (0,0,0,1)

        _Stencil ("Stencil ID", Float) = 0
        _StencilComp ("Stencil Comp", Float) = 8
        _StencilOp ("Stencil Op", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags{ "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float _Radius;          
            float _BorderWidth;     
            float4 _BorderColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos    : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            float sdRoundedBox(float2 p, float2 b, float r)
            {
                float2 q = abs(p) - b + r;
                return length(max(q,0)) - r;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 p = i.uv - 0.5;

                float r  = _Radius      / 1024.0;
                float bw = _BorderWidth / 1024.0;

                float2 b = float2(0.5 - r, 0.5 - r);

                float dist = sdRoundedBox(p, b, r);

                float insideMask = step(dist, 0.0);         
                float outerBorderMask = saturate(smoothstep(0.0, 0.001, dist)   
                                   * (1.0 - smoothstep(bw, bw + 0.001, dist))); 

                fixed4 tex = tex2D(_MainTex, i.uv) * i.color;

                fixed4 col = fixed4(0,0,0,0);

                col.rgb += _BorderColor.rgb * outerBorderMask;
                col.a   += _BorderColor.a   * outerBorderMask;

                col.rgb = lerp(col.rgb, tex.rgb, insideMask);
                col.a   = max(col.a, tex.a * insideMask);

                clip(col.a - 0.001);
                return col;
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}