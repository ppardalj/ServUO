using System;
using System.Linq;
using System.Text.RegularExpressions;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
    public class Route
    {
        private string m_Pattern;
        private Regex m_Matcher;

        public Route(string pattern)
        {
            m_Pattern = pattern;
            m_Matcher = new Regex(string.Format("^{0}$", Regex.Replace(pattern, @"\{(\w+)\}", @"(?<$1>\w+)")));
        }

        public bool IsMatch(string uri)
        {
            return m_Matcher.IsMatch(uri);
        }

        public Parameters GetMatchedParameters(string uri)
        {
            if (!IsMatch(uri))
                throw new ArgumentException();

            var match = m_Matcher.Match(uri);

            return m_Matcher.GetGroupNames()
                .Where(name => !name.Equals("0"))
                .ToDictionary(
                    name => name,
                    name => match.Groups[name].Value
                );
        }

        public override string ToString()
        {
            return m_Pattern;
        }
    }
}
