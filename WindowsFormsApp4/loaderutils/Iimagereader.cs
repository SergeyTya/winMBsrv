using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    interface Iimagereader
    {
        void Read();
        void Write(byte[] image);
        bool Compare(byte[] src, int size);

    }

    public class Reader : Iimagereader
    {

        public List<byte> image = new List<byte>();
        public bool valid = false;
        int imageBaseAdr = 0x8000000;
        int _requstedSize = 0;

        public bool Compare(byte[] src, int size)
        {
            bool res = true;
            cout("Comparing images ... ");

            if (src.Length >= size & image.Count >= size)
            {
                for (int i = 0; i < size; i++)
                {
                    if (src[i] != image[i])
                    {
                        res = false;
                    }
                }
            }
            else {
                res = false;
            }

            if (res) {
                cout("true");
                return true;
            }

            cout("false");
            return false;
        }

        public void Read()
        {
            image.Clear(); valid = false;
        }

        public void Write(byte[] Image)
        {
            throw new NotImplementedException();
        }

        public void setImageBaseAdr(int adr) { imageBaseAdr = adr; }
        public int getImageBaseAdr() { return imageBaseAdr; }
        public int getRequstedSize() { return _requstedSize; }
        public void setRequstedSize(int size) { _requstedSize = size; }





        protected void cout(string mes)
        {
            Debug.WriteLine(mes);
        }
    }
}
