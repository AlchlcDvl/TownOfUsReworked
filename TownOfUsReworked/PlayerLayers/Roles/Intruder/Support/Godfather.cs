namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Godfather : IntruderRole
    {
        public bool HasDeclared;
        public CustomButton DeclareButton;
        public DateTime LastDeclared;

        public Godfather(PlayerControl player) : base(player)
        {
            Name = "Godfather";
            RoleType = RoleEnum.Godfather;
            StartText = "Promote Your Fellow <color=#FF0000FF>Intruders</color> To Do Better";
            AbilitiesText = "- You can promote a fellow <color=#FF0000FF>Intruder</color> into becoming your successor\n- Promoting an <color=#FF0000FF>Intruder</color> turns them " +
                "into a <color=#6400FFFF>Mafioso</color>\n- If you die, the <color=#6400FFFF>Mafioso</color> will become the new <color=#404C08FF>Godfather</color>\nand inherits better " +
                $"abilities of their former role\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Godfather : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            Type = LayerEnum.Godfather;
            DeclareButton = new(this, "Promote", AbilityTypes.Direct, "Secondary", Declare);
            InspectorResults = InspectorResults.LeadsTheGroup;
        }

        public static void Declare(Godfather gf, PlayerControl target)
        {
            gf.HasDeclared = true;
            target.DisableButtons();
            var formerRole = GetRole(target);

            var mafioso = new Mafioso(target)
            {
                FormerRole = formerRole,
                Godfather = gf
            };

            mafioso.RoleUpdate(formerRole);
            target.EnableButtons();

            if (target == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Mafioso);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public void Declare()
        {
            if (Utils.IsTooFar(Player, DeclareButton.TargetPlayer) || HasDeclared)
                return;

            var interact = Utils.Interact(Player, DeclareButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Declare);
                writer.Write(PlayerId);
                writer.Write(DeclareButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Declare(this, DeclareButton.TargetPlayer);
            }
            else if (interact[0])
                LastDeclared = DateTime.UtcNow;
            else if (interact[1])
                LastDeclared.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => !player.Is(Faction) || player.GetRole() is RoleEnum.PromotedGodfather or RoleEnum.Mafioso or RoleEnum.Godfather;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            DeclareButton.Update("PROMOTE", true, !HasDeclared);
        }
    }
}