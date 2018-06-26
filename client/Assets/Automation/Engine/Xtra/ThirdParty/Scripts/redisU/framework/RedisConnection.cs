/// Copyright (c) 2011, Anshul Goyal <anshul dot goyal at hotmail dot com>
/// All rights reserved.
///
/// Author: Anshul Goyal
/// 
///	Redistribution and use in source and binary forms, with or without
/// modification, are permitted provided that the following conditions are met:
///
///  * Redistributions of source code must retain the above copyright notice,
///     this list of conditions and the following disclaimer.
///  * Redistributions in binary form must reproduce the above copyright
///    notice, this list of conditions and the following disclaimer in the
///    documentation and/or other materials provided with the distribution.
///  * Neither the name of Redis nor the names of its contributors may be used
///    to endorse or promote products derived from this software without
///    specific prior written permission.
///
///THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
///AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
///IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
///ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
///LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
///CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
///SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
///INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
///CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
///ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
///POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;
using System.Text;
using System.Threading;
using redisU.utils;

namespace redisU.framework
{
	public sealed class RedisConnection : RedisConnectionBase
	{
		private Thread dataListener = null;
		
		public RedisConnection()
		{
			CreateConnection();
			StartDataListener();
		}
		
		public RedisConnection(string host, int port)
		{
			CreateConnection(host, port);
			StartDataListener();
		}
		
		public void EndConnection()
		{
			StopDataListener();
			Dispose();	
		}
		
		public int DeleteKeys<K>(params K[] keys)
		{
			ValidateArguments<K>(keys);
			string[] reply = ExecuteCommand(RedisCommand.DEL, ChangeTypeArrayToString<K>(keys));
			return ConvertReplyToInt(reply);
		}
		
		public void _DeleteKeys<K>(params K[] keys)
		{
			ValidateArguments<K>(keys);
			EnqueueAndSendCommand(RedisCommand.DEL, ChangeTypeArrayToString<K>(keys));
		}
		
		public string[] GetKeysByPattern(string pattern)
		{
			ValidateArguments(pattern);
			return ExecuteCommand(RedisCommand.KEYS, pattern);
		}
		
		public string GetRandomKey()
		{
			string[] reply = ExecuteCommand(RedisCommand.RANDOMKEY);
			return ConvertReplyToString(reply);
		}
		
		/// <summary>
		/// Returns the timeout for the specified key in seconds
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>/
		public int GetTTL<K>(K key)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.TTL, ChangeTypeToString<K>(key));
			return ConvertReplyToInt(reply);
		}
		
		public bool IfExists<K>(K key)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.EXISTS, ChangeTypeToString<K>(key));
			return ConvertReplyToBool(reply);
		}
		
		public bool MoveKey<K>(K key, int db)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.MOVE, ChangeTypeToString<K>(key), Convert.ToString(db));
			return ConvertReplyToBool(reply);
		}
		
		public void _MoveKey<K>(K key, int db)
		{
			ValidateArguments<K>(key);
			EnqueueAndSendCommand(RedisCommand.MOVE, ChangeTypeToString<K>(key), Convert.ToString(db));
		}
		
		public Constants.StatusCode RenameKey<K1, K2>(K1 key, K2 newKey)
		{
			ValidateArguments<K1>(key);
			ValidateArguments<K2>(newKey);
			string[] reply = ExecuteCommand(RedisCommand.RENAME, ChangeTypeToString<K1>(key), ChangeTypeToString<K2>(newKey));
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public void _RenameKey<K1, K2>(K1 key, K2 newKey)
		{
			ValidateArguments<K1>(key);
			ValidateArguments<K2>(newKey);
			EnqueueAndSendCommand(RedisCommand.RENAME, ChangeTypeToString<K1>(key), ChangeTypeToString<K2>(newKey));
		}
		
		public Constants.KeyType GetKeyType<K>(K key)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.TYPE, ChangeTypeToString<K>(key));
			string retVal = ConvertReplyToString(reply);
			if(retVal == null)
				return Constants.KeyType.UNDEFINED;
			else
				return (Constants.KeyType)Enum.Parse(typeof(Constants.KeyType), retVal, true);
		}
		
		public bool SetKeyExpiry<K>(K key, int timeoutInSeconds)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.EXPIRE, ChangeTypeToString<K>(key), Convert.ToString(timeoutInSeconds));
			return ConvertReplyToBool(reply);
		}
		
		public void _SetKeyExpiry<K>(K key, int timeoutInSeconds)
		{
			ValidateArguments<K>(key);
			EnqueueAndSendCommand(RedisCommand.EXPIRE, ChangeTypeToString<K>(key), Convert.ToString(timeoutInSeconds));
		}
		
		public string GetObject<K>(RedisSubCommand subCommand, params K[] args)
		{
			ValidateArguments<K>(args);
			string[] commandParams = new string[args.Length + 1];
			commandParams[0] = subCommand.ToString();
			Array.Copy(ChangeTypeArrayToString<K>(args), 0, commandParams, 1, args.Length);
			string[] reply = ExecuteCommand(RedisCommand.OBJECT, commandParams);
			return ConvertReplyToString(reply);
		}
		
		public bool RenameKeyIfNotExist<K1, K2>(K1 key, K2 newKey)
		{
			ValidateArguments<K1>(key);
			ValidateArguments<K2>(newKey);
			string[] reply = ExecuteCommand(RedisCommand.RENAMENX, ChangeTypeToString<K1>(key), ChangeTypeToString<K2>(newKey));
			return ConvertReplyToBool(reply);
		}
		
		public void _RenameKeyIfNotExist<K1, K2>(K1 key, K2 newKey)
		{
			ValidateArguments<K1>(key);
			ValidateArguments<K2>(newKey);
			EnqueueAndSendCommand(RedisCommand.RENAMENX, ChangeTypeToString<K1>(key), ChangeTypeToString<K2>(newKey));
		}
		
		/// <summary>
		/// Sets the expiry for the specified key.
		/// timeout is specified in unix timestamp i.e. seconds since January 1, 1970.
		/// </summary>
		/// <returns>
		/// The <see cref="System.Boolean"/>.
		/// </returns>
		/// <param name='key'>
		/// If set to <c>true</c> key.
		/// </param>
		/// <param name='absoluteTimeout'>
		/// If set to <c>true</c> absolute timeout.
		/// </param>
		public bool SetExpireAt<K>(K key, int timeout)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.EXPIREAT, ChangeTypeToString<K>(key), Convert.ToString(timeout));
			return ConvertReplyToBool(reply);
		}
		
		public void _SetExpireAt<K>(K key, int timeout)
		{
			ValidateArguments<K>(key);
			EnqueueAndSendCommand(RedisCommand.EXPIREAT, ChangeTypeToString<K>(key), Convert.ToString(timeout));
		}
		
		public bool Persist<K>(K key)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.PERSIST, ChangeTypeToString<K>(key));
			return ConvertReplyToBool(reply);
		}
		
		public T Get<T, K>(K key)
		{	
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.GET, ChangeTypeToString<K>(key));
			return ChangeStringToType<T>(ConvertReplyToString(reply));
		}
		
		public Constants.StatusCode Set<K, V>(K key, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			string[] reply = ExecuteCommand(RedisCommand.SET, ChangeTypeToString<K>(key), ChangeTypeToString<V>(val));
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public void _Set<K, V>(K key, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			EnqueueAndSendCommand(RedisCommand.SET, ChangeTypeToString<K>(key), ChangeTypeToString<V>(val));
		}
		
		/// <summary>
		/// Appends the 'val' to the specified key's existing value.
		/// Returns the length of the final apended string.
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='val'>
		/// Value.
		/// </param>
		public int Append<K, V>(K key, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			string[] reply = ExecuteCommand(RedisCommand.APPEND, ChangeTypeToString<K>(key), ChangeTypeToString<V>(val));
			return ConvertReplyToInt(reply);
		}
		
		public void _Append<K, V>(K key, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			EnqueueAndSendCommand(RedisCommand.APPEND, ChangeTypeToString<K>(key), ChangeTypeToString<V>(val));
		}
		
		public string GetRangeOfValue<K>(K key, int start, int end)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.GETRANGE, ChangeTypeToString<K>(key), Convert.ToString(start), Convert.ToString(end));
			return ConvertReplyToString(reply);
		}
		
		public Constants.StatusCode SetMultipleKeys<K>(params K[] keys)
		{
			ValidateArguments<K>(keys);
			string[] reply = ExecuteCommand(RedisCommand.MSET, ChangeTypeArrayToString<K>(keys));
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public void _SetMultipleKeys<K>(params K[] keys)
		{
			ValidateArguments<K>(keys);
			EnqueueAndSendCommand(RedisCommand.MSET, ChangeTypeArrayToString<K>(keys));
		}
		
		public bool SetIfNotExist<K, V>(K key, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			string[] reply = ExecuteCommand(RedisCommand.SETNX, ChangeTypeToString<K>(key), ChangeTypeToString<V>(val));
			return ConvertReplyToBool(reply);
		}
		
		public void _SetIfNotExist<K, V>(K key, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			EnqueueAndSendCommand(RedisCommand.SETNX, ChangeTypeToString<K>(key), ChangeTypeToString<V>(val));
		}
		
		public int Decrement<K>(K key)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.DECR, ChangeTypeToString<K>(key));
			return ConvertReplyToInt(reply);
		}
		
		public T GetSet<T, K, V>(K key, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			string[] reply = ExecuteCommand(RedisCommand.GETSET, ChangeTypeToString<K>(key), ChangeTypeToString<V>(val));
			return ChangeStringToType<T>(ConvertReplyToString(reply));
		}
		
		public void SetMultipleKeysIfNotExist<K>(params K[] keys)
		{
			ValidateArguments<K>(keys);
			ExecuteCommand(RedisCommand.MSETNX, ChangeTypeArrayToString<K>(keys));
		}
		
		public void _SetMultipleKeysIfNotExist<K>(params K[] keys)
		{
			ValidateArguments<K>(keys);
			EnqueueAndSendCommand(RedisCommand.MSETNX, ChangeTypeArrayToString<K>(keys));
		}
		
		public void SetValueAtOffset<K, V>(K key, int offset, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			ExecuteCommand(RedisCommand.SETRANGE, ChangeTypeToString<K>(key), Convert.ToString(offset), ChangeTypeToString<V>(val));
		}
		
		public void _SetValueAtOffset<K, V>(K key, int offset, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			EnqueueAndSendCommand(RedisCommand.SETRANGE, ChangeTypeToString<K>(key), Convert.ToString(offset), ChangeTypeToString<V>(val));
		}
		
		public int DecrementBy<K>(K key, int decrementOffset)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.DECRBY, ChangeTypeToString<K>(key), Convert.ToString(decrementOffset));
			return ConvertReplyToInt(reply);
		}
		
		public int Increment<K>(K key)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.INCR, ChangeTypeToString<K>(key));
			return ConvertReplyToInt(reply);
		}
		
		public int Strlen<K>(K key)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.STRLEN, ChangeTypeToString<K>(key));
			return ConvertReplyToInt(reply);
		}
		
		public int IncrementBy<K>(K key, int incrementOffset)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.INCRBY, ChangeTypeToString<K>(key), Convert.ToString(incrementOffset));
			return ConvertReplyToInt(reply);
		}
		
		public int SetBitAtOffset<K>(K key, int offset, string val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<string>(val);
			string[] reply = ExecuteCommand(RedisCommand.SETBIT, ChangeTypeToString<K>(key), Convert.ToString(offset), val);
			return ConvertReplyToInt(reply);
		}
		
		public void _SetBitAtOffset<K>(K key, int offset, string val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<string>(val);
			EnqueueAndSendCommand(RedisCommand.SETBIT, ChangeTypeToString<K>(key), Convert.ToString(offset), val);
		}
		
		public int GetBitAtOffset<K>(K key, int offset)
		{
			ValidateArguments<K>(key);
			string[] reply = ExecuteCommand(RedisCommand.GETBIT, ChangeTypeToString<K>(key), Convert.ToString(offset));
			return ConvertReplyToInt(reply);
		}
		
		public T[] MultiGet<T, K>(params K[] keys)
		{
			ValidateArguments<K>(keys);
			string[] reply = ExecuteCommand(RedisCommand.MGET, ChangeTypeArrayToString<K>(keys));
			return ChangeStringArrayToType<T>(reply);
		}
		
		public Constants.StatusCode SetKeyWithExpiry<K, V>(K key, int timeoutSec, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			string[] reply = ExecuteCommand(RedisCommand.SETEX, ChangeTypeToString<K>(key), Convert.ToString(timeoutSec), ChangeTypeToString<V>(val));
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public void _SetKeyWithExpiry<K, V>(K key, int timeoutSec, V val)
		{
			ValidateArguments<K>(key);
			ValidateArguments<V>(val);
			EnqueueAndSendCommand(RedisCommand.SETEX, ChangeTypeToString<K>(key), Convert.ToString(timeoutSec), ChangeTypeToString<V>(val));
		}
		
		public int HashDeleteFields(string key, params string[] fields)
		{
			ValidateArguments(key);
			ValidateArguments(fields);
			string[] commandParams = new string[fields.Length + 1];
			commandParams[0] = key;
			Array.Copy(fields, 0, commandParams, 1, fields.Length);
			string[] reply = ExecuteCommand(RedisCommand.HDEL, commandParams);
			return ConvertReplyToInt(reply);
		}
		
		public void _HashDeleteFields(string key, params string[] fields)
		{
			ValidateArguments(key);
			ValidateArguments(fields);
			string[] commandParams = new string[fields.Length + 1];
			commandParams[0] = key;
			Array.Copy(fields, 0, commandParams, 1, fields.Length);
			EnqueueAndSendCommand(RedisCommand.HDEL, commandParams);
		}
		
		public Hashtable HashGetAllFieldsAndValues(string key)
		{
			ValidateArguments(key);
			string[] data = ExecuteCommand(RedisCommand.HGETALL, key);
			Hashtable fields = new Hashtable();
			for(int index = 0; index < data.Length; index += 2)
				fields.Add(data[index], data[index + 1]);
			
			return fields;
		}
		
		public int HashGetFieldCount(string key)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.HLEN, key);
			return ConvertReplyToInt(reply);
		}
		
		public int HashSetField(string key, string field, string val)
		{
			ValidateArguments(key, field, val);
			string[] reply = ExecuteCommand(RedisCommand.HSET, key, field, val);
			return ConvertReplyToInt(reply);
		}
		
		public void _HashSetField(string key, string field, string val)
		{
			ValidateArguments(key, field, val);
			EnqueueAndSendCommand(RedisCommand.HSET, key, field, val);
		}
		
		public bool HashIfFieldExist(string key, string field)
		{
			ValidateArguments(key, field);
			string[] reply = ExecuteCommand(RedisCommand.HEXISTS, key, field);
			return ConvertReplyToBool(reply);
		}
		
		public int HashIncrementFieldBy(string key, string field, int incrementby)
		{
			ValidateArguments(key, field);
			string[] reply = ExecuteCommand(RedisCommand.HINCRBY, key, field, Convert.ToString(incrementby));
			return ConvertReplyToInt(reply);
		}
		
		public string[] HashGetMultipleFieldsValue(string key, params string[] fields)
		{
			ValidateArguments(key);
			ValidateArguments(fields);
			string[] commandParams = new string[fields.Length + 1];
			commandParams[0] = key;
			Array.Copy(fields, 0, commandParams, 1, fields.Length);
			return ExecuteCommand(RedisCommand.HMGET, commandParams);
		}
		
		public int HashSetFieldIfNotExist(string key, string field, string val)
		{
			ValidateArguments(key, field, val);
			string[] reply = ExecuteCommand(RedisCommand.HSETNX, key, field, val);
			return ConvertReplyToInt(reply);
		}
		
		public void _HashSetFieldIfNotExist(string key, string field, string val)
		{
			ValidateArguments(key, field, val);
			EnqueueAndSendCommand(RedisCommand.HSETNX, key, field, val);
		}
		
		public string HashGetFieldValue(string key, string field)
		{
			ValidateArguments(key, field);
			string[] reply = ExecuteCommand(RedisCommand.HGET, key, field);
			return ConvertReplyToString(reply);
		}
		
		public string[] HashGetAllFields(string key)
		{
			ValidateArguments(key);
			return ExecuteCommand(RedisCommand.HKEYS, key);
		}
		
		public Constants.StatusCode HashSetMultipleFieldsValue(string key, Hashtable fieldValues)
		{
			string[] commandParams = new string[fieldValues.Count * 2 + 1];
			int index = 0;
			commandParams[index++] = key;
			foreach(string field in fieldValues.Keys)
			{
				commandParams[index++] = field;
				commandParams[index++] = fieldValues[field] as string;
			}
			ValidateArguments(commandParams);
			string[] reply = ExecuteCommand(RedisCommand.HMSET, commandParams);
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public void _HashSetMultipleFieldsValue(string key, Hashtable fieldValues)
		{
			string[] commandParams = new string[fieldValues.Count * 2 + 1];
			int index = 0;
			commandParams[index++] = key;
			foreach(string field in fieldValues.Keys)
			{
				commandParams[index++] = field;
				commandParams[index++] = fieldValues[field] as string;
			}
			ValidateArguments(commandParams);
			EnqueueAndSendCommand(RedisCommand.HMSET, commandParams);
		}
		
		public string[] HashGetAllValues(string key)
		{
			ValidateArguments(key);
			return ExecuteCommand(RedisCommand.HVALS, key);
		}
		
		/// <summary>
		/// Returns the left most element of the first non-empty list encountered.
		/// If all of the specified lists are empty, the connection is blocked until an element
		/// is inserted using LPUSH or RPUSH or timeout expires.
		/// A timeout of zero can be used to block indefinitely.
		/// </summary>
		/// <returns>
		/// The pop left blocking.
		/// </returns>
		/// <param name='timeoutSeconds'>
		/// Timeout seconds.
		/// </param>
		/// <param name='keys'>
		/// Keys.
		/// </param>
		public string[] ListPopLeftBlocking(int timeoutSeconds, params string[] keys)
		{
			string[] commandParams = new string[keys.Length + 1];
			Array.Copy(keys, 0, commandParams, 0, keys.Length);
			commandParams[keys.Length] = Convert.ToString(timeoutSeconds);
			ValidateArguments(commandParams);
			return ExecuteCommand(RedisCommand.BLPOP, commandParams);
		}
		
		public int ListLength(string key)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.LLEN, key);
			return ConvertReplyToInt(reply);
		}
		
		public int ListRemoveElementByCount(string key, int count, string val)
		{
			ValidateArguments(key, Convert.ToString(count), val);
			string[] reply = ExecuteCommand(RedisCommand.LREM, key, Convert.ToString(count), val);
			return ConvertReplyToInt(reply);
		}
		
		public void _ListRemoveElementByCount(string key, int count, string val)
		{
			ValidateArguments(key, Convert.ToString(count), val);
			EnqueueAndSendCommand(RedisCommand.LREM, key, Convert.ToString(count), val);
		}
		
		public int ListRightPush(string key, string val)
		{
			ValidateArguments(key, val);
			string[] reply = ExecuteCommand(RedisCommand.RPUSH, key, val);
			return ConvertReplyToInt(reply);
		}
		
		public void _ListRightPush(string key, string val)
		{
			ValidateArguments(key, val);
			EnqueueAndSendCommand(RedisCommand.RPUSH, key, val);
		}
		
		/// <summary>
		/// Returns the right most element of the first non-empty list encountered.
		/// If all of the specified lists are empty, the connection is blocked until an element
		/// is inserted using LPUSH or RPUSH or timeout expires.
		/// A timeout of zero can be used to block indefinitely.
		/// </summary>
		/// <returns>
		/// The pop right blocking.
		/// </returns>
		/// <param name='timeoutSeconds'>
		/// Timeout seconds.
		/// </param>
		/// <param name='keys'>
		/// Keys.
		/// </param>
		public string[] ListPopRightBlocking(int timeoutSeconds, params string[] keys)
		{
			string[] commandParams = new string[keys.Length + 1];
			Array.Copy(keys, 0, commandParams, 0, keys.Length);
			commandParams[keys.Length] = Convert.ToString(timeoutSeconds);
			ValidateArguments(commandParams);
			return ExecuteCommand(RedisCommand.BRPOP, commandParams);
		}
		
		public string ListLeftPop(string key)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.LPOP, key);
			return ConvertReplyToString(reply);
		}
		
		public Constants.StatusCode ListSetValueAtIndex(string key, int index, string val)
		{
			ValidateArguments(key, val);
			string[] reply = ExecuteCommand(RedisCommand.LSET, key, Convert.ToString(index), val);
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public void _ListSetValueAtIndex(string key, int index, string val)
		{
			ValidateArguments(key, val);
			EnqueueAndSendCommand(RedisCommand.LSET, key, Convert.ToString(index), val);
		}
		
		public int ListRightPushIfExist(string key, string val)
		{
			ValidateArguments(key, val);
			string[] reply = ExecuteCommand(RedisCommand.RPUSHX, key, val);
			return ConvertReplyToInt(reply);
		}
		
		public void _ListRightPushIfExist(string key, string val)
		{
			ValidateArguments(key, val);
			EnqueueAndSendCommand(RedisCommand.RPUSHX, key, val);
		}
		
		public string ListRightPopLeftPushBlocking(string srcKey, string destKey, int timeout)
		{
			ValidateArguments(srcKey, destKey);
			string[] reply = ExecuteCommand(RedisCommand.BRPOPLPUSH, srcKey, destKey, Convert.ToString(timeout));
			return ConvertReplyToString(reply);
		}
		
		public int ListLeftPush(string key, string val)
		{
			ValidateArguments(key, val);
			string[] reply = ExecuteCommand(RedisCommand.LPUSH, key, val);
			return ConvertReplyToInt(reply);
		}
		
		public void _ListLeftPush(string key, string val)
		{
			ValidateArguments(key, val);
			EnqueueAndSendCommand(RedisCommand.LPUSH, key, val);
		}
		
		public Constants.StatusCode ListLeftTrim(string key, int start, int stop)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.LTRIM, key, Convert.ToString(start), Convert.ToString(stop));
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public void _ListLeftTrim(string key, int start, int stop)
		{
			ValidateArguments(key);
			EnqueueAndSendCommand(RedisCommand.LTRIM, key, Convert.ToString(start), Convert.ToString(stop));
		}
		
		public string ListGetElementAtIndex(string key, int index)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.LINDEX, key, Convert.ToString(index));
			return ConvertReplyToString(reply);
		}
		
		public int ListLeftPushIfExist(string key, string val)
		{
			ValidateArguments(key, val);
			string[] reply = ExecuteCommand(RedisCommand.LPUSHX, key, val);
			return ConvertReplyToInt(reply);
		}
		
		public void _ListLeftPushIfExist(string key, string val)
		{
			ValidateArguments(key, val);
			EnqueueAndSendCommand(RedisCommand.LPUSHX, key, val);
		}
		
		public string ListRightPop(string key)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.RPOP, key);
			return ConvertReplyToString(reply);
		}
		
		public int ListInsertWithPivot(string key, Constants.ListInsert direction, string pivot, string val)
		{
			ValidateArguments(key, pivot, val);
			string[] reply = ExecuteCommand(RedisCommand.LINSERT, key, direction.ToString(), pivot, val);
			return ConvertReplyToInt(reply);
		}
		
		public void _ListInsertWithPivot(string key, Constants.ListInsert direction, string pivot, string val)
		{
			ValidateArguments(key, pivot, val);
			EnqueueAndSendCommand(RedisCommand.LINSERT, key, direction.ToString(), pivot, val);
		}
		
		public string[] ListGetElementsByRange(string key, int start, int stop)
		{
			ValidateArguments(key);
			return ExecuteCommand(RedisCommand.LRANGE, key, Convert.ToString(start), Convert.ToString(stop));
		}
		
		public string ListRightPopLeftPush(string srcKey, string destKey)
		{
			ValidateArguments(srcKey, destKey);
			string[] reply = ExecuteCommand(RedisCommand.BRPOPLPUSH, srcKey, destKey);
			return ConvertReplyToString(reply);
		}
		
		public int SETAddMembers(string key, params string[] members)
		{
			string[] commandParams = new string[members.Length + 1];
			commandParams[0] = key;
			Array.Copy(members, 0, commandParams, 1, members.Length);
			ValidateArguments(commandParams);
			string[] reply = ExecuteCommand(RedisCommand.SADD, commandParams);
			return ConvertReplyToInt(reply);
		}
		
		public void _SETAddMembers(string key, params string[] members)
		{
			string[] commandParams = new string[members.Length + 1];
			commandParams[0] = key;
			Array.Copy(members, 0, commandParams, 1, members.Length);
			ValidateArguments(commandParams);
			EnqueueAndSendCommand(RedisCommand.SADD, commandParams);
		}
		
		public string[] SETFindIntersection(params string[] keys)
		{
			ValidateArguments(keys);
			return ExecuteCommand(RedisCommand.SINTER, keys);
		}
		
		public int SETMoveMember(string srcSet, string destSet, string member)
		{
			ValidateArguments(srcSet, destSet, member);
			string[] reply = ExecuteCommand(RedisCommand.SMOVE, srcSet, destSet, member);
			return ConvertReplyToInt(reply);
		}
		
		public void _SETMoveMember(string srcSet, string destSet, string member)
		{
			ValidateArguments(srcSet, destSet, member);
			EnqueueAndSendCommand(RedisCommand.SMOVE, srcSet, destSet, member);
		}
		
		public string[] SETFindUnion(params string[] keys)
		{
			ValidateArguments(keys);
			return ExecuteCommand(RedisCommand.SUNION, keys);
		}
		
		public int SETTotalMembers(string key)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.SCARD, key);
			return ConvertReplyToInt(reply);
		}
		
		public int SETFindIntersectAndStore(string destKey, params string[] srcKeys)
		{
			string[] commandParams = new string[srcKeys.Length + 1];
			commandParams[0] = destKey;
			Array.Copy(srcKeys, 0, commandParams, 1, srcKeys.Length);
			ValidateArguments(commandParams);
			string[] reply = ExecuteCommand(RedisCommand.SINTERSTORE, commandParams);
			return ConvertReplyToInt(reply);
		}
		
		public void _SETFindIntersectAndStore(string destKey, params string[] srcKeys)
		{
			string[] commandParams = new string[srcKeys.Length + 1];
			commandParams[0] = destKey;
			Array.Copy(srcKeys, 0, commandParams, 1, srcKeys.Length);
			ValidateArguments(commandParams);
			EnqueueAndSendCommand(RedisCommand.SINTERSTORE, commandParams);
		}
		
		public string SETPopElement(string key)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.SPOP, key);
			return ConvertReplyToString(reply);
		}
		
		public int SETFindUnionAndStore(string destKey, params string[] srcKeys)
		{
			string[] commandParams = new string[srcKeys.Length + 1];
			commandParams[0] = destKey;
			Array.Copy(srcKeys, 0, commandParams, 1, srcKeys.Length);
			ValidateArguments(commandParams);
			string[] reply = ExecuteCommand(RedisCommand.SUNIONSTORE, commandParams);
			return ConvertReplyToInt(reply);
		}
		
		public void _SETFindUnionAndStore(string destKey, params string[] srcKeys)
		{
			string[] commandParams = new string[srcKeys.Length + 1];
			commandParams[0] = destKey;
			Array.Copy(srcKeys, 0, commandParams, 1, srcKeys.Length);
			ValidateArguments(commandParams);
			EnqueueAndSendCommand(RedisCommand.SUNIONSTORE, commandParams);
		}
		
		public string[] SETFindDifference(params string[] keys)
		{
			ValidateArguments(keys);
			return ExecuteCommand(RedisCommand.SDIFF, keys);
		}
		
		public bool SETIsMember(string key, string member)
		{
			ValidateArguments(key, member);
			string[] reply = ExecuteCommand(RedisCommand.SISMEMBER, key, member);
			return ConvertReplyToBool(reply);
		}
		
		public string SETGetRandomMember(string key)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.SRANDMEMBER, key);
			return ConvertReplyToString(reply);
		}
		
		public int SETFindDifferenceAndStore(params string[] keys)
		{
			ValidateArguments(keys);
			string[] reply = ExecuteCommand(RedisCommand.SDIFF, keys);
			return ConvertReplyToInt(reply);
		}
		
		public void _SETFindDifferenceAndStore(params string[] keys)
		{
			ValidateArguments(keys);
			EnqueueAndSendCommand(RedisCommand.SDIFF, keys);
		}
		
		public string[] SETGetAllMembers(string key)
		{
			ValidateArguments(key);
			return ExecuteCommand(RedisCommand.SMEMBERS, key);
		}
		
		public bool SETRemoveMember(string key, string member)
		{
			ValidateArguments(key, member);
			string[] reply = ExecuteCommand(RedisCommand.SREM, key, member);
			return ConvertReplyToBool(reply);
		}
		
		public void _SETRemoveMember(string key, string member)
		{
			ValidateArguments(key, member);
			EnqueueAndSendCommand(RedisCommand.SREM, key, member);
		}
		
		public int SSAddMemberWithScore(string key, double score, string member)
		{
			ValidateArguments(key, member);
			score = Math.Round(score, 2);
			string[] reply = ExecuteCommand(RedisCommand.ZADD, key, Convert.ToString(score), member);
			return ConvertReplyToInt(reply);
		}
		
		public void _SSAddMemberWithScore(string key, double score, string member)
		{
			ValidateArguments(key, member);
			score = Math.Round(score, 2);
			EnqueueAndSendCommand(RedisCommand.ZADD, key, Convert.ToString(score), member);
		}
		
		public int SSIntersection(string destKey, int numKeys, string[] keys, string[] weights, Constants.SSResultScore resultType)
		{
			string[] commandParams = new string[keys.Length + weights.Length + 5];
			commandParams[0] = destKey;
			commandParams[1] = Convert.ToString(numKeys);
			Array.Copy(keys, 0, commandParams, 2, keys.Length);
			commandParams[keys.Length + 2] = Constants.SSParam.WEIGHT.ToString();
			Array.Copy(weights, 0, commandParams, keys.Length + 3, weights.Length);
			commandParams[keys.Length + weights.Length + 3] = Constants.SSParam.AGGREGATE.ToString();
			commandParams[keys.Length + weights.Length + 4] = resultType.ToString();
			ValidateArguments(commandParams);
			string[] reply = ExecuteCommand(RedisCommand.ZINTERSTORE, commandParams);
			return ConvertReplyToInt(reply);
		}
		
		public void _SSIntersection(string destKey, int numKeys, string[] keys, string[] weights, Constants.SSResultScore resultType)
		{
			string[] commandParams = new string[keys.Length + weights.Length + 5];
			commandParams[0] = destKey;
			commandParams[1] = Convert.ToString(numKeys);
			Array.Copy(keys, 0, commandParams, 2, keys.Length);
			commandParams[keys.Length + 2] = Constants.SSParam.WEIGHT.ToString();
			Array.Copy(weights, 0, commandParams, keys.Length + 3, weights.Length);
			commandParams[keys.Length + weights.Length + 3] = Constants.SSParam.AGGREGATE.ToString();
			commandParams[keys.Length + weights.Length + 4] = resultType.ToString();
			ValidateArguments(commandParams);
			EnqueueAndSendCommand(RedisCommand.ZINTERSTORE, commandParams);
		}
		
		public int SSRemoveMember(string key, string member)
		{
			ValidateArguments(key, member);
			string[] reply = ExecuteCommand(RedisCommand.ZREM, key, member);
			return ConvertReplyToInt(reply);
		}
		
		public void _SSRemoveMember(string key, string member)
		{
			ValidateArguments(key, member);
			EnqueueAndSendCommand(RedisCommand.ZREM, key, member);
		}
		
		public string[] SSGetElementByScoreRangeDescending(string key, int minScore, int maxScore, bool withScores)
		{
			ValidateArguments(key);
			if(withScores)
				return ExecuteCommand(RedisCommand.ZREVRANGEBYSCORE, key, Convert.ToString(minScore), Convert.ToString(maxScore), Constants.SSParam.WITHSCORES.ToString());
			else
				return ExecuteCommand(RedisCommand.ZREVRANGEBYSCORE, key, Convert.ToString(minScore), Convert.ToString(maxScore));
		}
		
		public string[] SSGetElementsByScoreRangeDescending(string key, int minScore, int maxScore, bool withScores, int offset, int count)
		{
			ValidateArguments(key);
			if(withScores)
				return ExecuteCommand(RedisCommand.ZREVRANGEBYSCORE, key, Convert.ToString(minScore), Convert.ToString(maxScore), Constants.SSParam.WITHSCORES.ToString(), Constants.SSParam.LIMIT.ToString(), Convert.ToString(offset), Convert.ToString(count));
			else
				return ExecuteCommand(RedisCommand.ZREVRANGEBYSCORE, key, Convert.ToString(minScore), Convert.ToString(maxScore), Constants.SSParam.LIMIT.ToString(), Convert.ToString(offset), Convert.ToString(count));
		}
		
		public int SSGetCardinality(string key)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.ZCARD, key);
			return ConvertReplyToInt(reply);
		}
		
		public string[] SSGetElementsByIndexRangeAscending(string key, int minIndex, int maxIndex, bool withScores)
		{
			ValidateArguments(key);
			if(withScores)
				return ExecuteCommand(RedisCommand.ZRANGE, key, Convert.ToString(minIndex), Convert.ToString(maxIndex), Constants.SSParam.WITHSCORES.ToString());
			else
				return ExecuteCommand(RedisCommand.ZRANGE, key, Convert.ToString(minIndex), Convert.ToString(maxIndex));
		}
		
		public int SSRemoveElementsByIndexRange(string key, int minIndex, int maxIndex)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.ZREMRANGEBYRANK, key, Convert.ToString(minIndex), Convert.ToString(maxIndex));
			return ConvertReplyToInt(reply);
		}
		
		public void _SSRemoveElementsByIndexRange(string key, int minIndex, int maxIndex)
		{
			ValidateArguments(key);
			EnqueueAndSendCommand(RedisCommand.ZREMRANGEBYRANK, key, Convert.ToString(minIndex), Convert.ToString(maxIndex));
		}
		
		public string SSGetRankDescending(string key, string member)
		{
			ValidateArguments(key, member);
			string[] reply = ExecuteCommand(RedisCommand.ZREVRANK, key, member);
			return ConvertReplyToString(reply);
		}
		
		public int SSGetCountByScoreRange(string key, int minScore, int maxScore)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.ZCOUNT, key, Convert.ToString(minScore), Convert.ToString(maxScore));
			return ConvertReplyToInt(reply);
		}
		
		public string[] SSGetElementByScoreRangeAscending(string key, int minScore, int maxScore, bool withScores)
		{
			ValidateArguments(key);
			if(withScores)
				return ExecuteCommand(RedisCommand.ZRANGEBYSCORE, key, Convert.ToString(minScore), Convert.ToString(maxScore), Constants.SSParam.WITHSCORES.ToString());
			else
				return ExecuteCommand(RedisCommand.ZRANGEBYSCORE, key, Convert.ToString(minScore), Convert.ToString(maxScore));
		}
		
		public string[] SSGetElementsByScoreRangeAscending(string key, int minScore, int maxScore, bool withScores, int offset, int count)
		{
			ValidateArguments(key);
			if(withScores)
				return ExecuteCommand(RedisCommand.ZRANGEBYSCORE, key, Convert.ToString(minScore), Convert.ToString(maxScore), Constants.SSParam.WITHSCORES.ToString(), Constants.SSParam.LIMIT.ToString(), Convert.ToString(offset), Convert.ToString(count));
			else
				return ExecuteCommand(RedisCommand.ZRANGEBYSCORE, key, Convert.ToString(minScore), Convert.ToString(maxScore), Constants.SSParam.LIMIT.ToString(), Convert.ToString(offset), Convert.ToString(count));
		}
		
		public int SSRemoveElementsByScoreRange(string key, int minScore, int maxScore)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.ZREMRANGEBYSCORE, key, Convert.ToString(minScore), Convert.ToString(maxScore));
			return ConvertReplyToInt(reply);
		}
		
		public void _SSRemoveElementsByScoreRange(string key, int minScore, int maxScore)
		{
			ValidateArguments(key);
			EnqueueAndSendCommand(RedisCommand.ZREMRANGEBYSCORE, key, Convert.ToString(minScore), Convert.ToString(maxScore));
		}
		
		public string SSGetScore(string key, string member)
		{
			ValidateArguments(key, member);
			string[] reply = ExecuteCommand(RedisCommand.ZSCORE, key, member);
			return ConvertReplyToString(reply);
		}
		
		public string SSIncrementScore(string key, int incrementBy, string member)
		{
			ValidateArguments(key, member);
			string[] reply = ExecuteCommand(RedisCommand.ZINCRBY, key, Convert.ToString(incrementBy), member);
			return ConvertReplyToString(reply);
		}
		
		public string SSGetRankAscending(string key, string member)
		{
			ValidateArguments(key, member);
			string[] reply = ExecuteCommand(RedisCommand.ZRANK, key, member);
			return ConvertReplyToString(reply);
		}
		
		public string[] SSGetElementsByIndexRangeDescending(string key, int minIndex, int maxIndex, bool withScores)
		{
			ValidateArguments(key);
			if(withScores)
				return ExecuteCommand(RedisCommand.ZREVRANGE, key, Convert.ToString(minIndex), Convert.ToString(maxIndex), Constants.SSParam.WITHSCORES.ToString());
			else
				return ExecuteCommand(RedisCommand.ZREVRANGE, key, Convert.ToString(minIndex), Convert.ToString(maxIndex));
		}
		
		public int SSUnion(string destKey, int numKeys, string[] keys, string[] weights, Constants.SSResultScore resultType)
		{
			string[] commandParams = new string[keys.Length + weights.Length + 5];
			commandParams[0] = destKey;
			commandParams[1] = Convert.ToString(numKeys);
			Array.Copy(keys, 0, commandParams, 2, keys.Length);
			commandParams[keys.Length + 2] = Constants.SSParam.WEIGHT.ToString();
			Array.Copy(weights, 0, commandParams, keys.Length + 3, weights.Length);
			commandParams[keys.Length + weights.Length + 3] = Constants.SSParam.AGGREGATE.ToString();
			commandParams[keys.Length + weights.Length + 4] = resultType.ToString();
			ValidateArguments(commandParams);
			string[] reply = ExecuteCommand(RedisCommand.ZUNIONSTORE, commandParams);
			return ConvertReplyToInt(reply);
		}
		
		public void _SSUnion(string destKey, int numKeys, string[] keys, string[] weights, Constants.SSResultScore resultType)
		{
			string[] commandParams = new string[keys.Length + weights.Length + 5];
			commandParams[0] = destKey;
			commandParams[1] = Convert.ToString(numKeys);
			Array.Copy(keys, 0, commandParams, 2, keys.Length);
			commandParams[keys.Length + 2] = Constants.SSParam.WEIGHT.ToString();
			Array.Copy(weights, 0, commandParams, keys.Length + 3, weights.Length);
			commandParams[keys.Length + weights.Length + 3] = Constants.SSParam.AGGREGATE.ToString();
			commandParams[keys.Length + weights.Length + 4] = resultType.ToString();
			ValidateArguments(commandParams);
			EnqueueAndSendCommand(RedisCommand.ZUNIONSTORE, commandParams);
		}
		
		public void RollbackTransaction()
		{
			EnqueueAndSendCommand(RedisCommand.DISCARD);	
		}
		
		public void BeginTransaction()
		{
			isInTransaction = true;
			EnqueueAndSendCommand(RedisCommand.MULTI);	
		}
		
		public string[] CommitTransaction()
		{
			isInTransaction = false;
			return ExecuteCommand(RedisCommand.EXEC);
		}
		
		public void AddWatch(params string[] keys)
		{
			ValidateArguments(keys);
			EnqueueAndSendCommand(RedisCommand.WATCH, keys);
		}
		
		public void RemoveAllWatch()
		{
			EnqueueAndSendCommand(RedisCommand.UNWATCH);
		}
		
		public string Authenticate(string password)
		{
			ValidateArguments(password);
			string[] reply = ExecuteCommand(RedisCommand.AUTH, password);
			return ConvertReplyToString(reply);
		}
		
		public string IsAlive()
		{
			string[] reply = ExecuteCommand(RedisCommand.PING);
			return ConvertReplyToString(reply);
		}
		
		public Constants.StatusCode ChangeDB(int index)
		{
			string[] reply = ExecuteCommand(RedisCommand.SELECT, Convert.ToString(index));
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		/// <summary>
		/// Connection is closed as soon as all the pending replies are written to the client.
		/// </summary>
		public void Close()
		{
			ExecuteCommand(RedisCommand.QUIT);	
		}
		
		public void RewriteAppendOnlyFile()
		{
			ExecuteCommand(RedisCommand.BGREWRITEAOF);
		}
		
		public int GetDBSize()
		{
			string[] reply = ExecuteCommand(RedisCommand.DBSIZE);
			return ConvertReplyToInt(reply);
		}
		
		public string GetServerInfo()
		{
			string[] reply = ExecuteCommand(RedisCommand.INFO);
			return ConvertReplyToString(reply);
		}
		
		public Constants.StatusCode ChangeMaster(string host, int port)
		{
			string[] reply = ExecuteCommand(RedisCommand.SLAVEOF, host, Convert.ToString(port));
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public Constants.StatusCode SaveInBackground()
		{
			string[] reply = ExecuteCommand(RedisCommand.BGSAVE);
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public string GetDebugInfo(string key)
		{
			ValidateArguments(key);
			string[] reply = ExecuteCommand(RedisCommand.DEBUG, RedisSubCommand.OBJECT.ToString(), key);
			return ConvertReplyToString(reply);
		}
		
		public int GetLastSaveTimestamp()
		{
			string[] reply = ExecuteCommand(RedisCommand.LASTSAVE);
			return ConvertReplyToInt(reply);
		}
		
		public string GetConfigParams(string pattern)
		{
			string[] reply = ExecuteCommand(RedisCommand.CONFIG, RedisSubCommand.GET.ToString(), pattern);
			return ConvertReplyToString(reply);
		}
		
		public Constants.StatusCode SetConfigParam(string parameter, string val)
		{
			ValidateArguments(parameter, val);
			string[] reply = ExecuteCommand(RedisCommand.CONFIG, RedisSubCommand.SET.ToString(), parameter, val);
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public Constants.StatusCode ClearAllDB()
		{
			string[] reply = ExecuteCommand(RedisCommand.FLUSHALL);	
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public Constants.StatusCode ResetStats()
		{
			string[] reply = ExecuteCommand(RedisCommand.CONFIG, RedisSubCommand.RESETSTAT.ToString());
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public Constants.StatusCode ClearCurrentDB()
		{
			string[] reply = ExecuteCommand(RedisCommand.FLUSHDB);	
			return (Constants.StatusCode)Enum.Parse(typeof(Constants.StatusCode), ConvertReplyToString(reply), true);
		}
		
		public void Publish(string channel, string message)
		{
			ValidateArguments(channel, message);
			SendCommand(RedisCommand.PUBLISH, channel, message);
		}
		
		private void StartDataListener()
		{
			dataListener = new Thread(new ThreadStart(this.Listen));
			dataListener.Start();
		}
		
		private void StopDataListener()
		{
			if(dataListener != null)
			{
				try
				{
					dataListener.Abort();
					dataListener = null;
				}
				catch(ThreadAbortException)
				{
					//Do nothing. Nothing to clean up.
				}
			}
		}
		
		public void Listen()
		{
			while(true)
			{
				if(redisStream.IsDataAvailable() && sendCommanSyncQueue.Count > 0)
				{
					redisStream.HandleReply();
					sendCommanSyncQueue.Dequeue();
				}
			}
		}
	}
}