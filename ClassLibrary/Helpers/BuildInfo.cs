using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace ClassLibrary.Helpers
{
    public class BuildInfo
    {
        private static BuildInfo _buildInfo;

        private BuildInfo()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                Build = Build.AprilUpdate;
            }
            else if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
            {
                Build = Build.FallCreators;
            }
            else if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4))
            {
                Build = Build.Creators;
            }
            else if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3))
            {
                Build = Build.Anniversary;
            }
            else if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 2))
            {
                Build = Build.Threshold2;
            }
            else if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 1))
            {
                Build = Build.Threshold1;
            }
            else
            {
                Build = Build.Unknown;
            }

            
        }

        public static Build Build { get; private set; }
        public static bool AreEffectsFast { get; private set; }
        public static bool AreEffectsSupported { get; private set; }
        public static bool BeforeAprilUpdate => Build < Build.AprilUpdate;

        public static BuildInfo RetrieveApiInfo() => _buildInfo ?? (_buildInfo = new BuildInfo());
    }

    public enum Build
    {
        Unknown = 0,
        Threshold1 = 1507,   // 10240
        Threshold2 = 1511,   // 10586
        Anniversary = 1607,  // 14393 Redstone 1
        Creators = 1703,     // 15063 Redstone 2
        FallCreators = 1709,  // 16299 Redstone 3
        AprilUpdate = 1803  // 17134 Redstone 4
    }
}
