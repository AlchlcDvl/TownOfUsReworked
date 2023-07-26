namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Chameleon : Crew
    {
        public bool Enabled;
        public DateTime LastSwooped;
        public float TimeRemaining;
        public bool IsSwooped => TimeRemaining > 0f;
        public CustomButton SwoopButton;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Chameleon : Colors.Crew;
        public override string Name => "Chameleon";
        public override LayerEnum Type => LayerEnum.Chameleon;
        public override RoleEnum RoleType => RoleEnum.Chameleon;
        public override Func<string> StartText => () => "Go Invisible To Stalk Players";
        public override Func<string> AbilitiesText => () => "- You can turn invisible";
        public override InspectorResults InspectorResults => InspectorResults.Unseen;

        public Chameleon(PlayerControl player) : base(player)
        {
            UsesLeft = CustomGameOptions.SwoopCount;
            SwoopButton = new(this, "Swoop", AbilityTypes.Effect, "ActionSecondary", HitSwoop, true);
        }

        public float SwoopTimer()
        {
            var timespan = DateTime.UtcNow - LastSwooped;
            var num = Player.GetModifiedCooldown(CustomGameOptions.SwoopCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Invis()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Invis(Player);

            if (Meeting || IsDead)
                TimeRemaining = 0f;
        }

        public void Uninvis()
        {
            Enabled = false;
            LastSwooped = DateTime.UtcNow;
            DefaultOutfit(Player);
        }

        public void HitSwoop()
        {
            if (SwoopTimer() != 0f || IsSwooped || !ButtonUsable)
                return;

            TimeRemaining = CustomGameOptions.SwoopDuration;
            Invis();
            UsesLeft--;
            CallRpc(CustomRPC.Action, ActionsRPC.Swoop, this);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            SwoopButton.Update("SWOOP", SwoopTimer(), CustomGameOptions.SwoopCooldown, UsesLeft, IsSwooped, TimeRemaining, CustomGameOptions.SwoopDuration);
        }
    }
}