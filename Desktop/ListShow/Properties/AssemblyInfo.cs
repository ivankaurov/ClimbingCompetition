using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ListShow")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
#if FULL
[assembly: AssemblyProduct("ListShow for ClimbingCompetition")]
#else
[assembly: AssemblyProduct("ListShow for SpeedCompetition")]
#endif
[assembly: AssemblyCopyright("Copyright ©  2006-2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0d66353a-e0ba-4520-9e56-12aa9776f019")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
#if DEBUG
#if FULL
[assembly: AssemblyVersion("1.1.4.1")]
#else
[assembly: AssemblyVersion("0.6.4.0")]
#endif
#else
#if FULL
[assembly: AssemblyVersion("1.1.4.1")]
#else
[assembly: AssemblyVersion("0.6.1.1")]
#endif
#endif
#if FULL
[assembly: AssemblyFileVersionAttribute("1.1.4")]
#else
[assembly: AssemblyFileVersionAttribute("0.6.4")]
#endif
[assembly: NeutralResourcesLanguageAttribute("ru")]
