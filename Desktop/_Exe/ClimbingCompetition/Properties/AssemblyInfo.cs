using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;
using System;

// Information about this assembly is defined by the following
// attributes.
//
// change them to the information which is associated with the assembly
// you compile.
#if FULL
[assembly: AssemblyTitle("Climbing Competition")]
#else
[assembly: AssemblyTitle("Speed Competition")]
#endif
[assembly: AssemblyDescription("Внимание! Данная программа защищена законом об Авторском праве. Незаконное использование, копирование и распространение данной программы запрещено и преследуется по закону.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Спортивная Федерация Скалолазания С.-Петербурга")]
#if FULL
[assembly: AssemblyProduct("ClimbingCompetition 1.4")]
#else
[assembly: AssemblyProduct("SpeedCompetition 0.6")]
#endif
[assembly: AssemblyCopyright("(c) Ivan Kaurov 2006-")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all values by your own or you can build default build and revision
// numbers with the '*' character (the default):
[assembly: AssemblyVersion("1.4.5.1")]
[assembly: NeutralResourcesLanguageAttribute("")]
[assembly: AssemblyFileVersionAttribute("1.4.5")]
[assembly: GuidAttribute("abcdef03-e15d-4b4d-82af-4dff1fb3dbcc")]
