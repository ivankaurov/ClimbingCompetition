// <copyright file="Program.cs">
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
// <date>30.08.2016 23:34</date>

namespace CopyrightHeaderEditor
{
    using System;
    using System.IO;
    using System.Text;
    internal sealed class Program
    {
        private const string ProgramName = "ClimbingCompetition";

        private const string CopyrightENU =
@"// This file is part of " + Program.ProgramName + @".
//
//  " + Program.ProgramName + @" is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  " + Program.ProgramName + @" is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with " + Program.ProgramName + @".  If not, see <http://www.gnu.org/licenses/>.";

        private const string CopyrightRUS =
@"// (Этот файл — часть " + Program.ProgramName + @".
// 
// " + Program.ProgramName + @" - свободная программа: вы можете перераспространять ее и/или
// изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
// в каком она была опубликована Фондом свободного программного обеспечения;
// либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
// версии.
// 
// " + Program.ProgramName + @" распространяется в надежде, что она будет полезной,
// но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
// или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
// общественной лицензии GNU.
// 
// Вы должны были получить копию Стандартной общественной лицензии GNU
// вместе с этой программой. Если это не так, см. <http://www.gnu.org/licenses/>.)";

        private const string CopyrightStartTag = "// <copyright";

        private static readonly string CopyrightAll =
            string.Format("// <copyright file=\"{{0}}\">{0}// Copyright © {1} All Rights Reserved{0}" +
                Program.CopyrightENU + "{0}//{0}" + Program.CopyrightRUS +
                "{0}// </copyright>{0}// <author>Ivan Kaurov</author>{0}// <date>{2:dd.MM.yyyy HH:mm}</date>",
                Environment.NewLine,
                DateTime.Now.Year,
                DateTime.Now);

        static void Main(string[] args)
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (!directory.Name.Equals("ClimbingCompetition", StringComparison.OrdinalIgnoreCase))
            {
                if (directory.Parent == null)
                {
                    return;
                }

                directory = directory.Parent;
            }

            Program.ProcessDirectory(directory);
        }

        private static void ProcessDirectory(DirectoryInfo directory)
        {
            foreach (var csFile in directory.EnumerateFiles("*.cs"))
            {
                if (!csFile.Name.ToLowerInvariant().EndsWith("designer.cs"))
                {
                    Program.ProcessCSfile(csFile);
                }
            }

            foreach (var dir in directory.EnumerateDirectories())
            {
                Program.ProcessDirectory(dir);
            }
        }

        private static void ProcessCSfile(FileInfo fileInfo)
        {
            Encoding encoding;

            using (var file = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = new StreamReader(file))
                {
                    var firstLine = streamReader.ReadLine();
                    if (firstLine == null || firstLine.StartsWith(Program.CopyrightStartTag, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    encoding = streamReader.CurrentEncoding;
                }
            }

            var fileCopyright = string.Format(Program.CopyrightAll, fileInfo.Name);

            var fileContents = encoding.GetBytes(fileCopyright + Environment.NewLine + Environment.NewLine);

            using (var file = fileInfo.OpenRead())
            {
                var buffer = new byte[1024];
                using(var mstr = new MemoryStream())
                {
                    mstr.Write(fileContents, 0, fileContents.Length);

                    int n;
                    while((n = file.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        mstr.Write(buffer, 0, n);
                    }

                    fileContents = mstr.ToArray();
                }
            }

            using(var file = File.Open(fileInfo.FullName, FileMode.Create, FileAccess.Write))
            {
                file.Write(fileContents, 0, fileContents.Length);
            }
        }
    }
}
