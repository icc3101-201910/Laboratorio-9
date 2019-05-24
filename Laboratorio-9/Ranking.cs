using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Laboratorio_9
{
    public partial class Ranking : Form
    {
        // Sí, es posible tener más forms :)
        public Ranking()
        {
            InitializeComponent();
            var tiempos = File.ReadAllLines("tiempos.txt")
                .Select(tiempo => int.Parse(tiempo))
                .OrderBy(tiempo => tiempo)
                .Take(10);
            for(int i = 0; i < tiempos.Count(); i++)
            {
                string ranking = $"{i + 1}";
                string tiempo = $"{tiempos.ElementAt(i)}";
                listView1.Items.Add(new ListViewItem(new string[] { ranking, tiempo }));
            }
        }

        private void Ranking_Load(object sender, EventArgs e)
        {

        }
    }
}
