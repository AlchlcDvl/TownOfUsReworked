using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Snitch : Ability
    {
        public List<ArrowBehaviour> ImpArrows = new();
        public Dictionary<byte, ArrowBehaviour> SnitchArrows = new();

        public Snitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            TaskText = "- You can finish your tasks to get information on who's evil";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Snitch : Colors.Ability;
            AbilityType = AbilityEnum.Snitch;
            Hidden = !CustomGameOptions.SnitchKnows && !TasksDone;
            ImpArrows = new();
            SnitchArrows = new();
            Type = LayerEnum.Snitch;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = SnitchArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);

            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);

            SnitchArrows.Remove(arrow.Key);
        }

        public override void OnLobby()
        {
            SnitchArrows.Values.DestroyAll();
            SnitchArrows.Clear();
            ImpArrows.DestroyAll();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (PlayerControl.LocalPlayer.Data.IsDead || Player.Data.IsDead)
            {
                SnitchArrows.Values.DestroyAll();
                SnitchArrows.Clear();
                ImpArrows.DestroyAll();
                ImpArrows.Clear();
            }

            foreach (var arrow in ImpArrows)
                arrow.target = Player.transform.position;

            foreach (var arrow in SnitchArrows)
            {
                var player = Utils.PlayerById(arrow.Key);

                if (player?.Data.IsDead == true || player?.Data.Disconnected == true)
                    DestroyArrow(arrow.Key);
                else
                    arrow.Value.target = player.transform.position;
            }
        }
    }
}