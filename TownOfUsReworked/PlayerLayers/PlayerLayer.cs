using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using UnityEngine;
using HarmonyLib;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Custom;

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
        public LayerEnum Type = LayerEnum.None;

        public string KilledBy = "";
        public DeathReasonEnum DeathReason = DeathReasonEnum.Alive;

        public bool IsBlocked;
        public bool RoleBlockImmune;

        public bool IsDead => Player.Data.IsDead;

        public readonly static List<PlayerLayer> AllLayers = new();

        public virtual void OnLobby() => EndGame.Reset();

        public virtual void UpdateHud(HudManager __instance)
        {
            __instance.KillButton.SetTarget(null);
            __instance.KillButton.gameObject.SetActive(false);
            __instance.AbilityButton.gameObject.SetActive(false);

            var Vent = __instance.ImpostorVentButton.graphic.sprite;

            if (IsBlocked)
                Vent = AssetManager.Blocked;
            else if (Player.Is(Faction.Intruder))
                Vent = AssetManager.IntruderVent;
            else if (Player.Is(Faction.Syndicate))
                Vent = AssetManager.SyndicateVent;
            else if (Player.Is(Faction.Crew))
                Vent = AssetManager.CrewVent;
            else if (Player.Is(Faction.Neutral))
                Vent = AssetManager.NeutralVent;

            __instance.ImpostorVentButton.graphic.sprite = Vent;
            __instance.ImpostorVentButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "VENT";
            __instance.ImpostorVentButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;
            __instance.ImpostorVentButton.gameObject.SetActive(Player.CanVent() || Player.inVent || (Player.IsPostmortal() && Player.inVent));

            var closestDead = Player.GetClosestDeadPlayer(CustomGameOptions.ReportDistance);

            if (closestDead == null || Player.CannotUse())
                __instance.ReportButton.SetDisabled();
            else
                __instance.ReportButton.SetEnabled();

            __instance.ReportButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "REPORT";
            __instance.ReportButton.graphic.sprite = IsBlocked ? AssetManager.Blocked : AssetManager.Report;
            __instance.ReportButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            __instance.UseButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "USE";
            __instance.UseButton.graphic.sprite = IsBlocked ? AssetManager.Blocked : AssetManager.Use;
            __instance.UseButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            __instance.PetButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "PET";
            __instance.PetButton.graphic.sprite = IsBlocked ? AssetManager.Blocked : AssetManager.Pet;
            __instance.PetButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            if (Player.CannotUse())
                __instance.SabotageButton.SetDisabled();
            else
                __instance.SabotageButton.SetEnabled();

            __instance.SabotageButton.graphic.sprite = IsBlocked ? AssetManager.Blocked : (Player.Is(Faction.Syndicate) ? AssetManager.SyndicateSabotage : AssetManager.Sabotage);

            if (IsBlocked && Minigame.Instance)
                Minigame.Instance.Close();

            if (MapBehaviour.Instance && IsBlocked)
                MapBehaviour.Instance.Close();
        }

        public virtual void OnMeetingStart(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                player.RegenTask();

            EndGame.Reset();
        }

        public virtual void OnBodyReport(GameData.PlayerInfo info) => EndGame.Reset();

        protected PlayerLayer(PlayerControl player)
        {
            Player = player;
            PlayerName = player.Data.PlayerName;
            AllLayers.Add(this);
        }

        public string PlayerName;
        public PlayerControl Player;

        public int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        public int TasksCompleted => Player.Data.Tasks.ToArray().Count(x => x.Complete);
        public int TotalTasks => Player.Data.Tasks.ToArray().Length;
        public bool TasksDone => TasksLeft <= 0 || TasksCompleted >= TotalTasks;

        public string ColorString => $"<color=#{Color.ToHtmlStringRGBA()}>";

        public virtual bool GameEnd(LogicGameFlowNormal __instance) => true;

        public static List<PlayerLayer> GetLayers(PlayerControl player) => AllLayers.Where(x => x.Player == player).ToList();
    }
}