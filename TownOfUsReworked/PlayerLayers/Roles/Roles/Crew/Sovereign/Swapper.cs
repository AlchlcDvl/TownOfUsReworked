using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System;
using System.Linq;
using Hazel;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Swapper : CrewRole
    {
        public readonly List<GameObject> MoarButtons = new();
        public readonly List<bool> ListOfActives = new();
        public PlayerVoteArea Swap1;
        public PlayerVoteArea Swap2;

        public Swapper(PlayerControl player) : base(player)
        {
            Name = "Swapper";
            StartText = "Swap Votes For Maximum Chaos";
            AbilitiesText = "- You can swap the votes against 2 players in meetings";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Swapper : Colors.Crew;
            RoleType = RoleEnum.Swapper;
            RoleAlignment = RoleAlignment.CrewSov;
            AlignmentName = CSv;
            InspectorResults = InspectorResults.BringsChaos;
            MoarButtons = new();
            ListOfActives = new();
            Type = LayerEnum.Swapper;
        }

        public static void GenButton(Swapper role, int index, bool noButton)
        {
            if (noButton)
            {
                role.MoarButtons.Add(null);
                role.ListOfActives.Add(false);
                return;
            }

            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(confirmButton, confirmButton.transform);
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
                role.Swap1 = null;
                role.Swap2 = null;

                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i])
                        continue;

                    if (role.Swap1 == null)
                        role.Swap1 = MeetingHud.Instance.playerStates[i];
                    else
                        role.Swap2 = MeetingHud.Instance.playerStates[i];
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.SetSwaps);
                writer.Write(role.Player.PlayerId);

                if (role.Swap1 == null || role.Swap2 == null)
                {
                    writer.Write(byte.MaxValue);
                    writer.Write(byte.MaxValue);
                }
                else
                {
                    writer.Write(role.Swap1.TargetPlayerId);
                    writer.Write(role.Swap2.TargetPlayerId);
                }

                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            return Listener;
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            foreach (var swapper in GetRoles<Swapper>(RoleEnum.Swapper))
            {
                swapper.ListOfActives.Clear();
                swapper.MoarButtons.Clear();
                swapper.Swap1 = null;
                swapper.Swap2 = null;
            }

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var player = Utils.PlayerByVoteArea(__instance.playerStates[i]);
                var dead = player.Data.IsDead;
                var swap = player == PlayerControl.LocalPlayer && !CustomGameOptions.SwapSelf && player.Is(RoleEnum.Swapper);
                var voteswap = player == PlayerControl.LocalPlayer && __instance.playerStates[i].DidVote && player.Is(RoleEnum.Swapper) && !CustomGameOptions.SwapAfterVoting;
                GenButton(this, i, dead || swap || voteswap);
            }
        }
    }
}