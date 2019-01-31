using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Management.Infrastructure;


namespace DiskPartitionUtility
{
    public class Utility
    {
        private CimSession m_cimSession;

        public Utility()
        {
            m_cimSession = CimSession.Create("localhost");
        }

        public DiskDrive[] GetAllDiskPartitions()
        {
            List<DiskDrive> disksList = new List<DiskDrive>();

            IEnumerable<CimInstance> win32_diskdrive = m_cimSession.QueryInstances(@"root\cimv2", "WQL", @"select * from win32_DiskDrive");
            foreach(CimInstance d in win32_diskdrive)
            {
                disksList.Add(new DiskDrive(Int32.Parse(d.CimInstanceProperties["Index"].Value.ToString()), 
                    GetDiskPartitions(d.CimInstanceProperties["DeviceID"].Value.ToString())));
            }

            return disksList.ToArray();
        }

        // eg DeviceID                    : \\.\PHYSICALDRIVE0
        public DiskPartition[] GetDiskPartitions(string deviceID)
        {
            List<DiskPartition> partitionsList = new List<DiskPartition>();
            string query = String.Format("ASSOCIATORS OF {{Win32_DiskDrive.DeviceID=\"{0}\"}} WHERE AssocClass=Win32_DiskDriveToDiskPartition",
                    deviceID.Replace(@"\", @"\\"));
            IEnumerable<CimInstance> partitions = m_cimSession.QueryInstances(@"root\cimv2", "WQL", query);
            DiskPartition part;
            foreach (CimInstance p in partitions)
            {
                part = new DiskPartition(Int32.Parse(p.CimInstanceProperties["Index"].Value.ToString()), Int64.Parse(p.CimInstanceProperties["Size"].Value.ToString()));
                part.Volume = GetVolume(p.CimInstanceProperties["DeviceID"].Value.ToString());
                partitionsList.Add(part);
            }
            return partitionsList.ToArray();
        }

        public DiskVolume GetVolume(string partitionID)
        {
            string query = String.Format("ASSOCIATORS OF {{Win32_DiskPartition.DeviceID=\"{0}\"}} WHERE AssocClass=Win32_LogicalDiskToPartition", partitionID);
            IEnumerable<CimInstance> vols = m_cimSession.QueryInstances(@"root\cimv2", "WQL", query);
            CimInstance v = vols.FirstOrDefault();
            if(v == null)
            {
                return null;
            }
            // "System Reserved" and "EFI" are windows partitions. Leave them alone
            return new DiskVolume(v.CimInstanceProperties["Name"].Value.ToString(),
                v.CimInstanceProperties["VolumeName"].Value.ToString(),
                v.CimInstanceProperties["FileSystem"].Value.ToString());
        }
    }

    public class DiskDrive
    {
        private DiskPartition[] m_partitions;
        private int m_index;

        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }


        public DiskDrive(int index, DiskPartition[] p)
        {
            m_index = index;
            m_partitions = p;
        }

        public DiskPartition[] Partitions
        {
            get { return m_partitions; }
            set { m_partitions = value; }
        }
    }

    public class DiskPartition
    {
        public DiskPartition(int index, long size)
        {
            m_index = index;
            m_size = size;
        }

        private DiskVolume m_vol;

        public DiskVolume Volume
        {
            get { return m_vol; }
            set { m_vol = value; }
        }


        private int m_index;

        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }
        private long m_size;

        public long Size
        {
            get { return m_size; }
            set { m_size = value; }
        }
    }

    public class DiskVolume
    {
        public DiskVolume(string letter, string label, string fileSystem)
        {
            m_letter = letter;
            m_label = label;
            m_fileSystem = fileSystem;
        }

        private string m_letter;

        public string Letter
        {
            get { return m_letter; }
            set { m_letter = value; }
        }

        private string m_label;

        public string Label
        {
            get { return m_label; }
            set { m_label = value; }
        }

        private string m_fileSystem;

        public string FileSystem
        {
            get { return m_fileSystem; }
            set { m_fileSystem = value; }
        }

    }
}
