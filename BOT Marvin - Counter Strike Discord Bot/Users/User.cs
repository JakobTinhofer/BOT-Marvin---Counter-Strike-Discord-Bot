using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Users
{
    /// <summary>
    /// Represents a user in order to keep track of owned items, coins, etc. Can be found in the 'users' collection
    /// </summary>
    public class User
    {
        
        /// <summary>
        /// Creates a User from a Bson document out of the 'users' collection. Faster than <see cref="User"/>(<see cref="ulong"/> id).
        /// </summary>
        /// <param name="b"></param>
        public User(BsonDocument b) => initFromBson(b);

        /// <summary>
        /// Creates a new user from an id. If the id already exists in the 'users' collection, data is read from there. Else, a new entry is created.
        /// </summary>
        /// <param name="id">The 'user_id' field of the 'users' collection. Also, the <see cref="SocketUser.Id"/> id.</param>
        public User(ulong id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
            var t = MongoInterface.db.GetCollection<BsonDocument>("users").Find(filter).ToList();
            if(t.Count > 0)
            {
                initFromBson(t[0]);
            }
            else
            {
                BsonDocument b = new BsonDocument();
                b["user_id"] = new BsonInt64((long)id);
                UserID = id;
                b["inventory"] = new BsonArray();
                Inventory = new Dictionary<ItemHolder<Item>, int>();
                b["coins"] = new BsonInt64(0);
                Coins = 0;
                MongoInterface.db.GetCollection<BsonDocument>("users").InsertOne(b);
            }
            
            
        }

        /// <summary>
        /// Parses the json data in the given document
        /// </summary>
        /// <param name="b"></param>
        private void initFromBson(BsonDocument b)
        {
            UserID = (ulong)b["user_id"].ToInt64();
            ParseInventory(b);
            Coins = b["coins"].AsInt64;
        }

        /// <summary>
        /// Parses the player inventory from the given item ids
        /// </summary>
        /// <param name="b"></param>
        private void ParseInventory(BsonDocument b)
        {
            var li = b["inventory"].AsBsonArray.ToList();

            
            foreach (ObjectId id in li)
            {
                var itm = new ItemHolder<Item>(id);
                if (!Inventory.ContainsKey(itm))
                    Inventory.Add(itm, 1);
                else
                    Inventory[itm] += 1;
            }
        }

        /// <summary>
        /// The user id. Same as the discord user id.
        /// </summary>
        public ulong UserID { get; private set; }

        /// <summary>
        /// The Inventory with the item and the number of items owned.
        /// </summary>
        public Dictionary<ItemHolder<Item>, int> Inventory { get; private set; } = new Dictionary<ItemHolder<Item>, int>();

        /// <summary>
        /// The number of coins the player owns. 100 Coins = 1€
        /// </summary>
        public long Coins { get; private set; }

        

    }
}
