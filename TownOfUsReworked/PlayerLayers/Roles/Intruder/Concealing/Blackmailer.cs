namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Blackmailer : Intruder
    {
        public CustomButton BlackmailButton;
        public PlayerControl BlackmailedPlayer;
        public DateTime LastBlackmailed;
        public bool ShookAlready;
        public Sprite PrevOverlay;
        public Color PrevColor;

        public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Blackmailer : Colors.Intruder;
        public override string Name => "Blackmailer";
        public override LayerEnum Type => LayerEnum.Blackmailer;
        public override RoleEnum RoleType => RoleEnum.Blackmailer;
        public override Func<string> StartText => () => "You Know Their Secrets";
        public override Func<string> AbilitiesText => () => "- You can silence players to ensure they cannot hear what others say\n" + (CustomGameOptions.BMRevealed ? "- Everyone will be "
            + "alerted at the start of the meeting that someone has been silenced " : "") + (CustomGameOptions.WhispersNotPrivate ? "\n- You can read whispers during meetings" : "") +
            $"\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

        public Blackmailer(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.IntruderConceal;
            BlackmailedPlayer = null;
            BlackmailButton = new(this, "Blackmail", AbilityTypes.Direct, "Secondary", Blackmail, Exception1);
        }

        public float BlackmailTimer()
        {
            var timespan = DateTime.UtcNow - LastBlackmailed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BlackmailCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Blackmail()
        {
            if (BlackmailTimer() != 0f || IsTooFar(Player, BlackmailButton.TargetPlayer) || BlackmailButton.TargetPlayer == BlackmailedPlayer)
                return;

            var interact = Interact(Player, BlackmailButton.TargetPlayer);

            if (interact[3])
            {
                BlackmailedPlayer = BlackmailButton.TargetPlayer;
                CallRpc(CustomRPC.Action, ActionsRPC.Blackmail, this, BlackmailedPlayer);
            }

            if (interact[0])
                LastBlackmailed = DateTime.UtcNow;
            else if (interact[1])
                LastBlackmailed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => player == BlackmailedPlayer || (player.Is(Faction) && Faction != Faction.Crew && CustomGameOptions.BlackmailMates) ||
            (player.Is(SubFaction) && SubFaction != SubFaction.None && CustomGameOptions.BlackmailMates);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            BlackmailButton.Update("BLACKMAIL", BlackmailTimer(), CustomGameOptions.BlackmailCd);
        }
    }
}