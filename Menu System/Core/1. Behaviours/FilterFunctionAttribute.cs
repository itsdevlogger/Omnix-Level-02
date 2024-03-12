using System;

namespace MenuManagement.Behaviours
{
    [AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = false)]
    public class FilterFunctionAttribute : Attribute
    {
        public readonly string name;
        public readonly string description;


        public FilterFunctionAttribute(string name)
        {
            this.name = name;
            this.description = "";
        }


        public FilterFunctionAttribute(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
}