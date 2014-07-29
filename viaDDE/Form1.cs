using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using NDde.Client;

namespace viaDDE
{
    public partial class Form1 : Form
    {
        public delegate void TickReceived(Tick tick);

        public event TickReceived OnTick;

        public string[] allSymbols = new string[]{
            "EURUSD",
            "USDCHF",
            "USDJPY",
            "GBPUSD",
            "EURJPY",
            "EURCHF",
            "EURGBP",
            "USDCAD",
            "AUDUSD",
            "GBPCHF",
            "GBPJPY",
            "CHFJPY",
            "NZDUSD",
            "EURCAD",
            "AUDJPY",
            "EURAUD",
            "AUDCAD",
            "NZDJPY"
            };

        private Dictionary<string, List<double>> dataSource = new Dictionary<string,List<double>>();
        
        public Form1()
        {
            InitializeComponent();
            foreach (var symbol in allSymbols)
            {
                dataSource.Add(symbol, new List<double>());// create database for each symbol and currentylu 19 Symbols 
            }
            TheContainer.TheForm = this;
            this.OnTick += Form1_OnTick;
           



        }

        void Form1_OnTick(Form1.Tick tick)
        {
            if (this.InvokeRequired)
            {
                // Execute the same method, but this time on the GUI thread
                this.BeginInvoke(new TickReceived(Form1_OnTick), tick);

                // we return immedeately
                return;
            }
            
            
            
            // A first chance exception of type 'System.InvalidOperationException' occurred in System.Windows.Forms.dll
            //Additional information: Cross-thread operation not valid: Control 'cboSymbols' accessed from a thread other than the thread it was created on.
            // Control.CheckForIllegalCrossThreadCalls Property == false will fix 
            //'TheContainer.TheForm.cboSymbols.SelectedItem' threw an exception of type 'System.InvalidOperationException'
            if ((string)TheContainer.TheForm.cboSymbols.SelectedItem == tick.symbol)
            {   
                // Only 1 Chart -- Is tick Match 
               // Log(tick.Bid)
                // arrays #, datasource string 
                Log(tick.bid.ToString());
                chart1.Update();
                chart1.DataBind();
                
            }
        }

        private void eventLog1_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //------------------JULY 2014 ------------------------
            // Form1_Load event handler to initialize the Chart   ||--------------------- TEST 1
            //----------------------------------------------------
          //  this.princetonTempTableAdapter.Fill(this.pt1.PrincetonTemp);
        /*
            DataTable dtable=new DataTable();
           
          //  dtable = pt1.PrincetonTemp;
          
           // chart1.DataSource = bindingSource1;
       
            chart1.Series["Series1"].XValueMember =
         
            Convert.ToString(dtable.Columns[1]);
         
            chart1.Series["Series1"].YValueMembers =
         
            Convert.ToString(dtable.Columns[2]);
         
            chart1.Series["Series2"].XValueMember =
        
            Convert.ToString(dtable.Columns[1]);
        
            chart1.Series["Series2"].YValueMembers =
         
            Convert.ToString(dtable.Columns[3]);
         
            chart1.DataBind();
         
            chart1.Visible = true;
         */
          
            //------------------JULY 2014 ------------------------
            // Form1_Load event handler to initialize the Chart   ||--------------------- END TEST 1
            //----------------------------------------------------
            //---------THIS SHOULD WORK - JULY 2014-------
            // Set palette.
               this.chart1.Palette = ChartColorPalette.Bright;
            // Set title.
              this.chart1.Titles.Add("FX4BTC");
            var series = new Series();
            series.Name = "Series1";
            series.ChartType = SeriesChartType.FastLine;
            series.XValueType = ChartValueType.DateTime;
            series.YValueType = ChartValueType.Double;// for Forex currency 12.90493 value

            //------------------JULY 2014 ------------------------
            // Form1_Load event handler to initialize the Chart  || --------------------- TEST 2 
            //----------------------------------------------------
            //--assign an array of strings (Series) and an array of integers (Points)
            // set up some data
           var xvals = new[]
               {
                  new DateTime(2012, 4, 4), 
                  new DateTime(2012, 4, 5), 
                   new DateTime(2012, 4, 6), 
                   new DateTime(2012, 4, 7)
               };
           var yvals = new[] { 0.9050,0.90515,0.90520,0.90536 };
            //-for-loop, we add the strings to the Series collection and add the integers to the Points collections on those Series.
            // Data arrays.


          

            // Bind Data
            //----------------------------------------------------
            // xvals = Datetime, 
            // yvals = bid price
            //----------------------------------------------------
            // Bind Data
            chart1.Series["Series1"].Points.DataBindXY(xvals, yvals);
            
          //  chart1.DataSource = dataSource[(string)cboSymbols.SelectedItem];
          //  chart1.DataBind(dataSource)

            //------------------JULY 2014 ------------------------
            // Form1_Load event handler to initialize the Chart  || ------------------------------------ End Test 2
            //----------------------------------------------------
            // Load DataSOurce

            
            // bind IListSource, the interface can be used for Data Table and Data Set classes.
            //IList, the interface can be used for 1D arrays.
            //IBindingList, the interface can be used for the generic Binding List class.
            //IBindingListView, the interface can be used for Binding Source class.
            //properties of DataSource and ValueMember to data binding




            InitDDEClient();
            cboSymbols.Items.AddRange(allSymbols);
            cboSymbols.SelectedIndex = 0;

            //------------------------------------------------------
            // Add a button to allow the saving of the image for later use. 
            //this.chart1.SaveImage("C:\\chart.png", ChartImageFormat.Png);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopDDE();
        }
        //--strategy
        private void Log(string pText)
        {
            textBox1.AppendText(pText);
            textBox1.AppendText("\r\n");
        }

        private void OnAdvise(object sender, DdeAdviseEventArgs args)
        {
            //=========================================================================================
            //Log(args.Item + " " + args.Text); // get the DDE feed

            string[] fxData = (args.Item + " " + args.Text).Split(new char[] { ' ' });

            Tick tick = null;
            
            try{
                tick = new Tick()
                {
                    //EURUSD 2014/05/06 23:59 1.34455 1.34456
                    symbol = fxData[0],
                    date = DateTime.Parse(fxData[1] + " " + fxData[2]),
                    bid = double.Parse(fxData[3]),
                    ask = double.Parse(fxData[4])
                   

                };   
            }
           
            catch(Exception)
            {
                Log("Invalid tick data");
            }

            //-- Log Price -- works -- prints bid price for All symbol 
           //   Log(fxData[3]); Double checked July 2014 - so data is local and present here, 
            //-------------

           


           //------------------------------------------------
            // Bind Data
         //   chart1.Series["Series1"].Points.DataBindXY(fxData[1], fxData[3]);
        //    chart1.Series["Series1"].Points.DataBindXY(fxData[2], fxData[3]);
          //  chart1.Series["Series1"].Points.DataBind(dataSource, fxData[2], fxData[3],);

        //    string xVal = fxData[2];// Hours:Minutes == 23:59
        //    string YVal = fxData[3];

         //   chart1.Series["Series1"].Points.DataBindXY(xVal, YVal);
       //     chart1.Series["Series1"].Points.AddXY(xVal, YVal);

         //   chart1.Series[0].Points.AddXY(date, bid);

            //---------------------------------------------------
            // create data source
            if(tick != null)
            {
                if (dataSource.ContainsKey(tick.symbol))
                {
                    dataSource[tick.symbol].Insert(0, tick.bid);
                 //   Log(tick.bid);
                }
                else
                {
                   dataSource.Add(tick.symbol, new List<double>(){ tick.bid });
                }
                if (OnTick != null)
                {
                    OnTick.Invoke(tick);
                }
            }
           

            //------------------------------

        }

        private void OnDisconnected(object sender, DdeDisconnectedEventArgs args)
        {
            Log(
                "OnDisconnected: " +
                "IsServerInitiated=" + args.IsServerInitiated.ToString() + " " +
                "IsDisposed=" + args.IsDisposed.ToString());
            TheContainer.TheDdeClient = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ctrader open api - Buy Send 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ctrader open api - Sell Send 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //ctrader open api - Find and Close All Profitable Positions
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ctrader open api - Find and close All positions
            //that are at a floating Loss more than COmmission Cost plus 1 pip
        }

        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //user input - Select Curreny Pair for Renko Chart
            if (!dataSource.ContainsKey((string)cboSymbols.SelectedItem))
            {
                dataSource.Add((string)cboSymbols.SelectedItem, new List<double>());
            }
            // Sets ut the database 
            // Get and Set the dataSource for chart1 
            // database contains all the contents from the public class TICK , which containts, symbol, bid, time
            chart1.DataSource = dataSource[(string)cboSymbols.SelectedItem];

            Log("Changed data source to " + cboSymbols.SelectedItem);
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void btnStartDDE_Click(object sender, EventArgs e)
        {
            // Button " Start"
            ConnectToDDE();//works - debugger through
            SubscribeAll();
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopDDE();
        }


        private void InitDDEClient()
        {
            DdeClient client = new DdeClient("MT4", "QUOTE");
            TheContainer.TheDdeClient = client;

            // Subscribe to the Disconnected event.  This event will notify 
            // the application when a conversation has been terminated.
            client.Disconnected += OnDisconnected;
            client.Advise += OnAdvise;// ==================================================//
        }

        private void ConnectToDDE()
        {
            if (!TheContainer.TheDdeClient.IsConnected)
            {
                try
                {
                    TheContainer.TheDdeClient.Connect();
                    Log("DDE Client Started");

                }
                catch (Exception)
                {
                    Log("An exception was thrown during DDE connection");
                    Log("Ensure Metatrader 4 is running and DDE is enabled");
                    Log("To activate the DDE Server go to Tools -> Options");
                    Log("On the Server tab, ensure \"Enable DDE server\" is checked");
                }
            }
            else
            {
                Log("Already connected");
            }
        }

        private void SubscribeAll()
        {
            if (TheContainer.TheDdeClient != null && TheContainer.TheDdeClient.IsConnected)
            {
                var client = TheContainer.TheDdeClient;
                foreach (var symbol in allSymbols)
                {
                    client.StartAdvise(symbol, 1, true, 60000);
                }
              
            }
        }

       

        private void StopDDE()
        {
            if (TheContainer.TheDdeClient != null && TheContainer.TheDdeClient.IsConnected)
            {
                TheContainer.TheDdeClient.Disconnect();
            }

            Log("Disconnected");
            
        }

        public class Tick
        {
            public string symbol;
            public double bid;
            public double ask;
            public DateTime date;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }


        //---end strategy
    }

    // Not sure what the purpose of this..Singleton ?
    // these static variables could be on Form1
   
    public class TheContainer
    {
        public static Form1 TheForm;
        public static DdeClient TheDdeClient;
    }

}
