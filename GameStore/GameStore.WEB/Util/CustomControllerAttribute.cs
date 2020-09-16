using System;

namespace GameStore.Web.Util
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomControllerAttribute : Attribute
    {
        private readonly string _name;

        public CustomControllerAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
