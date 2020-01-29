namespace Newtonsoft.Json.Linq
{
    using System;
    using System.ComponentModel;

    public class JPropertyDescriptor : PropertyDescriptor
    {
        public JPropertyDescriptor(string name) : base(name, null)
        {
        }

        public override bool CanResetValue(object component) => 
            false;

        private static JObject CastInstance(object instance) => 
            ((JObject) instance);

        public override object GetValue(object component) => 
            CastInstance(component)[this.Name];

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            JToken token = (value is JToken) ? ((JToken) value) : new JValue(value);
            CastInstance(component)[this.Name] = token;
        }

        public override bool ShouldSerializeValue(object component) => 
            false;

        public override Type ComponentType =>
            typeof(JObject);

        public override bool IsReadOnly =>
            false;

        public override Type PropertyType =>
            typeof(object);

        protected override int NameHashCode =>
            base.NameHashCode;
    }
}

