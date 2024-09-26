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

    private bool ButtonsSet;

    public static ClientHandler Instance { get; private set; }

    public ClientHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void OnLobbyStart()
    {
        // if (!WikiRCButton)
        // {
        //     WikiRCButton = Instantiate(HUD().MapButton, HUD().MapButton.transform.parent);
        //     WikiRCButton.GetComponent<SpriteRenderer>().sprite = GetSprite("Wiki");
        //     WikiRCButton.OverrideOnClickListeners(ClientStuff.Open);
        //     WikiRCButton.name = "WikiAndRCButton";
        // }

        // if (!SettingsButton)
        // {
        //     SettingsButton = Instantiate(HUD().MapButton, HUD().MapButton.transform.parent);
        //     SettingsButton.OverrideOnClickListeners(ClientStuff.OpenSettings);
        //     SettingsButton.GetComponent<SpriteRenderer>().sprite = GetSprite("CurrentSettings");
        //     SettingsButton.name = "CustomSettingsButton";
        // }

        // if (!ClientOptionsButton)
        // {
        //     ClientOptionsButton = Instantiate(HUD().MapButton, HUD().MapButton.transform.parent);
        //     ClientOptionsButton.OverrideOnClickListeners(LobbyConsole.CreateMenu);
        //     ClientOptionsButton.GetComponent<SpriteRenderer>().sprite = GetSprite("Client");
        //     ClientOptionsButton.name = "ClientOptionsButton";
        // }

        // if (!ZoomButton)
        // {
        //     ZoomButton = Instantiate(HUD().MapButton, HUD().MapButton.transform.parent);
        //     ZoomButton.OverrideOnClickListeners(ClientStuff.ClickZoom);
        //     ZoomButton.name = "ZoomButton";
        // }

        ButtonsSet = true;
    }

    public void Update()
    {
        if (IsHnS() || !HUD() || !CustomPlayer.Local || !HUD().SettingsButton || !HUD().MapButton || !ButtonsSet)
            return;

        var pos = HUD().SettingsButton.transform.localPosition + new Vector3(0, -0.66f, -HUD().SettingsButton.transform.localPosition.z - 51f);
        WikiRCButton.transform.localPosition = pos;

        // pos += new Vector3(0, -0.66f, 0f);
        // HUD().MapButton.transform.localPosition = pos;

        // if (IsSubmerged() && CustomPlayer.LocalCustom.Dead)
        // {
        //     var floorButton = HUD().MapButton.transform.parent.Find($"{HUD().MapButton.name}(Clone)");

        //     if (floorButton)
        //     {
        //         pos += new Vector3(0, -0.66f, 0f);
        //         floorButton.localPosition = pos;
        //         floorButton.gameObject.SetActive(CustomPlayer.Local.Caught() || !CustomPlayer.Local.IsPostmortal());
        //     }
        // }

        // pos += new Vector3(0, -0.66f, 0f);
        // SettingsButton.transform.localPosition = pos;

        // pos += new Vector3(0, -0.66f, 0f);
        // ClientOptionsButton.transform.localPosition = pos;

        // pos += new Vector3(0, -0.66f, 0f);
        // ZoomButton.transform.localPosition = pos;

        ClientStuff.ResetButtonPos();
        WikiRCButton.gameObject.SetActive(!IntroCutscene.Instance && !IsFreePlay());
        SettingsButton.gameObject.SetActive(HUD().MapButton.gameObject.active && !IntroCutscene.Instance && IsNormal() && !IsFreePlay() && IsInGame());
        ClientOptionsButton.gameObject.SetActive(HUD().MapButton.gameObject.active && !IntroCutscene.Instance && IsNormal() && !IsFreePlay() && IsInGame());
        ZoomButton.gameObject.SetActive(HUD().MapButton.gameObject.active && IsNormal() && CustomPlayer.LocalCustom.Dead && !IntroCutscene.Instance && !IsFreePlay() && IsInGame() &&
            (!CustomPlayer.Local.IsPostmortal() || CustomPlayer.Local.Caught()));
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

        if (!IsInGame())
            return;

        HUD()?.TaskPanel?.gameObject?.SetActive(!RoleCardActive && !SettingsActive && !Zooming && !Meeting() && !(Map() && Map().IsOpen) && !WikiActive && !IsCustomHnS());
        var taskBar = FindObjectOfType<ProgressTracker>(true);

        if (taskBar)
        {
            if (GameSettings.TaskBarMode == TBMode.Invisible)
                taskBar.gameObject.SetActive(false);
            else
                taskBar.gameObject.SetActive(!RoleCardActive && !SettingsActive && !Zooming && !(Map() && Map().IsOpen) && !WikiActive);
        }
    }
}