-- Backfill: completar los campos pendientes de la tabla pedido para las pruebas de UI.
--
-- Estado real detectado en RITRAMASQL2017 (2026-09):
--   * vendor_id          ... NULL en los 20 pedidos (falta el vendedor asignado).
--   * direccion_entrega  ... vacia en 2 pedidos (7 y 9). Se deriva del cliente
--                            (COALESCE(customer_address, Customer_Dir, customer_zone)).
--   * notas              ... vacia en 8 pedidos. Se rellena con un placeholder.
--   * customer_id / customer_name ... ya estan completos (no se tocan).
--
-- Asignacion de vendor_id: DETERMINISTICA e IDEMPOTENTE. Se reparten los 9
-- vendedores activos (ORDER BY vendor_name) entre los clientes que aparecen en
-- pedido (ORDER BY customer_name) en ciclos de 9. El mismo cliente siempre cae
-- en el mismo vendedor, y nunca se sobrescribe un vendor ya asignado.
-- Solo actualiza filas con el campo en NULL/vacio (nunca pisa datos existentes).
--
-- Ejecutar: sqlcmd -S 192.168.10.10 -d RITRAMASQL2017 -U <user> -P <pwd> -C -i Scripts\Backfill_Pedidos_Completos.sql

-- 1) PREVIEW: pedidos con vendor_id vacio y el vendedor que recibiran.
SELECT p.numero,
       c.customer_name,
       v.vendor_name AS vendor_asignado,
       CASE
           WHEN p.direccion_entrega IS NULL OR LTRIM(RTRIM(p.direccion_entrega)) = ''
               THEN LTRIM(RTRIM(COALESCE(c.customer_address, c.Customer_Dir, c.customer_zone, 'Sin especificar')))
           ELSE p.direccion_entrega
       END AS direccion_resultante,
       CASE
           WHEN p.notas IS NULL OR LTRIM(RTRIM(p.notas)) = '' THEN 'Sin notas'
           ELSE p.notas
       END AS notas_resultante
FROM pedido p
JOIN customer c ON c.customer_id = p.customer_id
LEFT JOIN (
    SELECT vendor_id, vendor_name, ROW_NUMBER() OVER (ORDER BY vendor_name) AS rn FROM vendedor WHERE anulado = 0
) v ON v.rn = ((SELECT rn_c FROM (
                SELECT customer_id, ROW_NUMBER() OVER (ORDER BY customer_name) AS rn_c
                FROM customer WHERE customer_id IN (SELECT customer_id FROM pedido)
           ) cu WHERE cu.customer_id = c.customer_id) - 1) % (SELECT COUNT(*) FROM vendedor WHERE anulado = 0) + 1
WHERE p.vendor_id IS NULL
ORDER BY p.numero;

-- 2) CORRECCION: vendor_id (solo donde esta NULL).
;WITH Vend AS (
    SELECT vendor_id, ROW_NUMBER() OVER (ORDER BY vendor_name) AS rn
    FROM vendedor WHERE anulado = 0
),
Cust AS (
    SELECT customer_id, ROW_NUMBER() OVER (ORDER BY customer_name) AS rn
    FROM customer WHERE customer_id IN (SELECT customer_id FROM pedido)
)
UPDATE p
SET vendor_id = v.vendor_id
FROM pedido p
JOIN Cust c ON c.customer_id = p.customer_id
JOIN Vend v ON v.rn = ((c.rn - 1) % (SELECT COUNT(*) FROM Vend)) + 1
WHERE p.vendor_id IS NULL;

-- 3) CORRECCION: direccion_entrega desde el cliente (solo donde esta vacia).
UPDATE p
SET direccion_entrega = LTRIM(RTRIM(COALESCE(c.customer_address, c.Customer_Dir, c.customer_zone, 'Sin especificar')))
FROM pedido p
JOIN customer c ON c.customer_id = p.customer_id
WHERE p.direccion_entrega IS NULL OR LTRIM(RTRIM(p.direccion_entrega)) = '';

-- 4) CORRECCION: notas placeholder (solo donde esta vacia).
UPDATE pedido
SET notas = 'Sin notas'
WHERE notas IS NULL OR LTRIM(RTRIM(notas)) = '';

-- 5) VERIFY: deben quedar cero filas en los tres casos.
SELECT numero FROM pedido WHERE vendor_id IS NULL;
SELECT numero FROM pedido WHERE direccion_entrega IS NULL OR LTRIM(RTRIM(direccion_entrega)) = '';
SELECT numero FROM pedido WHERE notas IS NULL OR LTRIM(RTRIM(notas)) = '';