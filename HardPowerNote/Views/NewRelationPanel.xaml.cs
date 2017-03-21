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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HardCJT.ViewModels;

namespace HardPowerNote {
    /// <summary>
    /// Interaction logic for NewRelationPanel.xaml
    /// </summary>
    public partial class NewRelationPanel : StackPanel {
        public NewRelationPanel() {
            InitializeComponent();
        }

        public void Save_Click(object sender, EventArgs e) {
            //(DataContext as RelationshipVM).AddRelationToDatabase();
        }

    }
}
