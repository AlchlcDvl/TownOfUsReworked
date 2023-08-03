namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Chameleon : Crew
    {
        public bool Enabled { get; set; }
        public DateTime LastSwooped { get; set; }
        public float TimeRemaining { get; set; }
        public bool IsSwooped => TimeRemaining > 0f;
        public CustomButton SwoopButton { get; set; }
        public int UsesLeft { get; set; }
        public bool ButtonUsable => UsesLeft > 0;
        public float Timer => ButtonUtils.Timer(Player, LastSwooped, CustomGameOptions.SwoopCooldown);

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Chameleon : Colors.Crew;
        public override string Name => "Chameleon";
        public override LayerEnum Type => LayerEnum.Chameleon;
        public override RoleEnum RoleType => RoleEnum.Chameleon;
        public override Func<string> StartText => () => "Go Invisible To Stalk Players";
        public override Func<string> Description => () => "- You can turn invisible";
        public override InspectorResults InspectorResults => InspectorResults.Unseen;

        public Chameleon(PlayerControl player) : base(player)
        {
            UsesLeft = CustomGameOptions.SwoopCount;
            SwoopButton = new(this, "Swoop", AbilityTypes.Effect, "ActionSecondary", HitSwoop, true);
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
            if (Timer != 0f || IsSwooped || !ButtonUsable)
                return;

            TimeRemaining = CustomGameOptions.SwoopDuration;
            Invis();
            UsesLeft--;
            CallRpc(CustomRPC.Action, ActionsRPC.Swoop, this);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            SwoopButton.Update("SWOOP", Timer, CustomGameOptions.SwoopCooldown, UsesLeft, IsSwooped, TimeRemaining, CustomGameOptions.SwoopDuration);
        }
    }
}