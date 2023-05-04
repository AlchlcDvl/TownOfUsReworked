using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using TownOfUsReworked.Classes;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using System.Linq;
using static UnityEngine.UI.Button;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Dictator : CrewRole
    {
        public bool RoundOne;
        public bool Revealed;
        public List<byte> ToBeEjected;
        public readonly Dictionary<byte, GameObject> MoarButtons = new();
        public readonly Dictionary<byte, bool> Actives = new();
        public CustomButton RevealButton;
        public bool Ejected;
        public bool ToDie;

        public Dictator(PlayerControl player) : base(player)
        {
            Name = "Dictator";
            StartText = "You Have The Final Say";
            AbilitiesText = "- You can reveal yourself to the crew to eject up to 3 players in a meeting\n- When revealed, you cannot be protected";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Dictator : Colors.Crew;
            RoleType = RoleEnum.Dictator;
            RoleAlignment = RoleAlignment.CrewSov;
            AlignmentName = CSv;
            InspectorResults = InspectorResults.Manipulative;
            Type = LayerEnum.Dictator;
            ToBeEjected = new();
            Ejected = false;
            ToDie = false;
            RevealButton = new(this, "DictatorReveal", AbilityTypes.Effect, "ActionSecondary", Reveal);
        }

        public void Reveal()
        {
            if (RoundOne)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.DictatorReveal);
            writer.Write(Player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Revealed = true;
            Utils.Flash(Color);

            foreach (var medic in GetRoles<Medic>(RoleEnum.Medic))
            {
                if (medic.ShieldedPlayer == Player)
                    Medic.BreakShield(medic.PlayerId, Player.PlayerId, true);
            }

            foreach (var ret in GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                if (ret.ShieldedPlayer == Player)
                    Retributionist.BreakShield(ret.PlayerId, Player.PlayerId, true);
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RevealButton.Update("REVEAL", 0, 1, !Revealed, !Revealed && !RoundOne);
        }

        public void GenButton(PlayerVoteArea voteArea, MeetingHud __instance)
        {
            if (IsExempt(voteArea))
            {
                MoarButtons.Add(voteArea.TargetPlayerId, null);
                Actives.Add(voteArea.TargetPlayerId, false);
                return;
            }

            var template = voteArea.Buttons.transform.Find("CancelButton").gameObject;
            var targetBox = Object.Instantiate(template, voteArea.transform);
            targetBox.name = "DictateButton";
            targetBox.transform.localPosition = new(-0.4f, 0.03f, -1.3f);
            var renderer = targetBox.GetComponent<SpriteRenderer>();
            renderer.sprite = AssetManager.GetSprite("SwapDisabled");
            var button = targetBox.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener(SetActive(voteArea, __instance));
            button.OnMouseOut.RemoveAllListeners();
            button.OnMouseOut.AddListener((Action)(() => renderer.color = Actives[voteArea.TargetPlayerId] ? UnityEngine.Color.green : UnityEngine.Color.white));
            button.OnMouseOver.RemoveAllListeners();
            button.OnMouseOver.AddListener((Action)(() => renderer.color = UnityEngine.Color.red));
            var component2 = targetBox.GetComponent<BoxCollider2D>();
            component2.size = renderer.sprite.bounds.size;
            component2.offset = Vector2.zero;
            targetBox.transform.GetChild(0).gameObject.Destroy();
            MoarButtons.Add(voteArea.TargetPlayerId, targetBox);
            Actives.Add(voteArea.TargetPlayerId, false);
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);
            HideButtons();
            ToDie = ToBeEjected.Any(x => Utils.PlayerById(x).Is(Faction.Crew));
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.SetExiles);
            writer.Write(Player.PlayerId);
            writer.Write(ToDie);

            foreach (var bit in ToBeEjected)
                writer.Write(bit);

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        private Action SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
        {
            void Listener()
            {
                if (__instance.playerStates.Any(x => x.TargetPlayerId == Player.PlayerId && x.DidVote && !CustomGameOptions.DictateAfterVoting) || __instance.state ==
                    MeetingHud.VoteStates.Discussion || !Revealed || Ejected || ToDie)
                {
                    return;
                }

                var id = voteArea.TargetPlayerId;

                if (ToBeEjected.Contains(id))
                {
                    ToBeEjected.Remove(id);
                    Actives[id] = false;
                }
                else
                {
                    ToBeEjected.Add(id);
                    Actives[id] = true;
                }

                if (ToBeEjected.Count > 3)
                {
                    Actives[ToBeEjected[0]] = false;
                    ToBeEjected.Remove(ToBeEjected[0]);
                }

                foreach (var pair in MoarButtons)
                {
                    if (MoarButtons[pair.Key] == null)
                        continue;

                    MoarButtons[pair.Key].GetComponent<SpriteRenderer>().sprite = Actives[pair.Key] ? AssetManager.GetSprite("SwapActive") : AssetManager.GetSprite("SwapDisabled");
                    MoarButtons[pair.Key].GetComponent<SpriteRenderer>().color = Actives[pair.Key] ? UnityEngine.Color.green : UnityEngine.Color.white;
                }
            }

            return Listener;
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            foreach (var swapper in GetRoles<Dictator>(RoleEnum.Dictator))
            {
                Actives.Clear();
                MoarButtons.Clear();
                ToBeEjected.Clear();
            }

            foreach (var area in __instance.playerStates)
                GenButton(area, __instance);
        }

        private bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead)
                return true;

            var player = Utils.PlayerByVoteArea(voteArea);
            return player.Data.IsDead || player.Data.Disconnected || (player == Player && player == PlayerControl.LocalPlayer) || IsDead || !Revealed || Ejected;
        }

        public override void ConfirmVotePrefix(MeetingHud __instance)
        {
            base.ConfirmVotePrefix(__instance);

            if (MeetingHud.Instance.playerStates.Any(x => x.TargetPlayerId == Player.PlayerId && x.DidVote && !CustomGameOptions.DictateAfterVoting))
                HideButtons();

            if (ToBeEjected.Count != 0)
                return;

            ToDie = ToBeEjected.Any(x => Utils.PlayerById(x).Is(Faction.Crew));
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.SetExiles);
            writer.Write(Player.PlayerId);
            writer.Write(ToDie);

            foreach (var bit in ToBeEjected)
                writer.Write(bit);

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void HideSingle(byte targetId)
        {
            var button = MoarButtons[targetId];

            if (button == null)
                return;

            button.SetActive(false);
            button.GetComponent<PassiveButton>().OnClick = new ButtonClickedEvent();
            button.GetComponent<PassiveButton>().OnMouseOver.RemoveAllListeners();
            button.GetComponent<PassiveButton>().OnMouseOut.RemoveAllListeners();
            button.Destroy();
            MoarButtons[targetId] = null;
        }

        public void HideButtons()
        {
            for (byte i = 0; i < MoarButtons.Count; i++)
                HideSingle(i);
        }
    }
}