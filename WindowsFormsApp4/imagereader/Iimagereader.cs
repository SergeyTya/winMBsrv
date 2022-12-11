using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    interface Iimagereader
    {
        void Read();
        void Write(char[] image);
        void Compare();

    }

    public class Reader : Iimagereader
    {

        public void Compare()
        {
            throw new NotImplementedException();
        }

        public void Read()
        {
            image.Clear(); valid = false;
        }

        public void Write(char[] Image)
        {
            throw new NotImplementedException();
        }

        private List<string> logger;
        
        public string Logger {
            set {
                logger.Add(value);
            }

            get {
                return logger[logger.Count - 1];
            }
        }


        public List<byte> image = new List<byte>();
        public bool valid = false;
        int imageBaseAdr = 0x8000000;
        int _requstedSize = 128000;


    }
}
