using System;

namespace Boss.Core
{
    public interface IBossPhaseController
    {
        int CurrentPhase { get; }
        event Action<int> PhaseChanged;
    }
}
