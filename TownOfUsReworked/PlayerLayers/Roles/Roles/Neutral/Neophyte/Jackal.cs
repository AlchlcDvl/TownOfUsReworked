using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using System;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Jackal : Role
    {
        public PlayerControl EvilRecruit = null;
        public PlayerControl GoodRecruit = null;
        public PlayerControl BackupRecruit = null;
        public PlayerControl ClosestPlayer;
        private KillButton _recruitButton;
        public bool HasRecruited = false;
        public bool RecruitsDead => (EvilRecruit.Data.IsDead || EvilRecruit.Data.Disconnected) && (GoodRecruit.Data.Disconnected || GoodRecruit.Data.IsDead);
        public DateTime LastRecruited { get; set; }

        public Jackal(PlayerControl player) : base(player)
        {
            Name = "Jackal";
            Faction = Faction.Neutral;
            RoleType = RoleEnum.Jackal;
            StartText = "Gain A Majority";
            AbilitiesText = "- You can recruit one player into joining your organisation.";
            AttributesText = "- You start off with 2 recruits. 1 of them is either <color=#8BFDFDFF>Crew</color> or <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color>" + 
                "\nand the other is a <color=#008000FF>Syndicate</color>, <color=#FF0000FF>Intruder</color> or a <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killer</color>.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jackal : Colors.Neutral;
            SubFaction = SubFaction.Cabal;
            SubFactionColor = Colors.Cabal;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = "Neutral (Neophyte)";
            Results = InspResults.GAExeJackAg;
            Objectives = JackalWinCon;
            AlignmentDescription = NNDescription;
            FactionDescription = NeutralFactionDescription;
            RoleDescription = "You are a Jackal! You are a greedy double agent sent from a rival company! Use your recruits to your advantage and take over the mission!";
            SubFactionName = "Cabal";
        }

        public override void Wins()
        {
            CabalWin = true;
        }
        
        public float RecruitTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastRecruited;
            var num = CustomGameOptions.RecruitCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

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
            
            return false;
        }

        public KillButton RecruitButton
        {
            get => _recruitButton;
            set
            {
                _recruitButton = value;
                AddToExtraButtons(value);
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var jackTeam = new List<PlayerControl>();
            jackTeam.Add(PlayerControl.LocalPlayer);
            jackTeam.Add(GoodRecruit);
            jackTeam.Add(EvilRecruit);
            __instance.teamToShow = jackTeam;
        }
    }
}