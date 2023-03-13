using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Executioner : NeutralRole
    {
        public PlayerControl TargetPlayer = null;
        public bool TargetVotedOut;
        public List<byte> ToDoom;
        public bool HasDoomed = false;
        public AbilityButton DoomButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastDoomed;
        public int MaxUses;
        public bool CanDoom => TargetVotedOut && !HasDoomed && MaxUses > 0 && ToDoom.Count > 0;
        public bool Failed => !TargetVotedOut && (TargetPlayer == null || TargetPlayer.Data.IsDead || TargetPlayer.Data.Disconnected);

        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            StartText = "Eject Your Target";
            Objectives = "- Eject your target.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Executioner : Colors.Neutral;
            RoleType = RoleEnum.Executioner;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            RoleDescription = "You are an Executioner! You are a crazed stalker who only wants to see your target get ejected. Eject them at all costs!";
            ToDoom = new List<byte>();
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
            var timeSpan = utcNow - LastDoomed;
            var num = CustomGameOptions.DoomCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TurnJest()
        {
            var exe = Role.GetRole<Executioner>(Player);
            var newRole = new Jester(Player);
            newRole.RoleUpdate(exe);
            Player.RegenTask();
        }
    }
}