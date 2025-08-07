using System.Text.Json;
using System.Text.Json.Nodes;
using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public partial class EventViewModel : ObservableObject
{
    private EnvelopeItem? _event;
    [ObservableProperty] private JsonObject? _tags;
    [ObservableProperty] private JsonObject? _contexts;
    [ObservableProperty] private JsonObject? _extra;
    [ObservableProperty] private JsonObject? _sdk;

    public EventViewModel(EnvelopeService service, IOptions<AppConfig> config)
    {
        if (!string.IsNullOrEmpty(config.Value?.FilePath))
        {
            Task.Run(async () => Event = (await service.LoadAsync(config.Value.FilePath))?.TryGetEvent());
        }
    }

    public EnvelopeItem? Event
    {
        get => _event;
        private set
        {
            SetProperty(ref _event, value);
            var payload = value?.TryGetJsonPayload() as JsonObject;
            Tags = payload?.TryGetPropertyValue("tags", out var tags) == true
                ? tags?.AsObject()
                : null;
            Contexts = payload?.TryGetPropertyValue("contexts", out var contexts) == true
                ? contexts?.AsFlatObject()
                : null;
            Extra = payload?.TryGetPropertyValue("extra", out var extra) == true
                ? extra?.AsFlatObject()
                : null;
            Sdk = payload?.TryGetPropertyValue("sdk", out var sdk) == true
                ? sdk?.AsFlatObject()
                : null;
        }
    }
}

internal static partial class JsonExtensions
{
    public static JsonObject AsFlatObject(this JsonNode source)
    {
        var nodes = new Dictionary<string, JsonNode?>();

        Flatten(source, "");

        var result = new JsonObject();
        foreach (var kvp in nodes)
        {
            result[kvp.Key] = kvp.Value?.DeepClone();
        }
        return result;

        void Flatten(JsonNode? node, string prefix)
        {
            switch (node)
            {
                case JsonObject obj:
                    foreach (var (key, value) in obj)
                    {
                        var newKey = string.IsNullOrEmpty(prefix) ? key : $"{prefix}.{key}";
                        Flatten(value, newKey);
                    }
                    break;

                case JsonArray array:
                    if (array.Any(v => v is JsonObject or JsonArray))
                    {
                        for (var i = 0; i < array.Count; i++)
                        {
                            var newKey = $"{prefix}[{i}]";
                            Flatten(array[i], newKey);
                        }
                    } else {
                        var joined = string.Join(", ", array.Select(n => n.FormatNode()));
                        nodes[prefix] = JsonValue.Create("[" + joined + "]");
                    }
                    break;

                case JsonValue val:
                    nodes[prefix] = val;
                    break;

                case null:
                    nodes[prefix] = null;
                    break;

                default:
                    nodes[prefix] = node;
                    break;
            }
        }
    }

    private static object? FormatNode(this JsonNode? node)
    {
        return node switch
        {
            null => "null",
            JsonValue v when v.TryGetValue<bool>(out var b) => b ? "true" : "false",
            JsonValue v when v.TryGetValue<double>(out var d) => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
            JsonValue v when v.TryGetValue<long>(out var l) => l.ToString(),
            JsonValue v when v.TryGetValue<int>(out var i) => i.ToString(),
            JsonValue v when v.TryGetValue<string>(out var s) => s,
            _ => node?.ToJsonString() ?? "null"
        };
    }
}
