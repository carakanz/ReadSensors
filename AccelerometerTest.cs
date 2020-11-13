using System;
using System.Numerics;
using Xamarin.Essentials;

namespace ReadSensors
{
    public class AccelerometerTest
    {
        // Set speed delay for monitoring changes.
        const SensorSpeed speed = SensorSpeed.Fastest;
        public Vector3 Data { get; private set; }

        public AccelerometerTest()
        {
            // Register for reading changes, be sure to unsubscribe when finished
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }

        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            Data = e.Reading.Acceleration;
        }

        public Exception Toggle()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
                else
                    Accelerometer.Start(speed);
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
