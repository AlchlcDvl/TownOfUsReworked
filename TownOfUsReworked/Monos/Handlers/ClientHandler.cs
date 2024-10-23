using Cpp2IL.Core.Extensions;

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
    public readonly Dictionary<int, List<(Info, PassiveButton)>> Buttons = [];
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

    public PassiveButton ClientOptionsButton;
    public bool SettingsActive;

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

    public static GameObject Prefab;
    public static Vector3 Pos;

    public static ClientHandler Instance { get; private set; }

    public ClientHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void OnLobbyStart(LobbyBehaviour __instance)
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
            WikiRCButton.OverrideOnClickListeners(Open);
            WikiRCButton.name = "WikiAndRCButton";
            WikiRCButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("WikiInactive");
            WikiRCButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("WikiActive");
            WikiRCButton.transform.Find("Background").localPosition = Vector3.zero;
        }

        if (!ClientOptionsButton)
        {
            ClientOptionsButton = Instantiate(HUD().MapButton, ButtonsParent);
            ClientOptionsButton.OverrideOnClickListeners(CreateMenu);
            ClientOptionsButton.name = "ClientOptionsButton";
            ClientOptionsButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("SettingsInactive");
            ClientOptionsButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("SettingsActive");
            ClientOptionsButton.transform.Find("Background").localPosition = Vector3.zero;
        }

        if (!ZoomButton)
        {
            ZoomButton = Instantiate(HUD().MapButton, ButtonsParent);
            ZoomButton.OverrideOnClickListeners(ClickZoom);
            ZoomButton.name = "ZoomButton";
            ZoomButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("MinusInactive");
            ZoomButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("MinusActive");
            ZoomButton.transform.Find("Background").localPosition = Vector3.zero;
        }

        if (!Prefab)
        {
            var options = __instance.transform.FindChild("SmallBox").GetChild(0).GetComponent<OptionsConsole>();
            Prefab = Instantiate(options.MenuPrefab, null).DontUnload().DontDestroy();
            Prefab.SetActive(false);
            Prefab.name = "ClientOptionsMenuPrefab";
            Pos = options.CustomPosition;
        }

        ButtonsSet = true;
    }

    public void Update()
    {
        if (IsHnS() || !CustomPlayer.Local || !HUD().SettingsButton || !HUD().MapButton || !ButtonsSet)
            return;

        ResetButtonPos();
        var part2 = !IntroCutscene.Instance && ActiveTask() is not HauntMenuMinigame && !GameSettingMenu.Instance;
        WikiRCButton.gameObject.SetActive(part2);
        ClientOptionsButton.gameObject.SetActive(part2);
        ZoomButton.gameObject.SetActive(HUD().MapButton.gameObject.active && IsNormal() && CustomPlayer.LocalCustom.Dead && IsInGame() && part2 && (!CustomPlayer.Local.IsPostmortal() ||
            CustomPlayer.Local.Caught()) && !Meeting() && !IsFreePlay());

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

        var part = !RoleCardActive && !SettingsActive && !Zooming && !(Map() && Map().IsOpen) && !WikiActive && !IsCustomHnS() && !GameSettingMenu.Instance;
        HUD()?.TaskPanel?.gameObject?.SetActive(part && !Meeting() && !IsCustomHnS());
        TaskBar?.gameObject?.SetActive(part && GameSettings.TaskBarMode != TBMode.Invisible);
    }
    public static void ClickZoom()
    {
        CloseMenus(SkipEnum.Zooming);
        ClientHandler.Instance.Zooming = !ClientHandler.Instance.Zooming;
        Coroutines.Start(Zoom(ClientHandler.Instance.Zooming));
    }

    private static IEnumerator Zoom(bool inOut)
    {
        ClientHandler.Instance.ZoomButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite($"{(inOut ? "Plus" : "Minus")}Inactive");
        ClientHandler.Instance.ZoomButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite($"{(inOut ? "Plus" : "Minus")}Active");
        var change = 0.3f * (inOut ? 1 : -1);
        var limit = inOut ? 12f : 3f;
        // HUD().SetHudActive(false);

        for (var i = Camera.main.orthographicSize; inOut ? (i < 12f) : (i > 3f); i += change)
        {
            var size = Meeting() ? 3f : i;
            Camera.main.orthographicSize = size;
            Camera.allCameras.ForEach(x => x.orthographicSize = size);
            yield return EndFrame();
        }

        Camera.main.orthographicSize = limit;
        Camera.allCameras.ForEach(x => x.orthographicSize = limit);
        // ResolutionManager.ResolutionChanged.Invoke(Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
        // HUD().SetHudActive(true);
        yield break;
    }

    public static void OpenRoleCard()
    {
        CloseMenus(SkipEnum.RoleCard);

        if (LocalBlocked())
            return;

        if (ClientHandler.Instance.PhoneText)
            ClientHandler.Instance.PhoneText.gameObject.Destroy();

        ClientHandler.Instance.PhoneText = UObject.Instantiate(HUD().KillButton.cooldownTimerText, ClientHandler.Instance.Phone.transform);
        ClientHandler.Instance.PhoneText.enableWordWrapping = false;
        ClientHandler.Instance.PhoneText.transform.localScale = Vector3.one * 0.4f;
        ClientHandler.Instance.PhoneText.transform.localPosition = new(0, 0, -50f);
        ClientHandler.Instance.PhoneText.gameObject.layer = 5;
        ClientHandler.Instance.PhoneText.alignment = TextAlignmentOptions.Center;
        ClientHandler.Instance.PhoneText.name = "PhoneText";

        if (!ClientHandler.Instance.ToTheWiki)
        {
            ClientHandler.Instance.ToTheWiki = CreateButton("ToTheWiki", "Mod Wiki", () =>
            {
                OpenRoleCard();
                OpenWiki();
            });
        }

        ClientHandler.Instance.RoleCardActive = !ClientHandler.Instance.RoleCardActive;
        ClientHandler.Instance.PhoneText.text = CustomPlayer.Local.RoleCardInfo();
        ClientHandler.Instance.PhoneText.gameObject.SetActive(ClientHandler.Instance.RoleCardActive);
        ClientHandler.Instance.Phone.gameObject.SetActive(ClientHandler.Instance.RoleCardActive);
        ClientHandler.Instance.ToTheWiki.gameObject.SetActive(ClientHandler.Instance.RoleCardActive && IsNormal() && IsInGame());
    }

    public static void Open()
    {
        if (!ClientHandler.Instance.Phone)
        {
            ClientHandler.Instance.Phone = new GameObject("Phone") { layer = 5 }.AddComponent<SpriteRenderer>();
            ClientHandler.Instance.Phone.sprite = GetSprite("Phone");
            ClientHandler.Instance.Phone.transform.SetParent(HUD().transform);
            ClientHandler.Instance.Phone.transform.localPosition = new(0, 0, -49f);
            ClientHandler.Instance.Phone.transform.localScale *= 1.25f;
        }

        if (IsInGame())
            OpenRoleCard();
        else
            OpenWiki();
    }

    public static void CreateMenu()
    {
        CloseMenus(SkipEnum.Settings);

        if (GameSettingMenu.Instance)
        {
            GameSettingMenu.Instance.Close();
            return;
        }

        SettingsPatches.SettingsPage = 3;
        CustomPlayer.Local.NetTransform.Halt();
        var currentMenu = UObject.Instantiate(ClientHandler.Prefab);
        currentMenu.transform.SetParent(Camera.main.transform, false);
        currentMenu.transform.localPosition = ClientHandler.Pos;
        currentMenu.name = "ClientOptionsMenu";
        TransitionFade.Instance.DoTransitionFade(null, currentMenu.gameObject, null);
    }

    public static void OpenWiki()
    {
        CloseMenus(SkipEnum.Wiki);

        if (LocalBlocked())
            return;

        if (!ClientHandler.Instance.PagesSet)
        {
            var clone = Modules.Info.AllInfo.Clone();
            clone.RemoveAll(x => x.Name is "Invalid" or "None" || x.Type == InfoType.Lore);
            var i = 0;
            var j = 0;
            var k = 0;

            foreach (var pair in clone)
            {
                ClientHandler.Instance.Sorted.Add(j, new(pair is SymbolInfo symbol ? symbol.Symbol : pair.Name, pair));
                j++;
                k++;

                if (k >= 28 || (pair.Footer && pair != clone[^1]))
                {
                    i++;
                    k = 0;
                }
            }

            ClientHandler.Instance.MaxPage = i;
            ClientHandler.Instance.PagesSet = true;
        }

        if (!ClientHandler.Instance.NextButton)
        {
            ClientHandler.Instance.NextButton = CreateButton("WikiNextButton", "Next Page", () =>
            {
                ClientHandler.Instance.Page = CycleInt(ClientHandler.Instance.MaxPage, 0, ClientHandler.Instance.Page, true);
                ResetButtonPos();
            });
        }

        if (!ClientHandler.Instance.BackButton)
        {
            ClientHandler.Instance.BackButton = CreateButton("WikiBack", "Previous Page", () =>
            {
                if (ClientHandler.Instance.Selected == null)
                    ClientHandler.Instance.Page = CycleInt(ClientHandler.Instance.MaxPage, 0, ClientHandler.Instance.Page, false);
                else if (ClientHandler.Instance.LoreActive)
                {
                    ClientHandler.Instance.PhoneText.gameObject.SetActive(false);
                    AddInfo();
                    ClientHandler.Instance.LoreActive = false;
                }
                else
                {
                    ClientHandler.Instance.Selected = null;
                    ClientHandler.Instance.SelectionActive = false;
                    ClientHandler.Instance.LoreButton.gameObject.SetActive(false);
                    ClientHandler.Instance.NextButton.gameObject.SetActive(true);
                    ClientHandler.Instance.NextButton.transform.localPosition = new(2.5f, 1.6f, 0f);
                    ClientHandler.Instance.PhoneText.gameObject.SetActive(false);
                    ClientHandler.Instance.Entry.Clear();
                }

                ResetButtonPos();
            });
        }

        if (!ClientHandler.Instance.YourStatus)
        {
            ClientHandler.Instance.YourStatus = CreateButton("YourStatus", "Your Status", () =>
            {
                OpenWiki();
                OpenRoleCard();
            });
        }

        if (!ClientHandler.Instance.LoreButton)
        {
            ClientHandler.Instance.LoreButton = CreateButton("WikiLore", "Lore", () =>
            {
                ClientHandler.Instance.LoreActive = !ClientHandler.Instance.LoreActive;
                SetEntryText(Modules.Info.ColorIt(WrapText(LayerInfo.AllLore.Find(x => x.Name == ClientHandler.Instance.Selected.Name || x.Short == ClientHandler.Instance.Selected.Short).Description)));
                ClientHandler.Instance.PhoneText.text = ClientHandler.Instance.Entry[0];
                ClientHandler.Instance.PhoneText.transform.localPosition = new(-2.6f, 0.45f, -5f);
                ClientHandler.Instance.SelectionActive = true;
            });
        }

        if (ClientHandler.Instance.Buttons.Count == 0)
        {
            var i = 0;
            var j = 0;

            for (var k = 0; k < ClientHandler.Instance.Sorted.Count; k++)
            {
                if (!ClientHandler.Instance.Buttons.ContainsKey(i))
                    ClientHandler.Instance.Buttons.Add(i, []);

                var cache = ClientHandler.Instance.Sorted[k].Value;
                var cache2 = ClientHandler.Instance.Sorted[k].Key;
                var button = CreateButton($"{cache2}Info", cache2, () =>
                {
                    foreach (var buttons in ClientHandler.Instance.Buttons.Values)
                    {
                        if (buttons.Any())
                            buttons.Select(x => x.Item2).ForEach(x => x?.gameObject?.SetActive(false));
                    }

                    ClientHandler.Instance.Selected = cache;
                    ClientHandler.Instance.NextButton.gameObject.SetActive(false);
                    AddInfo();
                }, cache.Color);

                ClientHandler.Instance.Buttons[i].Add((cache, button));
                j++;

                if (j >= 28 || cache.Footer)
                {
                    i++;
                    j = 0;
                }
            }

            ClientHandler.Instance.Buttons.ToList().ForEach(x =>
            {
                if (!x.Value.Any())
                    ClientHandler.Instance.Buttons.Remove(x.Key);
            });
        }

        ClientHandler.Instance.WikiActive = !ClientHandler.Instance.WikiActive;
        ClientHandler.Instance.Phone.gameObject.SetActive(ClientHandler.Instance.WikiActive);
        ClientHandler.Instance.NextButton.gameObject.SetActive(ClientHandler.Instance.WikiActive);
        ClientHandler.Instance.BackButton.gameObject.SetActive(ClientHandler.Instance.WikiActive);
        ClientHandler.Instance.YourStatus.gameObject.SetActive(ClientHandler.Instance.WikiActive && IsNormal() && IsInGame());
        ResetButtonPos();
        ClientHandler.Instance.Selected = null;

        if (!ClientHandler.Instance.WikiActive && ClientHandler.Instance.PhoneText)
            ClientHandler.Instance.PhoneText.gameObject.SetActive(false);
    }

    public static void ResetButtonPos()
    {
        if (ClientHandler.Instance.BackButton)
            ClientHandler.Instance.BackButton.transform.localPosition = new(-2.6f, 1.6f, 0f);

        if (ClientHandler.Instance.NextButton)
            ClientHandler.Instance.NextButton.transform.localPosition = new(2.5f, 1.6f, 0f);

        if (ClientHandler.Instance.YourStatus)
            ClientHandler.Instance.YourStatus.transform.localPosition = new(0f, 1.6f, 0f);

        if (ClientHandler.Instance.ToTheWiki)
            ClientHandler.Instance.ToTheWiki.transform.localPosition = new(-2.6f, 1.6f, 0f);

        if (ClientHandler.Instance.Selected != null)
        {
            if (LayerInfo.AllLore.Any(x => x.Name == ClientHandler.Instance.Selected.Name || x.Short == ClientHandler.Instance.Selected.Short) && ClientHandler.Instance.LoreButton)
            {
                ClientHandler.Instance.LoreButton.gameObject.SetActive(!ClientHandler.Instance.LoreActive);
                ClientHandler.Instance.LoreButton.transform.localPosition = new(0f, -1.7f, 0f);
            }

            return;
        }

        var m = 0;

        foreach (var pair in ClientHandler.Instance.Buttons)
        {
            foreach (var pair2 in pair.Value)
            {
                var button = pair2.Item2;

                if (!button)
                    continue;

                var row = m / 4;
                var col = m % 4;
                button.transform.localPosition = new(-2.6f + (1.7f * col), 1f - (0.45f * row), -1f);
                button.gameObject.SetActive(ClientHandler.Instance.Page == pair.Key && ClientHandler.Instance.WikiActive);
                m++;

                if (m >= 28 || pair2.Item1.Footer)
                    m = 0;
            }
        }
    }

    public static void AddInfo()
    {
        if (ClientHandler.Instance.PhoneText)
            ClientHandler.Instance.PhoneText.gameObject.Destroy();

        ClientHandler.Instance.Selected.WikiEntry(out var result);
        SetEntryText(result);
        ClientHandler.Instance.PhoneText = UObject.Instantiate(HUD().TaskPanel.taskText, ClientHandler.Instance.Phone.transform);
        ClientHandler.Instance.PhoneText.color = UColor.white;
        ClientHandler.Instance.PhoneText.text = ClientHandler.Instance.Entry[0];
        ClientHandler.Instance.PhoneText.enableWordWrapping = false;
        ClientHandler.Instance.PhoneText.transform.localScale = Vector3.one * 0.75f;
        ClientHandler.Instance.PhoneText.transform.localPosition = new(-2.6f, 0.45f, -5f);
        ClientHandler.Instance.PhoneText.alignment = TextAlignmentOptions.TopLeft;
        ClientHandler.Instance.PhoneText.fontStyle = FontStyles.Bold;
        ClientHandler.Instance.PhoneText.gameObject.SetActive(true);
        ClientHandler.Instance.PhoneText.name = "PhoneText";
        ClientHandler.Instance.SelectionActive = true;
    }

    public static PassiveButton CreateButton(string name, string labelText, Action onClick, UColor? textColor = null)
    {
        var button = UObject.Instantiate(HUD().MapButton, ClientHandler.Instance.Phone.transform);
        button.name = $"{name}Button";
        button.transform.localScale = new(0.5f, 0.5f, 1f);
        button.GetComponent<BoxCollider2D>().size = new(2.5f, 0.55f);
        var label = UObject.Instantiate(HUD().TaskPanel.taskText, button.transform);
        label.color = textColor ?? UColor.white;
        label.text = labelText;
        label.enableWordWrapping = false;
        label.transform.localPosition = new(0f, 0f, label.transform.localPosition.z);
        label.transform.localScale *= 1.55f;
        label.alignment = TextAlignmentOptions.Center;
        label.fontStyle = FontStyles.Bold;
        label.name = $"{name}Text";
        var rend = button.transform.Find("Background").GetComponent<SpriteRenderer>();
        rend.sprite = GetSprite("Plate");
        rend.color = UColor.white;
        rend.transform.localScale = new(0.9f, 0.9f, 1f);
        button.OverrideOnClickListeners(onClick);
        button.OverrideOnMouseOverListeners(() => rend.color = UColor.yellow);
        button.OverrideOnMouseOutListeners(() => rend.color = UColor.white);
        button.transform.Find("Active").gameObject.Destroy();
        button.transform.Find("Inactive").gameObject.Destroy();
        return button;
    }

    public static void CloseMenus(SkipEnum skip = SkipEnum.None)
    {
        if (ClientHandler.Instance.WikiActive && skip != SkipEnum.Wiki)
            OpenWiki();

        if (ClientHandler.Instance.RoleCardActive && skip != SkipEnum.RoleCard)
            OpenRoleCard();

        if (ClientHandler.Instance.Zooming && skip != SkipEnum.Zooming)
            ClickZoom();

        if (MapPatch.MapActive && Map() && skip != SkipEnum.Map)
            Map().Close();

        if (ActiveTask() && skip != SkipEnum.Task)
            ActiveTask().Close();

        if (GameSettingMenu.Instance && skip != SkipEnum.Settings)
            GameSettingMenu.Instance.Close();
    }

    public static void SetEntryText(string result)
    {
        ClientHandler.Instance.Entry.Clear();
        ClientHandler.Instance.ResultPage = 0;
        var texts = result.Split('\n');
        var pos = 0;
        var result2 = "";

        foreach (var text in texts)
        {
            result2 += $"{text}\n";
            pos++;

            if (pos >= 19)
            {
                ClientHandler.Instance.Entry.Add(result2);
                result2 = "";
                pos -= 19;
            }
        }

        if (!IsNullEmptyOrWhiteSpace(result2))
            ClientHandler.Instance.Entry.Add(result2);
    }
}