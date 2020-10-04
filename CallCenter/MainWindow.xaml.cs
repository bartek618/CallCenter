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
        private CallsGenerator _callsGenerator;
        private Queue<Call> _calls = new Queue<Call>();
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

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Agent> Agents { get; private set; } = new ObservableCollection<Agent>();

        public MainWindow()
        {
            this.Closed += MainWindow_Closed;

            Agent.MinCallTimeInSec = 0;
            Agent.MaxCallTimeInSec = 10;
            Agent.OnCallEnded += Agent_OnCallEnded;
            Agent tom = new Agent("Tom");
            Agents.Add(tom);

            _callsGenerator = new CallsGenerator(0, 60);
            _callsGenerator.OnCallGenerated += CallsGenerator_OnCallGenerated;
            _callsGenerator.StartGeneratingContinously();

            InitializeComponent();

            DataContext = this;

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(100);
                    if (_calls.Count != 0 && Agents.Count != 0)
                    {
                        foreach (Agent agent in Agents)
                        {
                            if (agent.Busy == false)
                            {
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
            addAgentWindow.Show();
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Agent agentToDelete = AgentsListView.SelectedItem as Agent;
            Agents.Remove(agentToDelete);
            agentToDelete.Dispose();

            NotifyPropertyChanged(nameof(Agents));
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
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
    }
}
