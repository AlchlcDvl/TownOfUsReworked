using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Dracula : Role
    {
        public DateTime LastBitten { get; set; }
        public bool AlreadyConverted;
        public int VampCounter;
        public int DeadVamps;
        private KillButton _biteButton;
        public bool DracWins;
        public PlayerControl ClosestPlayer;
        public List<byte> AliveVamps = new List<byte>();
        public PlayerControl OldestVamp;

        public int VampsAlive => AliveVamps.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead
            && (Utils.PlayerById(x).Is(RoleEnum.Vampire) || Utils.PlayerById(x).Is(RoleEnum.Dracula)));

        public Dracula(PlayerControl player) : base(player)
        {
            Name = "Dracula";
            Faction = Faction.Neutral;
            SubFaction = SubFaction.Vampires;
            RoleType = RoleEnum.Dracula;
            ImpostorText = () => "Lead The Vampires TO Victory";
            TaskText = () => "Convert the <color=#8BFDFDFF>Crew</color>!";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.Dracula;
            else Color = Patches.Colors.Neutral;
            FactionName = "Neutral";
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.NeutralChaos;
            AlignmentName = "Neutral (Chaos)";
            AddToRoleHistory(RoleType);
        }

        public float ConvertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBitten;
            var num = CustomGameOptions.AlertCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Wins()
        {
            DracWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var vampTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            vampTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = vampTeam;
        }

        /*public void Convert()
        {
            System.Console.WriteLine("Convert 1");
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.)
            }
            System.Console.WriteLine("Convert 2");
        }*/

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Alignment.NeutralKill) || x.Data.IsCrew())) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.VampWin,
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
    }
}