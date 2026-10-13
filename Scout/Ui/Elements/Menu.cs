using System.Numerics;
using Raylib_cs;

namespace Scout.Ui.Elements;

internal class MenuItem : LabelButton
{
    public MenuItem(string name, Action onClick) : base(name)
    {
        this.DesiredSizeMode = UiDesiredSizeMode.FillParent;
        this.LabelAlign = LabelButtonAlign.Left;
        this.Border = false;

        this.OnClick += onClick;
    }

    public override void Measure()
    {
        // Call the LabelButton measure
        base.Measure();

        // Override the width value so that it spans the full width of the menu
        this.DesiredSize = new(
            UiLength.Flexible,
            this.Label.MeasureText.Y + Style.Padding.InnerPx * 2);
    }
}

internal class Menu : Container
{
    public LabelButton OpenButton;
    public MenuItem[] Items;
    public FloatingContainer ItemsMenu;

    public Menu(string name, MenuItem[] items)
    {
        // Go through each menu item and make sure it knows its parent
        this.Items = items;

        // The menu that will house the entries
        this.ItemsMenu = new()
        {
            Orientation = ContainerOrientation.Vertical,
            Hidden = true,
            BackgroundColour = Color.Magenta,
            Children = this.Items
        };

        // Button that will open the menu
        this.OpenButton = new(name)
        {
            DesiredSizeMode = UiDesiredSizeMode.FitToContent,
            Border = false
        };
        this.OpenButton.OnClick += () =>
        {
            this.ItemsMenu.Hidden = !this.ItemsMenu.Hidden;
            this.InvalidateLayout();
        };

        // Configure floating container
        this.Orientation = ContainerOrientation.Horizontal;
        this.BackgroundColour = UiManager.ColourScheme.Background;
        this.Children =
        [
            this.OpenButton,
            this.ItemsMenu
        ];
    }

    public override void Initialise()
    {
        this.Root.OnMouseDown += (_, _, _) =>
        {
            //this.ItemsMenu.Hidden = true;
            //this.InvalidateLayout();
        };
    }

    public override void Measure()
    {
        // Menu occupies only as much horizontal space as its button
        this.DesiredSize = new(
            this.OpenButton.DesiredSize.Width,
            UiLength.Flexible);

        // Calculate height from the menu items
        float height = this.Items.Sum(item => item.DesiredSize.Height.Value);

        this.ItemsMenu.DesiredSize = new(300.0f, height);
    }

    public override void UpdateLayout()
    {
        // Position the floating menu directly below the button
        Vector2 position = new(
            this.OpenButton.Bounds.X - this.Bounds.X,
            this.OpenButton.Bounds.Y + this.OpenButton.Bounds.Height - this.Bounds.Y);

        if (this.ItemsMenu.Position != position)
        {
            this.ItemsMenu.Position = position;

            // Recalculate because the floating position has changed
            this.CalculateBounds();
        }
    }
}
