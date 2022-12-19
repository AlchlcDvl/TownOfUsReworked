using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Executioner : Role
    {
        public PlayerControl TargetPlayer;
        public bool TargetVotedOut;

        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            StartText = TargetPlayer == null
                ? "You don't have a target for some reason... weird..."
                : $"Eject {TargetPlayer.name}";
            Objectives = TargetPlayer == null
                ? "- You don't have a target for some reason... weird..."
                : $"- Eject {TargetPlayer.name}!\nFake Tasks:";
            AbilitiesText = "- None.";
            AttributesText = "- None.";
            Base = false;
            IsRecruit = false;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Executioner : Colors.Neutral;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Executioner;
            Faction = Faction.Neutral;
            Scale = 1.4f;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            IntroText = $"Eject {TargetPlayer.name}";
            CoronerDeadReport = "This body has tons of incriminating pictures of someone. They must be an Executioner!";
            CoronerKillerReport = "";
            Results = InspResults.GAExeMedicPup;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            IntroSound = null;
            FactionDescription = "Your faction is Neutral! You do not have any team mates and can only by yourself or by other players after finishing" +
                " a certain objective.";
            AlignmentDescription = "You are a Neutral (Evil) role! You have a confliction win condition over others and upon achieving it will end the game. " +
                "Finish your objective before they finish you!";
            RoleDescription = $"You are an Executioner! You are a crazed stalker who only wants to see your target get ejected. Eject {TargetPlayer.name} " +
                "at all costs!";
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var exeTeam = new List<PlayerControl>();
            exeTeam.Add(PlayerControl.LocalPlayer);
            exeTeam.Add(TargetPlayer);
            __instance.teamToShow = exeTeam;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead)
                return true;

            if (!TargetVotedOut | !TargetPlayer.Data.IsDead | TargetPlayer == null)
                return true;

            Utils.EndGame();
            return false;
        }

        public override void Wins()
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return;
                
            TargetVotedOut = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }
    }
}