namespace TownOfUsReworked.Monos;

public class ColorHandler : MonoBehaviour
{
    public static ColorHandler Instance { get; private set; }
    public readonly List<(int, Renderer)> IDToRends = [];

    public ColorHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void SetRend(Renderer rend, int id)
    {
        IDToRends.RemoveAll(x => x.Item2 == rend);
        IDToRends.Add((id, rend));
    }

    public void Update() => IDToRends.ForEach((x) => CustomColorManager.SetColor(x.Item2, x.Item1));
}