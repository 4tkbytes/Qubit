// Add back the constant buffer for transformations
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
    
    // Use identity matrix for projection to see if it works
    float4 pos = float4(input.position, 1.0);
    
    // Identity matrix multiplication (effectively no projection)
    output.position_clip = pos;
    
    // Pass the color through unchanged
    output.outColor = input.color;
    
    return output;
}



