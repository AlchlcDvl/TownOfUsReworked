namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mystic : Crew
    {
        public DateTime LastRevealed;
        public static bool ConvertedDead => !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(SubFaction.None));
        public CustomButton RevealButton;

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Mystic : Colors.Crew;
        public override string Name => "Mystic";
        public override LayerEnum Type => LayerEnum.Mystic;
        public override RoleEnum RoleType => RoleEnum.Mystic;
        public override Func<string> StartText => () => "You Know When Converts Happen";
        public override Func<string> AbilitiesText => () => "- You can investigate players to see if they have been converted\n- Whenever someone has been converted, you will be alerted to"
            + " it\n- When all converted and converters die, you will become a <color=#71368AFF>Seer</color>";
        public override InspectorResults InspectorResults => InspectorResults.TracksOthers;

        public Mystic(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewAudit;
            RevealButton = new(this, "MysticReveal", AbilityTypes.Direct, "ActionSecondary", Reveal, Exception);
        }

        public float RevealTimer()
        {
            var timespan = DateTime.UtcNow - LastRevealed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.RevealCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void TurnSeer()
        {
            var newRole = new Seer(Player);
            newRole.RoleUpdate(this);

            if (CustomPlayer.Local.Is(RoleEnum.Seer))
                Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RevealButton.Update("REVEAL", RevealTimer(), CustomGameOptions.RevealCooldown);

            if (ConvertedDead && !IsDead)
            {
                CallRpc(CustomRPC.Change, TurnRPC.TurnSeer, this);
                TurnSeer();
            }
        }

        public void Reveal()
        {
            if (RevealTimer() != 0f || IsTooFar(Player, RevealButton.TargetPlayer))
                return;

            var interact = Interact(Player, RevealButton.TargetPlayer);

            if (interact[3])
            {
                if ((!RevealButton.TargetPlayer.Is(SubFaction) && SubFaction != SubFaction.None && !RevealButton.TargetPlayer.Is(RoleAlignment.NeutralNeo)) ||
                    RevealButton.TargetPlayer.IsFramed())
                {
                    Flash(new(255, 0, 0, 255));
                }
                else
                    Flash(new(0, 255, 0, 255));
            }

            if (interact[0])
                LastRevealed = DateTime.UtcNow;
            else if (interact[1])
                LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception(PlayerControl player) => player.Is(SubFaction) && SubFaction != SubFaction.None;
    }
}