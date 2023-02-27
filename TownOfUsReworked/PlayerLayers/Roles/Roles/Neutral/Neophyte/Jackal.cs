using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Jackal : Role
    {
        public PlayerControl EvilRecruit = null;
        public PlayerControl GoodRecruit = null;
        public PlayerControl BackupRecruit = null;
        public PlayerControl ClosestPlayer;
        private KillButton _recruitButton;
        public bool HasRecruited = false;
        public bool RecruitsDead => (EvilRecruit == null && GoodRecruit == null) || ((EvilRecruit != null && EvilRecruit.Data.IsDead || EvilRecruit.Data.Disconnected) &&
            (GoodRecruit != null && GoodRecruit.Data.Disconnected || GoodRecruit.Data.IsDead));
        public DateTime LastRecruited { get; set; }
        public List<byte> Recruited;

        public Jackal(PlayerControl player) : base(player)
        {
            Name = "Jackal";
            Faction = Faction.Neutral;
            RoleType = RoleEnum.Jackal;
            StartText = "Gain A Majority";
            AbilitiesText = "- You can recruit one player into joining your organisation.\n- You start off with 2 recruits. 1 of them is always <color=#8BFDFDFF>Crew</color>" + 
                "\nand the other is either a <color=#008000FF>Syndicate</color>, <color=#FF0000FF>Intruder</color> or a <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killer</color>.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jackal : Colors.Neutral;
            SubFaction = SubFaction.Cabal;
            SubFactionColor = Colors.Cabal;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = NN;
            RoleDescription = "You are a Jackal! You are a greedy double agent sent from a rival company! Use your recruits to your advantage and take over the mission!";
            SubFactionName = "Cabal";
            Recruited = new List<byte>();
            Recruited.Add(Player.PlayerId);
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
                CabalWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CabalWin);
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
                AddToAbilityButtons(value, this);
            }
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var jackTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            jackTeam.Add(PlayerControl.LocalPlayer);
            jackTeam.Add(GoodRecruit);
            jackTeam.Add(EvilRecruit);
            __instance.teamToShow = jackTeam;
        }
    }
}