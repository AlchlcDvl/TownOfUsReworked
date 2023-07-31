namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Intruder : Role
    {
        public DateTime LastKilled;
        public CustomButton KillButton;
        public string CommonAbilities => "- You can kill players" + (CustomGameOptions.IntrudersCanSabotage || (IsDead && CustomGameOptions.GhostsCanSabotage) ? ("\n- You can call " +
            "sabotages to distract the <color=#8CFFFFFF>Crew</color>") : "");

        public override Color32 Color => Colors.Intruder;
        public override Faction BaseFaction => Faction.Intruder;

        protected Intruder(PlayerControl player) : base(player)
        {
            Faction = Faction.Intruder;
            FactionColor = Colors.Intruder;
            Objectives = () => IntrudersWinCon;
            KillButton = new(this, "IntruderKill", AbilityTypes.Direct, "ActionSecondary", Kill, Exception);
            Player.Data.SetImpostor(true);
        }

        public float KillTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.IntKillCooldown) * 1000f;
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

            if (Player.Is(RoleEnum.Janitor))
            {
                var jani = (Janitor)this;

                if (interact[3] || interact[0])
                {
                    if (CustomGameOptions.JaniCooldownsLinked)
                        jani.LastCleaned = DateTime.UtcNow;
                }
                else if (interact[1])
                {
                    if (CustomGameOptions.JaniCooldownsLinked)
                        jani.LastCleaned.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
                else if (interact[2])
                {
                    if (CustomGameOptions.JaniCooldownsLinked)
                        jani.LastCleaned.AddSeconds(CustomGameOptions.VestKCReset);
                }
            }
            else if (Player.Is(RoleEnum.PromotedGodfather))
            {
                var gf = (PromotedGodfather)this;

                if (interact[3] || interact[0])
                {
                    if (CustomGameOptions.JaniCooldownsLinked && gf.FormerRole?.RoleType == RoleEnum.Janitor)
                        gf.LastCleaned = DateTime.UtcNow;
                }
                else if (interact[1])
                {
                    if (CustomGameOptions.JaniCooldownsLinked && gf.FormerRole?.RoleType == RoleEnum.Janitor)
                        gf.LastCleaned.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
                else if (interact[2])
                {
                    if (CustomGameOptions.JaniCooldownsLinked && gf.FormerRole?.RoleType == RoleEnum.Janitor)
                        gf.LastCleaned.AddSeconds(CustomGameOptions.VestKCReset);
                }
            }

            if (interact[3] || interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public bool Exception(PlayerControl player) =>  (player.Is(Faction) && Faction != Faction.Crew) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
            Player.IsLinkedTo(player);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.IntKillCooldown);
        }
    }
}