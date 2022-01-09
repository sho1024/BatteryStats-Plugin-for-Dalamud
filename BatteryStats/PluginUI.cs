using ImGuiNET;
using System;
using System.Numerics;
using System.Windows.Forms;
using Dalamud;
using Dalamud.Interface.Colors;

namespace BatteryStats
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        // private Configuration configuration;
        private PowerStatus p = SystemInformation.PowerStatus;

        private bool visible = true;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        // passing in the image here just for simplicity
        public PluginUI()
        {
            //this.configuration = configuration;
        }

        public void Dispose()
        {
            this.visible = false;
        }

        public void Draw()
        {
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawMainWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }
            BatteryChargeStatus chargeStatus = p.BatteryChargeStatus;

            ImGui.SetNextWindowSize(new Vector2(200, 30), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 35), new Vector2(200, 35));
            if (ImGui.Begin("Power status", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize))
            {
                int percent = (int)(p.BatteryLifePercent * 100);

                int secondsLeft = (int)(p.BatteryLifeRemaining);
                TimeSpan time = TimeSpan.FromSeconds(secondsLeft);
                string timeLeft = time.ToString(@"hh\:mm");
                ImGui.ProgressBar(p.BatteryLifePercent, new Vector2(188, 3), "");

                if (chargeStatus.Equals(BatteryChargeStatus.Critical))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
                    ImGui.Text("Critically low... " + percent.ToString() + @"%%");
                    ImGui.PopStyleColor();
                }

                if (chargeStatus.Equals(BatteryChargeStatus.Low))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
                    ImGui.Text(timeLeft + " " + percent.ToString() + @"%%");
                    ImGui.PopStyleColor();
                }

                if (chargeStatus.Equals(BatteryChargeStatus.Unknown))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedPurple);
                    ImGui.Text("???");
                    ImGui.PopStyleColor();
                }

                if (chargeStatus.Equals(BatteryChargeStatus.NoSystemBattery))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedPurple);
                    ImGui.Text("No battery?");
                    ImGui.PopStyleColor();
                }

                if (chargeStatus.Equals(BatteryChargeStatus.Charging))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                    ImGui.Text("Charging... ");
                    ImGui.PopStyleColor();
                }

                if (chargeStatus.Equals(BatteryChargeStatus.High) && secondsLeft.Equals(-1))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
                    ImGui.Text(percent.ToString() + @"%%");
                    ImGui.PopStyleColor();
                }

                if (chargeStatus.Equals(BatteryChargeStatus.High) && !secondsLeft.Equals(-1))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
                    ImGui.Text(timeLeft + " " + percent.ToString() + @"%%");
                    ImGui.PopStyleColor();
                }

                if (percent.Equals(100))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                    ImGui.Text(percent.ToString() + @"%%");
                    ImGui.PopStyleColor();
                }
            }
            ImGui.End();
        }
    }
}
