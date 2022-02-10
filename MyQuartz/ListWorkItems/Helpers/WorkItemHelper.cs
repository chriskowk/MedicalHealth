using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace ListWorkItems
{
    class WorkItemHelper
    {
        public static void List(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: WorkItemHelper.List <URL for TFS> <server path>");
                //Environment.Exit(1);
                return;
            }

            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(args[0]));
            VersionControlServer vcs = tpc.GetService<VersionControlServer>();

            // Get the changeset artifact URIs for each changeset in the history query
            List<string> changesetArtifactUris = new List<string>();

            foreach (Object obj in vcs.QueryHistory(args[1],                       // path we care about ($/project/whatever) 
                                                    VersionSpec.Latest,            // version of that path
                                                    0,                             // deletion ID (0 = not deleted) 
                                                    RecursionType.Full,            // entire tree - full recursion
                                                    null,                          // include changesets from all users
                                                    new ChangesetVersionSpec(1),   // start at the beginning of time
                                                    VersionSpec.Latest,            // end at latest
                                                    100,                            // only return these max count rows
                                                    false,                         // we don't want the files changed
                                                    true))                         // do history on the path
            {
                Changeset c = obj as Changeset;
                changesetArtifactUris.Add(c.ArtifactUri.AbsoluteUri);
            }

            // We'll use the linking service to get information about the associated work items
            ILinking linkingService = tpc.GetService<ILinking>();
            LinkFilter linkFilter = new LinkFilter();
            linkFilter.FilterType = FilterType.ToolType;
            linkFilter.FilterValues = new string[1] { ToolNames.WorkItemTracking };  // we only want work items

            // Convert the artifact URIs for the work items into strongly-typed objects holding the properties rather than name/value pairs 
            Artifact[] artifacts = linkingService.GetReferencingArtifacts(changesetArtifactUris.ToArray(), new LinkFilter[1] { linkFilter });
            AssociatedWorkItemInfo[] workItemInfos = AssociatedWorkItemInfo.FromArtifacts(artifacts);

            // Here we'll just print the IDs and titles of the work items
            foreach (AssociatedWorkItemInfo workItemInfo in workItemInfos.OrderByDescending(a => a.Id).ToList())
            {
                Console.WriteLine("Id: " + workItemInfo.Id + " Title: " + workItemInfo.Title);
            }
        }
    }

    internal class AssociatedWorkItemInfo
    {
        private int _id;
        private string _title;
        private string _assignedTo;
        private string _type;
        private string _state;

        private AssociatedWorkItemInfo()
        {
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public string AssignedTo
        {
            get
            {
                return _assignedTo;
            }
        }

        public string WorkItemType
        {
            get
            {
                return _type;
            }
        }

        public string State
        {
            get
            {
                return _state;
            }
        }

        internal static AssociatedWorkItemInfo[] FromArtifacts(IEnumerable<Artifact> artifacts)
        {
            if (null == artifacts)
            {
                return new AssociatedWorkItemInfo[0];
            }

            List<AssociatedWorkItemInfo> toReturn = new List<AssociatedWorkItemInfo>();

            foreach (Artifact artifact in artifacts)
            {
                if (artifact == null)
                {
                    continue;
                }

                AssociatedWorkItemInfo awii = new AssociatedWorkItemInfo();

                // Convert the name/value pairs into strongly-typed objects containing the work item info 
                foreach (ExtendedAttribute ea in artifact.ExtendedAttributes)
                {
                    if (string.Equals(ea.Name, "System.Id", StringComparison.OrdinalIgnoreCase))
                    {
                        int workItemId;

                        if (Int32.TryParse(ea.Value, out workItemId))
                        {
                            awii._id = workItemId;
                        }
                    }
                    else if (string.Equals(ea.Name, "System.Title", StringComparison.OrdinalIgnoreCase))
                    {
                        awii._title = ea.Value;
                    }
                    else if (string.Equals(ea.Name, "System.AssignedTo", StringComparison.OrdinalIgnoreCase))
                    {
                        awii._assignedTo = ea.Value;
                    }
                    else if (string.Equals(ea.Name, "System.State", StringComparison.OrdinalIgnoreCase))
                    {
                        awii._state = ea.Value;
                    }
                    else if (string.Equals(ea.Name, "System.WorkItemType", StringComparison.OrdinalIgnoreCase))
                    {
                        awii._type = ea.Value;
                    }
                }

                Debug.Assert(0 != awii._id, "Unable to decode artifact into AssociatedWorkItemInfo object.");

                if (0 != awii._id)
                {
                    toReturn.Add(awii);
                }
            }

            return toReturn.ToArray();
        }
    }
}

