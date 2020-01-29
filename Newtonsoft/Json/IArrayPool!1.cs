﻿namespace Newtonsoft.Json
{
    using System;

    public interface IArrayPool<T>
    {
        T[] Rent(int minimumLength);
        void Return(T[] array);
    }
}

