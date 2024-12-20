namespace ExtraInfo.Configuration;

static class ModConfig
{
    private const string jsonConfig = "ExtraInfo.json";

    public static Config ReadConfig(ICoreAPI api)
    {
        Config config;

        try
        {
            config = LoadConfig(api);

            if (config == null)
            {
                GenerateConfig(api);
                config = LoadConfig(api);
            }
            else
            {
                GenerateConfig(api, config);
            }
        }
        catch
        {
            GenerateConfig(api);
            config = LoadConfig(api);
        }

        return config;
    }

    public static void WriteConfig(ICoreAPI api, Config config) => GenerateConfig(api, config);

    private static Config LoadConfig(ICoreAPI api)
    {
        return api.LoadModConfig<Config>(jsonConfig);
    }

    private static void GenerateConfig(ICoreAPI api)
    {
        api.StoreModConfig(new Config(), jsonConfig);
    }

    private static void GenerateConfig(ICoreAPI api, Config previousConfig)
    {
        api.StoreModConfig(new Config(previousConfig), jsonConfig);
    }
}