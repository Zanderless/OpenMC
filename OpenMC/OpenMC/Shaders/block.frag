#version 330 core

in vec3 fPos;
in vec3 fNormal;
in vec3 fUV;

uniform sampler2DArray uTexture;
uniform vec3 uLightColor;
uniform vec3 uLightPos;

uniform int uLayerCount;

out vec4 out_color;

void main()
{
	float ambientStrength = 0.5;
	vec3 ambient = ambientStrength * uLightColor;

	vec3 norm = normalize(fNormal);
	vec3 lightDirection = normalize(uLightPos - fPos);
	float diff = max(dot(norm, lightDirection), 0.0);
	vec3 diffuse = diff * uLightColor;

	vec3 result = (ambient + diffuse);

	float layer = max(0, min(uLayerCount - 1, floor(fUV.z + 0.5)));
	vec3 uv = fUV;
	uv.z = layer;

	out_color = texture(uTexture, uv) * vec4(result, 1.0);
}