using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using UnityEngine;
using TownOfUsReworked.Classes;
using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch]
    public abstract class PlayerLayer
    {
        public Color32 Color = Colors.Layer;
        public string Name = "Layerless";
        public PlayerLayerEnum LayerType = PlayerLayerEnum.None;

        public string KilledBy = "";
        public DeathReasonEnum DeathReason = DeathReasonEnum.Alive;

        public bool IsBlocked;
        public bool RoleBlockImmune;

        public readonly static List<PlayerLayer> Layers = new();

        public virtual void OnLobby() {}

        public virtual void UpdateHud(HudManager __instance) {}

        public virtual void ButtonClick(AbilityButton __instance, out bool clicked) => clicked = true;

        protected PlayerLayer(PlayerControl player)
        {
            Player = player;
            Layers.Add(this);
        }

        private PlayerControl PlayerB { get; set; }
        public string PlayerName { get; set; }

        public PlayerControl Player
        {
            get => PlayerB;
            set
            {
                if (PlayerB != null)
                    PlayerB.NameText().color = new Color32(255, 255, 255, 255);

                PlayerB = value;
                PlayerName = value.Data.PlayerName;
            }
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)LayerType);

        public int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        public int TasksCompleted => Player.Data.Tasks.ToArray().Count(x => x.Complete);
        public int TotalTasks => Player.Data.Tasks.ToArray().Length;
        public bool TasksDone => TasksLeft <= 0 || TasksCompleted >= TotalTasks;

        public string ColorString => $"<color=#{Color.ToHtmlStringRGBA()}>";

        private bool Equals(PlayerLayer other) => Equals(Player, other.Player);

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(PlayerLayer))
                return false;

            return Equals((PlayerLayer)obj);
        }

        public virtual bool GameEnd(LogicGameFlowNormal __instance) => true;

        public static List<PlayerLayer> GetLayers(PlayerControl player) => Layers.Where(x => x.Player == player).ToList();

        public static bool operator == (PlayerLayer a, PlayerLayer b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Player == b.Player;
        }

        public static bool operator != (PlayerLayer a, PlayerLayer b) => !(a == b);
    }
}