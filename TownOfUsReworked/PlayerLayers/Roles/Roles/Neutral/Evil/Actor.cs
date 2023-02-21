using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Actor : Role
    {
        public bool Guessed;
        public bool ActorWins;
        public bool HasPretendTarget;
        public PlayerControl PretendTarget;
        public List<Role> PretendRoles;

        public Actor(PlayerControl player) : base(player)
        {
            Name = "Actor";
            StartText = "It Was Jest A Prank Bro";
            Objectives = $"- Get guessed as one of your target roles.\n- Your target roles are {PretendRoles[0].Name}, {PretendRoles[1].Name}, {PretendRoles[2].Name}," +
                $" {PretendRoles[3].Name} and {PretendRoles[4].Name}.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Actor : Colors.Neutral;
            RoleType = RoleEnum.Actor;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            RoleDescription = "You are an Actor! You are a crazed performer who wants to die! Get guessed as one of the roles you are pretending to be!";
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsIntAlly)
                IntruderWin = true;
            else if (IsSynAlly)
                SyndicateWin = true;
            else if (IsCrewAlly)
                CrewWin = true;
            else if (IsPersuaded)
                SectWin = true;
            else if (IsBitten)
                UndeadWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
            else if (CustomGameOptions.NoSolo == NoSolo.AllNeutrals)
                AllNeutralsWin = true;
            else
                ActorWins = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (!Guessed || !Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Guessed)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.ActorWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }   
            
            return true;
        }
    }
}