using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace HPIMS_WCF_Service
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string StatusOfServer(string serverName);

        [OperationContract]
        string WhoIsLoggedIn(string strComputer, string username, string password);

        [OperationContract]
        string[] DriveStorage(string strComputer, string username, string password);
    }
}
