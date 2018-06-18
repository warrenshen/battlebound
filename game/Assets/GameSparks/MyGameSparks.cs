#pragma warning disable 612,618
#pragma warning disable 0114
#pragma warning disable 0108

using System;
using System.Collections.Generic;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!
//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!
//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!

namespace GameSparks.Api.Messages {

		public class ScriptMessage_ChallengeTimeRunningOutMessage : ScriptMessage {
		
			public new static Action<ScriptMessage_ChallengeTimeRunningOutMessage> Listener;
	
			public ScriptMessage_ChallengeTimeRunningOutMessage(GSData data) : base(data){}
	
			private static ScriptMessage_ChallengeTimeRunningOutMessage Create(GSData data)
			{
				ScriptMessage_ChallengeTimeRunningOutMessage message = new ScriptMessage_ChallengeTimeRunningOutMessage (data);
				return message;
			}
	
			static ScriptMessage_ChallengeTimeRunningOutMessage()
			{
				handlers.Add (".ScriptMessage_ChallengeTimeRunningOutMessage", Create);
	
			}
			
			override public void NotifyListeners()
			{
				if (Listener != null)
				{
					Listener (this);
				}
			}
		}

}
