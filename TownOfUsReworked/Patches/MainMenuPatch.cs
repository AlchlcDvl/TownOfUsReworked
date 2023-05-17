using Assets.InnerNet;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class MainMenuPatch
    {
        private static AnnouncementPopUp popUp;

        public static void Prefix(MainMenuManager __instance)
        {
            CosmeticsLoader.LaunchFetchers();
            var amongUsLogo = GameObject.Find("bannerLogo_AmongUs");

            if (amongUsLogo != null)
            {
                amongUsLogo.transform.localScale *= 0.7f;
                amongUsLogo.transform.position += Vector3.up * 0.25f;
            }

            var tourewLogo = new GameObject("bannerLogo_TownOfUsReworked");
            tourewLogo.transform.position = new (0, 0.7f, 0);
            var renderer = tourewLogo.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetManager.GetSprite("TownOfUsReworkedBanner");

            var InvButton = GameObject.Find("InventoryButton");

            if (InvButton == null)
                return;

            var discObj = UObject.Instantiate(InvButton, InvButton.transform.parent);
            var iconrenderer1 = discObj.GetComponent<SpriteRenderer>();
            iconrenderer1.sprite = AssetManager.GetSprite("Discord");

            var button1 = discObj.GetComponent<PassiveButton>();
            button1.OnClick = new();
            button1.OnClick.AddListener((Action)(() => Application.OpenURL("https://discord.gg/cd27aDQDY9")));

            var announceObj = UObject.Instantiate(InvButton, InvButton.transform.parent);
            var iconrenderer2 = announceObj.GetComponent<SpriteRenderer>();
            iconrenderer2.sprite = AssetManager.GetSprite("Update");

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
                    SubTitle = "no idea what im doing anymore lmao",
                    PinState = false,
                    Date = "30.04.2023"
                };

                var changelog = Utils.CreateText("Changelog");
                changesAnnouncement.Text = $"<size=75%>{changelog}</size>";

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

            __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ =>
            {
                foreach (var tf in InvButton.transform.parent.GetComponentsInChildren<Transform>())
                    tf.localPosition = new(tf.localPosition.x * 0.7f, tf.localPosition.y);
            })));

            var local = GameObject.Find("PlayLocalButton");

            if (local != null)
            {
                local.transform.localScale = new(0.8f, 0.8f, 0.8f);
                local.transform.position = new(-0.8f, -1.6f, 0f);
            }

            var online = GameObject.Find("PlayOnlineButton");

            if (online != null)
            {
                online.transform.localScale = new(0.8f, 0.8f, 0.8f);
                online.transform.position = new(0.8f, -1.6f, 0f);
            }

            var howTo = GameObject.Find("HowToPlayButton");

            if (howTo != null)
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
        }
    }
}