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

namespace WindowsFormsApp4
{
   public class MODBUS_srv
    {

        public byte btDevAdr = 0;
        public bool blDevCnctd = false;
        public bool blReadIRreq = false;
        public bool blReadHRreq = false;
        public bool blUpdGridHR = false;
        public bool blnWriteAllHR = false;
        public bool blnReadAllHR = false;

        public bool blnScpEnbl = false; // разрешение опроса осцила по кнопке из главной формы
        public bool blnScpGetPreChRequest = false;
        public bool blnScpSetChRequest = false;
        public bool blnScpGetChRequest = false;

        public int scope_ADC_div=1;



        private bool blnSuspended = false;
        public bool suspend
        {
            get { return blnSuspended; }
            set {

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

        public SerialPort spPort = new SerialPort("COM1", 9600, Parity.None, 8);

        public List<string> logger = new List<string>();
        public UInt16[] uiInputReg = new UInt16[256];
        public UInt16[] uiHoldingReg = new UInt16[256];

        public enum comand
        {
            RD_INPUT = 0x4,
            RD_HOLDING = 0x3,
            WR_SINGLE = 0x6,
            WR_MULTI = 0x10,

        }

        static byte[] aucCRCLo = {
        0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7,
        0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E,
        0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9,
        0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,
        0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
        0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,
        0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D,
        0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,
        0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF,
        0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
        0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1,
        0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,
        0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB,
        0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA,
        0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
        0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,
        0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97,
        0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E,
        0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89,
        0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
        0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83,
        0x41, 0x81, 0x80, 0x40};
        static byte[] aucCRCHi = {
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40};


        public IModbusSerialMaster master;


        public MODBUS_srv()
        {
            btDevAdr = 0;
            blDevCnctd = false;
            //spPort.ReadTimeout = 5;
            master = ModbusSerialMaster.CreateRtu(spPort);
            //master.Transport.WriteTimeout = 5;
            //master.Transport.ReadTimeout = 5;
        }



        public void vMBCRC16(List<byte> pucFrame)
        {
            byte ucCRCHi = 0xFF;
            byte ucCRCLo = 0xFF;
            int iIndex;

            foreach (byte element in pucFrame)
            {
                iIndex = ucCRCLo ^ element;
                ucCRCLo = (byte)(ucCRCHi ^ aucCRCHi[iIndex]);
                ucCRCHi = aucCRCLo[iIndex];
            }
            pucFrame.Add(ucCRCLo);
            pucFrame.Add(ucCRCHi);
        }

        UInt16 chMBCRC16(byte[] pucFrame, UInt16 len)
        {
            byte ucCRCHi = 0xFF;
            byte ucCRCLo = 0xFF;
            int iIndex;

            for (int i = 0; i < len; i++)
            {
                try
                {
                    iIndex = ucCRCLo ^ pucFrame[i];
                    ucCRCLo = (byte)(ucCRCHi ^ aucCRCHi[iIndex]);
                    ucCRCHi = aucCRCLo[iIndex];
                }
                catch (Exception er) { return 0; };
            }

            return (UInt16)(ucCRCLo + (ucCRCHi << 8));
        }

        public async void vConnectToDevAsync()
        {
            string mes = null;
            if (!blDevCnctd)
            {
                Debug.WriteLine("Device is not connected");

                logger.Add("Поиск: " + btDevAdr.ToString()+"-"+ spPort.PortName+"-"+ spPort.BaudRate);

                List<byte> cmdGetID = new List<byte>() { btDevAdr, 0x2B, 0xE, 0x1, 0x1 };
                vMBCRC16(cmdGetID);
                try
                {
                    if(spPort.IsOpen)spPort.Write(cmdGetID.ToArray(), 0, cmdGetID.ToArray().Length);
                    Thread.Sleep(100);
                    mes = spPort.ReadExisting();
                }
                catch (Exception e) { Debug.WriteLine("сбой" + e.ToString()); };

                if (String.IsNullOrEmpty(mes))
                {
                    Debug.WriteLine("Нет ответа 1 " + mes);
                    return;
                }

                if (mes.Length<2)
                {
                    Debug.WriteLine("Нет ответа 2 " + mes);
                    return;
                }

                if ((byte)mes.ToCharArray()[1] != 0x2b)
                {
                    Debug.WriteLine("Нет ответа 3");
                    return;
                }
                if(mes.Length> 43)
                strDevID = mes.Substring(10, 8) + " " + mes.Substring(20, 10) + " " + mes.Substring(32, 10) + " " + mes.Substring(44, 9);
                logger.Add(strDevID);
 
                Thread.Sleep(100);
                if ( intReadData(0, 3, comand.RD_INPUT) < 0) iFail++;
                Thread.Sleep(100);
                if (this.uiInputReg[0] == 0 || this.uiInputReg[0] > 255 || this.uiInputReg[0] == 0 || this.uiInputReg[1] > 255)
                {
                    logger.Add("Неверное количество регистров устройства");
                    iFail++;
                    return; };

                blDevCnctd = true;
                uilHRadrForRead.Add(0);
                uilHRadrForRead.Add(256);
                return;
            }

        }

        public async void vPoll()
        {
            int delay = 10;
            while (true) {
                if (spPort.IsOpen && blDevCnctd)
                {
                    if (uiInputReg[0] > 0)
                    {
                        //читаем все РВ
                        // blReadIRreq = false;


                        await Task.Run(() =>
                        {
                            if (blReadIRreq)
                                if (intReadData(0, uiInputReg[0], comand.RD_INPUT) < 0)
                                {
                                    for (int i = 2; i < uiInputReg[0]; i++) uiInputReg[i] = 0;
                                    iFail++;

                                }
                        });
                        blReadIRreq = false;



                        //читаем несколько РХ
                        if (uilHRadrForRead.Count > 1)
                        {
                            int max = uilHRadrForRead.Max();
                            int min = uilHRadrForRead.Min();
                            if (max >= uiInputReg[1]) max = uiInputReg[1];
                            int len = max - min;
                            if ( intReadData((ushort)min, (ushort)max, comand.RD_HOLDING) < 0) iFail++;
                            blUpdGridHR = true;
                            uilHRadrForRead.Clear();
                        }

                        //читаем один РХ
                        if (uilHRadrForRead.Count > 0 & !blnWriteAllHR)
                        {
                            if (uilHRadrForRead[0] > uiInputReg[1] - 1) uilHRadrForRead[0] = (ushort)(uiInputReg[1] - 1);

                            if ( intReadData(uilHRadrForRead[0], 1, comand.RD_HOLDING) < 0) iFail++;
                            uilHRadrForRead.RemoveAt(0);
                            if (uilHRadrForRead.Count > 5) uilHRadrForRead.Clear();
                            blUpdGridHR = true;
                        }


                        //Пишем один РХ

                        await Task.Run(() =>
                        {
                             if (uialHRForWrite.Count != 0) if (uialHRForWrite[0].Length == 2)
                                {
                                    iWriteData(uialHRForWrite[0][0], uialHRForWrite[0][1]);
                                    uialHRForWrite.RemoveAt(0);
                                    if (uialHRForWrite.Count > 25) uialHRForWrite.Clear();
                                }
                        });

                      


                        //Пишем все РХ
                        await Task.Run(() =>
                        {
                            if (blnWriteAllHR)
                            {
                                ushort tempAdrForWrite = 0;
                                while (uialHRForWrite.Count > 0 && uialHRForWrite[0].Length == 1)
                                {
                                    iWriteData(tempAdrForWrite, uialHRForWrite[0][0]);
                                    tempAdrForWrite++;
                                    uialHRForWrite.RemoveAt(0);

                                }

                                //List<ushort> temp = new List<ushort>();

                                //foreach (var el in uialHRForWrite)
                                //{
                                //   if(el.Length ==1) temp.Add(el[0]);
                                //}
                                //iWriteData(0,temp);


                                uialHRForWrite.Clear();
                                blnWriteAllHR = false;
                            }
                        });

                        //Осциллограф
                        if (blnScpEnbl)
                        {
                            int temp = 2;
                            int i = 0;
                            while ((temp = await ScopeReadDataAsync(scope_ADC_div)) > 0)
                            {

                                await Task.Delay(2);
                                i++;
                                if (i > 8)
                                {
                                    logger.Add("Осцил - переполнение FIFO");
                                    break;
                                }
                            };
    

                            if (blnScpGetPreChRequest)
                            {
                                await ScopeGetPredefChnListAsync();
                                blnScpGetPreChRequest = false;

                            }

                            if (blnScpGetChRequest)
                            {
                                await ScopeGetChannelsAdrAsync();
                                blnScpGetChRequest = false;

                            }

                            if (blnScpSetChRequest)
                            {
                                //await ScopeSetChannelsAdrAsync(ScopeChnToRead);
                                await ScopeSetChannelsAdrAsync(ScopeChnToRead);
                                byte num = (byte)ScopeChnToRead.GroupBy(_ => _ != 0).ToList()[0].Count();
                                if(num<=4 && num >= 1) await ScopeSetParam(--num, 1);
                                blnScpSetChRequest = false;
                            }
                        };


                        
                        // ScopeSetChannel();
                        // 


                    }
                    else
                    {
                        if (intReadData(0, 3, comand.RD_INPUT) < 0 ) iFail++;
                    };
                }

                await Task.Delay(10);
            }

        }

        int intReadData(UInt16 adr, UInt16 count, comand cmd)
        {

            try
            {
                ushort[] temp;
                if (cmd == comand.RD_HOLDING)
                {
                    temp = master.ReadHoldingRegisters(1, adr, count);
                    //temp =master.ReadHoldingRegistersAsync(1, adr, count);
                    int i = 0;
                    foreach (var el in temp)
                    {
                        uiHoldingReg[adr + i] = el;
                        i++;
                    }


                }
                if (cmd == comand.RD_INPUT)
                {

                    //temp = master.ReadInputRegistersAsync(1, adr, count);
                    temp = master.ReadInputRegisters(1, adr, count); 
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
                return -1;
            }

        return 0;

        }

        int iWriteData  (UInt16 adr, UInt16 data)
        {

            try
            {
                master.WriteSingleRegisterAsync(1, adr, data);
                uilHRadrForRead.Add(adr);
            }
            catch (Exception e){
                logger.Add(e.Message);
            }

            return 0;
                   }


            //write multiply register
        unsafe int iWriteData(UInt16 adrW, List<ushort> data)
        {
            try
            {
                //master.WriteMultipleRegisters(1, adrW, data.ToArray());
                master.WriteMultipleRegistersAsync(1, adrW, data.ToArray());
                uilHRadrForRead.Add(0);
                uilHRadrForRead.Add(256);
            }
            catch (Exception e) {
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

         public ScopeFrameReader circbuf = new ScopeFrameReader(1500) ;


         async Task<int> ScopeReadDataAsync(int div)
        //int intReadScopeAsync(int div)
        {
            List<byte> cmdGetID = new List<byte>() { btDevAdr, 20, 0x1, 0x1, 0x1 };
            vMBCRC16(cmdGetID);
            int size;
            byte[] buff;


            try
            {
               
                if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
                spPort.ReadExisting();
                spPort.Write(cmdGetID.ToArray(), 0, cmdGetID.ToArray().Length);
                int i = 0;


                while (spPort.BytesToRead < 245)
                {
                    i++;

                    if (!spPort.IsOpen || i > 10000)
                    {
                        if (spPort.BytesToRead == 5) break;
                        logger.Add("Осциллограф таймаут xx " + spPort.BytesToRead.ToString());
                        //  Debug.WriteLine(" SCP TO");
                        await Task.Delay(10);
                        return -1;
                    }

                };


                // if (spPort.BytesToRead < 245 ) return -1;

                // Debug.WriteLine(i + " ms left");
                size = spPort.BytesToRead;
                buff = new byte[size];
                try { spPort.Read(buff, 0, size); }
                catch (TimeoutException ex) { logger.Add("Нет Ответа"); return -1; };
            }
            catch (Exception ex) { logger.Add("Ошибка записи в порт"); return -1; };
           

            if (size < 5) {
              //  Debug.WriteLine("no answer");
                logger.Add("Нет ответа"); return -1;

            }
            //Debug.WriteLine("Get data " + size);

            if (size == 5)
            {
                  // Debug.WriteLine("FIFO not ready");
              // Debug.WriteLine(sBtoS(buff, size));
                if (buff[1] == 1)
                {
                    logger.Add("Осциллограф не найден");
                    return -1;
                }

               // await Task.Delay (50);
                return 0;

            }


           // Debug.WriteLine(sBtoS(buff, size));

            ushort crc = chMBCRC16(buff, (ushort)(size - 2));
           if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2]) {
                Debug.WriteLine("CRC_ERROR");
               logger.Add("Осцил - Ошибка CRC ");
                return 0;
            }


            // Debug.WriteLine("Elemnts in FIFO " + buff[size - 3]);
            //  Debug.WriteLine("Ch in SCOPE     " + buff[size - 4]);
            // Debug.WriteLine("SCOPE  delay    " + buff[size - 5]);

            
            circbuf.ReadFrame(buff);

           // blnScpDataRdy = true;
            return buff[size-3]; // возвращаю количество фраймов в фифо МК
        }


        public async Task<int> ScopeGetPredefChnListAsync() {

            List<byte> cmdGetID = new List<byte>() { btDevAdr, 21};
            vMBCRC16(cmdGetID);
            int size;
            byte[] buff;


            try
            {

                if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
                spPort.ReadExisting();
                spPort.Write(cmdGetID.ToArray(), 0, cmdGetID.ToArray().Length);
                int i = 0;
                await Task.Delay(100);
                //Thread.Sleep(50);
                size = spPort.BytesToRead;
                buff = new byte[size];
                try { spPort.Read(buff, 0, size); }
                catch (TimeoutException ex) { logger.Add("Нет Ответа"); return -1; };

            }
            catch (Exception ex) { logger.Add("Ошибка записи в порт"); return -1; };

            ushort crc = chMBCRC16(buff, (ushort)(size - 2));
            if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2])
            {
                Debug.WriteLine("CRC_ERROR");
                logger.Add("Осцил - Ошибка CRC ");
                return 0;
            };


            circbuf.ReadTargetChl(buff);

            Debug.WriteLine(sBtoS(buff, size));

            Debug.WriteLine("Get target scope channels #" + circbuf.target_chnl.Count());

            return 0;
        }

        public async Task<int> ScopeSetChannelsAdrAsync(List <UInt32> chnls) {
       // public int ScopeSetChannelsAdrAsync(List <UInt32> chnls) {

            if (chnls.Count() > 4) return -1;
         //   while (chnls.Count() != 4) chnls.Add(0);
            List<byte> cmd = new List<byte>() { btDevAdr, 22 };
            foreach (var item in chnls) cmd.AddRange(BitConverter.GetBytes(item));
            vMBCRC16(cmd);
            int size;
            byte[] buff;

            try
            {

                if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
                spPort.ReadExisting();
                spPort.Write(cmd.ToArray(), 0, cmd.ToArray().Length);
                int i = 0;
               await Task.Delay(50);
                //Thread.Sleep(50);
                size = spPort.BytesToRead;
                if (size == 0) return -1;
                
                buff = new byte[size];
                try { spPort.Read(buff, 0, size); }
                catch (TimeoutException ex) { logger.Add("Нет Ответа"); return -1; };

            }
            catch (Exception ex) { logger.Add("Ошибка записи в порт"); return -1; };

            ushort crc = chMBCRC16(buff, (ushort)(size - 2));
            if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2])
            {
                Debug.WriteLine("CRC_ERROR");
                logger.Add("Осцил - Ошибка CRC ");
                return -1;
            };

            Debug.WriteLine(sBtoS(buff, size));

            return 0;
        }

        public async Task<int> ScopeSetParam(byte count, byte freq)
        { 
            List<byte> cmd = new List<byte>() { btDevAdr, 25, count, freq };
            vMBCRC16(cmd);
            int size;
            byte[] buff;

            try
            {

                if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
                spPort.ReadExisting();
                spPort.Write(cmd.ToArray(), 0, cmd.ToArray().Length);
                int i = 0;
                await Task.Delay(10);
                //Thread.Sleep(10);
                size = spPort.BytesToRead;
                if (size == 0) return -1;
                buff = new byte[size];
                try { spPort.Read(buff, 0, size); }
                catch (TimeoutException ex) { logger.Add("Нет Ответа"); return -1; };

            }
            catch (Exception ex) { logger.Add("Ошибка записи в порт"); return -1; };

            ushort crc = chMBCRC16(buff, (ushort)(size - 2));
            if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2])
            {
                Debug.WriteLine("CRC_ERROR");
                logger.Add("Осцил - Ошибка CRC ");
                return -1;
            };

            Debug.WriteLine(sBtoS(buff, size));

            return 0;
        }

        public async Task<Int32[]> ScopeGetChannelsAdrAsync()
        {

            List<byte> cmd = new List<byte>() { btDevAdr, 24 };
            vMBCRC16(cmd);
            int size;
            byte[] buff;

            try
            {

                if (!spPort.IsOpen) { this.blDevCnctd = false; return null; }
                spPort.ReadExisting();
                spPort.Write(cmd.ToArray(), 0, cmd.ToArray().Length);
                int i = 0;
                await Task.Delay(10);
                //Thread.Sleep(10);
                size = spPort.BytesToRead;
                buff = new byte[size];
                try { spPort.Read(buff, 0, size); }
                catch (TimeoutException ex) { logger.Add("Нет Ответа"); return null; };

            }
            catch (Exception ex) { logger.Add("Ошибка записи в порт"); return null; };

            ushort crc = chMBCRC16(buff, (ushort)(size - 2));
            if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2])
            {
                Debug.WriteLine("CRC_ERROR");
                logger.Add("Осцил - Ошибка CRC ");
                return null;
            };

            Debug.WriteLine(sBtoS(buff, size));

            int counter = 0;
            Int32[] temp = new Int32[4];
            for (int i = 0; i < 4; i++) temp[i] = BitConverter.ToInt32(buff, 2 + i * 4);

            return temp;
        }




        public string sBtoS(byte[] mas, int size)
        {
            string str="";
            int i=0;
            while (i <size)
            {
                str += Convert.ToString(mas[i], 16)+".";
                i++;
            }

            return str;
        }

        public void vReset()
        {

            blDevCnctd = false;
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
            

            this.spPort.Close();
            this.uilHRadrForRead.Clear();
            this.uialHRForWrite.Clear();
            //this.logger.Clear();
            this.suspend = false;


        }

        private void vLogByteArr(byte[] buff, int len)
        {

            string str = "";
            for (int i = 0; i < (len - 1); i++)
            {
                str += Convert.ToString(buff[i], 16);
            }
            logger.Add(str);
        }



    }
}
