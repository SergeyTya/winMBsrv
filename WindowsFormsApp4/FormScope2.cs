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
using System.Security.Cryptography;
using System.Windows.Documents;
using System.Xml;

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

        double _time_step = 1.0;

        private System.Timers.Timer _timer;

        private bool _paused = false;
        private bool Paused { 
            get { return _paused; } 
            set { _paused = value; chnls.addMissingFrame(); }
        }



        Scope_ch chnls = new Scope_ch();


        private static class ScopeParamNames
        {
            public static string SampleRate = "Дискретность";
            public static string Oversampling = "Делитель";
            public static string ChanelsNum = "Кол-во каналов";
            public static string TimeScale = "Разветка, с";
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
            CreateTreeView();
            PrepareGraph();
        }

        private void CreateTreeView()
        {
            XmlDocument xmlDoc;
            string xmlpath = @"..\..\scope_tree.xml";
            xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlpath);
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(new TreeNode(xmlDoc.DocumentElement.Name));
            TreeNode rootNode = new TreeNode();
            rootNode = treeView1.Nodes[0];
            AddNode(xmlDoc.DocumentElement, rootNode);
            treeView1.ExpandAll();
        }

        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i;
            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i< nodeList.Count; i++)
{
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);
                }
            }
            else
            {
                inTreeNode.Text = (inXmlNode.OuterXml).Trim();
            }
        }

        private void log_data(string msg) {

            BeginInvoke(new MyDelegate(() =>
            {
                this.textBox1.AppendText(Environment.NewLine + DateTime.Now.ToLongTimeString()+" "+msg);
            }));
        }

        int update_cnt = 0;
        private void TimerCallback()
        {
            if (server == null) {
                _timer.Stop();
                return;
            }

            

            _ = Task.Run(async () =>
            {
                bool newResValue;
                var myTask = OneTimeReadScopeAsync();
                if (await Task.WhenAny(myTask, Task.Delay((int)(_time_step * 1000))) == myTask)
                {
                    if (Paused) { return; }
                    newResValue = myTask.Result;
                    if (newResValue == false)
                    {
                        return;
                    };

                    update_cnt++;
                    if (update_cnt > 5)
                    {
                        updateGraph();
                        update_cnt = 0;
                    }

                    if (_time_step != chnls._frame_time)
                    {
                        _timer.Stop();
                        _time_step = chnls._frame_time * 0.5;
                        TimerStart();
                    }
                }else {
                    _time += chnls.addMissingFrame();
                    log_data(String.Format("Time out: FrameStep={0} TimeStep={1} ", chnls._frame_time, chnls._chnls_time_step));
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
                log_data(ex.Message);
            }
            TimerStart();
        }

        private void CreateControlTable1() {

            // Развертка
            controlTable1 = new ControlsTable(this.dataGridView1);

            CustomControl tmp = new CustomControl(
               ScopeParamNames.TimeScale,
               max: 100,
               min: 0.2,
               def: 1
            );
            tmp.OnControlEvetn+= TimeScaleChenged;
            controlTable1.addControl(tmp);

            // Усиление
            Action GainEvent = () => { chnls.setGains(
                new double[] {
                controlTable1.getControlValue(ScopeParamNames.Ch1_gain),
                controlTable1.getControlValue(ScopeParamNames.Ch2_gain),
                controlTable1.getControlValue(ScopeParamNames.Ch3_gain),
                controlTable1.getControlValue(ScopeParamNames.Ch4_gain),

                }); 
            };

            tmp = new CustomControl(ScopeParamNames.Ch1_gain, max: 100.0,min: 0.0,def: 1.0);
            tmp.OnControlEvetn += GainEvent.Invoke;
            controlTable1.addControl(tmp);
            tmp = new CustomControl(ScopeParamNames.Ch2_gain, max: 100.0, min: 0.0, def: 1.0);
            tmp.OnControlEvetn += GainEvent.Invoke;
            controlTable1.addControl(tmp);
            tmp = new CustomControl(ScopeParamNames.Ch3_gain, max: 100.0, min: 0.0, def: 1.0);
            tmp.OnControlEvetn += GainEvent.Invoke;
            controlTable1.addControl(tmp);
            tmp = new CustomControl(ScopeParamNames.Ch4_gain, max: 100.0, min: 0.0, def: 1.0);
            tmp.OnControlEvetn += GainEvent.Invoke;
            controlTable1.addControl(tmp);
            // Смещение
            Action OffsetEvent = () => {
                chnls.setOffsets(
                new double[] {
                controlTable1.getControlValue(ScopeParamNames.Ch1_shift),
                controlTable1.getControlValue(ScopeParamNames.Ch2_shift),
                controlTable1.getControlValue(ScopeParamNames.Ch3_shift),
                controlTable1.getControlValue(ScopeParamNames.Ch4_shift),
                });
            };

            tmp = new CustomControl(ScopeParamNames.Ch1_shift, max: 100000.0, min: -100000.0, def: 0.0);
            tmp.OnControlEvetn += OffsetEvent.Invoke;
            controlTable1.addControl(tmp);
            tmp = new CustomControl(ScopeParamNames.Ch2_shift, max: 100000.0, min: -100000.0, def: 0.0);
            tmp.OnControlEvetn += OffsetEvent.Invoke;
            controlTable1.addControl(tmp);
            tmp = new CustomControl(ScopeParamNames.Ch3_shift, max: 100000.0, min: -100000.0, def: 0.0);
            tmp.OnControlEvetn += OffsetEvent.Invoke;
            controlTable1.addControl(tmp);
            tmp = new CustomControl(ScopeParamNames.Ch4_shift, max: 100000.0, min: -100000.0, def: 0.0);
            tmp.OnControlEvetn += OffsetEvent.Invoke;
            controlTable1.addControl(tmp);




            controlTable1.RenderTable();


        }

        private void TimeScaleChenged() {
            double newScaleValue = controlTable1.getControlValue(ScopeParamNames.TimeScale);
            _timer.Stop();
            GraphPane pane = zedGraph1.GraphPane;
            pane.CurveList.Clear();
            chnls.capacity = (int)(newScaleValue / chnls._chnls_time_step);
            log_data("new TimeScale=" + newScaleValue);
            log_data("new capacity=" + chnls.capacity);

            _time = 0;
            createCruves();
            _timer.Start();
        }

        private void updateGraph()
        {
            try
            {                    
                double xmin = _time - chnls.capacity * chnls._chnls_time_step;      
                double xmax = _time;

                GraphPane pane1 = zedGraph1.GraphPane;
                pane1.XAxis.Scale.Min = xmin;
                pane1.XAxis.Scale.Max = xmax;
                zedGraph1.AxisChange();
                zedGraph1.Invalidate();
            }
            catch (System.InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
                log_data(ex.Message);
            }
            catch (System.NullReferenceException ex)
            {
                Debug.WriteLine(ex);
                log_data(ex.Message);
            }
            catch (System.ArgumentOutOfRangeException ex)
            {
                Debug.WriteLine(ex);
                log_data(ex.Message);
            }
        }

        private void createCruves() {

            GraphPane pane1 = zedGraph1.GraphPane;

            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane1.CurveList.Clear();

            // Добавим кривую пока еще без каких-либо точек


            var myCurve1 = pane1.AddCurve("канал 1", chnls._data_ch[0], Color.Red, SymbolType.Circle);
            var myCurve2 = pane1.AddCurve("канал 2", chnls._data_ch[1], Color.Blue, SymbolType.Circle);
            var myCurve3 = pane1.AddCurve("канал 3", chnls._data_ch[2], Color.Green, SymbolType.Circle);
            var myCurve4 = pane1.AddCurve("канал 4", chnls._data_ch[3], Color.Black, SymbolType.Circle);

            myCurve1.Symbol.Size = 2;
            myCurve2.Symbol.Size = 2;
            myCurve3.Symbol.Size = 2;
            myCurve4.Symbol.Size = 2;

        }

        private void PrepareGraph()
        {
            // Получим панель для рисования
            GraphPane pane1 = zedGraph1.GraphPane;

            // Show the horizontal scrollbar
            zedGraph1.IsShowHScrollBar = true;
            // Tell ZedGraph to automatically set the range of the scrollbar according to the data range
            zedGraph1.IsAutoScrollRange = true;
            // Make the scroll cover slightly more than the range of the data
            // (This is a brand new property in the development version -- leave this line out if you are using an older one).
            zedGraph1.ScrollGrace = .05;

            pane1.XAxis.Title.Text = "Время, c";
            pane1.Title.Text = "Scope";

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

            // Устанавливаем интересующий нас интервал по оси Y
            pane1.YAxis.Scale.Min = -1000;
            pane1.YAxis.Scale.Max = 1000;

            zedGraph1.ContextMenuBuilder +=
            new ZedGraphControl.ContextMenuBuilderEventHandler(zedGraph_ContextMenuBuilder);
            zedGraph1.ZoomEvent += 
                new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zedGraph1_ZoomEvent);

            createCruves();

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            zedGraph1.AxisChange();

            // Обновляем график
            zedGraph1.Invalidate();

            chnls.onChanged += TimeScaleChenged;



        }

        private class Scope_ch 
        {
            public delegate void MethodContainer();

            //Событие OnCount c типом делегата MethodContainer.
            public event MethodContainer onChanged;

            public RollingPointPairList[] _data_ch;
            private double[] gains = new double[4] { 1, 1, 1, 1 };
            private double[] offsets = new double[4] { 0, 0, 0, 0 };
            public double _frame_time=0;
            public int _ch_num = 0;
            public bool fifo_mpty = false;
            private int _capacity = 240 * 10;
            public int capacity {
                set {
                    _capacity = value;
                    _data_ch[0] = new RollingPointPairList(_capacity);
                    _data_ch[1] = new RollingPointPairList(_capacity);
                    _data_ch[2] = new RollingPointPairList(_capacity);
                    _data_ch[3] = new RollingPointPairList(_capacity);
                   
                } 
                get { return _capacity; } 
            }

            public double _chnls_time_step = 0;

            public Scope_ch() {
                _data_ch = new RollingPointPairList[4] {
                 new RollingPointPairList(_capacity),
                 new RollingPointPairList(_capacity),
                 new RollingPointPairList(_capacity),
                 new RollingPointPairList(_capacity)
                };
            }

            public void setGains(double[] newValue) {
                gains = newValue;
            }

            public void setOffsets(double[] newValue)
            {
                offsets = newValue;
            }

            public double addMissingFrame() {
                foreach (var el in _data_ch)
                {
                    el.Add(PointPairBase.Missing, PointPairBase.Missing);
                }

                return _frame_time;
            }

            public double addData(byte[] RXbuf, double time_now) 
            {

                /* _______________________________MODBUS SCOPE FRAME (TCP)______________________
                 *
                 *       +-----------+---------------+----------------------------+-------------+
                 * index |0-5   | 6   | 7  | 8 ... 247 |  248  |    249    |  250       |  
                 *       +-----------+---------------+----------------------------+-------------+
                 * FRAME |HEADER| ADR |CMD |    DATA   | delay | ch num    |  FIFO LEN  | 
                 * 
                 */

                int new_ch_num = RXbuf[249];
                double new_timestep = RXbuf[248] * 0.001;
                int fifo_len = RXbuf[250];
                double _time_now = time_now;

                if (new_ch_num != _ch_num | new_timestep != _chnls_time_step | _ch_num == 0 | _chnls_time_step ==0 ) {

                    _time_now += addMissingFrame();
                    _ch_num = new_ch_num;
                    _chnls_time_step = new_timestep;
                    onChanged();
                }

                int count = 0;
                var res = RXbuf.ToList().GetRange(8, 240).GroupBy(_ => count++ / 2).Select(v => (double)IPAddress.NetworkToHostOrder((BitConverter.ToInt16(v.ToArray(), 0)))).ToArray();
                count = 0;
                double[][] res2 = res.GroupBy(_ => count++ % _ch_num).Select(v => v.ToArray()).ToArray();
                _frame_time = _chnls_time_step * res2[0].Length;

                for (int i = 0; i < res2[0].Length; i++)
                {

                    for (int k = 0; k < 3; k++)
                    {

                        if (k < res2.Length)
                        {
                            _data_ch[k].Add(_time_now, res2[k][i] * gains[k] + offsets[k] );
                        }
                        else
                        {
                            _data_ch[k].Add(PointPairBase.Missing, PointPairBase.Missing);
                        }
                    }
                    _time_now += new_timestep;
                }
                return _time_now;
            }
        }


        private async Task<bool> OneTimeReadScopeAsync() {

            try
            {
                byte[] req = new byte[] { 0, 0, 0, 0, 0, 5, 1, 20, 0x1, 0x1, 0x1 };
                var RXbuf = await server.SendRawDataAsync(req);

                

                // check function code
                if (RXbuf[7] != 20 | RXbuf[5] != 245)
                {
                    if (RXbuf[7] != 148)
                    {
                        _time += chnls.addMissingFrame();
                        log_data("Frame: " + Encoding.UTF8.GetString(RXbuf).Trim('\0')+ String.Format(": FrameStep={0} TimeStep={1}", chnls._frame_time, chnls._chnls_time_step));
                    }
                    return false;
                };

                if (RXbuf[250] == 6) {
                    log_data("Scope: FIFO full");
                    _time += chnls.addMissingFrame();
                }
                // add data

                if (!Paused) _time = chnls.addData(RXbuf, _time);
                return true;
            }
            catch (ServerModbusTCPException ex)
            {
                log_data(ex.Message);
                return false;
            }
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
            newMenuItem.Click += (_, __) => { this.Paused = !this.Paused; };

            menuStrip.Items[7].Click += (_, __) => { this.Paused = false; };
        }

        private void zedGraph1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            Paused = true;
        }

    }
}
