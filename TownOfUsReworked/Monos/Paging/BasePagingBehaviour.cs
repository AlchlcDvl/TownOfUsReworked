namespace TownOfUsReworked.Monos;

public abstract class BasePagingBehaviour : MonoBehaviour
{
    public virtual int MaxPageIndex { get; }

    private int Page;
    public int PageIndex
    {
        get => Page;
        set
        {
            Page = value;
            OnPageChanged();
        }
    }

    public virtual void OnPageChanged() {}

    public virtual void Start() {}

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f)
            PageIndex = CycleInt(MaxPageIndex, 0, PageIndex, false);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f)
            PageIndex = CycleInt(MaxPageIndex, 0, PageIndex, true);
    }
}