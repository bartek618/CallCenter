﻿using CallCenterClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CallCenter
{
    /// <summary>
    /// Interaction logic for AddAgentWindow.xaml
    /// </summary>
    public partial class AddAgentWindow : Window
    {
        public event Action<Agent> OnAgentAdded;
        private readonly MainWindow _mainWindow;
        public AddAgentWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            InitializeComponent();

            NameTextBox.Focus();
        }
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text != "")
            {
                Close();

                Agent newAgent = new Agent(NameTextBox.Text);
                _mainWindow.Agents.Add(newAgent);

                OnAgentAdded(newAgent);
            }
            else
            {
                MessageBox.Show("Name can not be empty.");
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
