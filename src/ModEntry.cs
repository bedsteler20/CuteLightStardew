using CuteLights.Sdk;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CuteLight.GameMods.StardewValley;


internal sealed class ModEntry : Mod {

    private ModConfig? Config;

    private Light[] lights = Array.Empty<Light>();

    public override void Entry(IModHelper helper) {
        Config = Helper.ReadConfig<ModConfig>();

        Monitor.Log("Discovering lights");
        lights = LightDiscoverer.Discover();

        Monitor.Log($"Found {lights.Length} lights");
        foreach (var light in lights) {
            Monitor.Log($"Found light: {light}");
        }

        helper.Events.Player.Warped += OnWarped;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
        Monitor.Log("Game Launched 1");
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) {
            Monitor.Log("Failed to get config menu", LogLevel.Error);
            return;
        }
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config!)
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Enabled",
            getValue: () => Config!.Enabled,
            setValue: value => Config!.Enabled = value
        );

        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => "Inside Color",
            getValue: () => Config!.InsideColor,
            setValue: value => Config!.InsideColor = value
        );

        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => "Desert Color",
            getValue: () => Config!.DesertColor,
            setValue: value => Config!.DesertColor = value
        );

        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => "Spring Color",
            getValue: () => Config!.SpringColor,
            setValue: value => Config!.SpringColor = value
        );

        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => "Summer Color",
            getValue: () => Config!.SummerColor,
            setValue: value => Config!.SummerColor = value
        );

        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => "Fall Color",
            getValue: () => Config!.FallColor,
            setValue: value => Config!.FallColor = value
        );

        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => "Winter Color",
            getValue: () => Config!.WinterColor,
            setValue: value => Config!.WinterColor = value
        );

        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => "Island Color",
            getValue: () => Config!.IslandColor,
            setValue: value => Config!.IslandColor = value
        );

        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => "Rain Color",
            getValue: () => Config!.RainColor,
            setValue: value => Config!.RainColor = value
        );

        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => "Green Rain Color",
            getValue: () => Config!.GreenRainColor,
            setValue: value => Config!.GreenRainColor = value
        );
    }

    private async Task SetColor(LightColor color) {
        Monitor.Log($"Set color to {color}");
        var frame = new Frame();
        frame.SetColorAll(lights, color);
        foreach (var light in lights) {
            await light.SetColor(color);
        }
    }

    private LightColor HexToColor(string hex) {
        var r = Convert.ToByte(hex.Substring(1, 2), 16);
        var g = Convert.ToByte(hex.Substring(3, 2), 16);
        var b = Convert.ToByte(hex.Substring(5, 2), 16);
        return new LightColor(r, g, b);
    }

    private async void OnWarped(object? sender, WarpedEventArgs e) {
        if (!Config!.Enabled) return;

        var loc = e.NewLocation;
        Monitor.Log($"Warped to {loc.Name}");
        if (loc.InDesertContext()) {
            await SetColor(HexToColor(Config!.DesertColor));
        } else if (loc.InIslandContext()) {
            await SetColor(HexToColor(Config!.IslandColor));
        } else if (loc.IsOutdoors) {
            if (loc.IsRainingHere() || loc.IsLightningHere()) {
                await SetColor(HexToColor(Config!.RainColor));
            } else if (loc.IsGreenRainingHere()) {
                await SetColor(HexToColor(Config!.GreenRainColor));
            } else if (loc.IsSpringHere()) {
                await SetColor(HexToColor(Config!.SpringColor));
            } else if (loc.IsSummerHere()) {
                await SetColor(HexToColor(Config!.SummerColor));
            } else if (loc.IsFallHere()) {
                await SetColor(HexToColor(Config!.FallColor));
            } else if (loc.IsWinterHere()) {
                await SetColor(HexToColor(Config!.WinterColor));
            }
        } else {
            await SetColor(HexToColor(Config!.InsideColor));
        }
    }
}
