using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace AppConfig.Configuration.Install
{
    public class AssemblyInstaller
    {
        #region Private Variables
        private Process process;
        #endregion Private Variables

        #region Constructors
        public AssemblyInstaller(string AssemblyFileName)
        {
            this.Assembly = Assembly.LoadFile(AssemblyFileName);
        }
        public AssemblyInstaller(Assembly Assembly)
        {
            this.Assembly = Assembly;
        }
        #endregion Constructors

        #region Properties
        public Assembly Assembly { get; set; }
        public StreamReader OutputStreamReader { get; private set; }
        private Stream OutputStream;
        private StreamWriter OutputStreamWriter;

        public string InstallerFilePath
        {
            get
            {
                Version frameworkVersion = GetFrameworkVersion(Assembly);
                return Path.Combine(
                    Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                    "Microsoft.NET\\Framework\\" +
                    "v" + frameworkVersion.Major + "." + frameworkVersion.Minor + "." + frameworkVersion.Build +
                    "\\InstallUtil.exe");
            }
        }

        public string InstallerArguments
        {
            get
            {
                return "/LogToConsole=true /ShowCallStack=true \"" + Assembly.Location + "\"";
            }
        }

        public string CommandLineText
        {
            get { return "\"" + InstallerFilePath + "\" " + InstallerArguments; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Runs the InstallUtil.exe for the Assembly in the Assembly property.
        /// </summary>
        /// <returns>The standard output of the InstallUtil process</returns>
        public void RunAssemblyInstaller()
        {                
            //Initialize the Output Stream
            OutputStream = new MemoryStream();
            OutputStreamReader = new StreamReader(OutputStream);
            OutputStreamWriter = new StreamWriter(OutputStream);

            //Run the install util tool
            ProcessStartInfo processStartInfo = new ProcessStartInfo(InstallerFilePath, InstallerArguments);
            processStartInfo.WorkingDirectory = Path.GetDirectoryName(InstallerFilePath);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            process = new Process();
            process.StartInfo = processStartInfo;
            //process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
            //process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
            //process.Exited += new EventHandler(process_Exited);
            process.Start();

            new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(process_OutputDataReceived)).Start(process);
            new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(process_ErrorDataReceived)).Start(process);
        }

        void process_OutputDataReceived(object processObject)
        {
            Process process = (Process)processObject;
            char[] buffer = new char[100];
            while (true)
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    int count = process.StandardOutput.Read(buffer, 0, buffer.Length);
                    if (InstallerOutputReceived != null)
                        InstallerOutputReceived.Invoke(this,  new StringDataEventArgs(new string(buffer, 0, count)));
                }

                if (process.HasExited && process.StandardOutput.EndOfStream)
                    return;

                System.Threading.Thread.Sleep(100);
            }
        }

        void process_ErrorDataReceived(object processObject)
        {
            Process process = (Process)processObject;
            char[] buffer = new char[100];
            while (true)
            {
                while (!process.StandardError.EndOfStream)
                {
                    int count = process.StandardError.Read(buffer, 0, buffer.Length);
                    if (InstallerOutputReceived != null)
                        InstallerOutputReceived.Invoke(this, new StringDataEventArgs(new string(buffer, 0, count)));
                }

                if (process.HasExited && process.StandardOutput.EndOfStream)
                    return;

                System.Threading.Thread.Sleep(100);
            }
        }

        void process_Exited(object sender, EventArgs e)
        {
            //if (InstallerComplete != null)
            //    InstallerComplete.Invoke(this, e);
        }
        #endregion Methods

        #region Events
        public event EventHandler<StringDataEventArgs> InstallerOutputReceived;
        public event EventHandler<StringDataEventArgs> InstallerComplete;
        #endregion Events

        #region Static Methods
        /// <summary>
        /// This method is stubbed in and always returns 4.0.  Needs work.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public Version GetFrameworkVersion(Assembly assembly)
        {
            return new Version(4, 0, 30319);
        }
        #endregion Static Methods
    }

    public class StringDataEventArgs : EventArgs
    {
        public StringDataEventArgs(string Data)
        {
            this.Data = Data;
        }

        public string Data { get; private set; }
    }
}
