using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AddInSideViews;
using JetBrains.Annotations;
using Paratext.Data;
using Paratext.Data.ProjectFileAccess;
using Paratext.Data.ProjectSettingsAccess;
using SIL.Scripture;

namespace TvpMain.Import
{
    public class ImportManager
    {
        /// <summary>
        /// Paratext host interface.
        /// </summary>
        [NotNull] private readonly IHost _host;

        /// <summary>
        /// Active project name.
        /// </summary>
        [NotNull] private readonly string _projectName;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="projectName">Active project name (required).</param>
        public ImportManager([NotNull] IHost host, [NotNull] string projectName)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _projectName = projectName ?? throw new ArgumentNullException(nameof(projectName));
        }
    }
}
