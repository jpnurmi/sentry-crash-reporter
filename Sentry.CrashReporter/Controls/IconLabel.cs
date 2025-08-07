using Windows.ApplicationModel.DataTransfer;

namespace Sentry.CrashReporter.Controls;

public class IconLabel : StackPanel
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(UIElement), typeof(IconLabel),
            new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(IconLabel),
            new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty ToolTipProperty =
        DependencyProperty.Register(nameof(ToolTip), typeof(string), typeof(IconLabel),
            new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(IconLabel),
            new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty IsTextSelectionEnabledProperty =
        DependencyProperty.Register(nameof(IsTextSelectionEnabled), typeof(bool), typeof(IconLabel),
            new PropertyMetadata(true, OnPropertyChanged));

    public UIElement? Icon
    {
        get => (UIElement?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public Symbol? Symbol
    {
        get => (Symbol?)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    public string? ToolTip
    {
        get => (string?)GetValue(ToolTipProperty);
        set => SetValue(ToolTipProperty, value);
    }

    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsTextSelectionEnabled
    {
        get => (bool)GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    public IconLabel()
    {
        Orientation = Orientation.Horizontal;
        VerticalAlignment = VerticalAlignment.Center;
        Spacing = 8;
        Background = new SolidColorBrush(Colors.Transparent);
        UpdateChildren();
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IconLabel label)
        {
            label.UpdateChildren();
        }
    }

    private void UpdateChildren()
    {
        if (!DispatcherQueue.HasThreadAccess)
        {
            DispatcherQueue.TryEnqueue(UpdateChildren);
            return;
        }

        Children.Clear();

        var icon = Icon;
        if (icon is null && Symbol is {} symbol)
        {
            icon = new SymbolIcon()
                .Symbol(symbol)
                .VerticalAlignment(VerticalAlignment.Center);
        }
        if (icon is not null)
        {
            icon.PointerPressed += (_, _) =>
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(Text ?? string.Empty);
                Clipboard.SetContent(dataPackage);
            };
            Children.Add(icon);
        }

        if (Text != null)
        {
            Children.Add(new SelectableTextBlock()
                .Text(Text)
                .VerticalAlignment(VerticalAlignment.Center)
                .IsTextSelectionEnabled(IsTextSelectionEnabled));
        }

        ToolTipService.SetToolTip(this, ToolTip);
    }
}
