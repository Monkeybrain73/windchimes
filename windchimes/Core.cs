[assembly: ModInfo("windchimes",
                    Authors = new string[] { "xXx_Ape_xXx" },
                    Description = "Add decorative whind chimes",
                    Version = "1.4.0")]

namespace windchimes
{
    public class Core : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            if (api.Side == EnumAppSide.Server)
            {
                Configs.TryLoadServerConfig(api as ICoreServerAPI);
            }
            if (api.Side == EnumAppSide.Client)
            {
                Configs.TryLoadClientConfig(api as ICoreClientAPI);
            }

            api.RegisterBlockClass("WindChime", typeof(WindChime));

            api.Logger.Event("[WindChimes] mod started");

        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
        }

        public static class DebugUtil
        {
            public static void Log(ICoreAPI api, string message, params object[] args)
            {
                if (Configs.SConfig.EnableDebugLogging)
                {
                    api.World.Logger.Debug("[WindChimes] " + message, args);
                }
            }

            public static void Verbose(ICoreAPI api, string message, params object[] args)
            {
                if (Configs.SConfig.EnableDebugLogging)
                {
                    api.World.Logger.VerboseDebug("[WindChimes] " + message, args);
                }
            }

            public static void Error(ICoreAPI api, string message, params object[] args)
            {
                api.World.Logger.Error("[WindChimes] " + message, args);
            }

            /// <summary>
            /// Logs all public config values from any object (e.g. ServerConfig, ClientConfig)
            /// </summary>
            public static void LogConfig(ICoreAPI api, string title, object config)
            {
                if (config == null)
                {
                    Verbose(api, $"{title}: (null)");
                    return;
                }

                var type = config.GetType();
                var sb = new StringBuilder();
                sb.Append($"{title}: ");

                var properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    object value = prop.GetValue(config);

                    if (value is Array arr)
                    {
                        sb.AppendFormat("{0}=[{1}]; ", prop.Name, string.Join(", ", arr.Cast<object>()));
                    }
                    else
                    {
                        sb.AppendFormat("{0}={1}; ", prop.Name, value);
                    }
                }

                Verbose(api, sb.ToString());
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
