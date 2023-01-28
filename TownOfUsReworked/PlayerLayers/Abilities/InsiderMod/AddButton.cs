using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Abilities.InsiderMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        public static Sprite ActiveSprite => TownOfUsReworked.VoteCount;
        public static Sprite DisabledSprite => TownOfUsReworked.VoteCountDisabled;

        public static void GenButton(Insider role, int index, bool isDead)
        {
            if (isDead)
            {
                role.Buttons.Add(null);
                return;
            }

            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();
            renderer.sprite = DisabledSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(1.2f, 0.08f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, index));
            role.Buttons.Add(newButton);
        }

        private static Action SetActive(Insider role, int index)
        {
            void Listener()
            {
                foreach (var button in role.Buttons)
                {
                    if (button == null) continue;
                    button.GetComponent<SpriteRenderer>().sprite = DisabledSprite;
                }
                role.Buttons[index].GetComponent<SpriteRenderer>().sprite =
                    role.TargetId == (byte)index ? DisabledSprite : ActiveSprite;
                role.TargetId =
                    role.TargetId == (byte)index ? byte.MaxValue : (byte)index;
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Insider))
                return;

            var votecounter = Ability.GetAbility<Insider>(PlayerControl.LocalPlayer);
            votecounter.TargetId = byte.MaxValue;
            votecounter.Buttons.Clear();

            for (var i = 0; i < __instance.playerStates.Length; i++)
                GenButton(votecounter, i, __instance.playerStates[i].AmDead);
        }
    }
}