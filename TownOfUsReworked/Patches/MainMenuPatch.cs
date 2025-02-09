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
        Date = "29.10.2024",
        Text = $"<size=75%>{GetString("ModInfo")}</size>"
    };
    public static GameObject Logo;

    public static void Prefix(MainMenuManager __instance)
    {
        AllMonos.AddComponents();
        LoadVanillaSounds();
        CachedFirstDead = null;
        TownOfUsReworked.IsTest = false;
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
            CreatDownloadButton(__instance, "Reworked", y, pos, "UpdateReworked");
            y += 0.5f;
            pos += 0.5f;
        }

        if (ModUpdater.SubmergedUpdate || ModUpdater.CanDownloadSubmerged)
        {
            Info($"Submerged can be {(ModUpdater.SubmergedUpdate ? "updated" : "downloaded")}");
            CreatDownloadButton(__instance, "Submerged", y, pos, $"{(SubLoaded ? "Update" : "Download")}Submerged");
            y += 0.5f;
            pos += 0.5f;
        }

        if (ModUpdater.CanDownloadLevelImpostor)
        {
            Info("LevelImpostor can be downloaded");
            CreatDownloadButton(__instance, "LevelImpostor", y, pos, "DownloadLevelImpostor");
        }

        if (ModUpdater.ReworkedUpdate || ModUpdater.SubmergedUpdate)
        {
            var popup = UObject.Instantiate(TwitchManager.Instance.TwitchPopup);
            popup.TextAreaTMP.fontSize *= 0.7f;
            popup.TextAreaTMP.enableAutoSizing = false;
            popup.Show("A mod update is available!");
        }
    }

    private static void CreatDownloadButton(MainMenuManager __instance, string downloadType, float yValue1, float yValue2, string spriteName)
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

    public static void Postfix(MainMenuManager __instance)
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
        __instance.newsButton.transform.GetChild(0).GetChild(0).localScale = new(scale.x * 3.5f, scale.y, scale.z);
        __instance.newsButton.transform.GetChild(1).GetChild(0).localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);
        __instance.newsButton.transform.GetChild(2).GetChild(0).localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);
        __instance.myAccountButton.transform.GetChild(0).GetChild(0).localScale = new(scale.x * 3.5f, scale.y, scale.z);
        __instance.myAccountButton.transform.GetChild(1).GetChild(0).localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);
        __instance.myAccountButton.transform.GetChild(2).GetChild(0).localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);
        __instance.settingsButton.transform.GetChild(0).GetChild(0).localScale = new(scale.x * 3.5f, scale.y, scale.z);
        __instance.settingsButton.transform.GetChild(1).GetChild(0).localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);
        __instance.settingsButton.transform.GetChild(2).GetChild(0).localScale = new(scale.x * 1.9f, scale.y / 1.5f, scale.z);

        var ghObj = UObject.Instantiate(__instance.newsButton, __instance.newsButton.transform.parent);
        ghObj.name = "ReworkedGitHub";
        ghObj.OverrideOnClickListeners(() => Application.OpenURL(TownOfUsReworked.GitHubLink));
        ghObj.transform.localPosition = pos;
        pos.y = __instance.settingsButton.transform.localPosition.y;

        var discObj = UObject.Instantiate(__instance.settingsButton, __instance.settingsButton.transform.parent);
        discObj.name = "ReworkedDiscord";
        discObj.OverrideOnClickListeners(() => Application.OpenURL(TownOfUsReworked.DiscordInvite));
        discObj.transform.localPosition = pos;
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

        Coroutines.Start(PerformTimedAction(0.01f, _ =>
        {
            discObj.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().SetText("Mod Discord");
            ghObj.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().SetText("Mod GitHub");
            credObj.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().SetText("Mod Info");
        }));

        AddAsset("Hover", __instance.playButton.HoverSound);
        AddAsset("Click", __instance.playButton.ClickSound);

        ClientHandler.Instance.ButtonsSet = false;
    }
}

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
public static class MainMenuUpdatePatch
{
    public static void Postfix(MainMenuManager __instance)
    {
        try
        {
            var pos = __instance.newsButton.transform.GetChild(0).GetChild(0).transform.position;
            pos.x -= 0.1f;
            __instance.newsButton.transform.GetChild(0).GetChild(0).transform.position = pos;
            var pos2 = __instance.myAccountButton.transform.GetChild(0).GetChild(0).transform.position;
            pos2.x -= 0.1f;
            __instance.myAccountButton.transform.GetChild(0).GetChild(0).transform.position = pos2;
            var pos3 = __instance.settingsButton.transform.GetChild(0).GetChild(0).transform.position;
            pos3.x -= 0.1f;
            __instance.settingsButton.transform.GetChild(0).GetChild(0).transform.position = pos3;
            var pos4 = GameObject.Find("ReworkedDiscord").transform.GetChild(0).GetChild(0).transform.position;
            pos4.x -= 0.1f;
            GameObject.Find("ReworkedDiscord").transform.GetChild(0).GetChild(0).transform.position = pos4;
            var pos5 = GameObject.Find("ReworkedGitHub").transform.GetChild(0).GetChild(0).transform.position;
            pos5.x -= 0.1f;
            GameObject.Find("ReworkedGitHub").transform.GetChild(0).GetChild(0).transform.position = pos5;
            var pos6 = GameObject.Find("ReworkedModInfo").transform.GetChild(0).GetChild(0).transform.position;
            pos6.x -= 0.1f;
            GameObject.Find("ReworkedModInfo").transform.GetChild(0).GetChild(0).transform.position = pos6;
            MainMenuStartPatch.Logo.SetActive(!__instance.playLocalButton.isActiveAndEnabled && !__instance.creditsScreen.active);
        } catch {}
    }
}