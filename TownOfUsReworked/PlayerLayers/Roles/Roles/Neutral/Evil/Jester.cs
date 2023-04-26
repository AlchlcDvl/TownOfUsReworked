using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Custom;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Jester : NeutralRole
    {
        public bool VotedOut;
        public List<byte> ToHaunt = new();
        public bool HasHaunted;
        public CustomButton HauntButton;
        public DateTime LastHaunted;
        public int UsesLeft;
        public bool CanHaunt => VotedOut && !HasHaunted && UsesLeft > 0 && ToHaunt.Count > 0;

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            StartText = "It Was Jest A Prank Bro";
            AbilitiesText = "- After you get ejected, you can haunt a those who voted for you";
            Objectives = "- Get ejected";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jester : Colors.Neutral;
            RoleType = RoleEnum.Jester;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            ToHaunt = new();
            UsesLeft = CustomGameOptions.HauntCount <= ToHaunt.Count ? CustomGameOptions.HauntCount : ToHaunt.Count;
            Type = LayerEnum.Jester;
            HauntButton = new(this, "Haunt", AbilityTypes.Direct, "ActionSecondary", Haunt, true);
        }

        public void SetHaunted(MeetingHud __instance)
        {
            if (!VotedOut)
                return;

            ToHaunt.Clear();

            foreach (var state in __instance.playerStates)
            {
                if (state.AmDead || Utils.PlayerById(state.TargetPlayerId).Data.Disconnected || state.VotedFor != Player.PlayerId || state.TargetPlayerId == Player.PlayerId)
                    continue;

                ToHaunt.Add(state.TargetPlayerId);
            }

            while (ToHaunt.Count > CustomGameOptions.HauntCount)
            {
                ToHaunt.Shuffle();
                ToHaunt.Remove(ToHaunt[^1]);
            }
        }

        public float HauntTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastHaunted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.HauntCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var ToBeHaunted = PlayerControl.AllPlayerControls.ToArray().Where(x => ToHaunt.Contains(x.PlayerId) && !(x.GetSubFaction() == SubFaction && SubFaction !=
                SubFaction.None)).ToList();
            HauntButton.Update("HAUNT", HauntTimer(), CustomGameOptions.HauntCooldown, UsesLeft, ToBeHaunted, CanHaunt, CanHaunt);
        }

        public void Haunt()
        {
            if (Utils.IsTooFar(Player, HauntButton.TargetPlayer) || HauntTimer() != 0f || !CanHaunt)
                return;

            Utils.RpcMurderPlayer(Player, HauntButton.TargetPlayer, DeathReasonEnum.Haunted, false);
            HasHaunted = true;
            UsesLeft--;
            LastHaunted = DateTime.UtcNow;
        }
    }
}