using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiNET;
using System.Numerics;

namespace MPTimer
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "MP Timer";

        private const string commandName = "/pmptimer";

        private DalamudPluginInterface pluginInterface;
        private Configuration configuration;
        private MpTimer mpTimer;

        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }
        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            this.configuration = this.pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.configuration.Initialize(this.pluginInterface);


            this.mpTimer = new MpTimer(this.pluginInterface);
            this.mpTimer.enable = this.configuration.enable;
            this.mpTimer.clickThrough = this.configuration.clickThrough;
            this.mpTimer.boundaryTime = this.configuration.boundaryTime;
            this.mpTimer.combatOnly = this.configuration.combatOnly;
            this.mpTimer.leftColor = this.configuration.leftColor;
            this.mpTimer.rightColor = this.configuration.rightColor;


            this.pluginInterface.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open the MP Timer Config window."
            });

            this.pluginInterface.UiBuilder.OnBuildUi += DrawUI; 
            this.pluginInterface.UiBuilder.OnOpenConfigUi += (sender, args) => DrawConfigUI();
        }

        private void DrawConfigUI()
        {
            this.Visible = true;
        }


        private void DrawConfigWindow()
        {
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(375, 330), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("MP Timer Config", ref this.visible))
            {
                var mpTimerEnable = this.configuration.enable;
                if (ImGui.Checkbox("Enable", ref mpTimerEnable))
                {
                    this.mpTimer.enable = mpTimerEnable;
                    this.configuration.enable = mpTimerEnable;
                    this.configuration.Save();
                }

                var mpTimerClickThrough = this.configuration.clickThrough;
                if (ImGui.Checkbox("Click Through", ref mpTimerClickThrough))
                {
                    this.mpTimer.clickThrough = mpTimerClickThrough;
                    this.configuration.clickThrough = mpTimerClickThrough;
                    this.configuration.Save();
                }

                var mpTimerCombatOnly = this.configuration.combatOnly;
                if (ImGui.Checkbox("Combat Only", ref mpTimerCombatOnly))
                {
                    this.mpTimer.combatOnly = mpTimerCombatOnly;
                    this.configuration.combatOnly = mpTimerCombatOnly;
                    this.configuration.Save();
                }

                var mpTimerBoundaryTime = this.configuration.boundaryTime;
                if (ImGui.InputFloat("Boundary Time", ref mpTimerBoundaryTime))
                {
                    this.mpTimer.boundaryTime = mpTimerBoundaryTime;
                    this.configuration.boundaryTime = mpTimerBoundaryTime;
                    this.configuration.Save();
                }

                var leftColor = this.configuration.leftColor;
                if (ImGui.ColorEdit4("Left Colour", ref leftColor))
                {
                    this.mpTimer.leftColor = leftColor;
                    this.configuration.leftColor = leftColor;
                    this.configuration.Save();
                }

                var rightColor = this.configuration.rightColor;
                if (ImGui.ColorEdit4("Right Colour", ref rightColor))
                {
                    this.mpTimer.rightColor = rightColor;
                    this.configuration.rightColor = rightColor;
                    this.configuration.Save();
                }

            }
            ImGui.End();
        }

        public void Dispose()
        {
            this.pluginInterface.CommandManager.RemoveHandler(commandName);
            this.pluginInterface.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            this.Visible = true;
        }

        private void DrawUI()
        {
            this.DrawConfigWindow();
            this.mpTimer.Draw();
        }

    }
}
