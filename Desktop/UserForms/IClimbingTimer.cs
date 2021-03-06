// <copyright file="IClimbingTimer.cs">
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
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace ClimbingCompetition
{
    public abstract class AClimbingTimer : IDisposable
    {
        const int INTERNAL_TIMER_INTERVAL = 50;

        readonly TimeSpan rotationInterval;

        public TimeSpan RotationInterval
        {
            get { return rotationInterval; }
        }

        TimeSpan rInterval;

        public TimeSpan RemainingInterval { get { return rInterval; } }
        readonly SqlConnection cn;

        protected SqlConnection Connection { get { return cn; } }


        public event EventHandler<ClmTimerEventArgs> SecondPassed;
        public event EventHandler GetReady;
        public event EventHandler Completed;
        public event EventHandler<IntEventArgs> MinutePassed;

        private void OnSimpleEvent(EventHandler eventHandler)
        {
            var evT = Interlocked.CompareExchange(ref eventHandler, null, null);
            if (evT != null)
                evT(this, new EventArgs());
        }

        private void OnTypedEvent<T>(EventHandler<T> eventHandler, T eventArgs) where T : EventArgs
        {
            var evT = Interlocked.CompareExchange(ref eventHandler, null, null);
            if (evT != null)
                evT(this, eventArgs);
        }
        
        protected void OnSecondPassed()
        {
            OnTypedEvent(SecondPassed, new ClmTimerEventArgs(rotationInterval, RemainingInterval, IsRunning));
        }

        protected void OnGetReady() { OnSimpleEvent(GetReady); }
        protected void OnCompleted() { OnSimpleEvent(Completed); }
        protected void OnMinutePassed() { OnTypedEvent(MinutePassed, new IntEventArgs((int)Math.Round(RemainingInterval.TotalMinutes, 0))); }

        bool isRunning = false;
        public virtual Boolean IsRunning { get { return isRunning; } }

        readonly Timer timer;

        public AClimbingTimer(String connectionString, TimeSpan rotationInterval)
        {
            this.rInterval = this.rotationInterval = rotationInterval;
            this.cn = new SqlConnection(connectionString);
            this.cn.Open();
            timer = new Timer(TimerTick);
        }

        protected virtual void OnLocalTimerEvent() { }

        public Boolean AutoReset { get; set; }

        DateTime? lastTick = null;
        void TimerTick(object state)
        {
            if (!isRunning)
                return;
            try
            {
                var now = DateTime.Now;
                if (lastTick.HasValue && lastTick.Value.Second != now.Second || !lastTick.HasValue)
                {
                    OnSecondPassed();
                    rInterval = lastTick.HasValue ? (rInterval - (now - lastTick.Value)) : rotationInterval;
                    if (rInterval.CompareTo(TimeSpan.Zero) <= 0)
                    {
                        rInterval = TimeSpan.Zero;
                        OnMinutePassed();
                        OnCompleted();
                        if (AutoReset)
                            Reset();
                        else
                        {
                            this.isRunning = false;
                            return;
                        }
                    }
                    else if (12 == (int)Math.Round(rInterval.TotalSeconds))
                        OnGetReady();
                    else if (rInterval.Minutes > 0 && rInterval.Seconds == 2)
                        OnMinutePassed();
                }
                lastTick = now;
                ((state as Timer) ?? timer).Change(INTERNAL_TIMER_INTERVAL, Timeout.Infinite);
            }
            catch (ObjectDisposedException) { }
        }

        public virtual void Start()
        {
            if (!this.isRunning)
            {
                this.isRunning = true;
                timer.Change(0, Timeout.Infinite);
            }
        }

        public virtual void Stop()
        {
            this.isRunning = false;
        }

        public virtual void Reset()
        {
            this.lastTick = null;
            this.rInterval = rotationInterval;
        }

        #region IDIsposabe
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                timer.Dispose();
                cn.Dispose();
            }
        }

        ~AClimbingTimer() { Dispose(false); }
        #endregion
    }

    [Serializable]
    public sealed class ClmTimerEventArgs : EventArgs
    {
        readonly TimeSpan rotationInterval, remainingTime;

        public TimeSpan RemainingTime
        {
            get { return remainingTime; }
        } 


        public TimeSpan RotationInterval
        {
            get { return rotationInterval; }
        }

        readonly Boolean isRunning;

        public Boolean IsRunning
        {
            get { return isRunning; }
        }

        public ClmTimerEventArgs(TimeSpan rotationInterval, TimeSpan remainingTime, Boolean isRunning)
        {
            this.remainingTime = remainingTime;
            this.rotationInterval = rotationInterval;
            this.isRunning = isRunning;
        }
    }

    [Serializable]
    public sealed class IntEventArgs : EventArgs
    {
        readonly int value;
        public int Value { get { return value; } }
        public IntEventArgs(int value) { this.value = value; }
    }
}
