using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CustomMinionSlots
{
    public class CustomMinionSlotPlayer : ModPlayer
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
            // Apply minion slot settings when the player enters the world
            ApplyConfigMinionSlotLimit();
        }

        public override void ResetEffects()
        {
            // Ensure the player always has the correct minion slots
            Player.maxMinions = SavedMinionSlotLimit;
        }

        public void ApplyConfigMinionSlotLimit()
        {
            // Get the latest minion slot limit from the config
            var config = ModContent.GetInstance<CustomMinionSlotsConfig>();
            if (config != null)
            {
                SavedMinionSlotLimit = config.MinionSlotLimit;
                Player.maxMinions = SavedMinionSlotLimit;
            }
        }

        public override void PostUpdate()
        {
            var config = ModContent.GetInstance<CustomMinionSlotsConfig>();
            if (config.FasterSummoning)
            {
                foreach (Item item in Player.inventory)
                {
                    // Check if the item is a summoner weapon
                    if (item.DamageType == DamageClass.Summon && item.useTime > 1)
                    {
                        item.useTime = 5; // Reduce use time for faster summoning
                        item.useAnimation = 5; // Sync the animation speed with the use time
                    }
                }
            }
        }
    }
}
