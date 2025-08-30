sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;
    
float4 MorvoltWings(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Normalization
    float2 uv = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.wz;
    
    // Distance from the center
    float pct = distance(uv, float2(0.5, 0.5));
    
    float4 imageColor = tex2D(uImage0, coords);
    // Vary the color based on time
    // 2.85 is added to start with blue 
    float3 color = 0.75 + 0.75 * cos(2.85 + (uTime * 6.0) - pct + float3(0, 2, 4));
    
    return float4(color, imageColor.a) * imageColor.a;
}
    
technique Technique1
{
    pass MorvoltWings
    {
        PixelShader = compile ps_3_0 MorvoltWings();
    }
}