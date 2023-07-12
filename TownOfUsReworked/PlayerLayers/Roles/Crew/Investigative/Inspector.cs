using static TownOfUsReworked.Languages.Language;
namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Inspector : Crew
    {
        public DateTime LastInspected;
        public List<byte> Inspected = new();
        public CustomButton InspectButton;

        public Inspector(PlayerControl player) : base(player)
        {
            Name = GetString("Inspector");
            RoleType = RoleEnum.Inspector;
            StartText = () => GetString("InspectorStartText");
            AbilitiesText = () => GetString("InspectorAbilitiesText");
            Color = CustomGameOptions.CustomCrewColors ? Colors.Inspector : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            Inspected = new();
            InspectorResults = InspectorResults.GainsInfo;
            Type = LayerEnum.Inspector;
            InspectButton = new(this, "Inspect", AbilityTypes.Direct, "ActionSecondary", Inspect, Exception);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float InspectTimer()
        {
            var timespan = DateTime.UtcNow - LastInspected;
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

        public bool Exception(PlayerControl player) => Inspected.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
            (player.Is(SubFaction) && SubFaction != SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (player == Player.GetOtherLover() && CustomGameOptions.LoversRoles) || (player
            == Player.GetOtherRival() && CustomGameOptions.RivalsRoles) || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia) && CustomGameOptions.MafiaRoles);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            InspectButton.Update("INSPECT", InspectTimer(), CustomGameOptions.InspectCooldown);
        }
    }
}