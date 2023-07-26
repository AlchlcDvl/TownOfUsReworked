namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Monarch : Crew
    {
        public bool RoundOne;
        public CustomButton KnightButton;
        public List<byte> ToBeKnighted = new();
        public List<byte> Knighted = new();
        public DateTime LastKnighted;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool Protected => Knighted.Count > 0;

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Monarch : Colors.Crew;
        public override string Name => "Monarch";
        public override LayerEnum Type => LayerEnum.Monarch;
        public override RoleEnum RoleType => RoleEnum.Monarch;
        public override Func<string> StartText => () => "Knight Those Who You Trust";
        public override Func<string> AbilitiesText => () => $"- You can knight players\n- Knighted players will have their votes count {CustomGameOptions.KnightVoteCount + 1} times\n- As "
            + "long as a knight is alive, you cannot be killed";
        public override InspectorResults InspectorResults => InspectorResults.NewLens;

        public Monarch(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewSov;
            Knighted = new();
            ToBeKnighted = new();
            UsesLeft = CustomGameOptions.KnightCount;
            KnightButton = new(this, "Knight", AbilityTypes.Direct, "ActionSecondary", Knight, Exception, true);
        }

        public float KnightTimer()
        {
            var timespan = DateTime.UtcNow - LastKnighted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.KnightingCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Knight()
        {
            if (IsTooFar(Player, KnightButton.TargetPlayer) || KnightTimer() != 0f || !ButtonUsable || RoundOne)
                return;

            CallRpc(CustomRPC.Action, ActionsRPC.Knight, this, KnightButton.TargetPlayer.PlayerId);
            ToBeKnighted.Add(KnightButton.TargetPlayer.PlayerId);
            UsesLeft--;
            LastKnighted = DateTime.UtcNow;
        }

        public bool Exception(PlayerControl player) => ToBeKnighted.Contains(player.PlayerId) || player.IsKnighted();

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KnightButton.Update("KNIGHT", KnightTimer(), CustomGameOptions.KnightingCooldown, UsesLeft, ButtonUsable, !RoundOne && ButtonUsable);
        }
    }
}