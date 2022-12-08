using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Consigliere : Role
    {
        public List<byte> Investigated = new List<byte>();
        private KillButton _investigateButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastInvestigated { get; set; }
        public string role = CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "role" : "faction";

        public Consigliere(PlayerControl player) : base(player)
        {
            Name = "Consigliere";
            StartText = "See Players For Who They Really Are";
            AbilitiesText = $"- You can reveal a player's {role}.";
            AttributesText = "- None.";
            Color = CustomGameOptions.CustomImpColors ? Colors.Consigliere : Colors.Intruder;
            RoleType = RoleEnum.Consigliere;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            IntroText = "Kill those who oppose you";
            CoronerDeadReport = "The gun, magnifying glass and documents on the body indicate that they are a Consigliere!";
            CoronerKillerReport = "The gunshot mark seems to have been caused by a gun commonly used by Sheriffs. They were killed by a Consiliere!";
            Results = InspResults.SherConsigInspBm;
            SubFaction = SubFaction.None;
            FactionDescription = "You are an Intruder! Your main task is to kill anyone who dares to oppose you. Sabotage the systems, murder the crew, do anything" +
                " to ensure your victory over others.";
            RoleDescription = "You are a Consigliere! You are a corrupt Inspector who is so capable of finding someone's identity. Help your mate assassinate others" +
                " by revealing players for who they are!";
            AlignmentDescription = "You are a Intruder (Support) role! It is your job to ensure no one bats an eye to the things you or your mates do. Support them in " +
                "everyway possible.";
            Objectives = "- Kill: <color=#008000FF>Syndicate</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.\n   or\n- Have a critical sabotage reach 0 seconds.";
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            IntroSound = null;
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