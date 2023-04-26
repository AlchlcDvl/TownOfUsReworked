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
using Reactor.Utilities.Extensions;

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

        public void GenButton(PlayerVoteArea voteArea, bool noButton)
        {
            if (PlayerControl.LocalPlayer != Player)
                return;

            if (noButton)
            {
                MoarButtons.Add(null);
                ListOfActives.Add(false);
                return;
            }

            var button = voteArea.Buttons.transform.Find("CancelButton").gameObject;
            var newButton = Object.Instantiate(voteArea.Buttons.transform.Find("CancelButton").gameObject, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = AssetManager.GetSprite("SwapDisabled");
            newButton.name = "SwapButton";
            newButton.transform.localPosition = new(-0.95f, -0.02f, -1.3f);
            newButton.layer = 5;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(MeetingHud.Instance.playerStates.IndexOf(voteArea)));

            var component = newButton.GetComponent<BoxCollider2D>();
            component.size = renderer.sprite.bounds.size;
            component.offset = Vector2.zero;
            newButton.transform.GetChild(0).gameObject.Destroy();

            MoarButtons.Add(newButton);
            ListOfActives.Add(false);
        }

        private Action SetActive(int index)
        {
            void Listener()
            {
                if (MeetingHud.Instance.playerStates.Any(x => x.TargetPlayerId == Player.PlayerId && x.DidVote) || (ListOfActives.Count(x => x) == 2 &&
                    MoarButtons[index].GetComponent<SpriteRenderer>().sprite == AssetManager.GetSprite("SwapDisabled")))
                {
                    return;
                }

                MoarButtons[index].GetComponent<SpriteRenderer>().sprite = ListOfActives[index] ? AssetManager.GetSprite("SwapActive") : AssetManager.GetSprite("SwapDisabled");
                ListOfActives[index] = !ListOfActives[index];
                Swap1 = null;
                Swap2 = null;

                for (var i = 0; i < ListOfActives.Count; i++)
                {
                    if (!ListOfActives[i])
                        continue;

                    if (Swap1 == null)
                        Swap1 = MeetingHud.Instance.playerStates[i];
                    else
                        Swap2 = MeetingHud.Instance.playerStates[i];
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.SetSwaps);
                writer.Write(Player.PlayerId);

                if (Swap1 == null || Swap2 == null)
                {
                    writer.Write(byte.MaxValue);
                    writer.Write(byte.MaxValue);
                }
                else
                {
                    writer.Write(Swap1.TargetPlayerId);
                    writer.Write(Swap2.TargetPlayerId);
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

            if (PlayerControl.LocalPlayer != Player)
                return;

            foreach (var area in __instance.playerStates)
            {
                var player = Utils.PlayerByVoteArea(area);
                var dead = player.Data.IsDead;
                var swap = player == PlayerControl.LocalPlayer && !CustomGameOptions.SwapSelf && player.Is(RoleEnum.Swapper);
                GenButton(area, dead || swap);
            }
        }
    }
}