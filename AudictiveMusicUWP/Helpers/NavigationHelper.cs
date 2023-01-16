using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudictiveMusicUWP.Helpers
{
    class NavigationHelper
    {
        public static string GetParameter(string args, string attribute)
        {
            if (string.IsNullOrWhiteSpace(args) == false)
            {
                QueryString arguments = QueryString.Parse(args);

                // See what action is being requested 
                if (arguments.Contains(attribute))
                {
                    return arguments[attribute];
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool ContainsAttribute(string args, string attribute)
        {
            QueryString arguments = QueryString.Parse(args);

            return arguments.Contains(attribute);
        }
    }
}
