using System.Reflection;
using System.EnterpriseServices;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: ApplicationName("AppConfig")]
[assembly: ApplicationActivation(ActivationOption.Server)]
[assembly: ApplicationAccessControl(false, AccessChecksLevel=AccessChecksLevelOption.ApplicationComponent)]
#pragma warning disable 1699
[assembly: AssemblyKeyFile("StrongName.snk")]
[assembly: AssemblyTitle("Application Deployment Utilities")]
[assembly: AssemblyDescription("Time Saving Application Deployment Utilities")]
[assembly: AssemblyProduct("Time Saving Application Deployment Utilities")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("The app.config Project")]
[assembly: AssemblyCopyright("Copyright ©  2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("17947c7c-d484-41b6-abbb-741e369fcf90")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0")]
[assembly: AssemblyFileVersion("1.0.2")]
[assembly: AssemblyInformationalVersion("1.0 Beta 2 - Jan 4 2012")]