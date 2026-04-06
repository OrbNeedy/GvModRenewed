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
    
float4 PetrificationColor(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Get pixel color
    float4 imageColor = tex2D(uImage0, coords);
    // Average rgb values to remove saturation
    float finalPixelColor = imageColor.r + imageColor.g + imageColor.b;
    finalPixelColor /= 3.0;
    
    // Multiply by alpha for transparency 
    return float4(float3(finalPixelColor, finalPixelColor, finalPixelColor) * sampleColor.rgb, imageColor.a) * imageColor.a * sampleColor.a;
}
    
technique Technique1
{
    pass PetrificationColor
    {
        PixelShader = compile ps_3_0 PetrificationColor();
    }
}