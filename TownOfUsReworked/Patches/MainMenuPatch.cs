using Assets.InnerNet;
using Twitch;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class MainMenuStartPatch
    {
        private static AnnouncementPopUp popUp;
        public static GameObject Logo;

        public static void Prefix(MainMenuManager __instance)
        {
            ModUpdater.LaunchUpdater();
            CosmeticsLoader.LaunchFetchers(ModUpdater.HasReworkedUpdate);

            var amongUsLogo = GameObject.Find("bannerLogo_AmongUs");

            if (amongUsLogo)
            {
                amongUsLogo.transform.localScale *= 0.75f;
                amongUsLogo.transform.position += Vector3.up * 0.25f;
            }

            if (!Logo)
            {
                Logo = new GameObject("bannerLogo_TownOfUsReworked");
                Logo.transform.position = new (0, 0.7f, 0);
                Logo.transform.SetParent(amongUsLogo.transform);
                Logo.AddComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("TownOfUsReworkedBanner");
            }

            var InvButton = GameObject.Find("InventoryButton");

            if (InvButton == null)
                return;

            var discObj = UObject.Instantiate(InvButton, InvButton.transform.parent);
            discObj.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("Discord");

            var button1 = discObj.GetComponent<PassiveButton>();
            button1.OnClick = new();
            button1.OnClick.AddListener((Action)(() => Application.OpenURL("https://discord.gg/cd27aDQDY9")));

            var announceObj = UObject.Instantiate(InvButton, InvButton.transform.parent);
            announceObj.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("Update");

            var button2 = announceObj.GetComponent<PassiveButton>();
            button2.OnClick = new();
            button2.OnClick.AddListener((Action)(() =>
            {
                popUp?.Destroy();
                popUp = UObject.Instantiate(UObject.FindObjectOfType<AnnouncementPopUp>(true));
                popUp.gameObject.SetActive(true);

                var changesAnnouncement = new Announcement
                {
                    Id = "tourewChanges",
                    Language = 0,
                    Number = 500,
                    Title = "Town Of Us Reworked Changes",
                    ShortTitle = "Changes",
                    SubTitle = "No idea what I'm doing anymore lmao",
                    PinState = false,
                    Date = "30.04.2023",
                    Text = $"<size=75%>{Utils.CreateText("Changelog")}</size>"
                };

                __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ =>
                {
                    var backup = DataManager.Player.Announcements.allAnnouncements;
                    popUp.Init(false);
                    DataManager.Player.Announcements.allAnnouncements = new();
                    DataManager.Player.Announcements.allAnnouncements.Insert(0, changesAnnouncement);

                    foreach (var item in popUp.visibleAnnouncements)
                        item.Destroy();

                    foreach (var item in UObject.FindObjectsOfType<AnnouncementPanel>())
                    {
                        if (item != popUp.ErrorPanel)
                            item.gameObject.Destroy();
                    }

                    popUp.CreateAnnouncementList();
                    popUp.visibleAnnouncements[0].PassiveButton.OnClick = new();
                    DataManager.Player.Announcements.allAnnouncements = backup;
                    var titleText = GameObject.Find("Title_Text").GetComponent<TextMeshPro>();

                    if (titleText != null)
                        titleText.text = "";
                })));
            }));

            var credObj = UObject.Instantiate(InvButton, InvButton.transform.parent);
            credObj.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("Credits");

            var button3 = credObj.GetComponent<PassiveButton>();
            button3.OnClick = new();
            button3.OnClick.AddListener((Action)(() =>
            {
                popUp?.Destroy();
                popUp = UObject.Instantiate(UObject.FindObjectOfType<AnnouncementPopUp>(true));
                popUp.gameObject.SetActive(true);

                var creditsAnnouncement = new Announcement
                {
                    Id = "tourewCredits",
                    Language = 0,
                    Number = 500,
                    Title = "Town Of Us Reworked Credits",
                    ShortTitle = "Credits",
                    SubTitle = "This mod wasn't possible without these people!",
                    PinState = false,
                    Date = "30.04.2023",
                    Text = $"<size=75%>{Utils.CreateText("Credits")}</size>"
                };

                __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ =>
                {
                    var backup = DataManager.Player.Announcements.allAnnouncements;
                    popUp.Init(false);
                    DataManager.Player.Announcements.allAnnouncements = new();
                    DataManager.Player.Announcements.allAnnouncements.Insert(0, creditsAnnouncement);

                    foreach (var item in popUp.visibleAnnouncements)
                        item.Destroy();

                    foreach (var item in UObject.FindObjectsOfType<AnnouncementPanel>())
                    {
                        if (item != popUp.ErrorPanel)
                            item.gameObject.Destroy();
                    }

                    popUp.CreateAnnouncementList();
                    popUp.visibleAnnouncements[0].PassiveButton.OnClick = new();
                    DataManager.Player.Announcements.allAnnouncements = backup;
                    var titleText = GameObject.Find("Title_Text").GetComponent<TextMeshPro>();

                    if (titleText != null)
                        titleText.text = "";
                })));
            }));

            __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ =>
            {
                foreach (var tf in InvButton.transform.parent.GetComponentsInChildren<Transform>())
                    tf.localPosition = new(tf.localPosition.x * 0.7f, tf.localPosition.y);
            })));

            var local = GameObject.Find("PlayLocalButton");

            if (local)
            {
                local.transform.localScale = new(0.8f, 0.8f, 0.8f);
                local.transform.position = new(-0.8f, -1.6f, 0f);
            }

            var online = GameObject.Find("PlayOnlineButton");

            if (online)
            {
                online.transform.localScale = new(0.8f, 0.8f, 0.8f);
                online.transform.position = new(0.8f, -1.6f, 0f);
            }

            var howTo = GameObject.Find("HowToPlayButton");

            if (howTo)
            {
                howTo.transform.localScale = new(0.8f, 0.8f, 0.8f);
                howTo.transform.position = new(-2.4f, -1.71f, 0f);
            }

            var freeplay = GameObject.Find("FreePlayButton");

            if (freeplay)
            {
                freeplay.transform.localScale = new(0.8f, 0.8f, 0.8f);
                freeplay.transform.position = new(2.4f, -1.71f, 0f);
            }

            var template = GameObject.Find("ExitGameButton");

            if (template)
            {
                var i = 1;
                var pos = template.transform.localPosition;

                if (ModUpdater.HasReworkedUpdate)
                {
                    var touButton = UObject.Instantiate(template, null);
                    pos.y += 0.6f * i;
                    touButton.transform.localPosition = pos;
                    touButton.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("UpdateToUButton");
                    var passiveTOUButton = touButton.GetComponent<PassiveButton>();
                    passiveTOUButton.OnClick = new();
                    passiveTOUButton.OnClick.AddListener((Action)(() =>
                    {
                        ModUpdater.ExecuteUpdate("Reworked");
                        touButton.SetActive(false);
                    }));
                    var text = touButton.transform.GetChild(0).GetComponent<TMP_Text>();
                    __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => text.SetText(""))));
                    ModUpdater.InfoPopup = UObject.Instantiate(TwitchManager.Instance.TwitchPopup);
                    ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.7f;
                    ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = false;
                    i++;
                }

                if (ModUpdater.HasSubmergedUpdate)
                {
                    var submergedButton = UObject.Instantiate(template, null);
                    pos.y += 0.6f * i;
                    submergedButton.transform.localPosition = pos;
                    submergedButton.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("UpdateSubmergedButton");
                    var passiveSubmergedButton = submergedButton.GetComponent<PassiveButton>();
                    passiveSubmergedButton.OnClick = new();
                    passiveSubmergedButton.OnClick.AddListener((Action)(() =>
                    {
                        ModUpdater.ExecuteUpdate("Submerged");
                        submergedButton.SetActive(false);
                    }));
                    var text = submergedButton.transform.GetChild(0).GetComponent<TMP_Text>();
                    __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => text.SetText(""))));
                    ModUpdater.InfoPopup = UObject.Instantiate(TwitchManager.Instance.TwitchPopup);
                    ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.7f;
                    ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = false;
                    i++;
                }
            }
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
    public static class MainMenuUpdatePatch
    {
        private static int Flip = 1;
        private static float _time;
        private static bool Started;

        public static void Postfix()
        {
            MainMenuStartPatch.Logo?.transform.Rotate(Vector3.forward * 2 * Time.fixedDeltaTime * Flip);
            _time += Time.deltaTime;

            if (_time > 0.5f && !Started)
            {
                _time--;
                Flip *= -1;
                Started = true;
            }
            else if (_time > 1f)
            {
                _time--;
                Flip *= -1;
            }
        }
    }
}