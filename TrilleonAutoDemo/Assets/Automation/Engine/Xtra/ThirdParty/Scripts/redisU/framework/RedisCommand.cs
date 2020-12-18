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

namespace redisU.framework
{
	public enum RedisCommand
	{
		//Keys Commands
		DEL,
		KEYS,
		RANDOMKEY,
		TTL,
		EXISTS,
		MOVE,
		RENAME,
		TYPE,
		EXPIRE,
		OBJECT,
		RENAMENX,
		EXPIREAT,
		PERSIST,
		SORT,
		//Strings Commands
		APPEND,
		GETRANGE,
		MSET,
		SETNX,
		DECR,
		GETSET,
		MSETNX,
		SETRANGE,
		DECRBY,
		INCR,
		SET,
		STRLEN,
		GET,
		INCRBY,
		SETBIT,
		GETBIT,
		MGET,
		SETEX,
		//Hashes
		HDEL,
		HGETALL,
		HLEN,
		HSET,
		HEXISTS,
		HINCRBY,
		HMGET,
		HSETNX,
		HGET,
		HKEYS,
		HMSET,
		HVALS,
		//Lists
		BLPOP,
		LLEN,
		LREM,
		RPUSH,
		BRPOP,
		LPOP,
		LSET,
		RPUSHX,
		BRPOPLPUSH,
		LPUSH,
		LTRIM,
		LINDEX,
		LPUSHX,
		RPOP,
		LINSERT,
		LRANGE,
		RPOPLPUSH,
		//Sets
		SADD,
		SINTER,
		SMOVE,
		SUNION,
		SCARD,
		SINTERSTORE,
		SPOP,
		SUNIONSTORE,
		SDIFF,
		SISMEMBER,
		SRANDMEMBER,
		SDIFFSTORE,
		SMEMBERS,
		SREM,
		//SortedSets
		ZADD,
		ZINTERSTORE,
		ZREM,
		ZREVRANGEBYSCORE,
		ZCARD,
		ZRANGE,
		ZREMRANGEBYRANK,
		ZREVRANK,
		ZCOUNT,
		ZRANGEBYSCORE,
		ZREMRANGEBYSCORE,
		ZSCORE,
		ZINCRBY,
		ZRANK,
		ZREVRANGE,
		ZUNIONSTORE,
		//Pub/Sub
		PSUBSCRIBE,
		PUNSUBSCRIBE,
		UNSUBSCRIBE,
		PUBLISH,
		SUBSCRIBE,
		//Transactions
		DISCARD,
		MULTI,
		WATCH,
		EXEC,
		UNWATCH,
		//CONNECTION
		AUTH,
		PING,
		SELECT,
		ECHO,
		QUIT,
		//Server
		BGREWRITEAOF,
		DBSIZE,
		INFO,
		SLAVEOF,
		BGSAVE,
		DEBUG,
		LASTSAVE,
		SLOWLOG,
		CONFIG,
		MONITOR,
		SYNC,
		FLUSHALL,
		SAVE,
		FLUSHDB,
		SHUTDOWN
	}
	
	public enum RedisSubCommand
	{
		REFCOUNT = 0,
		ENCODING,
		IDLETIME,
		OBJECT,
		GET,
		SEGFAULT,
		SET,
		RESETSTAT
	};
}

