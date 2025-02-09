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

    public bool ButtonsSet;

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
        if (IsHnS() || !CustomPlayer.Local || !HUD().SettingsButton || !HUD().MapButton || !ButtonsSet || !ButtonsParent)
            return;

        ResetButtonPos();
        var part2 = !IntroCutscene.Instance && ActiveTask() is not HauntMenuMinigame && !GameSettingMenu.Instance && !PlayerCustomizationMenu.Instance;
        WikiRCButton.gameObject.SetActive(part2);
        ClientOptionsButton.gameObject.SetActive(part2 && !RoleCardActive);
        ZoomButton.gameObject.SetActive(HUD().MapButton.gameObject.active && IsNormal() && CustomPlayer.LocalCustom.Dead && IsInGame() && part2 && (!CustomPlayer.Local.IsPostmortal() ||
            CustomPlayer.Local.Caught()) && !Meeting() && !RoleCardActive);

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

        if (Input.GetKeyDown(KeyCode.Escape))
            CloseMenus();

        if (!IsInGame())
            return;

        var part = !RoleCardActive && !SettingsActive && !Zooming && !(Map() && Map().IsOpen) && !WikiActive && !IsCustomHnS() && !GameSettingMenu.Instance;
        HUD()?.TaskPanel?.gameObject?.SetActive(part && !Meeting() && !IsCustomHnS());
        TaskBar?.gameObject?.SetActive(part && GameSettings.TaskBarMode != TBMode.Invisible);
    }

    public void ClickZoom()
    {
        CloseMenus(SkipEnum.Zooming);
        Zooming = !Zooming;
        Coroutines.Start(Zoom(Zooming));
    }

    private static Vector3 MaxSize = Vector3.zero;
    private static Vector3 MinSize = Vector3.zero;

    [HideFromIl2Cpp]
    private IEnumerator Zoom(bool inOut)
    {
        ZoomButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite($"{(inOut ? "Plus" : "Minus")}Inactive");
        ZoomButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite($"{(inOut ? "Plus" : "Minus")}Active");

        var limit = inOut ? 12f : 3f;
        var original = Camera.main.orthographicSize;

        if (MinSize == Vector3.zero)
            MinSize = HUD().transform.localScale;

        if (MaxSize == Vector3.zero)
            MaxSize = HUD().transform.localScale * 4f;

        var sizeLimit = inOut ? MaxSize : MinSize;
        var originalSize = HUD().transform.localScale;

        yield return PerformTimedAction(1.5f, p =>
        {
            var size = Meeting() ? 3f : Mathf.Lerp(original, limit, p);
            Camera.main.orthographicSize = size;
            HUD().UICamera.orthographicSize = size;
            HUD().transform.localScale = Vector3.Lerp(originalSize, sizeLimit, p);
        });

        yield break;
    }

    public void OpenRoleCard()
    {
        CloseMenus(SkipEnum.RoleCard);

        if (LocalBlocked())
            return;

        if (PhoneText)
            PhoneText.gameObject.Destroy();

        PhoneText = Instantiate(HUD().KillButton.cooldownTimerText, Phone.transform);
        PhoneText.enableWordWrapping = false;
        PhoneText.transform.localScale = Vector3.one * 0.4f;
        PhoneText.transform.localPosition = new(0, 0, -50f);
        PhoneText.gameObject.layer = 5;
        PhoneText.alignment = TextAlignmentOptions.Center;
        PhoneText.name = "PhoneText";

        if (!ToTheWiki)
        {
            ToTheWiki = CreateButton("ToTheWiki", "Mod Wiki", () =>
            {
                OpenRoleCard();
                OpenWiki();
            });
        }

        RoleCardActive = !RoleCardActive;
        PhoneText.text = CustomPlayer.Local.RoleCardInfo();
        PhoneText.gameObject.SetActive(RoleCardActive);
        Phone.gameObject.SetActive(RoleCardActive);
        ToTheWiki.gameObject.SetActive(RoleCardActive && IsNormal() && IsInGame());
    }

    public void Open()
    {
        if (!Phone)
        {
            Phone = new GameObject("Phone") { layer = 5 }.AddComponent<SpriteRenderer>();
            Phone.sprite = GetSprite("Phone");
            Phone.transform.SetParent(HUD().transform);
            Phone.transform.localPosition = new(0, 0, -49f);
            Phone.transform.localScale *= 1.25f;
        }

        if (IsInGame())
            OpenRoleCard();
        else
            OpenWiki();
    }

    public static void CreateMenu()
    {
        Instance.CloseMenus(SkipEnum.Settings);

        if (GameSettingMenu.Instance)
        {
            GameSettingMenu.Instance.Close();
            return;
        }

        SettingsPatches.SettingsPage = 3;
        CustomPlayer.Local.NetTransform.Halt();
        var currentMenu = Instantiate(Prefab);
        currentMenu.transform.SetParent(Camera.main.transform, false);
        currentMenu.transform.localPosition = Pos;
        currentMenu.name = "ClientOptionsMenu";
        TransitionFade.Instance.DoTransitionFade(null, currentMenu.gameObject, null);
    }

    public static void OpenWiki()
    {
        Instance.CloseMenus(SkipEnum.Wiki);

        if (LocalBlocked())
            return;

        if (!Instance.PagesSet)
        {
            var clone = Modules.Info.AllInfo.Clone();
            clone.RemoveAll(x => x.Name is "Invalid" or "None" || x.Type == InfoType.Lore);
            var i = 0;
            var j = 0;
            var k = 0;

            foreach (var pair in clone)
            {
                Instance.Sorted.Add(j, new(pair is SymbolInfo symbol ? symbol.Symbol : pair.Name, pair));
                j++;
                k++;

                if (k >= 28 || (pair.Footer && pair != clone[^1]))
                {
                    i++;
                    k = 0;
                }
            }

            Instance.MaxPage = i;
            Instance.PagesSet = true;
        }

        if (!Instance.NextButton)
        {
            Instance.NextButton = Instance.CreateButton("WikiNextButton", "Next Page", () =>
            {
                Instance.Page = CycleInt(Instance.MaxPage, 0, Instance.Page, true);
                ResetButtonPos();
            });
        }

        if (!Instance.BackButton)
        {
            Instance.BackButton = Instance.CreateButton("WikiBack", "Previous Page", () =>
            {
                if (Instance.Selected == null)
                    Instance.Page = CycleInt(Instance.MaxPage, 0, Instance.Page, false);
                else if (Instance.LoreActive)
                {
                    Instance.PhoneText.gameObject.SetActive(false);
                    Instance.AddInfo();
                    Instance.LoreActive = false;
                }
                else
                {
                    Instance.Selected = null;
                    Instance.SelectionActive = false;
                    Instance.LoreButton.gameObject.SetActive(false);
                    Instance.NextButton.gameObject.SetActive(true);
                    Instance.NextButton.transform.localPosition = new(2.5f, 1.6f, 0f);
                    Instance.PhoneText.gameObject.SetActive(false);
                    Instance.Entry.Clear();
                }

                ResetButtonPos();
            });
        }

        if (!Instance.YourStatus)
        {
            Instance.YourStatus = Instance.CreateButton("YourStatus", "Your Status", () =>
            {
                OpenWiki();
                Instance.OpenRoleCard();
            });
        }

        if (!Instance.LoreButton)
        {
            Instance.LoreButton = Instance.CreateButton("WikiLore", "Lore", () =>
            {
                Instance.LoreActive = !Instance.LoreActive;
                Instance.SetEntryText(Modules.Info.ColorIt(WrapText(LayerInfo.AllLore.Find(x => x.Name == Instance.Selected.Name || x.Short == Instance.Selected.Short).Description)));
                Instance.PhoneText.text = Instance.Entry[0];
                Instance.PhoneText.transform.localPosition = new(-2.6f, 0.45f, -5f);
                Instance.SelectionActive = true;
            });
        }

        if (Instance.Buttons.Count == 0)
        {
            var i = 0;
            var j = 0;

            for (var k = 0; k < Instance.Sorted.Count; k++)
            {
                if (!Instance.Buttons.ContainsKey(i))
                    Instance.Buttons.Add(i, []);

                var cache = Instance.Sorted[k].Value;
                var cache2 = Instance.Sorted[k].Key;
                var button = Instance.CreateButton($"{cache2}Info", cache2, () =>
                {
                    foreach (var buttons in Instance.Buttons.Values)
                    {
                        if (buttons.Any())
                            buttons.Select(x => x.Item2).ForEach(x => x?.gameObject?.SetActive(false));
                    }

                    Instance.Selected = cache;
                    Instance.NextButton.gameObject.SetActive(false);
                    Instance.AddInfo();
                }, cache.Color);

                Instance.Buttons[i].Add((cache, button));
                j++;

                if (j >= 28 || cache.Footer)
                {
                    i++;
                    j = 0;
                }
            }

            Instance.Buttons.ToList().ForEach(x =>
            {
                if (!x.Value.Any())
                    Instance.Buttons.Remove(x.Key);
            });
        }

        Instance.WikiActive = !Instance.WikiActive;
        Instance.Phone.gameObject.SetActive(Instance.WikiActive);
        Instance.NextButton.gameObject.SetActive(Instance.WikiActive);
        Instance.BackButton.gameObject.SetActive(Instance.WikiActive);
        Instance.YourStatus.gameObject.SetActive(Instance.WikiActive && IsNormal() && IsInGame());
        ResetButtonPos();
        Instance.Selected = null;

        if (!Instance.WikiActive && Instance.PhoneText)
            Instance.PhoneText.gameObject.SetActive(false);
    }

    public static void ResetButtonPos()
    {
        if (Instance.BackButton)
            Instance.BackButton.transform.localPosition = new(-2.6f, 1.6f, 0f);

        if (Instance.NextButton)
            Instance.NextButton.transform.localPosition = new(2.5f, 1.6f, 0f);

        if (Instance.YourStatus)
            Instance.YourStatus.transform.localPosition = new(0f, 1.6f, 0f);

        if (Instance.ToTheWiki)
            Instance.ToTheWiki.transform.localPosition = new(-2.6f, 1.6f, 0f);

        if (Instance.Selected != null)
        {
            if (LayerInfo.AllLore.Any(x => x.Name == Instance.Selected.Name || x.Short == Instance.Selected.Short) && Instance.LoreButton)
            {
                Instance.LoreButton.gameObject.SetActive(!Instance.LoreActive);
                Instance.LoreButton.transform.localPosition = new(0f, -1.7f, 0f);
            }

            return;
        }

        var m = 0;

        foreach (var pair in Instance.Buttons)
        {
            foreach (var pair2 in pair.Value)
            {
                var button = pair2.Item2;

                if (!button)
                    continue;

                var row = m / 4;
                var col = m % 4;
                button.transform.localPosition = new(-2.6f + (1.7f * col), 1f - (0.45f * row), -1f);
                button.gameObject.SetActive(Instance.Page == pair.Key && Instance.WikiActive);
                m++;

                if (m >= 28 || pair2.Item1.Footer)
                    m = 0;
            }
        }
    }

    public void AddInfo()
    {
        if (PhoneText)
            PhoneText.gameObject.Destroy();

        Selected.WikiEntry(out var result);
        SetEntryText(result);
        PhoneText = Instantiate(HUD().TaskPanel.taskText, Phone.transform);
        PhoneText.color = UColor.white;
        PhoneText.text = Entry[0];
        PhoneText.enableWordWrapping = false;
        PhoneText.transform.localScale = Vector3.one * 0.75f;
        PhoneText.transform.localPosition = new(-2.6f, 0.45f, -5f);
        PhoneText.alignment = TextAlignmentOptions.TopLeft;
        PhoneText.fontStyle = FontStyles.Bold;
        PhoneText.gameObject.SetActive(true);
        PhoneText.name = "PhoneText";
        SelectionActive = true;
    }

    [HideFromIl2Cpp]
    private PassiveButton CreateButton(string name, string labelText, Action onClick, UColor? textColor = null)
    {
        var button = Instantiate(HUD().MapButton, Phone.transform);
        button.name = $"{name}Button";
        button.transform.localScale = new(0.5f, 0.5f, 1f);
        button.GetComponent<BoxCollider2D>().size = new(2.5f, 0.55f);
        var label = Instantiate(HUD().TaskPanel.taskText, button.transform);
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

    public void CloseMenus(SkipEnum skip = SkipEnum.None)
    {
        if (WikiActive && skip != SkipEnum.Wiki)
            OpenWiki();

        if (RoleCardActive && skip != SkipEnum.RoleCard)
            OpenRoleCard();

        if (Zooming && skip != SkipEnum.Zooming)
            ClickZoom();

        if (MapPatch.MapActive && Map() && skip != SkipEnum.Map)
            Map().Close();

        if (ActiveTask() && skip != SkipEnum.Task)
            ActiveTask().Close();

        if (GameSettingMenu.Instance && skip != SkipEnum.Settings)
            GameSettingMenu.Instance.Close();
    }

    public void SetEntryText(string result)
    {
        Entry.Clear();
        ResultPage = 0;
        var texts = result.Split('\n');
        var pos = 0;
        var result2 = "";

        foreach (var text in texts)
        {
            result2 += $"{text}\n";
            pos++;

            if (pos >= 19)
            {
                Entry.Add(result2);
                result2 = "";
                pos -= 19;
            }
        }

        if (!IsNullEmptyOrWhiteSpace(result2))
            Entry.Add(result2);
    }
}