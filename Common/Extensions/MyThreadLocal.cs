// <copyright file="MyThreadLocal.cs">
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public class MyThreadLocal<T> : IDisposable
    {
        readonly MyConcurrentDictionary<int, T> threadLocalValues = new MyConcurrentDictionary<int, T>(); 
        static readonly Boolean isDisposable;
        static MyThreadLocal()
        {
            isDisposable = typeof(T).GetInterfaces().Contains(typeof(IDisposable));
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (isDisposable && disposing)
                foreach (var dV in threadLocalValues.Values.Where(v => v != null))
                    ((IDisposable)dV).Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~MyThreadLocal() { this.Dispose(false); }

        readonly Func<T> valueFactory;
        public MyThreadLocal(Func<T> valueFactory)
        {
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");
            this.valueFactory = valueFactory;
        }

        public T Value
        {
            get
            {
                return threadLocalValues.GetOrAdd(System.Threading.Thread.CurrentThread.ManagedThreadId, key => valueFactory());
            }
        }
    }
}
