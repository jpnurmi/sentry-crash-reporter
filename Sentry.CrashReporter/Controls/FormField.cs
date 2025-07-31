namespace Sentry.CrashReporter.Controls;

public class FormField : Grid
{
    private readonly TextBlock _label = new();
    private readonly TextBox _textBox = new();

    public FormField()
    {
        RowSpacing = 4;

        RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        Children.Add(_label);
        Children.Add(_textBox);

        SetRow(_label, 0);
        SetRow(_textBox, 1);
    }

    public string Title
    {
        get => _label.Text;
        set => _label.Text = value;
    }

    public FormField Text(Action<IDependencyPropertyBuilder<string>> binding)
    {
        _textBox.Text(binding);
        return this;
    }

    public FormField TextBox(Action<TextBox> build)
    {
        build(_textBox);
        return this;
    }
}

public static class FormFieldExtensions
{
    public static FormField Title(this FormField field, string title)
    {
        field.Title = title;
        return field;
    }
}
