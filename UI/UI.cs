using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace CustomMinionSlots.UI
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
                var config = ModContent.GetInstance<Config>();
                config.UIPositionX = (int)Left.Pixels;
                config.UIPositionY = (int)Top.Pixels;
                
                // Notify the config has been updated
                config.OnChanged(); // This triggers any dynamic updates from the configuration
            }
        }
    }

    public class UI : UIState
    {
        private DraggableText minionSlotText;

        public override void OnInitialize()
        {
            var config = ModContent.GetInstance<Config>();

            // Fallback values if config is null
            float startX = config?.UIPositionX ?? 100f;
            float startY = config?.UIPositionY ?? 100f;

            // Initialize the draggable UI element
            minionSlotText = new DraggableText("Minion Slots: 0/0");
            minionSlotText.Left.Set(startX, 0f);
            minionSlotText.Top.Set(startY, 0f);

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
}
