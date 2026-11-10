using System.Reflection;

namespace UnitTests;

internal static class Tools
{
    internal static string GetEmbedded(string path)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"UnitTests.{path.Trim()}.txt";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}