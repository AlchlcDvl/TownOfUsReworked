using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Medic : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public bool MedicWin;
        public PlayerControl ClosestPlayer;
        public bool UsedAbility { get; set; } = false;
        public PlayerControl ShieldedPlayer { get; set; }
        public PlayerControl exShielded { get; set; }

        public Medic(PlayerControl player) : base(player)
        {
            Name = "Medic";
            ImpostorText = () => "Shield a <color=#8BFDFDFF>Crewmate</color> to protect them";
            TaskText = () => "Protect a <color=#8BFDFDFF>Crewmate</color> using a shield";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medic : Colors.Crew;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Medic;
            Faction = Faction.Crew;
            ShieldedPlayer = null;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = () => "Crew (Protective)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.GAExeMedicPup;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}