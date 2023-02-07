using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using Il2CppSystem.Collections.Generic;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Godfather : Role
    {
        public PlayerControl ClosestPlayer;
        public bool HasDeclared = false;
        public bool WasMafioso = false;
        public Role FormerRole = null;
        private KillButton _declareButton;
        private KillButton _killButton;
        public DateTime LastKilled { get; set; }
        public DateTime LastDeclared;

        public Godfather(PlayerControl player) : base(player)
        {
            Name = "Godfather";
            Faction = Faction.Intruder;
            RoleType = RoleEnum.Godfather;
            StartText = "Promote Your Fellow <color=#FF0000FF>Intruders</color> To Do Better";
            AbilitiesText = "- You can promote a fellow <color=#FF0000FF>Intruder</color> into becoming your successor.";
            AttributesText = "- Promoting an <color=#FF0000FF>Intruder</color> turns them into a <color=#6400FFFF>Mafioso</color>.\n- If you die, " +
                "the <color=#6400FFFF>Mafioso</color> become the new <color=#404C08FF>Godfather</color>\nand inherits better abilities of their former" +
                " role.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Godfather : Colors.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            Results = InspResults.GFMayorRebelPest;
            FactionDescription = IntruderFactionDescription;
            Objectives = IntrudersWinCon;
            AlignmentDescription = ISDescription;
            RoleDescription = "You are the Godfather! You are the leader of the Intruders. You can promote a fellow Intruder into becoming your Mafioso." +
                " When you die, the Mafioso will become the new Godfather and will inherit stronger variations of their former role!";
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.IntKillCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else
                IntruderWin = true;
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
            else if (Utils.IntrudersWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            return false;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (!IsRecruit)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                        team.Add(player);
                }
            }
            else
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public KillButton DeclareButton
        {
            get => _declareButton;
            set
            {
                _declareButton = value;
                AddToAbilityButtons(value, this);
            }
        }
    }
}