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
    
float4 Rebirth(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Normalization
    float2 uv = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.wz;
    
    // Offset that moves
    float2 offset = float2(0.175f * uTime, 0.1f * uTime);
    
    // Get colors
    float3 color = sampleColor;
    float4 imageColor = tex2D(uImage0, coords);
    float4 mask = tex2D(uImage1, offset);
    
    return float4(imageColor.rgb * mask.rgb * color, imageColor.a) * imageColor.a;
}
    
technique Technique1
{
    pass Rebirth
    {
        PixelShader = compile ps_3_0 Rebirth();
    }
}