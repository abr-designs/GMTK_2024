using System;

namespace Levels.Enums
{
    [Flags]
    public enum CONTROLS
    {
        NONE = 0,
        V1_SCALE = 1 << 0,
        V1_X_SCALE = 1 << 1,
        V1_Z_SCALE = 1 << 2,
        V1_X_POS = 1 << 7,
        V1_Z_POS = 1 << 4,
        V1_Y_ROT = 1 << 5,
        V2_XZ_SCALE = 1 << 6,
        V2_XZ_POS = 1 << 3,
    }
}