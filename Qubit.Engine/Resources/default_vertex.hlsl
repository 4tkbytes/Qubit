struct vs_in
{
    float3 position : POS;
};

struct vs_out
{
    float4 position_clip : SV_POSITION;
};

vs_out vs_main(vs_in input)
{
    vs_out output;
    output.position_clip = float4(input.position, 1.0);
    return output;
}