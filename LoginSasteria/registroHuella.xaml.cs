using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DPFP;
using DPFP.Capture;
using DPFP.Processing;
using MySql.Data.MySqlClient;

namespace LoginSasteria
{
    public partial class registroHuella : Window, DPFP.Capture.EventHandler
    {
        private Capture Capturador;
        private Enrollment Enroller;
        private Template TemplateHuella;

        public registroHuella()
        {
            InitializeComponent();
            IniciarCaptura();

            // 🛑 Detener captura cuando se cierre la ventana
            this.Closed += (s, e) =>
            {
                if (Capturador != null)
                {
                    try
                    {
                        Capturador.StopCapture();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al detener la captura: " + ex.Message);
                    }
                }
            };
        }

        private void btnSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void IniciarCaptura()
        {
            try
            {
                Capturador = new Capture();
                Enroller = new Enrollment();

                if (Capturador != null)
                {
                    Capturador.EventHandler = this;
                    Capturador.StartCapture();
                }
                else
                {
                    MessageBox.Show("No se pudo iniciar el lector de huellas.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar captura: " + ex.Message);
            }
        }

        public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
        {
            ProcesarHuella(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }
        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback CaptureFeedback) { }

        private void ProcesarHuella(Sample sample)
        {
            var features = ExtraerCaracteristicas(sample, DataPurpose.Enrollment);

            if (features != null)
            {
                Enroller.AddFeatures(features);

                if (Enroller.TemplateStatus == Enrollment.Status.Ready)
                {
                    TemplateHuella = Enroller.Template;

                    Capturador?.StopCapture();
                    MessageBox.Show("Huella registrada correctamente. Puede proceder a guardar.");
                }
                else if (Enroller.TemplateStatus == Enrollment.Status.Failed)
                {
                    Enroller.Clear();
                    Capturador?.StopCapture();
                    IniciarCaptura(); // Reintentar
                    MessageBox.Show("Falló el registro de huella. Intente nuevamente.");
                }
            }
            else
            {
                MessageBox.Show("Muestra de huella no válida. Intente nuevamente.");
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

                if (feedback == CaptureFeedback.Good)
                {
                    return features;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al extraer características de la huella: " + ex.Message);
                return null;
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (TemplateHuella == null)
            {
                MessageBox.Show("No hay huella registrada.");
                return;
            }

            byte[] huellaBytes;
            using (var ms = new MemoryStream())
            {
                TemplateHuella.Serialize(ms);
                huellaBytes = ms.ToArray();
            }

            if (huellaBytes.Length < 100)
            {
                MessageBox.Show("La huella capturada es demasiado pequeña o inválida.");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection("server=94.130.216.164;database=hismanreina_PruebasDBLeonV2;user=hismanreina_isa;password=Isaac@17Isaac@17;"))
                {
                    conn.Open();

                    var cmd = new MySqlCommand("INSERT INTO Empleado (Nombre, Usuario, pin, huella, superUser, almacen_idalmacen) VALUES (@nombre, @usuario, @pin, @huella, @super, @almacen)", conn);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@usuario", txtUsuario.Text);
                    cmd.Parameters.AddWithValue("@pin", txtPin.Text);
                    cmd.Parameters.AddWithValue("@huella", huellaBytes);
                    cmd.Parameters.AddWithValue("@super", chkSuper.IsChecked == true ? 1 : 0);

                    if (cmbAlmacen.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Tag?.ToString(), out int almacenId))
                    {
                        cmd.Parameters.AddWithValue("@almacen", almacenId);
                    }
                    else
                    {
                        MessageBox.Show("Por favor seleccione un almacén válido.");
                        return;
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Empleado registrado correctamente.");
                    MainWindow abrir = new MainWindow();
                    abrir.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en base de datos: " + ex.Message);
            }
        }

        private void btCancelar(object sender, RoutedEventArgs e)
        {
            ventanaInicio abrir = new ventanaInicio();
            abrir.Show();
            this.Close();
        }
    }
}
