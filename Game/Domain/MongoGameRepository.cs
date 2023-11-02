using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gameCollection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db, bool recreate = false)
        {
            if (recreate)
                db.DropCollection(CollectionName);
            
            gameCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            var filter = Builders<GameEntity>.Filter.Eq(game => game.Id, gameId);
            return gameCollection.Find(filter).First();
        }

        public void Update(GameEntity game)
        {
            var filter = Builders<GameEntity>.Filter.Eq(actual => actual.Id, game.Id);
            gameCollection.ReplaceOne(filter, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            var filter = Builders<GameEntity>.Filter.Eq(game => game.Status, GameStatus.WaitingToStart);
            return gameCollection.Find(filter).Limit(limit).ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var filterId = Builders<GameEntity>.Filter.Eq(update => update.Id, game.Id);
            var statusId = Builders<GameEntity>.Filter.Eq(update => update.Status, GameStatus.WaitingToStart);
            var compoundFilter = Builders<GameEntity>.Filter.And(filterId, statusId);

            var result = gameCollection.ReplaceOne(compoundFilter, game);
            
            return result.IsAcknowledged && result.IsModifiedCountAvailable && result.ModifiedCount == 1;
        }
    }
}