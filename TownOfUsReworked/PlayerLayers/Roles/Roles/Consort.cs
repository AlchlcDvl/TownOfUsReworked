using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Lobby.CustomOption;
using System;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Consort : Role
    {
        public bool ConsWin;
        public PlayerControl ClosestPlayer;
        public DateTime LastBlock { get; set; }
        public float TimeRemaining;
        private KillButton _roleblockButton;

        public Consort(PlayerControl player) : base(player)
        {
            Name = "Consort";
            Faction = Faction.Intruders;
            RoleType = RoleEnum.Consort;
            StartText = "Roleblock The Crew And Stop Them From Progressing";
            AbilitiesText = "- You can seduce players.";
            AttributesText = "- Seduction blocks your target from being able to use their abilities for a short while.\n- You are immune to blocks.\n" +
                "- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you.";
            Color = CustomGameOptions.CustomImpColors ? Colors.Consort : Colors.Crew;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            IntroText = "Kill those who oppose you";
            Results = InspResults.EscConsGliPois;
            FactionDescription = "You are an Intruder! Your main task is to kill anyone who dares to oppose you. Sabotage the systems, murder the crew, do anything" +
                " to ensure your victory over others.";
            AlignmentDescription = "You are a Intruder (Support) role! It is your job to ensure no one bats an eye to the things you or your mates do. Support them in " +
                "everyway possible.";
            Objectives = "- Kill: <color=#008000FF>Syndicate</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.\n   or\n- Have a critical sabotage reach 0 seconds.";
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            IntroSound = null;
            RoleDescription = "You are a Consort! You can have a little bit of \"fun time\" with players to ensure they are unable to stop you from killing" +
                " everyone.";
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var intTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }

        public KillButton RoleblockButton
        {
            get => _roleblockButton;
            set
            {
                _roleblockButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlock;
            var num = CustomGameOptions.MimicCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Roleblock()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Block(Player, ClosestPlayer);

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void Unroleblock()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Block(Player, ClosestPlayer);

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public override void Wins()
        {
            IntruderWin = true;
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
                x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0) |
                Utils.Sabotaged())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.IntruderWin,
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