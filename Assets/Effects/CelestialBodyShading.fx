sampler uImage0 : register(s0);

float2 uPosBody;
float2 uPosLight;
float uIntensity;
float uOffset;
 
float2 PolarToCartesian(float radius, float angle)
{
    float x = radius * cos(angle);
    float y = radius * sin(angle);
    return float2(x, y);
}

float4 CelestialBodyShading(float2 texCoord : TEXCOORD) : COLOR
{
    // Sample the texture
    float4 color = tex2D(uImage0, texCoord);

    float2 vec = uPosBody - uPosLight;
    float rot = atan2(vec.y, vec.x);

    float2 center = float2(0.5, 0.5) + PolarToCartesian(uOffset, rot);
    float2 d = uv - center;
    float dist = length(d);

    float3 newColor;

    float absIntensity = abs(uIntensity);
    float invIntensity = 1 - absIntensity;

    if (uIntensity < 0) 
    {
        newColor = color.rgb * clamp((smoothstep(0.25, 0.5, dist) + invIntensity), 0, 1);
    }
    else
    {
        newColor = color.rgb - color.rgb * smoothstep(0.25, 0.5, dist) * absIntensity;
    }
 
    return float4(newColor, color.a);
}

technique Technique1
{
    pass CelestialBodyShading
    {
        PixelShader = compile ps_2_0 CelestialBodyShading();
    }
}