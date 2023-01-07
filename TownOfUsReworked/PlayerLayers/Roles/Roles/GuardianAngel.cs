using System;
using UnityEngine;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class GuardianAngel : Role
    {
        public bool Enabled;
        public DateTime LastProtected;
        public float TimeRemaining;
        public int UsesLeft;
        public TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;
        public PlayerControl TargetPlayer = null;
        public bool TargetAlive => (!TargetPlayer.Data.IsDead && !TargetPlayer.Data.Disconnected && TargetPlayer != null && !Player.Data.Disconnected) ||
            TargetPlayer == null || TargetPlayer.Data.Disconnected;
        public bool Protecting => TimeRemaining > 0f;

        public GuardianAngel(PlayerControl player) : base(player)
        {
            Name = "Guardian Angel";
            StartText = "Protect Your Target With Your Life";
            Objectives = "- Have your target live to the end of the game.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.GuardianAngel : Colors.Neutral;
            LastProtected = DateTime.UtcNow;
            RoleType = RoleEnum.GuardianAngel;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            UsesLeft = CustomGameOptions.MaxProtects;
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = "Protect Your Target With Your Life";
            Results = InspResults.GAExeMedicPup;
            AbilitiesText = "- You can protect your target from death for a short while.";
            RoleDescription = "You are a Guardian Angel! You are an overprotective being from the heavens whose only job is to see your chosen live. Keep your target alive at all costs" +
                " even if they lose!";
            AlignmentDescription = NBDescription;
            FactionDescription = NeutralFactionDescription;
            AddToRoleHistory(RoleType);
        }

        public float ProtectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastProtected;
            var num = CustomGameOptions.ProtectCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Protect()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void UnProtect()
        {
            Enabled = false;
            LastProtected = DateTime.UtcNow;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var gaTeam = new List<PlayerControl>();
            gaTeam.Add(PlayerControl.LocalPlayer);
            gaTeam.Add(TargetPlayer);
            __instance.teamToShow = gaTeam;
        }
    }
}