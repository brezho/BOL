using X.AspNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet
{
    public static class Operator
    {
        private static string[] SuperUsers = new string[]
        {
            @"cnw\cnwgl3",
        };

        public static string Current
        {
            get
            {
                if (IsRunningAs())
                {
                    return ContextHelper.Get<string>(ContextKey.RunAsUserName);
                }
                return Real;
            }
        }

        public static string Real
        {
            get
            {
                return IsSystemUser() ? ContextKey.FrameworkService : ContextHelper.GetUserName();
            }
        }

        public static bool IsFrameworkService()
        {
            return Operator.Current ==  Operator.ContextKey.FrameworkService;
        }

        /// <summary>
        /// Determines whether the "run as" mode is starting.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the run as mode is starting; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRunningAs()
        {
            return ContextHelper.Get<string>(ContextKey.RunAsUserName).IsFilled();
        }

        /// <summary>
        /// Starts the "run as" mode.
        /// </summary>
        /// <param name="runAsUserName">Name of the user.</param>
        public static void RunAs(string runAsUserName)
        {
            ContextHelper.Set(ContextKey.RunAsUserName, runAsUserName, true);
        }

        /// <summary>
        /// Starts the "run as service" mode.
        /// </summary>
        public static void RunAsService()
        {
            RunAs(ContextKey.FrameworkService);
        }

        /// <summary>
        /// Starts the "run as public" mode.
        /// </summary>
        public static void RunAsPublic()
        {
            RunAs(ContextKey.FrameworkPublic);
        }

        /// <summary>
        /// Stops the "run as" mode.
        /// </summary>
        public static void StopRunningAs()
        {
            ContextHelper.Remove(ContextKey.RunAsUserName);
        }

        public static bool IsSuperUser()
        {
            // Bypass the run as mode
            string userName = ContextHelper.GetUserName().ToLowerInvariant();
            return SuperUsers.Any(u => u.ToLowerInvariant() == userName);
        }

        private static bool IsSystemUser()
        {
            try
            {
                NTAccount ntAccount = new NTAccount(ContextHelper.GetUserName());
                if (ntAccount != null)
                {
                    SecurityIdentifier currentSI = (SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier));
                    return currentSI.IsWellKnown(WellKnownSidType.LocalSystemSid) ||
                           currentSI.IsWellKnown(WellKnownSidType.NTAuthoritySid) ||
                           currentSI.IsWellKnown(WellKnownSidType.NetworkServiceSid) ||
                           currentSI.IsWellKnown(WellKnownSidType.LocalServiceSid);
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        internal class ContextKey
        {
            internal static readonly string RunAsUserName = "RunAsUserName";
            internal static readonly string UserName = "UserName";
            internal static readonly string FrameworkService = "FrameworkService";
            internal static readonly string FrameworkPublic = "FrameworkPublic";
        }
    }
}
