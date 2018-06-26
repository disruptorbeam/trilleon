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
using System.Text;
using System.Net.Sockets;
using redisU.utils;
using redisU.exceptions;

namespace redisU.framework
{
	public sealed class RedisSynchronousStream : RedisStream
	{
		private NetworkStream networkStream;
		
		public RedisSynchronousStream(Socket socket)
		{
			networkStream = new NetworkStream(socket);
			networkStream.ReadTimeout = Constants.NS_READ_TIMEOUT_MS;
			networkStream.WriteTimeout = Constants.NS_WRITE_TIMEOUT_MS;
		}
		
		public string[] GetResponse(byte[] bytes)
		{
			Flush();
			SendData(bytes);
			return WaitAndParseReply();
		}
		
		public void SendData(byte[] bytes)
		{
			networkStream.Write(bytes, 0, bytes.Length);
		}
		
		private string[] WaitAndParseReply()
		{
			string replyCode = ReadSingleByte();
			string[] result = null;
			
			if(replyCode.Equals(Constants.REPLY_BULK))
				result = new string[] {HandleBulkReply()};
			else if(replyCode.Equals(Constants.REPLY_MULTIBULK))
				result = HandleMultiBulkReply();
			else if(replyCode.Equals(Constants.REPLY_ERROR))
				HandleErrorReply();
			else if(replyCode.Equals(Constants.REPLY_INTEGER))
				result = new string[] {HandleIntegerReply()};
			else if(replyCode.Equals(Constants.REPLY_SINGLE_LINE))
				result = new string[] {HandleSingleLineReply()};
			
			return result;
		}
		
		public void HandleReply()
		{
			WaitAndParseReply();
		}
		
		public string HandleBulkReply()
		{
			int dataLength = Convert.ToInt32(Readline());
			if(dataLength < 0 || !IsDataAvailable())
				return null;
			
			byte[] data = new byte[dataLength];
			try
			{
				networkStream.Read(data, 0, data.Length);
				ReadSingleByte(); //Read Carriage Return (\r)
				ReadSingleByte(); //Read Line feed (\n)
			}
			catch(Exception)
			{
				//Console.WriteLine("Network Stream read timeout: " + e.Message);
				data = new byte[0];
			}
			return Encoding.UTF8.GetString(data);
		}
		
		public string[] HandleMultiBulkReply()
		{
			int numberOfBulkReplies = Convert.ToInt32(Readline());
			if(numberOfBulkReplies < 0)
				return null;
			string[] bulkReply = new string[numberOfBulkReplies];
			for(int index = 0; index < numberOfBulkReplies; index++)
			{	
				ReadSingleByte();
				bulkReply[index] = HandleBulkReply();
			}
			if(IsDataAvailable())
				ReadSingleByte();
			return bulkReply;
		}
		
		public void HandleErrorReply()
		{
			String errorMessage = Readline();
			Console.WriteLine(errorMessage);
			throw new RedisException(errorMessage);
		}
		
		public string HandleIntegerReply()
		{
			return Readline();
		}
		
		public string HandleSingleLineReply()
		{
			return Readline();
		}
		
		public string[] HandleChannelMessage()
		{	
			string[] channelMessageInfo = new string[3];
			for(int index = 0; index < channelMessageInfo.Length; index++)
			{
				string replyCode = ReadSingleByte();
				if(replyCode.Equals(string.Empty))
					return null;
				if(replyCode.Equals(Constants.REPLY_BULK))
					channelMessageInfo[index] = HandleBulkReply();
				else if(replyCode.Equals(Constants.REPLY_MULTIBULK))
				{
					channelMessageInfo = HandleMultiBulkReply();
					break;
				}
			}
			return channelMessageInfo;
		}
		
		public bool IsDataAvailable()
		{
			return networkStream.DataAvailable;	
		}
		
		private string Readline()
		{
			StringBuilder builder= new StringBuilder();
			string temp = string.Empty;
			while(true)
			{
				temp = ReadSingleByte();
				
				if(temp.Equals("\r"))
				{
					ReadSingleByte(); //Read \n
					break;
				}
				else
					builder.Append(temp);
			}
			return builder.ToString();
		}
		
		private string ReadSingleByte()
		{
			byte[] singleByteBuffer = new byte[1];
			try
			{
				networkStream.Read(singleByteBuffer, 0, singleByteBuffer.Length);
			}
			catch(Exception)
			{
				//Console.WriteLine("Network Stream read timeout: " + e.Message);
				singleByteBuffer = new byte[0];
			}
			return Encoding.UTF8.GetString(singleByteBuffer);
		}
		
		public void CloseConnection()
		{
			networkStream.Close();
			networkStream = null;
		}
		
		public void Flush()
		{
			while(networkStream.DataAvailable)
			{
				networkStream.ReadByte();	
			}
		}
	}
}

