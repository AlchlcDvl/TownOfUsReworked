using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Dracula : Role
    {
        public DateTime LastBitten { get; set; }
        public bool AlreadyConverted;
        public bool HasMaj;
        private KillButton _biteButton;
        public bool DracWins;
        public PlayerControl ClosestPlayer;
        public List<PlayerControl> AllVamps = new List<PlayerControl>();

        public Dracula(PlayerControl player) : base(player)
        {
            Name = "Dracula";
            Faction = Faction.Neutral;
            RoleType = RoleEnum.Dracula;
            ImpostorText = () => "Lead The <color=#7B8968FF>Vampires</color> To Victory";
            TaskText = () => "Convert the <color=#8BFDFDFF>Crew</color>!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Dracula : Colors.Neutral;
            SubFaction = SubFaction.Vampires;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = () => "Neutral (Neophyte)";
            IntroText = "Convert the <color=#8BFDFDFF>Crew</color> to gain a majority";
            CoronerDeadReport = "There are sharp fangs and pale skin on the body. They must be a Dracula!";
            CoronerKillerReport = "The body seems to have been drained of blood. They were sucked by a Dracula!";
            Results = InspResults.ShiftSwapSKDrac;
            AddToRoleHistory(RoleType);
        }

        public float ConvertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBitten;
            var num = CustomGameOptions.AlertCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public override void Wins()
        {
            DracWins = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var vampTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            vampTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = vampTeam;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                (x.Data.IsImpostor() | x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Crew) && !(x.Is(SubFaction.Vampires) |
                x.Is(RoleAlignment.NeutralBen)))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.DracWin,
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

        public KillButton BiteButton
        {
            get => _biteButton;
            set
            {
                _biteButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public List<PlayerControl> Vamps()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(SubFaction.Vampires))
                    AllVamps.Add(player);
            }

            return AllVamps;
        }

        public List<PlayerControl> AliveVamps()
        {
            var vamp = Vamps();

            foreach (var player in vamp)
            {
                if (player.Data.IsDead | player.Data.Disconnected)
                    vamp.Remove(player);
            }

            return vamp;
        }
    }
}