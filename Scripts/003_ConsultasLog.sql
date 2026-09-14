-- Consultas SQL para consultar el Log de Operaciones
-- Base de datos: InvoiceRitrama

-- ══════════════════════════════════════════════════════════════════════════════
-- 1. HISTORIAL GENERAL (últimas 24 horas)
-- ══════════════════════════════════════════════════════════════════════════════
SELECT 
    id,
    fecha_inicio,
    fecha_fin,
    DATEDIFF(SECOND, fecha_inicio, ISNULL(fecha_fin, GETDATE())) AS duracion_seg,
    tipo_operacion,
    descripcion,
    usuario,
    maquina,
    CASE WHEN exitoso = 1 THEN '✓' ELSE '✗' END AS estado,
    resultado
FROM operaciones_log
WHERE fecha_inicio > DATEADD(HOUR, -24, GETDATE())
ORDER BY fecha_inicio DESC;

-- ══════════════════════════════════════════════════════════════════════════════
-- 2. OPERACIONES FALLIDAS (para debugging)
-- ══════════════════════════════════════════════════════════════════════════════
SELECT 
    id,
    fecha_inicio,
    tipo_operacion,
    descripcion,
    usuario,
    maquina,
    ip_address,
    detalle_error
FROM operaciones_log
WHERE exitoso = 0
  AND fecha_inicio > DATEADD(DAY, -7, GETDATE())
ORDER BY fecha_inicio DESC;

-- ══════════════════════════════════════════════════════════════════════════════
-- 3. HISTORIAL DE UNA OC ESPECÍFICA
-- ══════════════════════════════════════════════════════════════════════════════
-- Cambiar @NumeroOC por el número de OC deseada
DECLARE @NumeroOC VARCHAR(20) = '4616';

SELECT 
    l.id,
    l.fecha_inicio,
    l.fecha_fin,
    l.tipo_operacion,
    l.descripcion,
    CASE WHEN l.exitoso = 1 THEN '✓ Exitoso' ELSE '✗ Fallido' END AS estado,
    l.resultado,
    l.detalle_error
FROM operaciones_log l
WHERE l.descripcion LIKE '%' + @NumeroOC + '%'
ORDER BY l.fecha_inicio DESC;

-- Detalle de operaciones de la OC
SELECT 
    d.fecha,
    d.accion,
    d.entidad,
    d.valor_anterior,
    d.valor_nuevo,
    d.notas
FROM operaciones_log_detalle d
INNER JOIN operaciones_log l ON l.id = d.operacion_id
WHERE l.descripcion LIKE '%' + @NumeroOC + '%'
ORDER BY l.fecha_inicio DESC, d.fecha;

-- ══════════════════════════════════════════════════════════════════════════════
-- 4. RESUMEN POR TIPO DE OPERACIÓN (hoy)
-- ══════════════════════════════════════════════════════════════════════════════
SELECT 
    tipo_operacion,
    COUNT(*) AS total,
    SUM(CASE WHEN exitoso = 1 THEN 1 ELSE 0 END) AS exitosas,
    SUM(CASE WHEN exitoso = 0 THEN 1 ELSE 0 END) AS fallidas,
    AVG(DATEDIFF(SECOND, fecha_inicio, ISNULL(fecha_fin, GETDATE()))) AS promedio_segundos
FROM operaciones_log
WHERE fecha_inicio > CAST(GETDATE() AS DATE)
GROUP BY tipo_operacion
ORDER BY total DESC;

-- ══════════════════════════════════════════════════════════════════════════════
-- 5. OPERACIONES POR USUARIO (hoy)
-- ══════════════════════════════════════════════════════════════════════════════
SELECT 
    usuario,
    COUNT(*) AS total_operaciones,
    SUM(CASE WHEN exitoso = 1 THEN 1 ELSE 0 END) AS exitosas,
    MIN(fecha_inicio) AS primera_operacion,
    MAX(fecha_inicio) AS ultima_operacion
FROM operaciones_log
WHERE fecha_inicio > CAST(GETDATE() AS DATE)
GROUP BY usuario
ORDER BY total_operaciones DESC;

-- ══════════════════════════════════════════════════════════════════════════════
-- 6. DETALLE COMPLETO DE UNA OPERACIÓN (para auditoría)
-- ══════════════════════════════════════════════════════════════════════════════
DECLARE @OperacionID BIGINT = 1; -- Cambiar por el ID de operación

-- Cabecera
SELECT 
    '═══════════════════════════════════════════════════════════════' AS separador,
    'OPERACIÓN #' + CAST(id AS VARCHAR) AS titulo,
    'Fecha Inicio: ' + CONVERT(VARCHAR, fecha_inicio, 121) AS inicio,
    'Fecha Fin: ' + ISNULL(CONVERT(VARCHAR, fecha_fin, 121), 'En proceso...') AS fin,
    'Tipo: ' + tipo_operacion AS tipo,
    'Descripción: ' + descripcion AS descripcion,
    'Usuario: ' + ISNULL(usuario, 'Sistema') AS usuario,
    'Máquina: ' + maquina AS maquina,
    'IP: ' + ip_address AS ip,
    'Estado: ' + CASE WHEN exitoso = 1 THEN '✓ EXITOSO' ELSE '✗ FALLIDO' END AS estado,
    'Resultado: ' + resultado AS resultado,
    'Error: ' + ISNULL(detalle_error, 'N/A') AS error
FROM operaciones_log
WHERE id = @OperacionID;

-- Detalles
SELECT 
    '[' + CONVERT(VARCHAR, fecha, 121) + '] ' + accion AS accion,
    'Entidad: ' + entidad AS entidad,
    'Anterior: ' + ISNULL(valor_anterior, 'N/A') AS anterior,
    'Nuevo: ' + ISNULL(valor_nuevo, 'N/A') AS nuevo,
    'Notas: ' + notas AS notas
FROM operaciones_log_detalle
WHERE operacion_id = @OperacionID
ORDER BY fecha;

-- ══════════════════════════════════════════════════════════════════════════════
-- 7. CONSULTA RÁPIDA: ¿QUÉ PASÓ CON ESTA OC?
-- ══════════════════════════════════════════════════════════════════════════════
-- Cambiar el número de OC
SELECT 
    CONVERT(VARCHAR, l.fecha_inicio, 121) AS cuando,
    l.tipo_operacion AS operacion,
    CASE WHEN l.exitoso = 1 THEN '✓' ELSE '✗' END AS estado,
    l.usuario AS quien,
    l.descripcion
FROM operaciones_log l
WHERE l.descripcion LIKE '%4616%'  -- ← Cambiar número de OC
ORDER BY l.fecha_inicio;

-- ══════════════════════════════════════════════════════════════════════════════
-- 8. TIMELINE DE UNA OC (orden cronológico)
-- ══════════════════════════════════════════════════════════════════════════════
DECLARE @OcNumero VARCHAR(20) = '4616';

SELECT 
    ROW_NUMBER() OVER (ORDER BY l.fecha_inicio) AS paso,
    CONVERT(VARCHAR, l.fecha_inicio, 108) AS hora,
    l.tipo_operacion AS accion,
    CASE WHEN l.exitoso = 1 THEN '✓' ELSE '✗' END AS estado,
    ISNULL(
        (SELECT STRING_AGG(d.accion + ': ' + ISNULL(d.valor_nuevo, ''), ' | ')
         FROM operaciones_log_detalle d 
         WHERE d.operacion_id = l.id),
        l.resultado
    ) AS detalle
FROM operaciones_log l
WHERE l.descripcion LIKE '%' + @OcNumero + '%'
ORDER BY l.fecha_inicio;

-- ══════════════════════════════════════════════════════════════════════════════
-- 9. LIMPIEZA DE LOGS ANTIGUOS
-- ══════════════════════════════════════════════════════════════════════════════
-- Eliminar logs mayores a 90 días (ejecutar con cuidado)
-- EXEC sp_LimpiarLogsAntiguos @DiasRetencion = 90;

-- Ver cuántos registros se eliminarían
SELECT 
    COUNT(*) AS registros_a_eliminar,
    MIN(fecha_inicio) AS fecha_mas_antigua,
    MAX(fecha_inicio) AS fecha_mas_reciente
FROM operaciones_log
WHERE fecha_inicio < DATEADD(DAY, -90, GETDATE());
