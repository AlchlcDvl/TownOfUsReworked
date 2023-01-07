using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using System;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Troll : Role
    {
        public bool Killed;
        public DateTime LastInteracted { get; set; }
        public PlayerControl ClosestPlayer;

        public Troll(PlayerControl player) : base(player)
        {
            Name = "Troll";
            StartText = "Troll Everyone With Your Death";
            AbilitiesText = "- You can interact with players.";
            AttributesText = "- Your interactions do nothing except spread infection and possibly kill you via touch sensitive roles.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Troll : Colors.Neutral;
            RoleType = RoleEnum.Troll;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            IntroText = "Die";
            Results = InspResults.ArsoCryoPBOpTroll;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (!Killed || !Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Killed)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TrollWin, SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }   
            
            return false;
        }

        public override void Wins()
        {
            Killed = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
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
    }
}