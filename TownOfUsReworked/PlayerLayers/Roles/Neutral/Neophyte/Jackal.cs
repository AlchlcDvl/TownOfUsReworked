namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Jackal : Neutral
    {
        public PlayerControl EvilRecruit;
        public PlayerControl GoodRecruit;
        public PlayerControl BackupRecruit;
        public CustomButton RecruitButton;
        public bool HasRecruited;
        public bool RecruitsDead => EvilRecruit == null || GoodRecruit == null || BackupRecruit == null;
        public DateTime LastRecruited;
        public List<byte> Recruited = new();

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Jackal : Colors.Neutral;
        public override string Name => "Jackal";
        public override LayerEnum Type => LayerEnum.Jackal;
        public override RoleEnum RoleType => RoleEnum.Jackal;
        public override Func<string> StartText => () => "Gain A Majority";
        public override Func<string> AbilitiesText => () => "- You start off with 2 recruits. 1 of them is always <color=#8CFFFFFF>Crew</color>\nand the other is either a " +
            "<color=#008000FF>Syndicate</color>, <color=#FF0000FF>Intruder</color> or a <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killer</color>\n- When both recruits die, you can"
            + " recruit a third member into the <color=#575657FF>Cabal</color>";
        public override InspectorResults InspectorResults => InspectorResults.BringsChaos;

        public Jackal(PlayerControl player) : base(player)
        {
            Objectives = () => "- Recruit or kill anyone who can oppose the <color=#575657FF>Cabal</color>";
            SubFaction = SubFaction.Cabal;
            SubFactionColor = Colors.Cabal;
            RoleAlignment = RoleAlignment.NeutralNeo;
            Recruited = new() { Player.PlayerId };
            RecruitButton = new(this, "Recruit", AbilityTypes.Direct, "ActionSecondary", Recruit, Exception);
            SubFactionSymbol = "$";
        }

        public float RecruitTimer()
        {
            var timespan = DateTime.UtcNow - LastRecruited;
            var num = Player.GetModifiedCooldown(CustomGameOptions.RecruitCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Recruit()
        {
            if (RecruitTimer() != 0f)
                return;

            var interact = Interact(Player, RecruitButton.TargetPlayer, false, true);

            if (interact[3])
                RoleGen.RpcConvert(RecruitButton.TargetPlayer.PlayerId, Player.PlayerId, SubFaction.Cabal);
            else if (interact[0])
                LastRecruited = DateTime.UtcNow;
            else if (interact[1])
                LastRecruited.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception(PlayerControl player) => Recruited.Contains(player.PlayerId);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RecruitButton.Update("RECRUIT", RecruitTimer(), CustomGameOptions.RecruitCooldown, true, RecruitsDead);
        }
    }
}