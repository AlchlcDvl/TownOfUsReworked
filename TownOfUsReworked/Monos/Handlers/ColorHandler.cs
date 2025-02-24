namespace TownOfUsReworked.Monos;

public class ColorHandler : MonoBehaviour
{
    private readonly List<(int, Renderer)> IDToRends = [];
    private readonly List<(UColor, Renderer)> ColorToRends = [];

    public void SetRend(Renderer rend, int id)
    {
        IDToRends.RemoveAll(x => x.Item2 == rend || !x.Item2);
        ColorToRends.RemoveAll(x => x.Item2 == rend || !x.Item2);
        IDToRends.Add((id, rend));
    }

    public void SetRend(Renderer rend, UColor color)
    {
        IDToRends.RemoveAll(x => x.Item2 == rend || !x.Item2);
        ColorToRends.RemoveAll(x => x.Item2 == rend || !x.Item2);
        ColorToRends.Add((color, rend));
    }

    public void Update()
    {
        IDToRends.ForEach(x => CustomColorManager.SetColor(x.Item2, x.Item1));
        ColorToRends.ForEach(x => CustomColorManager.SetColor(x.Item2, x.Item1));
    }
}