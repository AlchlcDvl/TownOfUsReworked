using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using System;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Troll : Role
    {
        public bool Killed;
        public DateTime LastInteracted { get; set; }
        private KillButton _interactbutton;
        public PlayerControl ClosestPlayer;
        public bool TrollWins { get; set; }

        public Troll(PlayerControl player) : base(player)
        {
            Name = "Troll";
            StartText = "Troll Everyone With Your Death";
            AbilitiesText = "- You can interact with players.\n- Your interactions do nothing except spread infection and possibly kill you via touch sensitive roles.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Troll : Colors.Neutral;
            RoleType = RoleEnum.Troll;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (!Killed || !Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Killed)
            {
                TrollWins = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.TrollWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
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

        public float InteractTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInteracted;
            var num = CustomGameOptions.InteractCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton InteractButton
        {
            get => _interactbutton;
            set
            {
                _interactbutton = value;
                AddToAbilityButtons(value, this);
            }
        }
    }
}