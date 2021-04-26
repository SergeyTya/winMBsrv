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
            if(_buffer[chnl].Count > _bufferSize) _buffer[chnl].RemoveRange(0, _buffer[chnl].Count - _bufferSize);
        }

        private void reset() {
            List<double> t_time = Enumerable.Range(0, _bufferSize).ToList().Select(_ => _ * _timestep).ToList();
            List<double> t_y = Enumerable.Range(0, _bufferSize).ToList().Select(_ => _ * 0.0).ToList();

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
            var res = buff.ToList().GetRange(2, 240).GroupBy(_ => count++ / 2).Select(v => IPAddress.NetworkToHostOrder((BitConverter.ToInt16(v.ToArray(), 0)))).ToArray();
            count = 0;
            short[][] res2 = res.GroupBy(_ => count++ % _chnl_num).Select(v => v.ToArray()).ToArray();
            
            if (res2.Length >= 1)   Add(res2[0].Select(_ => (double) _).ToList(), 0);
            if (res2.Length >= 2) { Add(res2[1].Select(_ => (double) _).ToList(), 1); } else { _buffer[1].Clear(); };
            if (res2.Length >= 3) { Add(res2[2].Select(_ => (double) _).ToList(), 2); } else { _buffer[2].Clear(); };
            if (res2.Length == 4) { Add(res2[3].Select(_ => (double) _).ToList(), 3); } else { _buffer[3].Clear(); };
        }

        public void Resize(int bufferSize) {

            reset();
            _bufferSize = bufferSize;
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

    }
}
