using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HPIMS_WCF_Service
{
    public class Server : IService
    {
        public string StatusOfServer(string serverName)
        {
            string output;
            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(serverName);
                if (reply.Status == IPStatus.Success)
                {
                    output = "True";
                }
                else
                {
                    output = Convert.ToString(reply.Status);
                }
            }
            catch (Exception exception)
            {
                output = exception.Message;
            }
            return output;
        }

        public string WhoIsLoggedIn(string strComputer, string username, string password)
        {
            string output;
            try
            {
                ConnectionOptions connection = new ConnectionOptions();
                connection.Username = username;
                connection.Password = password;
                connection.Authentication = AuthenticationLevel.Default;

                List<string> userList = new List<string>();

                ManagementScope scope = new ManagementScope("\\\\" + strComputer + "\\root\\CIMV2", connection);
                scope.Connect();
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Process WHERE Name = \'explorer.exe\'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    ManagementPath path = new ManagementPath("Win32_Process.Handle='" + queryObj["Handle"] + "'");
                    ManagementObject classInstance = new ManagementObject(scope, path, null);
                    ManagementBaseObject outParams = classInstance.InvokeMethod("GetOwner", null, null);
                    userList.Add(outParams["User"].ToString());
                }

                if (userList.Contains(username))
                {
                    output = "True";
                }
                else
                {
                    output = "False";
                }
                return output;
            }
            catch (ManagementException MEE)
            {
                return (MEE.Message);
            }
            catch (System.UnauthorizedAccessException UAEE)
            {
                return (UAEE.Message);
            }
            catch (Exception exception)
            {
                return (exception.Message);
            }
        }
        public string[] DriveStorage(string strComputer, string username, string password)
        {
            string[] driveName = { "C:", "E:", "F:" };
            string[] driveStatus = { "NA", "NA", "NA" };
            try
            {
                ConnectionOptions connection = new ConnectionOptions();
                connection.Username = username;
                connection.Password = password;
                connection.Authentication = AuthenticationLevel.Default;

                ManagementScope scope = new ManagementScope("\\\\" + strComputer + "\\root\\CIMV2", connection);
                scope.Connect();
                SelectQuery query = new SelectQuery("SELECT * FROM Win32_LogicalDisk");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();

                foreach (ManagementObject MO in queryCollection)
                {
                    string driveNameFetched = Convert.ToString(MO["Name"]);
                    for(int j=0; j < driveName.Length; j++)
                    {
                        if (driveNameFetched == driveName[j])
                        {
                            long divisor = Convert.ToInt64(MO["Size"]) - Convert.ToInt64(MO["FreeSpace"]);
                            long dividend = Convert.ToInt64(MO["Size"]);
                            decimal percentComplete = (decimal)((double)(100 * divisor) / dividend);
                            driveStatus[j] = Convert.ToString(decimal.Round(percentComplete, 2, MidpointRounding.AwayFromZero)) + "%";
                            break;
                        }
                    }  
                }
                return driveStatus;
            }
            catch (Exception)
            {
                return driveStatus;
            }
        }
    }
}