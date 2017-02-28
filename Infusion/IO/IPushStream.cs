﻿using System;

namespace Infusion.IO
{
    public interface IPushStream : IDisposable
    {
        void Write(byte[] buffer, int offset, int count);
        void WriteByte(byte value);
    }
}