// <copyright file="BaseObject.Ctor.cs">
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
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Concurrent;
using System.Reflection;

namespace DbAccessCore
{
    partial class BaseObject
    {
        public const int IID_SIZE = 25;
        public const int TYPE_SIZE = 512;
        public static readonly Type IID_TYPE = typeof(String);

        readonly Boolean isProxy;

        [NotMapped]
        public Boolean IsProxy { get { return isProxy; } }

        [NotMapped]
        public virtual Boolean NeedLtrForDelete { get { return false; } }

        protected BaseObject()
        {
            this.ObjectClass = this.GetType().FullName;
            this.isProxy = true;
        }

        public BaseObject(BaseContext context)
            : this()
        {
            this.ObjectClass = this.GetType().FullName;
            this.isProxy = false;

            String sIid;
            DateTime dtNow;
            context.GetObjectCreationValues(out sIid, out dtNow);
            this.Iid = sIid;
            this.CreateDate = this.UpdateDate = dtNow;
            this.UserCreated = this.UserUpdated = context.CurrentUserID;
        }

        readonly static ConcurrentDictionary<Type, Tuple<ConstructorInfo, Object>> constructors
            = new ConcurrentDictionary<Type, Tuple<ConstructorInfo, object>>();
        internal static BaseObject CreateEmpty(Type t, String iid)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            if (String.IsNullOrEmpty(iid))
                throw new ArgumentNullException("iid");
            if (t.IsSubclassOf(typeof(BaseObject)) || t.Equals(typeof(BaseObject)))
            {
                var ci = constructors.GetOrAdd(t, tp =>
                    new Tuple<ConstructorInfo, Object>(
                        t.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                         null,
                                         Type.EmptyTypes,
                                         null), new object()));
                if (ci.Item1 == null)
                    return null;
                BaseObject result;
                lock (ci.Item2) { result = (BaseObject)ci.Item1.Invoke(null); }

                result.Iid = iid;
                return result;
            }
            else
                return null;
        }

        internal static BaseObject CreateEmpty(String typeName, String iid)
        {
            var type = Extensions.ObjectExtensions.GetTypeByName(typeName);
            return type == null ? null : CreateEmpty(type, iid);
        }

        protected abstract void RemoveEntity(BaseContext context);
        protected virtual void RemoveLinkedCollections(BaseContext context, Log.LogicTransaction ltr) { }

        public void RemoveObject(BaseContext context, Log.LogicTransaction ltr = null)
        {
            if (context == null)
                return;

            var _ltr = ltr == null ? (this.NeedLtrForDelete ? context.BeginLtr() : null) : ltr;

            RemoveLinkedCollections(context, _ltr);

            RemoveChildCollection(context, this.RightsForThisObject, _ltr);

            if (_ltr != null)
                _ltr.AddDeletedObject(this, context);

            if (_ltr != null && ltr == null)
                _ltr.Commit(context);

            RemoveEntity(context);
        }

        protected void RemoveChildCollection(BaseContext context, IEnumerable<IIIDObject> childCollection, Log.LogicTransaction ltr)
        {
            if (context == null || childCollection == null)
                return;
            childCollection.ToList().ForEach(obj => obj.RemoveObject(context));
        }
    }
}
