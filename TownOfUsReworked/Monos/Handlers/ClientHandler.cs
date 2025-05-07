using static TownOfUsReworked.Monos.HandlerSingleton<TownOfUsReworked.Monos.ClientHandler>;

namespace TownOfUsReworked.Monos;

public sealed class ClientHandler : MonoBehaviour
{
    public PassiveButton ZoomButton;
    public bool Zooming;

    public SpriteRenderer Phone;
    public TextMeshPro PhoneText;

    public PassiveButton WikiRcButton;
    public bool WikiActive;
    public bool RoleCardActive;
    public PassiveButton ToTheWiki;
    public PassiveButton NextButton;
    public PassiveButton BackButton;
    public PassiveButton YourStatus;
    public readonly Dictionary<int, List<(Info, PassiveButton)>> Buttons = [];
    private readonly Dictionary<int, KeyValuePair<string, Info>> Sorted = [];
    public int Page;
    public int ResultPage;
    public int MaxPage;
    public bool PagesSet;
    private Info Selected;
    public bool LoreActive;
    public bool SelectionActive;
    private readonly List<string> Entry = [];
    // The max page line limit is 20

    public PassiveButton ClientOptionsButton;
    public bool SettingsActive;

    private Transform ButtonsParent;

    private ProgressTracker taskBar;
    private ProgressTracker TaskBar
    {
        get
        {
            if (!taskBar)
                taskBar = FindObjectOfType<ProgressTracker>(true);

            return taskBar;
        }
    }

    private static GameObject Prefab;
    private static Vector3 Pos;

    private static Vector3 MaxSize = Vector3.zero;
    private static Vector3 MinSize = Vector3.zero;

    public void OnHudStart(HudManager hud)
    {
        if (MinSize == Vector3.zero)
            MinSize = hud.transform.localScale;

        if (MaxSize == Vector3.zero)
            MaxSize = hud.transform.localScale * 4f;

        if (!ButtonsParent && hud.AbilityButton)
        {
            var obj = hud.AbilityButton.transform.parent;
            ButtonsParent = Instantiate(obj, obj.parent);
            ButtonsParent.name = "BottomLeft";
            ButtonsParent.transform.localPosition = new(-obj.localPosition.x, obj.localPosition.y, obj.localPosition.z);
            var grid = ButtonsParent.GetComponent<GridArrange>();
            grid.Alignment = GridArrange.StartAlign.Right;
            grid.MaxColumns = 3;
            grid.CellSize = new(0.85f, 0.8f);
            ButtonsParent.DestroyChildren();
        }

        if (!ButtonsParent)
            return;

        if (!WikiRcButton)
        {
            WikiRcButton = Instantiate(hud.MapButton, ButtonsParent);
            WikiRcButton.OverrideOnClickListeners(Open);
            WikiRcButton.name = "WikiAndRCButton";
            WikiRcButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("WikiInactive");
            WikiRcButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("WikiActive");
            WikiRcButton.transform.Find("Background").localPosition = Vector3.zero;
        }

        if (!ClientOptionsButton)
        {
            ClientOptionsButton = Instantiate(hud.MapButton, ButtonsParent);
            ClientOptionsButton.OverrideOnClickListeners(CreateMenu);
            ClientOptionsButton.name = "ClientOptionsButton";
            ClientOptionsButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("SettingsInactive");
            ClientOptionsButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("SettingsActive");
            ClientOptionsButton.transform.Find("Background").localPosition = Vector3.zero;
        }

        if (!ZoomButton)
        {
            ZoomButton = Instantiate(hud.MapButton, ButtonsParent);
            ZoomButton.OverrideOnClickListeners(ClickZoom);
            ZoomButton.name = "ZoomButton";
            ZoomButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("MinusInactive");
            ZoomButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("MinusActive");
            ZoomButton.transform.Find("Background").localPosition = Vector3.zero;
        }
    }

    public static void OnLobbyStart()
    {
        Instance.OnHudStart(HUD());

        if (!Prefab)
        {
            Prefab = GameStartManager.Instance.PlayerOptionsMenu;
            Pos = GameStartManager.Instance.GameOptionsPosition;
        }

        var menu = Prefab.GetComponent<GameSettingMenu>();
        var settings = menu.GameSettingsTab;
        var prefabs = new List<MonoBehaviour>();

        if (!SettingsPatches.NumberPrefab)
        {
            // Background = 0, Value Text = 1, Title = 2, - = 3, + = 4, Value Box = 5
            SettingsPatches.NumberPrefab = Instantiate(settings.numberOptionOrigin).DontDestroy();
            SettingsPatches.NumberPrefab.name = "NumberPrefab";

            SettingsPatches.NumberPrefab.MinusBtn.transform.localPosition += new Vector3(0.6f, 0f, 0f);
            SettingsPatches.NumberPrefab.PlusBtn.transform.localPosition += new Vector3(1.5f, 0f, 0f);
            SettingsPatches.NumberPrefab.PlusBtn.OverrideOnClickListeners(BlankVoid);
            SettingsPatches.NumberPrefab.MinusBtn.OverrideOnClickListeners(BlankVoid);

            var background = SettingsPatches.NumberPrefab.transform.GetChild(0);
            background.localPosition += new Vector3(-0.8f, 0f, 0f);
            background.localScale += new Vector3(1f, 0f, 0f);

            var title = SettingsPatches.NumberPrefab.TitleText;
            title.transform.localPosition = new(-2.0466f, 0f, -2.9968f);
            title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);

            var valueBox = SettingsPatches.NumberPrefab.transform.GetChild(5);
            valueBox.localPosition += new Vector3(1.05f, 0f, 0f);
            valueBox.localScale += new Vector3(0.2f, 0f, 0f);

            prefabs.Add(SettingsPatches.NumberPrefab);
        }

        if (!SettingsPatches.StringPrefab)
        {
            // Background = 0, Value Text = 1, Title = 2, < = 3, > = 4, Value Box = 5
            SettingsPatches.StringPrefab = Instantiate(settings.stringOptionOrigin).DontDestroy();
            SettingsPatches.StringPrefab.name = "StringPrefab";

            var background = SettingsPatches.StringPrefab.transform.GetChild(0);
            background.localPosition += new Vector3(-0.8f, 0f, 0f);
            background.localScale += new Vector3(1f, 0f, 0f);

            var title = SettingsPatches.StringPrefab.TitleText;
            title.transform.localPosition = new(-2.0466f, 0f, -2.9968f);
            title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);
            title.fontSize = 2.9f; // Why is it different for string options??

            var minus = SettingsPatches.StringPrefab.MinusBtn;
            minus.ChangeButtonText("<");
            minus.transform.localPosition += new Vector3(0.6f, 0f, 0f);
            minus.OverrideOnClickListeners(BlankVoid);

            var plus = SettingsPatches.StringPrefab.PlusBtn;
            plus.ChangeButtonText(">");
            plus.transform.localPosition += new Vector3(1.5f, 0f, 0f);
            plus.OverrideOnClickListeners(BlankVoid);

            var valueBox = SettingsPatches.StringPrefab.transform.GetChild(5);
            valueBox.localPosition += new Vector3(1.05f, 0f, 0f);
            valueBox.localScale += new Vector3(0.2f, 0f, 0f);

            prefabs.Add(SettingsPatches.StringPrefab);
        }

        if (!SettingsPatches.TogglePrefab)
        {
            // Title = 0, Toggle = 1, Background = 2
            SettingsPatches.TogglePrefab = Instantiate(settings.checkboxOrigin).DontDestroy();
            SettingsPatches.TogglePrefab.name = "TogglePrefab";
            SettingsPatches.TogglePrefab.transform.GetChild(1).localPosition += new Vector3(2.2f, 0f, 0f);

            var title = SettingsPatches.TogglePrefab.TitleText;
            title.transform.localPosition = new(-2.0466f, 0f, -2.9968f);
            title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);

            var background = SettingsPatches.TogglePrefab.transform.GetChild(2);
            background.localPosition += new Vector3(-0.8f, 0f, 0f);
            background.localScale += new Vector3(1f, 0f, 0f);

            prefabs.Add(SettingsPatches.TogglePrefab);
        }

        if (!SettingsPatches.MultiSelectPrefab)
        {
            // Background = 0, Value Text = 1, Title = 2, < = 3, > = 4, Value Box = 5, Button = 6
            SettingsPatches.MultiSelectPrefab = Instantiate(SettingsPatches.StringPrefab, null).DontDestroy();
            SettingsPatches.MultiSelectPrefab.name = "MultiSelectPrefab";
            SettingsPatches.MultiSelectPrefab.PlusBtn.gameObject.SetActive(false);
            SettingsPatches.MultiSelectPrefab.MinusBtn.gameObject.SetActive(false);

            var toggle = Instantiate(SettingsPatches.TogglePrefab.GetComponentInChildren<PassiveButton>(), SettingsPatches.MultiSelectPrefab.transform);
            toggle.name = "Button";
            toggle.transform.DestroyChildren();
            toggle.OverrideOnClickListeners(BlankVoid);

            var box = toggle.GetComponent<BoxCollider2D>();
            var prevColliderSize = box.size;
            prevColliderSize.x *= 4.97f;
            box.size = prevColliderSize;

            prefabs.Add(SettingsPatches.MultiSelectPrefab);
        }

        if (!SettingsPatches.MultiOptionPrefab)
        {
            // PassiveButton = 0, Text = 1, Box = 2
            SettingsPatches.MultiOptionPrefab = new GameObject("MultiSelectOptionPrefab").DontDestroy().AddComponent<BlankBehaviour>();

            var toggle = Instantiate(SettingsPatches.MultiSelectPrefab.transform.GetChild(6).GetComponent<PassiveButton>(), Vector3.zero, Quaternion.identity,
                SettingsPatches.MultiOptionPrefab.transform);
            toggle.name = "Button";
            toggle.transform.DestroyChildren();
            toggle.OverrideOnClickListeners(BlankVoid);

            var collider = toggle.GetComponent<BoxCollider2D>();
            collider.offset = Vector3.zero;
            collider.size = new(1.8f, 0.5292f);

            var text = Instantiate(SettingsPatches.MultiSelectPrefab.ValueText, Vector3.zero, Quaternion.identity, SettingsPatches.MultiOptionPrefab.transform);
            text.transform.localScale = new(0.6f, 0.6f, 1f);
            text.text = "ValueText";
            text.GetComponent<RectTransform>().sizeDelta = new(2.75f, 0.4f);

            var box = Instantiate(SettingsPatches.MultiSelectPrefab.transform.GetChild(5), Vector3.zero, Quaternion.identity, SettingsPatches.MultiOptionPrefab.transform);
            box.localScale = new(0.5f, 0.5f, 1f);
            box.name = "ValueBox";

            prefabs.Add(SettingsPatches.MultiOptionPrefab);
        }

        if (!SettingsPatches.HeaderPrefab)
        {
            SettingsPatches.HeaderPrefab = Instantiate(settings.categoryHeaderOrigin).DontDestroy();
            SettingsPatches.HeaderPrefab.name = "HeaderPrefab";
            SettingsPatches.HeaderPrefab.transform.localScale = Vector3.one * 0.63f;
            SettingsPatches.HeaderPrefab.Background.transform.localScale += new Vector3(0.7f, 0f, 0f);

            SettingsPatches.HeaderPrefab.transform.GetChild(1).gameObject.SetActive(false);

            var newButton = Instantiate(SettingsPatches.StringPrefab.PlusBtn, SettingsPatches.HeaderPrefab.transform);
            newButton.transform.localScale *= 0.7f;
            newButton.transform.localPosition = new(3.2f, -0.18f, 0f);
            newButton.OverrideOnClickListeners(BlankVoid);
            newButton.name = "Collapse";

            prefabs.Add(SettingsPatches.HeaderPrefab);
        }

        var roles = menu.RoleSettingsTab;

        if (!SettingsPatches.LayersPrefab)
        {
            // Title = 0, Role # = 1, Chance % = 2, Background = 3, Divider = 4, Cog = 5, Unique = 6, Active = 7
            //            ┗-----------┗----------- Value = 0, - = 1, + = 2, Value Box = 3 ┗-----------┗--------- Checkbox = 0
            SettingsPatches.LayersPrefab = Instantiate(roles.roleOptionSettingOrigin).DontDestroy();
            SettingsPatches.LayersPrefab.name = "LayersPrefab";
            SettingsPatches.LayersPrefab.titleText.alignment = TextAlignmentOptions.Left;
            SettingsPatches.LayersPrefab.role = null;
            SettingsPatches.LayersPrefab.transform.GetChild(0).localPosition += new Vector3(-0.1f, 0f, 0f);

            SettingsPatches.LayersPrefab.CountMinusBtn.OverrideOnClickListeners(BlankVoid);
            SettingsPatches.LayersPrefab.CountPlusBtn.OverrideOnClickListeners(BlankVoid);
            SettingsPatches.LayersPrefab.ChanceMinusBtn.OverrideOnClickListeners(BlankVoid);
            SettingsPatches.LayersPrefab.ChancePlusBtn.OverrideOnClickListeners(BlankVoid);

            var label = SettingsPatches.LayersPrefab.transform.GetChild(3);
            label.localScale += new Vector3(0.001f, 0f, 0f); // WHY THE FUCK IS THE BACKGROUND EVER SO SLIGHTLY SMALLER THAN THE HEADER?!
            label.localPosition = new(-0.3998f, -0.2953f, 4f);

            var newButton = Instantiate(SettingsPatches.LayersPrefab.CountMinusBtn, SettingsPatches.LayersPrefab.transform);
            newButton.name = "LayerSubSettingsButton";
            newButton.transform.localPosition = new(0.4719f, -0.2982f, -2f);
            newButton.transform.FindChild("Text_TMP").gameObject.Destroy();
            newButton.transform.FindChild("ButtonSprite").GetComponent<SpriteRenderer>().sprite = GetSprite("Cog");
            newButton.OverrideOnClickListeners(BlankVoid);

            var check = settings.checkboxOrigin.transform.GetChild(1);

            var unique = Instantiate(check, SettingsPatches.LayersPrefab.transform);
            unique.name = "Unique";
            unique.GetComponent<PassiveButton>().OverrideOnClickListeners(BlankVoid);
            unique.transform.localScale = new(0.6f, 0.6f, 1f);

            var active = Instantiate(check, SettingsPatches.LayersPrefab.transform);
            active.name = "Active";
            active.GetComponent<PassiveButton>().OverrideOnClickListeners(BlankVoid);
            active.transform.localScale = new(0.6f, 0.6f, 1f);

            if (LayerOption.Left == default)
                LayerOption.Left = SettingsPatches.LayersPrefab.transform.GetChild(1).localPosition;

            if (LayerOption.Right == default)
                LayerOption.Right = SettingsPatches.LayersPrefab.transform.GetChild(2).localPosition;

            if (LayerOption.Diff == default)
                LayerOption.Diff = (LayerOption.Left - LayerOption.Right) / 2;

            prefabs.Add(SettingsPatches.LayersPrefab);
        }

        if (!SettingsPatches.AlignmentPrefab)
        {
            // Header Label = 0, Header Text = 1, Quota Header = 2, Collapse = 3, Cog = 4
            //                                    ┗--------------- Dark Label = 0, Left = 1, Left Label = 2, Right Label = 3, Right = 4, Long Label = 5, Center = 6
            SettingsPatches.AlignmentPrefab = Instantiate(roles.categoryHeaderEditRoleOrigin).DontDestroy();
            SettingsPatches.AlignmentPrefab.name = "AlignmentPrefab";
            SettingsPatches.AlignmentPrefab.transform.GetChild(0).gameObject.SetActive(false);

            var quota = SettingsPatches.AlignmentPrefab.transform.GetChild(2);

            var single = Instantiate(quota.GetChild(3), quota);
            single.localScale += new Vector3(0.5f, 0f, 0f);
            single.localPosition += new Vector3(-0.956f, 0f, 0f);
            single.name = "SingleBG";

            var text = quota.GetChild(1);
            text.GetComponent<TextTranslatorTMP>().Destroy();

            var center = Instantiate(text, quota);
            center.name = "Center";
            center.GetComponent<TextTranslatorTMP>().Destroy();

            var newButton = Instantiate(SettingsPatches.LayersPrefab.CountMinusBtn, SettingsPatches.AlignmentPrefab.transform);
            newButton.name = "Collapse";
            newButton.transform.localPosition = new(-5.839f, -0.45f, -2f);
            newButton.GetComponentInChildren<TextMeshPro>().text = "-";
            newButton.OverrideOnClickListeners(BlankVoid);
            newButton.transform.localScale *= 0.7f;

            var newButton2 = Instantiate(SettingsPatches.LayersPrefab.transform.GetChild(5), SettingsPatches.AlignmentPrefab.transform);
            newButton2.name = "SubOptions";
            newButton2.transform.FindChild("Text_TMP").gameObject.Destroy();
            newButton2.transform.localPosition = new(-5.239f, -0.45f, -2f);
            newButton2.transform.localScale *= 0.7f;

            prefabs.Add(SettingsPatches.AlignmentPrefab);
        }

        if (!SettingsPatches.LayerHeaderPrefab)
        {
            // Using MissingBehaviour because it literally does nothing, and I need a MonoBehaviour reference for the layer header prefabs

            // Label = 0, Title = 1, Info = 2, Desc = 3, Collapse = 4
            //                       ┗---------┗------- Text = 0
            SettingsPatches.LayerHeaderPrefab = new GameObject("LayerHeaderPrefab").DontDestroy().AddComponent<BlankBehaviour>();

            var label = Instantiate(roles.AdvancedRolesSettings.transform.GetChild(2).GetChild(0), SettingsPatches.LayerHeaderPrefab.transform);
            label.localPosition += new Vector3(0.12f, 0.08f, 0f);
            label.name = "Label";

            var title = Instantiate(roles.AdvancedRolesSettings.transform.GetChild(2).GetChild(1), SettingsPatches.LayerHeaderPrefab.transform);
            title.GetComponent<RectTransform>().sizeDelta = new(1.8f, 0.2201f);
            title.localPosition += new Vector3(0f, 0.08f, 0f);
            title.name = "Title";

            var info = Instantiate(roles.AdvancedRolesSettings.transform.GetChild(3), SettingsPatches.LayerHeaderPrefab.transform);
            info.localPosition = new(-5.7f, -0.2f, 0f);
            info.name = "Info";

            var desc = Instantiate(roles.AdvancedRolesSettings.transform.GetChild(4), SettingsPatches.LayerHeaderPrefab.transform);
            desc.localPosition = new(-3.028f, -0.4478f, 0f);
            desc.localScale = new(0.1135f, 0.1494f, 0.5687f);
            desc.name = "Desc";

            var descText = desc.GetChild(0);
            descText.localScale = new(10f, 6.6931f, 1.7584f);
            descText.name = "Text";

            var newButton = Instantiate(SettingsPatches.StringPrefab.PlusBtn, SettingsPatches.LayerHeaderPrefab.transform);
            newButton.transform.localScale *= 0.4f;
            newButton.transform.localPosition = new(-2.1f, 0f, 0f);
            newButton.OverrideOnClickListeners(BlankVoid);
            newButton.name = "Collapse";

            LayerHeaderOption.OgLabel = label.GetComponent<SpriteRenderer>().sprite;
            LayerHeaderOption.OgPosition = label.localPosition;

            prefabs.Add(SettingsPatches.LayerHeaderPrefab);
        }

        foreach (var mono in prefabs)
        {
            foreach (var obj in mono.GetAllComponents<SpriteRenderer>())
            {
                obj.material.SetInt(PlayerMaterial.MaskLayer, 20);
                obj.material.SetFloat(StencilComp, 3f);
                obj.material.SetFloat(Stencil, 20);
                obj.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }

            foreach (var obj in mono.GetAllComponents<TextMeshPro>())
            {
                obj.fontMaterial.SetFloat(StencilComp, 3f);
                obj.fontMaterial.SetFloat(Stencil, 20);
            }

            mono.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if (!HudManager.InstanceExists)
            return;

        var hud = HUD();

        if (Input.GetKeyDown(KeyCode.Escape) && !ActiveTask() && !MapBehaviourPatches.MapActive)
            hud?.SettingsButton?.GetComponent<PassiveButton>()?.OnClick?.Invoke();

        if (IsHnS() || !CustomPlayer.Local || !ButtonsParent)
            return;

        ResetButtonPos();
        var part2 = !ActiveTask()?.TryCast<HauntMenuMinigame>() && !GameSettingMenu.Instance && !PlayerCustomizationMenu.Instance && !Meeting();

        if (WikiRcButton)
            WikiRcButton.gameObject.SetActive(part2);

        if (ClientOptionsButton)
            ClientOptionsButton.gameObject.SetActive(part2 && !RoleCardActive);

        if (hud!.MapButton)
            hud.MapButton.gameObject.SetActive(IsInGame());

        if (ZoomButton)
        {
            ZoomButton.gameObject.SetActive(hud.MapButton.isActiveAndEnabled && CustomPlayer.Local.HasDied() && IsInGame() && part2 && !RoleCardActive && (!CustomPlayer.Local.IsPostmortal() ||
                CustomPlayer.Local.Caught()));
        }

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

        var part = !RoleCardActive && !SettingsActive && !Zooming && !MapBehaviourPatches.MapActive && !WikiActive && !IsCustomHnS() && !GameSettingMenu.Instance;
        hud?.TaskPanel?.gameObject?.SetActive(part && !Meeting() && !IsCustomHnS());
        TaskBar?.gameObject?.SetActive(part && GameSettings.TaskBarMode != TBMode.Invisible);
    }

    public void ClickZoom()
    {
        CloseMenus(SkipEnum.Zooming);
        Zooming = !Zooming;
        Coroutines.Start(Zoom(Zooming));
    }

    [HideFromIl2Cpp]
    private IEnumerator Zoom(bool inOut)
    {
        ZoomButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite($"{(inOut ? "Plus" : "Minus")}Inactive");
        ZoomButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite = GetSprite($"{(inOut ? "Plus" : "Minus")}Active");

        if (!Camera.main)
            yield break;

        var limit = inOut ? 12f : 3f;
        var original = Camera.main.orthographicSize;

        var sizeLimit = inOut ? MaxSize : MinSize;
        var hud = HUD();
        var originalSize = hud.transform.localScale;

        yield return PerformTimedAction(1f, p =>
        {
            var size = Meeting() ? 3f : Mathf.Lerp(original, limit, p);
            Camera.main!.orthographicSize = size;
            hud.UICamera.orthographicSize = size;
            hud.transform.localScale = Vector3.Lerp(originalSize, sizeLimit, p);
        });
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

    private static void CreateMenu()
    {
        Instance.CloseMenus(SkipEnum.Settings);

        if (GameSettingMenu.Instance)
        {
            GameSettingMenu.Instance.Close();
            return;
        }

        CustomPlayer.Local.NetTransform.Halt();
        var currentMenu = Instantiate(Prefab, Camera.main!.transform, false);
        currentMenu.transform.localPosition = Pos;
        currentMenu.name = "ReworkedOptionsMenu";
        TransitionFade.Instance.DoTransitionFade(null, currentMenu.gameObject, null);

        if (IsLobby())
        {
            GameStartManager.Instance?.RulesViewPanel?.SetActive(false);
            GameStartManager.Instance?.SelectViewButton(false);
            GameStartManager.Instance?.LobbyInfoPane?.DeactivatePane();
        }
    }

    private static void OpenWiki()
    {
        Instance.CloseMenus(SkipEnum.Wiki);

        if (LocalBlocked())
            return;

        if (!Instance.PagesSet)
        {
            var clone = Modules.Info.AllInfo.Where(x => !x.ID.ContainsAny("Invalid", "None"));
            var count = clone.Count() - 1;
            var i = 0;
            var j = 0;
            var k = 0;

            foreach (var (l, pair) in clone.Indexed())
            {
                Instance.Sorted.Add(j, new(pair is SymbolInfo symbol ? symbol.Symbol : pair.ID, pair));
                j++;
                k++;

                if (k >= 28 || (pair.Footer && l < count))
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

        if (Instance.Buttons.Count == 0)
        {
            var i = 0;
            var j = 0;

            for (var k = 0; k < Instance.Sorted.Count; k++)
            {
                if (!Instance.Buttons.TryGetValue(i, out var buttons))
                    Instance.Buttons[i] = buttons = [];

                var cache = Instance.Sorted[k].Value;
                var cache2 = Instance.Sorted[k].Key;
                var button = Instance.CreateButton($"{cache2}Info", cache2, () =>
                {
                    Instance.Buttons.Values.Where(tuples => tuples.Any()).Do(list => list.Select(x => x.Item2).Do(x => x?.gameObject?.SetActive(false)));
                    Instance.Selected = cache;
                    Instance.NextButton.gameObject.SetActive(false);
                    Instance.AddInfo();
                }, cache.Color);
                buttons.Add((cache, button));
                j++;

                if (j < 28 && !cache.Footer)
                    continue;

                i++;
                j = 0;
            }

            Instance.Buttons.Do(x =>
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

    private static void ResetButtonPos()
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
            return;

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

        SetEntryText(Selected.WikiEntry());
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
    private PassiveButton CreateButton(string buttonName, string labelText, Action onClick, UColor? textColor = null)
    {
        var button = Instantiate(HUD().MapButton, Phone.transform);
        button.name = $"{buttonName}Button";
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
        label.name = $"{buttonName}Text";
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

        if (MapBehaviourPatches.MapActive && Map() && skip != SkipEnum.Map)
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
        var texts = result.TrueSplit('\n');
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