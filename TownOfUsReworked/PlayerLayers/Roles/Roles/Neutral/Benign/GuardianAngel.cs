using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class GuardianAngel : NeutralRole
    {
        public bool Enabled;
        public DateTime LastProtected;
        public float TimeRemaining;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public PlayerControl TargetPlayer;
        public bool TargetAlive => TargetPlayer?.Data.IsDead == false && TargetPlayer?.Data.Disconnected == false && !Player.Data.Disconnected;
        public bool Protecting => TimeRemaining > 0f;
        public AbilityButton ProtectButton;

        public GuardianAngel(PlayerControl player) : base(player)
        {
            Name = "Guardian Angel";
            StartText = "Protect Your Target With Your Life";
            Objectives = "- Have your target live to the end of the game.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.GuardianAngel : Colors.Neutral;
            LastProtected = DateTime.UtcNow;
            RoleType = RoleEnum.GuardianAngel;
            UsesLeft = CustomGameOptions.MaxProtects;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
            AbilitiesText = "- You can protect your target from death for a short while.";
            InspectorResults = InspectorResults.SeeksToProtect;
        }

        public float ProtectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastProtected;
            var num = CustomGameOptions.ProtectCd * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Protect()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void TurnSurv()
        {
            var newRole = new Survivor(Player)
            {
                UsesLeft = UsesLeft
            };

            newRole.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Survivor, "Your target died so you have become a <color=#DDDD00FF>Survivor</color>!");

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "SOmeone has changed their identity!");
        }

        public void UnProtect()
        {
            Enabled = false;
            LastProtected = DateTime.UtcNow;
        }
    }
}