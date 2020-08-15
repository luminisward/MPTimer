using Dalamud.Game.ClientState.Structs.JobGauge;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Numerics;

namespace MPTimer
{
    class MpTimer
    {
        private int lastMp = 10000;
        private DalamudPluginInterface pluginInterface;
        private long mpTickTimestamp = 0;


        public bool enable { get; set; } = false;
        public bool clickThrough { get; set; } = false;

        public float boundaryTime;

        public bool combatOnly { get; set; } = false;

        public Vector4 leftColor;
        public Vector4 rightColor;

        public MpTimer(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Draw()
        {
            if (!enable) { return; }

            if (combatOnly)
            {
                if (!pluginInterface.ClientState.Condition[Dalamud.Game.ClientState.ConditionFlag.InCombat]) { return; }
            }


            Dalamud.Game.ClientState.Actors.ActorTable actorTable = pluginInterface.ClientState.Actors;
            if (actorTable[0] is Dalamud.Game.ClientState.Actors.Types.PlayerCharacter pc)
            {
                if (pc.ClassJob.Id != 25) { return; }

                updateTime(pc);
                ImGui.SetNextWindowSizeConstraints(new Vector2(170, 10), new Vector2(float.MaxValue, float.MaxValue));

                var windowFlag = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar;
                if (clickThrough)
                {
                    windowFlag = windowFlag | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMouseInputs;
                }

                if (ImGui.Begin("mp tick bar", windowFlag))
                {
                    var width = 150;
                    var height = 8;
                    var windowPos = ImGui.GetWindowPos();
                    var basePos = new Vector2(windowPos.X + 5, windowPos.Y + 2);

                    var timestamp = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - mpTickTimestamp) % 3000;
                    ImGui.Text(mpTickTimestamp == 0 ? "Uninitialized" : String.Format("{0:N2}", 3 - timestamp / 1000.0));

                    ImGui.GetWindowDrawList().AddRect(new Vector2(basePos.X - 1, basePos.Y - 1), new Vector2(basePos.X + width + 1, basePos.Y + height + 1), ImGui.GetColorU32(new Vector4(0.000f, 0.000f, 0.000f, 1.000f)), 0, ImDrawCornerFlags.None, 1);

                    var boundaryPosX = basePos.X + width * (3 - boundaryTime) / 3;
                    ImGui.GetWindowDrawList().AddRectFilled(basePos, new Vector2(boundaryPosX, basePos.Y + height), ImGui.GetColorU32(leftColor));
                    ImGui.GetWindowDrawList().AddRectFilled(new Vector2(boundaryPosX, basePos.Y), new Vector2(basePos.X + width, basePos.Y + height), ImGui.GetColorU32(rightColor));


                    var cursorPosXOffset = width * timestamp / (float)3000.0;
                    var cursorPosX = basePos.X + cursorPosXOffset;
                    ImGui.GetWindowDrawList().AddRectFilled(new Vector2(cursorPosX, windowPos.Y), new Vector2(cursorPosX + 2, windowPos.Y + 12), ImGui.GetColorU32(new Vector4(1.000f, 1.000f, 1.000f, 1.000f)));


                    ImGui.End();
                }

            }

        }

        private void updateTime(Dalamud.Game.ClientState.Actors.Types.PlayerCharacter pc)
        {
            if (pluginInterface.ClientState.JobGauges.Get<BLMGauge>().InUmbralIce() && pc.CurrentMp - lastMp > 500)
            {
                if (mpTickTimestamp == 0)
                {
                    mpTickTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
                else if ((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - mpTickTimestamp) % 3000 > 100)
                {
                    mpTickTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }

            }
            lastMp = pc.CurrentMp;
        }

    }
}
