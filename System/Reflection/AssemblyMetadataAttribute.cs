namespace System.Reflection
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    internal class AssemblyMetadataAttribute : Attribute
    {
        public AssemblyMetadataAttribute(string key, string value)
        {
        }
    }
}

