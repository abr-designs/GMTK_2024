using System;
using UnityEngine;

namespace Interfaces
{
    public interface IDisplayResults
    {
        Coroutine Display(Action uiDisplayReady);
    }
}