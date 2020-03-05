using System;
using System.Reflection;

namespace TekHow.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static Type UnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }

        public static object UnderlyingValue(this MemberInfo member, object source)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(source);
                case MemberTypes.Property:
                    return ((PropertyInfo)member).GetValue(source);
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be if type  FieldInfo, PropertyInfo"
                    );
            }
        }
    }
}
