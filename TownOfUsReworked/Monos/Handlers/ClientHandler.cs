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

    public PassiveButton SettingsButton;
    public bool SettingsActive;

    public PassiveButton ClientOptionsButton;

    public static ClientHandler Instance { get; private set; }

    public ClientHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void Update()
    {
        if (IsHnS || !HUD || !CustomPlayer.Local)
            return;

        var pos = HUD.SettingsButton.transform.localPosition + new Vector3(0, -0.66f, -HUD.SettingsButton.transform.localPosition.z - 51f);

        if (!WikiRCButton)
        {
            WikiRCButton = Instantiate(HUD.MapButton, HUD.MapButton.transform.parent);
            WikiRCButton.GetComponent<SpriteRenderer>().sprite = GetSprite("Wiki");
            WikiRCButton.GetComponent<PassiveButton>().OnClick = new();
            WikiRCButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(ClientStuff.Open));
            WikiRCButton.name = "WikiAndRCButton";
        }

        WikiRCButton.gameObject.SetActive(!IntroCutscene.Instance && !IsFreePlay);
        WikiRCButton.transform.localPosition = pos;
        ClientStuff.ResetButtonPos();

        pos += new Vector3(0, -0.66f, 0f);
        HUD.MapButton.transform.localPosition = pos;

        if (IsSubmerged() && CustomPlayer.LocalCustom.Dead)
        {
            var floorButton = HUD.MapButton.transform.parent.Find($"{HUD.MapButton.name}(Clone)");

            if (floorButton && floorButton.gameObject.active)
            {
                pos += new Vector3(0, -0.66f, 0f);
                floorButton.localPosition = pos;
            }

            floorButton.gameObject.SetActive(CustomPlayer.Local.Caught() || !CustomPlayer.Local.IsPostmortal());
        }

        if (!SettingsButton)
        {
            SettingsButton = Instantiate(HUD.MapButton, HUD.MapButton.transform.parent);
            SettingsButton.GetComponent<SpriteRenderer>().sprite = GetSprite("CurrentSettings");
            SettingsButton.OnClick = new();
            SettingsButton.OnClick.AddListener(new Action(ClientStuff.OpenSettings));
            SettingsButton.name = "CustomSettingsButton";
        }

        pos += new Vector3(0, -0.66f, 0f);
        SettingsButton.gameObject.SetActive(HUD.MapButton.gameObject.active && !IntroCutscene.Instance && IsNormal && !IsFreePlay && IsInGame);
        SettingsButton.transform.localPosition = pos;

        if (!ClientOptionsButton)
        {
            ClientOptionsButton = Instantiate(HUD.MapButton, HUD.MapButton.transform.parent);
            ClientOptionsButton.OnClick = new();
            ClientOptionsButton.OnClick.AddListener(new Action(LobbyConsole.CreateMenu));
            ClientOptionsButton.name = "ClientOptionsButton";
        }

        pos += new Vector3(0, -0.66f, 0f);
        ClientOptionsButton.gameObject.SetActive(HUD.MapButton.gameObject.active && !IntroCutscene.Instance && IsNormal && !IsFreePlay && IsInGame);
        ClientOptionsButton.transform.localPosition = pos;
        ClientOptionsButton.GetComponent<SpriteRenderer>().sprite = GetSprite("Client");

        if (!ZoomButton)
        {
            ZoomButton = Instantiate(HUD.MapButton, HUD.MapButton.transform.parent);
            ZoomButton.OnClick = new();
            ZoomButton.OnClick.AddListener(new Action(ClientStuff.ClickZoom));
            ZoomButton.name = "ZoomButton";
        }

        pos += new Vector3(0, -0.66f, 0f);
        ZoomButton.gameObject.SetActive(HUD.MapButton.gameObject.active && IsNormal && CustomPlayer.LocalCustom.Dead && !IntroCutscene.Instance && !IsFreePlay &&
            (!CustomPlayer.Local.IsPostmortal() || CustomPlayer.Local.Caught()) && IsInGame);
        ZoomButton.transform.localPosition = pos;
        ZoomButton.GetComponent<SpriteRenderer>().sprite = GetSprite(Zooming ? "Plus" : "Minus");

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

        if (!IsInGame)
            return;

        if (HUD.TaskPanel)
            HUD.TaskPanel.gameObject.SetActive(!RoleCardActive && !SettingsActive && !Zooming && !Meeting && !(Map && Map.IsOpen) && !WikiActive && !IsCustomHnS);

        var taskBar = FindObjectOfType<ProgressTracker>(true);

        if (taskBar)
        {
            if (CustomGameOptions.TaskBarMode == TaskBar.Invisible)
                taskBar.gameObject.SetActive(false);
            else
                taskBar.gameObject.SetActive(!RoleCardActive && !SettingsActive && !Zooming && !(Map && Map.IsOpen) && !WikiActive);
        }
    }
}