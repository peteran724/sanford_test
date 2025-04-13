using System.ComponentModel;
using System.Reflection;

namespace BlueCorp.Common
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description;
        }
    }
}
