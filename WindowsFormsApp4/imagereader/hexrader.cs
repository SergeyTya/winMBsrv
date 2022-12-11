using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    class Hexrader : Reader
    {
        private string fname;
        private List<Record> records = new List<Record>();
        private bool EOFrecord = false;     // .hex end of file flag
        private int base_adr = 0;           // firmware base address
        private int byte_count = 0;             // image byte count
     
        public Hexrader(string filename) {
            fname = filename;
        }

        public new void compare()
        {
            throw new NotImplementedException();
        }

        public new void Read()
        {
            try
            {
                FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                Debug.WriteLine("Open " + fname);
                string str;
                image.Clear();
                while ((str = sr.ReadLine()) != null)
                {
                    bool res = true;
                    res = readRecord(str.ToArray());
                    //this->prinImageSizeInfo
                }
                //Debug.WriteLine("Readed " + image.Count + " bytes");
                Logger = "Readed " + image.Count + " bytes";
                Debug.WriteLine(Logger);
                records.Clear();
            }
            catch (System.IO.FileNotFoundException)
            {
                Logger = "Can't open " + fname;
                Debug.WriteLine(Logger);
                return;
            }
        }

        public new void Write(char[] Image)
        {
            throw new NotImplementedException();
        }

        private void addPld(byte[] pld)
        {
            image.AddRange(pld);
            byte_count += pld.Length; //byte counter
        }

        private void addPld(int cnt)
        {
            if (cnt <= 0) return;

            for (int i = 0; i < cnt; i++)
            {
                image.Add(0);
            }
            byte_count += cnt; //byte counter

            Logger = "Warning: " + " empty byte added ";
            Debug.WriteLine(Logger);
        }

        private bool readRecord(char[] buff) {
            int index = records.Count;

            if (EOFrecord) return false;

            Record tmp =new Record(buff);


            if (tmp.getValidState() == false)
            {
                //cout << "Not Valid Record #" << index << endl;
                Logger = "Not Valid Record #" + index;
                Debug.WriteLine(Logger);
                valid = false;
                return false;
            }

            records.Add(tmp);

            switch (tmp.getRecordType())
            {

                default:
                    break;

                case Record.RecordType.DATA_RECORD:
                    if ((byte_count & 0xFFFF) < tmp.getPldStartAdr())
                    {
                        //add empty bytes if record address greater than current image size
                        addPld(tmp.getPldStartAdr() - (byte_count & 0xFFFF));
                    };

                    addPld(tmp.getPld());

                    break;

                case Record.RecordType.END_OF_FILE:
                    EOFrecord = true;
                    return false;

                case Record.RecordType.ADV_SEGMENT_ADR:
                    int adr = (tmp.getPld()[1] + (tmp.getPld()[0] << 8)) << 16;

                    if (index > 0)
                    {
                        // add empty bytes if base address is changing
                        addPld(adr - (base_adr + byte_count));
                        //cout << "Flash base address " << int_to_hex(adr) << endl;
                    }
                    else
                    {
                        base_adr = adr; // setup image base address
                        Logger = "Flash base address " + adr;
                        Debug.WriteLine(Logger);
                    }

                    break;
            }

            return true;
        }
    }

    class Record
    {

	    public enum RecordType{
            CRC_ERR=-3,
	        LINE_START_ERR,
			NO_REC,
			DATA_RECORD,
	        END_OF_FILE ,
	        SEGMENT_ADR ,
			RUN_ADR80x86 ,
			ADV_SEGMENT_ADR ,
			RUN_ADR
        }

        public Record(char[] buff) {

            line = new String(buff);
            string tmp = line.Substring(1);
            int size = tmp.Length / 2;
            int crc_pos = size - 1;
            byte crc = 0;

            if (line[0] == ':')
            {
                valid = true;
                recordType = RecordType.NO_REC;
            }
            else
            {
                valid = false;
                recordType = RecordType.LINE_START_ERR;
                //std::cout << "Record start error:" << line << std::endl;
                System.Diagnostics.Debug.WriteLine("Record start error:" + line);
                return;
            };

            for (int i = 0; i < size; i++)
            {
                //  dataLine.Add(uint8_t(stoi(, 0, 16)));
                string prt;
                if (tmp.Length > 2) {
                    prt = tmp.Remove(2);
                    tmp = tmp.Substring(2);
                }
                else {
                    prt = tmp;
                }
                dataLine.Add(Convert.ToByte(prt, 16));
                if (i != crc_pos) crc += dataLine[i];
            };
            crc = (byte) (0xff & (0 - crc));

            if (dataLine[crc_pos] == crc)
            {
                valid = true;
                dataByteCount = dataLine[0];
                pldStartAdr = (dataLine[1] << 8) + dataLine[2];
                recordType = (RecordType) dataLine[3];
            }
            else
            {
                valid = false;
                recordType = RecordType.CRC_ERR;
                System.Diagnostics.Debug.WriteLine("Record CRC error:" + line);
                return;
            }
            return;
        }

        public bool getValidState() { return valid; }
        public RecordType getRecordType() {return recordType; }

        public int getPldStartAdr() {return pldStartAdr; }
        public int getPldSize()     { return dataByteCount; }
        public byte[] getPld()             { return dataLine.ToArray(); }

    string getLine()     {return line;}

	    private string line;        // records string line from
        List <byte> dataLine = new List<byte>();       // records data line
        bool valid = false;         // records valid flag
        int dataByteCount = 0;      // records pay load byte count
        int pldStartAdr = 0;        // records pay load flash address

        RecordType recordType = RecordType.NO_REC;

    }
}
