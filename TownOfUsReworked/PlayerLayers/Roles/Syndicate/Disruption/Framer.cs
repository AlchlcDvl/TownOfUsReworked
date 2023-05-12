namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Framer : SyndicateRole
    {
        public CustomButton FrameButton;
        public CustomButton RadialFrameButton;
        public List<byte> Framed = new();
        public DateTime LastFramed;

        public Framer(PlayerControl player) : base(player)
        {
            Name = "Framer";
            StartText = "Make Everyone Suspicious";
            AbilitiesText = "- You can frame players\n- Framed players will die very easily to killing roles and will appear to have the wrong results to investigative roles till you" +
                $" are dead\n- With the Chaos Drive, you can frame all players within a{CustomGameOptions.ChaosDriveFrameRadius}m radius\n{AbilitiesText}";
            RoleType = RoleEnum.Framer;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
            Color = CustomGameOptions.CustomSynColors ? Colors.Framer : Colors.Syndicate;
            Framed = new();
            Type = LayerEnum.Framer;
            FrameButton = new(this, "Frame", AbilityTypes.Direct, "Secondary", HitFrame, Exception1);
            RadialFrameButton = new(this, "Frame", AbilityTypes.Effect, "Secondary", RadialFrame);
            InspectorResults = InspectorResults.Manipulative;
        }

        public float FrameTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFramed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FrameCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Frame(PlayerControl player)
        {
            if (player.Is(Faction.Syndicate) || Framed.Contains(player.PlayerId))
                return;

            Framed.Add(player.PlayerId);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Frame);
            writer.Write(PlayerId);
            writer.Write(PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void HitFrame()
        {
            if (FrameTimer() != 0f || Utils.IsTooFar(Player, FrameButton.TargetPlayer) || HoldsDrive)
                return;

            var interact = Utils.Interact(Player, FrameButton.TargetPlayer);

            if (interact[3])
                Frame(FrameButton.TargetPlayer);

            if (interact[0])
                LastFramed = DateTime.UtcNow;
            else if (interact[1])
                LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void RadialFrame()
        {
            if (FrameTimer() != 0f || !HoldsDrive)
                return;

            foreach (var player in Utils.GetClosestPlayers(PlayerControl.LocalPlayer.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius))
                Frame(player);

            LastFramed = DateTime.UtcNow;
        }

        public bool Exception1(PlayerControl player) => Framed.Contains(player.PlayerId) || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            FrameButton.Update("FRAME", FrameTimer(), CustomGameOptions.FrameCooldown, true, !HoldsDrive);
            RadialFrameButton.Update("FRAME", FrameTimer(), CustomGameOptions.FrameCooldown, true, HoldsDrive);
        }
    }
}