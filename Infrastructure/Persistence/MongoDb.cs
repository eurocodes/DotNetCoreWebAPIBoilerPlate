using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Application.DTOs.Configurations;
using Core.Models.Attributes;
using LinqKit;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NetCore.AutoRegisterDi;

namespace Infrastructure.Persistence {
    [RegisterAsScoped]
    public class MongoDb : IMongoDb {
        private IMongoDatabase _dataStore;
        private MongoClient _client;
        private DBConfig _config;

        public MongoDb(IOptionsMonitor<SystemVariables> config) {
            configure(config.CurrentValue.MongoDB);
        }
        public MongoDb(DBConfig config) {
            configure(config);
        }

        private void configure(DBConfig config) {
            _config = config;
            string server = string.Concat(config.server, ":", config.port);
            server = !string.IsNullOrEmpty(config.username) ? string.Concat(config.username, ":", config.password, "@", server) : server;
            string _connectionString = string.Concat("mongodb://", server);
            _client = new MongoClient(_connectionString);
            _connect();
        }

        private bool _connect() {
            _dataStore = _dataStore ?? _client.GetDatabase(_config.database);
            return true;
        }

        public async Task<bool> createUniqueIndexes<T>(IMongoCollection<T> collection) {            
            var uniqueStringIndexProperties = typeof(T).GetProperties().Where(
            prop => Attribute.IsDefined(prop, typeof(DBIndex))).ToList();
            if (uniqueStringIndexProperties.Any()) {
                foreach (var propertyInfo in uniqueStringIndexProperties) {                    
                    var propertyInfoName = propertyInfo.Name;
                    DBIndex indexAttr = (DBIndex)Attribute.GetCustomAttribute(propertyInfo, typeof(DBIndex));
                    bool unique = indexAttr.isUnique;
                    var options = new CreateIndexOptions { Unique = unique };
                    var field = new StringFieldDefinition<T>(propertyInfoName);
                    var indexDefinition = new IndexKeysDefinitionBuilder<T>().Ascending(field);
                    var indexModel = new CreateIndexModel<T>(indexDefinition, options);
                    await collection.Indexes.CreateOneAsync(indexModel);
                }
            }
            return true;
        }
        public async Task<bool> collectionExists(string collectionName) {
            var filter = new BsonDocument("name", collectionName);
            var collections = await _dataStore.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            return await collections.AnyAsync();
        }
        public async Task<bool> insert<T>(T data) {
            bool collectionCreatedBefore = await collectionExists(typeof(T).Name);
            var collection = _dataStore.GetCollection<T>(typeof(T).Name);
            if (!collectionCreatedBefore)
                await createUniqueIndexes<T>(collection);
            await collection.InsertOneAsync(data);
            return true;
        }
        public FilterDefinition<T> getFilter<T>(Expression<Func<T, bool>> condition) {
            var filter = Builders<T>.Filter.Where(condition);
            return filter;
        }
        public FilterDefinition<T> getFilter<T>(IEnumerable<Expression<Func<T, bool>>> conditions) {
            var predicates = PredicateBuilder.New<T>();
            foreach (Expression<Func<T, bool>> condition in conditions) {
                predicates = predicates.And(condition);
            }
            var filter = Builders<T>.Filter.Where((Expression<Func<T, bool>>)predicates.Expand());
            return filter;
        }
        public FindOptions<T> getFindOptions<T>(int limit) {
            FindOptions<T> options = new FindOptions<T> { Limit = limit };
            return options;
        }
        public FindOptions<T> getFindOptions<T>(Dictionary<string, int> sorting) {
            List<SortDefinition<T>> definitions = new List<SortDefinition<T>>();
            if (sorting != null) {
                foreach (KeyValuePair<string, int> kvp in sorting) {
                    if (kvp.Value == 1) {
                        definitions.Add(Builders<T>.Sort.Ascending(kvp.Key));
                    } else {
                        definitions.Add(Builders<T>.Sort.Descending(kvp.Key));
                    }
                }
            }            
            FindOptions<T> options = new FindOptions<T> { Sort = Builders<T>.Sort.Combine(definitions) };
            return options;
        }
        public FindOptions<T> getFindOptions<T>(int limit, Dictionary<string, int> sorting) {
            FindOptions<T> limitOptions = this.getFindOptions<T>(limit);
            FindOptions<T> sortOptions = this.getFindOptions<T>(sorting);
            limitOptions.Sort = sortOptions.Sort;
            return limitOptions;
        }

        public async Task<bool> update<T>(FilterDefinition<T> filter, object updateField) {
            var collection = _dataStore.GetCollection<T>(typeof(T).Name);
            var updateDefination = new List<UpdateDefinition<T>>();
            foreach (var property in updateField.GetType().GetProperties()) {
                var value = property.GetValue(updateField);
                updateDefination.Add(Builders<T>.Update.Set(property.Name, value));
            }
            var combinedUpdate = Builders<T>.Update.Combine(updateDefination);
            var result = await collection.UpdateManyAsync(filter, combinedUpdate);
            return result.IsAcknowledged;
        }

        public async Task<bool> update<T>(FilterDefinition<T> filter, Dictionary<string, dynamic> updateField) {
            var collection = _dataStore.GetCollection<T>(typeof(T).Name);
            var updateDefination = new List<UpdateDefinition<T>>();
            foreach(KeyValuePair<string, dynamic> kvp in updateField) {
                var value = kvp.Value;
                try {
                    updateDefination.Add(Builders<T>.Update.Set(kvp.Key, value));
                } catch {
                    updateDefination.Add(Builders<T>.Update.Set(kvp.Key, ""));
                }
                
            }
            var combinedUpdate = Builders<T>.Update.Combine(updateDefination);
            var result = await collection.UpdateManyAsync(filter, combinedUpdate);
            return result.IsAcknowledged;
        }

        public async Task<bool> updateIncremental<T>(FilterDefinition<T> filter, object updateField) {
            var collection = _dataStore.GetCollection<T>(typeof(T).Name);
            var updateDefination = new List<UpdateDefinition<T>>();
            foreach (var property in updateField.GetType().GetProperties()) {
                var value = property.GetValue(updateField);
                updateDefination.Add(Builders<T>.Update.Inc(property.Name, value));
            }
            var combinedUpdate = Builders<T>.Update.Combine(updateDefination);
            var result = await collection.UpdateManyAsync(filter, combinedUpdate);
            return result.IsAcknowledged;
        }
        public async Task<IEnumerable<T>> select<T>() {
            return await select<T>(null, null);
        }

        public async Task<IEnumerable<T>> select<T>(FilterDefinition<T> filter) {
            return await select<T>(filter, null);
        }

        public async Task<IEnumerable<T>> select<T>(FindOptions<T> filter) {
            return await select<T>(null, filter);
        }

        public async Task<IEnumerable<T>> select<T>(FilterDefinition<T> filter, FindOptions<T> options) {
            var collection = _dataStore.GetCollection<T>(typeof(T).Name);
            List<T> result;
            if (filter == null) {
                filter = Builders<T>.Filter.Empty;
            }
            if (options != null) {
                result = (await collection.FindAsync(filter, options)).ToList<T>();
            } else {                
                result = (await collection.FindAsync(filter)).ToList<T>();
            }
            return result;
        }

        public async Task<bool> delete<T>(FilterDefinition<T> filter) {
            var collection = _dataStore.GetCollection<T>(typeof(T).Name);
            var result = await collection.DeleteManyAsync(filter);
            return result.IsAcknowledged;
        }
    }
}
