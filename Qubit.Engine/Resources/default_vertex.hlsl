cbuffer TransformBuffer : register(b0)
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;
};

struct vs_in
{
    float3 position : POSITION;
    float3 color : COLOR;
};

struct vs_out
{
    float4 position_clip : SV_POSITION;
    float3 outColor : COLOR;
};

vs_out vs_main(vs_in input)
{
    vs_out output;
    
    // Apply transformations
    float4 worldPos = mul(float4(input.position, 1.0), model);
    float4 viewPos = mul(worldPos, view);
    output.position_clip = mul(viewPos, projection);
    
    // Pass the color to the pixel shader
    output.outColor = input.color;
    
    return output;
}
