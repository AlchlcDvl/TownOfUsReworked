using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Custom;
using System.Linq;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Consort : IntruderRole
    {
        public DateTime LastBlock;
        public float TimeRemaining;
        public CustomButton BlockButton;
        public PlayerControl BlockTarget;
        public bool Enabled;
        public bool Blocking => TimeRemaining > 0f;
        public CustomMenu BlockMenu;

        public Consort(PlayerControl player) : base(player)
        {
            Name = "Consort";
            RoleType = RoleEnum.Consort;
            StartText = "Roleblock The Crew And Stop Them From Progressing";
            AbilitiesText = "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are immune to blocks\n" +
                $"- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Consort : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
            RoleBlockImmune = true;
            BlockMenu = new(Player, Click);
            Type = LayerEnum.Consort;
            BlockTarget = null;
            BlockButton = new(this, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary", Roleblock);
        }

        public void UnBlock()
        {
            Enabled = false;

            foreach (var layer in GetLayers(BlockTarget))
                layer.IsBlocked = false;

            BlockTarget = null;
            LastBlock = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
        }

        public void Block()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBlock;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ConsRoleblockCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                BlockTarget = player;
            else if (interact[0])
                LastBlock = DateTime.UtcNow;
            else if (interact[1])
                LastBlock.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Roleblock()
        {
            if (RoleblockTimer() != 0f)
                return;

            if (BlockTarget == null)
                BlockMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => x != Player).ToList());
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.ConsRoleblock);
                writer.Write(Player.PlayerId);
                writer.Write(BlockTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                Block();

                foreach (var layer in GetLayers(BlockTarget))
                    layer.IsBlocked = !layer.RoleBlockImmune;
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag = BlockTarget == null;
            BlockButton.Update(flag ? "SET TARGET" : "ROLEBLOCK", RoleblockTimer(), CustomGameOptions.ConsRoleblockCooldown, Blocking, TimeRemaining,
                CustomGameOptions.ConsRoleblockDuration);
        }
    }
}