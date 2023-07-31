namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Syndicate : Role
    {
        public DateTime LastKilled;
        public CustomButton KillButton;
        public string CommonAbilities => (RoleType is not RoleEnum.Anarchist and not RoleEnum.Sidekick && RoleAlignment != RoleAlignment.SyndicateKill && HoldsDrive ? ("- You can kill " +
            "players directly") : "- You can kill") + (CustomGameOptions.AltImps && ((CustomGameOptions.IntrudersCanSabotage && !IsDead) || (IsDead &&
            CustomGameOptions.GhostsCanSabotage)) ? "\n- You can sabotage the systems to distract the <color=#8CFFFFFF>Crew</color>" : "");
        public bool HoldsDrive => Player == DriveHolder || (CustomGameOptions.GlobalDrive && SyndicateHasChaosDrive);

        public override Color32 Color => Colors.Syndicate;
        public override Faction BaseFaction => Faction.Syndicate;

        protected Syndicate(PlayerControl player) : base(player)
        {
            Faction = Faction.Syndicate;
            FactionColor = Colors.Syndicate;
            Objectives = () => SyndicateWinCon;
            KillButton = new(this, "SyndicateKill", AbilityTypes.Direct, "ActionSecondary", Kill, Exception);
            Player.Data.SetImpostor(true);
        }

        public float KillTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(!HoldsDrive && RoleType == RoleEnum.Anarchist ? CustomGameOptions.AnarchKillCooldown : CustomGameOptions.ChaosDriveKillCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            if (!Local)
                return;

            var team = new List<PlayerControl> { CustomPlayer.Local };

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();
                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (player.Is(Faction) && player != CustomPlayer.Local)
                    team.Add(player);
            }

            if (Player.Is(ObjectifierEnum.Lovers))
                team.Add(Player.GetOtherLover());
            else if (Player.Is(ObjectifierEnum.Rivals))
                team.Add(Player.GetOtherRival());
            else if (Player.Is(ObjectifierEnum.Mafia))
            {
                foreach (var player in CustomPlayer.AllPlayers)
                {
                    if (player != Player && player.Is(ObjectifierEnum.Mafia))
                        team.Add(player);
                }
            }

            __instance.teamToShow = team.SystemToIl2Cpp();
        }

        public void Kill()
        {
            if (IsTooFar(Player, KillButton.TargetPlayer) || KillTimer() != 0f)
                return;

            var interact = Interact(Player, KillButton.TargetPlayer, true);

            if (interact[0] || interact[3])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public bool Exception(PlayerControl player) => (player.Is(Faction) && Faction != Faction.Crew) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
            Player.IsLinkedTo(player);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.ChaosDriveKillCooldown, true, (HoldsDrive && RoleAlignment != RoleAlignment.SyndicateKill) || Type is
                LayerEnum.Anarchist or LayerEnum.Sidekick or LayerEnum.Rebel);
        }
    }
}