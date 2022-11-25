using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Consigliere : Role
    {
        public System.Collections.Generic.List<byte> Investigated = new System.Collections.Generic.List<byte>();
        public KillButton _investigateButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastInvestigated { get; set; }

        public Consigliere(PlayerControl player) : base(player)
        {
            Name = "Consigliere";
            ImpostorText = () => "Reveal Everyone's Roles";
            TaskText = () => "Investigate the roles of the <color=#8BFDFD>Crew</color>";
            Color = CustomGameOptions.CustomImpColors ? Colors.Consigliere : Colors.Intruder;
            RoleType = RoleEnum.Consigliere;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = () => "Intruder (Support)";
            IntroText = "Kill those who oppose you";
            CoronerDeadReport = $"The gun, magnifying glass and documents on the body indicate that they are a Consigliere!";
            CoronerKillerReport = "The gunshot mark seems to have been caused by a gun commonly used by Investigators. They were killed by a Consiliere!";
            Results = InspResults.SherConsigInspBm;
            SubFaction = SubFaction.None;
            AddToRoleHistory(RoleType);
        }

        public float ConsigliereTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = CustomGameOptions.ConsigCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton InvestigateButton
        {
            get => _investigateButton;
            set
            {
                _investigateButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var intTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }

        public override void Wins()
        {
            IntruderWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Crew) |
                x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0) |
                Utils.Sabotaged())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.IntruderWin,
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