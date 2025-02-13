using Microsoft.Xna.Framework; // For Color
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CustomMinionSlots
{
    public class CustomMinionSlotsPlayer : ModPlayer
    {
        public int SavedMinionSlotLimit = 5; // Default value

        public override void SaveData(TagCompound tag)
        {
            tag["MinionSlotLimit"] = SavedMinionSlotLimit;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("MinionSlotLimit"))
            {
                SavedMinionSlotLimit = tag.GetInt("MinionSlotLimit");
            }
        }

        public override void OnEnterWorld()
        {
            ApplyConfigMinionSlotLimit();
        }

        public override void ResetEffects()
        {
            Player.maxMinions = SavedMinionSlotLimit;
        }

        public void ApplyConfigMinionSlotLimit()
        {
            var config = ModContent.GetInstance<Config>();
            if (config != null)
            {
                SavedMinionSlotLimit = config.MinionSlotLimit;
                Player.maxMinions = SavedMinionSlotLimit;
            }
        }

        public override void PostUpdate()
        {
            var config = ModContent.GetInstance<Config>();
            if (config.FasterSummoning)
            {
                foreach (Item item in Player.inventory)
                {
                    if (item.DamageType == DamageClass.Summon && item.useTime > 1)
                    {
                        item.useTime = 5;
                        item.useAnimation = 5;
                    }
                }
            }
        }

        public Color GetBiomeBasedColor()
        {
            if (Player.ZoneCorrupt) return Color.Purple;
            if (Player.ZoneCrimson) return Color.Red;
            if (Player.ZoneSnow) return Color.LightBlue;
            if (Player.ZoneJungle) return Color.Green;
            if (Player.ZoneDesert) return Color.SandyBrown;
            if (Player.ZoneBeach) return Color.Cyan;
            if (Player.ZoneUnderworldHeight) return Color.Orange;
            if (Player.ZoneSkyHeight) return Color.LightYellow;

            return Color.White;
        }
    }
}
