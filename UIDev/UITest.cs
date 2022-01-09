using ImGuiNET;
using ImGuiScene;
using System.Numerics;
using System.Windows.Forms;
//using Dalamud.Interface.Colors;


namespace UIDev
{
    class UITest : IPluginUIMock
    {
        public static void Main(string[] args)
        {
            UIBootstrap.Inititalize(new UITest());
        }

        private SimpleImGuiScene? scene;

        public void Initialize(SimpleImGuiScene scene)
        {
            // scene is a little different from what you have access to in dalamud
            // but it can accomplish the same things, and is really only used for initial setup here

            // eg, to load an image resource for use with ImGui 
            //this.goatImage = scene.LoadImage("goat.png");

            scene.OnBuildUI += Draw;

            this.Visible = true;

            // saving this only so we can kill the test application by closing the window
            // (instead of just by hitting escape)
            this.scene = scene;
        }

        public void Dispose()
        {
            //this.goatImage?.Dispose();
        }

        // You COULD go all out here and make your UI generic and work on interfaces etc, and then
        // mock dependencies and conceivably use exactly the same class in this testbed and the actual plugin
        // That is, however, a bit excessive in general - it could easily be done for this sample, but I
        // don't want to imply that is easy or the best way to go usually, so it's not done here either
        private void Draw()
        {
            DrawMainWindow();

            if (!Visible)
            {
                this.scene!.ShouldQuit = true;
            }
        }

        #region Nearly a copy/paste of PluginUI
        // private Configuration configuration;

        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }
            PowerStatus p = SystemInformation.PowerStatus;
            BatteryChargeStatus chargeStatus = p.BatteryChargeStatus;

            ImGui.SetNextWindowSize(new Vector2(200, 30), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 35), new Vector2(200, 35));
            if (ImGui.Begin("Power status", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize ))
            {   
                int percent = (int)(p.BatteryLifePercent * 100);

                int secondsLeft = (int)(p.BatteryLifeRemaining);
                System.TimeSpan time = System.TimeSpan.FromSeconds(secondsLeft);
                string timeLeft = time.ToString(@"hh\:mm");
                ImGui.ProgressBar(p.BatteryLifePercent, new Vector2(188, 3), "");

                if (!chargeStatus.Equals(BatteryChargeStatus.Charging))
                {
                    if (secondsLeft.Equals(-1))
                    {
                        ImGui.Text("Calculating time left..." + percent.ToString() + @"%%");
                    }
                    else
                    {
                        ImGui.Text(timeLeft + " " + percent.ToString() + @"%%");
                    }
                }
                else
                {
                    ImGui.Text("Charging... " + percent.ToString() + @"%%");
                }
            }
            ImGui.End();
        }
        #endregion
    }
}
