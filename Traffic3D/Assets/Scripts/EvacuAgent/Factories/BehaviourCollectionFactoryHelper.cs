using System;
using System.Linq;

public static class BehaviourCollectionFactoryHelper
{
    public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        return type.GetField(name)
            .GetCustomAttributes(false)
            .OfType<TAttribute>()
            .SingleOrDefault();
    }
}
