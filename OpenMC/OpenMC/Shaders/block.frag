#version 330 core

in vec3 fPos;
in vec3 fNormal;
in vec2 fUV;

uniform sampler2D uTexture;
uniform vec3 uLightColor;
uniform vec3 uLightPos;

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

	out_color = texture(uTexture, fUV) * vec4(result, 1.0);
}