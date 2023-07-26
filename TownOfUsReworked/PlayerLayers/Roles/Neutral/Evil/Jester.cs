namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Jester : Neutral
    {
        public bool VotedOut;
        public List<byte> ToHaunt = new();
        public bool HasHaunted;
        public CustomButton HauntButton;
        public DateTime LastHaunted;
        public int UsesLeft;
        public bool CanHaunt => VotedOut && !HasHaunted && UsesLeft > 0 && ToHaunt.Count > 0 && !CustomGameOptions.AvoidNeutralKingmakers;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Jester : Colors.Neutral;
        public override string Name => "Jester";
        public override LayerEnum Type => LayerEnum.Jester;
        public override RoleEnum RoleType => RoleEnum.Jester;
        public override Func<string> StartText => () => "It Was Jest A Prank Bro";
        public override Func<string> AbilitiesText => () => VotedOut ? "- You can haunt those who voted for you" : "- None";
        public override InspectorResults InspectorResults => InspectorResults.Manipulative;

        public Jester(PlayerControl player) : base(player)
        {
            Objectives = () => VotedOut ? "- You have been ejected" : "- Get ejected";
            RoleAlignment = RoleAlignment.NeutralEvil;
            ToHaunt = new();
            UsesLeft = CustomGameOptions.HauntCount;
            HauntButton = new(this, "Haunt", AbilityTypes.Direct, "ActionSecondary", Haunt, Exception, true, true);
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);

            if (VotedOut)
                return;

            ToHaunt.Clear();

            foreach (var state in __instance.playerStates)
            {
                var player = PlayerByVoteArea(state);

                if (state.AmDead || player.Data.Disconnected || state.VotedFor != PlayerId || state.TargetPlayerId == PlayerId || Player.IsLinkedTo(player))
                    continue;

                ToHaunt.Add(state.TargetPlayerId);
            }

            while (ToHaunt.Count > UsesLeft)
            {
                ToHaunt.Shuffle();
                ToHaunt.Remove(ToHaunt[^1]);
            }

            UsesLeft = CustomGameOptions.HauntCount <= ToHaunt.Count ? CustomGameOptions.HauntCount : ToHaunt.Count;
        }

        public float HauntTimer()
        {
            var timespan = DateTime.UtcNow - LastHaunted;
            var num = CustomGameOptions.HauntCooldown * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public bool Exception(PlayerControl player) => !ToHaunt.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            HauntButton.Update("HAUNT", HauntTimer(), CustomGameOptions.HauntCooldown, UsesLeft, CanHaunt, CanHaunt);
        }

        public void Haunt()
        {
            if (IsTooFar(Player, HauntButton.TargetPlayer) || HauntTimer() != 0f || !CanHaunt)
                return;

            RpcMurderPlayer(Player, HauntButton.TargetPlayer, DeathReasonEnum.Haunted, false);
            HasHaunted = true;
            UsesLeft--;
            LastHaunted = DateTime.UtcNow;
        }
    }
}