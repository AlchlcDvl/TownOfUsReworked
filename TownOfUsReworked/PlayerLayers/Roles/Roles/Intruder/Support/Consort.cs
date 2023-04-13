using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Consort : IntruderRole
    {
        public DateTime LastBlock;
        public float TimeRemaining;
        public AbilityButton BlockButton;
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
            BlockMenu = new CustomMenu(Player, new CustomMenu.Select(Click));
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

            if (Player.Data.IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected || MeetingHud.Instance || !BlockTarget.IsBlocked())
                TimeRemaining = 0f;
        }

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBlock;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ConsRoleblockCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
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
    }
}