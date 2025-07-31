namespace Sentry.CrashReporter.Controls;

public class FormField : StackPanel
{
    private readonly TextBlock _label = new();
    private readonly TextBox _textBox = new();

    public FormField()
    {
        Orientation = Orientation.Vertical;
        Spacing = 4;
        Children.Add(_label);
        Children.Add(_textBox);
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
