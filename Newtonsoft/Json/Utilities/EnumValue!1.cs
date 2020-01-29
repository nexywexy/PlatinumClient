namespace Newtonsoft.Json.Utilities
{
    using System;

    internal class EnumValue<T> where T: struct
    {
        private readonly string _name;
        private readonly T _value;

        public EnumValue(string name, T value)
        {
            this._name = name;
            this._value = value;
        }

        public string Name =>
            this._name;

        public T Value =>
            this._value;
    }
}

