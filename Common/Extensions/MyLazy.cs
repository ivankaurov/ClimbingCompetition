// <copyright file="MyLazy.cs">
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
    public sealed class MyLazy<T> : IDisposable
    {
        static readonly bool isDisposable;
        static MyLazy()
        {
            isDisposable = typeof(T).GetInterfaces().Contains(typeof(IDisposable));
        }

        readonly object syncRoot = new object();
        readonly Func<T> valueFactory;

        bool valueCreated = false;
        bool objectDisposed = false;
        T value;

        public bool ValueCreated { get { return this.valueCreated; } }

        public MyLazy(Func<T> valueFactory)
        {
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");
            this.valueFactory = valueFactory;
        }

        public void Dispose()
        {
            if (!objectDisposed)
            {
                if (isDisposable)
                    lock (syncRoot)
                    {
                        if (this.valueCreated)
                            ((IDisposable)value).Dispose();
                    }
                this.objectDisposed = true;
            }
        }

        public T Value
        {
            get
            {
                return this.GetOrCreateValue();
            }
        }

        public T GetOrCreateValue()
        {
            if (this.objectDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            lock (syncRoot)
            {
                if (!valueCreated)
                {
                    this.value = valueFactory();
                    this.valueCreated = true;
                }
                return this.value;
            }
        }

        public void ResetValue()
        {
            if (objectDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            lock (syncRoot)
            {
                if(this.valueCreated)
                {
                    if (isDisposable)
                        ((IDisposable)value).Dispose();
                    this.value = default(T);
                    this.valueCreated = false;
                }
            }
        }
    }
}
