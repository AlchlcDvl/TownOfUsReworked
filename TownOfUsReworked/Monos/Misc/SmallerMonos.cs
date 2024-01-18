namespace TownOfUsReworked.Monos;

public class ColorBehaviour : MonoBehaviour
{
    public Renderer Renderer;
    public int ID;

    public ColorBehaviour(IntPtr ptr) : base(ptr) {}

    public void AddRend(Renderer rend, int id)
    {
        Renderer = rend;
        ID = id;
    }

    public void Update()
    {
        if (!Renderer || ID == -1)
            return;

        CustomColorManager.SetColor(Renderer, ID);
    }
}

public class MissingBehaviour : MonoBehaviour
{
    public MissingBehaviour(IntPtr ptr) : base(ptr) {}
}