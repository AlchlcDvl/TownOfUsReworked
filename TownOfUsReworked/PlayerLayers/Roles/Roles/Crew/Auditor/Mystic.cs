using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using System.Linq;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mystic : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastRevealed { get; set; }
        public bool ConvertedDead => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && !x.Data.Disconnected && !x.Is(SubFaction.None)) == 0;
        private KillButton _revealButton;

        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            RoleType = RoleEnum.Mystic;
            Color = CustomGameOptions.CustomCrewColors ? Colors.Mystic : Colors.Crew;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewAudit;
            AlignmentName = CA;
            StartText = "You Know When Converts Happen";
            AbilitiesText = "- You can investigate players to see if they have been converted.\n- Whenever someone has been converted, you will be alerted to it.\n- When all converted" +
                " and converters die, you will become a <color=#71368AFF>Seer.</color>";
            Objectives = CrewWinCon;
            RoleDescription = "You are a Mystic! You know when someone's allegiance changes and to who! Get rid of those who have defected by finding and eliminating them!";
            InspectorResults = InspectorResults.TracksOthers;
        }

        public KillButton RevealButton
        {
            get => _revealButton;
            set
            {
                _revealButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float RevealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastRevealed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.RevealCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TurnSeer()
        {
            var mystic = Role.GetRole<Mystic>(Player);
            var role = new Seer(Player);
            role.RoleHistory.Add(mystic);
            role.RoleHistory.AddRange(mystic.RoleHistory);
            Player.RegenTask();

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Seer));
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;

            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.EvilRecruit);
            }

            __instance.teamToShow = team;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit && Utils.CabalWin())
            {
                CabalWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CabalWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsIntTraitor || IsIntFanatic) && Utils.IntrudersWin())
            {
                IntruderWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsSynTraitor || IsSynFanatic) && Utils.SyndicateWins())
            {
                SyndicateWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsPersuaded && Utils.SectWin())
            {
                SectWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SectWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsBitten && Utils.UndeadWin())
            {
                UndeadWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsResurrected && Utils.ReanimatedWin())
            {
                ReanimatedWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.CrewWins() && NotDefective)
            {
                CrewWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CrewWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}