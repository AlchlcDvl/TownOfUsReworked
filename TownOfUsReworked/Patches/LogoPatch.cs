using HarmonyLib;
using System;
using UnityEngine;
using static UnityEngine.UI.Button;
using TownOfUsReworked.Lobby;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class LogoPatch
    {
        public static Sprite Sprite => TownOfUsReworked.ToUBanner;

        static void Postfix(PingTracker __instance)
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
        }
    }
}