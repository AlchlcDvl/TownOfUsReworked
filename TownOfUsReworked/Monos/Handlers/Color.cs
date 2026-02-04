namespace TownOfUsReworked.Monos;

public sealed class ColorHandler : MonoBehaviour
{
    private readonly RendHandler<int> IdToRendHandler = new(CustomColorManager.SetColor);
    private readonly RendHandler<UColor> ColorToRendHandler = new(CustomColorManager.SetColor);
    private readonly RendHandler<ColorPair> ColorPairToRendHandler = new(CustomColorManager.SetColor);

    public void SetRend(Renderer rend, int id) => IdToRendHandler.SetRend(rend, id);

    public void SetRend(Renderer rend, UColor color) => ColorToRendHandler.SetRend(rend, color);

    public void SetRend(ColorPair pair, Renderer rend) => ColorPairToRendHandler.SetRend(rend, pair);

    public void Update() => IdToRendHandler.SetColors();
}

public sealed class RendHandler<T>(Action<Renderer, T> setColor)
{
    private readonly List<(Renderer Rend, T)> Rends = [];
    private readonly Action<Renderer, T> SetColor = setColor;

    public void SetRend(Renderer rend, T color)
    {
        Rends.RemoveAll(x => x.Rend == rend || !x.Rend);
        Rends.Add((rend, color));
        SetColor(rend, color);
    }

    public void SetColors() => Rends.ForEach(SetColor);
}