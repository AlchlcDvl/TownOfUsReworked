using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Executioner : NeutralRole
    {
        public PlayerControl TargetPlayer;
        public bool TargetVotedOut;
        public List<byte> ToDoom = new();
        public bool HasDoomed;
        public AbilityButton DoomButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastDoomed;
        public int MaxUses;
        public bool CanDoom => TargetVotedOut && !HasDoomed && MaxUses > 0 && ToDoom.Count > 0;
        public bool Failed => !TargetVotedOut && (TargetPlayer?.Data.IsDead == true || TargetPlayer?.Data.Disconnected == true);

        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            StartText = "Eject Your Target";
            Objectives = "- Eject your target.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Executioner : Colors.Neutral;
            RoleType = RoleEnum.Executioner;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            ToDoom = new();
            MaxUses = CustomGameOptions.DoomCount <= ToDoom.Count ? CustomGameOptions.DoomCount : ToDoom.Count;
        }

        public void SetDoomed(MeetingHud __instance)
        {
            if (!TargetVotedOut)
                return;

            ToDoom.Clear();

            foreach (var state in __instance.playerStates)
            {
                if (state.AmDead || Utils.PlayerByVoteArea(state).Data.Disconnected || state.VotedFor != TargetPlayer.PlayerId || state.TargetPlayerId == Player.PlayerId)
                    continue;

                ToDoom.Add(state.TargetPlayerId);
            }
        }

        public float DoomTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDoomed;
            var num = CustomGameOptions.DoomCooldown * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnJest()
        {
            var newRole = new Jester(Player);
            newRole.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Coroutines.Start(Utils.FlashCoroutine(Color));

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Seer));
        }
    }
}