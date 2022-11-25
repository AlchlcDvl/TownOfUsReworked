using System;
using UnityEngine;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

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
        public PlayerControl TargetPlayer;
        public bool GAWins { get; set; }
        public bool Protecting => TimeRemaining > 0f;

        public GuardianAngel(PlayerControl player) : base(player)
        {
            Name = "Guardian Angel";
            ImpostorText = () => TargetPlayer == null 
                    ? "You don't have a target for some reason... weird..."
                    : $"Protect {TargetPlayer.name} With Your Life!";
            TaskText = () => TargetPlayer == null
                    ? "You don't have a target for some reason... weird..."
                    : $"Protect {TargetPlayer.name}!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.GuardianAngel : Colors.Neutral;
            SubFaction = SubFaction.None;
            LastProtected = DateTime.UtcNow;
            RoleType = RoleEnum.GuardianAngel;
            Faction = Faction.Neutral;
            Scale = 1.4f;
            FactionName = "Neutral";
            UsesLeft = CustomGameOptions.MaxProtects;
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = () => "Neutral (Benign)";
            IntroText = $"Protect {TargetPlayer.name}";
            CoronerDeadReport = "The body's anatomy is out of this world. They must be a Guardian Angel!";
            CoronerKillerReport = "";
            Results = InspResults.GAExeMedicPup;
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

        public override void Wins()
        {
            if (!TargetPlayer.Data.IsDead)
                GAWins = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var gaTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            gaTeam.Add(PlayerControl.LocalPlayer);
            gaTeam.Add(TargetPlayer);
            __instance.teamToShow = gaTeam;
        }
    }
}