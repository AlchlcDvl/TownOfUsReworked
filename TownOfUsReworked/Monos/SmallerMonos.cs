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

        ColorUtils.SetChangingColor(Renderer, Id);
    }
}

public abstract class BasePagingBehaviour : MonoBehaviour
{
    protected BasePagingBehaviour(IntPtr ptr) : base(ptr) {}

    private int _page;
    public virtual int MaxPerPage => 15;
    public virtual int MaxPageIndex => 0;
    public virtual int PageIndex
    {
        get => _page;
        set
        {
            _page = value;
            OnPageChanged();
        }
    }

    public virtual void OnPageChanged() {}

    public virtual void Start() => OnPageChanged();

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f)
            PageIndex = CycleInt(MaxPageIndex, 0, PageIndex, false);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f)
            PageIndex = CycleInt(MaxPageIndex, 0, PageIndex, true);
    }
}

public class MissingBehaviour : MonoBehaviour
{
    public MissingBehaviour(IntPtr ptr) : base(ptr) {}
}