using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity turn);

        IReadOnlyCollection<GameTurnEntity> GetLastTurns(Guid gameId, int limit);
    }
}