using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TestGen.FormGensig2;
using TestGen;
using AsyncSocketTest;
using static AsyncSocketTest.ServerModbusTCP;
using System.Diagnostics;
using System.Threading;
using ZedGraph;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Net;

namespace WindowsFormsApp4
{
    public partial class FormScope2 : Form
    {

        private ControlsTable controlTable1;
        private ControlsTable controlTable2;
        private ServerModbusTCP server;
        private delegate void MyDelegate();

        private ConnectionSetups connection_setups = new ConnectionSetups();
     

        double _time = 0;
        double _time_step = 0.100;
        private System.Timers.Timer _timer;
        private bool _paused;

        LineItem myCurve1;
        LineItem myCurve2;
        LineItem myCurve3;
        LineItem myCurve4;


        private static class ScopeParamNames
        {
            public static string SampleRate = "Дискретность";
            public static string Oversampling = "Делитель";
            public static string ChanelsNum = "Кол-во каналов";
            public static string TimeScale = "Разветка";
            public static string Ch1_gain = "Множитель K1";
            public static string Ch1_shift = "Смещение K1";
            public static string Ch2_gain = "Множитель K2";
            public static string Ch2_shift = "Смещение K2";
            public static string Ch3_gain = "Множитель K3";
            public static string Ch3_shift = "Смещение K3";
            public static string Ch4_gain = "Множитель K4";
            public static string Ch4_shift = "Смещение K4";

        }

        public FormScope2()
        {
            InitializeComponent();
            CreateControlTable1();
            controlTable2 = new ControlsTable(this.dataGridView2);
            PrepareGraph();
        }

        private void log_data(string msg) {

            this.textBox1.Text += msg + "\n";
        }

        private void TimerCallback()
        {
           
            Scope_ch newResValue;

            _ = Task.Run(async () =>
            {
                var myTask = ReadScopeAsync();
                if (await Task.WhenAny(myTask, Task.Delay((int)(_time_step * 1000))) == myTask)
                {
                    newResValue = myTask.Result;
                    if (newResValue == null) { return; };


                    BeginInvoke(new MyDelegate(() =>
                    {
                        try
                        {
                            updateGraph(newResValue);
                        }
                        catch (System.InvalidOperationException ex)
                        {
                            Debug.WriteLine(ex);
                        }

                    }));

                    if (_time_step != newResValue._frame_time)
                    {
                        _time_step = newResValue._frame_time * 0.5;
                        _timer.Stop();
                        TimerStart();
                    }
                }
                else 
                {   
                    newResValue = null; 
                
                }
            });
        }


        public void TimerStart()
        {
            _timer = new System.Timers.Timer( (int) (_time_step * 1000) );
            _timer.Elapsed += (_, __) => TimerCallback();
            _timer.Start();
        }

        public void TimerStop()
        {
            _timer.Stop();
          
        }

        private void ThisFormClosing(object sender, FormClosingEventArgs e)
        {

            if (server != null) if (server.Connected)
                {
                    server.close();
                }
            TimerStop();
        }

        private void FormShown(object sender, EventArgs e)
        {
            try
            {

                //server = new ServerModbusTCP(connection_setups.ServerName, connection_setups.ServerPort);
                server = new ServerModbusTCP("localhost", 8888);

            }
            catch (ServerModbusTCPException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            TimerStart();
        }

        private void CreateControlTable1() {


            controlTable1 = new ControlsTable(this.dataGridView1);
            controlTable1.addControl(new CustomControl(
                ScopeParamNames.SampleRate,
                range: new string[] { "100", "200", "300", "500", "700", "1000" },
                max: 5,
                min: 0
            ));

            controlTable1.addControl(new CustomControl(
                ScopeParamNames.Oversampling,
                range: new string[] { "1", "2", "4", "8", "10" },
                max: 4,
                min: 0
            ));

            controlTable1.addControl(new CustomControl(
                ScopeParamNames.ChanelsNum,
                def: 4,
                max: 4,
                min: 1
            ));

            controlTable1.RenderTable();


        }

        double time_1;
        double time_dt;
        private void updateGraph(Scope_ch data)
        {

            double xmin = _time - _capacity * data._time_step;
            double xmax = _time;
            
            time_dt = _time - time_1;
            time_1 = _time;


            if (!_paused)
            {
                try
                {
                    GraphPane pane1 = zedGraph1.GraphPane;
                    pane1.XAxis.Scale.Min = xmin;
                    pane1.XAxis.Scale.Max = xmax;
                    zedGraph1.AxisChange();
                    zedGraph1.Invalidate();

                }
                catch (System.InvalidOperationException ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            if (data == null | !data.fifo_mpty)
            {

                for (int k = 0; k < 4; k++)
                {
                    _data_ch[k].Add(PointPairBase.Missing, PointPairBase.Missing); 
                }
                _time += _time_step;

                if (data == null) return;

            }


            for (int i = 0; i < data._data[0].Length; i++)
            {

                for (int k = 0; k < 3; k++)
                {

                    if (k < data._data.Length)
                    {
                        _data_ch[k].Add(_time, data._data[k][i]);
                    }
                    else
                    {
                        //_data_ch[k].Add(PointPairBase.Missing, PointPairBase.Missing);
                    }


                    //try
                    //{
                    //    _data_ch[k].Add(_time, data._data[k][i]);
                    //}
                    //catch (Exception e)
                    //{
                    //    
                    //}

                }
                _time += data._time_step;
            }


        }

        private void PrepareGraph()
        {
            // Получим панель для рисования
            GraphPane pane1 = zedGraph1.GraphPane;
            pane1.XAxis.Title.Text = "Время, c";
            pane1.Title.Text = "Задание";

            int labelsfontSize = 12;
            // Установим размеры шрифтов для меток вдоль осей
            pane1.XAxis.Scale.FontSpec.Size = labelsfontSize;
            pane1.YAxis.Scale.FontSpec.Size = labelsfontSize;
            pane1.XAxis.Title.FontSpec.Size = labelsfontSize;
            pane1.Title.FontSpec.Size = labelsfontSize;
            pane1.IsFontsScaled = false;

            // По оси Y установим автоматический подбор масштаба
            pane1.YAxis.Scale.MinAuto = true;
            pane1.YAxis.Scale.MaxAuto = true;

            // !!! Установим значение параметра IsBoundedRanges как true.
            // !!! Это означает, что при автоматическом подборе масштаба
            // !!! нужно учитывать только видимый интервал графика
            pane1.IsBoundedRanges = true;

            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane1.CurveList.Clear();

            // Добавим кривую пока еще без каких-либо точек
            myCurve1 = pane1.AddCurve("канал 1", _data_ch[0], Color.Gray, SymbolType.Circle);
            myCurve2 = pane1.AddCurve("канал 2", _data_ch[1], Color.Blue, SymbolType.None);
            myCurve3 = pane1.AddCurve("канал 3", _data_ch[2], Color.Red, SymbolType.None);
           // myCurve4 = pane1.AddCurve("канал 4", _data_ch[3], Color.Red, SymbolType.None);


            // Устанавливаем интересующий нас интервал по оси Y
            pane1.YAxis.Scale.Min = -1000;
            pane1.YAxis.Scale.Max = 1000;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            zedGraph1.AxisChange();

            // Обновляем график
            zedGraph1.Invalidate();

            zedGraph1.ContextMenuBuilder +=
            new ZedGraphControl.ContextMenuBuilderEventHandler(zedGraph_ContextMenuBuilder);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private class Scope_ch 
        {
            RollingPointPairList[] _data_ch;

            public double _time_step=0;
            public double _frame_time=0;
            public int _ch_num = 0;
            public bool fifo_mpty = false;
            int _capacity = 280 * 5;

            public Scope_ch() {
                _data_ch = new RollingPointPairList[4] {
                 new RollingPointPairList(_capacity),
                 new RollingPointPairList(_capacity),
                 new RollingPointPairList(_capacity),
                 new RollingPointPairList(_capacity)
            };
            }

            public void addData(byte[] RXbuf) {

                /* _______________________________MODBUS SCOPE FRAME (TCP)______________________
                 *
                 *       +-----------+---------------+----------------------------+-------------+
                 * index |0-5   | 6   | 7  | 8 ... 247 |  248  |    249    |  250       |  
                 *       +-----------+---------------+----------------------------+-------------+
                 * FRAME |HEADER| ADR |CMD |    DATA   | delay | ch num    |  FIFO LEN  | 
                 * 
                 */

                int new_ch_num = RXbuf[249];
                double new_timestep = (double)RXbuf[248] / 1000.0;
                int fifo_len = RXbuf[250];

                if (new_ch_num != _ch_num | new_timestep != _time_step) {

                    foreach (var el in _data_ch) { 
                    
                    }
                    _ch_num = new_ch_num;
                    _time_step = new_timestep;
                }

                int count = 0;
                var res = RXbuf.ToList().GetRange(8, 240).GroupBy(_ => count++ / 2).Select(v => (double)IPAddress.NetworkToHostOrder((BitConverter.ToInt16(v.ToArray(), 0)))).ToArray();
                count = 0;
                double[][] res2 = res.GroupBy(_ => count++ % _ch_num).Select(v => v.ToArray()).ToArray();
                _frame_time = _time_step * data[0].Length; 
            }

            public double[][] getData() {
               return _data.ToArray();
            }
        }


        private async Task<Scope_ch> ReadScopeAsync(){

            try
            {
                //var RXbuf = await server.SendRawDataAsync(new byte[] { (byte) connection_setups.SlaveAdr, 20, 0x1, 0x1, 0x1 });
                var RXbuf = await server.SendRawDataAsync(new byte[] { 0, 0, 0, 0, 0, 5, 1, 20, 0x1, 0x1, 0x1 });
               

                // check function code
                if (RXbuf[7] != 20) throw new ServerModbusTCPException("Sope wrong request");
                if (RXbuf[5] != 245) throw new ServerModbusTCPException("Sope wrong responce");

                if (RXbuf[250] != 0) {
                    int fifo_lvl = RXbuf[250];
                    for (int i=0; i< RXbuf[250]; i++)
                    {
                        RXbuf = await server.SendRawDataAsync(new byte[] { 0, 0, 0, 0, 0, 5, 1, 20, 0x1, 0x1, 0x1 });

                    }
                }

                       
                byte _chnl_num = RXbuf[249];
                double t_timestep = (double)RXbuf[248] / 1000.0;
                int count = 0;
                var res = RXbuf.ToList().GetRange(8, 240).GroupBy(_ => count++ / 2).Select(v => (double)IPAddress.NetworkToHostOrder((BitConverter.ToInt16(v.ToArray(), 0)))).ToArray();
                count = 0;
                double[][] res2 = res.GroupBy(_ => count++ % _chnl_num).Select(v => v.ToArray()).ToArray();

                Scope_ch retval = new Scope_ch(res2, (double)RXbuf[248] * 0.001);
                retval.fifo_mpty = RXbuf[250] == 0;
                return retval;

            }
            catch(Exception ex)
            {
                log_data(ex.Message);
            }
            return null;
        }



        private void zedGraph_ContextMenuBuilder(

           ZedGraphControl sender,
           ContextMenuStrip menuStrip,
           Point mousePt,
           ZedGraphControl.ContextMenuObjectState objState
           )
        {
            // Добавим свой пункт меню
            ToolStripItem newMenuItem = new ToolStripMenuItem("Пауза");
            menuStrip.Items.Add(newMenuItem);
            newMenuItem.Click += (_, __) => { this._paused = !this._paused; };
        }

    }
}
