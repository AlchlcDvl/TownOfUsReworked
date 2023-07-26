namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Silencer : Syndicate
    {
        public CustomButton SilenceButton;
        public PlayerControl SilencedPlayer;
        public DateTime LastSilenced;
        public bool ShookAlready;
        public Sprite PrevOverlay;
        public Color PrevColor;

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Silencer : Colors.Syndicate;
        public override string Name => "Silencer";
        public override LayerEnum Type => LayerEnum.Silencer;
        public override RoleEnum RoleType => RoleEnum.Silencer;
        public override Func<string> StartText => () => "You Are The One Who Hushes";
        public override Func<string> AbilitiesText => () => "- You can silence players to ensure they cannot hear what others say" + (CustomGameOptions.SilenceRevealed ? "\n- Everyone will"
            + " be alerted at the start of the meeting that someone has been silenced " : "") + (CustomGameOptions.WhispersNotPrivateSilencer ? "\n- You can read whispers during meetings"
            : "") + CommonAbilities;
        public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

        public Silencer(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.SyndicateDisrup;
            SilencedPlayer = null;
            SilenceButton = new(this, "Silence", AbilityTypes.Direct, "Secondary", Silence, Exception1);
        }

        public float SilenceTimer()
        {
            var timespan = DateTime.UtcNow - LastSilenced;
            var num = Player.GetModifiedCooldown(CustomGameOptions.SilenceCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Silence()
        {
            if (SilenceTimer() != 0f || IsTooFar(Player, SilenceButton.TargetPlayer) || SilenceButton.TargetPlayer == SilencedPlayer)
                return;

            var interact = Interact(Player, SilenceButton.TargetPlayer);

            if (interact[3])
            {
                SilencedPlayer = SilenceButton.TargetPlayer;
                CallRpc(CustomRPC.Action, ActionsRPC.Silence, this, SilencedPlayer);
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