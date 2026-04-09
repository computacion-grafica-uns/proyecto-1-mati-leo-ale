Shader "MyShader" 
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            uniform float4x4 _ModelMatrix;
            //uniform float4x4 _ViewMatrix;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = mul(mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_V, _ModelMatrix)), v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color; 
            }
            ENDCG
        }
    }
}