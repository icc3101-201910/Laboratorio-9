﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Laboratorio_9
{
    public partial class Form1 : Form
    {
        // Atributos para guardar filas y columnas
        private int FILAS;
        private int COLUMNAS;

        private const int BOMBMONS = 10;

        // Información de cada celda
        List<Button> listaBotones;
        Button[,] matrizBotones;
        bool[,] bombmons;
        int time;
        TableLayoutPanel gameTableLayout;

        public Form1()
        {
            InitializeComponent();

            configurarTableLayout();

            matrizBotones = new Button[FILAS, COLUMNAS];
            bombmons = new bool[FILAS, COLUMNAS];
            listaBotones = new List<Button>();

            // Agregaremos los botones con código, para no hacerlo 1 a 1...
            for (int fila = 0; fila < FILAS; fila++)
            {
                for(int columna = 0; columna < COLUMNAS; columna++)
                {
                    Button button = new Button();
                    button.Dock = DockStyle.Fill;
                    button.Margin = new Padding(0, 0, 0, 0);
                    button.Padding = new Padding(0, 0, 0, 0);
                    button.FlatStyle = FlatStyle.Popup;
                    button.Click += cell_Click;
                    button.BackColor = Color.LightGray;
                    button.FlatAppearance.BorderSize = 0;
                    gameTableLayout.Controls.Add(button, columna, fila);
                    matrizBotones[fila, columna] = button;
                    listaBotones.Add(button);
                }
            }

            configurarBombmons();
        }

        private void configurarTableLayout()
        {
            gameTableLayout = new TableLayoutPanel();

            // Será una matriz cuadrada de DIMENSIONES x DIMENSIONES
            int DIMENSIONES = 16;

            // Modificamos las filas y columnas del tableLayout
            gameTableLayout.RowCount = DIMENSIONES;
            gameTableLayout.ColumnCount = DIMENSIONES;

            // Estas constantes es para que sea más fácil de leer el código solamente
            FILAS = DIMENSIONES;
            COLUMNAS = DIMENSIONES;

            // Quiero que sea de más o menos 300x300
            // Hago esto para que el tamaño de todos los botones sea el mismo.
            // WindowsForms hace algo raro con la última columna y última fila
            // cuando estos valores no calzan bien...
            int tamanoBoton = (int)Math.Round(300.0 / DIMENSIONES);
            int tamanoTabla = tamanoBoton * DIMENSIONES;

            for (var i = 0; i < COLUMNAS; i++)
                gameTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, tamanoBoton));

            for (var i = 0; i < FILAS; i++)
                gameTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, tamanoBoton));

            gameTableLayout.Size = new Size(tamanoTabla, tamanoTabla);

            // Lo siguiente centra la tabla
            gameTableLayout.Anchor = AnchorStyles.None;

            tableLayoutPanel2.Controls.Add(gameTableLayout, 0, 1);
        }

        private void configurarBombmons()
        {
            // Bombmons
            for (int fila = 0; fila < FILAS; fila++)
                for (int columna = 0; columna < COLUMNAS; columna++)
                    bombmons[fila, columna] = false;

            // Creamos los 10 bombmons al azar
            Random random = new Random();
            int creados = 0;
            while(creados < BOMBMONS)
            {
                int fila = random.Next(FILAS);
                int columna = random.Next(COLUMNAS);
                bool existeUnBombmon = bombmons[fila, columna];
                if (!existeUnBombmon)
                {
                    bombmons[fila, columna] = true;
                    matrizBotones[fila, columna].Text = "b"; // solo para probar
                    creados++;
                }
            }

            // Reiniciamos los botones (ya que algunos pueden estar con color verde o deshabilitados)
            for (int fila = 0; fila < FILAS; fila++)
                for (int columna = 0; columna < COLUMNAS; columna++)
                {
                    matrizBotones[fila, columna].BackColor = Color.LightGray;
                    matrizBotones[fila, columna].Enabled = true;
                    matrizBotones[fila, columna].Text = "";
                }

            // Comenzamos el timer
            timer.Start();
            time = 0;
            ActualizarTextoReloj();
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
                    if (boton == matrizBotones[fila, columna])
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
                MostrarBombmons(true);
                DeshabilitarBotones();
                PausarTiempo();
                MessageBox.Show("Perdiste!! :c");
            }
            else
            {
                // Estilos para que no volvamos a apretarlo
                boton.Enabled = false;
                boton.BackColor = Color.Green;
                boton.Text = ContarBombmons(filaBoton, columnaBoton).ToString();
            }

            // Verificamos si ganamos
            if (Gana())
            {
                MostrarBombmons(false);
                DeshabilitarBotones();
                PausarTiempo();
                MessageBox.Show($"Yuhuuu!! Ganaste!! Te demoraste {timeText.Text}");
                File.AppendAllText("tiempos.txt", $"{time}\n");
            }
        }

        private void PausarTiempo()
        {
            timer.Stop();
        }

        private bool Gana()
        {
            int botonesVerdes = listaBotones.Where(button => button.BackColor == Color.Green).Count();
            return botonesVerdes + BOMBMONS == FILAS * COLUMNAS;
        }

        private bool ExisteBombmon(int fila, int columna)
        {
            return bombmons[fila, columna];
        }

        private void MostrarBombmons(bool perdio)
        {
            for (int fila = 0; fila < FILAS; fila++)
            {
                for (int columna = 0; columna < COLUMNAS; columna++)
                {
                    if (bombmons[fila, columna])
                    {
                        matrizBotones[fila, columna].BackColor = Color.Red;
                        if (perdio)
                            matrizBotones[fila, columna].Text = "💣";
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
                    matrizBotones[fila, columna].Enabled = false;
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

        private void resetButton_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("¿Quieres reiniciar tu juego?", "Reiniciar", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                configurarBombmons();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time += 1;
            ActualizarTextoReloj();
        }

        private void ActualizarTextoReloj()
        {
            int minutos = time / 60;
            int segundos = time - minutos * 60;
            string textoMinutos = minutos >= 10 ? minutos.ToString() : "0" + minutos;
            string textoSegundos = segundos >= 10 ? segundos.ToString() : "0" + segundos;
            timeText.Text = $"{textoMinutos}:{textoSegundos}";
        }

        private void rankingButton_Click(object sender, EventArgs e)
        {
            Form form = new Ranking();
            form.Show();
        }
    }
}
