using System.ComponentModel;

namespace Sentry.CrashReporter.Models;

public enum ExceptionCodeLinux : uint
{
    [Description("Hangup")] // (POSIX)
    SIGHUP = 0x1,

    [Description("Interrupt")] // (ANSI)
    SIGINT = 0x2,

    [Description("Quit")] // (POSIX)
    SIGQUIT = 0x3,

    [Description("Illegal instruction")] // (ANSI)
    SIGILL = 0x4,

    [Description("Trace trap")] // (POSIX)
    SIGTRAP = 0x5,

    [Description("Abort")] // (ANSI)
    SIGABRT = 0x6,

    [Description("BUS error")] // (4.2 BSD)
    SIGBUS = 0x7,

    [Description("Floating-point exception")] // (ANSI)
    SIGFPE = 0x8,

    [Description("Kill, unblockable")] // (POSIX)
    SIGKILL = 0x9,

    [Description("User-defined signal 1")] // (POSIX)
    SIGUSR1 = 0xa,

    [Description("Segmentation violation")] // (ANSI)
    SIGSEGV = 0xb,

    [Description("User-defined signal 2")] // (POSIX)
    SIGUSR2 = 0xc,

    [Description("Broken pipe")] // (POSIX)
    SIGPIPE = 0xd,

    [Description("Alarm clock")] // (POSIX)
    SIGALRM = 0xe,

    [Description("Termination")] // (ANSI)
    SIGTERM = 0xf,
    [Description("Stack fault")] SIGSTKFLT = 0x10,

    [Description("Child status has changed")] // (POSIX)
    SIGCHLD = 0x11,

    [Description("Continue")] // (POSIX)
    SIGCONT = 0x12,

    [Description("Stop, unblockable")] // (POSIX)
    SIGSTOP = 0x13,

    [Description("Keyboard stop")] // (POSIX)
    SIGTSTP = 0x14,

    [Description("Background read from tty")] // (POSIX)
    SIGTTIN = 0x15,

    [Description("Background write to tty")] // (POSIX)
    SIGTTOU = 0x16,

    [Description("Urgent condition on socket")] // (4.2 BSD)
    SIGURG = 0x17,

    [Description("CPU limit exceeded")] // (4.2 BSD)
    SIGXCPU = 0x18,

    [Description("File size limit exceeded")] // (4.2 BSD)
    SIGXFSZ = 0x19,

    [Description("Virtual alarm clock")] // (4.2 BSD)
    SIGVTALRM = 0x1a,

    [Description("Profiling alarm clock")] // (4.2 BSD)
    SIGPROF = 0x1b,

    [Description("Window size change")] // (4.3 BSD, Sun)
    SIGWINCH = 0x1c,

    [Description("I/O now possible")] // (4.2 BSD)
    SIGIO = 0x1d,

    [Description("Power failure restart")] // (System V)
    SIGPWR = 0x1e,
    [Description("Bad system call")] SIGSYS = 0x1f,
    [Description("Dump requested")] DUMP_REQUESTED = 0xffffffff
}

// public static class ExceptionCodeLinuxExtensions
// {
//     public static string? Desribe(this ExceptionCodeLinux code)
//     {
//         var member = code.GetType().GetMember(code.ToString()).SingleOrDefault();
//         var attribute =
//             member?.GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;
//         return attribute?.Description;
//     }
// }

public record ExceptionCode(string Type, string Value);

public static class ExceptionCodeExtensions
{
    public static ExceptionCode? AsExceptionCode(this uint code, string os)
    {
        Dictionary<string, Type> osEnums = new()
        {
            { "linux", typeof(ExceptionCodeLinux) }
            // { "windows", typeof(ExceptionCodeWindows) },
            // { "macos", typeof(ExceptionCodeMacos) }
        };

        if (!osEnums.TryGetValue(os.ToLowerInvariant(), out var enumType) || !Enum.IsDefined(enumType, code))
        {
            return null;
        }

        var name = Enum.ToObject(enumType, code).ToString();
        var member = enumType.GetMember(name ?? string.Empty).FirstOrDefault();
        var attribute = member?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .OfType<DescriptionAttribute>().FirstOrDefault();

        if (name is null || attribute is null)
        {
            return null;
        }

        return new ExceptionCode(name, attribute.Description);
    }
}
