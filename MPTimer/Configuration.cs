using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Numerics;

namespace MPTimer
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool enable { get; set; } = false;
        public bool clickThrough { get; set; } = false;

        public float boundaryTime = 1.2f;

        public bool combatOnly { get; set; } = false;

        public Vector4 leftColor = new Vector4(0.000f, 0.492f, 1.000f, 1.000f);
        public Vector4 rightColor = new Vector4(1.000f, 0.270f, 0.270f, 1.000f);

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private DalamudPluginInterface pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface.SavePluginConfig(this);
        }
    }
}
