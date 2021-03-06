﻿namespace net.derpaul.tf
{
    /// <summary>
    /// Class to read temperature using barometer sensor
    /// </summary>
    public class Temperature : Barometer
    {
        /// <summary>
        /// Measurement unit of sensor
        /// </summary>
        public override string Unit { get; } = "C";

        /// <summary>
        /// Read value from sensor and prepare real value
        /// </summary>
        /// <returns>Air pressure or 0.0</returns>
        protected override MeasurementValue ValueGetRaw()
        {
            MeasurementValue result = new MeasurementValue(Name, Unit, TemperatureConfig.Instance.SortOrder);

            if (_Bricklet == null)
            {
                return result;
            }

            int temperatureRaw = _Bricklet.GetChipTemperature();
            result.Value = temperatureRaw / 100.0;

            return result;
        }
    }
}