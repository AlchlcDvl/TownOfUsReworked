using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Lobby.CustomOption;
using System;
using TownOfUsReworked.Extensions;
using UnityEngine;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Escort : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastBlock { get; set; }
        public float TimeRemaining;

        public Escort(PlayerControl player) : base(player)
        {
            Name = "Escort";
            Faction = Faction.Crew;
            RoleType = RoleEnum.Escort;
            StartText = "Roleblock Players And Stop Them From Harming Others";
            AbilitiesText = "- You can seduce players.";
            AttributesText = "- Seduction blocks your target from being able to use their abilities for a short while.\n- You are immune to blocks.\n" +
                "- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Escort : Colors.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = "Crew (Support)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.EscConsGliPois;
            SubFaction = SubFaction.None;
            AlignmentDescription = "You are a Crew (Support) role! You have a miscellaneous ability that cannot be classified on it's own. Use your" +
                " abilities to their fullest extent to bring about a Crew victory.";
            FactionDescription = "Your faction is the Crew! You do not know who the other members of your faction are. It is your job to deduce" + 
                " who is evil and who is not. Eject or kill all evils or finish all of your tasks to win!";
            Objectives = "- Finish your tasks along with other Crew.\n   or\n- Kill: <color=#FF0000FF>Intruders</color>, <color=#008000FF>Syndicate</color>" + 
                " and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and " +
                "<color=#1D7CF2FF>Neophytes</color>.";
            RoleDescription = "You are an Escort! You can have a little bit of \"fun time\" with players to ensure they are unable to kill" +
                " anyone.";
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            IntroSound = null;
            Base = false;
            IsRecruit = false;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
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
            CrewWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (Utils.CrewWins())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CrewWin,
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