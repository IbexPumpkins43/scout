using Raylib_cs;

namespace Scout.Ui.Elements;

internal class Form : Container
{
    public event Action<Dictionary<string, string>>? OnSubmit;
    public event Action<Exception>? OnError;

    private const float _sectionHeight = 80.0f;

    private Dictionary<string, InputBox> _fields;
    private LabelButton _submitButton;

    private List<Container> _fieldContainers;
    private Container _buttonsContainer;

    private Container _errorBanner;
    private Label _errorBannerMessage;

    public Form(
        string title,
        (string, InputBox)[] fields,
        string submitButtonText = "Submit",
        LabelButton[]? additionalButtons = null)
    {
        // Build the inputs
        this._fields = fields.ToDictionary();
        this._fieldContainers = new();
        foreach ((string name, InputBox input) in fields)
        {
            this._fieldContainers.Add(
                new()
                {
                    Orientation = ContainerOrientation.Vertical,
                    Border = false,
                    DesiredSize = new(UiLength.Flexible, _sectionHeight),
                    Children =
                    [
                        new Label(name),
                        input
                    ]
                });
        }

        // Build the buttons container
        this._submitButton = new(submitButtonText);
        this._submitButton.OnClick += this.HandleSubmitButton;
        this._buttonsContainer = new()
        {
            Orientation = ContainerOrientation.Horizontal,
            Border = false,
            DesiredSize = new(UiLength.Flexible, _sectionHeight),
            Children =
            [
                this._submitButton,
                ..additionalButtons ?? []
            ]
        };

        // Setup the error banner
        this._errorBannerMessage = new("")
        {
            Colour = Color.White
        };
        this._errorBanner = new()
        {
            Orientation = ContainerOrientation.Horizontal,
            Hidden = true,
            DesiredSize = new(UiLength.Flexible, _sectionHeight),
            BackgroundColour = Color.Red,
            Children = [this._errorBannerMessage]
        };

        // Configure the container
        this.Orientation = ContainerOrientation.Vertical;
        this.Children =
        [
            // Title
            new Container()
            {
                DesiredSize = new(UiLength.Flexible, _sectionHeight),
                Border = false,
                Children =
                [
                    new Label(title)
                    {
                        FontSize = Style.Font.LargeSize,
                    },
                ]
            },
            ..this._fieldContainers,
            this._buttonsContainer,
            this._errorBanner
        ];
    }

    private void HandleSubmitButton()
    {
        try
        {
            this.OnSubmit?.Invoke(
                this._fields.Select((field) => (field.Key, field.Value.Input)).ToDictionary());
        }
        catch (Exception e)
        {
            this._errorBannerMessage.Text = e.Message;
            this._errorBanner.Hidden = false;
            this.OnError?.Invoke(e);
        }
    }
}
