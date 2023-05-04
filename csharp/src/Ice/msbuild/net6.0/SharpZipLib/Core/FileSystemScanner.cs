using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpZipLib.Core
{
    #region EventArgs

    /// <summary>
    /// Event arguments for scanning.
    /// </summary>
    public class ScanEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initialise a new instance of <see cref="ScanEventArgs"/>
        /// </summary>
        /// <param name="name">The file or directory name.</param>
        public ScanEventArgs(string name)
        {
            name_ = name;
        }

        #endregion Constructors

        /// <summary>
        /// The file or directory name for this event.
        /// </summary>
        public string Name
        {
            get { return name_; }
        }

        /// <summary>
        /// Get set a value indicating if scanning should continue or not.
        /// </summary>
        public bool ContinueRunning
        {
            get { return continueRunning_; }
            set { continueRunning_ = value; }
        }

        #region Instance Fields

        private string name_;
        private bool continueRunning_ = true;

        #endregion Instance Fields
    }

    /// <summary>
    /// Event arguments during processing of a single file or directory.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initialise a new instance of <see cref="ScanEventArgs"/>
        /// </summary>
        /// <param name="name">The file or directory name if known.</param>
        /// <param name="processed">The number of bytes processed so far</param>
        /// <param name="target">The total number of bytes to process, 0 if not known</param>
        public ProgressEventArgs(string name, long processed, long target)
        {
            name_ = name;
            processed_ = processed;
            target_ = target;
        }

        #endregion Constructors

        /// <summary>
        /// The name for this event if known.
        /// </summary>
        public string Name
        {
            get { return name_; }
        }

        /// <summary>
        /// Get set a value indicating whether scanning should continue or not.
        /// </summary>
        public bool ContinueRunning
        {
            get { return continueRunning_; }
            set { continueRunning_ = value; }
        }

        /// <summary>
        /// Get a percentage representing how much of the <see cref="Target"></see> has been processed
        /// </summary>
        /// <value>0.0 to 100.0 percent; 0 if target is not known.</value>
        public float PercentComplete
        {
            get
            {
                float result;
                if (target_ <= 0)
                {
                    result = 0;
                }
                else
                {
                    result = ((float)processed_ / (float)target_) * 100.0f;
                }
                return result;
            }
        }

        /// <summary>
        /// The number of bytes processed so far
        /// </summary>
        public long Processed
        {
            get { return processed_; }
        }

        /// <summary>
        /// The number of bytes to process.
        /// </summary>
        /// <remarks>Target may be 0 or negative if the value isnt known.</remarks>
        public long Target
        {
            get { return target_; }
        }

        #region Instance Fields

        private string name_;
        private long processed_;
        private long target_;
        private bool continueRunning_ = true;

        #endregion Instance Fields
    }

    /// <summary>
    /// Event arguments for directories.
    /// </summary>
    public class DirectoryEventArgs : ScanEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initialize an instance of <see cref="DirectoryEventArgs"></see>.
        /// </summary>
        /// <param name="name">The name for this directory.</param>
        /// <param name="hasMatchingFiles">Flag value indicating if any matching files are contained in this directory.</param>
        public DirectoryEventArgs(string name, bool hasMatchingFiles)
            : base(name)
        {
            hasMatchingFiles_ = hasMatchingFiles;
        }

        #endregion Constructors

        /// <summary>
        /// Get a value indicating if the directory contains any matching files or not.
        /// </summary>
        public bool HasMatchingFiles
        {
            get { return hasMatchingFiles_; }
        }

        private readonly

        #region Instance Fields

        bool hasMatchingFiles_;

        #endregion Instance Fields
    }

    /// <summary>
    /// Arguments passed when scan failures are detected.
    /// </summary>
    public class ScanFailureEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initialise a new instance of <see cref="ScanFailureEventArgs"></see>
        /// </summary>
        /// <param name="name">The name to apply.</param>
        /// <param name="e">The exception to use.</param>
        public ScanFailureEventArgs(string name, Exception e)
        {
            name_ = name;
            exception_ = e;
            continueRunning_ = true;
        }

        #endregion Constructors

        /// <summary>
        /// The applicable name.
        /// </summary>
        public string Name
        {
            get { return name_; }
        }

        /// <summary>
        /// The applicable exception.
        /// </summary>
        public Exception Exception
        {
            get { return exception_; }
        }

        /// <summary>
        /// Get / set a value indicating whether scanning should continue.
        /// </summary>
        public bool ContinueRunning
        {
            get { return continueRunning_; }
            set { continueRunning_ = value; }
        }

        #region Instance Fields

        private string name_;
        private Exception exception_;
        private bool continueRunning_;

        #endregion Instance Fields
    }

    #endregion EventArgs

    #region Delegates

    /// <summary>
    /// Delegate invoked before starting to process a file.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ProcessFileHandler(object sender, ScanEventArgs e);

    /// <summary>
    /// Delegate invoked during processing of a file or directory
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ProgressHandler(object sender, ProgressEventArgs e);

    /// <summary>
    /// Delegate invoked when a file has been completely processed.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The event arguments.</param>
    public delegate void CompletedFileHandler(object sender, ScanEventArgs e);

    /// <summary>
    /// Delegate invoked when a directory failure is detected.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The event arguments.</param>
    public delegate void DirectoryFailureHandler(object sender, ScanFailureEventArgs e);

    /// <summary>
    /// Delegate invoked when a file failure is detected.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The event arguments.</param>
    public delegate void FileFailureHandler(object sender, ScanFailureEventArgs e);

    #endregion Delegates 
}