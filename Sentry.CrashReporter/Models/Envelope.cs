using System.Text;
using System.Text.Json;

namespace Sentry.CrashReporter.Models;

public sealed class EnvelopeItem(JsonElement header, byte[] payload)
{
    public JsonElement Header { get; } = header;
    public byte[] Payload { get; } = payload;

    public string? TryGetType()
    {
        return TryGetHeader("type")?.GetString();
    }

    public JsonElement? TryGetHeader(string key)
    {
        return Header.TryGetProperty(key, out var value) ? value : null;
    }

    public JsonElement? TryGetPayload(string? key = null)
    {
        var json = JsonDocument.Parse(Payload).RootElement;
        if (string.IsNullOrEmpty(key))
        {
            return json;
        }

        return json.TryGetProperty(key, out var value) ? value : null;
    }

    internal async Task SerializeAsync(
        StreamWriter writer,
        CancellationToken cancellationToken = default)
    {
        var json = Header.GetRawText();
        await writer.WriteLineAsync(json.AsMemory(), cancellationToken).ConfigureAwait(false);

        var payload = Encoding.UTF8.GetString(Payload);
        await writer.WriteLineAsync(payload.AsMemory(), cancellationToken).ConfigureAwait(false);
    }

    internal static async Task<EnvelopeItem> DeserializeAsync(
        Stream stream, CancellationToken cancellationToken = default)
    {
        var buffer = await stream.ReadLineAsync(cancellationToken).ConfigureAwait(false) ??
                     throw new InvalidOperationException("Envelope item is malformed.");
        var doc = JsonDocument.Parse(buffer) ?? throw new InvalidOperationException("Envelope item is malformed.");

        var header = doc.RootElement;
        if (header.TryGetProperty("length", out var length))
        {
            var payload = new byte[length.GetUInt64()];
            var pos = 0;
            while (pos < payload.Length)
            {
                var read = await stream.ReadAsync(payload, pos, payload.Length - pos, cancellationToken).ConfigureAwait(false);
                if (read == 0)
                {
                    throw new InvalidOperationException("Envelope item payload is malformed.");
                }
                pos += read;
            }

            while (await stream.PeekAsync(cancellationToken) == '\n')
            {
                await stream.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            }

            return new EnvelopeItem(header, payload);
        }
        else
        {
            var payload = await stream.ReadLineAsync(cancellationToken).ConfigureAwait(false) ??
                          throw new InvalidOperationException("Envelope item payload is malformed.");
            return new EnvelopeItem(header, Encoding.UTF8.GetBytes(payload));
        }
    }
}

public sealed class Envelope(JsonElement header, IReadOnlyList<EnvelopeItem> items)
{
    public JsonElement Header { get; } = header;
    public IReadOnlyList<EnvelopeItem> Items { get; } = items;

    public string? TryGetDsn()
    {
        return TryGetHeader("dsn")?.GetString();
    }

    public string? TryGetEventId()
    {
        return TryGetHeader("event_id")?.GetString();
    }

    public JsonElement? TryGetHeader(string key)
    {
        return Header.TryGetProperty(key, out var value) ? value : null;
    }

    public EnvelopeItem? TryGetEvent()
    {
        return Items.FirstOrDefault(i => i.TryGetType() == "event");
    }

    public async Task SerializeAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        await using var writer = new StreamWriter(stream, leaveOpen: true);

        var json = Header.GetRawText();
        await writer.WriteLineAsync(json.AsMemory(), cancellationToken).ConfigureAwait(false);

        foreach (var item in Items)
        {
            await item.SerializeAsync(writer, cancellationToken).ConfigureAwait(false);
        }
    }

    public static async Task<Envelope> DeserializeAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        var buffer = await stream.ReadLineAsync(cancellationToken).ConfigureAwait(false) ??
                     throw new InvalidOperationException("Envelope header is malformed.");
        var header = JsonDocument.Parse(buffer).RootElement;

        var items = new List<EnvelopeItem>();
        while (stream.Position < stream.Length)
        {
            var item = await EnvelopeItem.DeserializeAsync(stream, cancellationToken).ConfigureAwait(false);
            items.Add(item);
        }

        return new Envelope(header, items);
    }

    public static Envelope FromJson(object header, IEnumerable<(object header, object payload)> items)
    {
        var envelopeHeader = JsonSerializer.SerializeToElement(header);

        var envelopeItems = new List<EnvelopeItem>();
        foreach (var item in items)
        {
            var itemHeader = JsonSerializer.SerializeToElement(item.header);
            var itemPayload = JsonSerializer.SerializeToElement(item.payload);
            envelopeItems.Add(new EnvelopeItem(itemHeader, Encoding.UTF8.GetBytes(itemPayload.GetRawText())));
        }

        return new Envelope(envelopeHeader, envelopeItems);
    }
}

internal static class StreamExtensions
{
    public static async Task<string?> ReadLineAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        var line = new List<byte>();
        var buffer = new byte[1];
        while (true)
        {
            var read = await stream.ReadAsync(buffer, 0, 1, cancellationToken).ConfigureAwait(false);
            if (read == 0 || buffer[0] == '\n')
            {
                break;
            }
            line.Add(buffer[0]);
        }
        return Encoding.UTF8.GetString(line.ToArray());
    }

    public static async Task<int> PeekAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        var pos = stream.Position;
        var buffer = new byte[1];
        var read = await stream.ReadAsync(buffer, 0, 1, cancellationToken).ConfigureAwait(false);
        stream.Position = pos;
        return read == 0 ? -1 : buffer[0];
    }
}
