namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Rebel : SyndicateRole
    {
        public bool HasDeclared;
        public CustomButton SidekickButton;
        public DateTime LastDeclared;

        public Rebel(PlayerControl player) : base(player)
        {
            Name = "Rebel";
            RoleType = RoleEnum.Rebel;
            StartText = () => "Promote Your Fellow <color=#008000FF>Syndicate</color> To Do Better";
            AbilitiesText = () => "- You can promote a fellow <color=#008000FF>Syndicate</color> into becoming your successor\n- Promoting an <color=#008000FF>Syndicate</color> turns " +
                "them into a <color=#979C9FFF>Sidekick</color>\n- If you die, the <color=#979C9FFF>Sidekick</color> become the new <color=#FFFCCEFF>Rebel</color>\nand inherits better " +
                $"abilities of their former role\n{CommonAbilities}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Rebel : Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            Type = LayerEnum.Rebel;
            SidekickButton = new(this, "Sidekick", AbilityTypes.Direct, "Secondary", Sidekick);
            InspectorResults = InspectorResults.LeadsTheGroup;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
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

            if (target == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Rebel);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public void Sidekick()
        {
            if (Utils.IsTooFar(Player, SidekickButton.TargetPlayer) || HasDeclared)
                return;

            var interact = Utils.Interact(Player, SidekickButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Sidekick);
                writer.Write(PlayerId);
                writer.Write(SidekickButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Sidekick(this, SidekickButton.TargetPlayer);
            }
            else if (interact[0])
                LastDeclared = DateTime.UtcNow;
            else if (interact[1])
                LastDeclared.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => !player.Is(Faction) || player.GetRole() is RoleEnum.PromotedRebel or RoleEnum.Sidekick or RoleEnum.Rebel;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            SidekickButton.Update("SIDEKICK", true, !HasDeclared);
        }
    }
}