namespace windchimes
{
    public class Configs
    {
        public static ServerConfig SConfig { get; set; } = new();
        public static ClientConfig CConfig { get; set; } = new();

        #region ServerConfig
        public class ServerConfig
        {
            public static ServerConfig Loaded { get; set; } = new ServerConfig();
            public bool EnableDebugLogging { get; set; } = false;
        }

        public static void TryLoadServerConfig(ICoreServerAPI api)
        {
            // bool isLoaded = api.ModLoader.IsModEnabled("configlib");
            // if (isLoaded) return;

            try
            {
                SConfig = api.LoadModConfig<ServerConfig>(Const.ConfigNameServer);
                if (SConfig == null)
                {
                    SConfig = new ServerConfig();
                    api.Logger.VerboseDebug($"[{Const.AppName}] Config file not found, creating a new one...");
                }
                api.StoreModConfig(SConfig, Const.ConfigNameServer);
                api.Logger.Event($"[{Const.AppName}] Server Config loaded");

                DebugUtil.LogConfig(api, "Loaded server config values", Configs.SConfig, DebugUtil.LogSide.Server);
            }
            catch (Exception ex)
            {
                api.Logger.Error($"[{Const.AppName}] Failed to load config, you probably made a typo:");
                api.Logger.Error(ex);
                SConfig = new ServerConfig();
            }
        }
        #endregion

        #region ClientConfig
        public class ClientConfig
        {
            public static ClientConfig Loaded { get; set; } = new ClientConfig();
            public float WindChimeMainVolumeMultiplier { get; set; } = 1.0f;
            public float BambooChimeVolume { get; set; } = 0.5f;
            public float WoodChimeVolume { get; set; } = 0.5f;
            public float BrassChimeVolume { get; set; } = 0.5f;
            public float CopperChimeVolume { get; set; } = 0.5f;
            public float CrystalChimeVolume { get; set; } = 0.5f;
            public float WindChimeMinVolume { get; set; } = 0.05f;
            public float WindChimeIndoorVolume { get; set; } = 0.1f; // Volume multiplier when player is indoors
            public float WindChimeMaxDistance { get; set; } = 16f; // Max distance at which chimes can be heard
            public float WindChimeFalloffExponent { get; set; } = 2.5f; // Exponent for distance attenuation curve (1.0 = linear, 2.0 = quadratic, etc.)
            // public float WindChimePitchFadeAmount { get; set; } = 0.25f; // Max pitch variation (+/-) based on distance
            // public float WindChimeDistanceFactor { get; set; } = 0.1f; // Volume decreases by this factor per block of distance
            // public float WindChimeObstructionFactor { get; set; } = 0.5f; // Volume decreases by this factor per obstruction (wall, roof) between player and chime
            // public float WindChimeMinWindSpeed { get; set; } = 0.2f; // Minimum wind speed for chimes to make sound
            // public float WindChimeMaxWindSpeed { get; set; } = 1.5f; // Wind speed at which chimes reach max volume
            public bool EnableDebugLogging { get; set; } = false;
        }

        public static void TryLoadClientConfig(ICoreClientAPI api)
        {
            bool isLoaded = api.ModLoader.IsModEnabled("configlib");
            if (isLoaded) return;

            try
            {
                CConfig = api.LoadModConfig<ClientConfig>(Const.ConfigNameClient);
                if (CConfig == null)
                {
                    CConfig = new ClientConfig();
                    api.Logger.VerboseDebug($"[{Const.AppName}] Config file not found, creating a new one...");
                }
                api.StoreModConfig(CConfig, Const.ConfigNameClient);
                api.Logger.Event($"[{Const.AppName}] Client Config loaded");
                DebugUtil.LogConfig(api, "Loaded client config values", Configs.CConfig, DebugUtil.LogSide.Client);
            }
            catch (Exception ex)
            {
                api.Logger.Error($"[{Const.AppName}] Failed to load config, you probably made a typo:");
                api.Logger.Error(ex);
                CConfig = new ClientConfig();
            }
        }
        #endregion

    }
}
