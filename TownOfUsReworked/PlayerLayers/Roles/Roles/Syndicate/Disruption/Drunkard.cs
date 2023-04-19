using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Functions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Custom;
using Hazel;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Drunkard : SyndicateRole
    {
        public CustomButton ConfuseButton;
        public bool Enabled;
        public float TimeRemaining;
        public DateTime LastConfused;
        public bool Confused => TimeRemaining > 0f;
        public CustomMenu ConfuseMenu;
        public PlayerControl ConfusedPlayer;

        public Drunkard(PlayerControl player) : base(player)
        {
            Name = "Drunkard";
            StartText = "Confuse The <color=#8BFDFDFF>Crew</color>";
            AbilitiesText = $"- You can invert player's controls for a short while\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Drunkard : Colors.Syndicate;
            LastConfused = DateTime.UtcNow;
            RoleType = RoleEnum.Drunkard;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
            ConfuseMenu = new(Player, Click);
            ConfusedPlayer = null;
            Type = LayerEnum.Drunkard;
            ConfuseButton = new(this, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary", Drunk);
        }

        public float DrunkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastConfused;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FreezeCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Confuse()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void Unconfuse()
        {
            Enabled = false;
            LastConfused = DateTime.UtcNow;

            if (HoldsDrive)
                Reverse.UnconfuseAll();
            else
                Reverse.UnconfuseSingle(ConfusedPlayer);
        }

        public void Click(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ConfusedPlayer = player;
            else if (interact[0])
                LastConfused = DateTime.UtcNow;
            else if (interact[1])
                LastConfused.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Drunk()
        {
            if (DrunkTimer() != 0f || Confused)
                return;

            if (HoldsDrive)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Confuse);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConfuseDuration;
                Confuse();
                Reverse.ConfuseAll();
            }
            else if (ConfusedPlayer == null)
                ConfuseMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != ConfusedPlayer).ToList());
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Confuse);
                writer.Write(Player.PlayerId);
                writer.Write(ConfusedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConfuseDuration;
                Confuse();
                Reverse.ConfuseSingle(ConfusedPlayer);
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag = ConfusedPlayer == null && !HoldsDrive;
            ConfuseButton.Update(flag ? "SET TARGET" : "CONFUSE", DrunkTimer(), CustomGameOptions.ConfuseCooldown, Confused, TimeRemaining, CustomGameOptions.ConfuseDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (ConfusedPlayer != null && !HoldsDrive && !Confused)
                    ConfusedPlayer = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}