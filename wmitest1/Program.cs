using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Management.Infrastructure;
using System.Text.RegularExpressions;
using DiskPartitionUtility;



namespace wmitest1
{
    class Program
    {
        static void Main(string[] args)
        {
            Utility util = new Utility();
            DiskDrive[] disks = util.GetAllDiskPartitions();
            foreach(DiskDrive d in disks)
            {
                Console.Out.WriteLine("Disk #{0}", d.Index);
                if(d.Partitions == null)
                {
                    continue;
                }
                foreach (DiskPartition p in d.Partitions)
                {
                    Console.Out.WriteLine("Partition #{0}, Size {1}", p.Index, p.Size);
                    if(p.Volume == null)
                    {
                        continue;
                    }
                    Console.Out.WriteLine("{0}, {1}, {2}", p.Volume.Letter, p.Volume.Label, p.Volume.FileSystem);
                }
            }

            Console.ReadLine();
        }
    }
}
