﻿namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Cryomaniac : Neutral
    {
        public CustomButton FreezeButton;
        public CustomButton DouseButton;
        public CustomButton KillButton;
        public List<byte> Doused;
        public bool FreezeUsed;
        public DateTime LastDoused;
        public DateTime LastKilled;
        public bool LastKiller => !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) ||
            x.Is(RoleAlignment.CrewKill) || x.Is(RoleAlignment.CrewAudit) || x.Is(RoleAlignment.NeutralPros) || x.Is(RoleAlignment.NeutralNeo) || (x.Is(RoleAlignment.NeutralKill) && x !=
            Player))) && CustomGameOptions.CryoLastKillerBoost;
        public int DousedAlive => Doused.Count(x => !PlayerById(x).Data.IsDead && !PlayerById(x).Data.Disconnected);

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Cryomaniac : Colors.Neutral;
        public override string Name => "Cryomaniac";
        public override LayerEnum Type => LayerEnum.Cryomaniac;
        public override RoleEnum RoleType => RoleEnum.Cryomaniac;
        public override Func<string> StartText => () => "Who Likes Ice Cream?";
        public override Func<string> AbilitiesText => () => "- You can douse players in coolant\n- Doused players can be frozen, which kills all of them at once at the start of the next " +
            $"meeting\n- People who interact with you will also get doused{(LastKiller ? "\n- You can kill normally" : "")}";
        public override InspectorResults InspectorResults => InspectorResults.SeeksToDestroy;

        public Cryomaniac(PlayerControl player) : base(player)
        {
            Objectives = () => "- Freeze anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralKill;
            Doused = new();
            DouseButton = new(this, "CryoDouse", AbilityTypes.Direct, "ActionSecondary", Douse, Exception);
            FreezeButton = new(this, "Freeze", AbilityTypes.Effect, "Secondary", Freeze);
            KillButton = new(this, "CryoKill", AbilityTypes.Direct, "Tertiary", Kill, Exception);
        }

        public float DouseTimer()
        {
            var timespan = DateTime.UtcNow - LastDoused;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CryoDouseCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public float KillTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DouseCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
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

        public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
        {
            if (!source.Is(RoleType) || Doused.Contains(target.PlayerId) || source != Player)
                return;

            Doused.Add(target.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.FreezeDouse, this, target);
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            foreach (var cryo in GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
            {
                if (cryo.FreezeUsed)
                {
                    foreach (var player in cryo.Doused)
                    {
                        var player2 = PlayerById(player);

                        if (player2.Data.IsDead || player2.Data.Disconnected || player2.Is(RoleEnum.Pestilence) || player2.IsProtected())
                            continue;

                        RpcMurderPlayer(Player, player2, DeathReasonEnum.Frozen);
                    }

                    cryo.Doused.Clear();
                    cryo.FreezeUsed = false;
                }
            }
        }

        public bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction
            is Faction.Intruder or Faction.Syndicate) || Player.IsLinkedTo(player);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            DouseButton.Update("DOUSE", DouseTimer(), CustomGameOptions.DouseCd);
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.DouseCd, true, LastKiller);
            FreezeButton.Update("FREEZE", true, DousedAlive > 0 && !FreezeUsed);
        }

        public void Douse()
        {
            if (IsTooFar(Player, DouseButton.TargetPlayer) || DouseTimer() != 0f || Doused.Contains(DouseButton.TargetPlayer.PlayerId))
                return;

            var interact = Interact(Player, DouseButton.TargetPlayer, LastKiller);

            if (interact[3])
                RpcSpreadDouse(Player, DouseButton.TargetPlayer);

            if (interact[0])
                LastDoused = DateTime.UtcNow;
            else if (interact[1])
                LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastDoused.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public void Freeze()
        {
            if (DousedAlive <= 0 || FreezeUsed)
                return;

            FreezeUsed = true;
        }
    }
}
