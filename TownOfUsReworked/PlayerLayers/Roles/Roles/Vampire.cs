using System.Linq;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Vampire : Role
    {
        public Vampire(PlayerControl player) : base(player)
        {
            Name = "Vampire";
            Faction = Faction.Neutral;
            RoleType = RoleEnum.Vampire;
            StartText = "Bite The <color=#8BFDFDFF>Crew</color> To Gain A Majority";
            AbilitiesText = "Convert the <color=#8BFDFDFF>Crew</color>!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Vampire : Colors.Neutral;
            SubFaction = SubFaction.Undead;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralPros;
            AlignmentName = "Neutral (Proselyte)";
            IntroText = "Gain a majority";
            Results = InspResults.SurvVHVampVig;
            IntroSound = null;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            Base = false;
            IsRecruit = false;
            AddToRoleHistory(RoleType);
        }

        public override void Wins()
        {
            VampWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
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
    }
}