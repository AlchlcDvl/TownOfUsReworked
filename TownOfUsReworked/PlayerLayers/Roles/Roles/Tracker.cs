using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Tracker : Role
    {
        public Dictionary<byte, ArrowBehaviour> TrackerArrows = new Dictionary<byte, ArrowBehaviour>();
        public PlayerControl ClosestPlayer;
        public DateTime LastTracked { get; set; }
        public int UsesLeft;
        public TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;

        public Tracker(PlayerControl player) : base(player)
        {
            Name = "Tracker";
            StartText = "Stalk Everyone To Monitor Their Movements";
            AbilitiesText = "Track suspicious players";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Tracker : Colors.Crew;
            LastTracked = DateTime.UtcNow;
            RoleType = RoleEnum.Tracker;
            Faction = Faction.Crew;
            UsesLeft = CustomGameOptions.MaxTracks;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.TrackAltTLTM;
            IntroSound = null;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            AddToRoleHistory(RoleType);
        }

        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTracked;
            var num = CustomGameOptions.TrackCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool IsTracking(PlayerControl player)
        {
            return TrackerArrows.ContainsKey(player.PlayerId);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = TrackerArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);

            TrackerArrows.Remove(arrow.Key);
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