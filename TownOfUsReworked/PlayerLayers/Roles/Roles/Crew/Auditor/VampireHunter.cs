using System;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using Hazel;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class VampireHunter : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastStaked { get; set; }
        public bool VampsDead => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && x.Is(SubFaction.Undead)) == 0;
        private KillButton _stakeButton;

        public VampireHunter(PlayerControl player) : base(player)
        {
            Name = "Vampire Hunter";
            StartText = "Stake The <color=#7B8968FF>Undead</color>";
            AbilitiesText = "Stake the <color=#7B8968FF>Undead</color>";
            Color = CustomGameOptions.CustomCrewColors ? Colors.VampireHunter : Colors.Crew;
            LastStaked = DateTime.UtcNow;
            RoleType = RoleEnum.VampireHunter;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewAudit;
            AlignmentName = "Crew (Auditor)";
            Results = InspResults.VigVHSurvGorg;
        }

        public KillButton StakeButton
        {
            get => _stakeButton;
            set
            {
                _stakeButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public float StakeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStaked;
            var num = CustomGameOptions.StakeCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.EvilRecruit);
            }

            __instance.teamToShow = team;
        }

        public void TurnVigilante()
        {
            var vh = Role.GetRole<VampireHunter>(Player);
            var role = new Vigilante(Player);
            role.RoleHistory.Add(vh);
            role.RoleHistory.AddRange(vh.RoleHistory);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer)
                    role.RegenTask();
            }
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsIntTraitor)
                IntruderWin = true;
            else if (IsSynTraitor)
                SyndicateWin = true;
            else
                CrewWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsIntTraitor || IsIntFanatic)
            {
                if (Utils.IntrudersWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.IntruderWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsSynTraitor || IsSynFanatic)
            {
                if (Utils.SyndicateWins())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SyndicateWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.CrewWins())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.CrewWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}