using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    class Bootloader
    {

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

        public SerialPort port = null;
        public bool devconected = false;
        public bool ChnlReq = false;
        public bool blogRemStr = false;
        // private bool ProgState;

        private List<byte[]> SavedHexData = new List<byte[]>();
        private List<byte[]> AdrHexData = new List<byte[]>();
        private List<byte[]> FileHexData = new List<byte[]>();
        private bool FValid = false;
        private int FSize = 0;
        public bool ProcBisy = false;
        private int bAdrForSend = 0;
        public List< string [] > logger = new List<string[]>();



        public Bootloader(SerialPort port)
        {
            this.port = port;
        }

        private string ReadPort(byte[] mes, int size)
        {
            int temp_bytecount = 0;
            System.Threading.Thread.Sleep(150);
            while (temp_bytecount != size)
            {
                System.Threading.Thread.Sleep(20);
                if (port.BytesToRead == temp_bytecount) return "таймаут";
                temp_bytecount = port.BytesToRead;
            };
            port.Read(mes, 0, size);
            return null;
        }
        private bool   WaitPortData(int count)
        {
            int i = 0;
            while (port.BytesToRead < count)
            {
                System.Threading.Thread.Sleep(2);
                i++;
                if (i > 250) return false;
            };
            return true;
        }


        private void   GetNativeBoot()/*загрузка через втроенный загрузчик МК*/
        {
            List<byte[]> TempData = new List<byte[]>();
            List<byte[]> TempAdr = new List<byte[]>();
            TempData = FileHexData;
            TempAdr = AdrHexData;

            int i = 0;
            byte[] rxdbuf = new byte[10];
            byte[] txdbuf = new byte[10];
            int baudrate = port.BaudRate;
            FileHexData.Clear();
            AdrHexData.Clear();
            /*начальная скорость*/
            port.BaudRate = 9600;
            logger.Add(new string[] { "Синхронизация ..."});
            port.ReadExisting();
            /* Синхронизация */
            for (i = 0; i < 512; i++)
                port.Write(new byte[] { 0x0 }, 0, 1);
            System.Threading.Thread.Sleep(100);
            try { port.Read(rxdbuf, 0, 3); }
            catch (TimeoutException)
            {
                for (i = 0; i < 512; i++)
                    port.Write(new byte[] { 0x0 }, 0, 1);
                try { port.Read(rxdbuf, 0, 3); }
                catch (TimeoutException)
                {
                    logger.Add(new string[] { "Сбой" });
                    return;
                }

            }
            logger.Add(new string[] { "ok" });

            try
            {
                /* Скорость обмена */
                logger.Add(new string[] { "Скорость обмена ..." });

                txdbuf[0] = (byte)'B';
                txdbuf[1] = (byte)(baudrate);
                txdbuf[2] = (byte)(baudrate >> 8);
                txdbuf[3] = (byte)(baudrate >> 16); 
                txdbuf[4] = (byte)0;

                port.Write(txdbuf, 0, 5);
                System.Threading.Thread.Sleep(500);
                port.ReadExisting();
                port.BaudRate = baudrate;
                /*Приглашение*/
                port.Write(new byte[] { 0xD, 0 }, 0, 1);
                ReadPort(rxdbuf, 3);
                if ((rxdbuf[0] != 0xd) || (rxdbuf[1] != 0xa) || (rxdbuf[2] != 0x3e))
                {
                    logger.Add(new string[] { "Сбой"} ); return;
                }
                logger.Add(new string[] { "ok", "false" });
                /*хекс с загрузчиком берется из ресурсов проекта*/
                logger.Add(new string[] { "Файл загрузчика ..."});
                string readres = null;
                /*создается временный файл*/
                string filename = Application.StartupPath.ToString() + "\\temp_10101.hex";
              //  File.WriteAllText(filename, Properties.Resources._1986_BOOT_UART, Encoding.Default);
                try
                {
                    /*читаются данные*/
                    readres = ReadHexFromFile(filename);
                    /*удаляется временный файл*/
                    File.Delete(filename);
                }
                catch (System.IO.FileNotFoundException) { logger.Add(new string[] { "файл ненайден" }); return; }

                if (string.IsNullOrEmpty(readres)) { readres = " ok "/* + Convert.ToString((FileHexData.Count * 256), 10) + " байт "*/; };
                logger.Add(new string[] { readres });

                logger.Add(new string[] { "Ожидание загрузчика ..." });
                /* Загрузка программы в ОЗУ*/
                int k = 0;
                foreach (byte[] el in AdrHexData)
                {
                    port.ReadExisting();
                    /*Установка адреса и размера */
                    txdbuf[0] = (byte)'L';
                    txdbuf[1] = el[0];
                    txdbuf[2] = el[1];
                    txdbuf[3] = el[2];
                    txdbuf[4] = el[3];
                    txdbuf[5] = (byte)(256 & 0xff);
                    txdbuf[6] = (byte)(256 >> 8); ;
                    txdbuf[7] = 0;
                    txdbuf[8] = 0;
                    port.Write(txdbuf, 0, 9);
                    // System.Threading.Thread.Sleep(100);
                    WaitPortData(1);
                    port.Read(rxdbuf, 0, 1);
                    if ((rxdbuf[0] != (byte)'L'))
                    {
                        logger.Add(new string[] { "Ошибка_1" }); return;
                    }
                    port.ReadExisting();
                    /*Запись массива*/
                    port.Write(FileHexData[k], 0, 256);
                    //System.Threading.Thread.Sleep(100);
                    WaitPortData(1);
                    port.Read(rxdbuf, 0, 1);
                    if ((rxdbuf[0] != (byte)'K'))
                    {
                        logger.Add(new string[] { "Ошибка_2" }); return;
                    }

                    /*Адрес для проверки*/
                    txdbuf[0] = (byte)'Y';
                    port.Write(txdbuf, 0, 9);
                    WaitPortData(258);
                    byte[] mes = new byte[256];
                    port.Read(mes, 0, 1);
                    port.Read(mes, 0, 256);
                    if (!FileHexData[k].SequenceEqual(mes))
                    {
                        logger.Add(new string[] { "Ошибка_3" }); return;
                    }
                    port.ReadExisting();
                    k++;
                }

                /*Команда на запуск с адресом таблицы прерываний*/
                txdbuf[0] = (byte)'R';
                txdbuf[1] = AdrHexData[0][0];
                txdbuf[2] = AdrHexData[0][1];
                txdbuf[3] = AdrHexData[0][2];
                txdbuf[4] = AdrHexData[0][3];
                port.Write(txdbuf, 0, 5);
                // System.Threading.Thread.Sleep(100);
                WaitPortData(1);
                port.Read(rxdbuf, 0, 1);
                if ((rxdbuf[0] != (byte)'R'))
                {
                    logger.Add(new string[] { "Ошибка_4" }); return;
                }
                port.ReadExisting();
                /*ИД загрузчика*/
                port.Write("I");
                System.Threading.Thread.Sleep(100);
                string res = port.ReadExisting();
                if (string.IsNullOrEmpty(res)) return;
                logger.Add(new string[] { res });
                devconected = true;
                FileHexData = TempData;
                AdrHexData = TempAdr;
            }
            catch (TimeoutException)
            {
                logger.Add(new string[] { "нет ответа" });
                return;
            };
        }
        private string ReadHexFromFile(string filename)
        {


            string res = null;
            string str;
            string temp_listrecord = "";
            int adrcount = 0;
            int count = 0;
            int baseadr = 0;
            FileHexData.Clear();
            FValid = false;
            ProcBisy = true;
            byte[] tempbt = new byte[256];
            bool endfile = false;
            bool endrecord = false;

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            FSize = 0;
            bAdrForSend = 0;

            while (endfile = ((str = sr.ReadLine()) != null && string.IsNullOrEmpty(res)))
            {
                /*считываю строку*/
                string bufstr = str;
                /* проверка стартового символа*/
                if (str.IndexOf(":") != 0) { res = "неверная строка"; break; };
                str = str.Substring(1);
                int size = 0, adr = 0, type = 0, crc;
                try
                {
                    /* проверка контрольной суммы */
                    //узнаю размер данных
                    size = Convert.ToInt32(str.Remove(2), 16) + 4;
                    byte[] fulldata = new byte[size]; crc = 0;
                    //читаю данные в массив int
                    int i = 0;
                    foreach (int element in fulldata)
                    {
                        fulldata[i] = Convert.ToByte(str.Remove(2), 16);
                        str = str.Substring(2);
                        crc += fulldata[i];
                        i++;
                    }
                    crc = (byte)(0 - crc);
                    if (crc != Convert.ToInt32(str, 16)) { res = "не совпала CRC"; break; };

                    /* смотрю конец и начало*/
                    type = fulldata[3];
                    if (type == 1) { FSize = adrcount; endrecord = true; break; }
                    if (type == 4)
                    {
                        baseadr = (fulldata[4] << 24) + (fulldata[5] << 16);
                        if (bAdrForSend == 0) bAdrForSend = baseadr;
                        adrcount = 0;
                        continue;
                    };
                    if (type == 5) continue;

                    /* считываю размер и адрес записи*/
                    size = fulldata[0];
                    adr = (fulldata[1] << 8) + fulldata[2];
                    /*в начале файла задаю базовое смещение*/
                    if (FileHexData.Count == 0) { adrcount = adr; };
                    /* пишу в лист стартовый адрес каждого блока */
                    if (count == 0)
                    {
                        byte[] tempadr = new byte[4];
                        int temp = baseadr + adrcount;
                        tempadr[3] = (byte)(temp >> 24);
                        tempadr[2] = (byte)(temp >> 16);
                        tempadr[1] = (byte)(temp >> 8);
                        tempadr[0] = (byte)(temp >> 0);
                        //BeginInvoke(new MyDelegate(textAcsess), Environment.NewLine + Convert.ToString(temp, 16), false);
                        AdrHexData.Add(tempadr);
                    }
                    /* проверка на пропущенные строки по адресу*/
                    if (adr != adrcount) { res = " сбой адресации 0x" + Convert.ToString((baseadr + adrcount), 16); break; };
                    adrcount += size;

                    /*заполняю лист: одна запись - 64 адреса по 4 байта */
                    temp_listrecord += bufstr.Substring(9, size * 2);

                    count++;

                    if (count == 16)
                    {
                        i = 0;

                        foreach (byte element in tempbt)
                        {
                            tempbt[i] = 0xFF;
                            if (i < temp_listrecord.Length / 2) tempbt[i] = Convert.ToByte(temp_listrecord.Substring((i) * 2, 2), 16);
                            i++;
                        }
                        FileHexData.Add(tempbt);
                        /*сбрасываю временные переменные*/
                        temp_listrecord = "";
                        count = 0;
                        tempbt = new byte[256];
                    };



                }
                catch (Exception ex) { res = " неверный формат (R1) " + size /*ex.ToString()*/ ; break; }
            }

            /* если не было последней записи - ошибка*/
            if (!endrecord && string.IsNullOrEmpty(res)) res = "не найден конец файла";

            /*для последней страницы количество байт которой меньше 256*/
            if (endfile && count != 0 && string.IsNullOrEmpty(res))
            {
                int i = 0;
                foreach (byte element in tempbt)
                {
                    tempbt[i] = 0xFF;
                    try
                    {
                        /* добовляю данные в массив*/
                        if (i < temp_listrecord.Length / 2) tempbt[i] = Convert.ToByte(temp_listrecord.Substring((i) * 2, 2), 16);
                    }
                    catch (Exception) { res = " неверный формат (R2)"; break; }
                    i++;
                }
                FileHexData.Add(tempbt);


            }
            sr.Close();
            fs.Close();
            FValid = string.IsNullOrEmpty(res);
            ProcBisy = false;
            return res;
        }
        private string ProgHex()
        {
            string mes = null;
            if (!FValid) return " файл не выбран";
            if (!port.IsOpen) return " порт не открыт";
            port.ReadExisting();
            if (!devconected) return "  устройство не подключенно";
            int startdelay = 0;
            logger.Add(new string[] { "Подготовка...." });
            while (startdelay < 10)
            {
                startdelay++;
                System.Threading.Thread.Sleep(500);
                if (ChnlReq) return " отменено";
            }

            try
            {
                try
                {
                    port.Write("A");
                    port.Write(new byte[] { 0x0, 0x0, (byte)(bAdrForSend >> 16), (byte)(bAdrForSend >> 24) }, 0, 4);
                    System.Threading.Thread.Sleep(500);
                    if (!String.IsNullOrEmpty(ReadPort(new byte[1], 1))) return "неверный базовый адрес";

                    logger.Add(new string[] { "Стираю флеш ...." , "true"});

                    port.Write("E");
                    System.Threading.Thread.Sleep(500);
                    string res1 = port.ReadExisting();
                    if (!res1.Equals("EOK")) { logger.Add(new string[] { " .... ошибка " + res1 }); return null; };
                    logger.Add(new string[] { "Стираю флеш  .... ok", "true"});

                    port.Write("A");
                    port.Write(new byte[] { 0x0, 0x0, (byte)(bAdrForSend >> 16), (byte)(bAdrForSend >> 24) }, 0, 4);
                    System.Threading.Thread.Sleep(500);
                    if (!String.IsNullOrEmpty(ReadPort(new byte[1], 1))) return "неверный базовый адрес";
                    logger.Add(new string[] { "Прошивка .... 0%" });
                    int i = 0, eks = 0;
                    foreach (byte[] el in FileHexData)
                    {
                        byte ks = 0;
                        int progress = (int)(((float)i / FileHexData.Count) * 100);
                        if (logger.Count < 2) logger.Add(new string[] { "Прошивка .... " + progress + "%" , "true"});
                        foreach (byte el2 in el)
                        {
                            ks += el2;
                        }
                        port.Write("P");
                        port.Write(el, 0, 256);
                        WaitPortData(1);
                        if (port.ReadByte() != ks) eks++;
                        i++;
                        if (ChnlReq) { ChnlReq = false; return "отменено"; }
                    }
                    if (eks != 0) logger.Add(new string[] { "Ошибки контрольной суммы .... " + eks + "!" });
                }
                catch (InvalidOperationException) { return " порт не открыт"; }
            }
            catch (TimeoutException) { return " таймаут"; };
            logger.Add(new string[] { "Прошивка .... ok", "true" });
            return null;
        }
        private string VerifyHex()
        {
            byte[] mes = new byte[256];
            string res = null;
            if (!FValid) return " файл не выбран ччч";
            if (!port.IsOpen) return " порт не открыт (V1)";
            port.ReadExisting();
            if (!devconected) return "  устройство не подключенно";
            logger.Add(new string[] { "Сверка файла ....  %"});
            try
            {
                try
                {
                    port.Write("A");
                    port.Write(new byte[] { 0x0, 0x0, (byte)(bAdrForSend >> 16), (byte)(bAdrForSend >> 24) }, 0, 4);
                    if (!String.IsNullOrEmpty(ReadPort(new byte[1], 1))) return "неверный базовый адрес (bAdrForSend)";
                }
                catch (InvalidOperationException) { return " порт не открыт (V2)"; }
            }
            catch (TimeoutException) { return " неверный базовый адрес (bAdrForSend) "; }

            int i = 0;
            foreach (byte[] element in FileHexData)
            {
                i++;
                try
                {
                    try
                    {
                        port.Write("V");
                        if (!WaitPortData(255)) return "таймаут (V3)"; ;
                        System.Threading.Thread.Sleep(2);
                        port.Read(mes, 0, 256);
                        if (ChnlReq) { ChnlReq = false; return "отменено"; }
                    }
                    catch (InvalidOperationException) { return " порт не открыт (V3)"; }
                }
                catch (TimeoutException) { return " таймаут (V4)"; }
                if (!element.SequenceEqual(mes)) return " несовпадение";
                int progress = (int)(((float)i / FileHexData.Count) * 100);
               if(logger.Count<2) logger.Add(new string[] { "Сверка файла .... " + progress + " %" , "true"});
            };
            logger.Add(new string[] { "Сверка файла ... ok", "true" });
            return null;
        }

        public void Reset()
        {
            SavedHexData.Clear();
            AdrHexData.Clear();
            FileHexData.Clear();
            SerialPort port = null;
        }
       
        private void vMBCRC16(List<byte> pucFrame)
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

        public void FOpen()
        {
            if (ProcBisy) return;
            FValid = false;
            AdrHexData.Clear();
            FileHexData.Clear();
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "Файл прошивки|*.hex";
            if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog2.FileName;

            String readres;
            readres = "Читаю файл: ";
            logger.Add(new string[] { readres});
            readres = filename;
            logger.Add(new string[] { readres });

            readres = ReadHexFromFile(filename);
            if (string.IsNullOrEmpty(readres)) { readres = "Ok. Считано " + Convert.ToString((FileHexData.Count * 256), 10) + " байт"; };
            logger.Add(new string[] { readres} );
            ProcBisy = false;

        }

        public void threadVeryfi()
        {
            if (ProcBisy) return;
            if (!devconected) return;

            this.ProcBisy = true;

            string res = VerifyHex();
            if (!string.IsNullOrEmpty(res)) logger.Add(new string[] { "Сверка файла ... " + res });
            ProcBisy = false;

            Reset();

        }

        public void threadProgram()
        {
            if (ProcBisy) return;
            if (!devconected) return;

           // FOpen();
            if (!FValid) return;

            this.ProcBisy = true;

            string res = ProgHex();
            if (!string.IsNullOrEmpty(res))
            {
                logger.Add(new string[] { "Прошивка ... " + res });
                ProcBisy = false;
                return;
            }
            ProcBisy =false;
            threadVeryfi();
            ProcBisy = false;
            Reset();
        }

        public bool GetLoader()
        {
            string mes = null;
            List<byte> cmdGoToBoot = new List<byte>() { 0x1, 0x06, 0x0, 0x0, 0x77, 0x77 };
            vMBCRC16(cmdGoToBoot);
            mes = this.port.ReadExisting();
            this.port.Write(cmdGoToBoot.ToArray(), 0, cmdGoToBoot.ToArray().Length);
            logger.Add(new string[] { "Ожидание загрузчика ...  " });
            System.Threading.Thread.Sleep(500);
            mes = port.ReadExisting();
            port.Write("I");
            System.Threading.Thread.Sleep(100);
            mes = null;
            mes =port.ReadExisting();

            if (String.IsNullOrEmpty(mes))
            {
                logger.Add(new string[] { "нет ответа" });

                return false;
            }
           logger.Add(new string[] { mes });

            devconected = true;
            return true;

        }

        public string sBtoS(byte[] mas, int size)
        {
            string str = "";
            int i = 0;
            while (i < size)
            {
                str += Convert.ToString(mas[i], 10) + ".";
                i++;
            }

            return str;
        }
    }
}
