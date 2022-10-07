using Hazel;
using System;
using System.Linq;
using TownOfUs.Extensions;
using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Juggernaut : Role
    {
        public Juggernaut(PlayerControl player) : base(player)
        {
            Name = "Juggernaut";
            ImpostorText = () => "Your Power Grows With Every Kill";
            TaskText = () => "With each kill your kill cooldown decreases\nFake Tasks:";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.Juggernaut;
            else Color = Patches.Colors.Neutral;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Juggernaut;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public bool JuggernautWins { get; set; }
        public int JuggKills { get; set; } = 0;

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || (x.Is(Alignment.NeutralKill) && !x.Is(RoleEnum.Juggernaut)) || x.Is(Alignment.NeutralChaos) ||
                    x.Is(Alignment.NeutralPower) || x.Data.IsCrew())) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.JuggernautWin,
                    SendOption.Reliable,
                    -1
                );
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            JuggernautWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = (CustomGameOptions.JuggKillCooldown + 5.0f - CustomGameOptions.JuggKillBonus * JuggKills) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var juggTeam = new List<PlayerControl>();
            juggTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = juggTeam;
        }
    }
}