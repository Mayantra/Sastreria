using System;
using System.Data;
using System.IO;
using System.Windows;
using DPFP;
using DPFP.Processing;
using DPFP.Verification;
using MySql.Data.MySqlClient;

namespace LoginSasteria
{
    public class FingerprintHandler
    {
        private DPFP.Capture.Capture Capturer;
        private Verification Verificator;
        private MainWindow mainWindow;

        public FingerprintHandler(MainWindow window)
        {
            mainWindow = window;
            Capturer = new DPFP.Capture.Capture();
            Verificator = new Verification();

            if (Capturer != null)
            {
                Capturer.EventHandler = new FingerprintCaptureHandler(this);
            }
            else
            {
                MessageBox.Show("No se pudo inicializar el lector de huellas");
            }
        }

        public void Start()
        {
            if (Capturer != null)
            {
                try
                {
                    Capturer.StartCapture();
                }
                catch (DPFP.Error.SDKException ex)
                {
                    MessageBox.Show("Error al iniciar la captura: " + ex.Message);
                }
            }
        }


        public void Stop()
        {
            Capturer.StopCapture();
        }

        public void ProcessSample(DPFP.Sample sample)
        {
            var features = ExtractFeatures(sample, DataPurpose.Verification);
            if (features == null)
            {
                MessageBox.Show("No se pudo extraer características de la muestra.");
                return;
            }

            bool huellaEncontrada = false;

            try
            {
                using (var conn = new MySqlConnection($"server={ConexionDB.servidor}; database={ConexionDB.db}; uid={ConexionDB.username}; pwd={ConexionDB.password};"))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT idEmpleado, Nombre, Usuario, huella FROM Empleado", conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader["huella"] == DBNull.Value)
                            continue;

                        try
                        {
                            var templateData = (byte[])reader["huella"];
                            if (templateData.Length < 1) continue;

                            using (var ms = new MemoryStream(templateData))
                            {
                                var template = new Template(ms);

                                var result = new Verification.Result();
                                Verificator.Verify(features, template, ref result);

                                if (result.Verified)
                                {
                                    huellaEncontrada = true;

                                    string nombre = reader.GetString("Nombre");
                                    string usuario = reader.GetString("Usuario"); // ← obtenemos el campo Usuario

                                    leerPass.usuario = usuario; // ← lo asignamos a la variable estática

                                    //MessageBox.Show("Acceso concedido a: " + nombre);

                                    mainWindow.Dispatcher.Invoke(() =>
                                    {
                                        var ventana = new ventanaInicio();
                                        ventana.Show();
                                        mainWindow.Close();
                                    });
                                    break;
                                }

                            }
                        }
                        catch (Exception templateEx)
                        {
                            MessageBox.Show("Error al procesar la huella almacenada: " + templateEx.Message);
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

        private FeatureSet ExtractFeatures(DPFP.Sample sample, DPFP.Processing.DataPurpose purpose)
        {
            try
            {
                var extractor = new FeatureExtraction();
                var feedback = DPFP.Capture.CaptureFeedback.None;
                var features = new FeatureSet();

                extractor.CreateFeatureSet(sample, purpose, ref feedback, ref features);

                if (feedback == DPFP.Capture.CaptureFeedback.Good)
                    return features;
                else
                {
                    MessageBox.Show("Muestra de huella no válida.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al extraer características de la huella: " + ex.Message);
                return null;
            }
        }
    }

    public class FingerprintCaptureHandler : DPFP.Capture.EventHandler
    {
        private readonly FingerprintHandler handler;

        public FingerprintCaptureHandler(FingerprintHandler handler)
        {
            this.handler = handler;
        }

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            handler.ProcessSample(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback) { }
    }
}
