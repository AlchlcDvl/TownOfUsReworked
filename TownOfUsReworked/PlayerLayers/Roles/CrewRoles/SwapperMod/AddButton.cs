using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        public static Sprite ActiveSprite => TownOfUsReworked.SwapperSwitch;
        public static Sprite DisabledSprite => TownOfUsReworked.SwapperSwitchDisabled;

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

            renderer.sprite = DisabledSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(-0.95f, 0.03f, -1.3f);
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
                if (role.ListOfActives.Count(x => x) == 2 && role.MoarButtons[index].GetComponent<SpriteRenderer>().sprite == DisabledSprite)
                    return;

                role.MoarButtons[index].GetComponent<SpriteRenderer>().sprite = role.ListOfActives[index] ? DisabledSprite : ActiveSprite;
                role.ListOfActives[index] = !role.ListOfActives[index];
                SwapVotes.Swap1 = null;
                SwapVotes.Swap2 = null;
                var toSet1 = true;

                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i])
                        continue;

                    if (toSet1)
                    {
                        SwapVotes.Swap1 = MeetingHud.Instance.playerStates[i];
                        toSet1 = false;
                    }
                    else
                        SwapVotes.Swap2 = MeetingHud.Instance.playerStates[i];
                }

                if (SwapVotes.Swap1 == null || SwapVotes.Swap2 == null)
                {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer2.Write((byte)ActionsRPC.SetSwaps);
                    writer2.Write(sbyte.MaxValue);
                    writer2.Write(sbyte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    return;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.SetSwaps);
                writer.Write(SwapVotes.Swap1.TargetPlayerId);
                writer.Write(SwapVotes.Swap2.TargetPlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Swapper))
            {
                var swapper = (Swapper) role;
                swapper.ListOfActives.Clear();
                swapper.MoarButtons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.Is(RoleEnum.Swapper))
                return;

            var swapperrole = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var player = Utils.PlayerById(__instance.playerStates[i].TargetPlayerId);
                var dead = player.Data.IsDead;
                var disconnected = player.Data.Disconnected;
                var swap = player == PlayerControl.LocalPlayer && !CustomGameOptions.SwapSelf && player.Is(RoleEnum.Swapper);
                var voteswap = player == PlayerControl.LocalPlayer && __instance.playerStates[i].DidVote && player.Is(RoleEnum.Swapper) && !CustomGameOptions.SwapAfterVoting;
                GenButton(swapperrole, i, dead || disconnected || swap || voteswap);
            }
        }
    }
}