struct vs_out
{
    float4 position_clip : SV_POSITION;
    float3 outColor : COLOR;
};

float4 ps_main(vs_out input) : SV_TARGET
{
    // Check if color values are valid
    if (input.outColor.r < 0.01f && input.outColor.g < 0.01f && input.outColor.b < 0.01f)
    {
        // If all color components are near zero, show pink for debugging
        return float4(1.0, 0.0, 1.0, 1.0);
    }
    
    // Output the interpolated color
    return float4(input.outColor, 1.0);
}
