﻿namespace Newtonsoft.Json.Serialization
{
    using System;
    using System.Collections.Generic;

    public interface IAttributeProvider
    {
        IList<Attribute> GetAttributes(bool inherit);
        IList<Attribute> GetAttributes(Type attributeType, bool inherit);
    }
}

