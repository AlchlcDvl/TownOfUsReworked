using Hazel;
using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Juggernaut : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public bool JuggernautWins { get; set; }
        public int JuggKills { get; set; } = 0;

        public Juggernaut(PlayerControl player) : base(player)
        {
            Name = "Juggernaut";
            StartText = "Your Power Grows With Every Kill";
            AbilitiesText = "With each kill your kill cooldown decreases\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Juggernaut : Colors.Neutral;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Juggernaut;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            Results = InspResults.JestJuggWWInv;
            Attack = AttackEnum.Basic;
            AttackString = "Basic";
            AddToRoleHistory(RoleType);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CabalWin,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    Wins();
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.NKWins(RoleType))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,(byte)CustomRPC.JuggernautWin,
                    SendOption.Reliable,-1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else
                JuggernautWins = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = (CustomGameOptions.JuggKillCooldown - CustomGameOptions.JuggKillBonus * JuggKills) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }
    }
}