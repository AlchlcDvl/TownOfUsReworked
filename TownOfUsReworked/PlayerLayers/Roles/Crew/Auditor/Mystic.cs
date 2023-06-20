namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mystic : Crew
    {
        public DateTime LastRevealed;
        public static bool ConvertedDead => !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(SubFaction.None));
        public CustomButton RevealButton;

        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            RoleType = RoleEnum.Mystic;
            Color = CustomGameOptions.CustomCrewColors ? Colors.Mystic : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewAudit;
            StartText = () => "You Know When Converts Happen";
            AbilitiesText = () => "- You can investigate players to see if they have been converted\n- Whenever someone has been converted, you will be alerted to it\n- When all converted"
                + " and converters die, you will become a <color=#71368AFF>Seer</color>";
            InspectorResults = InspectorResults.TracksOthers;
            Type = LayerEnum.Mystic;
            RevealButton = new(this, "Reveal", AbilityTypes.Direct, "ActionSecondary", Reveal, Exception);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float RevealTimer()
        {
            var timespan = DateTime.UtcNow - LastRevealed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.RevealCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnSeer()
        {
            var newRole = new Seer(Player);
            newRole.RoleUpdate(this);

            if (CustomPlayer.Local.Is(RoleEnum.Seer) && !IntroCutscene.Instance)
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RevealButton.Update("REVEAL", RevealTimer(), CustomGameOptions.RevealCooldown);

            if (ConvertedDead && !IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnSeer);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TurnSeer();
            }
        }

        public void Reveal()
        {
            if (RevealTimer() != 0f || Utils.IsTooFar(Player, RevealButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, RevealButton.TargetPlayer);

            if (interact[3])
            {
                if ((!RevealButton.TargetPlayer.Is(SubFaction) && SubFaction != SubFaction.None && !RevealButton.TargetPlayer.Is(RoleAlignment.NeutralNeo)) ||
                    RevealButton.TargetPlayer.IsFramed())
                {
                    Utils.Flash(new(255, 0, 0, 255));
                }
                else
                    Utils.Flash(new(0, 255, 0, 255));
            }

            if (interact[0])
                LastRevealed = DateTime.UtcNow;
            else if (interact[1])
                LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception(PlayerControl player) => player.Is(SubFaction) && SubFaction != SubFaction.None;
    }
}