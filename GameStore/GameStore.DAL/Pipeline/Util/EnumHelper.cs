﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace GameStore.DAL.Pipeline.Util
{
    [ExcludeFromCodeCoverage]
    public static class EnumHelper
    {
        public static T GetValueFromDescription<T>(string description)
            where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(
                        field,
                        typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }
    }
}
