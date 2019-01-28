float4x4  World;
float4x4  View;
float4x4  Projection;
texture MyTexture;

sampler mySampler = sampler_state {
	Texture = <MyTexture>;
};

struct VertexPositionTexture {
	float4 Position: POSITION;
	float2 TextureCoordinate : TEXCOORD;
};

VertexPositionTexture MyVertexShader(VertexPositionTexture input) {
	VertexPositionTexture output;
	float4 worldPos = mul(input.Position, World);
	float4 viewPos = mul(worldPos, View);
	output.Position = mul(viewPos, Projection);
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 MyPixelShader(VertexPositionTexture input) : COLOR
{
	return tex2D(mySampler, input.TextureCoordinate);
}

technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 MyVertexShader();
		PixelShader = compile ps_4_0 MyPixelShader();
	}
}