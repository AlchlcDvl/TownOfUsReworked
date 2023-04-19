using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Objects;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Coroner : CrewRole
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new();
        public List<byte> Reported = new();
        public CustomButton CompareButton;
        public DeadBody CurrentTarget;
        public DeadPlayer ReferenceBody;
        public PlayerControl ClosestPlayer;
        public DateTime LastCompared;
        public DateTime LastAutopsied;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public CustomButton AutopsyButton;

        public Coroner(PlayerControl player) : base(player)
        {
            Name = "Coroner";
            StartText = "Examine The Dead For Info";
            AbilitiesText = "- You know when players die and will be notified to as to where their body is for a brief period of time\n- You will get a report when you report a body";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Coroner : Colors.Crew;
            RoleType = RoleEnum.Coroner;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            BodyArrows = new();
            Reported = new();
            InspectorResults = InspectorResults.DealsWithDead;
            UsesLeft = 0;
            Type = LayerEnum.Coroner;
            AutopsyButton = new(this, AssetManager.Placeholder, AbilityTypes.Dead, "ActionSecondary", Autopsy);
            CompareButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", Compare, true);
        }

        public float CompareTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCompared;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CompareCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float AutopsyTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAutopsied;
            var num = Player.GetModifiedCooldown(CustomGameOptions.AutopsyCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);

            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);

            BodyArrows.Remove(arrow.Key);
        }

        public override void OnLobby()
        {
            BodyArrows.Values.DestroyAll();
            BodyArrows.Clear();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            AutopsyButton.Update("AUTOPSY", AutopsyTimer(), CustomGameOptions.AutopsyCooldown);
            CompareButton.Update("COMPARE", CompareTimer(), CustomGameOptions.CompareCooldown, UsesLeft, ButtonUsable, ReferenceBody != null && ButtonUsable);

            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x => Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId && DateTime.UtcNow <
                    y.KillTime.AddSeconds(CustomGameOptions.CoronerArrowDuration)));

                foreach (var bodyArrow in BodyArrows.Keys)
                {
                    if (!validBodies.Any(x => x.ParentId == bodyArrow))
                        DestroyArrow(bodyArrow);
                }

                foreach (var body in validBodies)
                {
                    if (!BodyArrows.ContainsKey(body.ParentId))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        BodyArrows.Add(body.ParentId, arrow);
                    }

                    BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                }
            }
            else if (BodyArrows.Count != 0)
            {
                BodyArrows.Values.DestroyAll();
                BodyArrows.Clear();
            }
        }

        public void Autopsy()
        {
            if (Utils.IsTooFar(Player, CurrentTarget) || AutopsyTimer() != 0f)
                return;

            var playerId = CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);
            Utils.Spread(Player, player);
            var matches = Murder.KilledPlayers.Where(x => x.PlayerId == playerId).ToArray();
            DeadPlayer killed = null;

            if (matches.Length > 0)
                killed = matches[0];

            if (killed == null)
                Utils.Flash(new Color32(255, 0, 0, 255));
            else
            {
                ReferenceBody = killed;
                UsesLeft = CustomGameOptions.CompareLimit;
                LastAutopsied = DateTime.UtcNow;
                Utils.Flash(Color);
            }
        }

        public void Compare()
        {
            if (ReferenceBody == null || Utils.IsTooFar(Player, ClosestPlayer) || CompareTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, ClosestPlayer);

            if (interact[3])
            {
                if (ClosestPlayer.PlayerId == ReferenceBody.KillerId || ClosestPlayer.IsFramed())
                    Utils.Flash(new Color32(255, 0, 0, 255));
                else
                    Utils.Flash(new Color32(0, 255, 0, 255));

                UsesLeft--;
            }

            if (interact[0])
                LastCompared = DateTime.UtcNow;
            else if (interact[1])
                LastCompared.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void OnBodyReport(GameData.PlayerInfo info)
        {
            base.OnBodyReport(info);

            if (info == null || PlayerControl.LocalPlayer != Player)
                return;

            var matches = Murder.KilledPlayers.Where(x => x.PlayerId == info.PlayerId).ToArray();
            DeadPlayer killer = null;

            if (matches.Length > 0)
                killer = matches[0];

            if (killer == null)
                return;

            Reported.Add(info.PlayerId);

            var br = new BodyReport
            {
                Killer = Utils.PlayerById(killer.KillerId),
                Body = Utils.PlayerById(killer.PlayerId),
                KillAge = (float)(DateTime.UtcNow - killer.KillTime).TotalMilliseconds
            };

            var reportMsg = BodyReport.ParseBodyReport(br);

            if (string.IsNullOrWhiteSpace(reportMsg))
                return;

            //Only Coroner can see this
            if (HudManager.Instance)
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reportMsg);
        }
    }
}