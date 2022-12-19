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
    public class Arsonist : Role
    {
        private KillButton _igniteButton;
        private KillButton _douseButton;
        public bool ArsonistWins;
        public bool LastKiller;
        public PlayerControl ClosestPlayerDouse;
        public PlayerControl ClosestPlayerIgnite;
        public List<byte> DousedPlayers = new List<byte>();
        public DateTime LastDoused;
        public DateTime LastIgnited;
        public int DousedAlive => DousedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null &&
            !Utils.PlayerById(x).Data.IsDead);

        public Arsonist(PlayerControl player) : base(player)
        {
            Name = "Arsonist";
            StartText = "Gasoline + Bean + Lighter = Bonfire";
            AbilitiesText = "- You can douse players in gasoline.\n- Doused players can then be ignite to kill all doused players at once.";
            AttributesText = "- People who interact with you will also get doused.";
            LastDoused = DateTime.UtcNow;
            RoleType = RoleEnum.Arsonist;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            IntroText = "Ignite those who oppose you";
            CoronerDeadReport = "There are burn marks and a smell of gasoline. They must be an Arsonist!";
            CoronerKillerReport = "The body has been completely charred. They were torched by an Arsonist!";
            Results = InspResults.ArsoCryoPBOpTroll;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Arsonist : Colors.Neutral;
            SubFaction = SubFaction.None;
            IntroSound = null;
            Attack = AttackEnum.Unstoppable;
            AttackString = "Unstoppable";
            Base = false;
            IsRecruit = false;
            DefenseString = "Basic";
            Defense = DefenseEnum.Basic;
            RoleDescription = "You are an Arsonist! This means that you do not kill directly and instead, bide your time by dousing other players" +
                " and igniting them later for mass murder. Be careful though, as you need be next to someone to ignite and if anyone sees you ignite," +
                $" you are done for. There are currently {DousedAlive} players doused.";
            FactionDescription = "Your faction is Neutral! You do not have any team mates and can only by yourself or by other players after finishing" +
                " a certain objective.";
            AlignmentDescription = "You are a Neutral (Killing) role! You side with no one and can only win by yourself. You have a special way to kill " +
                "and gain a large body count. Make sure no one survives.";
            Objectives = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#8BFDFD>Crew</color>, <color=#008000FF>Syndicate</color> and other " +
                "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>";
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

        public KillButton DouseButton
        {
            get => _douseButton;
            set
            {
                _douseButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruders) |
                (x.Is(RoleAlignment.NeutralKill) && !x.Is(RoleEnum.Arsonist)) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros) |
                x.Is(Faction.Crew) | x.Is(Faction.Syndicate))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ArsonistWin,
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
            ArsonistWins = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
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

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float IgniteTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastIgnited;
            var num = CustomGameOptions.IgniteCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ignite()
        {
            System.Console.WriteLine("Ignite 1");

            foreach (var arso in Role.GetRoles(RoleEnum.Arsonist))
            {
                var arso2 = (Arsonist)arso;

                foreach (var playerId in arso2.DousedPlayers)
                {
                    var player = Utils.PlayerById(playerId);

                    if (player == null | player.Data.Disconnected | player.Data.IsDead | player.Is(RoleEnum.Pestilence))
                        continue;

                    Utils.MurderPlayer(Player, player);
                }

                arso2.DousedPlayers.Clear();
            }
            
            System.Console.WriteLine("Ignite 2");
        }
    }
}
