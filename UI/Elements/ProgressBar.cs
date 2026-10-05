using Raylib_cs;

namespace Scout.UI;

internal partial class UIManager
{
    public void ProgressBar(int status, int maxStatus, string? text = null)
    {
        Rectangle progressBar = new(
            x: this._xOffset,
            y: this._yOffset,
            width: 250,
            height: Style.RegularFontSize + Style.InnerPadding * 2);

        Rectangle progressBarFill = new(
            x: this._xOffset,
            y: this._yOffset,
            width: 250 * ((float)status / (float)maxStatus),
            height: Style.RegularFontSize + Style.InnerPadding * 2);

        Raylib.BeginTextureMode(this._baseTexture);
        Raylib.DrawRectangleRec(progressBar, Style.BorderColour);
        Raylib.DrawRectangleRec(progressBarFill, Style.ButtonDownBgColour);
        this.Label($"{status}/{maxStatus}", Style.TextColour, progressBar.X + progressBar.Width + Style.OuterPadding, progressBar.Y + Style.InnerPadding);
        if (text != null)
        {
            
            this.Label($"{text ?? ""}", Style.TextColour,);
        }
        Raylib.EndTextureMode();
    }
}
