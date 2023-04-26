using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Custom;
using System.Linq;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Consigliere : IntruderRole
    {
        public List<byte> Investigated = new();
        public CustomButton InvestigateButton;
        public DateTime LastInvestigated;
        private readonly string role = CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "role" : "faction";
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
            Type = LayerEnum.Consigliere;
            InvestigateButton = new(this, "Investigate", AbilityTypes.Direct, "Secondary", Investigate);
        }

        public float ConsigliereTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInvestigated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ConsigCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Investigate()
        {
            if (ConsigliereTimer() != 0f || Utils.IsTooFar(Player, InvestigateButton.TargetPlayer) || Investigated.Contains(InvestigateButton.TargetPlayer.PlayerId))
                return;

            var interact = Utils.Interact(Player, InvestigateButton.TargetPlayer);

            if (interact[3])
                Investigated.Add(InvestigateButton.TargetPlayer.PlayerId);

            if (interact[0])
                LastInvestigated = DateTime.UtcNow;
            else if (interact[1])
                LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notInvestigated = PlayerControl.AllPlayerControls.ToArray().Where(x => !Investigated.Contains(x.PlayerId) && !((x.Is(Faction.Intruder) || x.GetSubFaction() ==
                Player.GetSubFaction()) && CustomGameOptions.FactionSeeRoles)).ToList();
            InvestigateButton.Update("INVESTIGATE", ConsigliereTimer(), CustomGameOptions.ConsigCd, notInvestigated);
        }
    }
}