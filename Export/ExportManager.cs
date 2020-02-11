using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AddInSideViews;
using TvpMain.Project;
using TvpMain.Result;
using TvpMain.Util;

namespace TvpMain.Export
{
    /// <summary>
    /// Saves Paratext project content to a target folder or back to the project.
    /// </summary>
    public class ExportManager : IDisposable
    {
        /// <summary>
        /// Paratext host interface.
        /// </summary>
        private readonly IHost _host;

        /// <summary>
        /// Active project name.
        /// </summary>
        private readonly string _activeProjectName;

        /// <summary>
        /// Project settings manager.
        /// </summary>
        private readonly ProjectManager _projectManager;

        /// <summary>
        /// Check results manager.
        /// </summary>
        private readonly ResultManager _resultManager;

        /// <summary>
        /// Current run's task semaphore.
        /// </summary>
        private SemaphoreSlim _runSemaphore;

        /// <summary>
        /// Cancellation token source.
        /// </summary>
        private CancellationTokenSource _runTokenSource;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        /// <param name="projectManager">Project settings manager (required).</param>
        /// <param name="resultManager">Check result manager (required).</param>
        public ExportManager(IHost host, string activeProjectName,
            ProjectManager projectManager,
            ResultManager resultManager)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));
            _projectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
            _resultManager = resultManager
                                  ?? throw new ArgumentNullException(nameof(resultManager));
        }

        /// <summary>
        /// Export project as USFM to a target directory, replacing parts of verses
        /// with check result suggestions that haven't been ignored.
        /// </summary>
        /// <param name="targetDir">Target directory (required).</param>
        /// <param name="isChangesOnly">True to only export books with result suggestions that haven't been ignored, false to export all books.</param>
        public void ExportProject(
            DirectoryInfo targetDir,
            bool isChangesOnly)
        {
            // set up semaphore and cancellation token to control execution and termination
            RecycleRunSemaphore(true);
            RecycleRunTokenSource(true);

            _resultManager.TryGetBookResults(null, null, isChangesOnly,
                _projectManager.PresentBookNums, out var bookResults);
        }

        /// <summary>
        /// Cancels a running check.
        /// </summary>
        public void CancelChecks()
        {
            _runTokenSource?.Cancel();
        }

        /// <summary>
        /// Recycle (dispose and optionally re-create) run semaphore.
        /// </summary>
        /// <param name="isCreateNew">True to create a new one, false to only dispose any existing one.</param>
        private void RecycleRunSemaphore(bool isCreateNew)
        {
            _runSemaphore?.Dispose();
            _runSemaphore = isCreateNew
                ? new SemaphoreSlim(MainConsts.MAX_EXPORT_THREADS)
                : null;
        }


        /// <summary>
        /// Recycle (dispose and optionally re-create) run cancellation token.
        /// </summary>
        /// <param name="isCreateNew">True to create a new one, false to only dispose any existing one.</param>
        private void RecycleRunTokenSource(bool isCreateNew)
        {
            _runTokenSource?.Dispose();
            _runTokenSource = isCreateNew
                ? new CancellationTokenSource()
                : null;
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            RecycleRunSemaphore(false);
            RecycleRunTokenSource(false);
        }
    }
}
