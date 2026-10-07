-- Backfill: recalcular el material restante archivado de las OC (REGLA RN-RESTANTE-OC).
--
-- Regla de negocio: el restante de un master en la OC nunca es 0 (ni un valor ajeno) si el
-- master tiene largo y la OC registra consumo. restante = largo_del_master - consumo_real_de_la_OC.
--   master 1: rest1_lenght = lenght_1 - util1_real_lenght  (y rest1_width = width_1 - util1_real_width)
--   master 2: rest2_lenght = lenght_2 - util2_real_lenght  (y rest2_width = width_2 - util2_real_width)
-- Si la OC es desperdicio, todo el master se consume como desperdicio -> restante 0
-- (igual que el libro contable de consumos). Tambien recalcula las columnas de texto
-- restante_rollid1/2 (formato decimal con coma, ej. '115,00').
-- Idempotente: solo actualiza cuando el valor archivado difiere del recalculo.
--
-- Ejecutar: sqlcmd -S 192.168.10.10 -d RITRAMASQL2017 -U <user> -P <pwd> -C -i Scripts\Backfill_Restante_OC.sql

-- Requerido: actualiza orden_corte que tiene indice filtrado (sqlcmd trae QUOTED_IDENTIFIER OFF).
SET QUOTED_IDENTIFIER ON;
GO

-- 1) PREVIEW: OC cuyo restante archivado difiere del valor real (largo - consumo).
SELECT numero, rollid_1, lenght_1, util1_real_lenght, desperdicio,
       rest1_lenght AS rest1_actual,
       CASE WHEN desperdicio = 1 THEN 0 ELSE ROUND(ISNULL(lenght_1, 0) - ISNULL(util1_real_lenght, 0), 2) END AS rest1_correcto,
       restante_rollid1 AS restante1_actual,
       REPLACE(CONVERT(varchar(20), CAST(CASE WHEN desperdicio = 1 THEN 0 ELSE ROUND(ISNULL(lenght_1, 0) - ISNULL(util1_real_lenght, 0), 2) END AS decimal(20, 2))), '.', ',') AS restante1_correcto,
       rollid_2, lenght_2, util2_real_lenght, desperdicio2,
       rest2_lenght AS rest2_actual,
       CASE WHEN desperdicio2 = 1 THEN 0 ELSE ROUND(ISNULL(lenght_2, 0) - ISNULL(util2_real_lenght, 0), 2) END AS rest2_correcto,
       restante_rollid2 AS restante2_actual,
       REPLACE(CONVERT(varchar(20), CAST(CASE WHEN desperdicio2 = 1 THEN 0 ELSE ROUND(ISNULL(lenght_2, 0) - ISNULL(util2_real_lenght, 0), 2) END AS decimal(20, 2))), '.', ',') AS restante2_correcto,
       step, anulada, CloseDocument
FROM orden_corte
CROSS APPLY (SELECT CASE WHEN desperdicio = 1 THEN 0 ELSE ROUND(ISNULL(lenght_1, 0) - ISNULL(util1_real_lenght, 0), 2) END AS e1,
                    CASE WHEN desperdicio2 = 1 THEN 0 ELSE ROUND(ISNULL(lenght_2, 0) - ISNULL(util2_real_lenght, 0), 2) END AS e2) calc
WHERE ISNULL(rest1_lenght, 0) != e1
   OR ISNULL(rest2_lenght, 0) != e2
   OR ISNULL(restante_rollid1, '') != REPLACE(CONVERT(varchar(20), CAST(e1 AS decimal(20, 2))), '.', ',')
   OR ISNULL(restante_rollid2, '') != REPLACE(CONVERT(varchar(20), CAST(e2 AS decimal(20, 2))), '.', ',')
ORDER BY numero;

-- 2) CORRECCION: recalcular los restantes archivados (numericos + columnas de texto).
UPDATE orden_corte
SET rest1_width  = ROUND(ISNULL(width_1,  0) - ISNULL(util1_real_width,  0), 2),
    rest1_lenght = CASE WHEN desperdicio = 1 THEN 0 ELSE ROUND(ISNULL(lenght_1,  0) - ISNULL(util1_real_lenght, 0), 2) END,
    rest2_width  = ROUND(ISNULL(width_2,  0) - ISNULL(util2_real_width,  0), 2),
    rest2_lenght = CASE WHEN desperdicio2 = 1 THEN 0 ELSE ROUND(ISNULL(lenght_2,  0) - ISNULL(util2_real_lenght, 0), 2) END,
    restante_rollid1 = REPLACE(CONVERT(varchar(20), CAST(CASE WHEN desperdicio = 1 THEN 0 ELSE ROUND(ISNULL(lenght_1,  0) - ISNULL(util1_real_lenght, 0), 2) END AS decimal(20, 2))), '.', ','),
    restante_rollid2 = REPLACE(CONVERT(varchar(20), CAST(CASE WHEN desperdicio2 = 1 THEN 0 ELSE ROUND(ISNULL(lenght_2,  0) - ISNULL(util2_real_lenght, 0), 2) END AS decimal(20, 2))), '.', ',')
WHERE ISNULL(rest1_lenght, 0) != CASE WHEN desperdicio = 1 THEN 0 ELSE ROUND(ISNULL(lenght_1,  0) - ISNULL(util1_real_lenght, 0), 2) END
   OR ISNULL(rest2_lenght, 0) != CASE WHEN desperdicio2 = 1 THEN 0 ELSE ROUND(ISNULL(lenght_2,  0) - ISNULL(util2_real_lenght, 0), 2) END
   OR ISNULL(restante_rollid1, '') != REPLACE(CONVERT(varchar(20), CAST(CASE WHEN desperdicio = 1 THEN 0 ELSE ROUND(ISNULL(lenght_1,  0) - ISNULL(util1_real_lenght, 0), 2) END AS decimal(20, 2))), '.', ',')
   OR ISNULL(restante_rollid2, '') != REPLACE(CONVERT(varchar(20), CAST(CASE WHEN desperdicio2 = 1 THEN 0 ELSE ROUND(ISNULL(lenght_2,  0) - ISNULL(util2_real_lenght, 0), 2) END AS decimal(20, 2))), '.', ',');

-- 3) VERIFICAR: deben quedar cero filas (restante archivado = largo - consumo en todas las OC).
SELECT numero, rollid_1, lenght_1, util1_real_lenght, rest1_lenght, restante_rollid1,
       rollid_2, lenght_2, util2_real_lenght, rest2_lenght, restante_rollid2
FROM orden_corte
CROSS APPLY (SELECT CASE WHEN desperdicio = 1 THEN 0 ELSE ROUND(ISNULL(lenght_1, 0) - ISNULL(util1_real_lenght, 0), 2) END AS e1,
                    CASE WHEN desperdicio2 = 1 THEN 0 ELSE ROUND(ISNULL(lenght_2, 0) - ISNULL(util2_real_lenght, 0), 2) END AS e2) calc
WHERE ISNULL(rest1_lenght, 0) != e1
   OR ISNULL(rest2_lenght, 0) != e2
   OR ISNULL(restante_rollid1, '') != REPLACE(CONVERT(varchar(20), CAST(e1 AS decimal(20, 2))), '.', ',')
   OR ISNULL(restante_rollid2, '') != REPLACE(CONVERT(varchar(20), CAST(e2 AS decimal(20, 2))), '.', ',');

-- 4) CASOS PARTICULARES: confirmar el dato antes/despues (OC 4625 y OC 4616).
SELECT numero, rollid_1, lenght_1, util1_real_lenght, rest1_lenght, restante_rollid1
FROM orden_corte WHERE numero IN (4616, 4625);