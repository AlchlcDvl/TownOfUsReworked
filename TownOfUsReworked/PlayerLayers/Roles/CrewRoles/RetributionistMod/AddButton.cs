using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        public static void GenButton(Retributionist role, int index, bool canRevive)
        {
            if (!canRevive)
            {
                role.OtherButtons.Add(null);
                role.ListOfActives.Add(false);
                return;
            }

            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = AssetManager.SwapperSwitchDisabled;
            newButton.transform.position = confirmButton.transform.position - new Vector3(-0.95f, 0.03f, -1.3f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, index));
            role.OtherButtons.Add(newButton);
            role.ListOfActives.Add(false);
        }

        private static Action SetActive(Retributionist role, int index)
        {
            void Listener()
            {
                if (role.ListOfActives.Count(x => x) == 1 && role.OtherButtons[index].GetComponent<SpriteRenderer>().sprite == AssetManager.SwapperSwitchDisabled)
                {
                    int active = 0;

                    for (var i = 0; i < role.ListOfActives.Count; i++)
                    {
                        if (role.ListOfActives[i])
                            active = i;
                    }

                    role.OtherButtons[active].GetComponent<SpriteRenderer>().sprite = role.ListOfActives[active] ? AssetManager.SwapperSwitchDisabled : AssetManager.SwapperSwitch;
                    role.ListOfActives[active] = !role.ListOfActives[active];
                }

                role.OtherButtons[index].GetComponent<SpriteRenderer>().sprite = role.ListOfActives[index] ? AssetManager.SwapperSwitchDisabled : AssetManager.SwapperSwitch;
                role.ListOfActives[index] = !role.ListOfActives[index];
                SetRevive.Imitate = null;

                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i])
                        continue;

                    SetRevive.Imitate = MeetingHud.Instance.playerStates[i];
                }
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Retributionist))
            {
                var ret = (Retributionist)role;
                ret.ListOfActives.Clear();
                ret.OtherButtons.Clear();
            }

            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return;

            var retRole = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == __instance.playerStates[i].TargetPlayerId)
                    {
                        var revivable = false;
                        var revivedRole = Role.GetRole(player).RoleType;

                        if (revivedRole == RoleEnum.Revealer)
                        {
                            var haunter = Role.GetRole<Revealer>(player);
                            revivedRole = haunter.FormerRole.RoleType;
                        }

                        if (player.Data.IsDead && !player.Data.Disconnected && (revivedRole == RoleEnum.Detective || revivedRole == RoleEnum.Seer || revivedRole == RoleEnum.Mystic ||
                            revivedRole == RoleEnum.Agent || revivedRole == RoleEnum.Tracker || revivedRole == RoleEnum.Medic || revivedRole == RoleEnum.Sheriff || 
                            revivedRole == RoleEnum.Veteran || revivedRole == RoleEnum.Altruist || revivedRole == RoleEnum.Engineer || revivedRole == RoleEnum.Vigilante ||
                            revivedRole == RoleEnum.Medium || revivedRole == RoleEnum.Operative || revivedRole == RoleEnum.Inspector || revivedRole == RoleEnum.Chameleon ||
                            revivedRole == RoleEnum.Coroner || revivedRole == RoleEnum.VampireHunter))
                            revivable = true;

                        if (retRole.Used.Contains(player.PlayerId))
                            revivable = false;

                        GenButton(retRole, i, revivable);
                    }
                }
            }
        }
    }
}