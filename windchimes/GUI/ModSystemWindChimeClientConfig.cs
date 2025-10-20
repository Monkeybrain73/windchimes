namespace windchimes
{
    public class ModSystemWindChimeClient : ModSystem
    {
        private ICoreClientAPI capi;
        private GuiDialogWindChimeConfig dialog;

        // public Configs.ClientConfig Settings { get; set; } = new();

        public override void StartClientSide(ICoreClientAPI api)
        {
            capi = api;

            bool isLoaded = api.ModLoader.IsModEnabled("configlib");
            DebugUtil.Verbose(api, $"[{Const.AppName}] {Lang.Get("Checking for ConfigLib mod...")}{(isLoaded ? Lang.Get("Found!") : Lang.Get("Not found!"))}", DebugUtil.LogSide.Client);

            if (isLoaded)
            {
                SubscribeToConfigChange(api);
                Configs.TryLoadClientConfig(api);
                DebugUtil.Verbose(api, $"[{Const.AppName}] {Lang.Get("ConfigLib mod found! Wind Chime default configuration GUI will be disabled.")}", DebugUtil.LogSide.Client);
                return;
            }
            else
            {
                DebugUtil.Verbose(api, $"[{Const.AppName}] {Lang.Get("ConfigLib mod not found! Wind Chime default configuration GUI will be used.")}", DebugUtil.LogSide.Client);

                Configs.TryLoadClientConfig(api);
                api.Input.RegisterHotKey("windchimeconfig", Lang.Get("Open Wind Chime Config"), GlKeys.O, HotkeyType.GUIOrOtherControls, false, true, false);
                api.Input.SetHotKeyHandler("windchimeconfig", OnHotkeyPressed);

                api.ChatCommands
                    .Create("windchimeconfig")
                    .WithDescription(Lang.Get("Open Wind Chime configuration menu"))
                    .HandleWith(_ => { OpenDialog(); return TextCommandResult.Success(); });

                base.StartClientSide(api);
            }

        }

        private bool OnHotkeyPressed(KeyCombination key)
        {
            OpenDialog();
            DebugUtil.Verbose(capi, Lang.Get("Wind Chime config hotkey pressed"), DebugUtil.LogSide.Client);
            return true;
        }


        private void OpenDialog()
        {
            if (dialog == null || !dialog.IsOpened())
            {
                dialog = new GuiDialogWindChimeConfig(capi, Configs.CConfig, Configs.SConfig);
                dialog.TryOpen();
                dialog.Focus();
            }
        }

        private void SubscribeToConfigChange(ICoreAPI api)
        {
            ConfigLibModSystem system = api.ModLoader.GetModSystem<ConfigLibModSystem>();

            system.SettingChanged += (domain, config, setting) =>
            {
                if (domain != "windchimes") return;

                setting.AssignSettingValue(Configs.CConfig);
            };

            system.ConfigsLoaded += () =>
            {
                system.GetConfig("windchimes")?.AssignSettingsValues(Configs.CConfig);
            };
        }

        public override void Dispose()
        {
            if (dialog != null && dialog.IsOpened())
            {
                dialog.TryClose();
                dialog = null;
            }
            base.Dispose();
        }
    }
}
