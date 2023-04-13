using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class AddButton
    {
        public static void GenButton(Retributionist role, PlayerVoteArea index, bool noButton)
        {
            if (noButton)
            {
                role.OtherButtons.Add(null);
                role.ListOfActives.Add(false);
                return;
            }

            var confirmButton = index.Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(confirmButton, index.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = AssetManager.SwapperSwitchDisabled;
            newButton.transform.position = new Vector3(-0.95f, -0.02f, -1.3f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, index));
            role.OtherButtons.Add(newButton);
            role.ListOfActives.Add(false);
        }

        private static Action SetActive(Retributionist role, PlayerVoteArea voteArea)
        {
            void Listener()
            {
                if (role.ListOfActives.Count(x => x) == 1 && role.OtherButtons[voteArea.TargetPlayerId].GetComponent<SpriteRenderer>().sprite == AssetManager.SwapperSwitchDisabled)
                {
                    var active = 0;

                    for (var i = 0; i < role.ListOfActives.Count; i++)
                    {
                        if (role.ListOfActives[i])
                            active = i;
                    }

                    role.OtherButtons[active].GetComponent<SpriteRenderer>().sprite = role.ListOfActives[active] ? AssetManager.SwapperSwitchDisabled : AssetManager.SwapperSwitch;
                    role.ListOfActives[active] = !role.ListOfActives[active];
                }

                role.OtherButtons[voteArea.TargetPlayerId].GetComponent<SpriteRenderer>().sprite = role.ListOfActives[voteArea.TargetPlayerId] ? AssetManager.SwapperSwitchDisabled :
                    AssetManager.SwapperSwitch;
                role.ListOfActives[voteArea.TargetPlayerId] = !role.ListOfActives[voteArea.TargetPlayerId];
                VotingComplete.Imitate = null;

                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i])
                        continue;

                    VotingComplete.Imitate = MeetingHud.Instance.playerStates[i];
                }
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var ret in Role.GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                ret.ListOfActives.Clear();
                ret.OtherButtons.Clear();
            }

            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return;

            var retRole = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            foreach (var voteArea in __instance.playerStates)
                GenButton(retRole, voteArea, !voteArea.AmDead);
        }
    }
}