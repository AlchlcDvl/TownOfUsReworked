using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using UnityEngine.UI;
using TownOfUs.Extensions;

namespace TownOfUs.CrewmateRoles.MysticMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        private static Sprite LighterSprite => TownOfUs.LighterSprite;
        public static Sprite DarkerSprite => TownOfUs.DarkerSprite;

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (player == null || player.Data.Disconnected)
                return true;
            else
                return false;
        }

        private static Action LighterDarkerHandler()
        {
            //Used to avoid the Lighter/Darker Indicators causing any button problems by giving them their own Listener events.
            void Listener() {}

            return Listener;
        }

        public static void GenButton(Mystic role, PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;
            if (IsExempt(voteArea))
            {
                return;
            }
            
            var colorButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(colorButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();

            PlayerControl playerControl = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == voteArea.TargetPlayerId);

            if (role.LightDarkColors[playerControl.GetDefaultOutfit().ColorId] == "lighter")
                renderer.sprite = LighterSprite;
            else
                renderer.sprite = DarkerSprite;
            
            newButton.transform.position = colorButton.transform.position - new Vector3(-0.8f, 0.2f, -2f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = colorButton.transform.parent.parent;
            var newButtonClickEvent = new Button.ButtonClickedEvent();
            newButtonClickEvent.AddListener(LighterDarkerHandler());
            newButton.GetComponent<PassiveButton>().OnClick = newButtonClickEvent;
            role.Buttons.Add(newButton);
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Mystic))
            {
                var mystic = (Mystic) role;
                mystic.Buttons.Clear();
            }
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            var mysticrole = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);
            foreach (var voteArea in __instance.playerStates)
            {
                try {
                    GenButton(mysticrole, voteArea);
                } catch {
                }
            }
        }
    }
}