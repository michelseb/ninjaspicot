using System;
using System.Linq;

namespace ZepLink.RiceNinja.Extensions
{
    public static class AssignableExtension
    {
        /// <summary>
        /// Determines whether the <paramref name="genericType"/> is assignable from
        /// <paramref name="givenType"/> taking into account generic definitions
        /// </summary>
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType == null || genericType == null)
            {
                return false;
            }

            return givenType == genericType
              || givenType.MapsToGenericTypeDefinition(genericType)
              || givenType.HasInterfaceThatMapsToGenericTypeDefinition(genericType)
              || givenType.BaseType.IsAssignableToGenericType(genericType);
        }

        private static bool HasInterfaceThatMapsToGenericTypeDefinition(this Type givenType, Type genericType)
        {
            return givenType
              .GetInterfaces()
              .Where(it => it.IsGenericType)
              .Any(it => it.GetGenericTypeDefinition() == genericType);
        }

        private static bool MapsToGenericTypeDefinition(this Type givenType, Type genericType)
        {
            return genericType.IsGenericTypeDefinition
              && givenType.IsGenericType
              && givenType.GetGenericTypeDefinition() == genericType;
        }

        public static bool IsInstanceOfGenericType(this Type type, Type genericType)
        {
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        public static Type GetGenericArgument(this Type type)
        {
            if (type.IsInterface)
                return GetInterfaceGenericArgument(type);

            while (type != null)
            {
                if (type.IsGenericType)
                    return type.GetGenericArguments().Last();

                type = type.BaseType;
            }

            return null;
        }

        private static Type GetInterfaceGenericArgument(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                    return type.GetGenericArguments().Last();

                type = type.GetInterfaces().First();
            }

            return null;
        }
    }
}
