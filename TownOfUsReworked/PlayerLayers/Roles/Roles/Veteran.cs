using System;
using UnityEngine;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Veteran : Role
    {
        public bool Enabled;
        public DateTime LastAlerted;
        public float TimeRemaining;
        public int UsesLeft;
        public TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;
        public bool CrewWin;
        public bool OnAlert => TimeRemaining > 0f;

        public Veteran(PlayerControl player) : base(player)
        {
            Name = "Veteran";
            ImpostorText = () => "Alert To Kill Anyone Who Touches You";
            TaskText = () => "Alert to kill whoever interacts with you";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Veteran : Colors.Crew;
            SubFaction = SubFaction.None;
            LastAlerted = DateTime.UtcNow;
            RoleType = RoleEnum.Veteran;
            Faction = Faction.Crew;
            UsesLeft = CustomGameOptions.MaxAlerts;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = () => "Crew (Killing)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.WraithDetGrenVet;
            AddToRoleHistory(RoleType);
        }

        public float AlertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAlerted;
            var num = CustomGameOptions.AlertCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void UnAlert()
        {
            Enabled = false;
            LastAlerted = DateTime.UtcNow;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public void Wins()
        {
            CrewWin = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Data.IsImpostor() |
                x.Is(RoleAlignment.NeutralKill) | x.Is(RoleAlignment.NeutralNeo) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralPros))) ==
                0) | (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.Disconnected && x.Is(Faction.Crew) && !x.Is(ObjectifierEnum.Lovers)
                && !x.Data.TasksDone()) == 0))
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