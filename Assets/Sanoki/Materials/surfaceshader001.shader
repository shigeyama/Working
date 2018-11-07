Shader "Custom/surfaceshader001" {
	Properties{
		_MainTex("Base (RGB)",2D) = "white" {}
		_ScrollX("Scroll X", float) = 0
		_ScrollY("Scroll Y", float) = 0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf Lambert

			struct Input {
				float2 uv_MainTex;
			};

			sampler2D _MainTex;
			float _ScrollX, _ScrollY;

			void surf(Input IN, inout SurfaceOutput o) {
				float2 scroll = float2(_ScrollX, _ScrollY) * _Time.y;
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex + scroll);
			}
			ENDCG
		}
			FallBack "Diffuse"
}
