using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public abstract class IntruderRole : Role
    {
        public DateTime LastKilled;
        public CustomButton KillButton;

        protected IntruderRole(PlayerControl player) : base(player)
        {
            Faction = Faction.Intruder;
            FactionColor = Colors.Intruder;
            Color = Colors.Intruder;
            Objectives = IntrudersWinCon;
            BaseFaction = Faction.Intruder;
            AbilitiesText = "- You can kill players\n- You can call sabotages to distract the <color=#8BFDFDFF>Crew</color>";
            KillButton = new(this, "IntruderKill", AbilityTypes.Direct, "ActionSecondary", Kill, Exception);
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.IntKillCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;

            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);

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

            __instance.teamToShow = team;
        }

        public void Kill()
        {
            if (Utils.IsTooFar(Player, KillButton.TargetPlayer) || KillTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, KillButton.TargetPlayer, true);

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

        public bool Exception(PlayerControl player) => player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || player == Player.GetOtherLover() || player ==
            Player.GetOtherRival() || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.IntKillCooldown);
        }
    }
}