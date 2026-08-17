Shader "UI/NeonLineFlow_BuiltIn"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (0.8, 0.3, 1.0, 1.0) // 흐르는 빛 색상
        _Threshold ("Line Brightness Threshold", Range(0, 1)) = 0.35 // 네온선 감지 밝기 기준
        _FlowSpeed ("Flow Speed (X, Y)", Vector) = (0.5, 0.5, 0, 0) // 빛이 흐르는 속도
        _Frequency ("Wave Frequency", Float) = 15.0 // 빛 무늬의 촘촘함
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

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
                float2 texcoord  : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _GlowColor;
            float _Threshold;
            float4 _FlowSpeed;
            float _Frequency;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

                // 1. 픽셀의 밝기(휘도) 계산
                float luminance = dot(c.rgb, float3(0.299, 0.587, 0.114));

                // 2. 밝기가 설정한 Threshold 이상인 네온선 영역만 마스킹
                float lineMask = smoothstep(_Threshold - 0.05, _Threshold + 0.05, luminance);

                // 3. 시간(_Time.y) 흐름에 따라 움직이는 빛 파동 생성
                float2 movingUV = IN.texcoord + _Time.y * _FlowSpeed.xy;
                float wave = sin((movingUV.x + movingUV.y) * _Frequency) * 0.5 + 0.5;

                // 4. 네온선 영역에만 빛을 강하게 합성
                fixed3 glow = _GlowColor.rgb * (wave * wave) * lineMask * _GlowColor.a * 2.0;
                c.rgb += glow;

                return c;
            }
            ENDCG
        }
    }
}