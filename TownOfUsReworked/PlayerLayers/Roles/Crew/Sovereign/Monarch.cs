namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Monarch : CrewRole
    {
        public bool RoundOne;
        public CustomButton KnightButton;
        public List<byte> ToBeKnighted = new();
        public List<byte> Knighted = new();
        public DateTime LastKnighted;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool Protected => Knighted.Count > 0 && Knighted.Any(x => Utils.PlayerById(x).Is(Faction.Crew));

        public Monarch(PlayerControl player) : base(player)
        {
            Name = "Monarch";
            StartText = "Knight Those Who You Trust";
            AbilitiesText = $"- You can knight players\n- Knighted players will have their votes count {CustomGameOptions.KnightVoteCount + 1} times";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Monarch : Colors.Crew;
            RoleType = RoleEnum.Monarch;
            RoleAlignment = RoleAlignment.CrewSov;
            InspectorResults = InspectorResults.NewLens;
            Type = LayerEnum.Monarch;
            Knighted = new();
            ToBeKnighted = new();
            UsesLeft = CustomGameOptions.KnightCount;
            KnightButton = new(this, "Knight", AbilityTypes.Direct, "ActionSecondary", Knight, Exception, true);
        }

        public float KnightTimer()
        {
            var timespan = DateTime.UtcNow - LastKnighted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.KnightingCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Knight()
        {
            if (Utils.IsTooFar(Player, KnightButton.TargetPlayer) || KnightTimer() != 0f || !ButtonUsable || RoundOne)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Knight);
            writer.Write(PlayerId);
            writer.Write(KnightButton.TargetPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            ToBeKnighted.Add(KnightButton.TargetPlayer.PlayerId);
            UsesLeft--;
        }

        public bool Exception(PlayerControl player) => ToBeKnighted.Contains(player.PlayerId) || player.IsKnighted();

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KnightButton.Update("KNIGHT", KnightTimer(), CustomGameOptions.KnightingCooldown, UsesLeft, ButtonUsable, !RoundOne && ButtonUsable);
        }
    }
}