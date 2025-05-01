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
    
    // Adjust for DirectX coordinate system
    // DirectX has (0,0) at the top-left corner and y increases downward
    // To center the quad, we keep the positions as they are
    // The offset might be happening due to viewport settings
    float4 pos = float4(input.position, 1.0);
    
    // DirectX expects y to be flipped compared to OpenGL
    // This might fix your positioning issue
    //pos.y = -pos.y;
    
    output.position_clip = pos;
    
    // Pass the color to the pixel shader
    output.outColor = input.color;
    
    return output;
}

