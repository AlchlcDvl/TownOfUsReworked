using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Consigliere : IntruderRole
    {
        public List<byte> Investigated;
        public AbilityButton InvestigateButton;
        public PlayerControl ClosestTarget;
        public DateTime LastInvestigated;
        public string role = CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "role" : "faction";
        public Ability ability => PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) ? Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer)
            : null;
        public string CanAssassinate => ability != null && CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "\n- You cannot assassinate players you have revealed." : "";

        public Consigliere(PlayerControl player) : base(player)
        {
            Name = "Consigliere";
            StartText = "See Players For Who They Really Are";
            AbilitiesText = $"- You can reveal a player's {role}.{CanAssassinate}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Consigliere : Colors.Intruder;
            RoleType = RoleEnum.Consigliere;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
            RoleDescription = "You are a Consigliere! You are a corrupt Inspector who is so capable of finding someone's identity. Help your mate assassinate or prioritise others" +
                " by revealing players for who they really are!";
            Investigated = new List<byte>();
        }

        public float ConsigliereTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ConsigCd, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}