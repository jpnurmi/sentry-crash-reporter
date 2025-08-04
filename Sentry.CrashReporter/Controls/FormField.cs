namespace Sentry.CrashReporter.Controls;

public class FormField : Grid
{
    public FormField()
    {
        RowSpacing = 4;
        RowDefinitions.Add("Auto");
        RowDefinitions.Add("*");

        var titleBlock = new TextBlock();
        Children.Add(titleBlock);
        SetRow(titleBlock, 0);

        var textBox = new TextBox();
        Children.Add(textBox);
        SetRow(textBox, 1);

        RegisterPropertyChangedCallback(TextProperty, (s, dp) => textBox.SetValue(TextBox.TextProperty, GetValue(dp)));
        RegisterPropertyChangedCallback(IsEnabledProperty,
            (s, dp) => textBox.SetValue(Control.IsEnabledProperty, GetValue(dp)));
        RegisterPropertyChangedCallback(AcceptsReturnProperty,
            (s, dp) => textBox.SetValue(TextBox.AcceptsReturnProperty, GetValue(dp)));
        RegisterPropertyChangedCallback(TextWrappingProperty,
            (s, dp) => textBox.SetValue(TextBox.TextWrappingProperty, GetValue(dp)));
        RegisterPropertyChangedCallback(TitleProperty,
            (s, dp) => titleBlock.SetValue(TextBlock.TextProperty, GetValue(dp)));
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(FormField),
            new PropertyMetadata(null));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(FormField),
            new PropertyMetadata(null));

    public new bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    public new static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(FormField),
            new PropertyMetadata(true));

    public bool AcceptsReturn
    {
        get => (bool)GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    public static readonly DependencyProperty AcceptsReturnProperty =
        DependencyProperty.Register(nameof(AcceptsReturn), typeof(bool), typeof(FormField),
            new PropertyMetadata(false));

    public TextWrapping TextWrapping
    {
        get => (TextWrapping)GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    public static readonly DependencyProperty TextWrappingProperty =
        DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(FormField),
            new PropertyMetadata(TextWrapping.NoWrap));
}
