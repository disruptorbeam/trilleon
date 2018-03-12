using System;

namespace PubNubMessaging.Core
{
    #region "Unit test interface"
    public interface IPubnubUnitTest
    {
        bool EnableStubTest {
            get;
            set;
        }

        string TestClassName {
            get;
            set;
        }

        string TestCaseName {
            get;
            set;
        }

        //string GetStubResponse (HttpWebRequest request);
    }
    #endregion

}

