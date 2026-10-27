SELECT
    p.full_name AS пациент,
    d.full_name AS врач,
    s.name AS специальность,
    a.appointment_date AS дата,
    a.appointment_time AS время,
    a.status AS статус
FROM appointments a
JOIN patients p ON a.patient_id = p.id
JOIN doctors d ON a.doctor_id = d.id
JOIN specialization s ON d.specialization_id = s.id
ORDER BY a.appointment_date, a.appointment_time
LIMIT 10;