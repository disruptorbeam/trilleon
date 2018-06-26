using System;
using System.Collections.Generic;

namespace PubNubMessaging.Core
{
    internal class SubscribeEnvelope
    {
        private List<SubscribeMessage> m { get; set;} //messages;
        private TimetokenMetadata t { get; set;} //subscribeMetadata;

        public List<SubscribeMessage> Messages{
            get{
                return m;
            }
            set {
                m = value;
            }
        }

        public TimetokenMetadata TimetokenMeta{
            get{
                return t;
            }
            set {
                t = value;
            }
        }
    }

    public class PNPresenceEvent
    {
        public string Action { get; set;} 
        public string UUID { get; set;} 
        public int Occupancy { get; set;} 
        public long Timestamp { get; set;}
        public object State { get; set;}
        public List<string> Join { get; set;}
        public List<string> Timeout { get; set;}
        public List<string> Leave { get; set;}

        public PNPresenceEvent(string action, string uuid, int Occupancy,
            long timestamp, object state, List<string> joins, List<string> leaves, List<string> timeouts){
            this.Action = action;
            this.UUID = uuid;
            this.Occupancy = Occupancy;
            this.Timestamp = timestamp;
            this.State = state;
            this.Join = joins;
            this.Leave = leaves;
            this.Timeout = timeouts;
        }
    }

    public class PNMessageResult
    {
        public object Payload { get; set;} 
        public string Subscription { get; set;} 
        public string Channel { get; set;} 
        public long Timetoken { get; set;} 
        public long OriginatingTimetoken { get; set;} 
        public object UserMetadata { get; set;} 
        public string IssuingClientId { get; set;} 

        public PNMessageResult(string subscribedChannel, string actualchannel, object payload,
            long timetoken, long originatingTimetoken, object userMetadata, string issuingClientId){
            this.Subscription = subscribedChannel;// change to channel group
            this.Channel = actualchannel; // change to channel
            this.Payload = payload;
            this.Timetoken = timetoken;
            this.OriginatingTimetoken = originatingTimetoken;
            this.UserMetadata = userMetadata;
            this.IssuingClientId = issuingClientId;
        }
    }

    public class PNPresenceEventResult
    {
        public string Event { get; set;} 
        public string Subscription { get; set;} 
        public string Channel { get; set;} 
        public string UUID { get; set;} 
        public long Timestamp { get; set;} 
        public long Timetoken { get; set;} 
        public int Occupancy { get; set;} 
        public object State { get; set;} 
        public object UserMetadata { get; set;} 
        public string IssuingClientId { get; set;} 
        public List<string> Join { get; set;}
        public List<string> Timeout { get; set;}
        public List<string> Leave { get; set;}

        public PNPresenceEventResult(string subscribedChannel, string actualchannel, string presenceEvent,
            long timetoken, long timestamp, object userMetadata, object state, string uuid, int occupancy,
            string issuingClientId, List<string> joins, List<string> leaves, List<string> timeouts
        ){
            this.Subscription = subscribedChannel;// change to channel group
            this.Channel = actualchannel; // change to channel
            this.Event = presenceEvent;
            this.UUID = uuid;
            this.Occupancy = occupancy;
            this.Timetoken = timetoken;
            this.Timestamp = timestamp;
            this.State = state;
            this.UserMetadata = userMetadata;
            this.IssuingClientId = issuingClientId;
            this.Join = joins;
            this.Leave = leaves;
            this.Timeout = timeouts;
        }
    }

    internal class SubscribeMessage
    {
        private string a { get; set;} //shard;
        private string b { get; set;} //subscriptionMatch
        private string c { get; set;} //channel
        private object d { get; set;} //payload
        //private bool ear { get; set;} //eat after reading
        private string f { get; set;} //flags
        private string i { get; set;} //issuingClientId
        private string k { get; set;} //subscribeKey
        private long s { get; set;} //sequenceNumber
        private TimetokenMetadata o { get; set;} //originatingTimetoken
        private TimetokenMetadata p { get; set;} //publishMetadata
        //private string r { get; set;} //replicationMap
        private object u { get; set;} //userMetadata
        //private string w { get; set;} //waypointList

        internal SubscribeMessage(string shard, string subscriptionMatch, string channel, object payload,
            string flags, string issuingClientId, string subscribeKey, long sequenceNumber, TimetokenMetadata originatingTimetoken,
            TimetokenMetadata publishMetadata, object userMetadata
        )
        {
            this.a = shard;
            this.b = subscriptionMatch;
            this.c = channel;
            this.d = payload;
            this.f = flags;
            this.i = issuingClientId;
            this.k = subscribeKey;
            this.s = sequenceNumber;
            this.o = originatingTimetoken;
            this.p = publishMetadata;
            this.u = userMetadata;
        }

        public string Shard{
            get{
                return a;
            }
        }

        public string SubscriptionMatch{
            get{
                return b;
            }
        }

        public string Channel{
            get{
                return c;
            }
        }

        public object Payload{
            get{
                return d;
            }
        }

        /*public bool EatAfterReading{
            get{
                return ear;
            }
        }*/

        public string Flags{
            get{
                return f;
            }
        }

        public string IssuingClientId{
            get{
                return i;
            }
        }

        public string SubscribeKey{
            get{
                return k;
            }
        }

        public long SequenceNumber{
            get{
                return s;
            }
        }

        public TimetokenMetadata OriginatingTimetoken{
            get{
                return o;
            }
        }

        public TimetokenMetadata PublishTimetokenMetadata{
            get{
                return p;
            }
        }

        /*public object ReplicationMap{
            get{
                return r;
            }
        }*/

        public object UserMetadata{
            get{
                return u;
            }
        }

        /*public string WaypointList{
            get{
                return w;
            }
        }*/


    }

    internal class TimetokenMetadata
    {
        private long t { get; set;} //timetoken;
        private string r { get; set;} //region;

        internal TimetokenMetadata(long timetoken, string region)
        {
            t = timetoken;
            r = region;
        }

        public long Timetoken { 
            get{
                return t;
            }
        }
        public string Region {
            get {
                return r;
            }
        }
    }
}

