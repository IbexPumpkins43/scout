using System.Numerics;
using Raylib_cs;

namespace Scout.UI;

internal partial class UIManager
{
    // TODO : Continue implementing this
    public bool InputBox(ref string text)
    {
        Rectangle box = new(
            x: this._xOffset,
            y: this._yOffset,
            width: 350 + Style.InnerPadding * 2,
            height: Style.RegularFontSize + Style.InnerPadding * 2);

        Vector2 mousePos = Raylib.GetMousePosition();
        if (Raylib.CheckCollisionPointRec(mousePos, box))
        {
            int key = Raylib.GetCharPressed();
            if (key != 0)
            {
                text += (char)key;
            }
            else
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Backspace)
                    || Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace)
                    && text.Length > 0)
                {
                    text = text.Remove(text.Length - 1);
                }
            }
        }

        int textWidth = Raylib.MeasureText(text, Style.RegularFontSize);
        if (textWidth > box.Width)
        {
            // TODO : trim text to fit in the box
        }

        Raylib.BeginTextureMode(this._baseTexture);
        this.DrawBox(box, Style.BorderColour, Style.ButtonBgColour);
        Raylib.DrawText(text, (int)box.X + Style.InnerPadding, (int)box.Y + Style.InnerPadding, Style.RegularFontSize, Style.TextColour);
        Raylib.EndTextureMode();

        return true;
    }

}
