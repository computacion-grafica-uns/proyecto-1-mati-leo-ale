Shader "ShaderBasico"{

	SubShader{

		Pass{
			Cull Off
			CGPROGRAM
			//decalramos que la funcion llamada "vert" definira el shader de vertices
			#pragma vertex vert
			//declaramos que la funcion llamada "frag" definira el shader de fragmentos
			#pragma fragment frag

			//datos de cada vertices
			struct appdata{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			//datos que pasaremos del shader de vertices al de fragmentos
			struct v2f{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				};

			//shader de vertices
			v2f vert(appdata v){
				v2f o;
				//veremos mas adelante que hace esta funcion
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				return o;
				}

			//shader de fragmentos
			fixed4 frag(v2f i) : SV_Target{
				//establecemos un color de salida
				return (i.color);
				}
			ENDCG
		}
	}
}