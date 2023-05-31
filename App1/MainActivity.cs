using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Bluetooth;
using Java.Util;
using Android.Widget;
using System.IO;
using System.Threading.Tasks;
using Android;

namespace App1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        
        ToggleButton conectar;
        Button Up;
        
        TextView Result;
        TextView Result2;
        TextView Result3;
        TextView Result4;
        TextView Result5;
        TextView Result6;
        int progres1;
        int progres2;
        int progres3;
        int progres4;
        int progres5;
        int progres6;


        private Java.Lang.String dataToSend;
        private Stream OutStream = null;
        private Stream InStream = null;
        private BluetoothSocket bluetoothsocket = null;
        private BluetoothAdapter bluetoothadapter = null;
        private static string address = "98:DA:20:04:F1:E3";
        private static UUID myuuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            conectar = FindViewById<ToggleButton>(Resource.Id.toggleButton1);
            //Result = FindViewById<TextView>(Resource.Id.textView1);

            Result = FindViewById<TextView>(Resource.Id.textView1);
            Result2 = FindViewById<TextView>(Resource.Id.textView2);
            Result3 = FindViewById<TextView>(Resource.Id.textView3);
            Result4 = FindViewById<TextView>(Resource.Id.textView4);
            Result5 = FindViewById<TextView>(Resource.Id.textView5);
           
            var seekbar1 = FindViewById<SeekBar>(Resource.Id.seekBar1);
            var seekbar2 = FindViewById<SeekBar>(Resource.Id.seekBar2);
            var seekbar3 = FindViewById<SeekBar>(Resource.Id.seekBar3);
            var seekbar4 = FindViewById<SeekBar>(Resource.Id.seekBar4);
            var seekbar5 = FindViewById<SeekBar>(Resource.Id.seekBar5);
            
            conectar.CheckedChange += HandleCheckChange;
            


            CheckBlueTooth();

            seekbar1.StopTrackingTouch += (s, e) =>
            {
                dataToSend = new Java.Lang.String("a" + progres1 + "\n");
                SendData(dataToSend);
            };

            seekbar1.ProgressChanged += (s, e) =>
            {
                progres1 = e.Progress;
                Result.Text = string.Format("1DOF: {0}", e.Progress);
            };

            seekbar2.StopTrackingTouch += (s, e) =>
            {
                dataToSend = new Java.Lang.String("b" + progres2 + "\n");
                SendData(dataToSend);
            };

            seekbar2.ProgressChanged += (s, e) =>
            {
                progres2 = e.Progress;
                Result2.Text = string.Format("2DOF: {0}", e.Progress);
            };
            seekbar3.StopTrackingTouch += (s, e) =>
            {
                dataToSend = new Java.Lang.String("c" + progres3 + "\n");
                SendData(dataToSend);
            };

            seekbar3.ProgressChanged += (s, e) =>
            {
                progres3 = e.Progress;
                Result3.Text = string.Format("3DOF: {0}", e.Progress);
            };
            seekbar4.StopTrackingTouch += (s, e) =>
            {
                dataToSend = new Java.Lang.String("d" + progres4 + "\n");
                SendData(dataToSend);
            };

            seekbar4.ProgressChanged += (s, e) =>
            {
                progres4 = e.Progress;
                Result4.Text = string.Format("4DOF: {0}", e.Progress);
            };
            seekbar5.StopTrackingTouch += (s, e) =>
            {
                dataToSend = new Java.Lang.String("e" + progres5 + "\n");
                SendData(dataToSend);
            };

            seekbar5.ProgressChanged += (s, e) =>
            {
                progres5 = e.Progress;
                Result5.Text = string.Format("5DOF: {0}", e.Progress);
            };

            /*   Up.Click += delegate
               {
                   Console.WriteLine("Da click al boton");
                   dataToSend = new Java.Lang.String("UP\n");
                   SendData(dataToSend); 
               };*/




        }

        private void CheckBlueTooth()
        {
            bluetoothadapter = BluetoothAdapter.DefaultAdapter;
            if (!bluetoothadapter.Enable())
            {
                Toast.MakeText(this, "Bluetooth desactivado", ToastLength.Short).Show();
            }
            if (bluetoothadapter == null)
            {
                Toast.MakeText(this, "Bluetooth no disponible", ToastLength.Short).Show();
            }
        }

        public void ConnectBlueTooth()
        {
            BluetoothDevice device = bluetoothadapter.GetRemoteDevice(address);
            System.Console.WriteLine("Conexion en curso" + device);

            bluetoothadapter.CancelDiscovery();
            try
            {
                bluetoothsocket = device.CreateRfcommSocketToServiceRecord(myuuid);
                bluetoothsocket.Connect();
                System.Console.WriteLine("Conexion Correcta");
            }
            catch (System.Exception)
            {
                try
                {
                    bluetoothsocket.Close();
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("No se pudo conectar");
                }
                System.Console.WriteLine("Socket Creado");
            }

            //Una vez conectados al bluetooth mandamos llamar el metodo que generara el hilo
            //que recibira los datos del arduino
            //ReciveData();
            //NOTA envio la letra e ya que el sketch esta configurado para funcionar cuando
            //recibe esta letra.
            //dataToSend = new Java.Lang.String("U");
            //SendData(dataToSend);

        }

        public void ReciveData()
        {
            try
            {
                InStream = bluetoothsocket.InputStream;
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Error");
            }

            Task.Factory.StartNew(() =>
            {
                byte[] buffer = new byte[1024];
                int bytes;
                while (true)
                {
                    try
                    {
                        bytes = InStream.Read(buffer, 0, buffer.Length);
                        if (bytes > 0)
                        {
                            RunOnUiThread(() =>
                            {
                                string valor = System.Text.Encoding.ASCII.GetString(buffer);
                                Result.Text = Result.Text + "\n" + valor;
                            });
                        }
                    }
                    catch (System.Exception)
                    {
                        RunOnUiThread(() =>
                        {
                            Result.Text = string.Empty;
                        });
                        break;
                    }
                }
            });
        }

        private void SendData(Java.Lang.String data)
        {
            try
            {
                try
                {
                    OutStream = bluetoothsocket.OutputStream;
                    Console.WriteLine("Todo bien");
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Error");
                }

                Java.Lang.String message = data;
                byte[] messagebuffer = message.GetBytes();
                try
                {

                    OutStream.Write(messagebuffer, 0, messagebuffer.Length);
                    Console.WriteLine("Todo bien 2");

                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine("Error: " + e.Message);
                }
            }catch(System.Exception e)
            {
                Console.WriteLine("ERROR GRANDOTE: " + e.Message);
            }
        }

        void HandleCheckChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                ConnectBlueTooth();
            }
            else
            {
                Console.WriteLine("No está checado");
                if (bluetoothsocket.IsConnected)
                {
                    try
                    {
                        bluetoothsocket.Close();
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine("Error: "+ ex.Message);
                    }
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
