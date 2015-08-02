namespace UglyToad.FakeItEasy.EntityFrameworkFakes
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ReflectionHelper
    {
        private const string Id = "ID";

        public static Func<T, object> GetId<T>()
        {
            var property = GetIdProperty<T>();

            if (property == null)
            {
                return null;
            }

            var getMethodInfo = property.GetGetMethod();

            var entityParameter = Expression.Parameter(typeof (T), "entity");

            var getterCall = Expression.Call(entityParameter, getMethodInfo);

            var castToObject = Expression.Convert(getterCall, typeof (object));

            var lambda = Expression.Lambda<Func<T, object>>(castToObject, entityParameter);

            return lambda.Compile();
        }

        private static PropertyInfo GetIdProperty<T>()
        {
            // Convention is "Id" or "ClassName + Id".
            var className = typeof (T).Name;

            // A property is considered public to reflection if it has at least one accessor that is public. 
            var properties =
                typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            PropertyInfo idPropertyInfo = null;

            foreach (var property in properties)
            {
                if (property.Name.Equals(Id, StringComparison.OrdinalIgnoreCase)
                    || property.Name.Equals(className + Id, StringComparison.OrdinalIgnoreCase))
                {
                    idPropertyInfo = property;
                    break;
                }
            }

            return idPropertyInfo;
        }
    }
}
