using HarmonyLib;
using System;
using UnityEngine;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;
using TownOfUsReworked.Classes;
using AmongUs.Data;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class MainMenuPatch
    {
        private static AnnouncementPopUp popUp;

        static void Postfix(MainMenuManager __instance)
        {
            var amongUsLogo = GameObject.Find("bannerLogo_AmongUs");

            if (amongUsLogo != null)
            {
                amongUsLogo.transform.localScale *= 0.5f;
                amongUsLogo.transform.position += Vector3.up * 0.25f;
            }

            var tourewLogo = new GameObject("bannerLogo_TownOfUsReworked");
            tourewLogo.transform.position = Vector3.up;
            var renderer = tourewLogo.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetManager.ToUBanner;

            var InvButton = GameObject.Find("InventoryButton");

            if (InvButton == null)
                return;

            var discObj = GameObject.Instantiate(InvButton, InvButton.transform.parent);
            var iconrenderer1 = discObj.GetComponent<SpriteRenderer>();
            iconrenderer1.sprite = AssetManager.DiscordImage;

            var button1 = discObj.GetComponent<PassiveButton>();
            button1.OnClick = new ButtonClickedEvent();
            button1.OnClick.AddListener((Action)(() => Application.OpenURL("https://discord.gg/cd27aDQDY9")));

            var announceObj = GameObject.Instantiate(InvButton, InvButton.transform.parent);
            var iconrenderer2 = announceObj.GetComponent<SpriteRenderer>();
            iconrenderer2.sprite = AssetManager.UpdateImage;

            var button2 = announceObj.GetComponent<PassiveButton>();
            button2.OnClick = new ButtonClickedEvent();
            button2.OnClick.AddListener((Action)(() =>
            {
                if (popUp != null)
                    Object.Destroy(popUp);

                popUp = Object.Instantiate(Object.FindObjectOfType<AnnouncementPopUp>(true));
                popUp.gameObject.SetActive(true);
                var changesAnnouncement = new Assets.InnerNet.Announcement();
                changesAnnouncement.Id = "tourewChanges";
                changesAnnouncement.Language = 0;
                changesAnnouncement.Number = 500;
                changesAnnouncement.Title = "Town Of Us Reworked Changes";
                changesAnnouncement.ShortTitle = "Changes";
                changesAnnouncement.SubTitle = "no idea what im doing anymore lmao";
                changesAnnouncement.PinState = false;
                changesAnnouncement.Date = "03.03.2023";
                var changelog = Utils.CreateText("Changelog");
                changesAnnouncement.Text = $"<size=60%>{changelog}</size>";

                __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>((p) =>
                {
                    if (p == 1)
                    {
                        var backup = DataManager.Player.Announcements.allAnnouncements;
                        popUp.Init(false);
                        DataManager.Player.Announcements.allAnnouncements = new();
                        DataManager.Player.Announcements.allAnnouncements.Insert(0, changesAnnouncement);

                        foreach (var item in popUp.visibleAnnouncements)
                            Object.Destroy(item);

                        foreach (var item in Object.FindObjectsOfType<AnnouncementPanel>())
                        {
                            if (item != popUp.ErrorPanel)
                                Object.Destroy(item.gameObject);
                        }

                        popUp.CreateAnnouncementList();
                        popUp.visibleAnnouncements[0].PassiveButton.OnClick.RemoveAllListeners();
                        DataManager.Player.Announcements.allAnnouncements = backup;
                        var titleText = GameObject.Find("Title_Text").GetComponent<TMPro.TextMeshPro>();

                        if (titleText != null)
                            titleText.text = "";
                    }
                })));
            }));
        }
    }
}