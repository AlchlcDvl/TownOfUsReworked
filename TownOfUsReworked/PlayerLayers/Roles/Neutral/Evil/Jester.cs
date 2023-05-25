namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Jester : NeutralRole
    {
        public bool VotedOut;
        public List<byte> ToHaunt = new();
        public bool HasHaunted;
        public CustomButton HauntButton;
        public DateTime LastHaunted;
        public int UsesLeft;
        public bool CanHaunt => VotedOut && !HasHaunted && UsesLeft > 0 && ToHaunt.Count > 0;

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            StartText = "It Was Jest A Prank Bro";
            AbilitiesText = "- After you get ejected, you can haunt those who voted for you";
            Objectives = "- Get ejected";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jester : Colors.Neutral;
            RoleType = RoleEnum.Jester;
            RoleAlignment = RoleAlignment.NeutralEvil;
            ToHaunt = new();
            UsesLeft = CustomGameOptions.HauntCount;
            Type = LayerEnum.Jester;
            HauntButton = new(this, "Haunt", AbilityTypes.Direct, "ActionSecondary", Haunt, Exception, true, true);
            InspectorResults = InspectorResults.Manipulative;
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);

            if (VotedOut)
                return;

            ToHaunt.Clear();

            foreach (var state in __instance.playerStates)
            {
                if (state.AmDead || Utils.PlayerById(state.TargetPlayerId).Data.Disconnected || state.VotedFor != Player.PlayerId || state.TargetPlayerId == Player.PlayerId)
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
            var num = Player.GetModifiedCooldown(CustomGameOptions.HauntCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public bool Exception(PlayerControl player) => !ToHaunt.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(ObjectifierEnum.Mafia) &&
            Player.Is(ObjectifierEnum.Mafia));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            HauntButton.Update("HAUNT", HauntTimer(), CustomGameOptions.HauntCooldown, UsesLeft, CanHaunt, CanHaunt);
        }

        public void Haunt()
        {
            if (Utils.IsTooFar(Player, HauntButton.TargetPlayer) || HauntTimer() != 0f || !CanHaunt)
                return;

            Utils.RpcMurderPlayer(Player, HauntButton.TargetPlayer, DeathReasonEnum.Haunted, false);
            HasHaunted = true;
            UsesLeft--;
            LastHaunted = DateTime.UtcNow;
        }
    }
}