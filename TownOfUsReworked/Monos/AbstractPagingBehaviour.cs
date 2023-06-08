namespace TownOfUsReworked.Monos
{
    public class AbstractPagingBehaviour : MonoBehaviour
    {
        public AbstractPagingBehaviour(IntPtr ptr) : base(ptr) {}

        private int _page;
        public virtual int MaxPerPage => 15;
        public virtual int MaxPageIndex => throw new NotImplementedException();
        public virtual void OnPageChanged() => throw new NotImplementedException();
        public virtual int PageIndex
        {
            get => _page;
            set
            {
                _page = value;
                OnPageChanged();
            }
        }

        public void Start() => OnPageChanged();

        public virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f)
            {
                if (PageIndex == 0)
                    PageIndex = MaxPageIndex;
                else
                    PageIndex--;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f)
            {
                if (PageIndex == MaxPageIndex)
                    PageIndex = 0;
                else
                    PageIndex++;
            }
        }
    }
}