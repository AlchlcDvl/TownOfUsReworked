namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Framer : Syndicate
    {
        public CustomButton FrameButton;
        public CustomButton RadialFrameButton;
        public List<byte> Framed = new();
        public DateTime LastFramed;

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Framer : Colors.Syndicate;
        public override string Name => "Framer";
        public override LayerEnum Type => LayerEnum.Framer;
        public override RoleEnum RoleType => RoleEnum.Framer;
        public override Func<string> StartText => () => "Make Everyone Suspicious";
        public override Func<string> AbilitiesText => () => $"- You can frame {(HoldsDrive ? $"all players within a {CustomGameOptions.ChaosDriveFrameRadius}m radius" : "a player")}\n- " +
            $"Framed players will die very easily to killing roles and will appear to have the wrong results to investigative roles till you are dead\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.Manipulative;

        public Framer(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.SyndicateDisrup;
            Framed = new();
            FrameButton = new(this, "Frame", AbilityTypes.Direct, "Secondary", Frame, Exception1);
            RadialFrameButton = new(this, "RadialFrame", AbilityTypes.Effect, "Secondary", RadialFrame);
        }

        public float FrameTimer()
        {
            var timespan = DateTime.UtcNow - LastFramed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FrameCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void RpcFrame(PlayerControl player)
        {
            if ((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || Framed.Contains(player.PlayerId))
                return;

            Framed.Add(player.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.Frame, this, player);
        }

        public void Frame()
        {
            if (FrameTimer() != 0f || IsTooFar(Player, FrameButton.TargetPlayer) || HoldsDrive)
                return;

            var interact = Interact(Player, FrameButton.TargetPlayer);

            if (interact[3])
                RpcFrame(FrameButton.TargetPlayer);

            if (interact[0])
                LastFramed = DateTime.UtcNow;
            else if (interact[1])
                LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void RadialFrame()
        {
            if (FrameTimer() != 0f || !HoldsDrive)
                return;

            GetClosestPlayers(CustomPlayer.Local.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius).ForEach(RpcFrame);
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