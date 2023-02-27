using HarmonyLib;
using System;
using UnityEngine;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;
using TMPro;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class MainMenuPatch
    {
        public static Sprite Sprite => TownOfUsReworked.ToUBanner;
        private static AnnouncementPopUp popUp;

        static void Postfix(MainMenuManager __instance)
        {
            var amongUsLogo = GameObject.Find("bannerLogo_AmongUs");

            if (amongUsLogo != null)
            {
                amongUsLogo.transform.localScale *= 0.5f;
                amongUsLogo.transform.position += Vector3.up * 0.25f;
            }

            var touLogo = new GameObject("bannerLogo_TownOfUs");
            touLogo.transform.position = Vector3.up;
            var renderer = touLogo.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite;

            var InvButton = GameObject.Find("InventoryButton");

            if (InvButton == null)
                return;

            var discObj = GameObject.Instantiate(InvButton, InvButton.transform.parent);
            var iconrenderer1 = discObj.GetComponent<SpriteRenderer>();
            iconrenderer1.sprite = TownOfUsReworked.DiscordImage;

            var button1 = discObj.GetComponent<PassiveButton>();
            button1.OnClick = new ButtonClickedEvent();
            button1.OnClick.AddListener((Action)(() => Application.OpenURL("https://discord.gg/cd27aDQDY9")));

            var announceObj = GameObject.Instantiate(InvButton, InvButton.transform.parent);
            var iconrenderer2 = announceObj.GetComponent<SpriteRenderer>();
            iconrenderer2.sprite = TownOfUsReworked.UpdateImage;

            var button2 = announceObj.GetComponent<PassiveButton>();
            button2.OnClick = new ButtonClickedEvent();
            button2.OnClick.AddListener((Action)(() =>
            {
                if (popUp != null)
                    Object.Destroy(popUp);

                popUp = Object.Instantiate(Object.FindObjectOfType<AnnouncementPopUp>(true));
                popUp.gameObject.SetActive(true);
                popUp.Init();
                var changelog = Utils.CreateText("Changelog", "Misc");
                popUp.AnnounceTextMeshPro.text = $"<size=60%>{changelog}</size>";

                __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>((p) =>
                {
                    if (p == 1)
                    {
                        var titleText = GameObject.Find("Title_Text").GetComponent<TMPro.TextMeshPro>();

                        if (titleText != null)
                        {
                            titleText.text = "   Changes";
                            titleText.alignment = TextAlignmentOptions.Center;
                        }
                    }
                })));
            }));
        }
    }
}