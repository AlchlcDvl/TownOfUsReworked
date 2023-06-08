namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Blackmailer : IntruderRole
    {
        public CustomButton BlackmailButton;
        public PlayerControl BlackmailedPlayer;
        public DateTime LastBlackmailed;
        public bool ShookAlready;
        public Sprite PrevOverlay;
        public Color PrevColor;

        public Blackmailer(PlayerControl player) : base(player)
        {
            Name = "Blackmailer";
            StartText = () => "You Know Their Secrets";
            AbilitiesText = () => "- You can silence players to ensure they cannot hear what others say\n" + (CustomGameOptions.BMRevealed ? "- Everyone will be alerted at the start of " +
                "the meeting that someone has been silenced " : "") + (CustomGameOptions.WhispersNotPrivate ? "\n- You can read whispers during meetings" : "") + $"\n{CommonAbilities}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Blackmailer : Colors.Intruder;
            RoleType = RoleEnum.Blackmailer;
            RoleAlignment = RoleAlignment.IntruderConceal;
            InspectorResults = InspectorResults.GainsInfo;
            BlackmailedPlayer = null;
            Type = LayerEnum.Blackmailer;
            BlackmailButton = new(this, "Blackmail", AbilityTypes.Direct, "Secondary", Blackmail, Exception1);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float BlackmailTimer()
        {
            var timespan = DateTime.UtcNow - LastBlackmailed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BlackmailCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Blackmail()
        {
            if (BlackmailTimer() != 0f || Utils.IsTooFar(Player, BlackmailButton.TargetPlayer) || BlackmailButton.TargetPlayer == BlackmailedPlayer)
                return;

            var interact = Utils.Interact(Player, BlackmailButton.TargetPlayer);

            if (interact[3])
            {
                BlackmailedPlayer = BlackmailButton.TargetPlayer;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Blackmail);
                writer.Write(PlayerId);
                writer.Write(BlackmailButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
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