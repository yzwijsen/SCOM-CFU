using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Monitoring;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var scomHost = "localhost";
            ManagementGroup mg = null;
            try
            {
                mg = ManagementGroup.Connect(scomHost);

                Console.WriteLine($"Connecting to {scomHost}");

                if (mg.IsConnected)
                {
                    Console.WriteLine("Connection succeeded.");
                }
                else
                {
                    throw new InvalidOperationException("Not connected to an SDK Service.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect");
                Console.WriteLine(ex.Message);
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Get All Rules
            IList<ManagementPackRule> rules = mg.Monitoring.GetRules();

            foreach(var rule in rules)
            {
                Console.WriteLine($"Rule ID: {rule.Id}");
                Console.WriteLine($"Rule Name: {rule.DisplayName}");
                Console.WriteLine($"Rule Target: {rule.Target.ToString()}");

                //Get GUID out of Alert Target field
                var ruleTargetText = rule.Target.ToString();
                var targetID = ruleTargetText.Substring(ruleTargetText.LastIndexOf('=') + 1);

                var testTargetClass = mg.EntityTypes.GetClass(Guid.Parse(targetID));

                Console.WriteLine($"Target ID: {targetID}");
                Console.WriteLine($"Target Name: {testTargetClass.DisplayName}");

                var mp = rule.GetManagementPack();
                Console.WriteLine($"MP ID: {mp.Id.ToString()}");
                Console.WriteLine($"MP Name: {mp.DisplayName}");
                Console.WriteLine("-----------------------------------------");
            }

            //Get All Monitors
            IList<ManagementPackMonitor> monitors = mg.Monitoring.GetMonitors();

            foreach(var monitor in monitors)
            {
                Console.WriteLine($"Monitor ID: {monitor.Id}");
                Console.WriteLine($"Monitor Name: {monitor.DisplayName}");
                Console.WriteLine($"Target Text: {monitor.Target.ToString()}");

                //Get GUID out of Alert Target field
                var monitorTargetText = monitor.Target.ToString();
                var targetID = monitorTargetText.Substring(monitorTargetText.LastIndexOf('=') + 1);

                var testTargetClass = mg.EntityTypes.GetClass(Guid.Parse(targetID));

                Console.WriteLine($"Target ID: {targetID}");
                Console.WriteLine($"Target Name: {testTargetClass.DisplayName}");

                var mp = monitor.GetManagementPack();
                Console.WriteLine($"MP ID: {mp.Id}");
                Console.WriteLine($"MP Name: {mp.DisplayName}");
                Console.WriteLine("-----------------------------------------");
            }

            //Get All Groups
            //var test = mg.GetMonitoringClass(Guid.Parse("test"));
            IList<MonitoringObjectGroup> scomGroups = mg.EntityObjects.GetRootObjectGroups<MonitoringObjectGroup>(ObjectQueryOptions.Default);

            foreach(var scomGroup in scomGroups)
            {
                Console.WriteLine($"Group ID: {scomGroup.Id.ToString()}");
                Console.WriteLine($"Group DisplayName: {scomGroup.DisplayName}");
                Console.WriteLine("-----------------------------------------");
            }



            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"Completed in {elapsedTime}");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"Found {rules.Count} rule(s)");
            Console.WriteLine($"Found {monitors.Count} monitor(s)");
            Console.WriteLine($"Found {scomGroups.Count} group(s)");
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("Done. Press Enter to close...");
            Console.ReadLine();

        }
    }
}
