// <copyright file="TaskExtensions.cs">
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
using System.Threading;
using System.Threading.Tasks;

namespace Extensions
{
    public static class TaskExtensions
    {
        public static Task<T> AttachAction<T>(this Task<T> task, Action<Task<T>> action, TaskContinuationOptions taskContinuationOptions = TaskContinuationOptions.None)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (action == null)
                return task;

            return task.ContinueWith(tsk => action(tsk), taskContinuationOptions)
                       .ContinueWith(tsk => task, TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap();
        }

        public static Task AttachAction(this Task task, Action<Task> action, TaskContinuationOptions taskContinuationOptions = TaskContinuationOptions.None)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (action == null)
                return task;

            return task.ContinueWith(tsk => action(tsk), taskContinuationOptions)
                       .ContinueWith(tsk => task, TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap();
        }

        public static Task<T> AttachTask<T>(this Task<T> task, Func<Task<T>, Task> continuationFactory, TaskContinuationOptions taskContinuationOptions = TaskContinuationOptions.None)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (continuationFactory == null)
                return task;

            return task.ContinueWith(tsk => continuationFactory(tsk), taskContinuationOptions | TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap()
                       .ContinueWith(tsk => task, TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap();
        }

        public static Task AttachTask(this Task task, Func<Task, Task> continuationFactory, TaskContinuationOptions taskContinuationOptions = TaskContinuationOptions.None)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (continuationFactory == null)
                return task;

            return task.ContinueWith(tsk => continuationFactory(tsk), taskContinuationOptions | TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap()
                       .ContinueWith(tsk => task, TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap();
        }

        public static T ExecuteSynchronuously<T>(this Task<T> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            try { return task.Result; }
            catch (AggregateException aex) { throw aex.Flatten().InnerExceptions.First(); }
        }

        public static void ExecuteSynchronuously(this Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            try { task.Wait(); }
            catch (AggregateException aex) { throw aex.Flatten().InnerExceptions.First(); }
        }

        public static void ProcessMultipleTasksResults(this IEnumerable<Task> src, Boolean checkForFaulted = true, Boolean checkForCancelled = true)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (checkForFaulted)
            {
                var exceptions = new List<Exception>(src.Where(t => t.IsFaulted).SelectMany(t => t.Exception.Flatten().InnerExceptions)).ToArray();
                if (exceptions.Length > 0)
                    throw new AggregateException(exceptions);
            }
            if (checkForCancelled && src.FirstOrDefault(tsk => tsk.IsCanceled) != null)
                throw new OperationCanceledException();
        }
    }
}
