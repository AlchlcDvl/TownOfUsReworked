namespace TownOfUsReworked.Monos;

public class ColorHandler : MonoBehaviour
{
    public static ColorHandler Instance { get; private set; }
    public readonly List<(int, Renderer)> IDToRends = [];
    public readonly List<(UColor, Renderer)> ColorToRends = [];

    public ColorHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void SetRend(Renderer rend, int id)
    {
        IDToRends.RemoveAll(x => x.Item2 == rend);
        ColorToRends.RemoveAll(x => x.Item2 == rend);
        IDToRends.Add((id, rend));
    }

    public void SetRend(Renderer rend, UColor color)
    {
        IDToRends.RemoveAll(x => x.Item2 == rend);
        ColorToRends.RemoveAll(x => x.Item2 == rend);
        ColorToRends.Add((color, rend));
    }

    public void Update()
    {
        IDToRends.ForEach(x => CustomColorManager.SetColor(x.Item2, x.Item1));
        ColorToRends.ForEach(x => CustomColorManager.SetColor(x.Item2, x.Item1));
    }
}