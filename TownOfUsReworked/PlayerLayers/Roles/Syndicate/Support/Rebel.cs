namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Rebel : Syndicate
    {
        public bool HasDeclared { get; set; }
        public CustomButton SidekickButton { get; set; }

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Rebel : Colors.Syndicate;
        public override string Name => "Rebel";
        public override LayerEnum Type => LayerEnum.Rebel;
        public override RoleEnum RoleType => RoleEnum.Rebel;
        public override Func<string> StartText => () => "Promote Your Fellow <color=#008000FF>Syndicate</color> To Do Better";
        public override Func<string> Description => () => "- You can promote a fellow <color=#008000FF>Syndicate</color> into becoming your successor\n- Promoting a <color=#008000FF>" +
            "Syndicate</color> turns them into a <color=#979C9FFF>Sidekick</color>\n- If you die, the <color=#979C9FFF>Sidekick</color> become the new <color=#FFFCCEFF>Rebel</color>\n" +
            $"and inherits better abilities of their former role\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.LeadsTheGroup;

        public Rebel(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.SyndicateSupport;
            SidekickButton = new(this, "Sidekick", AbilityTypes.Direct, "Secondary", Sidekick);
        }

        public static void Sidekick(Rebel reb, PlayerControl target)
        {
            reb.HasDeclared = true;
            var formerRole = GetRole(target);

            var sidekick = new Sidekick(target)
            {
                FormerRole = formerRole,
                Rebel = reb
            };

            sidekick.RoleUpdate(formerRole);

            if (target == CustomPlayer.Local)
                Flash(Colors.Rebel);

            if (CustomPlayer.Local.Is(RoleEnum.Seer))
                Flash(Colors.Seer);
        }

        public void Sidekick()
        {
            if (IsTooFar(Player, SidekickButton.TargetPlayer) || HasDeclared)
                return;

            var interact = Interact(Player, SidekickButton.TargetPlayer);

            if (interact[3])
            {
                CallRpc(CustomRPC.Action, ActionsRPC.Sidekick, this, SidekickButton.TargetPlayer);
                Sidekick(this, SidekickButton.TargetPlayer);
            }
        }

        public bool Exception1(PlayerControl player) => !player.Is(Faction) || (!(player.GetRole() is RoleEnum.PromotedRebel or RoleEnum.Sidekick or RoleEnum.Rebel) && player.Is(Faction));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            SidekickButton.Update("SIDEKICK", true, !HasDeclared);
        }
    }
}