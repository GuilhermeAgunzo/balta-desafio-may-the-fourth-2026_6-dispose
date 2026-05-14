namespace Dispose.Web.Models;

public sealed record UiAlert(Guid Id, string Title, string Message, AlertTone Tone);
