namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Silencer : SyndicateRole
    {
        public CustomButton SilenceButton;
        public PlayerControl SilencedPlayer;
        public DateTime LastSilenced;
        public bool ShookAlready;
        public Sprite PrevOverlay;
        public Color PrevColor;

        public Silencer(PlayerControl player) : base(player)
        {
            Name = "Silencer";
            StartText = () => "You Are The One Who Screams";
            AbilitiesText = () => "- You can silence players to ensure they cannot hear what others say\n" + (CustomGameOptions.SilenceRevealed ? "- Everyone will be alerted at the start "
                + "of the meeting that someone has been silenced " : "") + (CustomGameOptions.WhispersNotPrivateSilencer ? "\n- You can read whispers during meetings" : "") +
                $"\n{AbilitiesText()}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Silencer : Colors.Syndicate;
            RoleType = RoleEnum.Silencer;
            RoleAlignment = RoleAlignment.SyndicateDisrup;
            InspectorResults = InspectorResults.GainsInfo;
            SilencedPlayer = null;
            Type = LayerEnum.Silencer;
            SilenceButton = new(this, "Silence", AbilityTypes.Direct, "Secondary", Silence, Exception1);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float SilenceTimer()
        {
            var timespan = DateTime.UtcNow - LastSilenced;
            var num = Player.GetModifiedCooldown(CustomGameOptions.SilenceCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Silence()
        {
            if (SilenceTimer() != 0f || Utils.IsTooFar(Player, SilenceButton.TargetPlayer) || SilenceButton.TargetPlayer == SilencedPlayer)
                return;

            var interact = Utils.Interact(Player, SilenceButton.TargetPlayer);

            if (interact[3])
            {
                SilencedPlayer = SilenceButton.TargetPlayer;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Silence);
                writer.Write(PlayerId);
                writer.Write(SilenceButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            if (interact[0])
                LastSilenced = DateTime.UtcNow;
            else if (interact[1])
                LastSilenced.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => player == SilencedPlayer || (player.Is(Faction) && Faction != Faction.Crew && CustomGameOptions.SilenceMates) ||
            (player.Is(SubFaction) && SubFaction != SubFaction.None && CustomGameOptions.SilenceMates);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            SilenceButton.Update("SILENCE", SilenceTimer(), CustomGameOptions.SilenceCooldown);
        }
    }
}