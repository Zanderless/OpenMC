#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aUV;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec3 fNormal;
out vec3 fPos;
out vec3 fUV;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
    fPos = vec3(uModel * vec4(aPosition, 1.0));
    fNormal = mat3(transpose(inverse(uModel))) * aNormal;

    fUV = aUV;
}