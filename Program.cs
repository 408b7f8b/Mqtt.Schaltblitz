using System;

namespace Mqtt.Schaltblitz
{
    public class Konfiguration
    {
        public String Broker;
        public int Port;

        public string ClientId;

        public String topic;

        [Newtonsoft.Json.JsonProperty("Grenze")]
        public System.Collections.Generic.List<double> Grenze;

        [Newtonsoft.Json.JsonProperty("Farbe")]
        public System.Collections.Generic.List<System.Drawing.Color> Farbe;

        public int ledCount;
        public int Pin;

        public ws281x.Net.Neopixel neopixel;
    }

    class Program
    {
        public static Konfiguration k;
        public static System.Drawing.Color letzteFarbe;

        static void Main(string[] args)
        {
            string s;

            try
            {
                s = System.IO.File.ReadAllText("konfig.json");
            }
            catch
            {
                Console.WriteLine("konfig.json fehlt");
                return;
            }

            k = Newtonsoft.Json.JsonConvert.DeserializeObject<Konfiguration>(s);

            k.neopixel = new ws281x.Net.Neopixel(k.ledCount, k.Pin, rpi_ws281x.WS2811_STRIP_GRB);
            k.neopixel.Begin();

            System.Threading.Thread.Sleep(500);

            SetzeFarbe(System.Drawing.Color.Black);

            OpenNETCF.MQTT.MQTTClient client = new OpenNETCF.MQTT.MQTTClient(k.Broker, k.Port);
            client.MessageReceived += Client_MessageReceived;
            client.Subscriptions.Add(k.topic, OpenNETCF.MQTT.QoS.FireAndForget);
            client.ConnectAsync(k.ClientId);

            while (true)
            {

            }
        }

        private static void SetzeFarbe(System.Drawing.Color farbe)
        {
            for (int j = 0; j < k.ledCount; ++j) k.neopixel.SetPixelColor(j, farbe);
            letzteFarbe = farbe;

            k.neopixel.Show();
        }

        private static void Client_MessageReceived(string topic, OpenNETCF.MQTT.QoS qos, byte[] payload)
        {
            try
            {
                String umgewandelt = System.Text.Encoding.UTF8.GetString(payload);
                String wertString = umgewandelt.Split(new char[] { ',', '.' })[0];

                double wert = double.Parse(wertString);

                System.Drawing.Color Farbe = System.Drawing.Color.Black;

                for (int i = k.Grenze.Count - 1; i >= 0; --i)
                {
                    if (k.Grenze[i] <= wert)
                    {
                        Farbe = k.Farbe[i];
                        break;
                    }
                }

                if (Farbe != letzteFarbe) SetzeFarbe(Farbe);
            }
            catch
            {

            }
        }
    }
}
