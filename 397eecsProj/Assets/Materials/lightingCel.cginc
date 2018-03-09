#ifndef lightingCel
#define lightingCel

sampler2D _ramp;
float _maxLight;

half4 LightingCel(SurfaceOutput s, half3 lightDir, half atten) {
	half NdotL = dot (s.Normal, lightDir);
	NdotL = tex2D (_ramp, float2(NdotL, NdotL)).r;
	half4 col;
	col.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten)*_maxLight;
	col.a = s.Alpha;
	return col;
}

#endif

