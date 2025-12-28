using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Metadata.Ecma335;

namespace Iso.Engine.Core.UI.Elements
{
    public enum TextJustificationMode
    {
        LEFT,
        CENTER,
        RIGHT,
    }

    public enum VerticalAlignmentMode
    {
        TOP,
        CENTER,
        BOTTOM,
    }

    public class TextBlock : UIElement
    {
        public string text = "Text Block";
        public TextJustificationMode justification = TextJustificationMode.LEFT;
        public VerticalAlignmentMode verticalAlignment = VerticalAlignmentMode.CENTER;
        public SpriteFont font = Fonts.arial;
        public float fontScale = 1.0f;
        public Color tint = Color.White;

        public TextBlock()
        {
            visibility = UIVisibilityMode.VISIBLE_NO_HIT_TEST;
        }

        public TextBlock SetText(string text)
        {
            this.text = text;
            return this;
        }

        public TextBlock SetJustification(TextJustificationMode newJustification)
        {
            justification = newJustification;
            return this;
        }

        public TextBlock SetVerticalAlignment(VerticalAlignmentMode newVA)
        {
            verticalAlignment = newVA;
            return this;
        }

        public TextBlock SetFont(SpriteFont newFont)
        {
            font = newFont;
            return this;
        }

        public TextBlock SetFontScale(float newScale)
        {
            fontScale = newScale;
            return this;
        }

        public TextBlock SetTint(Color newTint)
        {
            tint = newTint;
            return this;
        }

        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            Vector2 textPosition = new Vector2(0, 0);
            Vector2 stringSize = font.MeasureString(text) * fontScale;

            switch (justification)
            {
                case TextJustificationMode.LEFT:
                    textPosition.X = actualRect.X;
                    break;

                case TextJustificationMode.RIGHT:
                    textPosition.X = actualRect.Right - stringSize.X;
                    break;

                case TextJustificationMode.CENTER:
                    textPosition.X = actualRect.X + (actualRect.Width / 2.0f) - (stringSize.X / 2.0f);
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignmentMode.TOP:
                    textPosition.Y = actualRect.Y;
                    break;

                case VerticalAlignmentMode.CENTER:
                    textPosition.Y = actualRect.Y + (actualRect.Height / 2.0f) - (stringSize.Y / 2.0f);
                    break;

                case VerticalAlignmentMode.BOTTOM:
                    textPosition.Y = actualRect.Y + actualRect.Height - stringSize.Y;
                    break;
            }

            sb.DrawString(font, text, textPosition, tint, 0.0f, Vector2.Zero, fontScale, SpriteEffects.None, 0.0f);
        }
    }
}
