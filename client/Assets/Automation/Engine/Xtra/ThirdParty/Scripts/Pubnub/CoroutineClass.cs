//#define REDUCE_PUBNUB_COROUTINES
using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PubNubMessaging.Core
{
    #region EventExt and Args
    static class EventExtensions
    {
        public static void Raise<T> (this EventHandler<T> handler, object sender, T args)
            where T : EventArgs
        {
            if (handler != null) {
                handler (sender, args);
            }
        }
    }

    internal class CustomEventArgs<T> : EventArgs
    {
        internal string Message;
        internal RequestState<T> PubnubRequestState;
        internal bool IsError;
        internal bool IsTimeout;
        internal CurrentRequestType CurrRequestType;
    }

    #if(REDUCE_PUBNUB_COROUTINES)
    internal class CurrentRequestTypeEventArgs : EventArgs
    {
        internal bool IsTimeout;
        internal CurrentRequestType CurrRequestType;
    }
    #endif

    #endregion

    #region CoroutineClass
    internal class CoroutineParams<T>
    {
        public string url;
        public int timeout;
        public int pause;
        public CurrentRequestType crt;
        public Type typeParameterType;
        public RequestState<T> requestState;

        public CoroutineParams (string url, int timeout, int pause, CurrentRequestType crt, Type typeParameterType, RequestState<T> requestState)
        {
            this.url = url;
            this.timeout = timeout;
            this.pause = pause;
            this.crt = crt;
            this.typeParameterType = typeParameterType;
            this.requestState = requestState;
        }
    }

    
    public enum CurrentRequestType
    {
        Heartbeat,
        PresenceHeartbeat,
        Subscribe,
        NonSubscribe
    }

    //Sending a IEnumerator from a complex object in StartCoroutine doesn't work for Web/WebGL
    //Dispose of www leads to random unhandled exceptions.
    //Generic methods dont work in StartCoroutine when the called with the string param name StartCoroutine("method", param)
    //StopCoroutine only works when the coroutine is started with string overload.
    class CoroutineClass : MonoBehaviour
    {
        #region "IL2CPP workarounds"

        //Got an exception when using JSON serialisation for [],
        //IL2CPP needs to know about the array type at compile time.
        //So please define private static filed like this:
        private static System.String[][] _unused;
        private static System.Int32[][] _unused2;
        private static System.Int64[][] _unused3;
        private static System.Int16[][] _unused4;
        private static System.UInt16[][] _unused5;
        private static System.UInt64[][] _unused6;
        private static System.UInt32[][] _unused7;
        private static System.Decimal[][] _unused8;
        private static System.Double[][] _unused9;
        private static System.Boolean[][] _unused91;
        private static System.Object[][] _unused92;
     
        private static long[][] _unused10;
        private static int[][] _unused11;
        private static float[][] _unused12;
        private static decimal[][] _unused13;
        private static uint[][] _unused14;
        private static ulong[][] _unused15;

        #endregion

        internal bool isHearbeatComplete = false;
        internal bool isPresenceHeartbeatComplete = false;
        internal bool isSubscribeComplete = false;
        internal bool isNonSubscribeComplete = false;

        private IEnumerator SubCoroutine;
        private IEnumerator SubTimeoutCoroutine;
        private IEnumerator NonSubCoroutine;
        private IEnumerator NonSubTimeoutCoroutine;
        private IEnumerator PresenceHeartbeatCoroutine;
        private IEnumerator PresenceHeartbeatTimeoutCoroutine;
        private IEnumerator HeartbeatCoroutine;
        private IEnumerator HeartbeatTimeoutCoroutine;
        private IEnumerator DelayRequestCoroutineHB;
        private IEnumerator DelayRequestCoroutinePHB;
        private IEnumerator DelayRequestCoroutineSub;

        internal WWW subscribeWww;
        internal WWW heartbeatWww;
        internal WWW presenceHeartbeatWww;
        internal WWW nonSubscribeWww;

        private EventHandler<EventArgs> subCoroutineComplete;

        public event EventHandler<EventArgs> SubCoroutineComplete {
            add {
                if (subCoroutineComplete == null || !subCoroutineComplete.GetInvocationList ().Contains (value)) {
                    subCoroutineComplete += value;
                }
            }
            remove {
                subCoroutineComplete -= value;
            }
        }

        private EventHandler<EventArgs> nonSubCoroutineComplete;

        public event EventHandler<EventArgs> NonSubCoroutineComplete {
            add {
                if (nonSubCoroutineComplete == null || !nonSubCoroutineComplete.GetInvocationList ().Contains (value)) {
                    nonSubCoroutineComplete += value;
                }
            }
            remove {
                nonSubCoroutineComplete -= value;
            }
        }

        private EventHandler<EventArgs> presenceHeartbeatCoroutineComplete;

        public event EventHandler<EventArgs> PresenceHeartbeatCoroutineComplete {
            add {
                if (presenceHeartbeatCoroutineComplete == null || !presenceHeartbeatCoroutineComplete.GetInvocationList ().Contains (value)) {
                    presenceHeartbeatCoroutineComplete += value;
                }
            }
            remove {
                presenceHeartbeatCoroutineComplete -= value;
            }
        }

        private EventHandler<EventArgs> heartbeatCoroutineComplete;

        public event EventHandler<EventArgs> HeartbeatCoroutineComplete {
            add {
                if (heartbeatCoroutineComplete == null || !heartbeatCoroutineComplete.GetInvocationList ().Contains (value)) {
                    heartbeatCoroutineComplete += value;
                }
            }
            remove {
                heartbeatCoroutineComplete -= value;
            }
        }

        public float subscribeTimer = 310; 
        public float heartbeatTimer = 10;
        public float presenceHeartbeatTimer = 10;
        public float nonSubscribeTimer = 15;
        public float heartbeatPauseTimer = 10;
        public float presenceHeartbeatPauseTimer = 10;
        public float subscribePauseTimer = 10;

        #if(REDUCE_PUBNUB_COROUTINES)
        internal bool runSubscribeTimer = false;
        internal bool runNonSubscribeTimer = false;
        internal bool runHeartbeatTimer = false;
        internal bool runPresenceHeartbeatTimer = false;
        internal bool runHeartbeatPauseTimer = false;
        internal bool runPresenceHeartbeatPauseTimer = false;
        internal bool runSubscribePauseTimer = false;

        public event EventHandler<EventArgs> heartbeatResumeEvent;

        public event EventHandler<EventArgs> HeartbeatResumeEvent {
            add {
                if (heartbeatResumeEvent == null || !heartbeatResumeEvent.GetInvocationList ().Contains (value)) {
                    heartbeatResumeEvent += value;
                }
            }
            remove {
                heartbeatResumeEvent -= value;
            }
        }

        public event EventHandler<EventArgs> presenceHeartbeatResumeEvent;

        public event EventHandler<EventArgs> PresenceHeartbeatResumeEvent {
            add {
                if (presenceHeartbeatResumeEvent == null || !presenceHeartbeatResumeEvent.GetInvocationList ().Contains (value)) {
                    presenceHeartbeatResumeEvent += value;
                }
            }
            remove {
                presenceHeartbeatResumeEvent -= value;
            }
        }

        public event EventHandler<EventArgs> subscribeResumeEvent;

        public event EventHandler<EventArgs> SubscribeResumeEvent {
            add {
                if (subscribeResumeEvent == null || !subscribeResumeEvent.GetInvocationList ().Contains (value)) {
                    subscribeResumeEvent += value;
                }
            }
            remove {
                subscribeResumeEvent -= value;
            }
        }

        public event EventHandler<EventArgs> subCompleteOrTimeoutEvent;

        public event EventHandler<EventArgs> SubCompleteOrTimeoutEvent {
            add {
                if (subCompleteOrTimeoutEvent == null || !subCompleteOrTimeoutEvent.GetInvocationList ().Contains (value)) {
                    subCompleteOrTimeoutEvent += value;
                }
            }
            remove {
                subCompleteOrTimeoutEvent -= value;
            }
        }

        public event EventHandler<EventArgs> nonsubCompleteOrTimeoutEvent;

        public event EventHandler<EventArgs> NonsubCompleteOrTimeoutEvent {
            add {
                if (nonsubCompleteOrTimeoutEvent == null || !nonsubCompleteOrTimeoutEvent.GetInvocationList ().Contains (value)) {
                    nonsubCompleteOrTimeoutEvent += value;
                }
            }
            remove {
                nonsubCompleteOrTimeoutEvent -= value;
            }
        }

        public event EventHandler<EventArgs> heartbeatCompleteOrTimeoutEvent;

        public event EventHandler<EventArgs> HeartbeatCompleteOrTimeoutEvent {
            add {
                if (heartbeatCompleteOrTimeoutEvent == null || !heartbeatCompleteOrTimeoutEvent.GetInvocationList ().Contains (value)) {
                    heartbeatCompleteOrTimeoutEvent += value;
                }
            }
            remove {
                heartbeatCompleteOrTimeoutEvent -= value;
            }
        }

        public event EventHandler<EventArgs> presenceHeartbeatCompleteOrTimeoutEvent;

        public event EventHandler<EventArgs> PresenceHeartbeatCompleteOrTimeoutEvent {
            add {
                if (presenceHeartbeatCompleteOrTimeoutEvent == null || !presenceHeartbeatCompleteOrTimeoutEvent.GetInvocationList ().Contains (value)) {
                    presenceHeartbeatCompleteOrTimeoutEvent += value;
                }
            }
            remove {
                presenceHeartbeatCompleteOrTimeoutEvent -= value;
            }
        }

        internal CurrentRequestTypeEventArgs CreateCurrentRequestTypeEventArgs(CurrentRequestType crt, bool isTimeout){
            CurrentRequestTypeEventArgs crtEa = new CurrentRequestTypeEventArgs();
            crtEa.CurrRequestType = crt;
            crtEa.IsTimeout = isTimeout;
            return crtEa;
        }

        SafeDictionary<CurrentRequestType, object> storedCoroutineParams = new SafeDictionary<CurrentRequestType, object> ();

        internal object GetCoroutineParams<T> (CurrentRequestType aKey){
            if (storedCoroutineParams.ContainsKey (aKey)) {
                if (storedCoroutineParams.ContainsKey (aKey)) {
                    return storedCoroutineParams [aKey];
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, GetCoroutineParams returning false", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, GetCoroutineParams returning null", DateTime.Now.ToString ()), LoggingMethod.LevelError);
            #endif
            return null;
        }

        internal void SetCoroutineParams<T> (CurrentRequestType key, CoroutineParams<T> cp){
            object storeCp = cp as object;
            storedCoroutineParams.AddOrUpdate (key, storeCp, (oldData, newData) => storeCp);
        }

        internal void RaiseEvents(bool isTimeout, CurrentRequestType crt)
        {
            StopTimeouts (crt);
            switch(crt){
            case CurrentRequestType.Subscribe:
                subCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            case CurrentRequestType.Heartbeat:
                heartbeatCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            case CurrentRequestType.PresenceHeartbeat:
                presenceHeartbeatCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            case CurrentRequestType.NonSubscribe:
                nonsubCompleteOrTimeoutEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, isTimeout));
                break;
            default:
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, No matching crt", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
                
                break;
            }
        }

        internal void CheckElapsedTime(CurrentRequestType crt, float timer, WWW www)
        {
            if (timer <= 0) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckElapsedTime: timeout {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                #endif
                
                RaiseEvents (true, crt);
            } else if ((www != null) && (www.isDone)) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckElapsedTime: done {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                #endif
                
                RaiseEvents (false, crt);
            } else if ((timer > 0) && (www == null) && (CheckIfRequestIsRunning(crt))) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckElapsedTime: www null request running {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                #endif  

            } else if ((timer > 0) && (www == null) && (!CheckIfRequestIsRunning(crt))) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckElapsedTime: www null request not running timer running {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                #endif  

            } else if ((timer > 0) && (!CheckIfRequestIsRunning(crt))) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckElapsedTime: request not running timer running {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                #endif  

            } else if ((timer > 0) && (www == null)) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckElapsedTime: www null timer running {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                #endif

            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                //LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckElapsedTime: timer {1}", DateTime.Now.ToString (), timer.ToString ()), LoggingMethod.LevelInfo);
                #endif
            }
        }

        internal bool CheckIfRequestIsRunning(CurrentRequestType crt){
            switch (crt) {
            case CurrentRequestType.Subscribe:
                return (!isSubscribeComplete)? true: false;
            case CurrentRequestType.Heartbeat:
                return (!isHearbeatComplete)? true: false;
            case CurrentRequestType.PresenceHeartbeat:
                return (!isPresenceHeartbeatComplete)? true: false;
            case CurrentRequestType.NonSubscribe:
                return (!isNonSubscribeComplete)? true: false;
            default:
                return false;
            }
        }

        internal void CheckPauseTime(CurrentRequestType crt, float timer)
        {
            if (timer <= 0) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, pause timeout {1}", DateTime.Now.ToString (), crt.ToString()), LoggingMethod.LevelInfo);
                #endif

                StopTimeouts (crt);

                switch (crt) {
                case CurrentRequestType.Heartbeat:
                    heartbeatResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, false));
                    break;
                case CurrentRequestType.PresenceHeartbeat:
                    presenceHeartbeatResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, false));
                    break;
                case CurrentRequestType.Subscribe:
                    subscribeResumeEvent.Raise (this, CreateCurrentRequestTypeEventArgs(crt, false));
                    break;
                }
            } 
        }

        void Start(){
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, REDUCE_PUBNUB_COROUTINES is set.",
                DateTime.Now.ToString ()
            ), LoggingMethod.LevelInfo);
            #endif
        }

        void Update() {
            if (runSubscribeTimer) {
                subscribeTimer -= Time.deltaTime;
                CheckElapsedTime (CurrentRequestType.Subscribe, subscribeTimer, subscribeWww);
            }
            if (runHeartbeatTimer) {
                heartbeatTimer -= Time.deltaTime;
                CheckElapsedTime (CurrentRequestType.Heartbeat, heartbeatTimer, heartbeatWww);
            }
            if (runPresenceHeartbeatTimer) {
                presenceHeartbeatTimer -= Time.deltaTime;
                CheckElapsedTime (CurrentRequestType.PresenceHeartbeat, presenceHeartbeatTimer, presenceHeartbeatWww);
            }
            if (runNonSubscribeTimer) {
                nonSubscribeTimer -= Time.deltaTime;
                CheckElapsedTime (CurrentRequestType.NonSubscribe, nonSubscribeTimer, nonSubscribeWww);
            }
            if (runPresenceHeartbeatPauseTimer) {
                presenceHeartbeatPauseTimer -= Time.deltaTime;
                CheckPauseTime (CurrentRequestType.PresenceHeartbeat, presenceHeartbeatPauseTimer);
            }
            if (runHeartbeatPauseTimer) {
                heartbeatPauseTimer -= Time.deltaTime;
                CheckPauseTime (CurrentRequestType.Heartbeat, heartbeatPauseTimer);
            }
            if (runSubscribePauseTimer) {
                subscribePauseTimer -= Time.deltaTime;
                CheckPauseTime (CurrentRequestType.Subscribe, subscribePauseTimer);
            }
        }

        void CoroutineClass_CompleteEvent<T> (object sender, EventArgs e)
        {
            CurrentRequestTypeEventArgs crtEa = e as CurrentRequestTypeEventArgs;
            if (crtEa != null) {
                CoroutineParams<T> cp = GetCoroutineParams<T> (crtEa.CurrRequestType) as CoroutineParams<T>;

                if (crtEa.IsTimeout) {
                    ProcessTimeout<T> (cp);
                } else {
                    switch (crtEa.CurrRequestType) {
                    case CurrentRequestType.Subscribe:
                        ProcessResponse<T> (subscribeWww, cp);
                        break;
                    case CurrentRequestType.Heartbeat:
                        ProcessResponse<T> (heartbeatWww, cp);
                        break;
                    case CurrentRequestType.PresenceHeartbeat:
                        ProcessResponse<T> (presenceHeartbeatWww, cp);
                        break;
                    case CurrentRequestType.NonSubscribe:
                        ProcessResponse<T> (nonSubscribeWww, cp);
                        break;
                    default:
                        break;
                    }
                }
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CurrentRequestTypeEventArgs null", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                #endif
            }
        }

        void CoroutineClass_ResumeEvent<T> (object sender, EventArgs e){
            CurrentRequestTypeEventArgs crtEa = e as CurrentRequestTypeEventArgs;
            CoroutineParams<T> cp = GetCoroutineParams<T> (crtEa.CurrRequestType) as CoroutineParams<T>;

            StartCoroutinesByName<T> (cp.url, cp.requestState, cp.timeout, cp.pause, cp.crt);
        }

        public void RemoveEventHandler<T>(CurrentRequestType crt, bool removeHeartbeats){
            switch (crt) {
            case CurrentRequestType.Heartbeat:
                if (removeHeartbeats) {
                    HeartbeatCompleteOrTimeoutEvent -= CoroutineClass_CompleteEvent<T>;
                    HeartbeatResumeEvent -= CoroutineClass_ResumeEvent<T>;
                }
                break;
            case CurrentRequestType.PresenceHeartbeat:
                if (removeHeartbeats) {
                    PresenceHeartbeatCompleteOrTimeoutEvent -= CoroutineClass_CompleteEvent<T>;
                    PresenceHeartbeatResumeEvent -= CoroutineClass_ResumeEvent<T>;
                }
                break;
            case CurrentRequestType.Subscribe:
                SubCompleteOrTimeoutEvent -= CoroutineClass_CompleteEvent<T>;
                SubscribeResumeEvent -= CoroutineClass_ResumeEvent<T>;
                break;
            case CurrentRequestType.NonSubscribe:
                NonsubCompleteOrTimeoutEvent -= CoroutineClass_CompleteEvent<T>;
                break;
            default:
                break;
            }            
        }

        internal void StopTimeouts(CurrentRequestType crt){
            switch (crt) {
            case CurrentRequestType.Heartbeat:
                runHeartbeatTimer = false;
                heartbeatTimer = 0;
                runHeartbeatPauseTimer = false;
                heartbeatPauseTimer = 0;
                break;
            case CurrentRequestType.PresenceHeartbeat:
                runPresenceHeartbeatTimer = false;
                presenceHeartbeatTimer = 0;
                runPresenceHeartbeatPauseTimer = false;
                presenceHeartbeatPauseTimer = 0;
                break;
            case CurrentRequestType.Subscribe:
                runSubscribeTimer = false;
                subscribeTimer = 0;
                runSubscribePauseTimer = false;
                subscribePauseTimer = 0;

                break;
            case CurrentRequestType.NonSubscribe:
                runNonSubscribeTimer = false;
                nonSubscribeTimer = 0;
                break;
            default:
                break;
            }
        }
        #endif

        public void DelayStartCoroutine<T>(string url, RequestState<T> pubnubRequestState, int timeout, int pause, CurrentRequestType crt)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, DelayStartCoroutine delay: {1} {2}", DateTime.Now.ToString (), pause.ToString(), crt.ToString()), LoggingMethod.LevelInfo);
            #endif

            #if(REDUCE_PUBNUB_COROUTINES)

            CoroutineParams<T> cp = new CoroutineParams<T> (url, timeout, pause, crt, typeof(T), pubnubRequestState);
            SetCoroutineParams<T> (crt, cp);

            if (pubnubRequestState.RespType == ResponseType.Heartbeat){
                heartbeatPauseTimer = pause;
                HeartbeatResumeEvent += CoroutineClass_ResumeEvent<T>;
                runHeartbeatPauseTimer = true;
            } else if (pubnubRequestState.RespType == ResponseType.PresenceHeartbeat){
                presenceHeartbeatPauseTimer = pause;
                PresenceHeartbeatResumeEvent += CoroutineClass_ResumeEvent<T>;
                runPresenceHeartbeatPauseTimer = true;
            } else {
                subscribePauseTimer = pause;
                SubscribeResumeEvent += CoroutineClass_ResumeEvent<T>;
                runSubscribePauseTimer = true;
            }

            #else
            if (pubnubRequestState.RespType == ResponseType.Heartbeat)
            {
                DelayRequestCoroutineHB = DelayRequest<T>(url, pubnubRequestState, timeout, pause, crt);
                StartCoroutine(DelayRequestCoroutineHB);
            }
            else if (pubnubRequestState.RespType == ResponseType.PresenceHeartbeat)
            {
                DelayRequestCoroutinePHB = DelayRequest<T>(url, pubnubRequestState, timeout, pause, crt);
                StartCoroutine(DelayRequestCoroutinePHB);
            }
            else
            {
                DelayRequestCoroutineSub = DelayRequest<T>(url, pubnubRequestState, timeout, pause, crt);
                StartCoroutine(DelayRequestCoroutineSub);
            }
            #endif
        }

        public void Run<T> (string url, RequestState<T> pubnubRequestState, int timeout, int pause)
        {
            //for heartbeat and presence heartbeat treat reconnect as pause
            CurrentRequestType crt;
            if ((pubnubRequestState.RespType == ResponseType.Heartbeat) || (pubnubRequestState.RespType == ResponseType.PresenceHeartbeat)) {
                crt = CurrentRequestType.PresenceHeartbeat;
                if (pubnubRequestState.RespType == ResponseType.Heartbeat) {
                    crt = CurrentRequestType.Heartbeat;
                }
                CheckComplete (crt);

                if (pubnubRequestState.Reconnect) {
                    DelayStartCoroutine<T>(url, pubnubRequestState, timeout, pause, crt);
                } else {
                    StartCoroutinesByName<T> (url, pubnubRequestState, timeout, pause, crt);
                }
            } else if (pubnubRequestState.RespType.Equals(ResponseType.SubscribeV2) || pubnubRequestState.RespType.Equals(ResponseType.PresenceV2)
                ) {
                crt = CurrentRequestType.Subscribe;

                CheckComplete (crt);

                #if (ENABLE_PUBNUB_LOGGING)
                if ((subscribeWww != null) && (!subscribeWww.isDone)) {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Run: subscribeWww running trying to abort {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                    if (subscribeWww == null) {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Run: subscribeWww aborted {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                    }
                }
                #endif
                if (pause > 0) {
                    DelayStartCoroutine<T>(url, pubnubRequestState, timeout, pause, crt);
                } else {
                    StartCoroutinesByName<T> (url, pubnubRequestState, timeout, pause, crt);
                }
            } else {
                crt = CurrentRequestType.NonSubscribe;

                CheckComplete (crt);

                StartCoroutinesByName<T> (url, pubnubRequestState, timeout, pause, crt);
            } 
        }

        internal void StartCoroutinesByName<T> (string url, RequestState<T> pubnubRequestState, int timeout, int pause, CurrentRequestType crt)
        {
            CoroutineParams<T> cp = new CoroutineParams<T> (url, timeout, pause, crt, typeof(T), pubnubRequestState);
            
            #if(REDUCE_PUBNUB_COROUTINES)
            SetCoroutineParams<T> (crt, cp);
            StopTimeouts(crt);
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, StartCoroutinesByName: URL {2} {1}", 
                DateTime.Now.ToString (), cp.url.ToString (), crt.ToString()), LoggingMethod.LevelInfo);
            #endif

            #endif

            if (crt == CurrentRequestType.Subscribe) {
                #if(!REDUCE_PUBNUB_COROUTINES)
                if (DelayRequestCoroutineSub != null)
                {
                    StopCoroutine(DelayRequestCoroutineSub);
                }

                if((SubTimeoutCoroutine != null) && (!isSubscribeComplete)){
                    StopCoroutine (SubTimeoutCoroutine);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopped existing timeout coroutine {1}", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }
                SubTimeoutCoroutine = CheckTimeoutSub<T> (cp);
                SubCoroutine = SendRequestSub<T> (cp);
                StartCoroutine (SubTimeoutCoroutine);
                StartCoroutine (SubCoroutine);

                #else
                subscribeTimer = timeout;
                runSubscribeTimer = true;
                SubCompleteOrTimeoutEvent += CoroutineClass_CompleteEvent<T>;
                isSubscribeComplete = false;

                subscribeWww = new WWW (cp.url);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, StartCoroutinesByName: {1} running", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                #endif

                #endif
                
            } else if (crt == CurrentRequestType.NonSubscribe) {
                #if(!REDUCE_PUBNUB_COROUTINES)
                if((NonSubTimeoutCoroutine != null) && (!isNonSubscribeComplete)){
                    StopCoroutine (NonSubTimeoutCoroutine);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopped existing timeout coroutine {1}", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }
                NonSubTimeoutCoroutine = CheckTimeoutNonSub<T> (cp);
                NonSubCoroutine = SendRequestNonSub<T> (cp);
                StartCoroutine (NonSubTimeoutCoroutine);
                StartCoroutine (NonSubCoroutine);

                #else
                nonSubscribeTimer = timeout;
                runNonSubscribeTimer = true;
                NonsubCompleteOrTimeoutEvent += CoroutineClass_CompleteEvent<T>;
                isNonSubscribeComplete = false;

                nonSubscribeWww = new WWW (cp.url);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, StartCoroutinesByName: {1} running", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                #endif

                #endif
            } else if (crt == CurrentRequestType.PresenceHeartbeat) {
                #if(!REDUCE_PUBNUB_COROUTINES)
                if((PresenceHeartbeatTimeoutCoroutine != null) && (!isPresenceHeartbeatComplete)){
                    StopCoroutine (PresenceHeartbeatTimeoutCoroutine);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, StartCoroutinesByName: Stopped existing timeout coroutine {1}", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }
                PresenceHeartbeatTimeoutCoroutine = CheckTimeoutPresenceHeartbeat<T> (cp);
                PresenceHeartbeatCoroutine = SendRequestPresenceHeartbeat<T> (cp);
                StartCoroutine (PresenceHeartbeatTimeoutCoroutine);
                StartCoroutine (PresenceHeartbeatCoroutine);
                #else

                presenceHeartbeatTimer = timeout;
                runPresenceHeartbeatTimer = true;
                PresenceHeartbeatCompleteOrTimeoutEvent += CoroutineClass_CompleteEvent<T>;
                isPresenceHeartbeatComplete = false;
                presenceHeartbeatWww = new WWW (cp.url);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, StartCoroutinesByName: {1} running", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                #endif

                #endif

            } else if (crt == CurrentRequestType.Heartbeat) {
                #if(!REDUCE_PUBNUB_COROUTINES)
                if((HeartbeatTimeoutCoroutine != null) && (!isHearbeatComplete)){
                    StopCoroutine (HeartbeatTimeoutCoroutine);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Stopped existing timeout coroutine {1}", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }
                HeartbeatTimeoutCoroutine = CheckTimeoutHeartbeat<T> (cp);
                HeartbeatCoroutine = SendRequestHeartbeat<T> (cp);
                StartCoroutine (HeartbeatTimeoutCoroutine);
                StartCoroutine (HeartbeatCoroutine);

                #else
                heartbeatTimer = timeout;
                runHeartbeatTimer = true;
                HeartbeatCompleteOrTimeoutEvent += CoroutineClass_CompleteEvent<T>;
                isHearbeatComplete = false;
                heartbeatWww = new WWW (cp.url);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, StartCoroutinesByName: {1} running", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                #endif

                #endif
            }
        }

        public IEnumerator DelayRequest<T> (string url, RequestState<T> pubnubRequestState, int timeout, int pause, CurrentRequestType crt)
        {
            yield return new WaitForSeconds (pause); 
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, DelayRequest timeout  {1}", DateTime.Now.ToString (), crt.ToString()), LoggingMethod.LevelInfo);
            #endif

            StartCoroutinesByName<T> (url, pubnubRequestState, timeout, pause, crt);
        }

        public void ProcessResponse<T> (WWW www, CoroutineParams<T> cp)
        {
            try {
                #if(REDUCE_PUBNUB_COROUTINES)
                RemoveEventHandler<T>(cp.crt, false);
                #endif
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessResponse: Process Request {1}, url: {2} ", 
                    DateTime.Now.ToString (), cp.crt.ToString (), www.url), LoggingMethod.LevelInfo);
                #endif

                if (www != null) {
                    SetComplete (cp.crt);
                    string message = "";
                    bool isError = false;

                    if (string.IsNullOrEmpty (www.error)) {
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessResponse: WWW Sub {1}\n Message: {2}\n URL: {3}", 
                            DateTime.Now.ToString (), cp.crt.ToString (), www.text, www.url), LoggingMethod.LevelInfo);
                        #endif
                        message = www.text;
                        isError = false;
                    } else {
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessResponse: WWW Sub {1}\n Error: {2}\n, text: {3}\n URL: {4}", 
                            DateTime.Now.ToString (), cp.crt.ToString (), www.error, www.text, www.url), LoggingMethod.LevelInfo);
                        #endif
                        message = string.Format ("{0}\"Error\": \"{1}\", \"Description\": {2}{3}", "{", www.error, www.text, "}");
                        isError = true;
                    } 

                    #if (ENABLE_PUBNUB_LOGGING)
                    if (cp.requestState == null) {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessResponse: WWW Sub request null2", 
                            DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                    } else {
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessResponse: WWW Sub request2 {1} {2}", 
                            DateTime.Now.ToString (), cp.requestState.RespType, cp.crt), LoggingMethod.LevelInfo);
                    }
                    #endif

                    FireEvent (message, isError, false, cp.requestState, cp.crt);
                } 
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessResponse: RunCoroutineSub {1}, Exception: {2}", DateTime.Now.ToString (), cp.crt.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }
        }

        #if(!REDUCE_PUBNUB_COROUTINES)
        public IEnumerator SendRequestSub<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, URL Sub {1} ", DateTime.Now.ToString (), cp.url.ToString ()), LoggingMethod.LevelInfo);
            #endif
            WWW www;

            isSubscribeComplete = false;

            subscribeWww = new WWW (cp.url);
            yield return subscribeWww;
            if ((subscribeWww != null) && (subscribeWww.isDone)) {
                www = subscribeWww;
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1} not null", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                #endif
            } else {
                www = null;
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1} null", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                #endif
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0},After www type  {1}", DateTime.Now.ToString (), typeof(T)), LoggingMethod.LevelError);
            #endif
            ProcessResponse<T> (www, cp);
        }

        public IEnumerator SendRequestNonSub<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, URL NonSub {1} ", DateTime.Now.ToString (), cp.url.ToString ()), LoggingMethod.LevelInfo);
            #endif
            WWW www;

            isNonSubscribeComplete = false;
            nonSubscribeWww = new WWW (cp.url);
            yield return nonSubscribeWww;
            if ((nonSubscribeWww != null) && (nonSubscribeWww.isDone)) {
                www = nonSubscribeWww;
            } else {
                www = null;
            }
             
            ProcessResponse (www, cp);
        }

        public IEnumerator SendRequestPresenceHeartbeat<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, URL PresenceHB {1} ", DateTime.Now.ToString (), cp.url.ToString ()), LoggingMethod.LevelInfo);
            #endif
            WWW www;

            isPresenceHeartbeatComplete = false;
            presenceHeartbeatWww = new WWW (cp.url);
            yield return presenceHeartbeatWww;
            if ((presenceHeartbeatWww != null) && (presenceHeartbeatWww.isDone)) {
                www = presenceHeartbeatWww;
            } else {
                www = null;
            }

            ProcessResponse (www, cp);
        }

        public IEnumerator SendRequestHeartbeat<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, URL Heartbeat {1} ", DateTime.Now.ToString (), cp.url.ToString ()), LoggingMethod.LevelInfo);
            #endif
            WWW www;

            isHearbeatComplete = false;
            heartbeatWww = new WWW (cp.url);
            yield return heartbeatWww;
            if ((heartbeatWww != null) && (heartbeatWww.isDone)) {
                www = heartbeatWww;
            } else {
                www = null;
            }

            ProcessResponse (www, cp);
        }
        #endif

        internal void SetComplete (CurrentRequestType crt)
        {
            try {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, In SetComplete:  {1}", DateTime.Now.ToString (), 
                    crt.ToString ()), LoggingMethod.LevelInfo);
                #endif

                #if(REDUCE_PUBNUB_COROUTINES)
                StopTimeouts(crt);
                #endif
                if (crt == CurrentRequestType.Heartbeat) {
                    #if(!REDUCE_PUBNUB_COROUTINES)
                    if(HeartbeatTimeoutCoroutine != null)
                        StopCoroutine (HeartbeatTimeoutCoroutine);
                    #endif
                    isHearbeatComplete = true;
                } else if (crt == CurrentRequestType.PresenceHeartbeat) {
                    #if(!REDUCE_PUBNUB_COROUTINES)
                    if(PresenceHeartbeatTimeoutCoroutine != null)
                        StopCoroutine (PresenceHeartbeatTimeoutCoroutine);
                    #endif
                    isPresenceHeartbeatComplete = true;
                } else if (crt == CurrentRequestType.Subscribe) {
                    #if(!REDUCE_PUBNUB_COROUTINES)
                    if(SubTimeoutCoroutine != null)
                        StopCoroutine (SubTimeoutCoroutine);
                    #endif
                    
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SetComplete: {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif

                    isSubscribeComplete = true;
                } else {
                    #if(!REDUCE_PUBNUB_COROUTINES)
                    if(NonSubTimeoutCoroutine != null)
                        StopCoroutine (NonSubTimeoutCoroutine);
                    #endif
                    isNonSubscribeComplete = true;
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SetComplete: Complete {1}", DateTime.Now.ToString (), crt.ToString()), LoggingMethod.LevelInfo);
                #endif
            } catch (Exception ex) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SetComplete: Exception: {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
            }

        }

        #if(!REDUCE_PUBNUB_COROUTINES)
        internal void StopRunningCoroutines(CurrentRequestType crt)
        {
            if (crt == CurrentRequestType.Heartbeat)
            {
                if ((heartbeatWww != null) && (!heartbeatWww.isDone))
                {
                    heartbeatWww.Dispose();
                    heartbeatWww = null;
                    if(HeartbeatCoroutine != null)
                        StopCoroutine(HeartbeatCoroutine);
                }
                if (DelayRequestCoroutineHB != null)
                {
                    StopCoroutine(DelayRequestCoroutineHB);
                }
            }
            else if (crt == CurrentRequestType.PresenceHeartbeat)
            {
                if ((presenceHeartbeatWww != null) && (!presenceHeartbeatWww.isDone))
                {
                    presenceHeartbeatWww.Dispose();
                    presenceHeartbeatWww = null;
                    if(PresenceHeartbeatCoroutine != null)
                        StopCoroutine(PresenceHeartbeatCoroutine);
                }
                if (DelayRequestCoroutinePHB != null)
                {
                    StopCoroutine(DelayRequestCoroutinePHB);
                }
            }
            else if ((crt == CurrentRequestType.Subscribe) && (subscribeWww != null) && (!subscribeWww.isDone))
            {
                subscribeWww = null;
                if(SubCoroutine != null)
                    StopCoroutine(SubCoroutine);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Coroutine stopped Subscribe: ", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
                #endif
                if (DelayRequestCoroutineSub != null)
                {
                    StopCoroutine(DelayRequestCoroutineSub);
                }
            }
            else if ((crt == CurrentRequestType.NonSubscribe) && (nonSubscribeWww != null) && (!nonSubscribeWww.isDone))
            {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Dispose nonSubscribeWww: ", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
                #endif
                nonSubscribeWww.Dispose();
                nonSubscribeWww = null;
                if(NonSubCoroutine != null)
                    StopCoroutine(NonSubCoroutine);
            }
        }
        #endif

        public bool CheckComplete (CurrentRequestType crt)
        {
            try {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckComplete:  {1}", 
                    DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                #endif

                #if(!REDUCE_PUBNUB_COROUTINES)
                if (crt == CurrentRequestType.Heartbeat) {
                    if ((!isHearbeatComplete) && (heartbeatWww != null) && (!heartbeatWww.isDone)) {   
                        if(HeartbeatCoroutine != null)
                            StopCoroutine (HeartbeatCoroutine);
                        return false;
                    }
                } else if (crt == CurrentRequestType.PresenceHeartbeat) {
                    if ((!isPresenceHeartbeatComplete) && (presenceHeartbeatWww != null) && (!presenceHeartbeatWww.isDone)) {
                        if(PresenceHeartbeatCoroutine != null)
                            StopCoroutine (PresenceHeartbeatCoroutine);
                        return false;
                    }
                } else if (crt == CurrentRequestType.Subscribe) {
                    if ((!isSubscribeComplete) && (subscribeWww != null) && (!subscribeWww.isDone)) {
                        if(SubCoroutine != null)
                            StopCoroutine (SubCoroutine);
                        return false;
                    }
                } else {
                    if ((!isNonSubscribeComplete) && (nonSubscribeWww != null) && (!nonSubscribeWww.isDone)) {
                        if(NonSubCoroutine != null)
                            StopCoroutine (NonSubCoroutine);
                        return false;
                    }
                }
                #else
                StopTimeouts(crt);

                if (crt == CurrentRequestType.Heartbeat) {
                    if ((!isHearbeatComplete) && ((heartbeatWww != null) && (!heartbeatWww.isDone))) {    
                        #if(!UNITY_ANDROID)
                        heartbeatWww.Dispose();
                        #endif
                        heartbeatWww = null;
                        return false;
                    }
                } else if (crt == CurrentRequestType.PresenceHeartbeat) {
                    if ((!isPresenceHeartbeatComplete) && ((presenceHeartbeatWww != null) && (!presenceHeartbeatWww.isDone))) {
                        #if(!UNITY_ANDROID)
                        presenceHeartbeatWww.Dispose();
                        #endif
                        presenceHeartbeatWww = null;
                        return false;
                    }
                } else if (crt == CurrentRequestType.Subscribe) {
                    if ((!isSubscribeComplete) && ((subscribeWww != null) && (!subscribeWww.isDone))) {
        
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckComplete: DISPOSING WWW", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                        #endif

                //TODO: Remove flag when unity bug is fixed. Currenlty calling this on Android hangs the whole app. 
                //Not fixed for Android as of Unity 5.3.5p4
                        #if(!UNITY_ANDROID)
                        subscribeWww.Dispose();
                        #endif
                
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckComplete: WWW disposed", DateTime.Now.ToString ()), LoggingMethod.LevelError);
                        #endif
                
                        subscribeWww = null;
                        return false;
                    }
                } else {
                    if ((!isNonSubscribeComplete) && ((nonSubscribeWww != null) && (!nonSubscribeWww.isDone))) {
                        #if(!UNITY_ANDROID)
                        nonSubscribeWww.Dispose();
                        #endif
                        nonSubscribeWww = null;
                        return false;
                    }
                }
                #endif

            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CheckComplete: Exception: {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }

            return true;
        }
    
        public void BounceRequest<T> (CurrentRequestType crt, RequestState<T> pubnubRequestState, bool fireEvent)
        {
            try {
                #if(!REDUCE_PUBNUB_COROUTINES)
                StopRunningCoroutines(crt);
                #else
                CheckComplete (crt);
                #endif
                SetComplete (crt);

                if ((pubnubRequestState != null) && fireEvent) {
                    FireEvent ("Aborted", true, false, pubnubRequestState, crt);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, BounceRequest: event fired {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }
            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, BounceRequest: Exception: {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
                #endif
            }
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, BounceRequest: {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
            #endif
        }
    
        public void ProcessTimeout<T> (CoroutineParams<T> cp)
        {
            try {
                #if(REDUCE_PUBNUB_COROUTINES)
                RemoveEventHandler<T>(cp.crt, false);
                #endif

                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessTimeout: {1}", DateTime.Now.ToString (), cp.crt.ToString ()), LoggingMethod.LevelInfo);
                #endif

                if (!CheckComplete (cp.crt)) {
                    if ((cp.typeParameterType == typeof(string)) || (cp.typeParameterType == typeof(object))) {
                        FireEvent ("Timed out", true, true, cp.requestState, cp.crt);
                        #if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessTimeout: WWW Error: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
                        #endif
                    } else {
                        throw new Exception ("'string' and 'object' are the only types supported in generic method calls");
                    }
                }

            } catch (Exception ex) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessTimeout: {1} {2}", DateTime.Now.ToString (), ex.ToString (), cp.crt.ToString ()), LoggingMethod.LevelError);
                #endif
            }
        }

        #if(!REDUCE_PUBNUB_COROUTINES)
        public IEnumerator CheckTimeoutSub<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, yielding: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
            #endif

            yield return new WaitForSeconds (cp.timeout); 
            ProcessTimeout<T> (cp);
        }
    
        public IEnumerator CheckTimeoutNonSub<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, yielding: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
            #endif
            yield return new WaitForSeconds (cp.timeout); 
            ProcessTimeout<T> (cp);
        }

        public IEnumerator CheckTimeoutPresenceHeartbeat<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, yielding: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
            #endif
            yield return new WaitForSeconds (cp.timeout); 
            ProcessTimeout<T> (cp);
        }

        public IEnumerator CheckTimeoutHeartbeat<T> (CoroutineParams<T> cp)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, yielding: {1} sec timeout", DateTime.Now.ToString (), cp.timeout.ToString ()), LoggingMethod.LevelInfo);
            #endif
            yield return new WaitForSeconds (cp.timeout); 
            ProcessTimeout<T> (cp);
        }
        #endif

        public void FireEvent<T> (string message, bool isError, bool isTimeout, RequestState<T> pubnubRequestState, CurrentRequestType crt)
        {
            CustomEventArgs<T> cea = new CustomEventArgs<T> ();
            cea.PubnubRequestState = pubnubRequestState;
            cea.Message = message;
            cea.IsError = isError;
            cea.IsTimeout = isTimeout;
            cea.CurrRequestType = crt;
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, FireEvent: Raising Event of type : {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
            #endif

            if ((crt == CurrentRequestType.Heartbeat) && (heartbeatCoroutineComplete != null)) {
                heartbeatCoroutineComplete.Raise (this, cea);
            } else if ((crt == CurrentRequestType.PresenceHeartbeat) && (presenceHeartbeatCoroutineComplete != null)) {
                presenceHeartbeatCoroutineComplete.Raise (this, cea);
            } else if ((crt == CurrentRequestType.Subscribe) && (subCoroutineComplete != null)) {
                subCoroutineComplete.Raise (this, cea);
            } else if ((crt == CurrentRequestType.NonSubscribe) && (nonSubCoroutineComplete != null)) {
                nonSubCoroutineComplete.Raise (this, cea);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, FireEvent: Request Type not matched {1}", DateTime.Now.ToString (), crt.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif
        }
    }
    #endregion

    #region "PubnubWebResponse and PubnubWebRequest"
    public class PubnubWebResponse
    {
        WWW www;

        public PubnubWebResponse (WWW www)
        {
            this.www = www;
        }

        public string ResponseUri {
            get {
                return www.url;
            }
        }

        public Dictionary<string, string> Headers {
            get {
                return www.responseHeaders;
            }
        }
    }

    public class PubnubWebRequest
    {
        WWW www;

        public PubnubWebRequest (WWW www)
        {
            this.www = www;
        }

        public string RequestUri {
            get {
                return www.url;
            }
        }

        public Dictionary<string, string> Headers {
            get {
                return www.responseHeaders;
            }
        }

    }
    #endregion
}
