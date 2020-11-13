using System;
using System.Numerics;
using Xamarin.Essentials;

namespace ReadSensors
{
    class GyroscopeTest
    {
        // Set speed delay for monitoring changes.
        const SensorSpeed speed = SensorSpeed.Fastest;
        public Vector3 Data { get; private set; }

        public GyroscopeTest()
        {
            // Register for reading changes, be sure to unsubscribe when finished
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
        }

        void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            Data = e.Reading.AngularVelocity;
        }

        public Exception Toggle()
        {
            try
            {
                if (Gyroscope.IsMonitoring)
                    Gyroscope.Stop();
                else
                    Gyroscope.Start(speed);
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