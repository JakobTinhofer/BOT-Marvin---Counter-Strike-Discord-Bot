using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Items
{
    /// <summary>
    /// Holds an item. Only actually instantiates the object once it is needed in order to save on resources.
    /// </summary>
    /// <typeparam name="T">The Item to hold. Has to inherit <see cref="Item"/></typeparam>
    public class ItemHolder<T> where T: Item
    {
        #region Equality Operators and similar

        public static bool operator==(ItemHolder<T> a, ItemHolder<T> b) {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.Get() == b.Get();
        }
        public static bool operator!=(ItemHolder<T> a, ItemHolder<T> b) {
            if (a is null && b is null)
                return false;
            if (a is null || b is null)
                return true;
            return a.Get() != b.Get(); 
        }

        public override bool Equals(Object a)
        {
            var h = a as ItemHolder<T>;
            if (h == null)
                return false;
            return h.Get() == Get();
        }

        public override int GetHashCode()
        {
            return Get().GetHashCode();
        }
        #endregion




        private BsonDocument itemDoc;
        private T item;

        /// <summary>
        /// Get the <see cref="Item"/> this object is holding. If not already instantiated, creates the <see cref="Item"/>.
        /// </summary>
        public T Get()
        {
            if (item == null)
                item = (T)Item.GetFromBSON(itemDoc);
            return item;
        }

        /// <summary>
        /// Create a holder from a bson document. This is prefered over the constructor from ID, since from ID needs an extra DB lookup.<br/>
        /// Once the item is first requested through <see cref="Get"/>, the item will be created using the information in this BSON.
        /// </summary>
        /// <param name="b">A bson document from the 'items' collection.</param>
        public ItemHolder(BsonDocument b)
        {
            itemDoc = b;
        }
        
        /// <summary>
        /// Creates a holder holding an already loaded item.
        /// </summary>
        /// <param name="i">Any Item.</param>
        public ItemHolder(T i)
        {
            item = i;
        }

        /// <summary>
        /// Create a holder from a bson document. <see cref="ItemHolder{T}"/>(<see cref="BsonDocument"/> b) is prefered over this, since from ID needs an extra DB lookup.<br/>
        /// Once the item is first requested through <see cref="Get"/>, the item will be created using the information in the bson document corresponding to this ObjectID.
        /// </summary>
        /// <param name="id">The '_id' field of an item in the 'items' collection.</param>
        public ItemHolder(ObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            itemDoc = MongoInterface.db.GetCollection<BsonDocument>("items").Find(filter).FirstOrDefault();
        }
    }
}
