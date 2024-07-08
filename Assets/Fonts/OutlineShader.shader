Shader "Custom/Outline"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _Outline("Outline width", Range(.002, 0.03)) = .005
    }
        SubShader
        {
            Tags {"Queue" = "Overlay" }
            LOD 100

            Pass
            {
                Name "BASE"
                Tags {"LightMode" = "Always"}
                Blend SrcAlpha OneMinusSrcAlpha
                Cull Off
                ZWrite On
                ZTest LEqual

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _OutlineColor;
                float _Outline;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }

                half4 frag(v2f i) : COLOR
                {
                    // sample the texture
                    half4 texcol = tex2D(_MainTex, i.uv);

                    // create outline color by comparing texture alpha
                    if (texcol.a < 0.1) discard; // only outline if alpha is significant

                    // sample the texture
                    half4 c;
                    c.rgb = texcol.rgb;
                    c.a = texcol.a;

                    // Apply outline
                    float2 outlineOffset = float2(_Outline, _Outline);
                    half4 outlineTexcol = tex2D(_MainTex, i.uv + outlineOffset);
                    if (outlineTexcol.a > 0.1)
                    {
                        c.rgb = lerp(_OutlineColor.rgb, texcol.rgb, texcol.a);
                        c.a = 1.0;
                    }

                    return c;
                }
                ENDCG
            }
        }
}
