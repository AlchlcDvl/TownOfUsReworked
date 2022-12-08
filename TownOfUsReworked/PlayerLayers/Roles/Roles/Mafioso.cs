using System.Linq;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Mafioso : Role
    {
        public Role FormerRole;
        public bool CanPromote => PlayerControl.AllPlayerControls.ToArray().ToList().Where(x => x.Is(RoleEnum.Godfather)).Count() == 0;

        public Mafioso(PlayerControl player) : base(player)
        {
            Name = "Mafioso";
            Faction = Faction.Intruders;
            RoleType = RoleEnum.Godfather;
            StartText = "Succeed The <color=#404C08FF>Godfather</color>";
            AbilitiesText = "- When the <color=#404C08FF>Godfather</color> dies, you will become the new <color=#404C08FF>Godfather</color> with boosted abilities of your former role.";
            AttributesText = "- None.";
            Color = CustomGameOptions.CustomImpColors ? Colors.Mafioso : Colors.Intruder;
            SubFaction = SubFaction.None;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderUtil;
            AlignmentName = "Intruder (Utility)";
            IntroText = "Kill those who oppose you";
            Results = InspResults.MineMafiSideDamp;
            IntroSound = null;
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            FactionDescription = "You are an Intruder! Your main task is to kill anyone who dares to oppose you. Sabotage the systems, murder the crew, do anything" +
                " to ensure your victory over others.";
            Objectives = "- Kill: <color=#008000FF>Syndicate</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.\n   or\n- Have a critical sabotage reach 0 seconds.";
            AlignmentDescription = "You are a Intruder (Utility) role! You usually have no special ability and cannot even appear under natural conditions.";
            RoleDescription = "You have become a Mafioso! You are the successor to the leader of the Intruders. When the Godfather dies, you will become the new" +
                " Godfather and will inherit stronger variations of your former role.";
            AddToRoleHistory(RoleType);
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

        public void TurnGodfather()
        {
            var formerRole = Role.GetRole<Mafioso>(Player).FormerRole;
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Godfather(Player);
            role.WasMafioso = true;
            role.HasDeclared = true;
            role.FormerRole = formerRole;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer)
                    role.RegenTask();
            }
        }
    }
}