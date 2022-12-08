using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using System.Linq;
using Hazel;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Dampyr : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }

        public Dampyr(PlayerControl player) : base(player)
        {
            Name = "Dampyr";
            StartText = "Kill Off The <color=#8BFDFDFF>Crew</color> To Gain A Majority";
            AbilitiesText = "- You can bite players to kill them.";
            AttributesText = "- You share a cooldown with the <color=#AC8A00FF>Dracula</color>, meaning if the <color=#AC8A00FF>Dracula</color> " +
                "converts, your kill cooldown resets.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Dampyr : Colors.Neutral;
            SubFaction = SubFaction.Undead;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Dampyr;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralPros;
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            IntroSound = null;
            AlignmentName = "Neutral (Proselyte)";
            FactionDescription = "Your faction is Neutral! You do not have any team mates and can only by yourself or by other players after finishing" +
                " a certain objective.";
            AlignmentDescription = "";
            IntroText = "Kill off the <color=#8BFDFDFF>Crew</color> and help the <color=#AC8A00FF>Dracula</color>";
            Results = InspResults.MineMafiSideDamp;
            Objectives = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#8BFDFDFF>Crew</color>, <color=#008000FF>Syndicate</color>" + 
                " and Non-<color=#7B8968FF>Vampire</color> <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, " +
                "<color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.";
            RoleDescription = "You have become a Dampyr! Your new goal is the help the Dracula convert everyone. Kill anyone who does not submit to the Dracula" +
                " and avoid losing the Dracula or else you're done for!";
            AddToRoleHistory(RoleType);
        }

        public override void Wins()
        {
            VampWin = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Data.IsImpostor() |
                x.Is(RoleAlignment.NeutralKill) | (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Dracula)) | x.Is(Faction.Syndicate) |
                (x.Is(RoleAlignment.NeutralPros) && !(x.Is(RoleEnum.Dampyr) | x.Is(RoleEnum.Vampire))))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UndeadWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            return false;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.MurdKCD * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}