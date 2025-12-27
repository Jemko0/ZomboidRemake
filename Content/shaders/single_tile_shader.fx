#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
texture TileTexture;

float2 AtlasSize = float2(64.0f, 96.0f);
float2 TileSize = float2(32.0f, 48.0f);

sampler TileSampler = sampler_state
{
    Texture = <TileTexture>;
    MinFilter = POINT;
    MagFilter = POINT;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float tileType = floor(input.Color.r * 255.0f - 0.5f);
    
    float tilesPerRow = AtlasSize.x / TileSize.x;
    
    float tileX = fmod(tileType, tilesPerRow);
    float tileY = floor(tileType / tilesPerRow);
    
    // uv offset
    float2 tileOffset = float2(tileX * TileSize.x, tileY * TileSize.y);
    
    // scale the input uv to the tile size and add offset
    float2 atlasUV = (input.TexCoord * TileSize + tileOffset) / AtlasSize;
    
    // sample the texture
    float4 texColor = tex2D(TileSampler, atlasUV);
    
    float4 finalColor = texColor;
    
    return finalColor;
}


technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
