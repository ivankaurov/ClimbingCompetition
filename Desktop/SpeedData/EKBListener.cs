// <copyright file="EKBListener.cs">
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

﻿//#if DEBUG
//#undef DEBUG
//#endif

#if DEBUG
#define WRITE_DIRECTLY_TO_BUFFER
#define ALLOW_RANDOM_DATA
#define RANDOM_DELAY
//#define BOULDER_TEST
#endif
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace ClimbingCompetition.SpeedData
{
    /// <summary>
    /// Класс отвечает за приём данных с ёбургской системы (скорость + задел под боулдеринг)
    /// </summary>
    public class EKBListener : SystemListener
    {
        protected SerialPort sp;
#if WRITE_DIRECTLY_TO_BUFFER
        private Thread thr = null;
#endif
#if ALLOW_RANDOM_DATA
        private Thread wrThr = null;
#endif
#if BOULDER_TEST
        private System.Timers.Timer t = new System.Timers.Timer(500);
        private const int TIME = 2;
        private int minRem = TIME, secRem = 0;
#endif
        private bool useOld;

        public string PortName
        {
            get
            {
                try { return sp.PortName; }
                catch { return ""; }
            }
        }

        private const byte H0 = 0x1b, H1 = 0xb1;

        public EKBListener(SerialPort sp, bool oldOne, Form owner, bool showErrors, Action<bool> persistShowErrors)
            : base(showErrors, persistShowErrors, owner)
        {
            this.sp = sp;
            sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            this.useOld = oldOne;
#if BOULDER_TEST
            t.AutoReset = true;
#endif
        }
#if BOULDER_TEST
        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            secRem--;
            if (secRem < 0)
            {
                secRem = 59;
                minRem--;
            }
            if (minRem < 0)
            {
                minRem = TIME - 1;
                secRem = 59;
            }
            byte[] b = GeneratePackageBoulder();
            m.WaitOne();
            try
            {
#if WRITE_DIRECTLY_TO_BUFFER
                foreach (byte bt in b)
                    unprocessed.Add(bt);
#else
                sp.Write(b, 0, b.Length);
#endif
            }
            finally { m.ReleaseMutex(); }
        }
#endif
        Mutex mPort = new Mutex();
        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            listenInner();
        }

        private bool GetDataFromPort()
        {
            byte[] buffer;
            mPort.WaitOne();
            int bytesRead;
            try
            {
#if DEBUG
                try
                {
#endif
                    if (sp.BytesToRead < 1)
                        return false;
                    buffer = new byte[sp.BytesToRead];
                    bytesRead = sp.Read(buffer, 0, buffer.Length);
#if DEBUG
                }
                catch
                {
                    return false;
                }
#endif
            }
            finally { mPort.ReleaseMutex(); }
            mBuffer.WaitOne();
            for (int i = 0; i < buffer.Length && i < bytesRead; i++)
                unprocessed.Add(buffer[i]);
            mBuffer.ReleaseMutex();
            return true;
        }

        protected override bool StartWorking()
        {
            if (sp == null)
                return false;
            try
            {
#if DEBUG
                try
                {
#endif
                    sp.Open();
#if DEBUG
                }
                catch { }
#endif
                working = true;
#if WRITE_DIRECTLY_TO_BUFFER
                thr = new Thread(listen);
                thr.Start();
#endif
                ShowMessage("Порт открыт", true);
#if ALLOW_RANDOM_DATA
                wrThr = new Thread(WriteToPort);
                wrThr.Start();
#endif
#if BOULDER_TEST
                minRem = TIME;
                secRem = 0;
                t.Start();
                t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
#endif
            }
            catch (Exception ex)
            {
                ShowMessage("Ошибка открытия порта\r\n" + ex.Message, true);
                working = false;
            }
            return working;
        }

        protected override bool StopWorking()
        {
            working = false;
            Thread.Sleep(50);
#if WRITE_DIRECTLY_TO_BUFFER
            if (thr != null)
                try { thr.Abort(); }
                catch { }
#endif
#if ALLOW_RANDOM_DATA
            if (wrThr != null)
                try { wrThr.Abort(); }
                catch { }
#endif
#if BOULDER_TEST
            try
            {
                t.Elapsed -= this.t_Elapsed;
                t.Stop();
            }
            catch { }
#endif
            try
            {
                sp.Close();
                return true;
            }
            catch (Exception ex)
            {
                ShowMessage("Ошибка закрытия порта\r\n" + ex.Message, true);
                return false;
            }
        }
        protected List<byte> unprocessed = new List<byte>();

        protected Mutex mBuffer = new Mutex();

        protected virtual void listen()
        {
            try
            {
                while (working)
                {
                    listenInner();
                    Thread.Sleep(50);
                }
            }
            catch (ThreadAbortException) { return; }
        }

        protected void listenInner()
        {
            GetDataFromPort();
            bool needToPrc = (unprocessed.Count >= (SERV_LENGTH + TAIL_LENGTH));

            if (needToPrc)
            {
                Thread thr = new Thread(processData);
                thr.Start();
            }
        }

        private const int SERV_LENGTH = 4;
        private const int TAIL_LENGTH = 2;
        protected void processDataOld()
        {
            if (unprocessed.Count < 8)
            {
                ShowMessage("Неправильный/неполный пакет", true);
                unprocessed.Clear();
                return;
            }
            byte[] data = new byte[6];
            for (int i = 0; i < data.Length; i++)
                data[i] = unprocessed[i + 1];
            int[] res = getFromByteArray(data);
            string s1 = res[0].ToString("0") + ":" + res[1].ToString("00") + "." + res[2].ToString("00");
            string s2 = res[3].ToString("0") + ":" + res[4].ToString("00") + "." + res[5].ToString("00");
            unprocessed.RemoveRange(0, 8);
            SystemListenerEventArgs sa1 = new SystemListenerEventArgs(
                 SystemListenerEventArgs.EventType.RESULT, 1, s1);
            SystemListenerEventArgs sa2 = new SystemListenerEventArgs(
                 SystemListenerEventArgs.EventType.RESULT, 2, s2);
            CommitEvent(this, sa1);
            CommitEvent(this, sa2);
        }

        protected void processData()
        {
            mBuffer.WaitOne();
            try
            {
                if (useOld)
                {
                    processDataOld();
                    return;
                }
                byte length = 0, type = 0xff;
                byte[] crc = new byte[TAIL_LENGTH], data = null;
                List<byte> d = new List<byte>();


                for (int i = 0; i < crc.Length; i++)
                    crc[i] = 0x00;


                ReadToStart(unprocessed);
                if (unprocessed.Count < SERV_LENGTH)
                    return;
                for (int i = 0; i < SERV_LENGTH; i++)
                    d.Add(unprocessed[i]);
                length = d[d.Count - 1];
                if (unprocessed.Count < (length + SERV_LENGTH + TAIL_LENGTH))
                    return;
                type = d[d.Count - 2];
                if (length > 0)
                {
                    data = new byte[length];
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = unprocessed[i + SERV_LENGTH];
                        d.Add(unprocessed[i + SERV_LENGTH]);
                    }
                }
                for (int i = 0; i < crc.Length; i++)
                {
                    crc[i] = unprocessed[i + SERV_LENGTH + data.Length];
                }

                for (int i = 0; i < 2; i++)
                    unprocessed.RemoveAt(0);

                int nCrc = 0x00;
                for (int i = 0; i < crc.Length; i++)
                    nCrc += crc[i] << (8 * (crc.Length - 1 - i));
                //int nCrct = (crc[0] << 8) + crc[1];
                int rC = 0;
                for (int i = 0; i < d.Count; i++)
                    rC += d[i];
                //if (nCrc != rC) //проверка на контрольную сумму
                //{
                //    ShowMessage("Неверная контрольная сумма");
                //    return;
                //}
                int[] res;
                if (data != null && data.Length > 0)
                    res = getFromByteArray(data);
                else
                    res = null;
                if (res != null && (type == 0x00 || type == 0x01) && res.Length > 2)
                {
                    string s = res[0].ToString("0") + ":" + res[1].ToString("00") + "." + res[2].ToString("00");
                    SystemListenerEventArgs earg = new SystemListenerEventArgs(SystemListenerEventArgs.EventType.RESULT, 1, s);
                    CommitEvent(this, earg);
                    if (res.Length > 5)
                    {
                        s = res[3].ToString("0") + ":" + res[4].ToString("00") + "." + res[5].ToString("00");
                        SystemListenerEventArgs earg2 = new SystemListenerEventArgs(SystemListenerEventArgs.EventType.RESULT, 2, s);
                        CommitEvent(this, earg2);
                    }
                }
                else if (res != null && res.Length == 2)
                    CommitEvent(this, new SystemListenerEventArgs(res[0], res[1]));
                else
                    ShowMessage("Неверный пакет");
            }
            catch (Exception ex)
            {
                Thread thrLM = new Thread(new ParameterizedThreadStart(delegate(object arg)
                {
                    string toShow;
                    if (arg == null || !(arg is string))
                        toShow = "";
                    else
                        toShow = (string)arg;
                    ShowMessage("Ошибка обработки данных с системы:\r\n" +
                        toShow, true);
                }));
                thrLM.Start(ex.Message);
            }
            finally { mBuffer.ReleaseMutex(); }
        }

        private static int[] getFromByteArray(byte[] data)//переведём из BCD в десятичные (массив)
        {
            int[] res = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                int[] bb = getFromByte(data[i]);
                res[i] = bb[0] * 10 + bb[1];
            }
            return res;
        }

        private static int[] getFromByte(byte b)//перевод из BCD байта в 2 десятичных числа
        {
            int[] r = new int[2];
            r[0] = b >> 4;
            r[1] = b - (r[0] << 4);
            return r;
        }
        
        private void ReadToStart(List<byte> unprocessed)//поиск начала пакета
        {
#if DEBUG
            string str = "";
            foreach (byte b in unprocessed)
                foreach (int i in getFromByte(b))
                    str += i.ToString() + ", ";
            ShowMessage(str);
#endif
            while (unprocessed.Count > 1)
            {
                byte b0 = unprocessed[0];
                byte b1 = unprocessed[1];
                if (b0 == H0 && b1 == H1)
                    return;
                else
                    unprocessed.RemoveAt(0);
            }
        }

#if ALLOW_RANDOM_DATA
        private void WriteToPort()
        {
            while (working)
            {
                Thread.Sleep(
#if RANDOM_DELAY
                    rnd.Next(6000, 10000)
#else
20000
#endif
);
                byte[] b = GeneratePackage();
                for (int i = 0; i < 2; i++)
                {
#if WRITE_DIRECTLY_TO_BUFFER
                    mBuffer.WaitOne();
                    try
                    {
                        foreach (byte bb in b)
                            unprocessed.Add(bb);
                    }
                    finally { mBuffer.ReleaseMutex(); }
#else
                    mPort.WaitOne();
                    try { sp.Write(b, 0, b.Length); }
                    finally { mPort.ReleaseMutex(); }
#endif
                }
            }
        }

        private Random rnd = new Random();
        private int[] GenerateTime()
        {
            int mSec = rnd.Next(580, 1431);
            int[] res = new int[3];
            res[0] = 0;
            res[2] = mSec % 100;
            res[1] = (mSec - res[2]) / 100;
            return res;
        }
        private byte[] GeneratePackage()
        {
            //bool errorPckg = ((rnd.Next() % 10) == 0);
            //errorPckg = false;
            //if (errorPckg)
            //{
            //    byte[] b = new byte[rnd.Next(25) + 1];
            //    rnd.NextBytes(b);
            //    ShowMessage("Мусор.\r\nПродолжить показывать инфосообщения?");
            //    return b;
            //}
            List<byte> l = new List<byte>();
            int noiseLength = rnd.Next(25);
            for (int i = 0; i <= noiseLength; i++)
                l.Add((byte)rnd.Next(120));
            l.Add(H0);
            l.Add(H1);
            l.Add((byte)(rnd.Next() % 2));
            l.Add((byte)0x06);
            var leftTime = GenerateTime();
            var rightTime = GenerateTime();
            //int lm = rnd.Next(3), ls = rnd.Next(60), lh = rnd.Next(100);
            //int rm = rnd.Next(3), rs = rnd.Next(60), rh = rnd.Next(100);
            l.Add(GetBCD(leftTime[0]));
            l.Add(GetBCD(leftTime[1]));
            l.Add(GetBCD(leftTime[2]));
            bool err = false;
           
            l.Add(GetBCD(rightTime[0]));
            l.Add(GetBCD(rightTime[1]));
            l.Add(GetBCD(rightTime[2]));

            int crc = 0;
            for (int i = l.Count - 1; i >= l.Count - 10; i--)
                crc += l[i];
            byte cH = (byte)(crc >> 8);
            byte cL = (byte)(crc - ((crc >> 8) << 8));

            if ((rnd.Next() % 7) == 0)
            {
                err = true;
                cH++;
            }

            l.Add(cH);
            l.Add(cL);
            string strErr;
            if (err)
                strErr = "Пакет содержит ошибку\r\n";
            else
                strErr = "";

            ShowMessage(strErr +
                    "Left: " + leftTime[0].ToString("0") + ":" + leftTime[1].ToString("00") + "." + leftTime[2].ToString("00") +
                    "\r\nRight:" + rightTime[0].ToString("0") + ":" + rightTime[1].ToString("00") + "." + rightTime[2].ToString("00") + "\r\n" +
                    "Продолжить показывать инфосообщения?");
            return l.ToArray();
        }
#endif
#if BOULDER_TEST
        private byte[] GeneratePackageBoulder()
        {
            List<byte> l = new List<byte>();
            l.Add(H0);
            l.Add(H1);
            l.Add((byte)(0x02));
            l.Add((byte)0x02);
            int mr = minRem, sr = secRem;
            l.Add(GetBCD(minRem));
            l.Add(GetBCD(secRem));
            /*if ((rnd.Next() % 7) == 0)
            {
                err = true;
                l.Add((byte)rnd.Next(15));
            }
            else
                err = false;*/

            int crc = 0;
            for (int i = l.Count - 1; i >= l.Count - 6; i--)
                crc += l[i];
            byte cH = (byte)(crc >> 8);
            byte cL = (byte)(crc - ((crc >> 8) << 8));


            l.Add(cH);
            l.Add(cL);


            ShowMessage("Time: " + mr.ToString("00") + ":" + sr.ToString("00") + "\r\n" +
                    "Продолжить показывать инфосообщения?");
            return l.ToArray();
        }
#endif
#if BOULDER_TEST || ALLOW_RANDOM_DATA
        private static byte GetBCD(int n)
        {
            if (n < 0 || n > 99)
                throw new ArgumentOutOfRangeException();
            int nL = (n % 10);
            int nH = (n - nL) / 10;
            byte res = (byte)(nH);
            res = (byte)(res << 4);
            res += (byte)nL;
            return res;
        }
#endif
    }
}
