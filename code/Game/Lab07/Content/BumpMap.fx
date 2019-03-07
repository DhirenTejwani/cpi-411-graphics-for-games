float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float3 CameraPosition;
float3 LightPosition;

float AmbientColor;
float AmbientIntensity;
float4 DiffuseColor;
float DiffuseIntensity;
float4 SpecularColor;
float SpecularIntensity;
float Shininess;

texture normalMap;

sampler tsampler1 = sampler_state
{
	texture = <normalMap>;
	magfilter = LINEAR; // None, POINT, LINEAR, Anisotropic
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap; // Clamp, Mirror, MirrorOnce, Wrap, Border
	AddressV = Wrap;
};

struct VertexShaderInput
{
	float4 Position: SV_POSITION0;
	float4 Normal: NORMAL0;
	float2 TexCoord: TEXCOORD0;
	float4 Tangent : TANGENT0;
	float4 Binormal: BINORMAL0;
	
};

struct VertexShaderOutput
{
	float4 Position: POSITION0;
	float3 Normal: TEXCOORD0;
	float3 Tangent: TEXCOORD1;	
	float3 Binormal: TEXCOORD2;	
	float2 TexCoord : TEXCOORD3;
	float3 Position3D: TEXCOORD4;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position,World);
	float4 viewPosition = mul(worldPosition,View);
	float4 projPosition = mul(viewPosition,Projection);
	output.Position = projPosition;

	output.Normal = normalize(mul(input.Normal,WorldInverseTranspose).xyz);
	output.Tangent = normalize(mul(input.Tangent,WorldInverseTranspose).xyz);
	output.Binormal = normalize(mul(input.Binormal,WorldInverseTranspose).xyz);
	output.Position3D = worldPosition.xyz;
	output.TexCoord = input.TexCoord;

	return output;

}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 L = normalize(LightPosition - input.Position3D);
	float3 V = normalize(CameraPosition - input.Position3D);
	float3 H = normalize(L + V);
	float3 normalTex = (tex2D(tsampler1, input.TexCoord).xyz - float3(0.5,0.5,0.5)) * 2.0;
	float3 bumpNormal  = input.Normal + normalTex.x * input.Tangent + normalTex.y * input.Binormal;
	//return tex2D(tsampler1, input.TexCoord);
	float4 diffuse = max(0, dot(bumpNormal, L));
	float4 specular = pow(saturate(dot(H, bumpNormal)), Shininess);
	return diffuse + specular;
}

technique bump
{
	pass Pass1{
		VertexShader=compile vs_4_0 VertexShaderFunction();
		PixelShader=compile ps_4_0 PixelShaderFunction();
	}
}