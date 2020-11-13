using System;
using System.Numerics;
using Xamarin.Essentials;

namespace ReadSensors
{
    class MagnetometerTest
    {
        // Set speed delay for monitoring changes.
        const SensorSpeed speed = SensorSpeed.Fastest;
        public Vector3 Data { get; private set; }

        public MagnetometerTest()
        {
            // Register for reading changes, be sure to unsubscribe when finished
            Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
        }

        void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            Data = e.Reading.MagneticField;
        }

        public Exception Toggle()
        {
            try
            {
                if (Magnetometer.IsMonitoring)
                    Magnetometer.Stop();
                else
                    Magnetometer.Start(speed);
                return null;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                return fnsEx;
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                return ex;
            }
        }
    }
}