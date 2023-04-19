/*using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.PlayerLayers;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using System;

namespace TownOfUsReworked.Custom
{
    //This is WIP
    [HarmonyPatch]
    public class CustomMeetingButtons
    {
        public readonly static List<CustomMeetingButtons> AllMeetingButtons = new();
        public Dictionary<byte, GameObject> Buttons;
        public List<bool> Actives;
        public Sprite ActiveSprite;
        public Sprite InactiveSprite;
        public Color32 HoverColor;
        public Color32 DefaultColor;
        public int ActiveLimit;
        public PlayerLayer Owner;
        public IsExempt Exempt;
        public SetActive Click;
        public bool UseAfterVote;
        public delegate bool IsExempt(PlayerVoteArea voteArea);
        public delegate void SetActive(byte index);

        public CustomMeetingButtons(PlayerLayer owner, Sprite active, Sprite inactive, IsExempt exempt, SetActive click)
        {
            Owner = owner;
            ActiveSprite = active;
            InactiveSprite = inactive;
            Exempt = exempt;
            Click = click;
            AllMeetingButtons.Add(this);
        }

        public void GenButtons(MeetingHud __instance)
        {
            if (Owner.Player != PlayerControl.LocalPlayer)
                return;
        }

        public void GenButton(MeetingHud __instance, byte index)
        {
            var voteArea = MeetingHud.Instance.playerStates[index];
            var button = voteArea.Buttons.transform.FindChild("CancelButton").gameObject;
            var newButton = Object.Instantiate(button, button.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = InactiveSprite;
            renderer.color = DefaultColor;
            newButton.transform.position = new Vector3(-0.95f, -0.02f, -1.3f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = button.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener((Action)(() => DoClick(__instance, index)));

            passive.OnMouseOver.RemoveAllListeners();
            passive.OnMouseOver.AddListener((Action)(() => renderer.color = HoverColor));

            passive.OnMouseOut.RemoveAllListeners();
            passive.OnMouseOut.AddListener((Action)(() => renderer.color = DefaultColor));

            Buttons.Add(index, newButton);
            Actives.Add(false);
        }

        public void DoClick(MeetingHud __instance, byte index)
        {
            if (__instance.state != MeetingHud.VoteStates.Discussion)
            {
                if ((__instance.playerStates[index].DidVote && UseAfterVote) || !__instance.playerStates[index].DidVote)
                    Click(index);
            }
        }
    }
}*/