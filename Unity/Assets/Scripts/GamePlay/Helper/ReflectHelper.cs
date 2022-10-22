using System;
using System.Collections.Generic;

namespace SunAndMoon
{
    public class ReflectHelper
    {
        public static void FindImplements<T>(Action<Type> handler)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = asm.GetTypes();
                foreach (var type in types)
                {
                    if (typeof(T).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    {
                        handler(type);
                    }
                }
            }
        }

        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            var attrs = type.GetCustomAttributes(true);
            foreach (var attr in attrs)
            {
                if (typeof(T).IsAssignableFrom(attr.GetType()))
                {
                    return attr as T;
                }
            }
            return null;
        }
    }
}