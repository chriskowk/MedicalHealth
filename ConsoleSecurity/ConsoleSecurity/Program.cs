using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleSecurity
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain myDomain = Thread.GetDomain();

            myDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal myPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;
            Console.WriteLine("{0} belongs to: ", myPrincipal.Identity.Name.ToString());
            Array wbirFields = Enum.GetValues(typeof(WindowsBuiltInRole));
            foreach (object roleName in wbirFields)
            {
                try
                {
                    // Cast the role name to a RID represented by the WindowsBuildInRole value.
                    Console.WriteLine("{0}? {1}.", roleName,
                        myPrincipal.IsInRole((WindowsBuiltInRole)roleName));
                    Console.WriteLine("The RID for this role is: " + ((int)roleName).ToString());

                }
                catch (Exception)
                {
                    Console.WriteLine("{0}: Could not obtain role for this RID.",
                        roleName);
                }
            }
            // Get the role using the string value of the role.
            Console.WriteLine("{0}? {1}.", "Administrators",
                myPrincipal.IsInRole("BUILTIN\\" + "Administrators"));
            Console.WriteLine("{0}? {1}.", "Users",
                myPrincipal.IsInRole("BUILTIN\\" + "Users"));
            // Get the role using the WindowsBuiltInRole enumeration value.
            Console.WriteLine("{0}? {1}.", WindowsBuiltInRole.Administrator,
               myPrincipal.IsInRole(WindowsBuiltInRole.Administrator));
            // Get the role using the WellKnownSidType.
            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            Console.WriteLine("WellKnownSidType BuiltinAdministratorsSid  {0}? {1}.", sid.Value, myPrincipal.IsInRole(sid));

            IList<string> ret = LocalGroupHelper.LocalGroupNameEnum();

            Console.ReadKey();
        }
    }
}
