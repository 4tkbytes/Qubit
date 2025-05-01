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
    
    float4 pos = float4(input.position, 1.0);

    output.position_clip = pos;
    
    // Pass the colour to the input
    output.outColor = input.color;
    
    return output;
}

