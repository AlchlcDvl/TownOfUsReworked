using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class AddButton
    {
        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = Utils.PlayerByVoteArea(voteArea);
            return player?.Data.Disconnected == true || !CustomGameOptions.LighterDarker;
        }

        public static void GenButton(PlayerVoteArea voteArea)
        {
            if (IsExempt(voteArea))
                return;

            var targetId = voteArea.TargetPlayerId;
            var colorButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(colorButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();

            var playerControl = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == targetId);
            var ColorString = Role.LightDarkColors[playerControl.GetDefaultOutfit().ColorId];

            if (ColorString == "lighter")
                renderer.sprite = AssetManager.Lighter;
            else if (ColorString == "darker")
                renderer.sprite = AssetManager.Darker;

            newButton.transform.position = colorButton.transform.position - new Vector3(-0.8f, 0.2f, -2f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = colorButton.transform.parent.parent;
            var newButtonClickEvent = new Button.ButtonClickedEvent();
            newButtonClickEvent.AddListener((Action)(() => {}));
            newButton.GetComponent<PassiveButton>().OnClick = newButtonClickEvent;
            Role.Buttons.Add(newButton);
        }

        public static void Postfix(MeetingHud __instance)
        {
            if (CustomGameOptions.LighterDarker)
            {
                foreach (var voteArea in __instance.playerStates)
                    GenButton(voteArea);
            }

            foreach (var player in PlayerControl.AllPlayerControls)
                player.RegenTask();
        }
    }
}