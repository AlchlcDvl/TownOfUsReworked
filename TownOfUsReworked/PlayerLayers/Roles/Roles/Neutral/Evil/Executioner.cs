using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Executioner : Role
    {
        public PlayerControl TargetPlayer = null;
        public bool TargetVotedOut;
        public List<byte> ToDoom = new List<byte>();
        public bool HasDoomed = false;
        private KillButton _doomButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastDoomed;

        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            StartText = "Eject Your Target";
            Objectives = "- Eject your target.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Executioner : Colors.Neutral;
            RoleType = RoleEnum.Executioner;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            RoleDescription = "You are an Executioner! You are a crazed stalker who only wants to see your target get ejected. Eject them at all costs!";
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            team.Add(TargetPlayer);
            __instance.teamToShow = team;
        }

        public void SetDoomed(MeetingHud __instance)
        {
            if (!TargetVotedOut)
                return;

            ToDoom.Clear();

            foreach (var state in __instance.playerStates)
            {
                if (state.AmDead || Utils.PlayerById(state.TargetPlayerId).Data.Disconnected || state.VotedFor != Player.PlayerId || state.TargetPlayerId == Player.PlayerId)
                    continue;
                
                ToDoom.Add(state.TargetPlayerId);
            }
        }

        public KillButton DoomButton
        {
            get => _doomButton;
            set
            {
                _doomButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public override void Wins()
        {
            if ((Player.Data.IsDead && !CustomGameOptions.ExeCanWinBeyondDeath) || Player.Data.Disconnected || TargetPlayer == null)
                return;

            if (IsRecruit)
                CabalWin = true;
            else if (IsIntAlly)
                IntruderWin = true;
            else if (IsSynAlly)
                SyndicateWin = true;
            else if (IsCrewAlly)
                CrewWin = true;
            else if (IsPersuaded)
                SectWin = true;
            else if (IsBitten)
                UndeadWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
            else if (CustomGameOptions.NoSolo == NoSolo.AllNeutrals)
                AllNeutralsWin = true;
        }

        public float HauntTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoomed;
            var num = CustomGameOptions.HauntCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}