using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace CustomMinionSlots
{
    public class DraggableText : UIElement
    {
        private string text;
        private Vector2 dragOffset; // Offset for calculating the position
        private bool isDragging;   // Whether the UI is currently being dragged

        public DraggableText(string text)
        {
            this.text = text;
            Width.Set(200f, 0f); // Set the draggable area's width
            Height.Set(50f, 0f); // Set the draggable area's height
        }

        public void SetText(string newText)
        {
            text = newText;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Vector2 position = GetDimensions().Position();
            Terraria.Utils.DrawBorderString(spriteBatch, text, position, Color.Yellow);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);

            // Start dragging when the mouse is pressed on the UI element
            if (Main.mouseLeft && !isDragging && ContainsPoint(mousePosition))
            {
                isDragging = true; // Start dragging
                dragOffset = mousePosition - GetDimensions().Position(); // Calculate drag offset
            }

            // Continue dragging even if the mouse leaves the UI bounds
            if (Main.mouseLeft && isDragging)
            {
                Left.Set(mousePosition.X - dragOffset.X, 0f);
                Top.Set(mousePosition.Y - dragOffset.Y, 0f);
                Recalculate(); // Refresh UI position
            }

            // Stop dragging when the mouse button is released
            if (!Main.mouseLeft && isDragging)
            {
                isDragging = false;

                // Save the new position to the config
                var config = ModContent.GetInstance<CustomMinionSlotsConfig>();
                config.UIPositionX = (int)Left.Pixels;
                config.UIPositionY = (int)Top.Pixels;
                ModContent.GetInstance<CustomMinionSlotsConfig>().OnChanged();
            }
        }
    }

     public class CustomMinionSlotsUI : UIState
    {
        private DraggableText minionSlotText;

        public override void OnInitialize()
        {
            var config = ModContent.GetInstance<CustomMinionSlotsConfig>();

            // Initialize the draggable UI element
            minionSlotText = new DraggableText("Minion Slots: 0/0");
            minionSlotText.Left.Set(config.UIPositionX, 0f);
            minionSlotText.Top.Set(config.UIPositionY, 0f);

            Append(minionSlotText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Main.LocalPlayer != null)
            {
                int usedSlots = (int)Main.LocalPlayer.slotsMinions;
                int maxSlots = Main.LocalPlayer.maxMinions;
                minionSlotText.SetText($"Minion Slots: {usedSlots}/{maxSlots}");
            }
        }
    }


     public class CustomMinionSlotsUISystem : ModSystem
    {
        private UserInterface _userInterface;
        private CustomMinionSlotsUI _customUI;

        public static CustomMinionSlotsUISystem Instance { get; private set; }

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
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.gameMenu) return; // Only update UI when in-game
            var config = ModContent.GetInstance<CustomMinionSlotsConfig>();
            if (config != null && config.ShowMinionUI)
            {
                _userInterface?.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            if (Main.gameMenu) return; // Only modify layers when in-game
            var config = ModContent.GetInstance<CustomMinionSlotsConfig>();
            if (config != null && config.ShowMinionUI)
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 2", System.StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                {
                    layers.Insert(index, new LegacyGameInterfaceLayer(
                        "CustomMinionSlots: Minion Slot UI",
                        delegate
                        {
                            _userInterface.Draw(Main.spriteBatch, new GameTime());
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
            var player = Main.LocalPlayer.GetModPlayer<CustomMinionSlotPlayer>();
            player.ApplyConfigMinionSlotLimit(); // Update the player's minion slots dynamically
        }

        public void UpdateVisibility()
        {
            var config = ModContent.GetInstance<CustomMinionSlotsConfig>();
            if (config != null)
            {
                if (!config.ShowMinionUI)
                {
                    _userInterface?.SetState(null); // Hide the UI
                }
                else
                {
                    if (_customUI == null)
                    {
                        _customUI = new CustomMinionSlotsUI(); // Reinitialize the UI if it was null
                        _userInterface.SetState(_customUI);
                    }
                    else
                    {
                        _userInterface?.SetState(_customUI); // Show the UI
                    }
                }
            }
        }
    }
}
