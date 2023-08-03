namespace TownOfUsReworked.CustomOptions
{
    public class CustomLayersOption : CustomOption
    {
        public readonly string Layer;
        private int CachedCount { get; set; }
        private int CachedChance { get; set; }
        private readonly int Max;
        private readonly int Min;
        //private List<CustomOption> RoleOptions = new();
        public static readonly List<CustomOption> AllRoleOptions = new();

        public CustomLayersOption(int id, MultiMenu menu, string name, int min = 1, int max = 15, string layer = "") : base(id, menu, name, CustomOptionType.Layers, 0, 0)
        {
            Layer = layer; //This will be used later but we'll see how it goes
            Min = min;
            Max = max;
            Format = (val, otherVal) => $"{val}% (x{otherVal})";
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            var role = Setting.Cast<RoleOptionSetting>();
            role.TitleText.text = Name;
            role.RoleMaxCount = 15;
            role.ChanceText.text = $"{Value}%";
            role.CountText.text = $"{OtherValue}";
            role.Role = null;
            role.RoleChance = 0;
        }

        public void ShowAdvancedOptions()
        {
            /*ShowRoleDetails();
            var tab = GameSettingMenu.Instance.RolesSettings.AllAdvancedSettingTabs[0];
            GameSettingMenu.Instance.RolesSettings.RoleChancesSettings.SetActive(false);
            GameSettingMenu.Instance.RolesSettings.AdvancedRolesSettings.SetActive(true);
            GameSettingMenu.Instance.RolesSettings.RefreshChildren();
            ControllerManager.Instance.CurrentUiState.BackButton = GameSettingMenu.Instance.AdvancedSettingsBackButton;*/
        }

        /*//I'll get to these later
        public void AddOptions(params CustomOption[] list)
        {
            RoleOptions = list.ToList();
            AllRoleOptions.AddRange(RoleOptions);
        }

        public void CloseAdvancedSettings()
        {
            ShowRoleDetails();
            GameSettingMenu.Instance.RolesSettings.CloseAdvancedSettings();
            ControllerManager.Instance.CurrentUiState.BackButton = GameSettingMenu.Instance.BackButton;
        }*/

        public int GetChance() => (int)Value;

        public int GetCount() => (int)OtherValue;

        public void IncreaseCount()
        {
            var chance = GetChance();
            var max = IsCustom ? Max : Min;
            var count = CycleInt(max, 0, GetCount(), true);

            if (chance == 0 && count > 0)
                chance = CachedChance == 0 ? 5 : CachedChance;
            else if (count == 0 && chance > 0)
            {
                CachedChance = chance;
                chance = 0;
            }

            //ShowRoleDetails();
            Set(chance, count);
        }

        public void DecreaseCount()
        {
            var chance = GetChance();
            var max = IsCustom ? Max : Min;
            var count = CycleInt(max, 0, GetCount(), false);

            if (chance == 0 && count > 0)
                chance = CachedChance == 0 ? 5 : CachedChance;
            else if (count == 0 && chance > 0)
            {
                CachedChance = chance;
                chance = 0;
            }

            //ShowRoleDetails();
            Set(chance, count);
        }

        public void IncreaseChance()
        {
            var count = GetCount();
            var increment = Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10;
            var chance = CycleInt(100, 0, GetChance(), true, increment);

            if (chance == 0 && count > 0)
            {
                CachedCount = count;
                count = 0;
            }
            else if (count == 0 && chance > 0)
                count = CachedCount == 0 || !IsCustom ? Min : CachedCount;

            //ShowRoleDetails();
            Set(chance, count);
        }

        public void DecreaseChance()
        {
            var count = GetCount();
            var decrement = Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10;
            var chance = CycleInt(100, 0, GetChance(), false, decrement);

            if (chance == 0 && count > 0)
            {
                CachedCount = count;
                count = 0;
            }
            else if (count == 0 && chance > 0)
                count = CachedCount == 0 || !IsCustom ? Min : CachedCount;

            //ShowRoleDetails();
            Set(chance, count);
        }

        public void ShowRoleDetails()
        {
            //This will be worked on later, when I figure out why the custom options aren't spawning in the Role menu instead
            /*var layer = Info.AllInfo.Find(x => x.Name == Layer || x.Short == Layer);
            
            if (layer == null)
                return;

            GameSettingMenu.Instance.RoleName.text = layer.Name;
            GameSettingMenu.Instance.RoleBlurb.text = layer.Description;
            GameSettingMenu.Instance.RoleIcon.sprite = GetSprite(Layer == layer.Short ? layer.Name : Layer);*/
        }
    }
}