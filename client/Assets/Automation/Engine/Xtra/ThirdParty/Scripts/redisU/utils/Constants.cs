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

namespace redisU.utils
{
	public sealed class Constants
	{
		public enum Stream
		{
			Synchronous = 0, //Default value
			Asynchronous = 1
		};
		
		public enum KeyType
		{
			STRING = 0,
			LIST,
			SET,
			ZSET,
			HASH,
			UNDEFINED
		};
		
		public enum ListInsert
		{
			BEFORE = 0,
			AFTER
		};
		
		public enum StatusCode
		{
			OK = 0
		};
		
		public enum SSParam
		{
			WEIGHT = 0,
			AGGREGATE,
			WITHSCORES,
			LIMIT
		};
		
		public enum SSResultScore
		{
			SUM = 0,
			MIN,
			MAX
		};
		
		public const string DEFAULT_REDIS_HOST = "localhost";
		public const int DEFAULT_REDIS_PORT = 6379;
		
		//Socket/NetworkStream timeout settings
		public const int SOCKET_SEND_TIMEOUT_MS = 1000;
		public const int SOCKET_RECEIVE_TIMEOUT_MS = 1000;
		public const int NS_READ_TIMEOUT_MS = 3000;
		public const int NS_WRITE_TIMEOUT_MS = 2000;
		
		//Data constants
		public const string NO_OP =  "0";
		
		//Redis command format options
		public const string CRLF = "\r\n";
		public const string NUM_ARGUMENTS = "*";
		public const string NUM_BYTES_ARGUMENT = "$";
		public const string REPLY_SINGLE_LINE = "+";
		public const string REPLY_ERROR = "-";
		public const string REPLY_INTEGER = ":";
		public const string REPLY_BULK = "$";
		public const string REPLY_MULTIBULK = "*";
		
		//Error Messages
		public const string ERR_MSG_EXECUTE_CMD = "Regular command not allowed when in subscribed mode.";
		public const string ERR_MSG_RECREATE_SUBSCRIPTION = "The previous subscription session was ended. Recreate the subscription to start listening for messages.";
	}
}

