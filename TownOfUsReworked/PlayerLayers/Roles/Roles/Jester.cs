using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Random = UnityEngine.Random;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Jester : Role
    {
        public bool VotedOut;

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            StartText = "It Was Jest A Prank Bro";
            AbilitiesText = "Get ejected!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jester : Colors.Neutral;
            RoleType = RoleEnum.Jester;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            Results = InspResults.JestJuggWWInv;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            VotedOut = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public void Haunt(MeetingHud __instance)
        {
            if (!VotedOut)
                return;
            
            var ToBeHaunted = new List<byte>();

            foreach (var state in __instance.playerStates)
            {
                if (state.AmDead || Utils.PlayerById(state.TargetPlayerId).Data.Disconnected || state.VotedFor != Player.PlayerId || state.TargetPlayerId == Player.PlayerId)
                    continue;
                
                ToBeHaunted.Add(state.TargetPlayerId);
            }

            var random = Random.RandomRangeInt(0, ToBeHaunted.Count);
            var ToBeHauntedPlayer = Utils.PlayerById(ToBeHaunted[random]);
            KillButtonTarget.DontRevive = ToBeHauntedPlayer.PlayerId;
            ToBeHauntedPlayer.Exiled();
            var role = GetRole(ToBeHauntedPlayer);
            role.DeathReason = DeathReasonEnum.Killed;
            role.KilledBy = " By " + Player.name;
        }
    }
}