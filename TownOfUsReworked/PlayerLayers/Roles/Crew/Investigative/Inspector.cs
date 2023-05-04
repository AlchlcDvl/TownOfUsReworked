using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using Il2CppSystem.Collections.Generic;
using System;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Inspector : CrewRole
    {
        public DateTime LastInspected;
        public List<byte> Inspected = new();
        public CustomButton InspectButton;

        public Inspector(PlayerControl player) : base(player)
        {
            Name = "Inspector";
            RoleType = RoleEnum.Inspector;
            StartText = "Inspect Players For Their Roles";
            AbilitiesText = "- You can check a player to get a role list of what they could be";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Inspector : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            Inspected = new();
            InspectorResults = InspectorResults.GainsInfo;
            Type = LayerEnum.Inspector;
            InspectButton = new(this, "Inspect", AbilityTypes.Direct, "ActionSecondary", Inspect);
        }

        public float InspectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInspected;
            var num = Player.GetModifiedCooldown(CustomGameOptions.InspectCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Inspect()
        {
            if (InspectTimer() != 0f || Utils.IsTooFar(Player, InspectButton.TargetPlayer) || Inspected.Contains(InspectButton.TargetPlayer.PlayerId))
                return;

            var interact = Utils.Interact(Player, InspectButton.TargetPlayer);

            if (interact[3])
                Inspected.Add(InspectButton.TargetPlayer.PlayerId);

            if (interact[0])
                LastInspected = DateTime.UtcNow;
            else if (interact[1])
                LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notinspected = PlayerControl.AllPlayerControls.ToArray().Where(x => !Inspected.Contains(x.PlayerId) || (Faction is Faction.Intruder or Faction.Syndicate &&
                CustomGameOptions.FactionSeeRoles && x.GetFaction() == Faction)).ToList();
            InspectButton.Update("INSPECT", InspectTimer(), CustomGameOptions.InspectCooldown, notinspected);
        }
    }
}