namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Jackal : NeutralRole
    {
        public PlayerControl EvilRecruit;
        public PlayerControl GoodRecruit;
        public PlayerControl BackupRecruit;
        public CustomButton RecruitButton;
        public bool HasRecruited;
        public bool RecruitsDead => (EvilRecruit == null || GoodRecruit == null || ((EvilRecruit?.Data.IsDead == true || EvilRecruit.Data.Disconnected) &&
            (GoodRecruit?.Data.Disconnected == true || GoodRecruit.Data.IsDead))) && BackupRecruit == null;
        public DateTime LastRecruited;
        public List<byte> Recruited = new();

        public Jackal(PlayerControl player) : base(player)
        {
            Name = "Jackal";
            RoleType = RoleEnum.Jackal;
            StartText = "Gain A Majority";
            AbilitiesText = "- You start off with 2 recruits. 1 of them is always <color=#8CFFFFFF>Crew</color>\nand the other is either a <color=#008000FF>Syndicate</color>, " +
                "<color=#FF0000FF>Intruder</color> or a <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killer</color>\n- When both recruits die, you can recruit a third member into" +
                " the <color=#575657FF>Cabal</color>";
            Objectives = "- Recruit or kill anyone who can oppose the <color=#575657FF>Cabal</color>";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jackal : Colors.Neutral;
            SubFaction = SubFaction.Cabal;
            SubFactionColor = Colors.Cabal;
            RoleAlignment = RoleAlignment.NeutralNeo;
            Recruited = new() { Player.PlayerId };
            Type = LayerEnum.Jackal;
            RecruitButton = new(this, "Recruit", AbilityTypes.Direct, "ActionSecondary", Recruit, Exception);
            InspectorResults = InspectorResults.BringsChaos;
        }

        public float RecruitTimer()
        {
            var timespan = DateTime.UtcNow - LastRecruited;
            var num = Player.GetModifiedCooldown(CustomGameOptions.RecruitCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Recruit()
        {
            if (RecruitTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, RecruitButton.TargetPlayer, false, true);

            if (interact[3])
            {
                RoleGen.Convert(RecruitButton.TargetPlayer.PlayerId, Player.PlayerId, SubFaction.Cabal, false);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Convert);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(RecruitButton.TargetPlayer.PlayerId);
                writer.Write((byte)SubFaction.Cabal);
                writer.Write(false);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
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