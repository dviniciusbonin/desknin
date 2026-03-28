namespace DeskNin.Models;

public static class SettingValue
{
    public static bool AsBool(string? stored) =>
        bool.TryParse(stored, out var b) && b;

    public static string FromBool(bool value) => value ? "true" : "false";
}
