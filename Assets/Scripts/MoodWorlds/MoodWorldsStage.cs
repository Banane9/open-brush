using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoodWorlds
{
    [Flags]
    public enum MoodWorldsStage
    {
        CreatingPositiveWorld = 1 << 30,
        CreatingNegativeWorld = 1 << 29,
        ReturningToPositiveWorld = 1 << 28,
        WrappingItUp = ReturningToPositiveWorld | 1,
        TyingItIn = ReturningToPositiveWorld | (1 << 1),
        LettingItGo = ReturningToPositiveWorld | (1 << 2),
        ReturnedToPositiveWorld = 1 << 27
    }
}