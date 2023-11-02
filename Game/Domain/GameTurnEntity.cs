using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public readonly Guid Id;
        public readonly Guid GameId;
        public readonly Guid WinningId;
        public readonly IReadOnlyCollection<Guid> Players;
        public readonly DateTime DateTime;

        public GameTurnEntity(Guid id, Guid gameId, Guid winningId, IEnumerable<Player> players, DateTime dateTime)
        {
            Id = id;
            WinningId = winningId;
            DateTime = dateTime;
            GameId = gameId;
            Players = players.Select(player => player.UserId).ToList();
        }
    }
}