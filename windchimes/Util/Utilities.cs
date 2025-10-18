namespace windchimes
{

    #region DebugUtil
    public static class DebugUtil
    {
        public enum LogSide
        {
            Server,
            Client,
            Both
        }

        public static void Log(ICoreAPI api, string message, LogSide side = LogSide.Both, params object[] args)
        {
            if (!Configs.SConfig.EnableDebugLogging && !Configs.CConfig.EnableDebugLogging) return;

            if ((side == LogSide.Server || side == LogSide.Both) && api.Side == EnumAppSide.Server && Configs.SConfig.EnableDebugLogging)
            {
                api.World.Logger.Debug($"[{Const.AppName}] " + message, args);
            }

            if ((side == LogSide.Client || side == LogSide.Both) && api.Side == EnumAppSide.Client && Configs.CConfig.EnableDebugLogging)
            {
                api.Logger.Debug($"[{Const.AppName}] " + message, args);
            }
        }

        public static void Verbose(ICoreAPI api, string message, LogSide side = LogSide.Both, params object[] args)
        {
            if (!Configs.SConfig.EnableDebugLogging && !Configs.CConfig.EnableDebugLogging) return;

            if ((side == LogSide.Server || side == LogSide.Both) && api.Side == EnumAppSide.Server && Configs.SConfig.EnableDebugLogging)
            {
                api.World.Logger.VerboseDebug($"[{Const.AppName}] " + message, args);
            }

            if ((side == LogSide.Client || side == LogSide.Both) && api.Side == EnumAppSide.Client && Configs.CConfig.EnableDebugLogging)
            {
                api.Logger.VerboseDebug($"[{Const.AppName}] " + message, args);
            }
        }

        public static void Error(ICoreAPI api, string message, LogSide side = LogSide.Both, params object[] args)
        {
            if (!Configs.SConfig.EnableDebugLogging && !Configs.CConfig.EnableDebugLogging) return;

            if ((side == LogSide.Server || side == LogSide.Both) && api.Side == EnumAppSide.Server && Configs.SConfig.EnableDebugLogging)
            {
                api.World.Logger.Error($"[{Const.AppName}] " + message, args);
            }

            if ((side == LogSide.Client || side == LogSide.Both) && api.Side == EnumAppSide.Client && Configs.CConfig.EnableDebugLogging)
            {
                api.Logger.Error($"[{Const.AppName}] " + message, args);
            }
        }

        public static void LogConfig(ICoreAPI api, string title, object config, LogSide side = LogSide.Both)
        {
            if (config == null)
            {
                Verbose(api, $"{title}: (null)", side);
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

            Verbose(api, sb.ToString(), side);
        }
    }
    #endregion

}
