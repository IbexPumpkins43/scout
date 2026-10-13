namespace Scout.Ui.Elements;

internal class ListItem : LabelButton
{
    public event Action<string>? OnItemSelected;

    public ListItem(string name) : base(name)
    {
        this.DesiredSizeMode = UiDesiredSizeMode.FillParent;
        this.LabelAlign = LabelButtonAlign.Left;
        this.Border = false;

        this.OnClick += () => this.OnItemSelected?.Invoke(this.Label.Text);
    }

    public override void Measure()
    {
        // Call the LabelButton measure
        base.Measure();

        // Override the width value so that it spans the full width of the list
        this.DesiredSize = new(
            UiLength.Flexible,
            this.Label.MeasureText.Y + Style.Padding.InnerPx * 2);
    }
}

internal class List : Container
{
    public ListItem[] Items;

    public event Action<string>? OnItemSelected;

    public List(ListItem[] items)
    {
        // Ensure each item is able to invoke the public handler for a selected item
        this.Items = items;
        Array.ForEach(
            this.Items,
            (item) => item.OnItemSelected += (itemName) => this.OnItemSelected?.Invoke(itemName));

        // Configure the container
        this.Orientation = ContainerOrientation.Vertical;
        this.BackgroundColour = UiManager.ColourScheme.Background;
        this.Children = this.Items;
    }

    public override void Measure()
    {
        // Menu occupies only as much horizontal space as its button
        //this.DesiredSize = new(
        //    this.OpenButton.DesiredSize.Width,
        //    UiLength.Flexible);

        // Calculate height from the menu items
        float height = this.Items.Sum(item => item.DesiredSize.Height.Value);

        this.DesiredSize = new(300.0f, height);
    }
}
