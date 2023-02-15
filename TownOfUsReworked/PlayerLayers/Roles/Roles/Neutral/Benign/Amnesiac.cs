using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Amnesiac : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public DeadBody CurrentTarget = null;
        private KillButton _rememberButton;

        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            StartText = "You Forgor :Skull:";
            AbilitiesText = "- You can copy over a player's role should you find their body.";
            RoleType = RoleEnum.Amnesiac;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = "Neutral (Benign)";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Amnesiac : Colors.Neutral;
            //IntroSound = TownOfUsReworked.AmnesiacIntro;
            RoleDescription = "Your are an Amnesiac! You know when players die and need to find a dead player. You cannot win as your current role and" +
                " instead need to win as the role you become after finding a dead body.";
            Objectives = "- Find a dead body, remember their role and then fulfill the win condition for that role.";
        }

        public KillButton RememberButton
        {
            get => _rememberButton;
            set
            {
                _rememberButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var amnesiacTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            amnesiacTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = amnesiacTeam;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
                
            BodyArrows.Remove(arrow.Key);
        }
        
        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }

                return false;
            }

            return true;
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
            else if (IsResurrected)
                ReanimatedWin = true;
            else if (CustomGameOptions.NoSolo == NoSolo.AllNeutrals)
                AllNeutralsWin = true;
        }
    }
}