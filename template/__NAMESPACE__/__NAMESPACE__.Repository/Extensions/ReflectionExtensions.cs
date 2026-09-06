using System.Reflection;

namespace __NAMESPACE__.Repository.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool IsSimple(this Type type) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? type.GetGenericArguments()[0].GetTypeInfo().IsSimple()
                : type.IsEnum
                  || type.IsPrimitive
                  || type.IsValueType
                  || type == typeof(string)
                  || type == typeof(double)
                  || type == typeof(decimal);

        public static bool IsCollection(this Type type)
            => !type.IsSimple() && type.GetInterface("IEnumerable`1") != null;
    }
}
