using AsyncSocketTest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using WindowsFormsApp4;
using ZedGraph;
using static AsyncSocketTest.ServerModbusTCP;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static WindowsFormsApp4.ScopeFrameReader;

namespace TestGen
{
    public partial class FormGensig2 : Form
    {
        private ControlsTable    controlTable;
        private ConnectionSetups connection_setups = new ConnectionSetups();
        private ServerModbusTCP  server;
        private delegate void MyDelegate();
        double _time = 0;
        double _time_step = 0.100;
        private System.Timers.Timer _timer;
        private bool _paused = false;

        private static class GenSigParamNames
        {
            public static string Enable = "Выход Разрешен";
            public static string Type = "Тип сигнала";
            public static string Target = "РХ задание";
            public static string ResEnable = "Выводить отклик";
            public static string Responce = "РВ отклик";
            public static string Period = "Период, с";
            public static string Amplitude = "Aмплитуда";
            public static string Offset = "Смещение";

        }

        public FormGensig2()
        {
            InitializeComponent();
            controlTable = new ControlsTable(this.dataGridView1);
            connection_setups = ConnectionSetups.read();
            PrepareGraph();

            controlTable.addControl( new CustomControl(GenSigParamNames.Enable, checkedbox: true, max: 1, min: 0));
            controlTable.addControl( new CustomControl(GenSigParamNames.ResEnable, checkedbox: true, max: 1, min: 0));
            controlTable.addControl( new CustomControl(GenSigParamNames.Target, def: 20));
            controlTable.addControl( new CustomControl(GenSigParamNames.Responce, def: 12));
            controlTable.addControl( new CustomControl(GenSigParamNames.Type, range: new string[3] { "Синус", "Меандр", "Треугольник" }, max: 2, min: 0));
            controlTable.addControl( new CustomControl(GenSigParamNames.Period, min: 0.1, def: 15));
            controlTable.addControl( new CustomControl(GenSigParamNames.Amplitude, min: 1, def: 3000));
            controlTable.addControl( new CustomControl(GenSigParamNames.Offset, min: 0));
            controlTable.RenderTable();

        }

        public void TimerStart(int intervalMs, Action callback)
        {
            _timer = new System.Timers.Timer(intervalMs);
            _timer.Elapsed += (_, __) => callback();
            _timer.Start();
        }

        public void TimerStop()
        {
            _timer.Stop();
        }

        private int gen_dir = 1;
        private double triag_sig = 0;
        private void TimerCallback()
        {
           
            _ = Task.Run(async () =>
            {
                double amp = controlTable.getControlValue(GenSigParamNames.Amplitude);
                double period = controlTable.getControlValue(GenSigParamNames.Period);
                double offset = controlTable.getControlValue(GenSigParamNames.Offset);
            
                double newRefValue = 0;
                double newResValue = 0;

                switch (controlTable.getControlValue(GenSigParamNames.Type))
                {
                    case 0:
                        newRefValue = amp * Math.Sin(2 * Math.PI * _time / period) + offset;
                        break;
                    case 1:
                        newRefValue = amp * Math.Sin(2 * Math.PI * _time / period);
                        if (newRefValue > 0) newRefValue = amp + offset;
                        if (newRefValue < 0) newRefValue = -amp + offset;
                        break;
                    case 2:
                        double delta = 4 * amp * _time_step / period;
                        triag_sig += delta * gen_dir;
                        if (triag_sig - offset >= amp ) gen_dir = -1;
                        if (triag_sig - offset <= -amp) gen_dir =  1;
                        newRefValue = triag_sig;
                        break;
                    default:

                        break;
                };
               
                var myTask = PollAsync((ushort) newRefValue);
                if (await Task.WhenAny(myTask, Task.Delay((int)(_time_step  * 1000))) == myTask)
                {
                    newResValue =(int) myTask.Result;
                }
                _time += _time_step;

                BeginInvoke(new MyDelegate(() => {
                    try
                    {
                       updateGraph(newRefValue, newResValue, _paused);
                    }
                    catch (System.InvalidOperationException ex)
                    {
                        Debug.WriteLine(ex);
                    }

                }));
            });
        }

        private async Task<Int16> PollAsync(ushort point_val)
        {

            int target_HR = (UInt16)Math.Round(controlTable.getControlValue(GenSigParamNames.Target), 0);
            int target_IR = (UInt16)Math.Round(controlTable.getControlValue(GenSigParamNames.Responce), 0);

     
            if ( controlTable.getControlValue(GenSigParamNames.Enable) == 1)
            {
                var refVal =  await server.ReadWriteHoldingsAsync(connection_setups.SlaveAdr, target_HR, 1, target_HR, new ushort[] { point_val });

                if (controlTable.getControlValue(GenSigParamNames.ResEnable) == 1) {
                    var respVal = await server.ReadInputsAsync(connection_setups.SlaveAdr, target_IR, 1);
                    if (respVal != null)
                    {
                        return (Int16) respVal[0];
                    }
                }
                if (refVal != null)
                {
                    return (Int16) refVal[0];
                }
            }
            return 0;
        }


        private void FormGensig2_Shown(object sender, EventArgs e)
        {
            try {
                
                server = new ServerModbusTCP(connection_setups.ServerName, connection_setups.ServerPort);
              
            }
            catch (ServerModbusTCPException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            
            TimerStart( (int) (_time_step*1000), TimerCallback);
        }

        private void FormGensig2_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if(server != null) if (server.Connected) {
                server.close();
            }
            TimerStop();

        }


        RollingPointPairList _data_refer;
        RollingPointPairList _data_respo;
        RollingPointPairList _data_error;
        int _capacity = 300;
        private void PrepareGraph()
        {
            _data_error = new RollingPointPairList(_capacity);
            _data_refer = new RollingPointPairList(_capacity);
            _data_respo = new RollingPointPairList(_capacity);
            // Получим панель для рисования
            GraphPane pane1 = zedGraph1.GraphPane;
            GraphPane pane2 = zedGraph2.GraphPane;

            pane1.XAxis.Title.Text = "Время, c";
            pane2.XAxis.Title.Text = "Время, c";

            pane1.Title.Text = "Задание";
            pane2.Title.Text = "Ошибка";


            int labelsfontSize = 12;
            // Установим размеры шрифтов для меток вдоль осей
            pane1.XAxis.Scale.FontSpec.Size = labelsfontSize;
            pane1.YAxis.Scale.FontSpec.Size = labelsfontSize;
            pane2.XAxis.Scale.FontSpec.Size = labelsfontSize;
            pane2.YAxis.Scale.FontSpec.Size = labelsfontSize;
            pane1.XAxis.Title.FontSpec.Size = labelsfontSize;
            pane2.XAxis.Title.FontSpec.Size = labelsfontSize;
            pane1.Title.FontSpec.Size = labelsfontSize;
            pane2.Title.FontSpec.Size = labelsfontSize;
            pane1.IsFontsScaled = false;
            pane2.IsFontsScaled = false;

            // По оси Y установим автоматический подбор масштаба
            pane2.YAxis.Scale.MinAuto = true;
            pane2.YAxis.Scale.MaxAuto = true;
            pane1.YAxis.Scale.MinAuto = true;
            pane1.YAxis.Scale.MaxAuto = true;


            // !!! Установим значение параметра IsBoundedRanges как true.
            // !!! Это означает, что при автоматическом подборе масштаба
            // !!! нужно учитывать только видимый интервал графика
            pane1.IsBoundedRanges = true;
            pane2.IsBoundedRanges = true;


            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane1.CurveList.Clear();
            pane2.CurveList.Clear();

            // Добавим кривую пока еще без каких-либо точек
            LineItem myCurve1 = pane1.AddCurve("Расчет ", _data_refer, Color.Blue, SymbolType.Diamond);
            LineItem myCurve2 = pane2.AddCurve(" ", _data_error, Color.Blue, SymbolType.Star);
            LineItem myCurve3 = pane1.AddCurve("Факт ", _data_respo, Color.Red,  SymbolType.Star);


            // Устанавливаем интересующий нас интервал по оси Y
            pane1.YAxis.Scale.Min = -1000;
            pane1.YAxis.Scale.Max =  1000;

            pane2.YAxis.Scale.Min = -1000;
            pane2.YAxis.Scale.Max =  1000;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            zedGraph1.AxisChange();
            zedGraph2.AxisChange();

            // Обновляем график
            zedGraph1.Invalidate();
            zedGraph2.Invalidate();

            zedGraph1.ContextMenuBuilder +=
            new ZedGraphControl.ContextMenuBuilderEventHandler(zedGraph_ContextMenuBuilder);
            zedGraph2.ContextMenuBuilder +=
            new ZedGraphControl.ContextMenuBuilderEventHandler(zedGraph_ContextMenuBuilder);

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

        private void updateGraph(double refVal, double resVal, bool paused) {

            double xmin = _time - _capacity * _time_step;
            double xmax = _time;

            _data_refer.Add(_time, refVal);
            _data_respo.Add(_time, resVal);
            _data_error.Add(_time, refVal - resVal);

            if (!paused) {
                try
                {
                    GraphPane pane1 = zedGraph1.GraphPane;
                    GraphPane pane2 = zedGraph2.GraphPane;
                    pane1.XAxis.Scale.Min = xmin;
                    pane1.XAxis.Scale.Max = xmax;
                    pane2.XAxis.Scale.Min = xmin;
                    pane2.XAxis.Scale.Max = xmax;
                    zedGraph1.AxisChange();
                    zedGraph1.Invalidate();
                    zedGraph2.AxisChange();
                    zedGraph2.Invalidate();
                }
                catch (System.InvalidOperationException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            
        }

    }
}
