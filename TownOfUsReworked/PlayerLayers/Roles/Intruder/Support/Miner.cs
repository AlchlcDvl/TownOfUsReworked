namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Miner : Intruder
    {
        public CustomButton MineButton;
        public DateTime LastMined;
        public bool CanPlace;
        public List<Vent> Vents = new();

        public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Miner : Colors.Intruder;
        public override string Name => "Miner";
        public override LayerEnum Type => LayerEnum.Miner;
        public override RoleEnum RoleType => RoleEnum.Miner;
        public override Func<string> StartText => () => "From The Top, Make It Drop, Boom, That's A Vent";
        public override Func<string> AbilitiesText => () => $"- You can mine a vent, forming a vent system of your own\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.NewLens;

        public Miner(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.IntruderSupport;
            MineButton = new(this, "Mine", AbilityTypes.Effect, "Secondary", Mine);
            Vents = new();
        }

        public float MineTimer()
        {
            var timespan = DateTime.UtcNow - LastMined;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MineCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Mine()
        {
            if (!CanPlace || MineTimer() != 0f)
                return;

            RpcSpawnVent(this);
            LastMined = DateTime.UtcNow;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var hits = Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0);
            hits = hits.Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not 8 and not 5).ToArray();
            CanPlace = hits.Count == 0 && Player.moveable && !GetPlayerElevator(Player).IsInElevator;
            MineButton.Update("MINE", MineTimer(), CustomGameOptions.MineCd, CanPlace);
        }
    }
}