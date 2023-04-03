using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Consigliere : IntruderRole
    {
        public List<byte> Investigated = new();
        public AbilityButton InvestigateButton;
        public PlayerControl ClosestTarget;
        public DateTime LastInvestigated;
        public string role = CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "role" : "faction";
        public static Ability Ability => PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) ? Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer) : null;
        public static string CanAssassinate => Ability != null && CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "\n- You cannot assassinate players you have revealed." : "";

        public Consigliere(PlayerControl player) : base(player)
        {
            Name = "Consigliere";
            StartText = "See Players For Who They Really Are";
            AbilitiesText = $"- You can reveal a player's {role}.{CanAssassinate}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Consigliere : Colors.Intruder;
            Type = RoleEnum.Consigliere;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
            Investigated = new();
        }

        public float ConsigliereTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInvestigated;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ConsigCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}