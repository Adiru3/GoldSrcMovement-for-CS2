using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Admin;

namespace GoldSrcMovement;

public class GoldSrcMovementConfig : BasePluginConfig
{
    [JsonPropertyName("PluginEnabled")]
    public bool PluginEnabled { get; set; } = true;

    [JsonPropertyName("AutoBhopEnabled")]
    public bool AutoBhopEnabled { get; set; } = true;

    [JsonPropertyName("RemoveLandingLag")]
    public bool RemoveLandingLag { get; set; } = true;

    [JsonPropertyName("QuakeAirAcceleration")]
    public bool QuakeAirAcceleration { get; set; } = true;

    [JsonPropertyName("RemoveSpeedCap")]
    public bool RemoveSpeedCap { get; set; } = true;

    [JsonPropertyName("InstantDuck")]
    public bool InstantDuck { get; set; } = true;
}

public class GoldSrcMovementPlugin : BasePlugin, IPluginConfig<GoldSrcMovementConfig>
{
    public override string ModuleName => "GoldSrcMovement";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Adiru (amazingb01) | https://github.com/Adiru3";

    public GoldSrcMovementConfig Config { get; set; } = new GoldSrcMovementConfig();

    public void OnConfigParsed(GoldSrcMovementConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        Console.WriteLine("[GoldSrcMovement] Plugin loaded. Created by Adiru (amazingb01) - https://github.com/Adiru3");

        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
    }

    private void OnMapStart(string mapName)
    {
        // Block 3: StepSize must be executed globally since CS2 removes m_flStepSize from the pawn.
        Server.ExecuteCommand("sv_stepsize 18"); 
    }

    [ConsoleCommand("css_gsrc_toggle", "Toggle GoldSrc Movement on or off")]
    [RequiresPermissions("@css/generic")]
    public void OnToggleCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        Config.PluginEnabled = !Config.PluginEnabled;
        Server.PrintToChatAll($" \x04[GoldSrcMovement]\x01 Plugin is now {(Config.PluginEnabled ? "\x04ENABLED\x01" : "\x02DISABLED\x01")}.");
        
        string configPath = Path.Combine(ModuleDirectory, "GoldSrcMovement.json");
        File.WriteAllText(configPath, JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true }));
    }

    private void OnTick()
    {
        if (!Config.PluginEnabled) return;

        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null || !player.IsValid || !player.PawnIsAlive)
                continue;

            MovementLogic.ApplyBlock1And2(player, Config);
        }
    }
}
