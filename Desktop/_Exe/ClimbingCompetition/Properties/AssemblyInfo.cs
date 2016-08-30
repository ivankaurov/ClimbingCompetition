// <copyright file="AssemblyInfo.cs">
// Copyright © 2016 All Rights Reserved
// This file is part of ClimbingCompetition.
//
//  ClimbingCompetition is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  ClimbingCompetition is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with ClimbingCompetition.  If not, see <http://www.gnu.org/licenses/>.
//
// (Этот файл — часть ClimbingCompetition.
// 
// ClimbingCompetition - свободная программа: вы можете перераспространять ее и/или
// изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
// в каком она была опубликована Фондом свободного программного обеспечения;
// либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
// версии.
// 
// ClimbingCompetition распространяется в надежде, что она будет полезной,
// но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
// или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
// общественной лицензии GNU.
// 
// Вы должны были получить копию Стандартной общественной лицензии GNU
// вместе с этой программой. Если это не так, см. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

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
[assembly: AssemblyDescription("��������! ������ ��������� �������� ������� �� ��������� �����. ���������� �������������, ����������� � ��������������� ������ ��������� ��������� � ������������ �� ������.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("���������� ��������� ������������ �.-����������")]
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
