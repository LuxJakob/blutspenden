CREATE OR REPLACE VIEW vw_vital_stats_averages AS
SELECT 
    donation_date,
    ROUND(AVG(weight_kg::numeric) OVER (), 2) AS average_weight_kg,
    ROUND(AVG(amount_donated_ml::numeric) OVER (), 0) AS average_amount_donated_ml,
    ROUND(AVG(pulse::numeric) OVER (), 0) AS average_pulse,
    ROUND(AVG(temperature::numeric) OVER (), 2) AS average_temperature,
    ROUND(AVG(blood_pressure_lower::numeric) OVER (), 0) AS average_blood_pressure_lower,
    ROUND(AVG(blood_pressure_upper::numeric) OVER (), 0) AS average_blood_pressure_upper,
    ROUND(AVG(hemoglobin::numeric) OVER (), 2) AS average_hemoglobin
FROM vital_stats_vb;