using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using UnityEngine;
using HarmonyLib;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch]
    public class PlayerLayer
    {
        public Color32 Color = Colors.Layer;
        public string Name = "Layerless";

        public PlayerLayerEnum LayerType = PlayerLayerEnum.None;
        public RoleEnum RoleType = RoleEnum.None;
        public ModifierEnum ModifierType = ModifierEnum.None;
        public ObjectifierEnum ObjectifierType = ObjectifierEnum.None;
        public AbilityEnum AbilityType = AbilityEnum.None;

        public string KilledBy = "";
        public DeathReasonEnum DeathReason = DeathReasonEnum.Alive;

        public bool IsBlocked;
        public bool RoleBlockImmune;

        public bool IsDead => Player.Data.IsDead;

        public readonly static List<PlayerLayer> Layers = new();

        public virtual void OnLobby() {}

        public virtual void Effect() {}

        public virtual void UpdateHud(HudManager __instance) {}

        public virtual void ButtonClick(AbilityButton __instance, out bool clicked) => clicked = true;

        protected PlayerLayer(PlayerControl player)
        {
            Player = player;
            PlayerName = player.Data.PlayerName;
            Layers.Add(this);
        }

        public string PlayerName;
        public PlayerControl Player;

        public int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        public int TasksCompleted => Player.Data.Tasks.ToArray().Count(x => x.Complete);
        public int TotalTasks => Player.Data.Tasks.ToArray().Length;
        public bool TasksDone => TasksLeft <= 0 || TasksCompleted >= TotalTasks;

        public string ColorString => $"<color=#{Color.ToHtmlStringRGBA()}>";

        public virtual bool GameEnd(LogicGameFlowNormal __instance) => true;

        public static List<PlayerLayer> GetLayers(PlayerControl player) => Layers.Where(x => x.Player == player).ToList();
    }
}