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
        private CimSession cimSession;

        public Utility()
        {
            cimSession = CimSession.Create("localhost");
        }

        public DiskDrive[] GetDiskPartitions()
        {
            List<DiskDrive> disks = new List<DiskDrive>();
            IEnumerable<CimInstance> win32_diskdrive = cimSession.QueryInstances(@"root\cimv2",
                            "WQL",
                            @"select * Size from win32_DiskDrive Where DeviceID");
            string query;
            foreach (CimInstance d in win32_diskdrive)
            {
                query = String.Format("ASSOCIATORS OF {{Win32_DiskDrive.DeviceID=\"{0}\"}} WHERE AssocClass=Win32_DiskDriveToDiskPartition",
                    d.CimInstanceProperties["DeviceID"].ToString().Replace(@"\", @"\\"));
                IEnumerable<CimInstance> partitionmap = cimSession.QueryInstances(@"root\cimv2",
                            "WQL",
                            query);
            }

            return null;
        }

    }

    public class DiskDrive
    {
        private DiskPartition[] m_partitions;

        public DiskDrive()
        {

        }

        public DiskPartition[] Partitions
        {
            get { return m_partitions; }
            set { m_partitions = value; }
        }
    }

    public class DiskPartition
    {
        public DiskPartition()
        {

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
        private string m_letter;

        public string Letter
        {
            get { return m_letter; }
            set { m_letter = value; }
        }

        private int m_index;

        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        private string m_label;

        public string Label
        {
            get { return m_label; }
            set { m_label = value; }
        }

        public DiskVolume()
        {
            
        }
    }
}
