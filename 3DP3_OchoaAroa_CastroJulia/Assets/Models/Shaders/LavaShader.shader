Shader "Custom/LavaShader"
{
    Properties
    {
        _Color("TextureColor",Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white"
        _Scale("TextureScale",float) = 1
        _Offset("TextureOffset",float) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Scale;
            float _Offset;
            float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = (v.uv + _Time.x) * _Scale;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) + _Color;
                return col;
            }
            ENDCG
        }
    }
}
