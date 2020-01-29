namespace Newtonsoft.Json.Utilities
{
    using System;
    using System.Runtime.CompilerServices;

    internal class ReflectionMember
    {
        public Type MemberType { get; set; }

        public Func<object, object> Getter { get; set; }

        public Action<object, object> Setter { get; set; }
    }
}

