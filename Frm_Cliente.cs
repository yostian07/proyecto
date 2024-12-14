using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Cliente
{
    public partial class Frm_Cliente : Form
    {
        // Declaración de atributos para la conexión
        private TcpClient client = new TcpClient();
        private ASCIIEncoding encoder = new ASCIIEncoding();
        private NetworkStream stream = null; // Flujo de red
        private byte[] buffer = new byte[4096]; // Buffer inicializado
        private string message = ""; // Variable para almacenar mensajes

        public Frm_Cliente()
        {
            InitializeComponent();
        }

        // Botón para conectar al servidor
        private void Btn_Conectar_Click(object sender, EventArgs e)
        {
            Btn_Conectar.Enabled = false;

            // Validar si la IP del servidor fue ingresada
            if (Txt_Servidor.Text == "")
            {
                MessageBox.Show("Por favor, ingrese una dirección IP para el servidor.", "Error al Conectar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Btn_Conectar.Enabled = true;
                return;
            }

            // Intentar conectarse al servidor
            if (!client.Connected)
            {
                try
                {
                    client = new TcpClient();
                    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(Txt_Servidor.Text), 30000); // Puerto 30000
                    client.Connect(serverEndPoint);

                    stream = client.GetStream(); // Inicia el flujo de datos
                    buffer = encoder.GetBytes("Hello Server!"); // Mensaje de conexión inicial
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();

                    MessageBox.Show("Conexión Realizada con Éxito");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar al servidor: " + ex.Message);
                    Btn_Conectar.Enabled = true; // Reactivar el botón si falla
                }
            }
            else
            {
                MessageBox.Show("El cliente ya está conectado al servidor.");
            }
        }

        // Botón para desconectarse del servidor
        private void Btn_Desconectar_Click(object sender, EventArgs e)
        {
            Btn_Conectar.Enabled = true;

            if (client.Connected)
            {
                try
                {
                    if (stream != null)
                        stream.Close(); // Cierra el flujo de datos
                    client.Close(); // Cierra el cliente
                    MessageBox.Show("Desconexión Realizada con Éxito");
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Error al desconectar: " + exc.Message);
                }
            }
            else
            {
                MessageBox.Show("El cliente ya está desconectado.");
            }
        }

        // Botón para enviar un mensaje al servidor
        private void Bnt_Enviar_Click(object sender, EventArgs e)
        {
            if (!client.Connected)
            {
                MessageBox.Show("El cliente no está conectado al servidor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Envía el mensaje al servidor
                NetworkStream clientStream = client.GetStream();
                buffer = encoder.GetBytes(Txt_Mensaje.Text.Trim()); // Convierte el mensaje en bytes
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();

                // Recibe la respuesta del servidor
                byte[] response = new byte[4096];
                int bytesRead = clientStream.Read(response, 0, 4096);
                string serverResponse = encoder.GetString(response, 0, bytesRead);

                // Muestra la respuesta en el cuadro de texto
                Txt_Mensaje.Text = serverResponse;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar el mensaje: " + ex.Message);
            }
        }
    }
}
