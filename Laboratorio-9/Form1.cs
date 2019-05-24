using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorio_9
{
    public partial class Form1 : Form
    {
        // Atributos para guardar filas y columnas, constante para no
        // poder alterar su valor
        private const int FILAS = 8;
        private const int COLUMNAS = 8;

        // Información de cada celda
        Button[,] buttons;
        bool[,] bombmons;

        public Form1()
        {
            InitializeComponent();

            buttons = new Button[FILAS, COLUMNAS];
            bombmons = new bool[FILAS, COLUMNAS];

            // Agregaremos los botones con código, para no hacerlo 1 a 1...
            for(int fila = 0; fila < FILAS; fila++)
            {
                for(int columna = 0; columna < COLUMNAS; columna++)
                {
                    Button button = new Button();
                    button.Dock = DockStyle.Fill;
                    button.Margin = new Padding(0, 0, 0, 0);
                    button.Padding = new Padding(0, 0, 0, 0);
                    button.FlatStyle = FlatStyle.Popup;
                    button.FlatAppearance.BorderSize = 1;
                    button.Click += cell_Click;
                    tableLayoutPanel1.Controls.Add(button, columna, fila);
                    buttons[fila, columna] = button;
                }
            }

            // Bombmons
            for (int fila = 0; fila < FILAS; fila++)
                for (int columna = 0; columna < COLUMNAS; columna++)
                    bombmons[fila, columna] = false;

            // Creamos los 10 bombmons al azar
            Random random = new Random();
            int creados = 0;
            while(creados < 10)
            {
                int fila = random.Next(FILAS);
                int columna = random.Next(COLUMNAS);
                bool existeUnBombmon = bombmons[fila, columna];
                if (!existeUnBombmon)
                {
                    bombmons[fila, columna] = true;
                    // buttons[fila, columna].Text = "b"; // solo para probar
                    creados++;
                }
            }
        }

        private void cell_Click(object sender, EventArgs e)
        {
            Button boton = (Button)sender;
            int filaBoton = 0;
            int columnaBoton = 0;

            // Buscamos la posición del botón
            for (int fila = 0; fila < FILAS; fila++)
            {
                for (int columna = 0; columna < COLUMNAS; columna++)
                {
                    if (boton == buttons[fila, columna])
                    {
                        filaBoton = fila;
                        columnaBoton = columna;
                        break;
                    }
                }
            }

            // Vemos si en la posición existe un bombmon
            if (ExisteBombmon(filaBoton, columnaBoton))
            {
                MostrarBombmons();
                DeshabilitarBotones();
                MessageBox.Show("Perdiste!! :c");
            }
            else
            {
                // Estilos para que no volvamos a apretarlo
                boton.Enabled = false;
                boton.BackColor = Color.Green;
                boton.Text = ContarBombmons(filaBoton, columnaBoton).ToString();
            }
        }

        private bool ExisteBombmon(int fila, int columna)
        {
            return bombmons[fila, columna];
        }

        private void MostrarBombmons()
        {
            for (int fila = 0; fila < FILAS; fila++)
            {
                for (int columna = 0; columna < COLUMNAS; columna++)
                {
                    if (bombmons[fila, columna])
                    {
                        buttons[fila, columna].BackColor = Color.Red;
                    }
                }
            }
        }

        private void DeshabilitarBotones()
        {
            for (int fila = 0; fila < FILAS; fila++)
            {
                for (int columna = 0; columna < COLUMNAS; columna++)
                {
                    buttons[fila, columna].Enabled = false;
                }
            }
        }

        private int ContarBombmons(int filaCentral, int columnaCentral)
        {
            int cantidad = 0;
            for(int fila = filaCentral - 1; fila <= filaCentral + 1; fila++)
            {
                for(int columna = columnaCentral - 1; columna <= columnaCentral + 1; columna++)
                {
                    // Verificamos si la posición es válida...
                    bool filaValida = fila >= 0 && fila < FILAS;
                    bool columnaValida = columna >= 0 && columna < COLUMNAS;
                    if (filaValida && columnaValida)
                    {
                        if (bombmons[fila, columna])
                        {
                            // encontramos uno!
                            cantidad += 1;
                        }
                    }
                }
            }
            return cantidad;
        }
    }
}
