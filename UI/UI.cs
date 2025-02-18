using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics; // Required for DynamicSpriteFont
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using System;

namespace CustomMinionSlots.UI
{
    public class DraggableText : UIElement
    {
        private string text;
        private Vector2 dragOffset;
        private bool isDragging;

        public Color TextColor { get; set; }
        public DynamicSpriteFont Font { get; set; }
        public bool DragEnabled { get; set; }
        public float Scale { get; private set; } = 1.1f;

        public DraggableText(string text, Color textColor, DynamicSpriteFont font)
        {
            this.text = text;
            TextColor = textColor;
            Font = font;
            DragEnabled = true;
            UpdateSize();
        }

        public void SetPosition(float x, float y)
        {
            Left.Set(x, 0f);
            Top.Set(y, 0f);
            Recalculate();
        }

        public void SetText(string newText)
        {
            text = newText;
            UpdateSize();
        }

        public void SetFont(DynamicSpriteFont newFont)
        {
            Font = newFont;
            UpdateSize();
        }

        public void SetUIScale(float scale)
        {
            Scale = scale;
            UpdateSize();
        }

        private void UpdateSize()
        {
            if (Font != null)
            {
                Vector2 textSize = Font.MeasureString(text) * Scale;
                Width.Set(textSize.X, 0f);
                Height.Set(textSize.Y, 0f);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Vector2 position = GetDimensions().Position();

            if (Font != null)
            {
                // Outline settings
                float outlineThickness = 1f;
                Color outlineColor = Color.Black;

                // Create a brighter version of TextColor.
                // Multiply each channel by 1.5 and clamp to 255.
                Color brighter = new Color(
                    (byte)Math.Min(TextColor.R * 1.5f, 255),
                    (byte)Math.Min(TextColor.G * 1.5f, 255),
                    (byte)Math.Min(TextColor.B * 1.5f, 255),
                    TextColor.A);

                // Get total width of the text.
                float totalWidth = Font.MeasureString(text).X * Scale;
                float accumulatedWidth = 0f;
                Vector2 drawPosition = position;

                // Draw each character with a gradient.
                for (int i = 0; i < text.Length; i++)
                {
                    string letter = text[i].ToString();
                    Vector2 letterSize = Font.MeasureString(letter) * Scale;
                    // Calculate the gradient factor (0 at left, 1 at right).
                    float t = totalWidth > 0 ? accumulatedWidth / totalWidth : 0;
                    // Interpolate between brighter (left) and TextColor (right).
                    Color letterColor = Color.Lerp(brighter, TextColor, t);

                    // Draw letter outline (optional).
                    spriteBatch.DrawString(Font, letter, drawPosition + new Vector2(-outlineThickness, 0), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(Font, letter, drawPosition + new Vector2(outlineThickness, 0), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(Font, letter, drawPosition + new Vector2(0, -outlineThickness), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(Font, letter, drawPosition + new Vector2(0, outlineThickness), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

                    // Draw the letter with the gradient color.
                    spriteBatch.DrawString(Font, letter, drawPosition, letterColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

                    // Advance the draw position.
                    accumulatedWidth += letterSize.X;
                    drawPosition.X += letterSize.X;
                }
            }
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!DragEnabled)
                return;

            Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);

            if (Main.mouseLeft && !isDragging && ContainsPoint(mousePosition))
            {
                isDragging = true;
                dragOffset = mousePosition - GetDimensions().Position();
            }

            if (Main.mouseLeft && isDragging)
            {
                // Calculate the new position based on the drag.
                float newX = mousePosition.X - dragOffset.X;
                float newY = mousePosition.Y - dragOffset.Y;

                // Get the dimensions of this UI element.
                CalculatedStyle dims = GetDimensions();

                // Clamp newX and newY so the element stays within the screen bounds.
                newX = MathHelper.Clamp(newX, 0, Main.screenWidth - dims.Width);
                newY = MathHelper.Clamp(newY, 0, Main.screenHeight - dims.Height);

                Left.Set(newX, 0f);
                Top.Set(newY, 0f);
                Recalculate();
            }

            if (!Main.mouseLeft && isDragging)
            {
                isDragging = false;
                // Save the clamped position to config.
                var config = ModContent.GetInstance<Config>();
                config.UIPositionX = (int)Left.Pixels;
                config.UIPositionY = (int)Top.Pixels;
                config.OnChanged();
            }
        }
    }

    public class CustomMinionSlotsUI : UIState
    {
        public DraggableText minionSlotText;

        public override void OnInitialize()
        {
            var config = ModContent.GetInstance<Config>();
            float startX = config?.UIPositionX ?? 100f;
            float startY = config?.UIPositionY ?? 100f;

            minionSlotText = new DraggableText("Minions: 0/0", Color.Yellow, FontAssets.MouseText.Value);
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
                minionSlotText.SetText($"Minions: {usedSlots}/{maxSlots}");

                var config = ModContent.GetInstance<Config>();
                if (config.EnableBiomeBasedColor)
                {
                    minionSlotText.TextColor = GetBiomeColor();
                }
                else
                {
                    minionSlotText.TextColor = config.MinionUIBaseColor;
                    minionSlotText.SetFont(FontAssets.MouseText.Value);
                }

                minionSlotText.DragEnabled = config.EnableUIDragging;
            }
        }

        private Color GetBiomeColor()
        {
            var player = Main.LocalPlayer;

            if (player.ZoneUnderworldHeight) return Color.DarkRed; // Underworld
            if (player.ZoneSnow) return Color.LightBlue; // Snow Biome
            if (player.ZoneJungle) return Color.Green; // Jungle Biome
            if (player.ZoneHallow) return Color.Pink; // Hallow Biome
            if (player.ZoneDesert) return Color.SandyBrown; // Desert Biome
            if (player.ZoneDungeon) return Color.DarkBlue; // Dungeon
            if (player.ZoneBeach) return Color.Cyan; // Beach
            if (player.ZoneCorrupt) return Color.Purple; // Corruption Biome
            if (player.ZoneCrimson) return Color.Red; // Crimson Biome
            if (player.ZoneGlowshroom) return Color.MediumPurple; // Glowing Mushroom Biome
            if (player.ZoneGranite) return Color.Gray; // Granite Biome
            if (player.ZoneMarble) return Color.WhiteSmoke; // Marble Biome
            if (player.ZoneMeteor) return Color.OrangeRed; // Meteor Biome
            if (player.ZoneLihzhardTemple) return Color.Goldenrod; // Lihzahrd Temple
            if (player.ZoneTowerNebula) return Color.Violet; // Nebula Tower
            if (player.ZoneTowerSolar) return Color.Orange; // Solar Tower
            if (player.ZoneTowerStardust) return Color.LightSkyBlue; // Stardust Tower
            if (player.ZoneTowerVortex) return Color.Teal; // Vortex Tower
            if (player.ZoneShimmer) return Color.Lavender; // Shimmer Biome
            if (player.ZoneGemCave) return Color.Turquoise; // Gem Cave
            if (player.ZoneUndergroundDesert) return Color.DarkKhaki; // Underground Desert
            if (player.ZoneRockLayerHeight) return Color.Brown; // Caverns
            if (player.ZoneSkyHeight) return Color.LightYellow; // Sky Biome
            if (player.ZonePeaceCandle) return Color.PaleTurquoise; // Peace Candle
            if (player.ZoneShadowCandle) return Color.DarkSlateBlue; // Shadow Candle
            if (player.ZoneRain) return Color.SlateGray; // Rain Biome
            if (player.ZoneOverworldHeight) return Color.LightGreen; // Overworld
            if (player.ZoneDirtLayerHeight) return Color.SaddleBrown; // Underground
            if (player.ZoneForest) return Color.ForestGreen; // Forest
            if (player.ZoneGraveyard) return Color.DarkSlateGray; // Graveyard

            return Color.White; // Default
        }
    }
}
