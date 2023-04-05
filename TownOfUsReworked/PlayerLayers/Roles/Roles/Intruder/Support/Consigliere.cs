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
        private string CanAssassinate => Ability.GetAbility(Player) != null && Player.Is(AbilityEnum.Assassin) && CustomGameOptions.ConsigInfo == ConsigInfo.Role ?
            "\n- You cannot assassinate players you have revealed" : "";

        public Consigliere(PlayerControl player) : base(player)
        {
            Name = "Consigliere";
            StartText = "See Players For Who They Really Are";
            AbilitiesText = $"- You can reveal a player's {role}{CanAssassinate}\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Consigliere : Colors.Intruder;
            RoleType = RoleEnum.Consigliere;
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