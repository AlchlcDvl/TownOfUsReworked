namespace TownOfUsReworked.Monos;

public class ColorHandler : MonoBehaviour
{
    public static ColorHandler Instance { get; private set; }
    public readonly List<(int, Renderer Rend)> IDToRends = [];

    public ColorHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void SetRend(Renderer rend, int id)
    {
        IDToRends.RemoveAll(x => x.Rend == rend);
        IDToRends.Add((id, rend));
        CustomColorManager.SetColor(rend, id);
    }

    public void Update()
    {
        if (!IsLobby && !IsInGame)
            return;

        foreach (var (id, rend) in IDToRends)
            CustomColorManager.SetColor(rend, id);
    }
}