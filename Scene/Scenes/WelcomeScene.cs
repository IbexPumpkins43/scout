namespace Scout.Scenes;

internal class WelcomeScene(SceneManagerData data) : Scene(data)
{
    private List<string> _items = new();
    private int _itemsOffset = 0;
    private int _itemSelected = -1;

    public override void Load()
    {
        foreach (string file in Directory.GetFiles(Config.DataPath + "Maps/"))
        {
            int lastFwdSlash = file.LastIndexOf('/') + 1;
            int length = file.Length - lastFwdSlash;
            string basename = file.Substring(lastFwdSlash, length);

            this._items.Add(basename);
        }

        this._items.Add("Finland");
        this._items.Add("Iceland");
        this._items.Add("Norway");
        this._items.Add("Sweden");
        this._items.Add("Denmark");
        this._items.Add("Germany");
        this._items.Add("United Kingdom");
        this._items.Add("France");
        this._items.Add("Ireland");
        this._items.Add("Russia");
        this._items.Add("China");
        this._items.Add("United States");
        this._items.Add("Finland");
        this._items.Add("Iceland");
        this._items.Add("Norway");
        this._items.Add("Sweden");
        this._items.Add("Denmark");
        this._items.Add("Germany");
        this._items.Add("United Kingdom");
        this._items.Add("France");
        this._items.Add("Ireland");
        this._items.Add("Russia");
        this._items.Add("China");
        this._items.Add("Finland");
        this._items.Add("Iceland");
        this._items.Add("Norway");
        this._items.Add("Sweden");
        this._items.Add("Denmark");
        this._items.Add("Germany");
        this._items.Add("United Kingdom");
        this._items.Add("France");
        this._items.Add("Ireland");
        this._items.Add("Russia");
        this._items.Add("China");
    }

    public override void Update() {}

    public override void Render()
    {
        this.UIManager.BeginFrame();
        this.UIManager.Label("Maps");
        this._itemSelected = this.UIManager.List(this._items, ref this._itemsOffset, this._itemSelected);
        if (this._itemSelected != -1)
        {
            this.UIManager.SameLine = true;
            this.UIManager.LabelButton("Load");
            this.UIManager.LabelButton("Delete");
            this.UIManager.ProgressBar(100, 100, "test progress bar!");
            this.UIManager.SameLine = false;
        }
        this.UIManager.EndFrame();
    }
}
