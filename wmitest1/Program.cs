using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Management.Infrastructure;
using System.Text.RegularExpressions;



namespace wmitest1
{
    class Program
    {
        static void Main(string[] args)
        {
            CimSession cimSession = CimSession.Create("localhost");
            IEnumerable<CimInstance> queryInstances = cimSession.QueryInstances(@"root\cimv2",
                            "WQL",
                            @"select DeviceID, Model, Size from win32_DiskDrive Where DeviceID like '%PHYSICALDRIVE3'");

            string disk="";

            foreach (CimInstance cimInstance in queryInstances)
            {
             
                Console.WriteLine("DeviceID name: {0}", cimInstance.CimInstanceProperties["DeviceID"].Value);
                disk = cimInstance.CimInstanceProperties["DeviceID"].Value.ToString();
                Console.WriteLine("Model name: {0}", cimInstance.CimInstanceProperties["Model"].Value);
            }
            
            string query = String.Format("ASSOCIATORS OF {{Win32_DiskDrive.DeviceID=\"{0}\"}} WHERE AssocClass=Win32_DiskDriveToDiskPartition", disk.Replace(@"\", @"\\"));
            Console.Out.WriteLine(query);
            IEnumerable<CimInstance> partitionmap = cimSession.QueryInstances(@"root\cimv2",
                            "WQL",
                            query);
            string query2;
            IEnumerable<CimInstance> vols;
            foreach (CimInstance p in partitionmap)
            {
                Console.WriteLine("DeviceID: {0}", p.CimInstanceProperties["DeviceID"].Value);
                query2 = String.Format("ASSOCIATORS OF {{Win32_DiskPartition.DeviceID=\"{0}\"}} WHERE AssocClass=Win32_LogicalDiskToPartition", p.CimInstanceProperties["DeviceID"].Value);
                vols = cimSession.QueryInstances(@"root\cimv2", "WQL", query2);
                foreach (CimInstance v in vols)
                {
                    Console.WriteLine("Name: {0}", v.CimInstanceProperties["Name"].Value);
                    Console.WriteLine("Size: {0}", v.CimInstanceProperties["Size"].Value);
                    // "System Reserved" and "EFI" are windows partitions. Leave them alone
                    Console.WriteLine("Size: {0}", v.CimInstanceProperties["VolumeName"].Value);
                }
            }

            

            Console.ReadLine();
        }
    }
}
