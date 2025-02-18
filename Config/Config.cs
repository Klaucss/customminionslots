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

        [Range(1, 100)]
        [DefaultValue(5)]
        public int MinionSlotLimit { get; set; }

        [DefaultValue(true)]
        public bool ShowMinionUI { get; set; }

        [DefaultValue(true)]
        public bool EnableUIDragging { get; set; }

        // UI Position Group
        [Header("$Mods.CustomMinionSlots.Configs.Headers.UIPosition")]

        [DefaultValue(1268)]
        public int UIPositionX { get; set; }

        [DefaultValue(15)]
        public int UIPositionY { get; set; }


        [Range(0.5f, 3.0f)]
        [DefaultValue(1.1f)]
        public float UIScale { get; set; }

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

        [DefaultValue(typeof(Color), "255, 255, 0, 255")]
        public Color MinionUIBaseColor { get; set; }

        [DefaultValue(true)]
        public bool EnableBiomeBasedColor { get; set; }

        // Gameplay Enhancements Group
        [Header("$Mods.CustomMinionSlots.Configs.Headers.GameplayEnhancements")]

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
