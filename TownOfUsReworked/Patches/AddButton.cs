using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        private static Sprite LighterSprite => TownOfUsReworked.LighterSprite;
        public static Sprite DarkerSprite => TownOfUsReworked.DarkerSprite;

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = Utils.PlayerById(voteArea.TargetPlayerId);

            if (player == null || player.Data.Disconnected || !CustomGameOptions.LighterDarker)
                return true;
            else
                return false;
        }

        public static void GenButton(PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;

            if (IsExempt(voteArea))
                return;
                
            var colorButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(colorButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();

            PlayerControl playerControl = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == targetId);
            var ColorString = Role.LightDarkColors[playerControl.GetDefaultOutfit().ColorId];

            if (ColorString == "lighter")
                renderer.sprite = LighterSprite;
            else if (ColorString == "darker")
                renderer.sprite = DarkerSprite;
                
            newButton.transform.position = colorButton.transform.position - new Vector3(-0.8f, 0.2f, -2f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = colorButton.transform.parent.parent;
            var newButtonClickEvent = new Button.ButtonClickedEvent();
            newButtonClickEvent.AddListener(LighterDarkerHandler());
            newButton.GetComponent<PassiveButton>().OnClick = newButtonClickEvent;
            Role.Buttons.Add(newButton);
        }

        private static Action LighterDarkerHandler()
        {
            //Used to avoid the Lighter/Darker Indicators causing any button problems by giving them their own Listener events.
            void Listener() {}
            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            if (CustomGameOptions.LighterDarker)
            {
                foreach (var voteArea in __instance.playerStates)
                {   
                    try
                    {
                        GenButton(voteArea);
                    } catch {}
                }
            }
        }
    }
}