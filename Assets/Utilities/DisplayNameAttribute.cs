using System;

namespace Utilities
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayNameAttribute : Attribute
    {
        string _name;
        public string Name
        {
            get => _name;
        }

        public DisplayNameAttribute(string name)
        {
            _name = name;
        }
    }
}
