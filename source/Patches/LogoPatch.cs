using HarmonyLib;
using System;
using UnityEngine;
using static UnityEngine.UI.Button;

namespace TownOfUs {
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class LogoPatch
    {
        private static Sprite Sprite => TownOfUs.ToUBanner;
        static void Postfix(PingTracker __instance) {
            var amongUsLogo = GameObject.Find("bannerLogo_AmongUs");
            if (amongUsLogo != null) {
                amongUsLogo.transform.localScale *= 0.6f;
                amongUsLogo.transform.position += Vector3.up * 0.25f;
            }

            var touLogo = new GameObject("bannerLogo_TownOfUs");
            touLogo.transform.position = Vector3.up;
            var renderer = touLogo.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite;


            var InvButton = GameObject.Find("InventoryButton");
            if (InvButton == null) return;
            var horseObj = GameObject.Instantiate(InvButton, InvButton.transform.parent);

            var iconrenderer = horseObj.GetComponent<SpriteRenderer>();
            iconrenderer.sprite = Patches.ClientOptions.HorseEnabled ? TownOfUs.HorseEnabledImage : TownOfUs.HorseDisabledImage;


            var button = horseObj.GetComponent<PassiveButton>();
            button.OnClick = new ButtonClickedEvent();
            button.OnClick.AddListener((Action)(() =>
            {
                Patches.ClientOptions.HorseEnabled = !Patches.ClientOptions.HorseEnabled;
                iconrenderer.sprite = Patches.ClientOptions.HorseEnabled ? TownOfUs.HorseEnabledImage : TownOfUs.HorseDisabledImage;
                var particles = GameObject.FindObjectOfType<PlayerParticles>();
                if (particles != null)
                {
                    particles.pool.ReclaimAll();
                    particles.Start();
                }
            }));
        }
    }
}