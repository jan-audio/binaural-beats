using System.Runtime.InteropServices;

namespace CreateData
{
    public class DeviceDescription
    {
        public long FreeSpace { get; private set; }
        public long DiskTotal { get; private set; }
        public int Used { get; private set; }
        public string OS { get; private set; }

        public string MachineName { get; private set; }
        public string UserName { get; private set; }
        public bool Is64BitOperatingSystem { get; private set; }
        public int ProcessorCount { get; private set; }
        public bool Is64BitProcess { get; private set; }
        public string OSVersion_Platform { get; private set; }
        public string OSVersion_Version { get; private set; }
        public string OSVersion_ServicePack { get; private set; }
        public string UserDomainName { get; private set; }
        public string Version { get; private set; }
        public long WorkingSet { get; private set; }

        /// <summary>
        /// GB
        /// </summary>
        public double Mem { get; private set; }

        internal static DeviceDescription Instance()
        {
            DriveInfo d = new DriveInfo(AppDomain.CurrentDomain.BaseDirectory);
            var obj = new DeviceDescription();

            obj.FreeSpace = d.TotalFreeSpace / (1024 * 1024);
            obj.DiskTotal = d.TotalSize / (1024 * 1024);
            obj.Used = (int)((100 * d.AvailableFreeSpace) / d.TotalSize);
            obj.OS = RuntimeInformation.OSDescription;
            obj.Mem = Math.Round((double)GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024 * 1024), 1);
            obj.MachineName = Environment.MachineName;
            obj.UserName = Environment.UserName;
            obj.Is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
            obj.ProcessorCount = Environment.ProcessorCount;
            obj.Is64BitProcess = Environment.Is64BitProcess;
            obj.OSVersion_Platform = Environment.OSVersion.Platform.ToString();
            obj.OSVersion_Version = Environment.OSVersion.Version.ToString();
            obj.OSVersion_ServicePack = Environment.OSVersion.ServicePack;
            obj.UserDomainName = Environment.UserDomainName;
            obj.Version = Environment.Version.ToString();
            obj.WorkingSet = Environment.WorkingSet;

            return obj;
        }
    }
}
