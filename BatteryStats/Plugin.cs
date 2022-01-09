using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System;
using System.IO;
using System.Reflection;

namespace BatteryStats
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Battery Stats";

        private const string commandName = "/batterystats";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        private Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
            this.PluginUi = new PluginUI();

            this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Display battery informations using /batterystats"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawUI;
        }

        public void Dispose()
        {
            this.PluginInterface.UiBuilder.Draw -= DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi -= DrawUI;
            this.PluginUi.Dispose();
            this.CommandManager.RemoveHandler(commandName);
        }

        private void OnCommand(string command, string args)
        {
            this.PluginUi.Visible = !this.PluginUi.Visible;
        }

        private void DrawUI()
        {
            this.PluginUi.Draw();
        }
    }
}
