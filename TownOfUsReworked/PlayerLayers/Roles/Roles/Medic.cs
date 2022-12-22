using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Medic : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public PlayerControl ClosestPlayer;
        public bool UsedAbility { get; set; } = false;
        public PlayerControl ShieldedPlayer { get; set; }
        public PlayerControl exShielded { get; set; }

        public Medic(PlayerControl player) : base(player)
        {
            Name = "Medic";
            StartText = "Shield a <color=#8BFDFDFF>Crewmate</color> to protect them";
            AbilitiesText = "Protect a <color=#8BFDFDFF>Crewmate</color> using a shield";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medic : Colors.Crew;
            RoleType = RoleEnum.Medic;
            Faction = Faction.Crew;
            ShieldedPlayer = null;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = "Crew (Protective)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.GAExeMedicPup;
            IntroSound = TownOfUsReworked.MedicIntro;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            CrewWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (Utils.CrewWins())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CrewWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}