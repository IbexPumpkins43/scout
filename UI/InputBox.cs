using System.Numerics;
using Raylib_cs;

namespace Scout.UI;

internal partial class UIManager
{
    public bool InputBox(ref string text)
    {
        Rectangle box = new(
            x: _xOffset + Style.OuterPadding,
            y: _yOffset + Style.OuterPadding,
            width: 350 + Style.InnerPadding * 2,
            height: Style.FontSize + Style.InnerPadding * 2);

        Vector2 mousePos = Raylib.GetMousePosition();
        if (Raylib.CheckCollisionPointRec(mousePos, box))
        {   
            int key = Raylib.GetCharPressed();
            if (key != 0)
            {
                text += (char)key;
            }
        }

        int textWidth = Raylib.MeasureText(text, Style.FontSize);
        if (textWidth > box.Width)
        {
            
        }

        Raylib.BeginTextureMode(this._baseTexture);
        this.DrawBox(box, Style.BorderColour, Style.ButtonBgColour);
        Raylib.DrawText(text, (int)box.X + Style.InnerPadding, (int)box.Y + Style.InnerPadding, Style.FontSize, Style.TextColour);
        Raylib.EndTextureMode();

        return true;
    }

}