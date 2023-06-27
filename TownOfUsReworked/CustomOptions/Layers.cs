namespace TownOfUsReworked.CustomOptions
{
    public class CustomLayersOption : CustomOption
    {
        public string Layer;
        public List<CustomOption> RoleOptions = new();
        public static readonly List<CustomOption> AllRoleOptions = new();

        public CustomLayersOption(int id, MultiMenu menu, string name, string layer) : base(id, menu, name, CustomOptionType.Layers, 0) => Layer = layer;

        public override void OptionCreated()
        {
            base.OptionCreated();
            var role = Setting.Cast<RoleOptionSetting>();
            role.TitleText.text = Name;
            role.RoleMaxCount = 15;
            role.ChanceText.text = "0%";
            role.CountText.text = "0";
        }

        public override string ToString() => GetChance().ToString() + "% + " + GetCount().ToString();

        public void AddOptions(params CustomOption[] list)
        {
            RoleOptions = list.ToList();
            AllRoleOptions.AddRange(RoleOptions);
        }

        public void ShowAdvancedOptions()
        {
            ShowRoleDetails();
            var tab = GameSettingMenu.Instance.RolesSettings.AllAdvancedSettingTabs[0];
            GameSettingMenu.Instance.RolesSettings.RoleChancesSettings.SetActive(false);
            GameSettingMenu.Instance.RolesSettings.AdvancedRolesSettings.SetActive(true);
            GameSettingMenu.Instance.RolesSettings.RefreshChildren();
            ControllerManager.Instance.CurrentUiState.BackButton = GameSettingMenu.Instance.AdvancedSettingsBackButton;
        }

        public void CloseAdvancedSettings()
        {
            ShowRoleDetails();
            GameSettingMenu.Instance.RolesSettings.CloseAdvancedSettings();
            ControllerManager.Instance.CurrentUiState.BackButton = GameSettingMenu.Instance.BackButton;
        }

        public int GetChance() => int.Parse(Setting.Cast<RoleOptionSetting>().ChanceText.text.Replace("%", ""));

        public int GetCount() => int.Parse(Setting.Cast<RoleOptionSetting>().CountText.text);

        public void IncreaseCount()
        {
            var count = GetCount();

            if (count + 1 > 15)
                count = 0;
            else
                count++;

            Setting.Cast<RoleOptionSetting>().CountText.text = count.ToString();
            ShowRoleDetails();
        }

        public void DecreaseCount()
        {
            var count = GetCount();

            if (count - 1 < 0)
                count = 15;
            else
                count--;

            Setting.Cast<RoleOptionSetting>().CountText.text = count.ToString();
            ShowRoleDetails();
        }

        public void IncreaseChance()
        {
            var chance = GetChance();
            var increment = Input.GetKeyDown(KeyCode.LeftShift) ? 5 : 10;

            if (chance + increment > 100)
                chance = 0;
            else
                chance += increment;

            Setting.Cast<RoleOptionSetting>().ChanceText.text = chance.ToString() + "%";

            if (GetCount() <= 0 && GetChance() > 0)
                Setting.Cast<RoleOptionSetting>().CountText.text = "1";

            ShowRoleDetails();
        }

        public void DecreaseChance()
        {
            var chance = GetChance();
            var increment = Input.GetKeyDown(KeyCode.LeftShift) ? 5 : 10;

            if (chance - increment < 0)
                chance = 100;
            else
                chance -= increment;

            Setting.Cast<RoleOptionSetting>().ChanceText.text = chance.ToString() + "%";

            if (GetCount() > 0 && GetChance() <= 0)
                Setting.Cast<RoleOptionSetting>().CountText.text = "0";

            ShowRoleDetails();
        }

        public void ShowRoleDetails()
        {
            if (!Info.AllInfo.Any(x => x.Name == Layer || x.Short == Layer))
                return;

            var layer = Info.AllInfo.Find(x => x.Name == Layer || x.Short == Layer);
            GameSettingMenu.Instance.RoleName.text = layer.Name;
            GameSettingMenu.Instance.RoleBlurb.text = layer.Description;
            GameSettingMenu.Instance.RoleIcon.sprite = AssetManager.GetSprite(Layer == layer.Short ? layer.Name : Layer);
        }
    }
}