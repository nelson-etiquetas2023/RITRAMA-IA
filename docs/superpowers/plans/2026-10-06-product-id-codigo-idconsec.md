# Plan: inversión de convención Product_ID ↔ IdConsec

**Objetivo:** `Product_ID` vuelve a ser el código del producto (como espera toda la app);
el consecutivo del sistema pasa a la columna nueva `IdConsec`. Así el importador de
iniciales (`MasterInic.part_number = producto.product_id`) vuelve a casar.

Aprobado por el usuario: 2026-10-06.

## Pasos

- [x] 1. BD `ritrama2026`: añadir `IdConsec INT NOT NULL` (único), poblarlo con el
      `product_id` actual (1..5), pasar `Product_ID ← codigo_ritrama`, eliminar columna
      `codigo_ritrama` + índice `UX_producto_codigo_ritrama`, semillero `control.PROD` al
      máximo IdConsec. Antes: comprobar FKs hacia `producto`.
      *(Hecho: 5 filas, 00753/01874/02033/02102/02207 con IdConsec 1..5, sin huérfanos.)*
- [x] 2. BD `RITRAMA2025-TEST`: misma estructura (IdConsec poblado con ROW_NUMBER,
      drop de codigo_ritrama, semillero PROD al máximo IdConsec).
      *(Hecho: 396 productos con IdConsec 1..396, par1 = 396.)*
- [x] 3. Fila de inventario: `MasterInic.part_number '0753' → '00753'`.
- [x] 4. Código: `Product.IdConsec`; quitar `Codigo_Ritrama` del modelo/mapper/queries
      (`R.cs`), validador, `ProductsService` (insert/update/uniqueness sobre `product_id`),
      importador (product_id = código, IdConsec = consecutivo), `ProductoExportado`,
      formulario `FrmProductos` (Consecutivo = IdConsec, Código Ritrama = Product_ID).
      *Decisión añadida: el código es editable y, al cambiarlo en edición, el servicio lo
      propaga en la misma transacción a todas las tablas que lo referencian (descubiertas
      en runtime por `INFORMATION_SCHEMA`: MasterInic.part_number, orden_corte,
      rolls_details, pedido_detalle, item_despacho, ItemsMateria, rcdespacho,
      orden_compra_detalle, RollsInic), validando `CHARACTER_MAXIMUM_LENGTH` para no
      truncar. La fila se localiza por `IdConsec`; máximo del código = 25 caracteres.*
- [x] 5. Tests: adaptar los que asumen Product_ID consecutivo; suite completa.
      *(Adaptados `FrmProductosLayoutTests` y `FrmProductosFiltroCategoriaTests`;
      nuevos `ProductValidatorTests`, `ProductMapperTests` y caso de importación.)*
- [x] 6. Verificación: build 0/0, `dotnet test` todo en verde, y consulta SQL confirmando
      que el master `12504170` aparece en el buscador (JOIN con `producto`).
      *(Build 0 errores / 0 advertencias; 653/653 tests; JOIN devuelve
      `roll_id 12504170 → part_number '00753'`.)*

## Archivos clave
- `Models/Product.cs`, `Services/ProductsService/{ProductValidator,ProductMapper,ProductsService,ProductsImportService}.cs`
- `R.cs` (`SELECT_QUERY_PRODUCTS`, `INSERT_PRODUCT`, `UPDATE_PRODUCT`, `SELECT_PRODUCT_EXISTS*`)
- `Forms/FrmProductos.cs`, `Models/ProductoExportado.cs`
- `Scripts/` (script de migración nuevo para ambas BDs)
