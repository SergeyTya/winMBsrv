using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4
{
    public class ScopeFrameReader
    {
        List< List<double>> _buffer = new List<List<double>>();
        List<double> _time = new List<double>();
        int _bufferSize;
        int _pointer;
        Object _lock = new object();
        int _chnl_num;
        double _timestep;

        public double TimeStep { get { return _timestep; } }
        public int BufferSize { get { return _bufferSize; } set { Resize(value); } }
        public List<double> Time { get { return _time; } }

        public class Target_cnl {

            public UInt32 _adr;
            public string _info;

            private int _type;
            public string _type_name;

            public int Type {
                get { return _type; }
                set {
                    _type = value;
                    switch (_type)
                    {
                        case 0: _type_name = "undef"; break;
                        case 1: _type_name = "int8_t"; break;
                        case 2: _type_name = "uint8_t"; break;
                        case 3: _type_name = "int16_t"; break;
                        case 4: _type_name = "uint16_t"; break;
                        case 5: _type_name = "int32_t"; break;
                        case 6: _type_name = "uint32_t"; break;
                        case 7: _type_name = "_iq"; break;
                        case 8: _type_name = "float"; break;
                        default: break;
                    }
                }
            }

            public Target_cnl(UInt32 adr,string info, int type) {
                _adr = adr;
                _info = info;
                Type = type;
            }
        }

        public List<Target_cnl> target_chnl = new List<Target_cnl>();

        public ScopeFrameReader(int bufferSize)
        {
            _buffer.Add(new List<double>(bufferSize));
            _buffer.Add(new List<double>(bufferSize));
            _buffer.Add(new List<double>(bufferSize));
            _buffer.Add(new List<double>(bufferSize));
            _time = new List<double>(_bufferSize);

            _bufferSize = bufferSize;
            _pointer = 0;

            reset();
        }

        private void Add(List<double> toadd, int chnl) {
            _buffer[chnl].AddRange(toadd);
            _pointer += toadd.Count;
            if (_pointer > _bufferSize) _pointer = 0;
            if (_buffer[chnl].Count > _bufferSize)
            {
                _buffer[chnl].RemoveRange(0, _buffer[chnl].Count - _bufferSize);
                
            }
        }

        private void AddTime(double now, int count) {
            _time.AddRange(Enumerable.Range(0, count).Select(_ => (double)(_ * _timestep + now + _timestep)));
            if (_time.Count > _bufferSize)  _time.RemoveRange(0, _time.Count - _bufferSize);

        }

        private void reset() {
            List<double> t_time = Enumerable.Range(0, _bufferSize).Select(_ => _ * _timestep).ToList();
            List<double> t_y = Enumerable.Range(0, _bufferSize).Select(_ => _ * 0.0).ToList();

            foreach (var item in _buffer) { item .Clear(); item.AddRange(t_y); };
            _time.Clear();
            _time.AddRange(t_time);
            _pointer = 0;
        }

        public void ReadFrame(byte[] buff) {
            if (buff.Length != 247) return;
            if (_chnl_num != buff[247 - 4]) { reset(); _chnl_num = buff[247 - 4]; };
            double t_timestep = (double) buff[247 - 5] / 1000.0;
            if(_timestep != t_timestep) {  _timestep = t_timestep; reset(); };
            int count = 0;
            var res = buff.ToList().GetRange(2, 240).GroupBy(_ => count++ / 2).Select(v => (double) IPAddress.NetworkToHostOrder((BitConverter.ToInt16(v.ToArray(), 0)))).ToArray();
            count = 0;
            double[][] res2 = res.GroupBy(_ => count++ % _chnl_num).Select(v => v.ToArray()).ToArray();

            if (res2.Length >= 1)   Add(res2[0].ToList(), 0);
            if (res2.Length >= 2) { Add(res2[1].ToList(), 1); } else { _buffer[1].Clear(); };
            if (res2.Length >= 3) { Add(res2[2].ToList(), 2); } else { _buffer[2].Clear(); };
            if (res2.Length == 4) { Add(res2[3].ToList(), 3); } else { _buffer[3].Clear(); };

            AddTime(_time.Last(), res2[0].ToList().Count);
        }

        private void Resize(int bufferSize) {

            reset();
            _bufferSize = bufferSize;
        }

        public void SetSweepTime(double time) {
            int count = (int) (time / _timestep);
            if (count < 500) count = 500;
            Resize(count);
        }

        public int GetChlDataCount(int chnl)
        {
            if (chnl > _buffer.Count - 1) return 0;
            return _buffer[chnl].Count;
        }

        public List<double> GetChnlData(int chnl)
        {
            if (chnl > _buffer.Count - 1) return null;
            return _buffer[chnl];
        }

        public void ReadTargetChl(byte[] buff) {


            /* _______________________________MODBUS SCOPE CHANNEl LIST FRAME________________________________
            *
            *       +-----------+---------------+----------------------------+-------------+
            * index | 0   | 1  | 2    |                                   | 252 253 |
            *       +-----------+---------------+----------------------------+-------------+
            * FRAME | ADR |CMD |CHNNUM|      ADR 4b, INFO 5b, TYPE 1b     |   CRC   |
            *
            */

            target_chnl.Clear();

            int cnlnum =(int) buff[2];
            int count = 0;
            var res = buff.ToList().GetRange(3, buff.Length - 3).GroupBy(_ => count++ / 10 );

            foreach (var item in res)
            {
                if (item.Count() < 10) break;
                UInt32 adr =BitConverter.ToUInt32(item.ToArray(),0);
                string info = Encoding.UTF8.GetString(item.ToList().GetRange(4, 5).ToArray());
                int type = item.ToArray()[9];

                target_chnl.Add(new Target_cnl(adr, info, type) );
            }

        }

    }
}
