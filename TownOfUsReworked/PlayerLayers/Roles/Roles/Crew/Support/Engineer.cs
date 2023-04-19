using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using System;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Functions;
using System.Linq;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Engineer : CrewRole
    {
        public CustomButton FixButton;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public DateTime LastFixed;

        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            StartText = "Just Fix It";
            AbilitiesText = "- You can fix sabotages at any time during the game\n- You can vent";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Engineer : Colors.Crew;
            RoleType = RoleEnum.Engineer;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            InspectorResults = InspectorResults.DifferentLens;
            UsesLeft = CustomGameOptions.MaxFixes;
            Type = LayerEnum.Engineer;
            FixButton = new(this, AssetManager.Fix, AbilityTypes.Effect, "ActionSecondary", Fix, true);
        }

        public float FixTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFixed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FixCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Fix()
        {
            if (!ButtonUsable || FixTimer() != 0f)
                return;

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

            if (system == null)
                return;

            var dummyActive = system.dummy.IsActive;
            var sabActive = system.specials.ToArray().Any(s => s.IsActive);

            if (!sabActive || dummyActive)
                return;

            UsesLeft--;
            LastFixed = DateTime.UtcNow;

            switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
            {
                case 1:
                    var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

                    if (comms2.IsActive)
                        FixFunctions.FixMiraComms();

                    var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor2.IsActive)
                        FixFunctions.FixReactor(SystemTypes.Reactor);

                    var oxygen2 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (oxygen2.IsActive)
                        FixFunctions.FixOxygen();

                    var lights2 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights2.IsActive)
                        FixFunctions.FixLights(lights2);

                    break;

                case 2:
                    var comms3 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms3.IsActive)
                        FixFunctions.FixComms();

                    var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (seismic.IsActive)
                        FixFunctions.FixReactor(SystemTypes.Laboratory);

                    var lights3 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights3.IsActive)
                        FixFunctions.FixLights(lights3);

                    break;

                case 0:
                case 3:
                    var comms1 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms1.IsActive)
                        FixFunctions.FixComms();

                    var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor1.IsActive)
                        FixFunctions.FixReactor(SystemTypes.Reactor);

                    var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (oxygen1.IsActive)
                        FixFunctions.FixOxygen();

                    var lights1 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights1.IsActive)
                        FixFunctions.FixLights(lights1);

                    break;

                case 4:
                    var comms4 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms4.IsActive)
                        FixFunctions.FixComms();

                    var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                    if (reactor.IsActive)
                        FixFunctions.FixAirshipReactor();

                    var lights4 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights4.IsActive)
                        FixFunctions.FixLights(lights4);

                    break;

                case 5:
                    if (!SubmergedCompatibility.Loaded)
                        break;

                    var reactor5 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor5.IsActive)
                        FixFunctions.FixReactor(SystemTypes.Reactor);

                    var lights5 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights5.IsActive)
                        FixFunctions.FixLights(lights5);

                    var comms5 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms5.IsActive)
                        FixFunctions.FixComms();

                    foreach (var i in PlayerControl.LocalPlayer.myTasks)
                    {
                        if (i.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                            FixFunctions.FixSubOxygen();
                    }

                    break;

                case 6:
                    var comms6 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms6.IsActive)
                        FixFunctions.FixComms();

                    var reactor6 = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactor6.IsActive)
                        FixFunctions.FixReactor(SystemTypes.Laboratory);

                    var oxygen6 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (oxygen6.IsActive)
                        FixFunctions.FixOxygen();

                    var lights6 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights6.IsActive)
                        FixFunctions.FixLights(lights6);

                    break;
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = system?.dummy.IsActive;
            var active = system?.specials.ToArray().Any(s => s.IsActive);
            var condition = active == true && dummyActive == false;
            FixButton.Update("FIX", FixTimer(), CustomGameOptions.FixCooldown, UsesLeft, condition && ButtonUsable, ButtonUsable);
        }
    }
}