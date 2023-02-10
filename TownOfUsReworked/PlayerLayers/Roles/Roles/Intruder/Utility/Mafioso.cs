using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using System;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Mafioso : Role
    {
        public Role FormerRole = null;
        public Godfather Godfather;
        public bool CanPromote => Godfather.Player.Data.IsDead || Godfather.Player.Data.Disconnected;
        private KillButton _killButton;
        public DateTime LastKilled { get; set; }
        public PlayerControl ClosestPlayer = null;

        public Mafioso(PlayerControl player) : base(player)
        {
            Name = "Mafioso";
            Faction = Faction.Intruder;
            RoleType = RoleEnum.Mafioso;
            StartText = "Succeed The <color=#404C08FF>Godfather</color>";
            AbilitiesText = "- When the <color=#404C08FF>Godfather</color> dies, you will become the new <color=#404C08FF>Godfather</color> with boosted abilities of your former role.";
            AttributesText = "- None.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Mafioso : Colors.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderUtil;
            AlignmentName = "Intruder (Utility)";
            FactionDescription = "You are an Intruder! Your main task is to kill anyone who dares to oppose you. Sabotage the systems, murder the crew, do anything" +
                " to ensure your victory over others.";
            Objectives = "- Kill: <color=#008000FF>Syndicate</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.\n   or\n- Have a critical sabotage reach 0 seconds.";
            AlignmentDescription = "You are a Intruder (Utility) role! You usually have no special ability and cannot even appear under natural conditions.";
            RoleDescription = "You have become a Mafioso! You are the successor to the leader of the Intruders. When the Godfather dies, you will become the new" +
                " Godfather and will inherit stronger variations of your former role.";
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
            else if (IsPersuaded)
                SectWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
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
            else if (IsPersuaded)
            {
                if (Utils.SectWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SectWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsResurrected)
            {
                if (Utils.ReanimatedWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.ReanimatedWin);
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

        public void TurnGodfather()
        {
            var mafioso = Role.GetRole<Mafioso>(Player);
            var formerRole = mafioso.FormerRole;
            var role = new Godfather(Player);
            role.WasMafioso = true;
            role.HasDeclared = !CustomGameOptions.PromotedMafiosoCanPromote;
            role.FormerRole = formerRole;
            role.RoleHistory.Add(mafioso);
            role.RoleHistory.AddRange(mafioso.RoleHistory);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer)
                {
                    role.RegenTask();
                    Coroutines.Start(Utils.FlashCoroutine(Color));
                }
            }
        }
    }
}