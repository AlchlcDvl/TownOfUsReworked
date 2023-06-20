namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Troll : Neutral
    {
        public bool Killed;
        public DateTime LastInteracted;
        public CustomButton InteractButton;

        public Troll(PlayerControl player) : base(player)
        {
            Name = "Troll";
            StartText = () => "Troll Everyone With Your Death";
            AbilitiesText = () => "- You can interact with players\n- Your interactions do nothing except spread infection and possibly kill you via touch sensitive roles";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Troll : Colors.Neutral;
            RoleType = RoleEnum.Troll;
            RoleAlignment = RoleAlignment.NeutralEvil;
            Objectives = () => Killed ? "- You have successfully trolled someone" : "- Get killed";
            Type = LayerEnum.Troll;
            InteractButton = new(this, "Placeholder", AbilityTypes.Direct, "ActionSecondary", Interact);
            InspectorResults = InspectorResults.Manipulative;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float InteractTimer()
        {
            var timespan = DateTime.UtcNow - LastInteracted;
            var num = CustomGameOptions.InteractCooldown * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Interact()
        {
            if (InteractTimer() != 0f || Utils.IsTooFar(Player, InteractButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, InteractButton.TargetPlayer);

            if (interact[3] || interact[0])
                LastInteracted = DateTime.UtcNow;
            else if (interact[0])
                LastInteracted = DateTime.UtcNow;
            else if (interact[1])
                LastInteracted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            InteractButton.Update("INTERACT", InteractTimer(), CustomGameOptions.InteractCooldown);
        }
    }
}