using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using Hazel;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Dampyr : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        private KillButton _killButton;

        public Dampyr(PlayerControl player) : base(player)
        {
            Name = "Dampyr";
            StartText = "Kill Off The <color=#8BFDFDFF>Crew</color> To Gain A Majority";
            AbilitiesText = "- You can bite players to kill them.";
            AttributesText = "- You share a cooldown with the <color=#AC8A00FF>Dracula</color>, meaning if the <color=#AC8A00FF>Dracula</color> converts, your kill cooldown resets.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Dampyr : Colors.Neutral;
            SubFaction = SubFaction.Undead;
            RoleType = RoleEnum.Dampyr;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralPros;
            AlignmentName = "Neutral (Proselyte)";
            FactionDescription = NeutralFactionDescription;
            AlignmentDescription = NPDescription;
            Results = InspResults.JestMafiSideDamp;
            Objectives = UndeadWinCon;
            RoleDescription = "You have become a Dampyr! Your new goal is the help the Dracula convert everyone. Kill anyone who does not submit to the Dracula" +
                " and avoid losing the Dracula or else you're done for!";
            SubFactionColor = Colors.Undead;
            SubFactionName = "Undead";
        }

        public override void Wins()
        {
            UndeadWin = true;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool GameEnd(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.UndeadWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            return false;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.DampBiteCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}