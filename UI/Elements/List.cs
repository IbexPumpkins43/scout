using System.Numerics;
using Raylib_cs;

namespace Scout.UI;

internal partial class UIManager
{
    public int List<T>(List<T> items, ref int listOffset, int currSelected = -1)
    {
        Rectangle listBox = new(
            x: this._xOffset,
            y: this._yOffset,
            width: Style.ListMaxWidth,
            height: Style.ListMaxItemsVisible * Style.RegularFontSize + Style.InnerPadding * 2);

        Raylib.BeginTextureMode(this._baseTexture);
        this.DrawBox(listBox, Style.BorderColour, Style.Bg);
        int newSelected = currSelected;
        int limit = Math.Min(items.Count - listOffset, Style.ListMaxItemsVisible);
        for (int i = listOffset; i < limit; i++)
        {
            if (this.ListItem<T>(items[i], i, newSelected))
            {
                newSelected = i;
            }
        }
        Raylib.EndTextureMode();

        this.UpdateOffsets(listBox.Width, listBox.Height);

        return newSelected;
    }

    public int List<T>(T[] items, ref int listOffset, int currSelected = -1) =>
        List<T>(items.ToList(), ref listOffset, currSelected);

    private bool ListItem<T>(T item, int index, int currSelected)
    {
        Rectangle listItem = new();
        listItem.Width = Style.ListMaxWidth;
        listItem.Height = Style.RegularFontSize + Style.InnerPadding * 2;
        listItem.X = this._xOffset;
        // TODO : Fix this absolute mess
        listItem.Y = this._yOffset + listItem.Height * index - (index > 0 ? listItem.Height * index : 0) + (index > 0 ? Style.InnerPadding * index : 0);

        Color textColour = currSelected == index
            ? Style.ButtonDownTextColour
            : Style.TextColour;
        Color bgColour = currSelected == index
            ? Style.ButtonDownBgColour
            : (index % 2 != 0 && index != 0
                ? Style.ButtonBgAltColour
                : Style.ButtonBgColour);
        Color borderColour = Style.BorderColour;
        bool wasClicked = false;

        Vector2 mouse = Raylib.GetMousePosition();
        if (Raylib.CheckCollisionPointRec(mouse, listItem))
        {
            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                wasClicked = true;
            }
            else if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                borderColour = Style.BorderAltColour;
                bgColour = Style.ButtonDownBgColour;
            }
        }

        Raylib.BeginTextureMode(this._baseTexture);
        this.DrawBox(listItem, borderColour, bgColour);
        this.Label($"{item}", textColour, listItem.X + Style.InnerPadding, listItem.Y + Style.InnerPadding);
        Raylib.EndTextureMode();

        return wasClicked;
    }
}
