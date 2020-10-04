using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CallCenterClassLibrary;

namespace CallCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Fields and properties
        private readonly CallsGenerator _callsGenerator;
        private readonly Queue<Call> _calls = new Queue<Call>();
        public ObservableCollection<Agent> Agents { get; private set; } = new ObservableCollection<Agent>();
        private string _consoleString = "";
        public string ConsoleString
        {
            get { return _consoleString; }
            private set
            {
                _consoleString = value;
                NotifyPropertyChanged();
            }
        }
        private bool stop;
        #endregion
        #region Events
        public event PropertyChangedEventHandler PropertyChanged; 
        #endregion
        #region Methods
        public MainWindow()
        {
            this.Closed += MainWindow_Closed;

            //Initialize Agents.
            Agent.OnCallEnded += Agent_OnCallEnded;
            Agent.MinCallTimeInSec = 0;
            Agent.MaxCallTimeInSec = 30;
            Agent tom = new Agent("Tom");
            Agents.Add(tom);

            //Initialize calls generator.
            _callsGenerator = new CallsGenerator(0, 30);
            _callsGenerator.OnCallGenerated += CallsGenerator_OnCallGenerated;
            _callsGenerator.StartGeneratingContinously();

            InitializeComponent();

            DataContext = this;

            RunCallsDispatcherTask();
        }
        private void RunCallsDispatcherTask()
        {
            Task.Run(() =>
            {
                while (!stop)
                {
                    Thread.Sleep(100);

                    //if there are calls and agents available
                    if (_calls.Count != 0 && Agents.Count != 0)
                    {
                        //Iterate through agents.
                        foreach (Agent agent in Agents)
                        {
                            if (agent.Busy == false)
                            {
                                //Take call.
                                Call currentCall = _calls.Dequeue();
                                agent.TakeCall(currentCall);
                                UpdateConsoleString($"{agent.Name} answered call {currentCall.Id}");
                                break;
                            }
                        }
                    }
                }
            });
        }
        private void CallsGenerator_OnCallGenerated(Call call)
        {
            _calls.Enqueue(call);
            UpdateConsoleString($"Generated call {call.Id}. Calls waiting: {_calls.Count}");
        }
        private void Agent_OnCallEnded(Agent agent, Call call)
        {
            UpdateConsoleString($"{agent.Name} ended call {call.Id}. Duration: {call.DurationInSec}s.");
        }
        private void UpdateConsoleString(string message)
        {
            ConsoleString = ConsoleString.Insert(0, message + Environment.NewLine);
        }
        private void GenerateCallButton_Click(object sender, RoutedEventArgs e)
        {
            _callsGenerator.GenerateCall();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddAgentWindow addAgentWindow = new AddAgentWindow(this);
            addAgentWindow.OnAgentAdded += AddAgentWindow_OnAgentAdded;
            addAgentWindow.Show();
        }
        private void AddAgentWindow_OnAgentAdded(Agent agent)
        {
            UpdateConsoleString($"Agent {agent.Name} added");
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AgentsListView.SelectedItem is Agent agentToDelete)
            {
                Agents.Remove(agentToDelete);
                agentToDelete.Dispose();
                NotifyPropertyChanged(nameof(Agents));

                UpdateConsoleString($"Agent {agentToDelete.Name} deleted");
            }
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //Stop calls dispatcher task.
            stop = true;

            //Dispose.
            _callsGenerator.Dispose();
            foreach (Agent agent in Agents)
            {
                agent.Dispose();
            }
        }
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion
    }
}
