using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Jester : NeutralRole
    {
        public bool VotedOut;
        public List<byte> ToHaunt;
        public bool HasHaunted = false;
        public AbilityButton HauntButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastHaunted;
        public int MaxUses;
        public bool CanHaunt => VotedOut && !HasHaunted && MaxUses > 0 && ToHaunt.Count > 0;

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            StartText = "It Was Jest A Prank Bro";
            AbilitiesText = "- After you get ejected, you can haunt a player.";
            Objectives = "- Get ejected.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jester : Colors.Neutral;
            RoleType = RoleEnum.Jester;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            RoleDescription = "You are a Jester! You are a suicidal lunatic who wants to be thrown out of the airlock. Get yourself ejected at all costs!";
            ToHaunt = new List<byte>();
            MaxUses = CustomGameOptions.HauntCount <= ToHaunt.Count ? CustomGameOptions.HauntCount : ToHaunt.Count;
        }

        public void SetHaunted(MeetingHud __instance)
        {
            if (!VotedOut)
                return;

            ToHaunt.Clear();

            foreach (var state in __instance.playerStates)
            {
                if (state.AmDead || Utils.PlayerById(state.TargetPlayerId).Data.Disconnected || state.VotedFor != Player.PlayerId || state.TargetPlayerId == Player.PlayerId)
                    continue;
                
                ToHaunt.Add(state.TargetPlayerId);
            }

            while (ToHaunt.Count > CustomGameOptions.HauntCount)
            {
                ToHaunt.Shuffle();
                ToHaunt.Remove(ToHaunt[ToHaunt.Count - 1]);
            }
        }

        public float HauntTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastHaunted;
            var num = CustomGameOptions.HauntCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}