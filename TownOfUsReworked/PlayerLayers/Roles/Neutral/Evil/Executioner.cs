using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Custom;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Executioner : NeutralRole
    {
        public PlayerControl TargetPlayer;
        public bool TargetVotedOut;
        public List<byte> ToDoom = new();
        public bool HasDoomed;
        public CustomButton DoomButton;
        public DateTime LastDoomed;
        public int UsesLeft;
        public bool CanDoom => TargetVotedOut && !HasDoomed && UsesLeft > 0 && ToDoom.Count > 0;
        public bool Failed => TargetPlayer == null || (!TargetVotedOut && (TargetPlayer.Data.IsDead || TargetPlayer.Data.Disconnected));

        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            StartText = "Eject Your Target";
            Objectives = "- Eject your target";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Executioner : Colors.Neutral;
            RoleType = RoleEnum.Executioner;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            ToDoom = new();
            UsesLeft = CustomGameOptions.DoomCount;
            AbilitiesText = "- After your target has been ejected, you can doom players who voted for them\n- If your target dies, you will become a <color=#F7B3DAFF>Jester</color>";
            Type = LayerEnum.Executioner;
            DoomButton = new(this, "Doom", AbilityTypes.Direct, "ActionSecondary", Doom, Exception, true);
            InspectorResults = InspectorResults.Manipulative;
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);

            if (TargetVotedOut)
                return;

            ToDoom.Clear();

            foreach (var state in __instance.playerStates)
            {
                if (state.AmDead || Utils.PlayerByVoteArea(state).Data.Disconnected || state.VotedFor != TargetPlayer.PlayerId || state.TargetPlayerId == Player.PlayerId)
                    continue;

                ToDoom.Add(state.TargetPlayerId);
            }

            while (ToDoom.Count > UsesLeft)
            {
                ToDoom.Shuffle();
                ToDoom.Remove(ToDoom[^1]);
            }

            UsesLeft = CustomGameOptions.DoomCount <= ToDoom.Count ? CustomGameOptions.DoomCount : ToDoom.Count;
        }

        public float DoomTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDoomed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DoomCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnJest()
        {
            var newRole = new Jester(Player);
            newRole.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Jester);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public void Doom()
        {
            if (Utils.IsTooFar(Player, DoomButton.TargetPlayer) || DoomTimer() != 0f || !CanDoom)
                return;

            Utils.RpcMurderPlayer(Player, DoomButton.TargetPlayer, DeathReasonEnum.Doomed, false);
            HasDoomed = true;
            UsesLeft--;
            LastDoomed = DateTime.UtcNow;
        }

        public bool Exception(PlayerControl player) => !ToDoom.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(ObjectifierEnum.Mafia) &&
            Player.Is(ObjectifierEnum.Mafia));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            DoomButton.Update("DOOM", DoomTimer(), CustomGameOptions.DoomCooldown, UsesLeft, CanDoom, CanDoom);

            if (Failed && !IsDead)
            {
                TurnJest();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnJest);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}