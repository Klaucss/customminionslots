using Terraria.ModLoader.Config;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using CustomMinionSlots.UI;

namespace CustomMinionSlots
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // General Settings Group
        [Header("$Mods.CustomMinionSlots.Configs.Headers.GeneralSettings")]

        [Label("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.MinionSlotLimit.Label")]
        [Tooltip("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.MinionSlotLimit.Tooltip")]
        [Range(1, 100)]
        [DefaultValue(5)]
        public int MinionSlotLimit { get; set; }

        [Label("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.ShowMinionUI.Label")]
        [Tooltip("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.ShowMinionUI.Tooltip")]
        [DefaultValue(true)]
        public bool ShowMinionUI { get; set; }

        [Label("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.EnableUIDragging.Label")]
        [Tooltip("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.EnableUIDragging.Tooltip")]
        [DefaultValue(true)]
        public bool EnableUIDragging { get; set; }

        // UI Position Group
        [Header("$Mods.CustomMinionSlots.Configs.Headers.UIPosition")]

        [Label("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.UIPositionX.Label")]
        [Tooltip("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.UIPositionX.Tooltip")]
        [Range(0, 1920)]
        [DefaultValue(500)]
        public int UIPositionX { get; set; }

        [Label("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.UIPositionY.Label")]
        [Tooltip("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.UIPositionY.Tooltip")]
        [Range(0, 1080)]
        [DefaultValue(50)]
        public int UIPositionY { get; set; }

        [Label("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.UIScale.Label")]
        [Tooltip("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.UIScale.Tooltip")]
        [Range(0.5f, 3.0f)]
        [DefaultValue(1.1f)]
        public float UIScale { get; set; }

        [Label("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.ResetUIScaleFlag.Label")]
        [Tooltip("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.ResetUIScaleFlag.Tooltip")]
        [DefaultValue(false)]
        public bool ResetUIScaleFlag
        {
            get => false;
            set
            {
                if (value)
                {
                    UIScale = 1.1f;

                    if (UISystem.Instance?.CustomUI != null)
                    {
                        UISystem.Instance.CustomUI.minionSlotText.SetUIScale(UIScale);
                    }
                }
            }
        }

        // Appearance Settings Group
        [Header("$Mods.CustomMinionSlots.Configs.Headers.AppearanceSettings")]

        [Label("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.MinionUIBaseColor.Label")]
        [Tooltip("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.MinionUIBaseColor.Tooltip")]
        [DefaultValue(typeof(Color), "255, 255, 0, 255")]
        public Color MinionUIBaseColor { get; set; }

        [Label("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.EnableBiomeBasedColor.Label")]
        [Tooltip("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.EnableBiomeBasedColor.Tooltip")]
        [DefaultValue(true)]
        public bool EnableBiomeBasedColor { get; set; }

        // Gameplay Enhancements Group
        [Header("$Mods.CustomMinionSlots.Configs.Headers.GameplayEnhancements")]

        [Label("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.FasterSummoning.Label")]
        [Tooltip("$Mods.CustomMinionSlots.Configs.CustomMinionSlotsConfig.FasterSummoning.Tooltip")]
        [DefaultValue(false)]
        public bool FasterSummoning { get; set; }

        public override void OnChanged()
        {
            if (UISystem.Instance == null) return;

            UISystem.Instance.UpdateVisibility();
            UISystem.Instance.ApplyConfigChanges();

            if (UISystem.Instance.CustomUI != null)
            {
                UISystem.Instance.CustomUI.minionSlotText.SetPosition(UIPositionX, UIPositionY);
                UISystem.Instance.CustomUI.minionSlotText.SetUIScale(UIScale);
            }
        }
    }
}
