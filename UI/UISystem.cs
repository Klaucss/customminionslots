using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Graphics;
using Terraria.UI;

namespace CustomMinionSlots.UI
{
    public class UISystem : ModSystem
    {
        private UserInterface _userInterface;
        private CustomMinionSlotsUI _customUI;

        public static UISystem Instance { get; private set; }

        public CustomMinionSlotsUI CustomUI => _customUI;

        public override void Load()
        {
            Instance = this; // Assign the instance
            if (!Main.dedServ)
            {
                _customUI = new CustomMinionSlotsUI();
                _userInterface = new UserInterface();
                _userInterface.SetState(_customUI);
            }
        }

        public override void Unload()
        {
            Instance = null; // Clear the instance
            _customUI = null;
            _userInterface = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            var config = ModContent.GetInstance<Config>();
            if (config != null && config.ShowMinionUI)
            {
                _userInterface?.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            var config = ModContent.GetInstance<Config>();
            if (config != null && config.ShowMinionUI)
            {
                int index = layers.FindIndex(layer => layer.Name.ToLower() == "vanilla: mouse text".ToLower());
                if (index != -1)
                {
                    layers.Insert(index, new LegacyGameInterfaceLayer(
                        "CustomMinionSlots: Minion Slot UI",
                        delegate
                        {
                            _userInterface?.Draw(Main.spriteBatch, new GameTime());
                            return true;
                        },
                        InterfaceScaleType.UI)
                    );
                }
            }
        }

        public void ApplyConfigChanges()
        {
            if (Main.gameMenu || Main.LocalPlayer == null) return; // Ensure this only runs in-game
            var player = Main.LocalPlayer.GetModPlayer<CustomMinionSlotsPlayer>();
            player.ApplyConfigMinionSlotLimit(); // Update the player's minion slots dynamically
        }

        public void UpdateVisibility()
        {
            var config = ModContent.GetInstance<Config>();
            if (config != null)
            {
                if (!config.ShowMinionUI)
                {
                    _userInterface?.SetState(null); // Hide the UI
                }
                else
                {
                    _userInterface?.SetState(_customUI); // Show the UI
                }
            }
        }
    }
}
