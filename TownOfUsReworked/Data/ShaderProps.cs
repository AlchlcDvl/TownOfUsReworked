namespace TownOfUsReworked.Data;

public static class ShaderProps
{
    public static readonly int Outline = Shader.PropertyToID("_Outline");
    public static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    public static readonly int AddColor = Shader.PropertyToID("_AddColor");
    public static readonly int Mask = Shader.PropertyToID("_Mask");
    public static readonly int StencilComp = Shader.PropertyToID("_StencilComp");
    public static readonly int Stencil = Shader.PropertyToID("_Stencil");
    public static readonly int FaceDilate = Shader.PropertyToID("_FaceDilate");
    public static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
    public static readonly int Color1 = Shader.PropertyToID("_Color");
}