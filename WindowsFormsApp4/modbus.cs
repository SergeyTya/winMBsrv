#define my_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Modbus.Data;
using Modbus.Device;
using Modbus.Utility;
using System.Net;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;
using AsyncSocketTest;
using System.Collections;
using static AsyncSocketTest.ServerModbusTCP;
using ctsServerAdapter;
using System.Dynamic;

namespace WindowsFormsApp4
{
    public class MODBUS_srv
    {

        public bool isDeviceConnected = false;

        public byte btDevAdr = 0;
  
        public bool blReadIRreq = false;
        public bool blReadHRreq = false;
        public bool blUpdGridHR = false;
        public bool blnWriteAllHR = false;
        public bool blnReadAllHR = false;

        public bool blnScpEnbl = false; // разрешение опроса осцила по кнопке из главной формы
        public bool blnScpGetPreChRequest = false;
        public bool blnScpSetChRequest = false;
        public bool blnScpGetChRequest = false;

        public int scope_ADC_div = 1;

        private bool blnSuspended = false;
        public bool suspend
        {
            get { return blnSuspended; }
            set
            {

                if (blnSuspended)
                {
                    uilHRadrForRead.Clear();
                    uialHRForWrite.Clear();
                    blnReadAllHR = true;
                }

                blnSuspended = value;
            }

        }


        public List<UInt16> uilHRadrForRead = new List<UInt16>();
        public List<UInt16[]> uialHRForWrite = new List<UInt16[]>();
        public List<UInt32> ScopeChnToRead = new List<UInt32>();

        public double iFail = 0;

        public String strDevID = null;

        public List<string> logger = new List<string>();
        public UInt16[] uiInputReg = new UInt16[256];
        public UInt16[] uiHoldingReg = new UInt16[256];

        public string info;

        public enum comand
        {
            RD_INPUT = 0x4,
            RD_HOLDING = 0x3,
            WR_SINGLE = 0x6,
            WR_MULTI = 0x10,

        }

        public bool OpenConsole { set; get; } = false;

        // public IModbusSerialMaster master;

        public AsyncSocketTest.ServerModbusTCP tcp_master;

        public MODBUS_srv()
        {
            btDevAdr = 1;
            isDeviceConnected = false;

        }

        public async void ConnectToDevAsync(string host= "localhost", int port= 8888)
        {
            if (!isDeviceConnected)
            {
                try
                {
                    tcp_master = new ServerModbusTCP(host, port);
                }
                catch (ServerModbusTCPException ex)
                {
                    logger.Add(ex.Message);
                    return;
                }
                info = await tcp_master.GetInfoAsync();
                logger.Add(info);

               byte[] cmdGetID = new byte[]  { 0, 0, 0, 0, 0 , 5, btDevAdr, 0x2B, 0xE, 0x1, 0x1 };

                var resp = await tcp_master.SendRawDataAsync(cmdGetID);

                if (resp.Length > 43)
                {
                    string result = System.Text.Encoding.UTF8.GetString(resp);
                    strDevID = Regex.Replace(result.Substring(10), @"[^0-9a-zA-Z-_. ]+", " ");
                    Debug.WriteLine("---------->" + strDevID);
                }
                logger.Add(strDevID);

                if (await intReadDataAsync(0, 3, comand.RD_INPUT) < 0) iFail++;

                if (this.uiInputReg[0] == 0 || this.uiInputReg[0] > 255 || this.uiInputReg[0] == 0 || this.uiInputReg[1] > 255)
                {
                    logger.Add("Неверное количество регистров устройства");
                    iFail++;
                    return;
                };
                Debug.WriteLine("Connected");
                isDeviceConnected = true;
                uilHRadrForRead.Add(0);
                uilHRadrForRead.Add(256);
                return;
            }

        }

        public async void SlavePollAsync(int delay)
        {
            while (true)
            {

                while (suspend) await Task.Delay(1000);

                if (isDeviceConnected)
                {
                    if (uiInputReg[0] > 0)
                    {
                        //читаем несколько РХ
                        if (uilHRadrForRead.Count > 1)
                        {
                            int max = uilHRadrForRead.Max();
                            int min = uilHRadrForRead.Min();
                            if (max >= uiInputReg[1]) max = uiInputReg[1];
                            int len = max - min;
                            if (await intReadDataAsync((ushort)min, (ushort)max, comand.RD_HOLDING) < 0) iFail++;
                            blUpdGridHR = true;
                            uilHRadrForRead.Clear();
                            await Task.Delay(delay);
                            continue;
                        }

                        //читаем один РХ
                        if (uilHRadrForRead.Count > 0 & !blnWriteAllHR)
                        {
                            if (uilHRadrForRead[0] > uiInputReg[1] - 1) uilHRadrForRead[0] = (ushort)(uiInputReg[1] - 1);

                            if (await intReadDataAsync(uilHRadrForRead[0], 1, comand.RD_HOLDING) < 0) iFail++;
                            uilHRadrForRead.RemoveAt(0);
                            if (uilHRadrForRead.Count > 5) uilHRadrForRead.Clear();
                            blUpdGridHR = true;
                            await Task.Delay(delay);
                            continue;
                        }


                        //Пишем один РХ

                        if (uialHRForWrite.Count != 0) if (uialHRForWrite[0].Length == 2)
                            {
                                await iWriteDataAsync(uialHRForWrite[0][0], uialHRForWrite[0][1]);
                                uialHRForWrite.RemoveAt(0);
                                if (uialHRForWrite.Count > 25) uialHRForWrite.Clear();
                                await Task.Delay(delay);
                                continue;
                            }


                        if (blnWriteAllHR)
                        {
                            ushort tempAdrForWrite = 0;
                            while (uialHRForWrite.Count > 0 && uialHRForWrite[0].Length == 1)
                            {
                                await iWriteDataAsync(tempAdrForWrite, uialHRForWrite[0][0]);
                                tempAdrForWrite++;
                                uialHRForWrite.RemoveAt(0);
                            }
                            uialHRForWrite.Clear();
                            blnWriteAllHR = false;
                            await Task.Delay(delay);
                            continue;
                        }
                    }
                    else
                    {
                        if (await intReadDataAsync(0, 3, comand.RD_INPUT) < 0) iFail++;
                    }
                }

                if (isDeviceConnected & blReadIRreq)
                    if (await intReadDataAsync(0, uiInputReg[0], comand.RD_INPUT) < 0)
                    {
                        for (int i = 2; i < uiInputReg[0]; i++) uiInputReg[i] = 0;
                        iFail++;

                    }
                blReadIRreq = false;

                await Task.Delay(delay);
            }

        }

        async Task<int> intReadDataAsync(UInt16 adr, UInt16 count, comand cmd)
        {

            try
            {
                ushort[] temp;
                if (cmd == comand.RD_HOLDING)
                {
                    temp = await tcp_master.ReadHoldingsAsync(1, adr, count);
                    int i = 0;
                    foreach (var el in temp)
                    {
                        uiHoldingReg[adr + i] = el;
                        i++;
                    }


                }
                if (cmd == comand.RD_INPUT)
                {
                    temp = await tcp_master.ReadInputsAsync(1, adr, count);
                    int i = 0;
                    foreach (var el in temp)
                    {
                        uiInputReg[adr + i] = el;
                        i++;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Add(e.Message);
                Debug.WriteLine(e);
                if (e is System.IO.IOException)
                {
                    Close();
                }
                return -1;
            }
            return 0;
        }

        async Task<int> iWriteDataAsync(UInt16 adr, UInt16 data)
        {

            try
            {
                await tcp_master.WriteHoldingsAsync(1, adr, new ushort[] { data });
                uilHRadrForRead.Add(adr);
            }
            catch (Exception e)
            {
                logger.Add(e.Message);
            }

            return 0;
        }

        //write multiply register
        async Task<int> iWriteData(UInt16 adrW, List<ushort> data)
        {
            try
            {
                await tcp_master.WriteHoldingsAsync(1, adrW, data.ToArray());
                uilHRadrForRead.Add(0);
                uilHRadrForRead.Add(256);
            }
            catch (Exception e)
            {
                logger.Add(e.Message);
            }

            return 0;
        }

        /* _______________________________MODBUS SCOPE FRAME________________________________
         *
         *       +-----------+---------------+----------------------------+-------------+
         * index | 0   | 1  | 2 ... 249 |  250  |     251   | 252 253 | 
         *       +-----------+---------------+----------------------------+-------------+
         * FRAME | ADR |CMD |    DATA   | chnum | FIFO DATA |   CRC   |     
         * 
         */

        public ScopeFrameReader circbuf = new ScopeFrameReader(1500);


        async Task<int> ScopeReadDataAsync(int div)
        //int intReadScopeAsync(int div)
        {
            //List<byte> cmdGetID = new List<byte>() { btDevAdr, 20, 0x1, 0x1, 0x1 };
            //vMBCRC16(cmdGetID);
            //int size;
            //byte[] buff;


            //try
            //{

            //    if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
            //    spPort.ReadExisting();
            //    spPort.Write(cmdGetID.ToArray(), 0, cmdGetID.ToArray().Length);
            //    int i = 0;


            //    while (spPort.BytesToRead < 245)
            //    {
            //        i++;

            //        if (!spPort.IsOpen || i > 10000)
            //        {
            //            if (spPort.BytesToRead == 5) break;
            //            logger.Add("Осциллограф таймаут xx " + spPort.BytesToRead.ToString());
            //            //  Debug.WriteLine(" SCP TO");
            //            await Task.Delay(10);
            //            return -1;
            //        }

            //    };


            //    // if (spPort.BytesToRead < 245 ) return -1;

            //    // Debug.WriteLine(i + " ms left");
            //    size = spPort.BytesToRead;
            //    buff = new byte[size];
            //    try { spPort.Read(buff, 0, size); }
            //    catch (TimeoutException ex) { logger.Add("Нет Ответа"); return -1; };
            //}
            //catch (Exception ex) { logger.Add("Ошибка записи в порт"); return -1; };


            //if (size < 5)
            //{
            //    //  Debug.WriteLine("no answer");
            //    logger.Add("Нет ответа"); return -1;

            //}
            ////Debug.WriteLine("Get data " + size);

            //if (size == 5)
            //{
            //    // Debug.WriteLine("FIFO not ready");
            //    // Debug.WriteLine(sBtoS(buff, size));
            //    if (buff[1] == 1)
            //    {
            //        logger.Add("Осциллограф не найден");
            //        return -1;
            //    }

            //    // await Task.Delay (50);
            //    return 0;

            //}


            //// Debug.WriteLine(sBtoS(buff, size));

            //ushort crc = chMBCRC16(buff, (ushort)(size - 2));
            //if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2])
            //{
            //    Debug.WriteLine("CRC_ERROR");
            //    logger.Add("Осцил - Ошибка CRC ");
            //    return 0;
            //}


            //// Debug.WriteLine("Elemnts in FIFO " + buff[size - 3]);
            ////  Debug.WriteLine("Ch in SCOPE     " + buff[size - 4]);
            //// Debug.WriteLine("SCOPE  delay    " + buff[size - 5]);


            //circbuf.ReadFrame(buff);

            //// blnScpDataRdy = true;
            //return buff[size - 3]; // возвращаю количество фраймов в фифо МК
            return 0;
        }


        public async Task<int> ScopeGetPredefChnListAsync()
        {

            //List<byte> cmdGetID = new List<byte>() { btDevAdr, 21 };
            //vMBCRC16(cmdGetID);
            //int size;
            //byte[] buff;


            //try
            //{

            //    if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
            //    spPort.ReadExisting();
            //    spPort.Write(cmdGetID.ToArray(), 0, cmdGetID.ToArray().Length);
            //    int i = 0;
            //    await Task.Delay(100);
            //    //Thread.Sleep(50);
            //    size = spPort.BytesToRead;
            //    buff = new byte[size];
            //    try { spPort.Read(buff, 0, size); }
            //    catch (TimeoutException ex) { logger.Add("Нет Ответа"); return -1; };

            //}
            //catch (Exception ex) { logger.Add("Ошибка записи в порт"); return -1; };

            //ushort crc = chMBCRC16(buff, (ushort)(size - 2));
            //if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2])
            //{
            //    Debug.WriteLine("CRC_ERROR");
            //    logger.Add("Осцил - Ошибка CRC ");
            //    return 0;
            //};


            //circbuf.ReadTargetChl(buff);

            //Debug.WriteLine(sBtoS(buff, size));

            //Debug.WriteLine("Get target scope channels #" + circbuf.target_chnl.Count());

            return 0;
        }

        public async Task<int> ScopeSetChannelsAdrAsync(List<UInt32> chnls)
        {
            //// public int ScopeSetChannelsAdrAsync(List <UInt32> chnls) {

            //if (chnls.Count() > 4) return -1;
            ////   while (chnls.Count() != 4) chnls.Add(0);
            //List<byte> cmd = new List<byte>() { btDevAdr, 22 };
            //foreach (var item in chnls) cmd.AddRange(BitConverter.GetBytes(item));
            //vMBCRC16(cmd);
            //int size;
            //byte[] buff;

            //try
            //{

            //    if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
            //    spPort.ReadExisting();
            //    spPort.Write(cmd.ToArray(), 0, cmd.ToArray().Length);
            //    int i = 0;
            //    await Task.Delay(50);
            //    //Thread.Sleep(50);
            //    size = spPort.BytesToRead;
            //    if (size == 0) return -1;

            //    buff = new byte[size];
            //    try { spPort.Read(buff, 0, size); }
            //    catch (TimeoutException ex) { logger.Add("Нет Ответа"); return -1; };

            //}
            //catch (Exception ex) { logger.Add("Ошибка записи в порт"); return -1; };

            //ushort crc = chMBCRC16(buff, (ushort)(size - 2));
            //if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2])
            //{
            //    Debug.WriteLine("CRC_ERROR");
            //    logger.Add("Осцил - Ошибка CRC ");
            //    return -1;
            //};

            //Debug.WriteLine(sBtoS(buff, size));

            return 0;
        }

        public async Task<int> ScopeSetParam(byte count, byte freq)
        {
            //List<byte> cmd = new List<byte>() { btDevAdr, 25, count, freq };
            //vMBCRC16(cmd);
            //int size;
            //byte[] buff;

            //try
            //{

            //    if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
            //    spPort.ReadExisting();
            //    spPort.Write(cmd.ToArray(), 0, cmd.ToArray().Length);
            //    int i = 0;
            //    await Task.Delay(10);
            //    //Thread.Sleep(10);
            //    size = spPort.BytesToRead;
            //    if (size == 0) return -1;
            //    buff = new byte[size];
            //    try { spPort.Read(buff, 0, size); }
            //    catch (TimeoutException ex) { logger.Add("Нет Ответа"); return -1; };

            //}
            //catch (Exception ex) { logger.Add("Ошибка записи в порт"); return -1; };

            //ushort crc = chMBCRC16(buff, (ushort)(size - 2));
            //if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2])
            //{
            //    Debug.WriteLine("CRC_ERROR");
            //    logger.Add("Осцил - Ошибка CRC ");
            //    return -1;
            //};

            //Debug.WriteLine(sBtoS(buff, size));

            return 0;
        }

        public async Task<Int32[]> ScopeGetChannelsAdrAsync()
        {

            //List<byte> cmd = new List<byte>() { btDevAdr, 24 };
            //vMBCRC16(cmd);
            //int size;
            //byte[] buff;

            //try
            //{

            //    if (!spPort.IsOpen) { this.blDevCnctd = false; return null; }
            //    spPort.ReadExisting();
            //    spPort.Write(cmd.ToArray(), 0, cmd.ToArray().Length);
            //    int i = 0;
            //    await Task.Delay(10);
            //    //Thread.Sleep(10);
            //    size = spPort.BytesToRead;
            //    buff = new byte[size];
            //    try { spPort.Read(buff, 0, size); }
            //    catch (TimeoutException ex) { logger.Add("Нет Ответа"); return null; };

            //}
            //catch (Exception ex) { logger.Add("Ошибка записи в порт"); return null; };

            //ushort crc = chMBCRC16(buff, (ushort)(size - 2));
            //if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2])
            //{
            //    Debug.WriteLine("CRC_ERROR");
            //    logger.Add("Осцил - Ошибка CRC ");
            //    return null;
            //};

            //Debug.WriteLine(sBtoS(buff, size));

            //int counter = 0;
            //Int32[] temp = new Int32[4];
            //for (int i = 0; i < 4; i++) temp[i] = BitConverter.ToInt32(buff, 2 + i * 4);

            //return temp;
            return new int[] {0};
        }




        public string sBtoS(byte[] mas, int size)
        {
            string str = "";
            int i = 0;
            while (i < size)
            {
                str += Convert.ToString(mas[i], 16) + ".";
                i++;
            }

            return str;
        }

        public void Close()
        {

            if (CtsServerAdapter.isAlaive()) {
                
                Task.Run(() =>
                 {
                     logger.Add("Останавливаю сервер");
                     while (true) {
                         if (
                         CtsServerAdapter.isAlaive() == false
                         ) 
                         { 
                         break; }
                        Task.Delay(100);
                     }
                     logger.Add("Ok");
                });
                CtsServerAdapter.Close();
            }
           isDeviceConnected = false;
            iFail = 0;
            strDevID = "";


            int i = 0;
            foreach (UInt16 el in uiHoldingReg)
            {
                uiHoldingReg[i] = 0;
                i++;
            }

            i = 0;
            foreach (UInt16 el in uiInputReg)
            {
                uiInputReg[i] = 0;
                i++;
            }
            this.uilHRadrForRead.Clear();
            this.uialHRForWrite.Clear();
            this.suspend = false;
            tcp_master.close();
        }

        public async Task<bool> StartPort(string name, int speed, string server = "localhost", int server_port = 8888) {
            SerialPort port = new SerialPort();
            try
            {
                port.Parity = Parity.None;
                port.DataBits = 8;
                port.ReadTimeout = 500;
                port.PortName = name;
                port.BaudRate = speed;
                var dmes = String.Format("Поиск {0} {1}", port.PortName, port.BaudRate);
                Debug.WriteLine(dmes);
                logger.Add(dmes);
                port.Open();
                logger.Add("Открытие порта");
                IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
                logger.Add("Запрос устройства");
                await master.ReadHoldingRegistersAsync(btDevAdr, 0, 1);
                port.Close();
                logger.Add("Запуск сервера");
                CtsServerAdapter.Start(server, server_port, port.PortName, port.BaudRate, OpenConsole);
                ConnectToDevAsync(server, server_port);
                return true;
            }
            catch (Exception ex)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
                logger.Add(ex.Message.ToString());
                return false;
            }
        }


        public void ConncetToRunningServer(string host, int port)
        {
            ConnectToDevAsync(host, port);
        }



        public async void SearchPort(string server = "localhost", int server_port = 8888) {

            if (isDeviceConnected) Close();
            
            List<String> broken_ports = new List<string>();
            int[] speeds_avalible = new int[] { 9600, 38400, 115200, 128000, 230400, 406000 };
            List<String> ports_avalible = SerialPort.GetPortNames().ToList();
            foreach (String name in ports_avalible)
            {
                foreach (int speed in speeds_avalible)
                {
                    if (await StartPort(name, speed, server, server_port) == true) {
                        return;
                    }
                }
            }
        }


        public async Task<Tuple<string, int>> GetComInfoAsync() {
            string tmp =  await tcp_master.GetInfoAsync();

            try
            {
                var data = tmp.Split(' ')[1].Split(':');
                Debug.WriteLine(data[0]);
                Debug.WriteLine(data[1]);
                return new Tuple<string, int>(data[0], Convert.ToInt32(data[1]));
            }
            catch (System.IndexOutOfRangeException ex)
            {
                Debug.WriteLine(ex.Message);
                return new Tuple<string, int>("COM0", 0000);
            }
        }

    }
}
