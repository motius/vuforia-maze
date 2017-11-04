// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "Custom/NearFade" {
     Properties {
         _Color("Main Color", Color) = (1,1,1,1)
		 _NearFade("Near Fade", Range(0,500)) = 6
		 _FarFade("Far Fade", Range(0,500)) = 8
     }
 
     SubShader {
         Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
 
         /////////////////////////////////////////////////////////
         /// Second Pass
         /////////////////////////////////////////////////////////
 
         Pass {
             // Now render color channel
             Blend SrcAlpha OneMinusSrcAlpha
 
             CGPROGRAM
			 #pragma vertex vert
             #pragma fragment frag
			 #include "UnityCG.cginc"
			 #include "Lighting.cginc"
 
             sampler2D _MainTex;
			 float4 _MainTex_ST;
             fixed4 _Color;
			 float _NearFade;
			 float _FarFade;
 
             struct appdata {
                 float4 vertex : POSITION;
				 float3 normal : NORMAL;
                 float2 uv : TEXCOORD0;
             };
 
             struct v2f {
				float4 pos : SV_POSITION;
				float4 col : COLOR0;
                float2 uv : TEXCOORD0;
				float t : Float;
             };
 
             v2f vert(appdata v) {
                v2f o;

				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				// UnityCG.cginc includes UnityShaderVariables.cginc
				// This file defines _WorldSpaceLightPos0 and _World2Object
				// UNITY_LIGHTMODEL_AMBIENT is a built in variable, defined here
				// http://docs.unity3d.com/Documentation/Components/SL-BuiltinStateInPrograms.html
				// _LightColor0 is defined in Lighting.cginc
				float4x4 modelMatrixInverse = unity_WorldToObject;
				float3 normalDirection = normalize(float3(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz));
				float3 lightDirection = normalize(float3(_WorldSpaceLightPos0.xyz));
				float3 diffuseReflection = float3(_LightColor0.rgb) * max(0.0, dot(normalDirection.xyz, lightDirection));
				o.col = (float4(diffuseReflection, 1.0) + UNITY_LIGHTMODEL_AMBIENT) * _Color;

				// Calculate distance to camera
				float3 worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
				float dist = distance(worldPos, _WorldSpaceCameraPos);
				if(dist < _NearFade)
					o.t = 0;
				else if(dist > _FarFade)
					o.t = 1;
				else
					o.t = (dist - _NearFade) / (_FarFade - _NearFade);

				return o;
             }
 
             fixed4 frag(v2f i) : SV_Target{
				float4 color = (tex2D (_MainTex, i.uv) * i.col);
                return float4(color.rgb, color.a * i.t);
             }
             ENDCG
         }
     }
 
     Fallback "Diffuse"
 }