﻿#define my_DEBUG

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
        public bool blnScpRstreq = false; // запрос на сброс данных осцила
        public bool blnScpDataRdy = false;  // готовность данных

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

        public double iFail = 0;
        public int iScpChNum = 3;
        public int scp_cntmax = 1200;
        public int scp_cnt = 0;

        public String strDevID = null;

        public SerialPort spPort = new SerialPort("COM1", 9600, Parity.None, 8);

        public List<string> logger = new List<string>();
        public UInt16[] uiInputReg = new UInt16[256];
        public UInt16[] uiHoldingReg = new UInt16[256];

        public List<double[]> uialScope = new List<double[]>();
        public List<double[]> uialScopeSHD = new List<double[]>();
        public double[] daGain   = new double[4] { 1, 1, 1, 1 };
        public double[] daOffset = new double[4] { 0, 0, 0, 0 };

        public List<List<double[]>> lldScopeBuf = new List<List<double[]>>();

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
        
        public MODBUS_srv()
        {
            btDevAdr = 0;
            blDevCnctd = false;

            uialScope.Add(new double[scp_cntmax]);
            uialScope.Add(new double[scp_cntmax]);
            uialScope.Add(new double[scp_cntmax]);
            uialScope.Add(new double[scp_cntmax]);

            uialScopeSHD.Add(new double[scp_cntmax]);
            uialScopeSHD.Add(new double[scp_cntmax]);
            uialScopeSHD.Add(new double[scp_cntmax]);
            uialScopeSHD.Add(new double[scp_cntmax]);

            lldScopeBuf.Add(new List<double[]>());
            lldScopeBuf.Add(new List<double[]>());
            lldScopeBuf.Add(new List<double[]>());
            lldScopeBuf.Add(new List<double[]>());
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

        public void vConnectToDev()
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
                if (intReadData(0, 3, comand.RD_INPUT) < 0) iFail++;
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

        public void vPoll()
        {
 
            if (spPort.IsOpen && blDevCnctd)
            {
                if (uiInputReg[0] > 0)
                {
                    //читаем все РВ
                   // blReadIRreq = false;
                    if (blReadIRreq)
                        if (intReadData(0, uiInputReg[0], comand.RD_INPUT) < 0)
                        {
                            for(int i = 2; i < uiInputReg[0]; i++) uiInputReg[i] = 0;
                            iFail++; 
                        }
                    blReadIRreq = false;

                    

                    //читаем несколько РХ
                    if (uilHRadrForRead.Count > 1) 
                    {
                        int max = uilHRadrForRead.Max();
                        int min = uilHRadrForRead.Min();
                        if (max >= uiInputReg[1] ) max = uiInputReg[1] ;
                        int len = max - min;
                        if (intReadData((ushort)min, (ushort)max, comand.RD_HOLDING) < 0) iFail++;
                        blUpdGridHR = true;
                        uilHRadrForRead.Clear();
                    }

                    //читаем один РХ
                    if (uilHRadrForRead.Count > 0 & !blnWriteAllHR)
                    {
                        if (uilHRadrForRead[0] > uiInputReg[1] - 1) uilHRadrForRead[0] = (ushort)(uiInputReg[1] - 1);

                        if (intReadData(uilHRadrForRead[0], 1, comand.RD_HOLDING) < 0) iFail++;
                        uilHRadrForRead.RemoveAt(0);
                        if (uilHRadrForRead.Count > 5) uilHRadrForRead.Clear();
                        blUpdGridHR = true;
                    }
                   

                    //Пишем один РХ
                    if (uialHRForWrite.Count > 0 && uialHRForWrite[0].Length==2)
                    {
                        iWriteData(uialHRForWrite[0][0], uialHRForWrite[0][1]);
                        uialHRForWrite.RemoveAt(0);
                        if (uialHRForWrite.Count > 25) uialHRForWrite.Clear();
                    }
                    

                    //Пишем все РХ
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

                    //Осциллограф
                    if (blnScpEnbl)
                    {
                        int temp = 2;
                        int i = 0;
                        while ((temp = intReadScope(scope_ADC_div)) > 0)
                        {

                            Thread.Sleep(1);
                            i++;
                            if (i > 8)
                            {
                                logger.Add("Осцил - переполнение FIFO");
                                break;
                            }
                        };
                        //если ошибка выключаем окно
                        if (temp < 0)
                        {
                            blnScpEnbl = false;
                        }
                    };


                } else
                {
                    if (intReadData(0, 3, comand.RD_INPUT) < 0) iFail++;
                };
            }

        }

        int intReadData(UInt16 adr, UInt16 count, comand cmd)
        {
            byte[] buff = new byte[] { btDevAdr, (byte)cmd, (byte)(adr >> 8), (byte)adr, (byte)(count >> 8), (byte)count, 0, 0 };
            UInt16 crc = chMBCRC16(buff, 6);
            int size = count * 2 + 6;

            buff[7] = (byte)(crc >> 8);
            buff[6] = (byte)(crc & 0xFF);
#if DEBUG
            if (cmd == comand.RD_HOLDING)
            {
                //Debug.WriteLine(cmd.ToString() + " adr=" + adr.ToString() + " count=" + count.ToString());
                //Debug.WriteLine("<-" + sBtoS(buff, 8));
                logger.Add(cmd.ToString() + " adr=" + adr.ToString() + " count=" + count.ToString());
            }
#endif
            try
            {
                if (!spPort.IsOpen) { this.blDevCnctd = false;  return -1; }
                spPort.ReadExisting();
                spPort.Write(buff, 0, 8);

               // while (spPort.BytesToRead != count * 2 + 5) { if (!spPort.IsOpen) break; };

                int i = 0;
                while (spPort.BytesToRead < count * 2 + 5)
                {
                    i++;
                    if (!spPort.IsOpen) break;
                    if (i > 500) break;
                    Thread.Sleep(1);
                };

                size = spPort.BytesToRead;
                buff = new byte[size];
                try { spPort.Read(buff, 0, size); }
                catch (TimeoutException ex) { logger.Add("Нет Ответа");  return -1; };
            }
            catch (Exception ex) { logger.Add("Ошибка записи в порт");  return -1; };

            if (size < 3) { logger.Add("Нет ответа"); return -1; }
           
         //   Debug.WriteLine(sBtoS(buff, size));
            crc = chMBCRC16(buff, (ushort)(size - 2));
            if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2]) { logger.Add("Ошибка CRC"); return -1; }

            if (buff[1] == (byte)cmd)
            {
                int k = 0;
                for (int i = 3; i < buff[2]+2; i += 2)
                {
                    UInt16 temp = (UInt16)(buff[i + 1] + (buff[i] << 8));

                    if (cmd == comand.RD_HOLDING) {
                        uiHoldingReg[adr+k] = temp;
                    };
                    if (cmd == comand.RD_INPUT) uiInputReg[adr+k] = temp;
                    k++;
                }
#if DEBUG
                if (cmd == comand.RD_HOLDING) {
                    logger.Add("-> " + sBtoS(buff, (UInt16)buff.Length));
                }
                

#endif
            }
            else { logger.Add("ModBus ошибка команды чтения. Код" + buff[1].ToString()); return -1; };

            return 0;
        }

        int iWriteData  (UInt16 adr, UInt16 data)
        {
            
            byte[] buff = new byte[] { btDevAdr, 6, (byte)(adr >> 8), (byte)adr, (byte)(data >> 8), (byte)data, 0, 0 };
            UInt16 crc = chMBCRC16(buff, 6);
            buff[7] = (byte)(crc >> 8);
            buff[6] = (byte)(crc & 0xFF);
#if DEBUG
            //Debug.WriteLine( "WRITE SINGLE HR" + " adr=" + adr.ToString()+ " data=" +data.ToString() );
            //Debug.WriteLine("<-" + sBtoS(buff, 8));
            logger.Add("WRITE SINGLE HR" + " adr=" + adr.ToString() + " data=" + data.ToString());

#endif
            int size = 0;

            try
            {
                
                if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
                spPort.ReadExisting();
                spPort.Write(buff, 0, 8);
                //  System.Threading.Thread.Sleep(100);
                //while (spPort.BytesToRead != 8)  { if (!spPort.IsOpen) break; };
                int i=0;
                while (spPort.BytesToRead < 8)
                {
                    i++;
                    if (!spPort.IsOpen) break;
                    if (i > 500) break;
                    Thread.Sleep(1);
                };
                size = spPort.BytesToRead;
                buff = new byte[size];
                try { spPort.Read(buff, 0, size); }
                catch (TimeoutException ex) { logger.Add("Нет овета"); iFail++; return -1; };
            }
            catch (Exception ex) { Debug.WriteLine("Ошибка записи в порт"); return -1; };

#if DEBUG
            //Debug.WriteLine("->" + sBtoS(buff, (UInt16)buff.Length));
            logger.Add("->" + sBtoS(buff, (UInt16)buff.Length));
#endif       
            if (size < 3) return -1;
            crc = chMBCRC16(buff, (ushort)(size - 2));
            if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2]) { logger.Add("Ошибка CRC"); iFail++; return 0; }

            if (buff[1] == 6)
            {
                uilHRadrForRead.Add(adr);
            }
            else
            {
                logger.Add("ModBus ошибка команды записи. Код " + buff[1].ToString()); return -1;
            }
            if (!blReadIRreq) blReadIRreq = true;

            return 0;
        }


        //write multiply register
        unsafe int iWriteData  (UInt16 adrW, List <ushort> data)
        {
 
            List<byte> buf = new List<byte>();
            int cntW = data.Count;  


            byte dsize = (byte)(cntW * 2);

            buf.AddRange
               (
               new byte[] { btDevAdr, 0x10,
                                            ((byte *) &adrW  )[1], ((byte*)&adrW  )[0],
                                            ((byte *) &cntW  )[1], ((byte*)&cntW  )[0],
                                            dsize
               }
               );

            for (int i = 0; i < cntW; i++)
            {
                ushort temp = data[i];
                buf.AddRange(new byte[] { ((byte*)&temp)[1], ((byte*)&temp)[0]});
            }
            vMBCRC16(buf);

#if DEBUG
            //Debug.WriteLine("WRITE MULTI HR" + " adr=" + adrW.ToString()+ " count=" + cntW.ToString());
            Debug.WriteLine("<-" + sBtoS(buf.ToArray(), (UInt16)buf.Count));
            logger.Add("WRITE MULTI HR" + " adr=" + adrW.ToString() + " count=" + cntW.ToString());
#endif

            byte[] buff = new byte[10];
            int size = 0;
            try
            {
                if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
                spPort.ReadExisting();
                spPort.Write(buf.ToArray(), 0, buf.Count);
                int i = 0;
                while (spPort.BytesToRead < 8)
                {
                    i++;
                    if (!spPort.IsOpen) break;
                    if (i > 1500) break;
                    Thread.Sleep(1);
                };
                size = spPort.BytesToRead;
                buff = new byte[size];
                try { spPort.Read(buff, 0, size); }
                catch (TimeoutException ex) { logger.Add("Нет ответа1"); iFail++; return -1; };
            }
            catch (Exception ex) { Debug.WriteLine("Ошибка записи в порт"); return -1; };
            if (size < 3) { logger.Add("Нет ответа2"); ; return -1; }
#if DEBUG
            //Debug.WriteLine("->" + sBtoS(buff, (UInt16)buff.Length));
            logger.Add("->" + sBtoS(buff, (UInt16)buff.Length));
#endif

            UInt16 crc = chMBCRC16(buff, (ushort)(size - 2));
            if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2]) { logger.Add("Ошибка CRC"); iFail++; return 0; }

            if (buff[1] == 0x10)
            {

                uilHRadrForRead.Add(0);
                uilHRadrForRead.Add(256);
            }
            else
            {
                logger.Add("Ошибка записи нескольких регистров. Код " + buff[1].ToString()); return -1;
            }

            return 0;
        }

        unsafe int iWriteData  (UInt16 adrR, UInt16 cntR, UInt16 adrW, UInt16 cntW, UInt16* data )
        {

            byte* bdata = (byte *) data;
            List<byte> buf = new List<byte>();
            byte dsize = (byte) (cntW*2);

            buf.AddRange
               (
               new byte[] { btDevAdr, 0x17, ((byte *) &adrR  )[1], ((byte*)&adrR  )[0],
                                            ((byte *) &cntR  )[1], ((byte*)&cntR  )[0],
                                            ((byte *) &adrW  )[1], ((byte*)&adrW  )[0],
                                            ((byte *) &cntW  )[1], ((byte*)&cntW  )[0],
                                            dsize
               }
               );

            for (int i = 0; i < cntW; i++)
            {
                buf.AddRange(new byte[] { ((byte *) &data[i])[1], ((byte*) &data[i])[0], });
            }

            vMBCRC16(buf);
#if DEBUG
            //Debug.WriteLine("READ WRITE MULTI HR" + "; adrR=" + adrW.ToString() + "; count=" + cntW.ToString() + "; adrW=" + adrW.ToString() + "; count=" + cntW.ToString());
            //Debug.WriteLine("<-" + sBtoS(buf.ToArray(), 8));
            logger.Add("READ WRITE MULTI HR" + "; adrR=" + adrW.ToString() + "; count=" + cntW.ToString() + "; adrW=" + adrW.ToString() + "; count=" + cntW.ToString());
#endif

            byte[] buff = new byte[10];
            int size = 0;
            try
            {
                if (!spPort.IsOpen) { this.blDevCnctd = false; return -1; }
                spPort.ReadExisting();
                spPort.Write(buf.ToArray(), 0, buf.Count);
                System.Threading.Thread.Sleep(100);
                size = spPort.BytesToRead;
                buff = new byte[size];
                try { spPort.Read(buff, 0, size); }
                catch (TimeoutException ex) { logger.Add("Time out 1"); iFail++; return -1; };
            }
            catch (Exception ex) { Debug.WriteLine("ex 2"); return -1; };

            Debug.WriteLine(sBtoS(buff, buff.Length));

            if (size < 3) { Debug.WriteLine("no ans"); return -1;  }

#if DEBUG
            //Debug.WriteLine("->" + sBtoS(buff, (UInt16)buff.Length));
            logger.Add("->" + sBtoS(buff, (UInt16)buff.Length));
#endif  

            UInt16 crc = chMBCRC16(buff, (ushort)(size - 2));
            if ((crc >> 8) != buff[size - 1] || (crc & 0xFF) != buff[size - 2]) { logger.Add("CRC error"); iFail++; return 0; }

            if (buff[1] == 0x17)
            {
               
             for (int i =0 ; i < buff[2] / 2; i++)
                {
                    uiHoldingReg[i] =(UInt16) ( (buff[3 + i * 2]<<8) + buff[4 + i * 2]);
                }

                uilHRadrForRead.Add(0);
                uilHRadrForRead.Add(256);
            }
            else
            {
                logger.Add("Ошибка чтения/записи нескольких регистров. Код " + buff[1].ToString()); return -1;
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

        unsafe int intReadScope(int div)
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

                    Thread.Sleep(2);

                    if (!spPort.IsOpen || i > 100)
                    {
                        if (spPort.BytesToRead == 5) break;

                        logger.Add("Осциллограф таймаут xx "+ spPort.BytesToRead.ToString() );
                      //  Debug.WriteLine(" SCP TO");
                        
                        return -1;
                    }

                };
                // Debug.WriteLine(i + " ms left");
                Thread.Sleep(5);
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
                   Debug.WriteLine("FIFO not ready");
              // Debug.WriteLine(sBtoS(buff, size));
                if (buff[1] == 1)
                {
                    logger.Add("Осциллограф не найден");
                    return -1;
                }
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
           // Debug.WriteLine("Ch in SCOPE     " + buff[size - 4]);
           // Debug.WriteLine("SCOPE  delay    " + buff[size - 5]);

            for (int i = 2; i < size-7;)
            {

                if (scp_cnt == scp_cntmax) blnScpRstreq = true; 


                if (blnScpRstreq)
                {
                    blnScpRstreq = false;
                    scp_cnt = 0;
                    if (scp_cntmax == 0) scp_cntmax = 1200;
                    if (uialScope[0].Length != scp_cntmax)  //если изменили размер развертки
                    {
                        uialScope[0] = new double[scp_cntmax];  
                        uialScope[1] = new double[scp_cntmax];
                        uialScope[2] = new double[scp_cntmax];
                        uialScope[3] = new double[scp_cntmax];
                    }

                    break;
                }

                for (int k=0; k <buff[size - 4];k++) // для всех каналов во фрейме
                {

                    uialScope[k][scp_cnt] =  (double)(buff[i++] << 8);
                    uialScope[k][scp_cnt] += (double)(buff[i++]     ); // собираем 16 бит

                    if (uialScope[k][scp_cnt] > (1 << 15)) uialScope[k][scp_cnt] = (uialScope[k][scp_cnt] - (1 << 16)); // делаем инт16
                    uialScope[k][scp_cnt] = uialScope[k][scp_cnt] * daGain[k] + daOffset[k]; // смещение усиление
                }

                i+= buff[size - 4]*2*(div-1); //прореживаем значения если делитель больше 1
                scp_cnt++;
                blnScpDataRdy = true;
            }

            

            iScpChNum = buff[size - 4]; // количество каналов
            return buff[size-3]; // возвращаю количество фраймов в фифо МК
        }

        public string sBtoS(byte[] mas, int size)
        {
            string str="";
            int i=0;
            while (i <size)
            {
                str += Convert.ToString(mas[i], 10)+".";
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
