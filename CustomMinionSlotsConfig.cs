using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace CustomMinionSlots
{
    public class CustomMinionSlotsConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide; // Save setting per player

        [Label("Minion Slot Limit")]
        [Tooltip("Set the limit for the number of minions.")]
        [Range(1, 100)] // Allows values between 1 and 100
        [DefaultValue(5)]
        public int MinionSlotLimit { get; set; }

        [Label("Show Minion Slot UI")]
        [Tooltip("Enable or disable the Minion Slot UI display.")]
        [DefaultValue(true)]
        public bool ShowMinionUI { get; set; }

        [Label("UI Position X")]
        [Tooltip("Set the horizontal position of the Minion Slot UI. Enter a value in pixels.")]
        [Range(0, 1920)] // Update to the maximum screen width or dynamically adjust
        [DefaultValue(1238)] // Use integer for whole numbers
        public int UIPositionX { get; set; } // Change to int for easier handling of screen pixels

        [Label("UI Position Y")]
        [Tooltip("Set the vertical position of the Minion Slot UI. Enter a value in pixels.")]
        [Range(0, 1080)] // Update to the maximum screen height or dynamically adjust
        [DefaultValue(16)] // Use integer for whole numbers
        public int UIPositionY { get; set; } // Change to int for easier handling of screen pixels

        [Label("Faster Summoning")]
        [Tooltip("Enable faster summoning by reducing the time between summoner weapon uses.")]
        [DefaultValue(false)]
        public bool FasterSummoning { get; set; }

        public override void OnChanged()
        {
            // Notify that the UI visibility or position may have changed
            if (CustomMinionSlotsUISystem.Instance != null)
            {
                CustomMinionSlotsUISystem.Instance.UpdateVisibility();
                CustomMinionSlotsUISystem.Instance?.ApplyConfigChanges();
            }
        }
    }
}
