namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Miner : IntruderRole
    {
        public CustomButton MineButton;
        public DateTime LastMined;
        public bool CanPlace;

        public Miner(PlayerControl player) : base(player)
        {
            Name = "Miner";
            StartText = () => "From The Top, Make It Drop, Boom, That's A Vent";
            AbilitiesText = () => $"- You can mine a vent, forming a vent system of your own\n{AbilitiesText()}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Miner : Colors.Intruder;
            RoleType = RoleEnum.Miner;
            RoleAlignment = RoleAlignment.IntruderSupport;
            Type = LayerEnum.Miner;
            MineButton = new(this, "Mine", AbilityTypes.Effect, "Secondary", Mine);
            InspectorResults = InspectorResults.NewLens;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float MineTimer()
        {
            var timespan = DateTime.UtcNow - LastMined;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MineCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Mine()
        {
            if (!CanPlace || MineTimer() != 0f)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Mine);
            var position = Player.transform.position;
            var id = Utils.GetAvailableId();
            writer.Write(id);
            writer.Write(PlayerId);
            writer.Write(position);
            writer.Write(position.z + 0.01f);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Utils.SpawnVent(id, this, position, position.z + 0.01f);
            LastMined = DateTime.UtcNow;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var hits = Physics2D.OverlapBoxAll(Player.transform.position, Utils.GetSize(), 0);
            hits = hits.Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
            CanPlace = hits.Count == 0 && Player.moveable && !ModCompatibility.GetPlayerElevator(Player).Item1;
            MineButton.Update("MINE", MineTimer(), CustomGameOptions.MineCd, CanPlace);
        }
    }
}