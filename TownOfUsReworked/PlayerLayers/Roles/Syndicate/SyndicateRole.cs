namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class SyndicateRole : Role
    {
        public DateTime LastKilled;
        public CustomButton KillButton;
        public bool HoldsDrive => Player == DriveHolder || (CustomGameOptions.GlobalDrive && SyndicateHasChaosDrive);

        protected SyndicateRole(PlayerControl player) : base(player)
        {
            Faction = Faction.Syndicate;
            FactionColor = Colors.Syndicate;
            Color = Colors.Syndicate;
            Objectives = SyndicateWinCon;
            BaseFaction = Faction.Syndicate;
            AbilitiesText = (RoleType is not RoleEnum.Anarchist and not RoleEnum.Sidekick && RoleAlignment != RoleAlignment.SyndicateKill ? "- With the Chaos Drive, you can kill " +
                "players directly" : "- You can kill") + (CustomGameOptions.AltImps ? "- You can sabotage the systems to distract the <color=#8CFFFFFF>Crew</color>" : "");
            KillButton = new(this, "SyndicateKill", AbilityTypes.Direct, "ActionSecondary", Kill, Exception);
        }

        public float KillTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(!HoldsDrive && RoleType is RoleEnum.Anarchist ? CustomGameOptions.AnarchKillCooldown : CustomGameOptions.ChaosDriveKillCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;

            var team = new List<PlayerControl> { PlayerControl.LocalPlayer };

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                    team.Add(player);
            }

            if (Player.Is(ObjectifierEnum.Lovers))
                team.Add(Player.GetOtherLover());
            else if (Player.Is(ObjectifierEnum.Rivals))
                team.Add(Player.GetOtherRival());
            else if (Player.Is(ObjectifierEnum.Mafia))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != Player && player.Is(ObjectifierEnum.Mafia))
                        team.Add(player);
                }
            }

            __instance.teamToShow = team.SystemToIl2Cpp();
        }

        public void Kill()
        {
            if (Utils.IsTooFar(Player, KillButton.TargetPlayer) || KillTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, KillButton.TargetPlayer, true);

            if (interact[0] || interact[3])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public bool Exception(PlayerControl player) => (player.Is(Faction) && Faction != Faction.Crew) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || player ==
            Player.GetOtherLover() || player == Player.GetOtherRival() || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.ChaosDriveKillCooldown, true, HoldsDrive || Type is LayerEnum.Anarchist or LayerEnum.Sidekick or LayerEnum.Rebel);
        }
    }
}