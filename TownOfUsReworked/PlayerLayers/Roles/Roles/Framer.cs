using System;
using System.Collections.Generic;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Framer : Role
    {
        private KillButton _frameButton;
        public PlayerControl ClosestPlayer;
        public List<byte> DousedPlayers = new List<byte>();
        public DateTime LastFramed;

        public Framer(PlayerControl player) : base(player)
        {
            Name = "Framer";
            StartText = "Make Everyone Suspicious";
            AbilitiesText = "- You can frame players in gasoline.";
            AttributesText = "- Framed players will die very easily to <color=#FFFF00FF>Vigilantes</color> and <color=#073763FF>Assassins</color>.\n- Framed players will appear to have" +
                " the wrong results to investigative roles till the Framer is dead.";
            RoleType = RoleEnum.Framer;
            Faction = Faction.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = "Syndicate (Disruption)";
            Results = InspResults.ArsoCryoPBOpTroll;
            Color = CustomGameOptions.CustomSynColors ? Colors.Framer : Colors.Syndicate;
            RoleDescription = "You are a Framer! This means that you are unrivalled in the art of gaslighting. Framed players always appear to be evil, regardless of their role!";
            FactionDescription = SyndicateFactionDescription;
            AlignmentDescription = SDDescription;
            Objectives = IsRecruit ? JackalWinCon : SyndicateWinCon;
            AddToRoleHistory(RoleType);
        }

        public KillButton FrameButton
        {
            get => _frameButton;
            set
            {
                _frameButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

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

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else
                SyndicateWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
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
            else if (Utils.SyndicateWins())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyndicateWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public float FrameTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFramed;
            var num = CustomGameOptions.FrameCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
