using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;

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
        }

        public override void Wins()
        {
            UndeadWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.UndeadWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            return false;
        }
    }
}