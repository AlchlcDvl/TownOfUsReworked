namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Medic : CrewRole
    {
        public bool UsedAbility => ShieldedPlayer != null || ExShielded != null;
        public PlayerControl ShieldedPlayer;
        public PlayerControl ExShielded;
        public CustomButton ShieldButton;

        public Medic(PlayerControl player) : base(player)
        {
            Name = "Medic";
            StartText = () => "Shield A Player To Protect Them";
            AbilitiesText = () => "- You can shield a player to prevent them from dying to others\n- If your target is attacked, you will be notified of it by default\n- Your shield does "
                + "not save your target from suicides";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medic : Colors.Crew;
            RoleType = RoleEnum.Medic;
            ShieldedPlayer = null;
            ExShielded = null;
            RoleAlignment = RoleAlignment.CrewProt;
            InspectorResults = InspectorResults.PreservesLife;
            Type = LayerEnum.Medic;
            ShieldButton = new(this, "Shield", AbilityTypes.Direct, "ActionSecondary", Protect, Exception);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void Protect()
        {
            if (Utils.IsTooFar(Player, ShieldButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, ShieldButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Protect);
                writer.Write(PlayerId);
                writer.Write(ShieldButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                ShieldedPlayer = ShieldButton.TargetPlayer;
            }
        }

        public bool Exception(PlayerControl player) => player == ShieldedPlayer;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ShieldButton.Update("SHIELD", !UsedAbility, !UsedAbility);
        }

        public static void BreakShield(byte medicId, byte playerId, bool flag)
        {
            var role = GetRole<Medic>(Utils.PlayerById(medicId));

            if ((PlayerControl.LocalPlayer.PlayerId == playerId && (int)CustomGameOptions.NotificationShield is 1 or 2) || (PlayerControl.LocalPlayer.PlayerId == medicId &&
                (int)CustomGameOptions.NotificationShield is 0 or 2) || CustomGameOptions.NotificationShield == NotificationOptions.Everyone)
            {
                Utils.Flash(role.Color);
            }

            if (!flag)
                return;

            var player = Utils.PlayerById(playerId);

            foreach (var role2 in GetRoles<Medic>(RoleEnum.Medic))
            {
                if (role2.ShieldedPlayer.PlayerId == playerId)
                {
                    role2.ShieldedPlayer = null;
                    role2.ExShielded = player;
                    Utils.LogSomething(player.name + " Is Ex-Shielded");
                }
            }

            player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.MyRend().material.SetFloat("_Outline", 0f);
        }
    }
}