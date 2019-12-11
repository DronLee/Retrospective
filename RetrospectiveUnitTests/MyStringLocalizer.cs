using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Retrospective;

namespace RetrospectiveUnitTests
{
    internal class MyStringLocalizer : IStringLocalizer<SharedResources>
    {
        public LocalizedString this[string name] => new LocalizedString(name, name);

        public LocalizedString this[string name, params object[] arguments] => throw new System.NotImplementedException();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new System.NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}