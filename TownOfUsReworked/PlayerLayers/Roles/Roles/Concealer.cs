using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Concealer : Role
    {
        private KillButton _concealButton;
        public bool Enabled;
        public DateTime LastConcealed { get; set; }
        public float TimeRemaining;
        public bool Concealed => TimeRemaining > 0f;

        public Concealer(PlayerControl player) : base(player)
        {
            Name = "Concealer";
            StartText = "Make The <color=#8BFDFDFF>Crew</color> Invisible For Some Chaos";
            AbilitiesText = "- You can turn off player's ability to see things properly, making all other players appear invisible to themselves.";
            AttributesText = "- None.";
            Color = CustomGameOptions.CustomSynColors ? Colors.Concealer : Colors.Syndicate;
            RoleType = RoleEnum.Concealer;
            Faction = Faction.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            AlignmentName = "Syndicate (Support)";
            IntroText = "Cause chaos and kill your opposition";
            CoronerDeadReport = "The ghillie suit on this body makes it look like they are nearly invisible! They must be a Concealer";
            CoronerKillerReport = "There seems to be a strange material on this body. It looks like it's used in ghilli suits. They were killed by a Concealer!";
            Results = InspResults.ConcealGorg;
            SubFaction = SubFaction.None;
            IntroSound = null;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            RoleDescription = "You are a Concealer! You can turn everyone invisible to everyone else but themselves by making them unable to see things properly. " +
                "Use this to get away from crime scenes as fast as possible!";
            AlignmentDescription = "You are a Syndicate (Support) role! It is your job to ensure no one bats an eye to the things you or your mates do. Support them in " +
                "everyway possible.";
            FactionDescription = "Your faction is the Syndicate! Your faction has low killing power and is instead geared towards delaying the wins of other factions" +
                " and causing some good old chaos. After a certain number of meeting, one of you will recieve the \"Chaos Drive\" which will enhance your powers and " +
                "give you the ability to kill, if you didn't already.";          
            Objectives = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.";
            AddToRoleHistory(RoleType);
        }

        public KillButton ConcealButton
        {
            get => _concealButton;
            set
            {
                _concealButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Conceal()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Conceal();
        }

        public void UnConceal()
        {
            Enabled = false;
            LastConcealed = DateTime.UtcNow;
            Utils.DefaultOutfitAll();
        }

        public float ConcealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastConcealed;
            var num = CustomGameOptions.CamouflagerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var synTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Syndicate))
                    synTeam.Add(player);
            }
            __instance.teamToShow = synTeam;
        }

        public override void Wins()
        {
            SyndicateWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Crew) |
                x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Intruders) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0) | 
                (CustomGameOptions.AltImps && Utils.Sabotaged()))
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
    }
}