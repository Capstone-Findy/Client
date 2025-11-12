using System;


namespace Findy.JsonHelper
{
    public static class JsonHelper
    {
        public static string WrapJsonIfObject(string input)
        {
            string trimmed = input?.Trim();
            if (string.IsNullOrEmpty(trimmed)) return "{}";
            if (trimmed.StartsWith("{")) return trimmed;
            return "{}";
        }
        public static string WrapJsonIfBare(string input)
        {
            string trimmed = input?.Trim();
            if (string.IsNullOrEmpty(trimmed)) return "{}";
            if (trimmed.StartsWith("{")) return trimmed;
            return $"{{\"token\":\"{trimmed}\"}}";
        }
    }
}