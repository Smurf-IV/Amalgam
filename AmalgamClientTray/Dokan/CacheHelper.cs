using System;
using System.Collections.Generic;

namespace AmalgamClientTray.Dokan
{
   /// <summary>
   /// stolen from the discussions in http://blogs.infosupport.com/blogs/frankb/archive/2009/03/15/CacheDictionary-for-.Net-3.5_2C00_-using-ReaderWriterLockSlim-_3F00_.aspx
   /// And then made it more useable for the cache timeout implementation.
   /// I did play with the ConcurrentDictonary, but this made the simplecity of using a Mutex and a normal dictionary very difficult to read.
   /// </summary>
   /// <example>
   /// Use it like a dictionay and then add the functions required
   /// </example>
   /// <remarks>
   /// Does not implement all the interfaces of IDictioanry.
   /// All Thread access locking is perfromed with this object, so no need for access locking by the caller.
   /// </remarks>
   public class CacheHelper<TKey, TValue>
   {
#region private fields
      private readonly object cacheLock = new object();
      private class ValueObject<TValueObj>
      {
         private readonly DateTimeOffset Created;
         public readonly TValueObj CacheValue;

         public ValueObject(uint expireSeconds, TValueObj value)
         {
            Created = new DateTimeOffset(DateTime.Now).AddSeconds(expireSeconds);
            CacheValue = value;
         }

         public bool IsValid
         {
            get { return Created > DateTime.Now; }
         }
      }

      private readonly uint expireSeconds;
      private readonly Dictionary<TKey, ValueObject<TValue>> Cache = new Dictionary<TKey, ValueObject<TValue>>();

#endregion

      /// <summary>
      /// Constructor with the timout value
      /// </summary>
      /// <param name="expireSeconds">timeout cannot be -ve</param>
      /// <remarks>
      /// expiresecounds must be less than 14 hours otherwise the DateTimeOffset for each object will throw an exception
      /// </remarks>
      public CacheHelper(uint expireSeconds)
      {
         this.expireSeconds = expireSeconds;
      }

      /// <summary>
      /// Value replacement and retrieval
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      public TValue this[TKey key]
      {
         get
         {
            lock (cacheLock)
            {
               ValueObject<TValue> value = Cache[key];
               if (value != null)
               {
                  if (value.IsValid)
                     return value.CacheValue;
                  else
                  {
                     Cache.Remove(key);
                  }
               }
            }
            throw new KeyNotFoundException();

            return default(TValue);
         }
         set
         {
            lock (cacheLock)
            {
               Cache[key] = new ValueObject<TValue>(expireSeconds, value);
            }
         }
      }

      /// <summary>
      /// Does the value exist at this key that has not timed out ?
      /// </summary>
      /// <param name="key"></param>
      /// <param name="value"></param>
      /// <returns></returns>
      public bool TryGetValue(TKey key, out TValue value)
      {
         lock (cacheLock)
         {
            ValueObject<TValue> valueobj;
            if (Cache.TryGetValue(key, out valueobj) )
            {
               if (valueobj.IsValid)
               {
                  value = valueobj.CacheValue;
                  return true;
               }
               else
               {
                  Cache.Remove(key);
               }
            }
         }

         value = default(TValue);
         return false;
      }

      /// <summary>
      /// Remove the value
      /// </summary>
      /// <param name="key"></param>
      public void Remove(TKey key)
      {
         lock (cacheLock)
         {
            Cache.Remove(key);
         }
      }
   }
}
