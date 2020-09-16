using System;

namespace GameStore.Web.Util
{
    public static class ControllerExtensions
    {
        public static string GetName(Type controllerType)
        {
            // Get instance of the attribute.
            CustomControllerAttribute attribute =
                (CustomControllerAttribute)Attribute.GetCustomAttribute(controllerType, typeof(CustomControllerAttribute));

            if (attribute == null)
            {
                return nameof(controllerType);
            }

            return attribute.Name;
        }
    }
}
