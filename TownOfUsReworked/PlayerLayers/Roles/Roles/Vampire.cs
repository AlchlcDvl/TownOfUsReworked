using System.Collections.Generic;
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
        public bool AlreadyConverted;
        public int VampCounter;
        public bool VampWins;
        public PlayerControl ClosestPlayerBite;
        public List<PlayerControl> AllVamps = new List<PlayerControl>();

        public Vampire(PlayerControl player) : base(player)
        {
            Name = "Vampire";
            Faction = Faction.Neutral;
            RoleType = RoleEnum.Vampire;
            ImpostorText = () => "Bite The <color=#8BFDFDFF>Crew</color> To Gain A Majority";
            TaskText = () => "Convert the <color=#8BFDFDFF>Crew</color>!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Vampire : Colors.Neutral;
            SubFaction = SubFaction.Vampires;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralPower;
            AlignmentName = () => "Neutral (Power)";
            IntroText = "Gain a majority";
            Results = InspResults.SurvVHVampVig;
            AddToRoleHistory(RoleType);
        }

        public void Wins()
        {
            VampWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                (x.Data.IsImpostor() | x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Crew))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampWin,
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