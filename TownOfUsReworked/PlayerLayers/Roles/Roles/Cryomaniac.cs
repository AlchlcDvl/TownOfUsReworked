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
    public class Cryomaniac : Role
    {
        private KillButton _freezeButton;
        public bool CryoWins;
        public PlayerControl ClosestPlayer;
        public List<byte> DousedPlayers = new List<byte>();
        public bool FreezeUsed;
        public DateTime LastDoused;

        public Cryomaniac(PlayerControl player) : base(player)
        {
            Name = "Cryomaniac";
            ImpostorText = () => "Douse players and ignite the light";
            TaskText = () => "Douse players and ignite to kill everyone\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Cryomaniac : Colors.Neutral;
            SubFaction = SubFaction.None;
            LastDoused = DateTime.UtcNow;
            RoleType = RoleEnum.Cryomaniac;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = () => "Neutral (Killing)";
            IntroText = "Ignite those who oppose you";
            CoronerDeadReport = "There are burn marks and a smell of gasoline. They must be an Arsonist!";
            CoronerKillerReport = "The bodies have been completely charred. They were torched by an Arsonist!";
            Results = InspResults.ArsoCryoPBOpTroll;
            AddToRoleHistory(RoleType);
        }

        public KillButton FreezeButton
        {
            get => _freezeButton;
            set
            {
                _freezeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (CheckEveryoneDoused())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CryomaniacWin,
                    SendOption.Reliable, -1);
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
            CryoWins = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public bool CheckEveryoneDoused()
        {
            var cryoId = Player.PlayerId;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == cryoId | player.Data.IsDead | player.Data.Disconnected)
                    continue;

                if (!DousedPlayers.Contains(player.PlayerId))
                    return false;
            }

            return true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var cryoTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            cryoTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = cryoTeam;
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = CustomGameOptions.DouseCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
