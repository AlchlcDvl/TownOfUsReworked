using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class AddButton
    {
        public static void GenButton(Swapper role, int index, bool noButton)
        {
            if (noButton)
            {
                role.MoarButtons.Add(null);
                role.ListOfActives.Add(false);
                return;
            }

            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = AssetManager.SwapperSwitchDisabled;
            newButton.transform.position = new Vector3(-0.95f, -0.02f, -1.3f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, index));
            role.MoarButtons.Add(newButton);
            role.ListOfActives.Add(false);
        }

        private static Action SetActive(Swapper role, int index)
        {
            void Listener()
            {
                if (role.ListOfActives.Count(x => x) == 2 && role.MoarButtons[index].GetComponent<SpriteRenderer>().sprite == AssetManager.SwapperSwitchDisabled)
                    return;

                role.MoarButtons[index].GetComponent<SpriteRenderer>().sprite = role.ListOfActives[index] ? AssetManager.SwapperSwitchDisabled : AssetManager.SwapperSwitch;
                role.ListOfActives[index] = !role.ListOfActives[index];
                SwapVotes.Swap1 = null;
                SwapVotes.Swap2 = null;

                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i])
                        continue;

                    if (SwapVotes.Swap1 == null)
                        SwapVotes.Swap1 = MeetingHud.Instance.playerStates[i];
                    else
                        SwapVotes.Swap2 = MeetingHud.Instance.playerStates[i];
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.SetSwaps);

                if (SwapVotes.Swap1 == null || SwapVotes.Swap2 == null)
                {
                    writer.Write(byte.MaxValue);
                    writer.Write(byte.MaxValue);
                }
                else
                {
                    writer.Write(SwapVotes.Swap1.TargetPlayerId);
                    writer.Write(SwapVotes.Swap2.TargetPlayerId);
                }

                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var swapper in Role.GetRoles<Swapper>(RoleEnum.Swapper))
            {
                swapper.ListOfActives.Clear();
                swapper.MoarButtons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.Is(RoleEnum.Swapper))
                return;

            var swapperrole = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var player = Utils.PlayerByVoteArea(__instance.playerStates[i]);
                var dead = player.Data.IsDead;
                var swap = player == PlayerControl.LocalPlayer && !CustomGameOptions.SwapSelf && player.Is(RoleEnum.Swapper);
                var voteswap = player == PlayerControl.LocalPlayer && __instance.playerStates[i].DidVote && player.Is(RoleEnum.Swapper) && !CustomGameOptions.SwapAfterVoting;
                GenButton(swapperrole, i, dead || swap || voteswap);
            }
        }
    }
}