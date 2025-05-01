struct vs_out
{
    float4 position_clip : SV_POSITION;
    float3 outColor : COLOR;
};

float4 ps_main(vs_out input) : SV_TARGET
{
    // Use the interpolated color from the vertex shader
    return float4(input.outColor, 1.0);
}
