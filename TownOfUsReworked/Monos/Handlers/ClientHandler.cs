namespace TownOfUsReworked.Monos;

public class ClientHandler : MonoBehaviour
{
    public PassiveButton ZoomButton;
    public bool Zooming;

    public SpriteRenderer Phone;
    public TextMeshPro PhoneText;

    public PassiveButton WikiRCButton;
    public bool WikiActive;
    public bool RoleCardActive;
    public PassiveButton ToTheWiki;
    public PassiveButton NextButton;
    public PassiveButton BackButton;
    public PassiveButton LoreButton;
    public PassiveButton YourStatus;
    public readonly Dictionary<int, List<PassiveButton>> Buttons = [];
    public readonly Dictionary<int, KeyValuePair<string, Info>> Sorted = [];
    public int Page;
    public int ResultPage;
    public int MaxPage;
    public bool PagesSet;
    public Info Selected;
    public bool LoreActive;
    public bool SelectionActive;
    public readonly List<string> Entry = [];
    // Max page line limit is 20

    // public PassiveButton SettingsButton;
    // public bool SettingsActive;

    public PassiveButton ClientOptionsButton;

    private bool ButtonsSet;

    private Transform ButtonsParent;

    private ProgressTracker _taskBar;
    private ProgressTracker TaskBar
    {
        get
        {
            if (!_taskBar)
                _taskBar = FindObjectOfType<ProgressTracker>(true);

            return _taskBar;
        }
    }

    public static ClientHandler Instance { get; private set; }

    public ClientHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void OnLobbyStart()
    {
        if (!ButtonsParent)
        {
            var obj = HUD().AbilityButton.transform.parent;
            ButtonsParent = Instantiate(obj, obj.parent);
            ButtonsParent.name = "BottomLeft";
            ButtonsParent.transform.localPosition = new(-obj.localPosition.x, obj.localPosition.y, obj.localPosition.z);
            var grid = ButtonsParent.GetComponent<GridArrange>();
            grid.Alignment = GridArrange.StartAlign.Right;
            grid.MaxColumns = 2;
            grid.CellSize = new(0.85f, 0.8f);
            var count = ButtonsParent.GetChildCount();

            for (var i = 0; i < count; i++)
                ButtonsParent.GetChild(i).gameObject.Destroy();
        }

        if (!WikiRCButton)
        {
            WikiRCButton = Instantiate(HUD().MapButton, ButtonsParent);
            WikiRCButton.OverrideOnClickListeners(ClientStuff.Open);
            WikiRCButton.name = "WikiAndRCButton";
            WikiRCButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("WikiInactive");
            WikiRCButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("WikiActive");
            WikiRCButton.transform.Find("Background").localPosition = Vector3.zero;
        }

        // if (!SettingsButton)
        // {
        //     SettingsButton = Instantiate(HUD().MapButton, ButtonsParent);
        //     SettingsButton.OverrideOnClickListeners(ClientStuff.OpenSettings);
        //     SettingsButton.name = "CustomSettingsButton";
        //     SettingsButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("SettingsInactive");
        //     SettingsButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("SettingsActive");
        //     SettingsButton.transform.Find("Background").localPosition = Vector3.zero;
        // }

        if (!ClientOptionsButton)
        {
            ClientOptionsButton = Instantiate(HUD().MapButton, ButtonsParent);
            ClientOptionsButton.OverrideOnClickListeners(LobbyConsole.CreateMenu);
            ClientOptionsButton.name = "ClientOptionsButton";
            ClientOptionsButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("ClientInactive");
            ClientOptionsButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("ClientActive");
            ClientOptionsButton.transform.Find("Background").localPosition = Vector3.zero;
        }

        if (!ZoomButton)
        {
            ZoomButton = Instantiate(HUD().MapButton, ButtonsParent);
            ZoomButton.OverrideOnClickListeners(ClientStuff.ClickZoom);
            ZoomButton.name = "ZoomButton";
            ZoomButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("MinusInactive");
            ZoomButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("MinusActive");
            ZoomButton.transform.Find("Background").localPosition = Vector3.zero;
        }

        ButtonsSet = true;
    }

    public void Update()
    {
        if (IsHnS() || !CustomPlayer.Local || !HUD().SettingsButton || !HUD().MapButton || !ButtonsSet)
            return;

        ClientStuff.ResetButtonPos();
        WikiRCButton.gameObject.SetActive(!IntroCutscene.Instance && !IsFreePlay() && ActiveTask() is not HauntMenuMinigame && !GameSettingMenu.Instance);
        // SettingsButton.gameObject.SetActive(HUD().MapButton.gameObject.active && !IntroCutscene.Instance && IsNormal() && !IsFreePlay() && IsInGame() && ActiveTask() is not HauntMenuMinigame);
        ClientOptionsButton.gameObject.SetActive(HUD().MapButton.gameObject.active && !IntroCutscene.Instance && IsNormal() && !IsFreePlay() && IsInGame() && ActiveTask() is not HauntMenuMinigame &&
            !GameSettingMenu.Instance);
        ZoomButton.gameObject.SetActive(HUD().MapButton.gameObject.active && IsNormal() && CustomPlayer.LocalCustom.Dead && !IntroCutscene.Instance && !IsFreePlay() && IsInGame() && (!CustomPlayer.Local.IsPostmortal() ||
            CustomPlayer.Local.Caught()) && ActiveTask() is not HauntMenuMinigame && !GameSettingMenu.Instance);

        if (PhoneText)
        {
            if (RoleCardActive)
                PhoneText.text = CustomPlayer.Local.RoleCardInfo();
            else if ((LoreActive || SelectionActive) && Entry.Count > 1)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f)
                    ResultPage = CycleInt(Entry.Count - 1, 0, ResultPage, false);
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f)
                    ResultPage = CycleInt(Entry.Count - 1, 0, ResultPage, true);

                PhoneText.text = Entry[ResultPage];
            }
        }

        if (!IsInGame())
            return;

        var part = !RoleCardActive /*&& !SettingsActive*/ && !Zooming && !(Map() && Map().IsOpen) && !WikiActive && !IsCustomHnS() && !GameSettingMenu.Instance;
        HUD()?.TaskPanel?.gameObject?.SetActive(part && !Meeting() && !IsCustomHnS());
        TaskBar?.gameObject?.SetActive(part && GameSettings.TaskBarMode != TBMode.Invisible);
    }
}