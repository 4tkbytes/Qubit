struct vs_out
{
    float4 position_clip : SV_POSITION;
    float3 outColor : COLOR;
};

float4 ps_main(vs_out input) : SV_TARGET
{
    // Always return red to debug
    return float4(1.0, 0.0, 0.0, 1.0);
}
