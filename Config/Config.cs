using Terraria.ModLoader.Config;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using CustomMinionSlots.UI;

namespace CustomMinionSlots
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide; // Save setting per player

        [Label("Minion Slot Limit")]
        [Tooltip("Set the limit for the number of minions.")]
        [Range(1, 100)]
        [DefaultValue(5)]
        public int MinionSlotLimit { get; set; }

        [Label("Show Minion Slot UI")]
        [Tooltip("Enable or disable the Minion Slot UI display.")]
        [DefaultValue(true)]
        public bool ShowMinionUI { get; set; }

        [Label("UI Position X")]
        [Tooltip("Set the horizontal position of the Minion Slot UI. Enter a value in pixels.")]
        [Range(0, 1920)]
        [DefaultValue(1238)]
        public int UIPositionX { get; set; }

        [Label("UI Position Y")]
        [Tooltip("Set the vertical position of the Minion Slot UI. Enter a value in pixels.")]
        [Range(0, 1080)]
        [DefaultValue(16)]
        public int UIPositionY { get; set; }

        [Label("Faster Summoning")]
        [Tooltip("Enable faster summoning by reducing the time between summoner weapon uses.")]
        [DefaultValue(false)]
        public bool FasterSummoning { get; set; }

        [Label("Custom Minion Slots UI Color")]
        [Tooltip("Set the color of the Minion Slots UI.")]
        [DefaultValue(typeof(Color), "255, 255, 0, 255")] // Default: Yellow
        public Color MinionUIBaseColor { get; set; }

        [Label("Enable Biome-Based Color")]
        [Tooltip("Change the color of the Minion Slots UI based on the player's current biome.")]
        [DefaultValue(false)]
        public bool EnableBiomeBasedColor { get; set; }

        public override void OnChanged()
        {
            if (UISystem.Instance != null)
            {
                UISystem.Instance.UpdateVisibility();
                UISystem.Instance.ApplyConfigChanges();
            }
        }
    }
}