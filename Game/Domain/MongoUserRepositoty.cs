using System;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database, bool recreate = false)
        {
            if (recreate)
                database.DropCollection(CollectionName);
            
            userCollection = database.GetCollection<UserEntity>(CollectionName);

            var indexKeysDefinition = new IndexKeysDefinitionBuilder<UserEntity>()
                .Ascending(d => d.Login);
            var loginOptions = new CreateIndexOptions { Unique = true };
            var loginAscendingIdx = new CreateIndexModel<UserEntity>(indexKeysDefinition, loginOptions);

            userCollection.Indexes.CreateOne(loginAscendingIdx);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);

            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var filter = Builders<UserEntity>.Filter.Eq(user => user.Id, id);
            return userCollection.Find(filter).First();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var filter = Builders<UserEntity>.Filter.Eq(user => user.Login, login);
            var find = userCollection.Find(filter).FirstOrDefault();
            if (find != null)
                return find;

            var newUser = new UserEntity(Guid.Empty) { Login = login };
            userCollection.InsertOne(newUser);
            return newUser;
        }

        public void Update(UserEntity user)
        {
            var filter = Builders<UserEntity>.Filter.Eq(actual => actual.Id, user.Id);
            userCollection.ReplaceOne(filter, user);
        }

        public void Delete(Guid id)
        {
            var filter = Builders<UserEntity>.Filter.Eq(user => user.Id, id);
            userCollection.DeleteOne(filter);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var sort = Builders<UserEntity>.Sort.Ascending(user => user.Login);

            var result = userCollection
                .Find(FilterDefinition<UserEntity>.Empty)
                .Sort(sort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            return new PageList<UserEntity>(result,
                userCollection.CountDocuments(FilterDefinition<UserEntity>.Empty),
                pageNumber,
                pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}