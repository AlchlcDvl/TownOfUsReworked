using Assets.InnerNet;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public static class MainMenuStartPatch
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
        PinState = false,
        Date = "16.07.2023",
        Text = $"<size=75%>{CreateText("ModInfo")}</size>"
    };
    public static GameObject Logo;

    public static void Prefix(MainMenuManager __instance)
    {
        ModUpdater.LaunchUpdater();
        Init();
        Generate.GenerateAll();
        RoleGen.ResetEverything();
        Info.SetAllInfo();
        UpdateNames.PlayerNames.Clear();

        if (!Logo)
        {
            Logo = new GameObject("TownOfUsReworkedLogo");
            Logo.transform.position = new (2f, -0.1f, 100f);
            Logo.AddComponent<SpriteRenderer>().sprite = GetSprite("TownOfUsReworkedBanner");
            Logo.transform.SetParent(__instance.rightPanelMask.transform);
        }
    }

    public static void Postfix(MainMenuManager __instance)
    {
        CosmeticsLoader.LaunchFetchers(ModUpdater.HasReworkedUpdate);
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
        GameObject.Find("NewsButton").transform.GetChild(0).GetChild(0).transform.localScale = new(scale.x * 3.5f, scale.y, scale.z);
        GameObject.Find("NewsButton").transform.GetChild(1).GetChild(0).transform.localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);
        GameObject.Find("NewsButton").transform.GetChild(2).GetChild(0).transform.localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);
        GameObject.Find("AcountButton").transform.GetChild(0).GetChild(0).transform.localScale = new(scale.x * 3.5f, scale.y, scale.z);
        GameObject.Find("AcountButton").transform.GetChild(1).GetChild(0).transform.localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);
        GameObject.Find("AcountButton").transform.GetChild(2).GetChild(0).transform.localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);
        GameObject.Find("SettingsButton").transform.GetChild(0).GetChild(0).transform.localScale = new(scale.x * 3.5f, scale.y, scale.z);
        GameObject.Find("SettingsButton").transform.GetChild(1).GetChild(0).transform.localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);
        GameObject.Find("SettingsButton").transform.GetChild(2).GetChild(0).transform.localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);

        var ghObj = UObject.Instantiate(__instance.newsButton, __instance.newsButton.transform.parent);
        ghObj.gameObject.name = "ReworkedGitHub";
        ghObj.OnClick = new();
        ghObj.OnClick.AddListener((Action)(() => Application.OpenURL(TownOfUsReworked.GitHubLink)));
        ghObj.transform.localPosition = pos;
        pos.y = __instance.settingsButton.transform.localPosition.y;

        var discObj = UObject.Instantiate(__instance.settingsButton, __instance.settingsButton.transform.parent);
        discObj.gameObject.name = "ReworkedDiscord";
        discObj.OnClick = new();
        discObj.OnClick.AddListener((Action)(() => Application.OpenURL(TownOfUsReworked.DiscordInvite)));
        discObj.transform.localPosition = pos;
        pos.y = __instance.myAccountButton.transform.localPosition.y;

        var credObj = UObject.Instantiate(__instance.myAccountButton, __instance.myAccountButton.transform.parent);
        credObj.gameObject.name = "ReworkedModInfo";
        credObj.OnClick = new();
        credObj.OnClick.AddListener((Action)(() =>
        {
            PopUp?.Destroy();
            var template = UObject.FindObjectOfType<AnnouncementPopUp>(true);

            if (template == null)
            {
                LogSomething("Pop up was null");
                return;
            }

            PopUp = UObject.Instantiate(template);
            PopUp.gameObject.SetActive(true);

            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(p =>
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
            })));
        }));
        credObj.transform.localPosition = pos;

        __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ =>
        {
            GameObject.Find("ReworkedDiscord").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().SetText("Mod Discord");
            GameObject.Find("ReworkedGitHub").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().SetText("Mod GitHub");
            GameObject.Find("ReworkedModInfo").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().SetText("Mod Info");
        })));

        var template = GameObject.Find("ExitGameButton");

        if (template)
        {
            var pos2 = template.transform.localPosition;

            if (ModUpdater.HasReworkedUpdate)
            {
                var touButton = UObject.Instantiate(template, null);
                pos2.y += 0.6f;
                touButton.name = "ReworkedUpdater";
                touButton.transform.localPosition = pos2;
                touButton.transform.localScale = new(0.44f, 0.84f, 1f);
                touButton.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetSprite("UpdateToUButton");
                touButton.transform.SetParent(GameObject.Find("RightPanel").transform);
                var aspect = touButton.GetComponent<AspectPosition>();
                aspect.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
                aspect.DistanceFromEdge = new(1.5f, 1f, 0f);
                var passiveTOUButton = touButton.GetComponent<PassiveButton>();
                passiveTOUButton.OnClick = new();
                passiveTOUButton.OnClick.AddListener((Action)(() =>
                {
                    ModUpdater.ExecuteUpdate("Reworked");
                    touButton.SetActive(false);
                }));
                __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ =>
                {
                    touButton.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().SetText("");
                    aspect.AdjustPosition();
                })));
                ModUpdater.InfoPopup = UObject.Instantiate(TwitchManager.Instance.TwitchPopup);
                ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.7f;
                ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = false;
            }

            if (ModUpdater.HasSubmergedUpdate)
            {
                var submergedButton = UObject.Instantiate(template, null);
                pos2.y += 0.6f;
                submergedButton.name = "SubmergedUpdater";
                submergedButton.transform.localPosition = pos2;
                submergedButton.transform.localScale = new(0.44f, 0.84f, 1f);
                submergedButton.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetSprite("UpdateToUButton");
                submergedButton.transform.SetParent(GameObject.Find("RightPanel").transform);
                var aspect = submergedButton.GetComponent<AspectPosition>();
                aspect.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
                aspect.DistanceFromEdge = new(1.5f, 1f, 0f);
                var passiveTOUButton = submergedButton.GetComponent<PassiveButton>();
                passiveTOUButton.OnClick = new();
                passiveTOUButton.OnClick.AddListener((Action)(() =>
                {
                    ModUpdater.ExecuteUpdate("Submerged");
                    submergedButton.SetActive(false);
                }));
                __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ =>
                {
                    submergedButton.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().SetText("");
                    aspect.AdjustPosition();
                })));
                ModUpdater.InfoPopup = UObject.Instantiate(TwitchManager.Instance.TwitchPopup);
                ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.7f;
                ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = false;
            }
        }
    }
}

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
public static class MainMenuUpdatePatch
{
    public static void Postfix(MainMenuManager __instance)
    {
        try
        {
            var pos = GameObject.Find("NewsButton").transform.GetChild(0).GetChild(0).transform.position;
            pos.x -= 0.1f;
            GameObject.Find("NewsButton").transform.GetChild(0).GetChild(0).transform.position = pos;
            var pos2 = GameObject.Find("AcountButton").transform.GetChild(0).GetChild(0).transform.position;
            pos2.x -= 0.1f;
            GameObject.Find("AcountButton").transform.GetChild(0).GetChild(0).transform.position = pos2;
            var pos3 = GameObject.Find("SettingsButton").transform.GetChild(0).GetChild(0).transform.position;
            pos3.x -= 0.1f;
            GameObject.Find("SettingsButton").transform.GetChild(0).GetChild(0).transform.position = pos3;
            var pos4 = GameObject.Find("ReworkedDiscord").transform.GetChild(0).GetChild(0).transform.position;
            pos4.x -= 0.1f;
            GameObject.Find("ReworkedDiscord").transform.GetChild(0).GetChild(0).transform.position = pos4;
            var pos5 = GameObject.Find("ReworkedGitHub").transform.GetChild(0).GetChild(0).transform.position;
            pos5.x -= 0.1f;
            GameObject.Find("ReworkedGitHub").transform.GetChild(0).GetChild(0).transform.position = pos5;
            var pos6 = GameObject.Find("ReworkedModInfo").transform.GetChild(0).GetChild(0).transform.position;
            pos6.x -= 0.1f;
            GameObject.Find("ReworkedModInfo").transform.GetChild(0).GetChild(0).transform.position = pos6;
            MainMenuStartPatch.Logo.SetActive(!__instance.playLocalButton.isActiveAndEnabled);
            VersionShowerPatch.ModVersion.gameObject.SetActive(!__instance.playLocalButton.isActiveAndEnabled);
        } catch {}
    }
}