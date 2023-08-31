Shader "NamNH/Image"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
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

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "Default"

        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord  : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Offset;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                #if UNITY_UV_STARTS_AT_TOP
                    OUT.texcoord.y = 1-OUT.texcoord.y;
                #endif
                
                return OUT;
            }

            

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord * _Offset.zw + _Offset.xy;
                half4 color = tex2D(_MainTex, uv);

                clip (color.a - 0.001);

                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
}
