using Assets.InnerNet;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MainMenuManager))]
public static class MainMenuPatches
{
    private static AnnouncementPopUp PopUp;
    private static readonly Announcement ModInfo = new()
    {
        Id = "tourewInfo",
        Language = 0,
        Number = 500,
        Title = "Town Of Us Reworked Info",
        ShortTitle = "Mod Info",
        SubTitle = "",
        PinState = true,
        Date = "29.10.2024",
        Text = $"<size=75%>{GetString("ModInfo")}</size>"
    };
    public static GameObject Logo;

    [HarmonyPatch(nameof(MainMenuManager.Start))]
    public static void Prefix()
    {
        LoadVanillaSounds();
        AllMonos.AddComponents();
        CachedFirstDead = null;
        var rightPanel = GameObject.Find("RightPanel");

        if (!Logo && rightPanel)
        {
            Logo = new GameObject("ReworkedLogo");
            Logo.transform.position = new(2f, 0f, 100f);
            var rend = Logo.AddComponent<SpriteRenderer>();
            rend.sprite = GetSprite("Banner");
            var former = rend.materials.ToArray().ToList();
            former.Add(UnityGet<Material>("GlitchedPlayer"));
            rend.SetMaterialArray(former.ToArray());
            Logo.transform.SetParent(rightPanel.transform);
        }

        var y = -0.5f;
        var pos = 0.75f;

        // If there's a possible download, create and show the buttons for it
        if (ModUpdater.ReworkedUpdate)
        {
            Info("Reworked can be updated");
            CreatDownloadButton("Reworked", y, pos, "UpdateReworked");
            y += 0.5f;
            pos += 0.5f;
        }

        if (ModUpdater.SubmergedUpdate || ModUpdater.CanDownloadSubmerged)
        {
            Info($"Submerged can be {(ModUpdater.SubmergedUpdate ? "updated" : "downloaded")}");
            CreatDownloadButton("Submerged", y, pos, $"{(SubLoaded ? "Update" : "Download")}Submerged");
            y += 0.5f;
            pos += 0.5f;
        }

        if (ModUpdater.CanDownloadLevelImpostor)
        {
            Info("LevelImpostor can be downloaded");
            CreatDownloadButton("LevelImpostor", y, pos, "DownloadLevelImpostor");
        }

        if (ModUpdater.ReworkedUpdate || ModUpdater.SubmergedUpdate)
        {
            var popup = UObject.Instantiate(TwitchManager.Instance.TwitchPopup);
            popup.TextAreaTMP.fontSize *= 0.7f;
            popup.TextAreaTMP.enableAutoSizing = false;
            popup.Show("A mod update is available!");
        }
    }

    private static void CreatDownloadButton(string downloadType, float yValue1, float yValue2, string spriteName)
    {
        var template = GameObject.Find("ExitGameButton");
        var rightPanel = GameObject.Find("RightPanel");

        if (!template || !rightPanel)
            return;

        var button = UObject.Instantiate(template, rightPanel.transform);
        button.transform.localPosition = new(button.transform.localPosition.x, yValue1, button.transform.localPosition.z);
        button.transform.localScale = new(0.44f, 0.84f, 1f);
        button.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = button.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = GetSprite(spriteName);
        button.name = $"{downloadType}Download";

        var pos = button.GetComponent<AspectPosition>();
        pos.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
        pos.DistanceFromEdge = new(1.5f, yValue2, 0f);

        button.GetComponent<PassiveButton>().OverrideOnClickListeners(() =>
        {
            Coroutines.Start(ModUpdater.DownloadUpdate(downloadType));
            button.SetActive(false);
        });

        Coroutines.Start(PerformTimedAction(0.1f, _ =>
        {
            button.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().SetText("");
            pos.AdjustPosition();
        }));
    }

    [HarmonyPatch(nameof(MainMenuManager.Awake)), HarmonyPostfix]
    public static void StartPostfix(MainMenuManager __instance)
    {
        var scale = __instance.newsButton.transform.localScale;
        var pos = __instance.newsButton.transform.position;
        var diff = __instance.newsButton.transform.position.y - __instance.myAccountButton.transform.position.y;
        pos.x = __instance.creditsButton.transform.position.x;
        scale.x /= 2.1f;
        scale.y *= 0.95f;
        __instance.newsButton.transform.localScale = scale;
        __instance.settingsButton.transform.localScale = scale;
        __instance.myAccountButton.transform.localScale = scale;
        __instance.newsButton.transform.position = pos;
        pos.y -= diff;
        __instance.settingsButton.transform.position = pos;
        pos.y -= diff;
        __instance.myAccountButton.transform.position = pos;
        pos.y = __instance.newsButton.transform.localPosition.y;
        pos.x = __instance.quitButton.transform.localPosition.x;
        __instance.newsButton.transform.GetChild(0).localPosition -= new Vector3(0.3f, 0f, 0f);
        __instance.newsButton.buttonText.transform.localScale = new(scale.x * 3.5f, scale.y, scale.z);
        __instance.newsButton.transform.GetChild(1).GetChild(0).localScale = new(1, scale.y / 1.5f, scale.z);
        __instance.newsButton.transform.GetChild(2).GetChild(0).localScale = new(1, scale.y / 1.5f, scale.z);
        __instance.myAccountButton.transform.GetChild(0).localPosition -= new Vector3(0.3f, 0f, 0f);
        __instance.myAccountButton.buttonText.transform.localScale = new(scale.x * 3.5f, scale.y, scale.z);
        __instance.myAccountButton.transform.GetChild(1).GetChild(0).localScale = new(1, scale.y / 1.5f, scale.z);
        __instance.myAccountButton.transform.GetChild(2).GetChild(0).localScale = new(1, scale.y / 1.5f, scale.z);
        __instance.settingsButton.transform.GetChild(0).localPosition -= new Vector3(0.3f, 0f, 0f);
        __instance.settingsButton.buttonText.transform.localScale = new(scale.x * 3.5f, scale.y, scale.z);
        __instance.settingsButton.transform.GetChild(1).GetChild(0).localScale = new(1, scale.y / 1.5f, scale.z);
        __instance.settingsButton.transform.GetChild(2).GetChild(0).localScale = new(1, scale.y / 1.5f, scale.z);

        var ghObj = UObject.Instantiate(__instance.newsButton, __instance.newsButton.transform.parent);
        ghObj.name = "ReworkedGitHub";
        ghObj.OverrideOnClickListeners(() => Application.OpenURL(TownOfUsReworked.GitHubLink));
        ghObj.transform.localPosition = pos;
        ghObj.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = ghObj.transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().sprite = GetSprite("GitHub");
        ghObj.transform.GetChild(3).gameObject.Destroy();
        ghObj.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
        ghObj.buttonText.SetText("GitHub Repo");
        __instance.mainButtons.Add(ghObj);
        __instance.newsButton.ControllerNav.selectOnRight = ghObj;
        ghObj.ControllerNav.selectOnLeft = __instance.newsButton;
        ghObj.ControllerNav.selectOnUp = __instance.shopButton;
        pos.y = __instance.settingsButton.transform.localPosition.y;

        var discObj = UObject.Instantiate(__instance.settingsButton, __instance.settingsButton.transform.parent);
        discObj.name = "ReworkedDiscord";
        discObj.OverrideOnClickListeners(() => Application.OpenURL(TownOfUsReworked.DiscordInvite));
        discObj.transform.localPosition = pos;
        discObj.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = discObj.transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().sprite = GetSprite("Discord");
        discObj.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
        discObj.buttonText.SetText("Reworked Discord");
        discObj.ControllerNav.selectOnUp = ghObj;
        __instance.mainButtons.Add(discObj);
        __instance.settingsButton.ControllerNav.selectOnRight = discObj;
        discObj.ControllerNav.selectOnLeft = __instance.settingsButton;
        discObj.ControllerNav.selectOnUp = ghObj;
        ghObj.ControllerNav.selectOnDown = discObj;
        pos.y = __instance.myAccountButton.transform.localPosition.y;

        var credObj = UObject.Instantiate(__instance.myAccountButton, __instance.myAccountButton.transform.parent);
        credObj.name = "ReworkedModInfo";
        credObj.OverrideOnClickListeners(() =>
        {
            PopUp?.Destroy();
            var template = UObject.FindObjectOfType<AnnouncementPopUp>(true);

            if (!template)
            {
                Error("Pop up was null");
                return;
            }

            PopUp = UObject.Instantiate(template);
            PopUp.gameObject.SetActive(true);

            Coroutines.Start(PerformTimedAction(0.1f, p =>
            {
                if (p == 1)
                {
                    var backup = DataManager.Player.Announcements.allAnnouncements;
                    DataManager.Player.Announcements.allAnnouncements = new();
                    PopUp.Init(false);
                    DataManager.Player.Announcements.SetAnnouncements(new[] { ModInfo });
                    var copy = DataManager.Player.Announcements.allAnnouncements;
                    PopUp.CreateAnnouncementList();
                    PopUp.UpdateAnnouncementText(ModInfo.Number);
                    PopUp.visibleAnnouncements[0].PassiveButton.OnClick = new();
                    DataManager.Player.Announcements.allAnnouncements = backup;
                }
            }));
        });
        credObj.transform.localPosition = pos;
        credObj.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = credObj.transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().sprite = GetSprite("Info");
        credObj.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
        __instance.mainButtons.Add(credObj);
        __instance.myAccountButton.ControllerNav.selectOnRight = credObj;
        credObj.ControllerNav.selectOnLeft = __instance.myAccountButton;
        credObj.ControllerNav.selectOnUp = discObj;
        discObj.ControllerNav.selectOnDown = credObj;
        credObj.ControllerNav.selectOnDown = __instance.quitButton;
        credObj.buttonText.SetText("Reworked Info");

        __instance.quitButton.ControllerNav.selectOnUp = credObj;

        var prefab = __instance.transform.GetAllComponents<StatsPopup>().First();
        var popup = UObject.Instantiate(prefab, prefab.transform.parent);
        popup.name = "AchievementPopup";
        popup.gameObject.SetActive(false);
        var title = popup.transform.FindChild("Title_TMP").GetComponent<TextMeshPro>();
        title.GetComponent<TextTranslatorTMP>().Destroy();
        title.SetText(TranslationManager.Translate("Title.Achievements"));

        var achievementsButton = UObject.Instantiate(__instance.accountCTAButton, __instance.accountCTAButton.transform.parent);
        var statsButton = __instance.accountCTAButton.transform.parent.GetChild(__instance.accountCTAButton.transform.GetSiblingIndex() + 1).GetComponent<PassiveButton>();
        var achPos = statsButton.transform.localPosition;
        achPos.y = achPos.y - Mathf.Abs(__instance.accountCTAButton.transform.localPosition.y - achPos.y);
        achievementsButton.transform.localPosition = achPos;
        achievementsButton.name = "AchievementsButton";
        achievementsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
        achievementsButton.buttonText.SetText(TranslationManager.Translate("Title.Achievements"));
        achievementsButton.OverrideOnClickListeners(() => popup.gameObject.SetActive(true));
        achievementsButton.ControllerNav.selectOnUp = statsButton;
        statsButton.ControllerNav.selectOnDown = achievementsButton;

        AddAsset("Placeholder", __instance.playButton.HoverSound);
        AddAsset("Hover", __instance.playButton.HoverSound);
        AddAsset("Click", __instance.playButton.ClickSound);
    }

    [HarmonyPatch(nameof(MainMenuManager.LateUpdate)), HarmonyPostfix]
    public static void LateUpdatePostfix(MainMenuManager __instance) => Logo.SetActive(!__instance.playLocalButton.isActiveAndEnabled && !__instance.creditsScreen.active &&
        !__instance.accountButtons.active);

    [HarmonyPatch(nameof(MainMenuManager.ResetScreen)), HarmonyPostfix]
    public static void ResetScreenPostfix(MainMenuManager __instance)
    {
        foreach (var button in __instance.mainButtons)
        {
            if (!button.IsAny(__instance.myAccountButton, __instance.settingsButton, __instance.newsButton))
                button.ControllerNav.selectOnRight = null;
        }
    }

    [HarmonyPatch(nameof(MainMenuManager.OpenGameModeMenu)), HarmonyPostfix]
    public static void OpenGameModeMenuPostfix(MainMenuManager __instance)
    {
        foreach (var button in __instance.mainButtons)
        {
            if (!button.IsAny(__instance.myAccountButton, __instance.settingsButton, __instance.newsButton))
            {
                if (__instance.playLocalButton.gameObject.activeSelf)
                    button.ControllerNav.selectOnRight = __instance.playLocalButton;
                else if (__instance.PlayOnlineButton.enabled)
                    button.ControllerNav.selectOnRight = __instance.PlayOnlineButton;
                else
                    button.ControllerNav.selectOnRight = __instance.howToPlayButton;
            }
        }
    }

    [HarmonyPatch(nameof(MainMenuManager.OpenAccountMenu)), HarmonyPostfix]
    public static void OpenAccountMenuPostfix(MainMenuManager __instance)
    {
        foreach (var button in __instance.mainButtons)
        {
            if (!button.IsAny(__instance.myAccountButton, __instance.settingsButton, __instance.newsButton))
                button.ControllerNav.selectOnRight = __instance.accountCTAButton;
        }
    }
}