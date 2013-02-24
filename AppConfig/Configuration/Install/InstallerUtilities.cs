using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.EnterpriseServices.Internal;
using Microsoft.Win32;
using System.Diagnostics;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Web.Configuration;

namespace AppConfig.Configuration.Install
{
    public static class InstallerUtilities
    {
        public static bool InstallationInProgress
        {
            get
            {
                return Assembly.GetEntryAssembly().FullName.StartsWith("InstallUtil");
            }
        }

        public static Assembly GetCurrentInstallingAssembly()
        {
            StackTrace stackTrace = new StackTrace();
            for (int i = 1; i < stackTrace.FrameCount; i++)
            {
                StackFrame stackFrame = stackTrace.GetFrame(i);
                MethodBase methodBase = stackFrame.GetMethod();
                //Return the assembly for this method if the method is part of a class that inherits the installer class
                if (methodBase.DeclaringType.IsSubclassOf(typeof(System.Configuration.Install.Installer)))
                    return methodBase.DeclaringType.Assembly;
            }
            return null;
        }

        /// <summary>
        /// Get the assembly that was referenced when the installation was started
        /// </summary>
        /// <returns>The installing assembly</returns>
        public static Assembly GetEntryInstallingAssembly()
        {
            StackTrace stackTrace = new StackTrace();
            for (int i = stackTrace.FrameCount - 1; i > -1; i--)
            {
                StackFrame stackFrame = stackTrace.GetFrame(i);
                MethodBase methodBase = stackFrame.GetMethod();
                //Return the assembly for this method if the method is part of a class that inherits the installer class
                if (methodBase.DeclaringType != typeof(System.Configuration.Install.Installer)
                    && !methodBase.DeclaringType.Assembly.FullName.StartsWith("System.Configuration")
                    && methodBase.DeclaringType.IsSubclassOf(typeof(System.Configuration.Install.Installer)))
                    return methodBase.DeclaringType.Assembly;
            }
            return null;
        }

        /// <summary>
        /// Gets the configuration settings for the installing assembly
        /// </summary>
        /// <returns></returns>
        public static System.Configuration.Configuration GetInstallingConfiguration()
        {
            Assembly assembly = GetEntryInstallingAssembly();

            if (IsWebApplication())
            {
                string websiteRootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(assembly.Location));

                VirtualDirectoryMapping vdm = new VirtualDirectoryMapping(websiteRootDirectory, true);
                WebConfigurationFileMap wcfm = new WebConfigurationFileMap();
                wcfm.VirtualDirectories.Add("/", vdm);

                return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
            }
            else
            {
                string configFilePath = Path.Combine(
                    Path.GetDirectoryName(assembly.Location),
                    Path.GetFileNameWithoutExtension(assembly.Location));

                return ConfigurationManager.OpenExeConfiguration(configFilePath);
            }
        }

        /// <summary>
        /// Determines if the installation process is running in a web application
        /// </summary>
        /// <returns></returns>
        public static bool IsWebApplication()
        {
            string assemblyDirectory = Path.GetDirectoryName(GetEntryInstallingAssembly().Location);
            string directoryName = Path.GetFileName(assemblyDirectory);
            if (directoryName == null || directoryName.ToLower() != "bin")
                return false;

            string webRootDirectory = Path.GetDirectoryName(assemblyDirectory);
            if (webRootDirectory == null || !File.Exists(Path.Combine(webRootDirectory, "Web.Config")))
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetWebRootDirectory()
        {
            if (!IsWebApplication())
                return null;

            Assembly assembly = GetEntryInstallingAssembly();
            if (assembly == null)
                return null;

            string assemblyDirectory = Path.GetDirectoryName(assembly.Location);
            string directoryName = Path.GetFileName(assemblyDirectory);
            if (directoryName == null || directoryName.ToLower() != "bin")
                return null;

            string webRootDirectory = Path.GetDirectoryName(assemblyDirectory);
            if (webRootDirectory == null || !File.Exists(Path.Combine(webRootDirectory, "Web.Config")))
                return null;

            return webRootDirectory;
        }

        /// <summary>
        /// Installs the calling assembly into the .Net Global Assembly Cache.  Note: If you want this assembly to be visible to other developers be sure to also call RegisterWithVisualStudio.
        /// </summary>
        public static void InstallIntoGAC()
        {
            InstallIntoGAC(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Installs the assembly into the .Net Global Assembly Cache.  Note: If you want this assembly to be visible to other developers be sure to also call RegisterWithVisualStudio.
        /// </summary>
        /// <param name="assembly"></param>
        public static void InstallIntoGAC(Assembly assembly)
        {
            //Publish the assembly to the global assembly cache
            Publish p = new Publish();
            p.GacInstall(assembly.Location);
        }

        /// <summary>
        /// Removes the calling assebly from the Global Assembly Cache
        /// </summary>
        public static void UninstallFromGAC()
        {
            UninstallFromGAC(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Removes the given assembly from the Global Assembly Cache
        /// </summary>
        /// <param name="assembly"></param>
        public static void UninstallFromGAC(Assembly assembly)
        {
            //Remove the assembly from the global assembly cache
            Publish p = new Publish();
            p.GacRemove(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Registers the calling assembly with Visual Studio so it can be referenced by other assemblies
        /// </summary>
        public static void RegisterWithVisualStudio()
        {
            RegisterWithVisualStudio(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Registers the given assembly with Visual Studio so it can be referenced by other assemblies
        /// </summary>
        /// <param name="assembly"></param>
        public static void RegisterWithVisualStudio(Assembly assembly)
        {
            //Make the assembly visible to Visual Studio
            //HKLM\Software\Microsoft\.NETFramework\[Version]\AssemblyFolderEx
            RegistryKey rkVisualStudioReference = Registry.LocalMachine.CreateSubKey(
                @"Software\Microsoft\.NETFramework\" + assembly.ImageRuntimeVersion + @"\AssemblyFolderEx\" + assembly.GetName().Name);
            //Add the folder to the (Default) string value
            rkVisualStudioReference.SetValue("", Path.GetDirectoryName(assembly.Location));
        }

        /// <summary>
        /// Removes registration of the calling assembly from Visual Studio
        /// </summary>
        public static void UnregisterFromVisualStudio()
        {
            UnregisterFromVisualStudio(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Removes registration of the given assembly from Visual Studio
        /// </summary>
        /// <param name="assembly">The assembly that will be unregisteded from visual studio visiblity</param>
        public static void UnregisterFromVisualStudio(Assembly assembly)
        {
            //Remove the registry entry that makes the assembly visible to Visual Studio
            //HKLM\Software\Microsoft\.NETFramework\[Version]\AssemblyFolderEx
            Registry.LocalMachine.DeleteSubKey(
                @"Software\Microsoft\.NETFramework\" + assembly.ImageRuntimeVersion + @"\AssemblyFolderEx\" + assembly.GetName().Name);
        }

        #region File Association

        /// <summary>
        /// Associates a file extension with the installing assembly.
        /// </summary>
        /// <param name="FileTypeDescription">The description of the file type</param>
        /// <param name="Extension">The extension to use. i.e. pdf for a pdf file</param>
        /// <param name="Icon">The path to an icon that will be displayed with the extension</param>
        public static void AssociateFileExtension(string FileTypeDescription, string Extension, string Icon)
        {
            AssociateFileExtension(GetCurrentInstallingAssembly(), FileTypeDescription, Extension, Icon);
        }

        /// <summary>
        /// Associates a file extension with an installing assembly.
        /// </summary>
        /// <param name="Assembly">The assembly to associate</param>
        /// <param name="FileTypeDescription">The description of the file type</param>
        /// <param name="Extension">The extension to use. i.e. .pdf for a pdf file</param>
        /// <param name="Icon">The path to an icon that will be displayed with the extension</param>
        public static void AssociateFileExtension(Assembly Assembly, string FileTypeDescription, string Extension, string Icon)
        {
            if (!File.Exists(Icon))
                throw new Exception("The icon file '" + Icon + "' to be associated with the extension '" + Extension + "' was not found.");

            string progID = Assembly.GetName().Name + Extension;

            Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\" + progID, null, FileTypeDescription);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\" + progID + @"\DefaultIcon", null, Icon);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\" + progID + @"\shell\open\command", null, Assembly.Location + " \"%1\"");
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\" + Extension, null, progID);

        }

        /// <summary>
        /// Removes the association of a file extension with the installing assembly.
        /// </summary>
        /// <param name="Extension">The extension to use. i.e. pdf for a pdf file</param>
        /// <param name="Icon">The path to an icon that will be displayed with the extension</param>
        public static void DisassociateFileExtension(string Extension)
        {
            DisassociateFileExtension(GetCurrentInstallingAssembly(), Extension);
        }

        public static void DisassociateFileExtension(Assembly Assembly, string Extension)
        {
            string progID = Assembly.GetName().Name + Extension;

            Registry.LocalMachine.DeleteSubKey(@"Software\Classes\" + Extension, false);
            Registry.LocalMachine.DeleteSubKey(@"Software\Classes\" + progID, false);
        }

        // Return true if extension already associated in registry
        public static bool IsAssociated(string extension)
        {
            return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
        }

        [DllImport("Kernel32.dll")]
        private static extern uint GetShortPathName(string lpszLongPath, [Out] StringBuilder lpszShortPath, uint cchBuffer);

        // Return short path format of a file name
        private static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            uint iRet = GetShortPathName(longName, s, iSize);
            return s.ToString();
        }

        #endregion File Association
    }
}
