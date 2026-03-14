CREATE OR REPLACE VIEW vw_vital_stats_thresholds AS
SELECT 
    donation_date,
    -- Hardcoded thresholds for Grafana visualization
    100 AS pulse_max,
    60 AS pulse_min,
    37.5 AS temperature_max,
    36.1 AS temperature_min,
    80 AS blood_pressure_lower_max,
    60 AS blood_pressure_lower_min,
    120 AS blood_pressure_upper_max,
    90 AS blood_pressure_upper_min,
    17.5 AS hemoglobin_max,
    13.5 AS hemoglobin_min
FROM vital_stats_vb;