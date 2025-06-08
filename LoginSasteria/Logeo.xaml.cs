using System;
using System.Data;
using System.IO;
using System.Windows;
using DPFP;
using DPFP.Capture;
using DPFP.Processing;
using DPFP.Verification;
using MySql.Data.MySqlClient;

namespace LoginSasteria
{
    public partial class Logeo : Window, DPFP.Capture.EventHandler
    {
        ProcesoVenta venta = new ProcesoVenta();
        leerPass read = new leerPass();

        private DPFP.Capture.Capture Capturador;
        private Verification Verificador;

        private bool accesoPorHuella = false; // ✅ Flag para saber si el acceso fue por huella

        public Logeo()
        {
            InitializeComponent();
            IniciarLector();

            this.Closed += (s, e) =>
            {
                try { Capturador?.StopCapture(); } catch { }
            };
        }

        private void IniciarLector()
        {
            try
            {
                Capturador = new DPFP.Capture.Capture();
                Verificador = new Verification();

                if (Capturador != null)
                {
                    Capturador.EventHandler = this;
                    Capturador.StartCapture();
                }
                else
                {
                    MessageBox.Show("No se pudo inicializar el lector de huellas.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar lector: " + ex.Message);
            }
        }

        // =============== EVENTOS DE CAPTURA DE HUELLAS ===============

        public void OnComplete(object Capture, string ReaderSerialNumber, Sample sample)
        {
            Dispatcher.Invoke(() => ProcesarHuella(sample));
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }
        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback feedback) { }

        private void ProcesarHuella(Sample sample)
        {
            var features = ExtraerCaracteristicas(sample, DataPurpose.Verification);
            if (features == null) return;

            bool huellaEncontrada = false;

            try
            {
                using (var conn = new MySqlConnection($"server={ConexionDB.servidor}; database={ConexionDB.db}; uid={ConexionDB.username}; pwd={ConexionDB.password};"))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT idEmpleado, Nombre, Usuario, pin, huella FROM Empleado", conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader["huella"] == DBNull.Value) continue;

                        var templateData = (byte[])reader["huella"];
                        if (templateData.Length < 1) continue;

                        using (var ms = new MemoryStream(templateData))
                        {
                            var template = new Template(ms);
                            var result = new Verification.Result();
                            Verificador.Verify(features, template, ref result);

                            if (result.Verified)
                            {
                                huellaEncontrada = true;

                                string usuario = reader.GetString("Usuario");
                                // string pin = reader.GetString("pin"); ← ya no se asigna para evitar doble acceso

                                accesoPorHuella = true; // ✅ Activar flag

                                txUser.Text = usuario;
                                // No asignamos PassBox.Password

                                Capturador?.StopCapture(); // ✋ Detener lectura

                                acceso(); // 👈 Ejecutamos autenticación

                                break;
                            }
                        }
                    }

                    if (!huellaEncontrada)
                    {
                        MessageBox.Show("No se encontró una huella coincidente.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar huella: " + ex.Message);
            }
        }

        private FeatureSet ExtraerCaracteristicas(Sample sample, DataPurpose purpose)
        {
            try
            {
                var extractor = new FeatureExtraction();
                var feedback = CaptureFeedback.None;
                var features = new FeatureSet();

                extractor.CreateFeatureSet(sample, purpose, ref feedback, ref features);

                return feedback == CaptureFeedback.Good ? features : null;
            }
            catch
            {
                return null;
            }
        }

        // =============== ACCESO NORMAL POR PIN O HUELLA ===============

        void acceso()
        {
            string user = txUser.Text;
            string pass = PassBox.Password;

            if (accesoPorHuella || read.getAcceso(user, pass))
            {
                int aux = read.getIdLogUser(user);
                venta.setVendedor(aux);
                venta.setLog(true);
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o PIN incorrectos.");
                txUser.Text = "";
                PassBox.Password = "";
            }

            accesoPorHuella = false; // ✅ Resetear flag para futuros accesos
        }

        private void lecturapass(object sender, RoutedEventArgs e)
        {
            acceso();
        }

        private void CanelarLog(object sender, RoutedEventArgs e)
        {
            Capturador?.StopCapture();
            venta.Show();
            this.Close();
        }
    }
}
