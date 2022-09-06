using System.Reflection;

namespace ZepLink.RiceNinja.Utils
{
    public static class ReflectionUtils
    {
        public static void SetFieldValue<T>(object obj, string name, T val)
        {
            var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, val);
        }
    }
}
