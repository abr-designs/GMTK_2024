using System;

namespace Levels.Enums
{
    [Flags]
    public enum CONTROLS
    {
        NONE = 0,
        SCALE = 1 << 0,
        X_SCALE = 1 << 1,
        Z_SCALE = 1 << 2,
        X_POS = 1 << 3,
        Z_POS = 1 << 4
    }
}