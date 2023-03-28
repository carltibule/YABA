using System;

namespace YABA.Common.Attributes
{
    public class ClaimNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public ClaimNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
