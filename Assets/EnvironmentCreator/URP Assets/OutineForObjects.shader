Shader "Custom/SmoothOutlineShader"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness("Outline Thickness", Float) = 1.0
    }
        SubShader
        {
            Tags { "Queue" = "Overlay" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            Lighting Off
            ZWrite Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 texcoord : TEXCOORD0;
                    float4 color : COLOR;
                    float2 screenPos : TEXCOORD1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _OutlineColor;
                float _OutlineThickness;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.color = v.color;
                    o.screenPos = ComputeScreenPos(o.vertex);
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    float2 outlineOffset = _OutlineThickness / _ScreenParams.xy;
                    half4 color = tex2D(_MainTex, i.texcoord) * i.color;
                    float alpha = color.a;

                    // Increase the sample count for a smoother outline
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            float2 offset = float2(x, y) * outlineOffset;
                            half4 sample = tex2D(_MainTex, i.texcoord + offset);
                            alpha = max(alpha, sample.a);
                        }
                    }

                    half4 outlineColor = _OutlineColor * alpha;
                    half4 finalColor = lerp(color, outlineColor, alpha);

                    return finalColor;
                }
                ENDCG
            }
        }
}

