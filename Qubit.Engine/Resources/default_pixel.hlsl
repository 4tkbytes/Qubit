struct vs_out
{
    float4 position_clip : SV_POSITION;
};

float4 ps_main(vs_out input) : SV_TARGET
{
    return float4(1.0, 0.5, 0.2, 1.0);
}