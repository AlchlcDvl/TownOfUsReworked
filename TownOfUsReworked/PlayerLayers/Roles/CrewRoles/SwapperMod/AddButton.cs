using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        private static int _mostRecentId;
        private static Sprite ActiveSprite => TownOfUsReworked.SwapperSwitch;
        public static Sprite DisabledSprite => TownOfUsReworked.SwapperSwitchDisabled;

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead)
                return true;

            var player = Utils.PlayerById(voteArea.TargetPlayerId);

            if (PlayerControl.LocalPlayer.Is(Faction.Neutral) || PlayerControl.LocalPlayer.Is(Faction.Crew) || PlayerControl.LocalPlayer.Is(Faction.Syndicate))
            {
                if (player == null || player.Data.IsDead || player.Data.Disconnected)
                    return true;
            }
            else
            {
                if (player == null || player.Data.IsImpostor() || player.Data.IsDead || player.Data.Disconnected)
                    return true;
            }

            var role = Role.GetRole(player);
            return role != null;
        }

        public static void GenButton(Swapper role, PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;

            if (IsExempt(voteArea))
            {
                role.MoarButtons[targetId] = null;
                role.ListOfActives[targetId] = false;
                return;
            }

            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(confirmButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = DisabledSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.5f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(Swap(role, voteArea));
            role.MoarButtons.Add(newButton);
            role.ListOfActives.Add(false);
        }

        private static Action Swap(Swapper role, PlayerVoteArea voteArea)
        {
            void Listener()
            {
                var targetId = voteArea.TargetPlayerId;

                if (role.ListOfActives.Count(x => x) == 2 && role.MoarButtons[targetId].GetComponent<SpriteRenderer>().sprite == DisabledSprite)
                    return;

                role.MoarButtons[targetId].GetComponent<SpriteRenderer>().sprite = role.ListOfActives[targetId] ? DisabledSprite : ActiveSprite;
                role.ListOfActives[targetId] = !role.ListOfActives[targetId];

                _mostRecentId = targetId;
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage(string.Join(" ", role.ListOfActives));

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
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetSwaps, SendOption.Reliable, -1);
                    writer2.Write(sbyte.MaxValue);
                    writer2.Write(sbyte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    return;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetSwaps, SendOption.Reliable, -1);
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
                var swapper = (Swapper)role;
                swapper.ListOfActives.Clear();
                swapper.MoarButtons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swapper))
                return;

            var swapperrole = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);

            foreach (var state in __instance.playerStates)
                GenButton(swapperrole, state);
        }
    }
}