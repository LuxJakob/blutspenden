CREATE OR REPLACE VIEW vw_yearly_donation_totals AS
WITH current_year_stats AS (
    SELECT 
        COUNT(CASE WHEN donation_type = 'blood' THEN 1 END) AS blood_donations,
        COUNT(CASE WHEN donation_type = 'plasma' THEN 1 END) AS plasma_donations,
        COALESCE(SUM(amount_donated_ml), 0) / 1000.0 AS total_litres
    FROM vital_stats_vb
    -- The optimized date check: greater than Jan 1st of the current year
    WHERE donation_date::timestamp >= date_trunc('year', CURRENT_DATE)
      AND donation_date::timestamp < date_trunc('year', CURRENT_DATE + INTERVAL '1 year')
)
SELECT 
    (blood_donations + plasma_donations) AS donation_times,
    blood_donations,
    (6 - blood_donations) AS max_blood_donations,
    plasma_donations,
    (60 - blood_donations - plasma_donations) AS max_plasma_donations,
    ROUND(total_litres::numeric, 2) AS total_litres
FROM current_year_stats;