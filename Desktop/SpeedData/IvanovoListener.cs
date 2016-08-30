// <copyright file="IvanovoListener.cs">
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
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;


namespace ClimbingCompetition.SpeedData
{
    public class IvanovoListener : SystemListener
    {
        private IPAddress ip;
        private int port;
        private Socket socket;
        const int PACKAGE_SIZE = 16;
        byte[] buffer;

        public IvanovoListener(IPAddress ip, int port, Form owner, bool showErrors, Action<bool> persistShowErrors)
            : base(showErrors, persistShowErrors, owner)
        {
            this.ip = ip;
            this.port = port;
        }
        public IPAddress IP
        {
            get { return ip; }
            set { ip = value; }
        }
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        protected override bool StartWorking()
        {
            try
            {
                socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(ip, port);
                socket.Connect(ep);
                List<byte> unprocessed = new List<byte>();
                buffer = new byte[PACKAGE_SIZE];
                working = true;
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, endReciveAsync, unprocessed);
                ShowMessage("Соединение с системой по адресу " + ep.Address.ToString() + " на порту " + ep.Port.ToString() + " успешно установлено", true);

            }
            catch (Exception ex)
            {
                ShowMessage("Ошибка установления соединения:\r\n" + ex.Message, true);
                working = false;
            }
            return working;
        }

        private void endReciveAsync(IAsyncResult res)
        {
            if (!res.IsCompleted)
                return;
            try
            {
                int k = socket.EndReceive(res);
                List<byte> unprocessed = (List<byte>)res.AsyncState;
                for (int i = 0; i < k && i < buffer.Length; i++)
                    unprocessed.Add(buffer[i]);
                ShowBuffer(k);
                ProcessBuffer(unprocessed);
                if (working)
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, endReciveAsync, unprocessed);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Ошибка получения данных:\r\n" + ex.Message + "\r\nПерезапустите систему считывания данных");
                StopListening();
            }
            catch { }
        }
        void ShowBuffer(int len)
        {
            if (len == 0)
                return;
            string str = "";
            for (int i = 0; i < buffer.Length && i < len; i++)
                str += (char)buffer[i] + "; ";
            ShowMessage("Пришли данные:\r\n" + str +
                "\r\nПродолжить показывать инфосообщения?");
        }


        private void ProcessBuffer(List<byte> unprocessed)
        {
            while (unprocessed.Count > 0)
                if (unprocessed[0] != 0xAA)
                    unprocessed.RemoveAt(0);
                else
                    break;
            if (unprocessed.Count < 3)
                return;
            int len = unprocessed[1];
            if (unprocessed.Count < len + 2)
                return;
            byte[] msg = new byte[len];
            for (int i = 0; i < 2; i++)
                unprocessed.RemoveAt(0);
            for (int i = 0; i < msg.Length && unprocessed.Count > 0; i++)
            {
                msg[i] = unprocessed[0];
                if (msg[i] == 0xAA)
                {
                    ShowMessage("Битый пакет.\r\nПродоожить показывать инфосообщения?");
                    return;
                }
                unprocessed.RemoveAt(0);
            }
            if (msg.Length == 1)
            {
                switch (msg[0])
                {
                    case 0x01:
                        CommitEvent(this, new SystemListenerEventArgs(1, 0));
                        break;
                    case 0x02:
                        CommitEvent(this, new SystemListenerEventArgs(0, 10));
                        break;
                    case 0x03:
                        CommitEvent(this, new SystemListenerEventArgs(0, 0));
                        break;
                }
            }
            else if (msg.Length == 0x0E)
            {
                int min = getValue(new byte[] { msg[0], msg[1] });
                int sec, hund, mls;
                if (min == -1)
                    CommitEvent(this, new SystemListenerEventArgs(SystemListenerEventArgs.EventType.FALSTART,
                         1, "Фальстарт на 1 трассе"));
                else if (min >= 0)
                {
                    sec = getValue(new byte[] { msg[2], msg[3] });
                    if (sec < 0 || sec > 59)
                        return;
                    hund = getValue(new byte[] { msg[4], msg[5] });
                    if (hund < 0)
                        return;
                    mls = msg[6] - 0x30;


                    CommitEvent(this, new SystemListenerEventArgs(SystemListenerEventArgs.EventType.RESULT,
                        1, min.ToString("0") + ":" + sec.ToString("00") + "." + hund.ToString("00")));
                }
                else
                    return;
                min = getValue(new byte[] { msg[7], msg[8] });
                if (min == -1)
                    CommitEvent(this, new SystemListenerEventArgs(SystemListenerEventArgs.EventType.FALSTART,
                         2, "Фальстарт на 2 трассе"));
                else if (min >= 0)
                {
                    sec = getValue(new byte[] { msg[9], msg[10] });
                    if (sec < 0 || sec > 59)
                        return;
                    hund = getValue(new byte[] { msg[11], msg[12] });
                    if (hund < 0)
                        return;
                    mls = msg[13] - 0x30;

                    CommitEvent(this, new SystemListenerEventArgs(SystemListenerEventArgs.EventType.RESULT,
                        2, min.ToString("0") + ":" + sec.ToString("00") + "." + hund.ToString("00")));
                }
                else
                    return;
            }
        }

        private static int getValue(byte[] val)
        {


            if (val.Length != 2)
                return -2;
            if (val[0] == 0x46)
                return -1;
            int max, min;
            max = val[0] - 0x30;
            min = val[1] - 0x30;
            if (max < 0 || max > 10 || min < 0 || min > 10)
                return -3;
            return max * 10 + min;
        }

        protected override bool StopWorking()
        {
            working = false;
            try
            {
                if (socket != null)
                    socket.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка закрытия соединения:\r\n" + ex.Message);
                return false;
            }
        }
    }
}
