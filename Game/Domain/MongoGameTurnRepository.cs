using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private const string CollectionName = "gameTurns";

        private readonly IMongoCollection<GameTurnEntity> turnsCollection;

        public MongoGameTurnRepository(IMongoDatabase db, bool recreate = false)
        {
            if (recreate)
                db.DropCollection(CollectionName);

            turnsCollection = db.GetCollection<GameTurnEntity>(CollectionName);
            
            var indexKeysDefinition = new IndexKeysDefinitionBuilder<GameTurnEntity>().Ascending(d => d.GameId).Descending(d => d.DateTime);
            var dateTimeIndex = new CreateIndexModel<GameTurnEntity>(indexKeysDefinition);

            turnsCollection.Indexes.CreateOne(dateTimeIndex);
        }

        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            turnsCollection.InsertOne(turn);
            return turn;
        }

        public IReadOnlyCollection<GameTurnEntity> GetLastTurns(Guid gameId, int limit)
        {
            var filter = Builders<GameTurnEntity>.Filter.Eq(turn => turn.Id, gameId);
            var sort = Builders<GameTurnEntity>.Sort.Descending(turn => turn.DateTime);

            return turnsCollection.Find(filter).Sort(sort).Limit(limit).ToList();
        }
    }
}