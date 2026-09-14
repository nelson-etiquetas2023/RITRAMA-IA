-- ============================================================================
-- BACKFILL total_salida EN ORDEN_CORTE (Ritrama2025)
-- Correccion historica: el INSERT guardaba total_salida en 0 por desalineacion
-- de parametros (@p51 hardcodeado). Se recalcula desde la suma de cortes.width
-- (que es la fuente de verdad del cuadre a lo ancho).
-- Ejecutar en la base de datos correspondiente (dev y/o produccion).
-- Es un script one-time; se puede correr las veces que se quiera (idempotente):
-- solo actualiza filas cuyo total_salida difiera de la suma de cortes.
-- ============================================================================

-- 1) PREVIEW: ordenes con total_salida desalineado respecto a la suma de cortes
SELECT oc.numero,
       oc.total_salida                                       AS total_salida_actual,
       ROUND(SUM(c.width), 2)                                AS suma_cortes,
       oc.width_1                                            AS ancho_master,
       CASE WHEN ROUND(SUM(c.width), 2) > oc.width_1 + 0.01 THEN 'INCORRECTO: excede ancho del master' ELSE 'OK' END AS revision
FROM orden_corte oc
JOIN cortes c ON c.orden = oc.numero
GROUP BY oc.numero, oc.total_salida, oc.width_1
HAVING ISNULL(oc.total_salida, 0) = 0
    OR ABS(ISNULL(oc.total_salida, 0) - ROUND(SUM(c.width), 2)) > 0.01
ORDER BY oc.numero;

-- 2) APLICAR: recalcula total_salida desde la suma de anchos de los cortes.
--    Solo toca ordenes que tengan cortes definidos y valor desalineado.
UPDATE oc
SET total_salida = x.suma_cortes
FROM orden_corte oc
JOIN (
    SELECT c.orden, ROUND(SUM(c.width), 2) AS suma_cortes
    FROM cortes c
    GROUP BY c.orden
) x ON x.orden = oc.numero
WHERE ISNULL(oc.total_salida, 0) = 0
   OR ABS(ISNULL(oc.total_salida, 0) - x.suma_cortes) > 0.01;

-- 3) VERIFICAR: ya no debe quedar ninguna orden abierta con total_salida desalineado
SELECT oc.numero,
       oc.total_salida AS total_salida_actual,
       ROUND(SUM(c.width), 2) AS suma_cortes
FROM orden_corte oc
JOIN cortes c ON c.orden = oc.numero
GROUP BY oc.numero, oc.total_salida
HAVING ISNULL(oc.total_salida, 0) = 0
    OR ABS(ISNULL(oc.total_salida, 0) - ROUND(SUM(c.width), 2)) > 0.01
ORDER BY oc.numero;