using Terraria;
using Terraria.ModLoader;

namespace CustomMinionSlots
{
    public class System : ModSystem
    {
        private static bool updateRequested = false;

        public override void PostUpdatePlayers()
        {
            if (updateRequested)
            {
                updateRequested = false;

                // Loop through all possible player slots in Terraria
                for (int i = 0; i < Main.maxPlayers; i++) // Terraria supports up to 255 players in theory
                {
                    Player player = Main.player[i];
                    if (player != null && player.active) // Ensure the player is active
                    {
                        player.GetModPlayer<CustomMinionSlotsPlayer>().ApplyConfigMinionSlotLimit();
                    }
                }
            }
        }

        public static void RequestUpdate()
        {
            updateRequested = true;
        }
    }
}
