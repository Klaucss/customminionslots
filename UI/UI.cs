using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics; // Required for DynamicSpriteFont
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

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
            this.TextColor = textColor;
            this.Font = font;
            this.DragEnabled = true;
            Width.Set(200f, 0f);
            Height.Set(50f, 0f);
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
        }

        public void SetFont(DynamicSpriteFont newFont)
        {
            Font = newFont;
        }

        public void SetUIScale(float scale)
        {
            Scale = scale;
            Width.Set(200f * scale, 0f); // Adjust width based on scale
            Height.Set(50f * scale, 0f); // Adjust height based on scale
            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Vector2 position = GetDimensions().Position();

            // Ensure the font is valid before drawing
            if (Font != null)
            {
                float outlineThickness = 2f; // Thickness of the outline
                Color outlineColor = Color.Black; // Color of the outline

                // Draw the outline (8 directions around the text)
                spriteBatch.DrawString(Font, text, position + new Vector2(-outlineThickness, 0), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(Font, text, position + new Vector2(outlineThickness, 0), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(Font, text, position + new Vector2(0, -outlineThickness), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(Font, text, position + new Vector2(0, outlineThickness), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(Font, text, position + new Vector2(-outlineThickness, -outlineThickness), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(Font, text, position + new Vector2(-outlineThickness, outlineThickness), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(Font, text, position + new Vector2(outlineThickness, -outlineThickness), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(Font, text, position + new Vector2(outlineThickness, outlineThickness), outlineColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

                // Draw the main text
                spriteBatch.DrawString(Font, text, position, TextColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            }
            else
            {
                ModContent.GetInstance<CustomMinionSlots>().Logger.Warn("Font is null, skipping drawing.");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!DragEnabled) return;

            Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);

            if (Main.mouseLeft && !isDragging && ContainsPoint(mousePosition))
            {
                isDragging = true;
                dragOffset = mousePosition - GetDimensions().Position();
            }

            if (Main.mouseLeft && isDragging)
            {
                Left.Set(mousePosition.X - dragOffset.X, 0f);
                Top.Set(mousePosition.Y - dragOffset.Y, 0f);
                Recalculate();
            }

            if (!Main.mouseLeft && isDragging)
            {
                isDragging = false;
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
