namespace TownOfUsReworked.Monos;

public class ColorBehaviour : MonoBehaviour
{
    public Renderer Renderer;
    public int Id;

    public ColorBehaviour(IntPtr ptr) : base(ptr) {}

    public void AddRend(Renderer rend, int id)
    {
        Renderer = rend;
        Id = id;
    }

    public void Update()
    {
        if (!Renderer)
            return;

        CustomColors.SetChangingColor(Renderer, Id);
    }
}

public class MissingBehaviour : MonoBehaviour
{
    public MissingBehaviour(IntPtr ptr) : base(ptr) {}
}