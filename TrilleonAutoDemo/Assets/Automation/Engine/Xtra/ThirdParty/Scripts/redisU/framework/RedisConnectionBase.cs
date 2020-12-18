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
using System.Net;
using System.Net.Sockets;
using redisU.utils;

namespace redisU.framework
{
	public abstract class RedisConnectionBase
	{
		private Socket clientSocket = null;
		public RedisStream redisStream = null;
		public Queue sendCommanSyncQueue = null;
		protected bool isInTransaction = false;
		
		public RedisConnectionBase()
		{
			sendCommanSyncQueue = Queue.Synchronized(new Queue());	
		}
		
		protected void CreateConnection()
		{
			CreateConnection(Constants.DEFAULT_REDIS_HOST, Constants.DEFAULT_REDIS_PORT);
		}
		
		protected void CreateConnection(string host, int port)
		{
			clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			clientSocket.SendTimeout = Constants.SOCKET_SEND_TIMEOUT_MS;
			clientSocket.ReceiveTimeout = Constants.SOCKET_RECEIVE_TIMEOUT_MS;
			clientSocket.Connect(host, port);
			redisStream = new RedisSynchronousStream(clientSocket);
		}
		
		protected string BuildCommand(RedisCommand command, params string[] args)
		{
			StringBuilder commandBuilder = new StringBuilder();
			commandBuilder.Append(Constants.NUM_ARGUMENTS);
			commandBuilder.Append(args.Length + 1).Append(Constants.CRLF);
			
			commandBuilder.Append(Constants.NUM_BYTES_ARGUMENT);
			commandBuilder.Append(command.ToString().Length).Append(Constants.CRLF);
			commandBuilder.Append(command.ToString()).Append(Constants.CRLF);
			
			foreach(string arg in args)
			{
				commandBuilder.Append(Constants.NUM_BYTES_ARGUMENT);
				commandBuilder.Append(arg.Length).Append(Constants.CRLF);
				commandBuilder.Append(arg).Append(Constants.CRLF);
			}
			return commandBuilder.ToString();
		}
		
		protected string[] ExecuteCommand(RedisCommand command, params string[] args)
		{
			while(sendCommanSyncQueue.Count != 0);
			byte[] bytes = PrepareBytesToSend(command, args);
			return redisStream.GetResponse(bytes);
		}
		
		protected void SendCommand(RedisCommand command, params string[] args)
		{
			byte[] bytes = PrepareBytesToSend(command, args);
			redisStream.SendData(bytes);
		}
		
		protected void EnqueueAndSendCommand(RedisCommand command, params string[] args)
		{
			sendCommanSyncQueue.Enqueue(command);
			SendCommand(command, args);
		}
		
		protected byte[] PrepareBytesToSend(RedisCommand command, string[] args)
		{
			string data = BuildCommand(command, args);
			return Encoding.UTF8.GetBytes(data);
		}
		
		protected bool ConvertReplyToBool(string[] reply)
		{
			if(isInTransaction) return false;
			string retVal = ((reply != null && reply.Length > 0) ? reply[0] : null);
			return ((retVal == null || retVal.Equals(Constants.NO_OP)) ? false : true);
		}
		
		protected int ConvertReplyToInt(string[] reply)
		{
			if(isInTransaction) return -1;
			string retVal = ((reply != null && reply.Length > 0) ? reply[0] : null);
			return ((retVal == null) ? -1 : Convert.ToInt32(retVal));	
		}
		
		protected string ConvertReplyToString(string[] reply)
		{
			return ((reply != null && reply.Length > 0) ? reply[0] : null);
		}
		
		protected T ChangeStringToType<T>(string val)
		{
			return (T)Convert.ChangeType(val, typeof(T));
		}
		
		protected string ChangeTypeToString<T>(T val)
		{
			return (string)Convert.ChangeType(val, typeof(string));
		}
		
		protected string[] ChangeTypeArrayToString<T>(params T[] vals)
		{
			string[] args = new string[vals.Length];
			int index = 0;
			foreach(T val in vals)
				args[index++] = Convert.ToString(val);
			return args;
		}
			
		protected T[] ChangeStringArrayToType<T>(params string[] vals)
		{
			T[] args = new T[vals.Length];
			int index = 0;
			foreach(string val in vals)
				args[index++] = (T)Convert.ChangeType(val, typeof(T));
			return args;
		}
		
		protected void ValidateArguments<T>(params T[] args)
		{
			foreach(T argument in args)
				if(argument == null)
					throw new ArgumentNullException();
		}
		
		protected void ValidateArguments(params string[] args)
		{
			foreach(string argument in args)
				if(argument == null)
					throw new ArgumentNullException();
		}
		
		protected void Dispose()
		{
			if(redisStream != null)
			{
				redisStream.CloseConnection();
				redisStream = null;
			}
			if(clientSocket != null)
			{
				clientSocket.Close();
				clientSocket = null;
			}
		}
	}
}

