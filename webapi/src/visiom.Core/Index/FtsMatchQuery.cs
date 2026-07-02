using System.Text;

namespace visiom.Core.Index
{
    internal static class FtsMatchQuery
    {
        // Quote each alphanumeric token and AND them, so user input can't inject FTS5 operators.
        public static string Build(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;

            var tokens = new List<string>();
            var token = new StringBuilder();
            foreach (var ch in query)
            {
                if (char.IsLetterOrDigit(ch)) token.Append(ch);
                else if (token.Length > 0) { tokens.Add(token.ToString()); token.Clear(); }
            }
            if (token.Length > 0) tokens.Add(token.ToString());

            if (tokens.Count == 0) return null;

            return string.Join(" AND ", tokens.Select(t => $"\"{t.Replace("\"", "\"\"")}\""));
        }
    }
}
