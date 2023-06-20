namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Executioner : Neutral
    {
        public PlayerControl TargetPlayer;
        public bool TargetVotedOut;
        public List<byte> ToDoom = new();
        public bool HasDoomed;
        public CustomButton DoomButton;
        public DateTime LastDoomed;
        public int UsesLeft;
        public bool CanDoom => TargetVotedOut && !HasDoomed && UsesLeft > 0 && ToDoom.Count > 0 && !CustomGameOptions.AvoidNeutralKingmakers;
        public bool Failed => TargetPlayer != null && !TargetVotedOut && (TargetPlayer.Data.IsDead || TargetPlayer.Data.Disconnected);
        public int Rounds;
        public CustomButton TargetButton;
        public bool TargetFailed => TargetPlayer == null && Rounds > 2;

        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            StartText = () => "Find Someone To Eject";
            Objectives = () => TargetVotedOut ? $"- {TargetPlayer?.name} has been ejected" : (TargetPlayer == null ? "- Find a target to eject" : $"- Eject {TargetPlayer?.name}");
            Color = CustomGameOptions.CustomNeutColors ? Colors.Executioner : Colors.Neutral;
            RoleType = RoleEnum.Executioner;
            RoleAlignment = RoleAlignment.NeutralEvil;
            ToDoom = new();
            UsesLeft = CustomGameOptions.DoomCount;
            AbilitiesText = () => $"- After {TargetPlayer?.name} has been ejected, you can doom players who voted for them\n- If {TargetPlayer?.name} dies, you will become a " +
                "<color=#F7B3DAFF>Jester</color>";
            Type = LayerEnum.Executioner;
            DoomButton = new(this, "Doom", AbilityTypes.Direct, "ActionSecondary", Doom, Exception, true);
            TargetButton = new(this, "ExeTarget", AbilityTypes.Direct, "ActionSecondary", SelectTarget);
            Rounds = 0;
            InspectorResults = InspectorResults.Manipulative;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void SelectTarget()
        {
            if (TargetPlayer != null)
                return;

            TargetPlayer = TargetButton.TargetPlayer;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
            writer.Write((byte)TargetRPC.SetExeTarget);
            writer.Write(PlayerId);
            writer.Write(TargetPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);

            if (TargetVotedOut)
                return;

            ToDoom.Clear();

            foreach (var state in __instance.playerStates)
            {
                if (state.AmDead || Utils.PlayerByVoteArea(state).Data.Disconnected || state.VotedFor != TargetPlayer.PlayerId || state.TargetPlayerId == Player.PlayerId)
                    continue;

                ToDoom.Add(state.TargetPlayerId);
            }

            while (ToDoom.Count > UsesLeft)
            {
                ToDoom.Shuffle();
                ToDoom.Remove(ToDoom[^1]);
            }

            UsesLeft = CustomGameOptions.DoomCount <= ToDoom.Count ? CustomGameOptions.DoomCount : ToDoom.Count;
        }

        public float DoomTimer()
        {
            var timespan = DateTime.UtcNow - LastDoomed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DoomCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnJest()
        {
            var newRole = new Jester(Player);
            newRole.RoleUpdate(this);

            if (Local && !IntroCutscene.Instance)
                Utils.Flash(Colors.Jester);

            if (CustomPlayer.Local.Is(RoleEnum.Seer) && !IntroCutscene.Instance)
                Utils.Flash(Colors.Seer);
        }

        public void Doom()
        {
            if (Utils.IsTooFar(Player, DoomButton.TargetPlayer) || DoomTimer() != 0f || !CanDoom)
                return;

            Utils.RpcMurderPlayer(Player, DoomButton.TargetPlayer, DeathReasonEnum.Doomed, false);
            HasDoomed = true;
            UsesLeft--;
            LastDoomed = DateTime.UtcNow;
        }

        public bool Exception(PlayerControl player) => !ToDoom.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(ObjectifierEnum.Mafia) &&
            Player.Is(ObjectifierEnum.Mafia));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            DoomButton.Update("DOOM", DoomTimer(), CustomGameOptions.DoomCooldown, UsesLeft, CanDoom, CanDoom && TargetPlayer != null);
            TargetButton.Update("SET TARGET", 0, 1, true, TargetPlayer == null);

            if ((TargetFailed || Failed) && !IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnJest);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TurnJest();
            }
        }
    }
}