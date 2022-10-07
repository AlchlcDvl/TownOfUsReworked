using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Arsonist : Role
    {
        private KillButton _igniteButton;
        public bool ArsonistWins;
        public PlayerControl ClosestPlayerDouse;
        public PlayerControl ClosestPlayerIgnite;
        public List<byte> DousedPlayers = new List<byte>();
        public DateTime LastDoused;
        public DateTime LastIgnited;

        public int DousedAlive => DousedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead);


        public Arsonist(PlayerControl player) : base(player)
        {
            Name = "Arsonist";
            ImpostorText = () => "Gasoline + Bean + Lighter = Bonfire";
            TaskText = () => "Douse players in gas and ignite them\nFake Tasks:";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.Arsonist;
            else Color = Patches.Colors.Neutral;
            LastDoused = DateTime.UtcNow;
            RoleType = RoleEnum.Arsonist;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            AddToRoleHistory(RoleType);
        }

        public KillButton IgniteButton
        {
            get => _igniteButton;
            set
            {
                _igniteButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || (x.Is(Alignment.NeutralKill) && !x.Is(RoleEnum.Arsonist)) || x.Is(Alignment.NeutralChaos) ||
                    x.Is(Alignment.NeutralPower) || x.Data.IsCrew())) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.ArsonistWin,
                    SendOption.Reliable,
                    -1);
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
            ArsonistWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var arsonistTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            arsonistTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = arsonistTeam;
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = CustomGameOptions.DouseCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float IgniteTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = CustomGameOptions.IgniteCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ignite()
        {
            System.Console.WriteLine("Ignite 1");
            foreach (var playerId in DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                if (player == null ||
                    player.Data.Disconnected ||
                    player.Data.IsDead ||
                    player.Is(RoleEnum.Pestilence))
                    continue;
                Utils.MurderPlayer(Player, player);
            }
            DousedPlayers.Clear();
            System.Console.WriteLine("Ignite 2");
        }
    }
}
