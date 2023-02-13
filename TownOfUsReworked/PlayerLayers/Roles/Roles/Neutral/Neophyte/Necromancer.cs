using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using System.Collections.Generic;
using TownOfUsReworked.Classes;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Necromancer : Role
    {
        public bool CurrentlyReviving = false;
        public DeadBody CurrentTarget = null;
        public PlayerControl ClosestPlayer;
        private KillButton _resurrectButton;
        public List<byte> Resurrected = new List<byte>();
        
        public Necromancer(PlayerControl player) : base(player)
        {
            Name = "Necromancer";
            StartText = "Resurrect The Dead Into Doing Your Bidding";
            AbilitiesText = "- You can revive a dead body at the cost of your own life.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Necromancer : Colors.Neutral;
            RoleType = RoleEnum.Necromancer;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = "Neutral (Neophyte)";
            RoleDescription = "Your are a Necromancer! You can revive a dead person if you find their body. Be careful though, because it takes time" +
                " to revive someone and a meeting being called will kill both you and your target.";
            Objectives = CrewWinCon;
            SubFaction = SubFaction.Reanimated;
            SubFactionName = "Reanimated";
            SubFactionColor = Colors.Reanimated;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(SubFaction) && player != PlayerControl.LocalPlayer)
                    team.Add(player);
            }

            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            ReanimatedWin = true;
        }

        public KillButton ReviveButton
        {
            get => _resurrectButton;
            set
            {
                _resurrectButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.ReanimatedWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}