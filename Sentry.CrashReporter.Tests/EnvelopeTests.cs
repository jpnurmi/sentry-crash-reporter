namespace Sentry.CrashReporter.Tests;

public class EnvelopeTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TwoItems()
    {
        using var file = File.OpenRead("Envelopes/two_items.envelope");
        var envelope = Envelope.DeserializeAsync(file).GetAwaiter().GetResult();

        envelope.TryGetDsn().Should().Be("https://e12d836b15bb49d7bbf99e64295d995b:@sentry.io/42");
        envelope.TryGetEventId().Should().Be("9ec79c33ec9942ab8353589fcb2e04dc");
        envelope.Items.Should().HaveCount(2);

        var attachment = envelope.Items[0];
        attachment.TryGetType().Should().Be("attachment");
        attachment.TryGetHeader("length")?.GetInt64().Should().Be(10);
        attachment.TryGetHeader("content_type")?.GetString().Should().Be("text/plain");
        attachment.TryGetHeader("filename")?.GetString().Should().Be("hello.txt");
        attachment.Payload.Length.Should().Be(10);
        attachment.TryGetPayload()?.Should().BeNull();

        var message = envelope.Items[1];
        message.TryGetType().Should().Be("event");
        message.TryGetHeader("length")?.GetInt64().Should().Be(41);
        message.TryGetHeader("content_type")?.GetString().Should().Be("application/json");
        message.TryGetHeader("filename")?.GetString().Should().Be("application.log");
        message.Payload.Length.Should().Be(41);
        message.TryGetPayload()?.ToString().Should().BeEquivalentTo("""{"message":"hello world","level":"error"}""");
    }

    [Test]
    public void TwoEmptyAttachments()
    {
        using var file = File.OpenRead("Envelopes/two_empty_attachments.envelope");
        var envelope = Envelope.DeserializeAsync(file).GetAwaiter().GetResult();

        envelope.TryGetDsn().Should().BeNull();
        envelope.TryGetEventId().Should().Be("9ec79c33ec9942ab8353589fcb2e04dc");
        envelope.Items.Should().HaveCount(2);

        foreach (var attachment in envelope.Items)
        {
            attachment.TryGetType().Should().Be("attachment");
            attachment.TryGetHeader("length")?.GetInt64().Should().Be(0);
            attachment.TryGetHeader("content_type")?.GetString().Should().BeNull();
            attachment.TryGetHeader("filename")?.GetString().Should().BeNull();
            attachment.Payload.Length.Should().Be(0);
            attachment.TryGetPayload()?.Should().BeNull();
        }
    }

    [Test]
    public void ImplicitLength()
    {
        using var file = File.OpenRead("Envelopes/implicit_length.envelope");
        var envelope = Envelope.DeserializeAsync(file).GetAwaiter().GetResult();

        envelope.TryGetDsn().Should().BeNull();
        envelope.TryGetEventId().Should().Be("9ec79c33ec9942ab8353589fcb2e04dc");
        envelope.Items.Should().HaveCount(1);

        var attachment = envelope.Items[0];
        attachment.TryGetType().Should().Be("attachment");
        attachment.TryGetHeader("length")?.GetInt64().Should().Be(10);
        attachment.TryGetHeader("content_type")?.GetString().Should().BeNull();
        attachment.TryGetHeader("filename")?.GetString().Should().BeNull();
        attachment.Payload.Length.Should().Be(10);
        attachment.TryGetPayload()?.Should().BeNull();
    }

    [Test]
    public void EmptyHeadersEof()
    {
        using var file = File.OpenRead("Envelopes/empty_headers_eof.envelope");
        var envelope = Envelope.DeserializeAsync(file).GetAwaiter().GetResult();

        envelope.TryGetDsn().Should().BeNull();
        envelope.TryGetEventId().Should().BeNull();
        envelope.Items.Should().HaveCount(1);

        var session = envelope.Items[0];
        session.TryGetType().Should().Be("session");
        session.TryGetHeader("length").Should().BeNull();
        session.Payload.Length.Should().Be(75);
        session.TryGetPayload()?.ToString().Should().BeEquivalentTo("""{"started": "2020-02-07T14:16:00Z","attrs":{"release":"sentry-test@1.0.0"}}""");
    }
}
