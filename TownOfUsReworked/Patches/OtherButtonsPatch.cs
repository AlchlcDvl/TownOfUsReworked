namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class OtherButtonsPatch
{
    private static GameObject ZoomButton;
    public static bool Zooming;
    private static Vector3 Pos;

    private static SpriteRenderer Phone;
    private static TextMeshPro PhoneText;

    private static GameObject RoleCardButton;
    public static bool RoleCardActive;
    private static Vector3 Pos2;
    private static Transform ToTheWiki;

    private static GameObject SettingsButton;
    public static bool SettingsActive;
    private static Vector3 Pos3;

    private static GameObject WikiButton;
    public static bool WikiActive;
    private static Vector3 Pos4;
    private static Transform NextButton;
    private static Transform BackButton;
    private static Transform LoreButton;
    private static Transform YourStatus;
    private static readonly Dictionary<int, List<Transform>> Buttons = new();
    private static readonly Dictionary<int, KeyValuePair<string, Info>> Sorted = new();
    private static int Page;
    private static int ResultPage;
    private static int MaxPage;
    private static bool PagesSet;
    private static Info Selected;
    private static bool LoreActive;
    private static bool SelectionActive;
    private static readonly List<string> Entry = new();
    //Max page line limit is 20, keeping this in mind for now

    private static Vector3 MapPos;
    private static bool MapModified;

    public static float Size => Zooming ? 4f : 1f;

    public static void Postfix(HudManager __instance)
    {
        try
        {
            if (IsHnS)
                return;

            __instance.GameSettings.text = SettingsPatches.Settings();

            if (__instance.TaskPanel)
                __instance.TaskPanel.gameObject.SetActive(!RoleCardActive && !SettingsActive && !Zooming && !Meeting && !(Map && Map.IsOpen) && !WikiActive && !IsCustomHnS);

            MapPos = __instance.SettingsButton.transform.localPosition + new Vector3(0, -0.66f, -__instance.SettingsButton.transform.localPosition.z - 51f);

            if (!MapModified)
            {
                MapModified = true;
                __instance.MapButton.gameObject.GetComponent<PassiveButton>().OnClick.AddListener(new Action(CloseMenus));
            }

            if (!WikiButton)
            {
                WikiButton = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                WikiButton.GetComponent<SpriteRenderer>().sprite = GetSprite("Wiki");
                WikiButton.GetComponent<PassiveButton>().OnClick = new();
                WikiButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(OpenWiki));
                WikiButton.name = "WikiButton";
            }

            WikiButton.SetActive(!IntroCutscene.Instance && !IsFreePlay);
            WikiButton.transform.localPosition = MapPos;
            ResetButtonPos();

            Pos = MapPos + new Vector3(0, -0.66f, 0f);
            __instance.MapButton.transform.localPosition = Pos;

            if (IsSubmerged())
            {
                var floorButton = __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)");

                if (floorButton && floorButton.gameObject.active)
                {
                    Pos += new Vector3(0, -0.66f, 0f);
                    floorButton.localPosition = Pos;
                }
            }

            if (!SettingsButton)
            {
                SettingsButton = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                SettingsButton.GetComponent<SpriteRenderer>().sprite = GetSprite("CurrentSettings");
                SettingsButton.GetComponent<PassiveButton>().OnClick = new();
                SettingsButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(OpenSettings));
                SettingsButton.name = "CustomSettingsButton";
            }

            Pos2 = Pos + new Vector3(0, -0.66f, 0f);
            SettingsButton.SetActive(__instance.MapButton.gameObject.active && !IntroCutscene.Instance && IsNormal && !IsFreePlay);
            SettingsButton.transform.localPosition = Pos2;

            if (!RoleCardButton)
            {
                RoleCardButton = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                RoleCardButton.GetComponent<SpriteRenderer>().sprite = GetSprite("Help");
                RoleCardButton.GetComponent<PassiveButton>().OnClick = new();
                RoleCardButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(OpenRoleCard));
                RoleCardButton.name = "RoleCardButton";
            }

            Pos3 = Pos2 + new Vector3(0, -0.66f, 0f);
            RoleCardButton.SetActive(__instance.MapButton.gameObject.active && IsNormal && !IntroCutscene.Instance && !IsFreePlay);
            RoleCardButton.transform.localPosition = Pos3;

            if (!ZoomButton)
            {
                ZoomButton = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                ZoomButton.GetComponent<PassiveButton>().OnClick = new();
                ZoomButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(ClickZoom));
                ZoomButton.name = "ZoomButton";
            }

            Pos4 = Pos3 + new Vector3(0, -0.66f, 0f);
            ZoomButton.SetActive(__instance.MapButton.gameObject.active && IsNormal && CustomPlayer.LocalCustom.IsDead && !IntroCutscene.Instance && (!CustomPlayer.Local.IsPostmortal() ||
                (CustomPlayer.Local.IsPostmortal() && CustomPlayer.Local.Caught())) && !IsFreePlay);
            ZoomButton.transform.localPosition = Pos4;
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
        } catch {}
    }

    public static void Zoom()
    {
        Zooming = !Zooming;

        if (!Zooming)
            return;

        Camera.main.orthographicSize = 3f * Size;

        foreach (var cam in Camera.allCameras)
        {
            if (cam?.gameObject.name == "UI Camera")
                cam.orthographicSize = 3f * Size;
        }

        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
    }

    private static void ClickZoom()
    {
        if (WikiActive)
            OpenWiki();

        if (RoleCardActive)
            OpenRoleCard();

        if (SettingsActive)
            OpenSettings();

        if (Map)
            Map.Close();

        if (!Meeting)
            Zoom();
    }

    public static void OpenSettings()
    {
        if (WikiActive)
            OpenWiki();

        if (RoleCardActive)
            OpenRoleCard();

        if (Zooming)
            Zoom();

        if (Map)
            Map.Close();

        if (LocalBlocked)
            return;

        SettingsActive = !SettingsActive;
        HUD.GameSettings.gameObject.SetActive(SettingsActive);
    }

    public static void OpenRoleCard()
    {
        if (WikiActive)
            OpenWiki();

        if (Zooming)
            Zoom();

        if (SettingsActive)
            OpenSettings();

        if (Map)
            Map.Close();

        if (LocalBlocked)
            return;

        if (!Phone)
        {
            Phone = new GameObject("Phone") { layer = 5 }.AddComponent<SpriteRenderer>();
            Phone.sprite = GetSprite("Phone");
            Phone.transform.SetParent(HUD.transform);
            Phone.transform.localPosition = new(0, 0, -49f);
            Phone.transform.localScale *= 1.25f;
        }

        if (PhoneText)
            PhoneText.gameObject.Destroy();

        PhoneText = UObject.Instantiate(HUD.KillButton.cooldownTimerText, Phone.transform);
        PhoneText.enableWordWrapping = false;
        PhoneText.transform.localScale = Vector3.one * 0.4f;
        PhoneText.transform.localPosition = new(0, 0, -50f);
        PhoneText.gameObject.layer = 5;
        PhoneText.alignment = TextAlignmentOptions.Center;
        PhoneText.name = "PhoneText";

        if (ToTheWiki == null)
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
        ToTheWiki.gameObject.SetActive(RoleCardActive && IsNormal && IsInGame);
    }

    public static void OpenWiki()
    {
        if (RoleCardActive)
            OpenRoleCard();

        if (Zooming)
            Zoom();

        if (SettingsActive)
            OpenSettings();

        if (Map)
            Map.Close();

        if (LocalBlocked)
            return;

        if (!Phone)
        {
            Phone = new GameObject("Phone") { layer = 5 }.AddComponent<SpriteRenderer>();
            Phone.sprite = GetSprite("Phone");
            Phone.transform.SetParent(HUD.transform);
            Phone.transform.localPosition = new(0, 0, -49f);
            Phone.transform.localScale *= 1.25f;
        }

        if (!PagesSet)
        {
            var clone = Info.AllInfo;
            var keys = new List<Info>();

            foreach (var info in clone)
            {
                if (info.Name is "Invalid" or "None" || info.Type == InfoType.Lore)
                    keys.Add(info);
            }

            clone.RemoveAll(keys.Contains);
            var i = 0;
            var j = 0;
            var k = 0;

            foreach (var pair in clone)
            {
                Sorted.Add(j, new(pair.Name, pair));
                j++;
                k++;

                if (k >= 28)
                {
                    i++;
                    k -= 28;
                }
            }

            MaxPage = i;
            PagesSet = true;
        }

        if (NextButton == null)
        {
            NextButton = CreateButton("WikiNextButton", "Next Page", () =>
            {
                Page = CycleInt(MaxPage, 0, Page, true);
                ResetButtonPos();
            });
        }

        if (BackButton == null)
        {
            BackButton = CreateButton("WikiBack", "Previous Page", () =>
            {
                if (Selected == null)
                    Page = CycleInt(MaxPage, 0, Page, false);
                else if (LoreActive)
                {
                    DisableText();
                    AddInfo();
                    LoreActive = false;
                }
                else
                {
                    Selected = null;
                    SelectionActive = false;
                    LoreButton.gameObject.SetActive(false);
                    NextButton.gameObject.SetActive(true);
                    NextButton.localPosition = new(2.5f, 1.6f, 0f);
                    DisableText();
                    Entry.Clear();
                }

                ResetButtonPos();
            });
        }

        if (YourStatus == null)
        {
            YourStatus = CreateButton("YourStatus", "Your Status", () =>
            {
                OpenWiki();
                OpenRoleCard();
            });
        }

        if (LoreButton == null)
        {
            LoreButton = CreateButton("WikiLore", "Lore", () =>
            {
                LoreActive = !LoreActive;
                SetEntryText(Info.ColorIt(WrapText(LayerInfo.AllLore.Find(x => x.Name == Selected.Name || x.Short == Selected.Short).Description)));
                PhoneText.text = Entry[0];
                PhoneText.transform.localPosition = new(-2.6f, 0.45f, -5f);
                SelectionActive = true;
            });
        }

        if (Buttons.Count == 0)
        {
            var i = 0;
            var j = 0;

            for (var k = 0; k < Sorted.Count; k++)
            {
                if (!Buttons.ContainsKey(i))
                    Buttons.Add(i, new());

                var cache = Sorted[k].Value;
                var button = CreateButton($"{Sorted[k].Key}Info", Sorted[k].Key, () =>
                {
                    foreach (var buttons in Buttons.Values)
                    {
                        if (buttons.Count > 0)
                            buttons.ForEach(x => x?.gameObject?.SetActive(false));
                    }

                    Selected = cache;
                    NextButton.gameObject.SetActive(false);
                    AddInfo();
                }, Sorted[k].Value.Color);

                Buttons[i].Add(button);
                j++;

                if (j >= 28)
                {
                    i++;
                    j -= 28;
                }
            }
        }

        WikiActive = !WikiActive;
        Phone.gameObject.SetActive(WikiActive);
        NextButton.gameObject.SetActive(WikiActive);
        BackButton.gameObject.SetActive(WikiActive);
        YourStatus.gameObject.SetActive(WikiActive && IsNormal && IsInGame);
        ResetButtonPos();
        Selected = null;

        if (!WikiActive && PhoneText)
            DisableText();
    }

    private static void ResetButtonPos()
    {
        if (BackButton != null)
            BackButton.localPosition = new(-2.6f, 1.6f, 0f);

        if (NextButton != null)
            NextButton.localPosition = new(2.5f, 1.6f, 0f);

        if (YourStatus != null)
            YourStatus.localPosition = new(0f, 1.6f, 0f);

        if (ToTheWiki != null)
            ToTheWiki.localPosition = new(-2.6f, 1.6f, 0f);

        if (Selected != null)
        {
            if (LayerInfo.AllLore.Any(x => x.Name == Selected.Name || x.Short == Selected.Short) && LoreButton != null)
            {
                LoreButton.gameObject.SetActive(!LoreActive);
                LoreButton.localPosition = new(0f, -1.7f, 0f);
            }

            return;
        }

        var m = 0;

        foreach (var pair in Buttons)
        {
            foreach (var button in pair.Value)
            {
                if (button == null)
                    continue;

                var row = m / 4;
                var col = m % 4;
                button.localPosition = new(-2.6f + (1.7f * col), 1f - (0.45f * row), -1f);
                button.gameObject.SetActive(Page == pair.Key && WikiActive);
                m++;

                if (m >= 28)
                    m -= 28;
            }
        }
    }

    private static void AddInfo()
    {
        if (PhoneText)
            PhoneText.gameObject.Destroy();

        Selected.WikiEntry(out var result);
        SetEntryText(result);
        PhoneText = UObject.Instantiate(HUD.TaskPanel.taskText, Phone.transform);
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

    private static void DisableText() => PhoneText.gameObject.SetActive(false);

    private static Transform CreateButton(string name, string labelText, Action onClick, Color? textColor = null)
    {
        var button = UObject.Instantiate(HUD.MapButton.transform, Phone.transform);
        button.name = $"{name}Button";
        button.localScale = new(0.5f, 0.5f, 1f);
        button.GetComponent<BoxCollider2D>().size = new(2.5f, 0.55f);
        var label = UObject.Instantiate(HUD.TaskPanel.taskText, button);
        label.color = textColor ?? UColor.white;
        label.text = labelText;
        label.enableWordWrapping = false;
        label.transform.localPosition = new(0f, 0f, label.transform.localPosition.z);
        label.transform.localScale *= 1.55f;
        label.alignment = TextAlignmentOptions.Center;
        label.fontStyle = FontStyles.Bold;
        label.name = $"{name}Text";
        var rend = button.GetComponent<SpriteRenderer>();
        rend.sprite = GetSprite("Plate");
        var passive = button.GetComponent<PassiveButton>();
        passive.OnClick = new();
        passive.OnClick.AddListener(onClick);
        passive.OnMouseOver = new();
        passive.OnMouseOver.AddListener((Action)(() => rend.color = UColor.yellow));
        passive.OnMouseOut = new();
        passive.OnMouseOut.AddListener((Action)(() => rend.color = UColor.white));
        return button;
    }

    public static void CloseMenus()
    {
        if (WikiActive)
            OpenWiki();

        if (RoleCardActive)
            OpenRoleCard();

        if (Zooming)
            Zoom();

        if (SettingsActive)
            OpenSettings();

        if (Map)
            Map.Close();
    }

    private static void SetEntryText(string result)
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