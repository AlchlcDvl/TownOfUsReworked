using Il2CppSystem.Collections.Generic;
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
            Results = InspResults.SurvVHVampVig;
            AddToRoleHistory(RoleType);
        }

        public override void Wins()
        {
            UndeadWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.UndeadWin())
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