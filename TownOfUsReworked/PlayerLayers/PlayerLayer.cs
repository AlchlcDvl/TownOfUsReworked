using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch]
    public abstract class PlayerLayer
    {
        protected internal Color32 Color = Colors.Layer;
        protected internal string Name = "Layerless";
        protected internal PlayerLayerEnum LayerType = PlayerLayerEnum.None;

        protected internal string KilledBy = "";
        protected internal DeathReasonEnum DeathReason = DeathReasonEnum.Alive;

        public readonly static List<PlayerLayer> Layers = new();

        protected internal virtual void OnLobby() {}

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

        protected internal int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        protected internal int TasksCompleted => Player.Data.Tasks.ToArray().Count(x => x.Complete);
        protected internal int TotalTasks => Player.Data.Tasks.ToArray().Length;
        protected internal bool TasksDone => TasksLeft <= 0 || TasksCompleted >= TotalTasks;

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

        protected internal virtual bool GameEnd(LogicGameFlowNormal __instance) => true;

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